using System;
using System.Reflection;
using ChatterChirper.Models;
using ChatterChirper.Utils;
using ColossalFramework;

namespace ChatterChirper.Systems
{
    public static class CitySnapshot
    {
        private static readonly object Sync = new object();
        private static CityContext _current = new CityContext();
        private static bool _updateScheduled;
        private static uint _lastUpdateFrame;

        // Update at most every 256 simulation frames
        private const uint UpdateInterval = 256u;

        public static CityContext GetContext()
        {
            ScheduleUpdateIfNeeded();
            lock (Sync)
            {
                return _current;
            }
        }

        private static void ScheduleUpdateIfNeeded()
        {
            try
            {
                if (!Singleton<SimulationManager>.exists) return;
                var sim = SimulationManager.instance;
                uint frame = sim.m_currentFrameIndex;

                if (frame - _lastUpdateFrame < UpdateInterval) return;
                if (_updateScheduled) return;

                _updateScheduled = true;
                sim.AddAction(UpdateNow);
            }
            catch (Exception ex)
            {
                ModLogger.Warning("CitySnapshot schedule failed: " + ex.Message);
            }
        }

        private static void UpdateNow()
        {
            try
            {
                var next = new CityContext();

                // Time of day
                if (Singleton<SimulationManager>.exists)
                {
                    var sim = SimulationManager.instance;
                    next.TimeOfDay = (float)sim.m_currentGameTime.TimeOfDay.TotalHours;
                    next.IsNight = next.TimeOfDay < 6f || next.TimeOfDay > 20f;
                    _lastUpdateFrame = sim.m_currentFrameIndex;
                }

                // Economy (money) via reflection to avoid API mismatches
                if (Singleton<EconomyManager>.exists)
                {
                    var econ = Singleton<EconomyManager>.instance;
                    next.Money = ReadLongField(econ, "m_cash", null, next.Money);
                }

                // District 0 (city-wide stats)
                TryFillFromDistrict(next);

                // Traffic flow (fallback with smoothed simulation)
                if (next.TrafficFlow <= 0f)
                {
                    // keep a reasonable range if not available
                    next.TrafficFlow = 60f;
                }

                // Apply smoothing to reduce spikes and align with UI behavior
                lock (Sync)
                {
                    _current.TrafficFlow = Smooth(_current.TrafficFlow, next.TrafficFlow, 0.15f);
                    _current.Happiness = (int)Smooth(_current.Happiness, next.Happiness, 0.15f);
                    _current.Unemployment = (int)Smooth(_current.Unemployment, next.Unemployment, 0.15f);
                    _current.TaxRateResidential = next.TaxRateResidential;
                    _current.Population = (int)Smooth(_current.Population, next.Population, 0.15f);
                    _current.CrimeRate = (int)Smooth(_current.CrimeRate, next.CrimeRate, 0.15f);
                    _current.FireHazard = (int)Smooth(_current.FireHazard, next.FireHazard, 0.15f);
                    _current.HealthAvg = (int)Smooth(_current.HealthAvg, next.HealthAvg, 0.15f);
                    _current.EducationAvg = (int)Smooth(_current.EducationAvg, next.EducationAvg, 0.15f);
                    _current.ElectricityAvailability = (int)Smooth(_current.ElectricityAvailability, next.ElectricityAvailability, 0.15f);
                    _current.WaterAvailability = (int)Smooth(_current.WaterAvailability, next.WaterAvailability, 0.15f);
                    _current.GarbageStatus = (int)Smooth(_current.GarbageStatus, next.GarbageStatus, 0.15f);
                    _current.TimeOfDay = next.TimeOfDay;
                    _current.IsNight = next.IsNight;
                    _current.IsDisasterActive = next.IsDisasterActive;
                    _current.Money = next.Money;
                }
            }
            catch (Exception ex)
            {
                ModLogger.Warning("CitySnapshot update failed: " + ex.Message);
            }
            finally
            {
                _updateScheduled = false;
            }
        }

        private static void TryFillFromDistrict(CityContext next)
        {
            try
            {
                if (!Singleton<DistrictManager>.exists) return;

                var manager = Singleton<DistrictManager>.instance;
                var districtsField = typeof(DistrictManager).GetField("m_districts", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (districtsField == null) return;

                var districts = districtsField.GetValue(manager);
                if (districts == null) return;

                var bufferField = districts.GetType().GetField("m_buffer", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (bufferField == null) return;

                var buffer = bufferField.GetValue(districts) as Array;
                if (buffer == null || buffer.Length == 0) return;

                var city = buffer.GetValue(0);
                if (city == null) return;

                next.Population = ReadIntField(city, "m_populationData", "m_finalCount", next.Population);
                next.Happiness = ReadIntField(city, "m_happiness", null, next.Happiness);
                next.CrimeRate = ReadIntField(city, "m_crimeRate", null, next.CrimeRate);
                next.FireHazard = ReadIntField(city, "m_fireHazard", null, next.FireHazard);
                next.HealthAvg = ReadIntField(city, "m_healthData", "m_averageHealth", next.HealthAvg);
                next.EducationAvg = ReadIntField(city, "m_educationData", "m_averageEducation", next.EducationAvg);
                next.GarbageStatus = ReadIntField(city, "m_garbage", null, next.GarbageStatus);

                // Disaster state (approximate): use reflection to avoid API mismatches across game versions
                next.IsDisasterActive = ReadDisasterActive();
            }
            catch (Exception ex)
            {
                ModLogger.Warning("CitySnapshot district read failed: " + ex.Message);
            }
        }

        private static bool ReadDisasterActive()
        {
            try
            {
                if (!Singleton<DisasterManager>.exists) return false;
                var manager = Singleton<DisasterManager>.instance;
                var t = manager.GetType();

                // Try property first
                var prop = t.GetProperty("IsDisasterRunning", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (prop != null)
                {
                    var val = prop.GetValue(manager, null);
                    if (val is bool) return (bool)val;
                }

                // Try method next
                var method = t.GetMethod("IsDisasterRunning", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (method != null)
                {
                    var val = method.Invoke(manager, null);
                    if (val is bool) return (bool)val;
                }
            }
            catch { }

            return false;
        }

        private static long ReadLongField(object owner, string fieldName, string subFieldName, long fallback)
        {
            try
            {
                var f = owner.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (f == null) return fallback;
                var v = f.GetValue(owner);
                if (v == null) return fallback;

                if (!string.IsNullOrEmpty(subFieldName))
                {
                    var sub = v.GetType().GetField(subFieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    if (sub == null) return fallback;
                    var subVal = sub.GetValue(v);
                    if (subVal == null) return fallback;
                    return Convert.ToInt64(subVal);
                }

                return Convert.ToInt64(v);
            }
            catch
            {
                return fallback;
            }
        }

        private static int ReadIntField(object owner, string fieldName, string subFieldName, int fallback)
        {
            try
            {
                var f = owner.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (f == null) return fallback;
                var v = f.GetValue(owner);
                if (v == null) return fallback;

                if (!string.IsNullOrEmpty(subFieldName))
                {
                    var sub = v.GetType().GetField(subFieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    if (sub == null) return fallback;
                    var subVal = sub.GetValue(v);
                    if (subVal == null) return fallback;
                    return Convert.ToInt32(subVal);
                }

                return Convert.ToInt32(v);
            }
            catch
            {
                return fallback;
            }
        }

        private static float Smooth(float oldValue, float newValue, float factor)
        {
            return oldValue * (1f - factor) + newValue * factor;
        }
    }
}
using ICities;

namespace ChatterChirper.Models
{
    /// <summary>
    /// Wraps an original IChirperMessage to override its text while preserving other properties.
    /// This is necessary because the default CitizenMessage implementation in the game is often immutable
    /// or calculated dynamically from localization IDs.
    /// </summary>
    public class ChirperMessageProxy : IChirperMessage
    {
        private readonly IChirperMessage _original;
        private readonly string _overriddenText;

        public ChirperMessageProxy(IChirperMessage original, string newText)
        {
            _original = original;
            _overriddenText = newText;
        }

        public uint senderID
        {
            get { return _original.senderID; }
        }

        public string senderName
        {
            get { return _original.senderName; }
        }

        public string text
        {
            get { return _overriddenText; }
        }
    }
}

# Data Model: Message Library

## Core Concepts

The system relies on a "Message Library" loaded from JSON. This defines the content available for substitution.

## Entity: Message Definition

Represents a single possible Chirper message or a template group.

| Field | Type | Description |
| :--- | :--- | :--- |
| `id` | string | Unique identifier (e.g., `traffic_high_01`). |
| `category` | enum | `traffic`, `housing`, `tax`, `job`, `service`, `disaster`, `praise`, `flavor`. |
| `severity` | int | 1-5 level. 1 = Minor, 5 = Critical. |
| `conditions` | object | Key-value pairs of thresholds required. |
| `weight` | float | Probability multiplier (default 1.0). |
| `cooldown` | int | Seconds before this specific message can appear again. |
| `texts` | string[] | Array of possible localized strings. |

### JSON Representation

```json
{
  "id": "traffic_jam_downtown",
  "category": "traffic",
  "severity": 4,
  "conditions": {
    "traffic_flow_max": 40,
    "happiness_min": 0
  },
  "weight": 2.0,
  "cooldown": 1200,
  "texts": [
    "Traffic is a nightmare today!",
    "I've been in this car for 3 hours. #traffic #fail"
  ]
}
```

## Entity: City Context

Normalized internal state snapshot used for logic evaluation.

| Field | Type | Description |
| :--- | :--- | :--- |
| `TrafficFlow` | float | 0.0 to 100.0 (Global). |
| `GlobalHappiness` | float | 0.0 to 100.0. |
| `Unemployment` | float | 0.0 to 100.0 (Percentage). |
| `TaxRateResidential`| float | Percentage range. |
| `IsDisasterActive` | bool | True if any disaster is running. |

## Entity: Mod Configuration

Persisted user settings.

| Field | Type | Default | Description |
| :--- | :--- | :--- | :--- |
| `Enabled` | bool | true | Master switch. |
| `Toxicity` | float | 1.0 | Multiplier for "Mean" message selection (if implemented). |
| `ReplaceChance` | float | 1.0 | 0.0 = Vanilla, 1.0 = All Custom. |

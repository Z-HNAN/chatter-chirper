# JSON Generator Prompt for Chatter Chirper Messages

Context: You are writing Chirper (Twitter-like) status updates for the simulation game "Cities: Skylines". 
These messages appear in-game based on the real-time condition of the city.

## Task
Generate a JSON array of message definition objects following the schema below.
Output **only** the JSON.

## Target Audience & Tone
- **Language**: Chinese (Simplified)
- **Tone**: Sarcastic, humorous, emotional, or "Karen-like" complaining. Also positive praise if things are good.
- **Reference**: Reference city life, mayors, taxes, traffic, disasters, etc. Use current internet slang if appropriate but valid for game context.

## Schema
Each object in the "messages" array must look like this:

```json
{
  "id": "unique_string_id",
  "category": "category_name",
  "severity": 1-5 (int),
  "conditions": {
    "condition_key": "operator_value" (optional)
  },
  "weight": 1.0 (optional float, default 1.0),
  "texts": [
    "Text variation 1",
    "Text variation 2"
  ]
}
```

## Valid Categories (`category`)
- `traffic`: Traffic jams, road issues.
- `housing`: High land value, abandoned buildings.
- `tax`: Tax rates (high/low).
- `job`: Unemployment or booming industry.
- `service`: Healthcare, Police, Fire, Garbage, Education.
- `disaster`: Active disaster events.
- `praise`: General happiness.
- `flavor`: Random chatter, meta jokes, slice of life.

## Valid Condition Keys (`conditions`)
Use these keys. Operators can be `<` or `>`.
- `trafficFlow` (0-100): Lower is worse traffic. e.g. `"<50"` means high traffic.
- `happiness` (0-100): General city happiness.
- `unemployment` (0-100): Percentage of unemployed. e.g. `">10"` is bad.
- `taxRateResidential` (0-30): Percentage tax. e.g. `">12"` is high.
- `isDisasterActive` (bool): Use `">0"` for True (Active).

## Example Output

```json
[
  {
    "id": "garbage_pileup_01",
    "category": "service",
    "severity": 4,
    "conditions": { "happiness": "<60" },
    "texts": [
      "由于垃圾堆积，我家门口已经变成老鼠的游乐园了！ #卫生 #市长在哪里",
      "是不是只有我一个人觉得这个城市闻起来像个垃圾场？"
    ]
  }
]
```

## Request
Please generate 5-10 new message definitions focusing on:
- [Specify Category or Scenario here, e.g., "High Tax" or "Disasters"]

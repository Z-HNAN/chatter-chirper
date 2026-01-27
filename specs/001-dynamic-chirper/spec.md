# Feature Specification: Context-aware Dynamic Chirper Messages

**Feature Branch**: `001-dynamic-chirper`
**Created**: 2026-01-27
**Status**: Draft
**Input**: User description: "Context-aware Dynamic Chirper Messages for Cities: Skylines"

## User Scenarios & Testing *(mandatory)*

<!--
  IMPORTANT: User stories should be PRIORITIZED as user journeys ordered by importance.
  Each user story/journey must be INDEPENDENTLY TESTABLE - meaning if you implement just ONE of them,
  you should still have a viable MVP (Minimum Viable Product) that delivers value.
-->

### User Story 1 - Context-Aware Message Feed (Priority: P1)

As a Player, I want Chirper messages to reflect the actual problems and successes of my city (like traffic jams or high crime), so that the social feed feels alive and relevant rather than random.

**Why this priority**: This is the core value proposition of the mod—replacing generic messages with context-aware ones.

**Independent Test**: Can be tested by artificially inducing a specific city state (e.g., stopping all traffic) and observing if the next Chirper message references the traffic jam.

**Acceptance Scenarios**:

1. **Given** a city with average traffic flow below 50% (congestion), **When** a Chirper message is triggered, **Then** a message from the "traffic" category complaining about congestion is displayed.
2. **Given** a city with very high residential happiness (>90%), **When** a Chirper message is triggered, **Then** a message from the "praise" category reflecting citizen satisfaction is displayed.
3. **Given** a city with no major issues, **When** a Chirper message is triggered, **Then** a "flavor" or "meta" message is displayed to keep the feed active.

---

### User Story 2 - Mod Configuration & Customization (Priority: P2)

As a Mod User, I want to be able to toggle the mod on/off and adjust the "toxicity" (sarcasm level) of the messages, so that I can tailor the experience to my preferred difficulty and tone.

**Why this priority**: Essential for user retention; some players may find constant complaints annoying or want a purely vanilla experience temporarily without uninstalling.

**Independent Test**: Can be tested by opening the mod options panel, changing settings, and verifying behavior change in-game.

**Acceptance Scenarios**:

1. **Given** the mod is set to "Disabled", **When** playing the game, **Then** only original vanilla Chirper messages appear.
2. **Given** "Toxicity" is set to "High", **When** a negative event occurs (e.g., high taxes), **Then** the generated message uses more aggressive/sarcastic language.
3. **Given** "Operation Mode" is set to "Mixed", **When** messages are generated over time, **Then** a mix of original vanilla messages and new dynamic messages appear (based on probability setting).

---

### User Story 3 - Message Variety & Repetition Control (Priority: P3)

As a Player, I want to see a variety of messages for the same topic and not see the exact same text repeated back-to-back, so that the illusion of a living city is maintained.

**Why this priority**: Improves long-term engagement and prevents the mod from feeling mechanical.

**Independent Test**: Trigger multiple messages in rapid succession for the same condition (e.g., consistently high taxes) and verify unique texts.

**Acceptance Scenarios**:

1. **Given** a traffic alert was just displayed, **When** another traffic alert is triggered immediately after, **Then** the text must be different from the previous one.
2. **Given** a limited pool of messages, **When** messages are selected, **Then** weighted randomization is used to favor less recently seen messages.

### Edge Cases

- **Zero Population**: If the city has 0 residents, the mod should disable dynamic messages and revert to vanilla or silence to avoid illogical specific complaints.
- **Missing Library**: If the external message database file is missing or corrupted, the mod must gracefully degrade to "Disabled" mode and log an error, ensuring the game does not crash.
- **Simultaneous Events**: If multiple high-priority events occur simultaneously (e.g., Disaster + Major Traffic Jam), the system should prioritize the Disaster message.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: System MUST be able to intercept or coexist with the original Chirper message trigger mechanism to inject custom content.
- **FR-002**: System MUST evaluate game state conditions including: Traffic Flow, District Happiness, Unemployment Rate, Tax Rate, Service Coverage, and Disaster Status.
- **FR-003**: System MUST include a predefined, offline "Message Library" categorized by topic (Traffic, Housing, Tax, Job, Service, Disaster, Praise, Meta).
- **FR-004**: System MUST select messages from the library that match the current highest-priority city conditions (e.g., Disaster > Traffic > General).
- **FR-005**: System MUST support configurable parameters: Enable/Disable Mod, Operation Mode (Replace/Mixed/Off), and Toxicity/Intensity Level.
- **FR-006**: System MUST ensure messages do not repeat consecutively (cooldown/history check).
- **FR-007**: System MUST operate completely offline with no external API dependency or data transmission.
- **FR-008**: System MUST preserve the original visual style and trigger timing of the Chirper UI (no UI layout changes).

### Key Entities *(include if feature involves data)*

- **Message Definition**: Data structure containing the message text, category (e.g., 'Traffic'), intensity level (1-5), and required condition thresholds.
- **City Context**: Object representing the gathered real-time statistics of the city simulation.
- **Configuration Profile**: User settings controlling the behavior of the mod.

### Assumptions & Dependencies

- **Dependency**: The mod depends on the *Cities: Skylines* modding API (`ICities`) being available and unchanged in relevant message triggering hooks.
- **Assumption**: The game provides accessible API endpoints to read real-time statistics like traffic flow and happiness without significant performance penalty.
- **Assumption**: Users accept that enabling the mod disables Steam Achievements (standard for C:S mods).

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: Mod loads and runs without throwing exceptions in the game's debug log during a standard 30-minute play session.
- **SC-002**: In "Fully Replace" mode, 100% of displayed Chirper messages are from the custom library (excluding tutorial/scripted event messages if unmodifiable).
- **SC-003**: User can change the "Toxic" setting and observe a change in message sentiment within the next 3 generated messages for a negative condition.
- **SC-004**: The mod file size remains reasonable (e.g., under 50MB) despite containing the message database, ensuring quick Workshop downloads.

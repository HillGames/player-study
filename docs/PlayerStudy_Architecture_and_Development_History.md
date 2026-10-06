# Player Study — Architecture and Development History

This document records the architecture, design discussions, decisions, rejected approaches, implementation milestones, problems, test results, and reasoning behind the Player Study project.

It is intended to live under `doc/` in the repository. It is a history/design document, so source code is intentionally not embedded here. Implementation details belong in the authoritative source files listed below.

# 1. Engineering Principle

The project's golden rule is:

> Every component must be scalable, extensible, reusable, and independently testable.

The project is being built incrementally. Abstractions should be introduced when requirements justify them rather than for speculative future use.

# 2. Project and Solution

The project is a Godot 4.8 development project using C#/.NET 8.

Solution structure:

```text
PlayerStudy.sln
├── PlayerStudy.csproj
├── PlayerStudy.Core/
│   └── PlayerStudy.Core.csproj
└── PlayerStudy.Test/
    └── PlayerStudy.Test.csproj
```

Dependency direction:

```text
                  PlayerStudy.Core
                   /                             /                              ↓               ↓
        PlayerStudy          PlayerStudy.Test
           Godot                NUnit
```

`PlayerStudy.Core` must not depend on Godot. The Godot project integrates Core with the engine, while the test project validates Core behavior independently.

# 3. Initial Player Prototype and Architectural Direction

The original player prototype directly read Godot input, calculated movement, assigned `CharacterBody2D` velocity, and called `MoveAndSlide()`.

That prototype successfully proved basic movement but mixed input, gameplay decisions, and physical execution.

The final direction separates these responsibilities:

```text
Godot Input
    ↓
InputAdapter
    ↓
MoveCommand
    ↓
MessageQueue
    ↓
PlayerController
    ├── PlayerMovement
    └── StateMachine
    ↓
Velocity / Player State
    ↓
Godot Player
    ↓
CharacterBody2D / MoveAndSlide()
```

The Godot Player is the physical/integration representation. The Core PlayerController is gameplay logic and is not a Godot Node.

# 4. Core Result, Error, and Logging Infrastructure

Core infrastructure was established before player-specific implementation.

Authoritative files:

- `PlayerStudy.Core/Common/ErrorCode.cs`
- `PlayerStudy.Core/Common/Error.cs`
- `PlayerStudy.Core/Common/Result.cs`
- `PlayerStudy.Core/Logging/LogLevel.cs`
- `PlayerStudy.Core/Logging/ILogger.cs`

The Result/Error model provides explicit success/failure reporting. State transitions use these results for normal operational failures, while unexpected exceptions are captured and converted into appropriate errors.

Core logging uses an abstraction rather than Godot logging APIs. Dynamic/static logging configuration remains a future concern and was intentionally not expanded prematurely.

# 5. Generic State Machine

The generic state machine was implemented before player states.

Authoritative files:

- `PlayerStudy.Core/StateMachine/IState.cs`
- `PlayerStudy.Core/StateMachine/IStateMachine.cs`
- `PlayerStudy.Core/StateMachine/StateMachine.cs`

The generic state contract contains Enter and Exit operations. The state machine manages the current state and transitions.

It deliberately knows nothing about Player, Godot, movement, animation, or player-specific events.

# 6. State Machine Rules and Testing

The established transition rules are:

- First state: enter first, then make it current.
- Normal transition: exit old state, enter new state, then make new state current.
- Same state: no exit/enter; succeeds.
- Exit failure: new state is not entered and old state remains current.
- Enter failure: transition fails and the previous state is re-entered.
- Exceptions from Enter/Exit are captured and converted into errors.
- Restoration failure is logged critically; no speculative Faulted state was introduced.

StateMachine tests cover initial state, transitions, same-state behavior, ordering, failures, exceptions, restoration, null input, and logging.

Result: **17/17 StateMachine tests passed.**

# 7. Messaging Architecture

Messaging was added as the next Core foundation.

Authoritative files:

- `PlayerStudy.Core/Messaging/IMessage.cs`
- `PlayerStudy.Core/Messaging/ICommand.cs`
- `PlayerStudy.Core/Messaging/IEvent.cs`
- `PlayerStudy.Core/Messaging/MessageQueue.cs`

Definitions:

- Command = intent/request: “Please do this.”
- Event = fact/notification: “This happened.”
- Message = common transport abstraction.

The queue transports messages but does not interpret them.

The first version intentionally does not add artificial capacity, threading, asynchronous processing, priorities, timestamps, correlation IDs, cancellation, serialization, or networking.

# 8. Messaging Tests

Messaging tests are located in:

- `PlayerStudy.Test/Messaging/MessageContractTest.cs`
- `PlayerStudy.Test/Messaging/MessageQueueTest.cs`

Coverage includes command/event/message relationships, FIFO behavior, null rejection, queue state, concrete message preservation, and multiple operations.

Results:

- 8 contract tests passed.
- 18 queue tests passed.
- 26 messaging tests passed in total.

# 9. Player State Decision: Use IState

A major design question was whether Player state should use an enum or the existing `IState` abstraction.

An enum-based player state representation was considered and rejected.

The project already has a generic `StateMachine<TState>` constrained to `IState`. Therefore the player uses concrete `IState` implementations:

- `PlayerIdleState`
- `PlayerMovingState`

The generic StateMachine is reused without modification.

This avoids a parallel state-machine concept.

# 10. Player Movement Architecture

Momentum-based movement changed the movement architecture.

The desired behavior is not:

```text
Input → Velocity
```

but:

```text
Input
  ↓
Target Velocity
  ↓
Acceleration / Deceleration
  ↓
Current Velocity
```

Movement is therefore a separate Core component rather than being embedded in PlayerController or individual states.

The movement model owns acceleration, deceleration, target speed, and current velocity. Godot later applies the resulting velocity physically.

# 11. Analog Controller Input Decision

Digital-only direction values were rejected because controller input must preserve analog magnitude.

Movement input is represented by a two-dimensional vector whose components normally range from -1 to +1.

Examples include:

- `(0, 0)` no input.
- `(1, 0)` full right.
- `(-1, 0)` full left.
- `(0, -1)` full up.
- `(0.5, 0)` half-strength right.
- `(0.5, -0.5)` partial diagonal input.

The input vector therefore represents both direction and input strength.

This same command interface can later support keyboard, controllers, AI, replay, and other input sources.

# 12. Core Vector2 Decision

The discussion initially considered separate X/Y floating-point values.

The final decision is to use a Core-owned vector type.

Godot's `Vector2` is not used inside Core because that would introduce a Core → Godot dependency.

Authoritative files:

- `PlayerStudy.Core/Common/Math/Vector2.cs`
- `PlayerStudy.Test/Common/Math/Vector2Test.cs`

The Core vector currently supports the minimum operations required for movement, including coordinates, zero, length, normalization, arithmetic, scalar operations, and equality.

Vector tests cover these operations and currently pass.

# 13. Movement Settings

Movement settings remain scalar values because they describe magnitudes rather than directions.

Authoritative file:

`PlayerStudy.Core/Player/Movement/PlayerMovementSettings.cs`

Current settings:

- MaxSpeed.
- Acceleration.
- Deceleration.

The intended separation is:

```text
Input       → Vector2
Velocity    → Vector2
MaxSpeed    → float
Acceleration→ float
Deceleration→ float
```

YAML, JSON, and binary configuration were considered. The decision for the current phase is to keep strongly typed C# settings in Core.

A future Godot Resource or data-driven configuration layer may supply these values without changing the Core movement model.

# 14. PlayerMovement

Authoritative file:

`PlayerStudy.Core/Player/Movement/PlayerMovement.cs`

Responsibilities:

- Receive Core Vector2 input.
- Accept elapsed time.
- Convert input magnitude into target speed.
- Produce target velocity.
- Accelerate toward target velocity.
- Decelerate toward zero when input is absent.
- Prevent excessive input magnitude.
- Prevent resulting velocity from exceeding MaxSpeed.

The movement component has no Godot dependency.

# 15. Movement Semantics

The system preserves analog magnitude.

For example, with MaxSpeed 200 and input `(0.5, 0)`, target speed is 100 rather than 200.

The current velocity approaches that target through acceleration.

When input becomes zero, target velocity becomes zero and deceleration brings current velocity toward zero.

Diagonal input is treated as a vector so that full diagonal input cannot unintentionally create a speed greater than MaxSpeed.

# 16. Movement Tests

Authoritative file:

`PlayerStudy.Test/Player/Movement/PlayerMovementTest.cs`

The tests cover:

- Initial zero velocity.
- Acceleration.
- Reaching maximum speed.
- Partial analog input.
- Deceleration.
- Stopping.
- Negative movement.
- Vertical movement.
- Diagonal movement.
- Input magnitude limits.
- Partial analog magnitude preservation.
- Direction preservation.
- Negative delta-time rejection.

Result: **13/13 PlayerMovement tests passed.**

# 17. MoveCommand

Authoritative file:

`PlayerStudy.Core/Player/Messages/MoveCommand.cs`

MoveCommand implements the existing `ICommand` abstraction and carries a Core Vector2 representing movement input.

The command represents intent, not movement simulation.

It does not calculate velocity, acceleration, deceleration, or physical movement.

The decision was also made not to clamp values inside MoveCommand. Input constraints belong to PlayerMovement, while MoveCommand transports the input it receives.

# 18. MoveCommand Tests

Authoritative file:

`PlayerStudy.Test/Player/Messages/MoveCommandTest.cs`

The tests verify command/message contracts, zero input, full input, analog input, and preservation of the supplied vector.

The command intentionally does not modify the supplied input.

These tests are included in the current full-suite result.

# 19. Player Idle and Moving States

Authoritative files:

- `PlayerStudy.Core/Player/States/PlayerIdleState.cs`
- `PlayerStudy.Core/Player/States/PlayerMovingState.cs`

Both implement the existing `IState` contract.

For the first movement slice, Enter and Exit are intentionally minimal and successful.

The states represent gameplay state. They do not read input, calculate velocity, call Godot, or perform physics.

The separation is:

```text
PlayerIdleState
    → represents Idle

PlayerMovingState
    → represents Moving

PlayerMovement
    → calculates velocity

PlayerController
    → coordinates the components

StateMachine
    → performs state transitions
```

# 20. Player State Tests

Authoritative file:

`PlayerStudy.Test/Player/States/PlayerStateTest.cs`

The tests verify:

- Idle implements IState.
- Moving implements IState.
- Idle enters successfully.
- Idle exits successfully.
- Moving enters successfully.
- Moving exits successfully.

Result: **6/6 player state tests passed.**

# 21. Current Full Test Milestone

The latest full test run reported:

```text
Total tests: 77
Passed: 77
Failed: 0
Skipped: 0
Build: succeeded
```

This includes StateMachine, messaging, Vector2, movement, MoveCommand, and player state coverage.

This milestone confirms that the new movement foundation did not break the earlier Core architecture.

# 22. PlayerController — Next Component

The next major component is:

`PlayerStudy.Core/Player/PlayerController.cs`

It will coordinate the components already built.

Responsibilities are expected to include:

- Own the player movement model.
- Own the Player Idle and Moving state instances.
- Own the generic state machine.
- Process MoveCommand messages.
- Pass movement input to PlayerMovement.
- Change Idle/Moving based on movement intent.
- Expose the resulting Core velocity.
- Later publish a player-specific state-change event.

It remains a pure Core class and must not depend on Godot.

# 23. Intended PlayerController Flow

The target Core flow is:

```text
MoveCommand
     ↓
MessageQueue
     ↓
PlayerController
     ├── PlayerMovement
     │       ↓
     │    Velocity
     │
     └── StateMachine<IState>
             ↓
        Idle / Moving
```

For the first slice, the intended state semantics are:

- Non-zero movement input → Moving.
- Zero movement input → Idle.

Momentum is handled independently by PlayerMovement.

The exact relationship between residual velocity and state semantics should be verified through PlayerController tests rather than assumed.

# 24. Player State Change Event

A future player-specific event is expected:

`PlayerStateChangedEvent`

It is a notification, not a movement command.

The conceptual distinction is:

```text
Command → “Please do this.”
State   → “I am doing this.”
Event   → “This happened.”
```

The generic StateMachine must remain generic. PlayerController will own player-specific coordination and can publish a player-specific event after a successful state change.

The exact event payload remains a design decision for the PlayerController implementation. Avoiding unnecessary coupling between event consumers and concrete state classes is an explicit concern.

# 25. Godot Integration Boundary

Godot integration comes after the Core PlayerController slice is stable.

The intended boundary is:

```text
Godot Keyboard / Controller
          ↓
     InputAdapter
          ↓
      MoveCommand
          ↓
      MessageQueue
          ↓
    PlayerController
          ↓
    PlayerMovement
          ↓
       Velocity
          ↓
      Godot Player
          ↓
 CharacterBody2D
          ↓
     MoveAndSlide()
```

Core determines gameplay movement behavior. Godot executes physical movement and collision.

# 26. Input Adapter

The future InputAdapter will be Godot-specific.

Its only responsibility is to translate Godot device input into Core MoveCommand input.

Keyboard and controller input can therefore share the same Core command path.

The adapter does not calculate acceleration or velocity.

# 27. Future Player Brainstorm

Future capabilities discussed, but intentionally not part of the first vertical slice, include:

Movement:
- Walk.
- Run.
- Strafe.
- Movement progression.

Equipment:
- Flashlight.
- Knife.
- Handgun.
- Rifle.
- Shotgun.

Actions:
- Shooting.
- Flashlight toggle.
- Knife throw.
- Melee.

Progression:
- XP.
- Weapon unlocking.
- Movement speed progression.
- Bullet damage progression.

Gameplay:
- Stealth.
- Flashlight distraction.
- Rear attacks.
- Knife combat.
- Long-range combat.

These remain brainstorming requirements only.

# 28. Architectural Insight From Future Features

The future brainstorm demonstrated why one giant Player state machine would become problematic.

Locomotion, equipment, actions, and progression are separate capability dimensions.

The project should therefore avoid a combinatorial state machine containing every combination of movement, weapon, action, and progression state.

The current slice intentionally does not introduce those future abstractions.

# 29. Problems and Corrections

### State representation

A player state enum was considered and rejected. Existing IState-based states are the correct fit.

### Missing movement data

It was noticed that a parameterless movement command could not communicate direction. MoveCommand therefore carries movement input.

### Digital-only input

Digital 0/1 movement was insufficient for analog controllers. The command now carries a signed two-dimensional input vector.

### Godot Vector2 in Core

Using Godot.Vector2 in Core would violate dependency direction. A Core Vector2 was introduced.

### Separate X/Y movement values

Separate velocity/input floats were reconsidered. Core Vector2 provides a cleaner mathematical representation.

### External movement configuration

YAML/binary configuration was considered but deferred. Strongly typed Core settings are sufficient for this phase.

### Velocity responsibility

The final decision is that PlayerMovement calculates gameplay velocity, while Godot physically applies it.

# 30. Roadmap

```text
PHASE 0 — Architecture Foundation
    Core project
    Result / Error
    Logging
    StateMachine
        ↓
PHASE 1 — Core Messaging
    Message
    Command
    Event
    MessageQueue
        ↓
PHASE 2 — Player Core Movement Slice
    Core Vector2
    PlayerMovementSettings
    PlayerMovement
    MoveCommand
    PlayerIdleState
    PlayerMovingState
    PlayerController
    Player State Changed Event
        ↓
PHASE 3 — Godot Integration
    Godot Player Node
    Input Adapter
    PlayerController integration
    Movement → CharacterBody2D
    Core ↔ Godot boundary
        ↓
PHASE 4 — Player Test Scene
    Spawn player
    WASD
    Controller input
    Idle / Moving
    Momentum movement
    Collision
    Animation
    Debug information
        ↓
PHASE 5 — Player Validation
    Unit tests
    Integration tests
    Godot runtime tests
    Edge cases
        ↓
PLAYER VERTICAL SLICE COMPLETE
        ↓
ONLY THEN:
World
Enemy
Combat
Objectives
Progression
Additional player capabilities
```

# 31. Authoritative Project File References

| Area | File |
|---|---|
| Errors | `PlayerStudy.Core/Common/ErrorCode.cs` |
| Errors | `PlayerStudy.Core/Common/Error.cs` |
| Results | `PlayerStudy.Core/Common/Result.cs` |
| Logging | `PlayerStudy.Core/Logging/LogLevel.cs` |
| Logging | `PlayerStudy.Core/Logging/ILogger.cs` |
| Core vector | `PlayerStudy.Core/Common/Math/Vector2.cs` |
| State contract | `PlayerStudy.Core/StateMachine/IState.cs` |
| State machine contract | `PlayerStudy.Core/StateMachine/IStateMachine.cs` |
| State machine | `PlayerStudy.Core/StateMachine/StateMachine.cs` |
| Messaging | `PlayerStudy.Core/Messaging/IMessage.cs` |
| Commands | `PlayerStudy.Core/Messaging/ICommand.cs` |
| Events | `PlayerStudy.Core/Messaging/IEvent.cs` |
| Message queue | `PlayerStudy.Core/Messaging/MessageQueue.cs` |
| Movement settings | `PlayerStudy.Core/Player/Movement/PlayerMovementSettings.cs` |
| Movement | `PlayerStudy.Core/Player/Movement/PlayerMovement.cs` |
| Move command | `PlayerStudy.Core/Player/Messages/MoveCommand.cs` |
| Idle state | `PlayerStudy.Core/Player/States/PlayerIdleState.cs` |
| Moving state | `PlayerStudy.Core/Player/States/PlayerMovingState.cs` |
| State tests | `PlayerStudy.Test/StateMachine/StateMachineTest.cs` |
| Message tests | `PlayerStudy.Test/Messaging/MessageContractTest.cs` |
| Queue tests | `PlayerStudy.Test/Messaging/MessageQueueTest.cs` |
| Vector tests | `PlayerStudy.Test/Common/Math/Vector2Test.cs` |
| Movement tests | `PlayerStudy.Test/Player/Movement/PlayerMovementTest.cs` |
| Move command tests | `PlayerStudy.Test/Player/Messages/MoveCommandTest.cs` |
| Player state tests | `PlayerStudy.Test/Player/States/PlayerStateTest.cs` |
| Documentation | `doc/` |

# 32. Current Implementation Status

| Component | Status |
|---|---|
| Core project | Complete |
| Result / Error | Complete |
| Logging abstractions | Complete |
| Generic StateMachine | Complete |
| StateMachine tests | 17 passing |
| Messaging contracts | Complete |
| MessageQueue | Complete |
| Messaging tests | 26 passing |
| Core Vector2 | Complete |
| Vector2 tests | 8 passing |
| PlayerMovementSettings | Complete |
| PlayerMovement | Complete |
| PlayerMovement tests | 13 passing |
| MoveCommand | Complete |
| MoveCommand tests | Passing |
| PlayerIdleState | Complete |
| PlayerMovingState | Complete |
| Player state tests | 6 passing |
| PlayerController | Next |
| PlayerStateChangedEvent | Next |
| Godot integration | Later |
| Godot player test scene | Later |

# 33. Development Method Going Forward

The project continues with a one-concept-at-a-time development loop:

```text
Discuss requirement
        ↓
Define responsibility
        ↓
Choose boundary
        ↓
Implement smallest useful component
        ↓
Write unit tests
        ↓
Run complete test suite
        ↓
Review architecture
        ↓
Proceed to next component
```

The objective is not maximum abstraction. The objective is a foundation that can grow without forcing unrelated systems to become coupled, while ensuring important Core components remain independently testable.
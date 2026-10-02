# Player character

## Abilities
- Idle
- Move
	- Walk 
	- Run
	- Strafe Left
	- Strafe Right
- Combat
	- Melee attack
	- Range attack
	- Reload

## State Machine

This decides the state of the Player character. Extends the state machine Interface. Player will hae 2 dedicated state machines
- Movement State
- Combat State
- Weapon State


## States

Logic and animation will be calcuated adn executed as per this states.

### Movement States

Which type of animation to be played and the movement logic to be calculated.

**1) Idle**
**2) Walk**
**3) Run**
**4) Strafe Left**
**5) Strafe Right**

### Combat States

Which type of attack to be done , melee or ranged attack animation and logic

**1) Melee**
**2) Shoot**
**3) Reload**

### Weapon State

According to the state set the visibility of the sprite will be changed.

**1) FlashLight**
**2) Knife**
**3) Hand Gun**
**4) Rifle**
**5) ShotGun**


## Godot Tree

```
Player (CharecterBody2D)
├── Visual (Node2D)
│   ├── Legs (Sprite2D)
│   ├── Body_flashlight (Sprite2D)
│   ├── Body_knife (Sprite2D)
│   ├── Body_handgun (Sprite2D)
│   ├── Body_rifle (Sprite2D)
│   ├── Body_shotgun (Sprite2D)
│   └── Animation (Node2D)
│       ├── AnimationPlayer (AnimationPlayer)
│       └── AnimationTree (AnimationTree)
├── Collossion (Node2D)
│   ├── Hit_box (CollisionBox2D)
│   └── Hurt_box (CollisionBox2D)
└── PlayerController (Node2D)


```


## Animation

Animation player has following animations
| State | Flash Light | Knife | Hand Gun | Rifle | ShotGun |
|-------|-------------|-------|----------|-------|---------|
| Idle | X | X | X | X | X |
| Idle Melee  | X | X | X | X | X |
| Idle Shoot  | O | O | X | X | X |
| Idle Reload | O | O | X | X | X |
| Walk | X | X | X | X | X |
| walk Melee | X | X | X | X | X |
| walk Shoot | O | O | X | X | X |
| Walk reload | O | O | X | X | X |
| Run  | X | X | X | X | X |
| Run Melee | X | X | X | X | X |
| Run Shoot | O | O | X | X | X |
| Run Reload | O | O | X | X | X |
| Strafe Left | X | X | X | X | X |
| Strafe Left Melee | X | X | X | X | X |
| Strafe Left Shoot | O | O | X | X | X |
| Strafe Left Reload | O | O | X | X | X |
| Strafe Right  | X | X | X | X | X |
| Strafe Right Melee | X | X | X | X | X |
| Strafe Right Shoot | O | O | X | X | X |
| Strafe Right Reload | O | O | X | X | X |


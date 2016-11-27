Advance Sniper Starter kit

This is the realistic sniper shooting system.


Include


- Sniper rifle M24 3d Model (.fbx) with animations (Shoot , Idle ,Bolt Action)
- Realistic long range shooting system (the bullet projectile like battlefield3)
- Bullet Camera Action when hit the target

- Basic FPS controller
- Basic Enemy

- Zombie Shooting Demo


How to setup

*Gun (Bolt action system)
- import your gun with animation label (Shoot , Idle , Bolt)
- add Gun.js component to the gun
- edit parameter
	Bullet -> Bullet prefab
	Shell -> Shell prefeb, this prefeb will spawn out after shoot the gun.
	Bullet Spawn -> point of bullet spawner
	Shell Spawn -> point of shell spawner
	Cameras -> list of cameras using for swith between normal view and sniper view
	Fire Rate -> 0 is very fast
	Sound Gun Fire -> Gun Sound
	Sound Bolt End
	Sound Bolt Start
	Force -> force for push the bullet out (bullet must be the riggid body object).

* Bullet (Riggid body)
- add Bullet.js component to your bullet model or prefab
- edit parameter
	Particle Hit -> the particle spawning at target
	Look Target Hit -> Enable/Disable Target camera action
	Owner -> owner of bullet , the bullet will ignore it
	Damage -> Damage of bullet

* Target
- (Importance) Every Target must have DamageManager.js component

***See the sample for more information.
if you have any question mail me :hwrstudio@gmail.com

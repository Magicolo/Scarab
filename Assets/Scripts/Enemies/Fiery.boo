import UnityEngine

class Fiery (Enemy): 

	public fireTrailDelay as single = 0.5

	fireTrailCounter as single

	override def Normal():
		super.Normal()
		
		if fireTrailCounter >= fireTrailDelay:
			fireTrailCounter = 0
			newFire as GameObject = hObjectPool.Instance.Spawn(References.Sprites.FireID, transform.position, Quaternion.identity)
			fire as Fire = newFire.GetComponent[of Fire]()
			fire.transform.localScale = transform.localScale * 3
			fire.damage = Damage / 3
			fire.lifeTime = 10
			fire.destroyOnRoomChange = true
			fire.hitEnemies = false
		else:
			fireTrailCounter += Time.deltaTime

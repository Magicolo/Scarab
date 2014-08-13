import UnityEngine

class Trapper (Enemy): 

	public growthFactor as single = 2
	
	override def ApplyDamage(amount as single):
		if CurrentState != DeathGrowth:
			super.ApplyDamage(amount)
			transform.localScale = currentScale + currentScale * (1 - Health / MaxHealth) * (growthFactor / 10)
			
	override def Kill():
		CurrentState = DeathGrowth
		
	def DeathGrowth():
		currentState = "DeathGrowth"
		transform.localScale *= 1 + Time.deltaTime / 2
		
		if transform.localScale.magnitude >= growthFactor * currentScale.magnitude:
			newExplosion as GameObject = hObjectPool.Instance.Spawn(References.Sprites.ExplosionID, transform.position, Quaternion.identity)
			explosion as Explosion = newExplosion.GetComponent[of Explosion]()
			explosion.owner = gameObject
			explosion.damage = Damage
			explosion.transform.localScale = transform.localScale * 3
			source as AudioSource = AudioPlayer.Play("SFX_Character_Explosion")
			source.pitch = 10 / explosion.transform.localScale.magnitude
			super.Kill()
		

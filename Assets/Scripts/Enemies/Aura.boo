import UnityEngine

class Aura (Enemy): 

	public auraBonus as single = 75
	public auraShineSpeed as single = 3
	public auraShineStrength as single = 0.5

	auraObject as GameObject
	auraCollider as CircleCollider2D
	isActive as bool

	override def Awake():
		super.Awake()
		
		auraObject = transform.FindChild("Aura").gameObject
		auraCollider = auraObject.GetComponent(CircleCollider2D)

	override def OnEnable():
		super.OnEnable()
		

	def OnDisable():
		auraCollider.enabled = false

	override def ApplyDamage(amount as single):
		invincible as bool
		
		if References.RoomManager.currentRoom != null:
			if References.RoomManager.currentRoom.enemies.Count > 1:
				for enemy as Enemy in References.RoomManager.currentRoom.enemies:
					if not enemy isa Aura:
						invincible = true
						break
		if invincible:
			spriteRenderer.color = Color.white
			SpawnInfoText(0.ToString(), gameObject, Color.white)
			AudioPlayer.Play("SFX_Character_Invincible")
		else:
			super.ApplyDamage(amount)
			
	override def Normal():
		super.Normal()
		
		if not auraCollider.enabled:
			auraCollider.enabled = true
		
	override def Move():
		if References.Player != null:
			closestEnemy as Enemy = GetClosestEnemy()
			if closestEnemy != null:
				distance as single = Vector3.Distance(transform.position, closestEnemy.transform.position)
				if distance < sightRadius and distance > minDistance:
					transform.rigidbody2D.MovePosition(transform.position + transform.TransformDirection(Vector2.right) * MoveSpeed * Time.deltaTime)
					transform.rigidbody2D.MoveRotation(transform.LookingAt2D(closestEnemy.transform, 0, Mathf.Max(MoveSpeed, 1)).eulerAngles.z)
				elif distance <= minDistance:
					transform.rigidbody2D.MoveRotation(transform.LookingAt2D(closestEnemy.transform, 0, Mathf.Max(MoveSpeed, 1)).eulerAngles.z)
			else:
				transform.rigidbody2D.MoveRotation(transform.LookingAt2D(References.Player.transform, 0, Mathf.Max(MoveSpeed, 1)).eulerAngles.z)
		
	def GetClosestEnemy() as Enemy:
		shortestDistance as single = 100000
		closestEnemy as Enemy
		
		for enemy as Enemy in References.RoomManager.currentRoom.enemies:
			if not enemy isa Aura:
				distance = Vector2.Distance(transform.position, enemy.transform.position)
				if enemy.insideAura:
					distance += 5
				if distance < shortestDistance:
					closestEnemy = enemy
					shortestDistance = distance
				
		return closestEnemy

	def OnTriggerStay2D(collision as Collider2D):
		enemy as Enemy = collision.GetComponent[of Enemy]()
		if enemy != null:
			if not enemy isa Aura:
				if not enemy.insideAura:
					enemy.transform.localScale *= 1 + (auraBonus / 100)
					enemy.currentScale = enemy.transform.localScale
					enemy.insideAura = true
				
				for stat as Stat in enemy.stats:
					if stat.name != "MaxHealth" and stat.name != "BurstAmount":
						stat.aura = auraBonus
				enemy.UpdateStats()
				
				enemy.BurstAmount = Mathf.Max(enemy.BurstAmount, 1)
				enemy.shineSpeed = auraShineSpeed
				enemy.shineStrength = auraShineStrength
		
	def OnTriggerExit2D(collision as Collider2D):
		enemy = collision.GetComponent[of Enemy]()
		if enemy != null:
			if not enemy isa Aura:
				enemy.insideAura = false
				enemy.UpdateStats()
				enemy.transform.localScale = enemy.initScale * enemy.currentModifier
				enemy.BurstAmount = Mathf.Max(enemy.BurstAmount, 1)
				enemy.shineSpeed = enemy.initShineSpeed
				enemy.shineStrength = enemy.initShineStrength

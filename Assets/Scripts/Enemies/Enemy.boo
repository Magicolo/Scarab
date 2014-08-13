import UnityEngine

class Enemy (Fighter):
	
	[Separator]
	[Range(0, 10)] public minModifier as single = 1
	[Range(0, 10)] public maxModifier as single = 2
	public minDistance as single = 2
	public sightRadius as single = 10
	public baseExperience as single = 3
	[Range(0, 100)] public baseDropChance as single = 10
	[Range(0, 1)] public shineStrength as single = 0
	public shineSpeed as single = 1
	public bark as bool = true
	
	[HideInInspector] public initColor as Color
	[HideInInspector] public initShineSpeed as single
	[HideInInspector] public initShineStrength as single
	[HideInInspector] public initScale as Vector2
	[HideInInspector] public currentScale as Vector2
	[HideInInspector] public currentModifier as single
	[HideInInspector] public roomBonus as single
	[HideInInspector] public insideAura as bool
	
	experience as int
	dropChance as single
	playerInRange as bool
	collisionCounter as single
	collisionDelay as single = 0.25
	barkCounter as single
	barkDelay as single
	
	override def Awake():
		super.Awake()
		
		initColor = spriteRenderer.color
		initShineSpeed = shineSpeed
		initShineStrength = shineStrength
		initScale = transform.localScale
		barkDelay = Random.Range(5, 30)
		
	override def OnEnable():
		super.OnEnable()
		
		insideAura = false
		currentModifier = HF.ProportionalRandomRange(minModifier, maxModifier)
		dropChance = baseDropChance * currentModifier
		transform.localScale = initScale * currentModifier
		currentScale = transform.localScale
		experience = baseExperience * (currentModifier + roomBonus)
		maxHealth.bonus = (currentModifier - 1) * 100 + roomBonus * 100
		fireSpeed.bonus = (currentModifier - 1) * 100 + roomBonus * 50
		damage.bonus = (currentModifier - 1) * 100 + roomBonus * 50
		UpdateStats()
		BurstAmount = Mathf.Max(BurstAmount, 1)
		Health = MaxHealth
	
	override def Update():
		super.Update()
		
		collisionCounter += Time.deltaTime
	
	override def UpdateStats():
		for stat as Stat in stats:
			if not insideAura:
				stat.aura = 0
		super.UpdateStats()
	
	override def Normal():
		super.Normal()
		
		Bark()
		Shine()
		Move()
	
	override def Fire():
		if playerInRange:
			super.Fire()

	override def Kill():
		super.Kill()
		
		References.RoomManager.currentRoom.EnemyHasDied(self)
		References.Player.ApplyExperience(experience)
		DropItem()
	
	virtual def Move():
		if References.Player != null:
			distance as single = Vector3.Distance(transform.position, References.Player.transform.position)
			if distance < sightRadius:
				playerInRange = true
				if distance > minDistance:
					transform.rigidbody2D.MovePosition(transform.position + transform.TransformDirection(Vector2.right) * MoveSpeed * Time.deltaTime)
					transform.rigidbody2D.MoveRotation(transform.LookingAt2D(References.Player.transform, 0, Mathf.Max(MoveSpeed, 1)).eulerAngles.z)
				else:
					transform.rigidbody2D.MoveRotation(transform.LookingAt2D(References.Player.transform, 0, Mathf.Max(MoveSpeed, 1)).eulerAngles.z)
			else:
				playerInRange = false
		else:
			playerInRange = false
	
	def Bark():
		if bark:
			if barkCounter > barkDelay:
				SpawnRandomBark(self)
				barkCounter = 0
				barkDelay = Random.Range(15, 30)
			else:
				barkCounter += Time.deltaTime
			
	def DropItem():
		if Random.value <= dropChance / 100:
			SpawnRandomBonusItem(transform.position, currentModifier)
	
	def Shine():
		if shineStrength > 0 and shineSpeed > 0:
			alpha as single = NormalColor.a
			NormalColor = initColor + Color.white * shineStrength * (Mathf.Sin(shineSpeed * Time.time * Mathf.PI * 2) / 2)
			NormalColor.a = alpha
			
	def OnCollisionStay2D(collision as Collision2D):
		if References.Player != null:
			if collision.gameObject == References.Player.gameObject:
				if collisionCounter >= collisionDelay:
					References.Player.ApplyDamage(Damage)
					collisionCounter = 0
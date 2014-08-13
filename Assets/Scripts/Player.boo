import UnityEngine

class Player (Fighter):
		
	public Regeneration as single:
		get: return regeneration.value
		set: regeneration.value = value
	public regeneration as Stat = Stat("Regeneration", 5, 0)

	[Separator]
	public lives as int = 100
	public maxLevel as int = 100
	public fireKey as KeyCode = KeyCode.Mouse1
	
	[HideInInspector] public level as int = 1
	[HideInInspector] public experience as single
	[HideInInspector] public nextLevel as single
	[HideInInspector] public mouseFollow as FollowMouse2D
	[HideInInspector] public currentScale as Vector2

	[HideInInspector] public hasMap as bool
	[HideInInspector] public hasCompass as bool
	[HideInInspector] public hasSword as bool
	[HideInInspector] public hasBossKey as bool
	
	sword as Sword
	levelList as (single) = array(single, maxLevel)
	collisionCounter as single
	collisionDelay as single = 0.25
	pMoveSpeed as single
	pTurnSpeed as single

	override def Awake():
		sword = transform.FindChild("Sword").GetComponent(Sword)
		sword.owner = self
		sword.gameObject.SetActive(false)
		mouseFollow = GetComponent(FollowMouse2D)
		mouseFollow.moveSpeed = moveSpeed.base
		mouseFollow.turnSpeed = moveSpeed.base * 3
		stats.Add(regeneration)
		currentScale = transform.localScale
		
		super.Awake()
		
		GenerateLevelList()
	
	override def Fire():
		if Input.GetKey(fireKey):
			if hasSword:
				fireCounter = 0
				AudioPlayer.Play("SFX_Weapon_Sword_Slash")
				sword.Swing()
			else:
				super.Fire()
	
	override def Kill():
		if lives <= 0:
			super.Kill()
		else:
			Alive = false
			spriteRenderer.color = DeadColor
			gameObject.layer = 20
			CurrentState = Dead
			pMoveSpeed = mouseFollow.moveSpeed
			pTurnSpeed = mouseFollow.turnSpeed
			mouseFollow.moveSpeed = 0
			mouseFollow.turnSpeed = 0
			source as AudioSource = AudioPlayer.Play("SFX_Character_Die")
			source.pitch += 2 / transform.localScale.magnitude**0.5 - 1
			
	override def Dead():
		currentState = "Dead"
		
		if lives <= 0:
			super.Dead()
		else:
			spriteRenderer.color.a = Mathf.Lerp(spriteRenderer.color.a, 0, 2 * Time.deltaTime)
			if spriteRenderer.color.a.Round(0.05) == 0:
				References.RoomManager.ChangeCurrentRoom(References.RoomManager.lastCompletedRoom.roomCoord, false)
				transform.position = GetUnoccupiedArea(References.RoomManager.lastCompletedRoom.transform.position, transform.localScale)
				mouseFollow.targetPosition = transform.position
				mouseFollow.moveSpeed = pMoveSpeed
				mouseFollow.turnSpeed = pTurnSpeed
				spriteRenderer.color.a = 1
				gameObject.layer = 8
				Health = MaxHealth
				Alive = true
				lives -= 1
				CurrentState = Normal
	
	def GenerateLevelList():
		for i in range(levelList.Length):
			if i == 1:
				levelList[i] = 12
			elif i > 1:
				levelList[i] = (i**2 + levelList[i - 1] + i * 5).Round(1)
		nextLevel = levelList[level]
	
	def ApplyRegeneration():
		regenAmount = (Mathf.Min(Health + MaxHealth * (Regeneration / 100), MaxHealth) - Health).Round(1)
		if regenAmount > 0:
			Health += regenAmount
			SpawnInfoText(regenAmount.ToString(), gameObject, Color(0, 0.85, 0, 1), ColorMode.Normal, 2)
			AudioPlayer.Play("SFX_Character_Heal")
	
	def ApplyExperience(amount as single):
		experience += amount
		SpawnInfoText(amount.ToString(), gameObject, Color.blue, ColorMode.Normal, 1.5)
		while experience >= nextLevel:
			ApplyLevelUp()
			
	def ApplyLevelUp():
		level += 1
		nextLevel = levelList[level]
		for stat as Stat in stats:
			if stat.name != "BurstAmount" or stat.name != "Regeneration":
				stat.base *= stat.levelUpFactor
		transform.localScale *= 1.05
		currentScale = transform.localScale
		SpawnInfoText("LEVEL UP", gameObject, Color.white, ColorMode.Random, 3)
		AudioPlayer.Play("SFX_Character_Levelup")
		UpdateStats()
		Health = MaxHealth
	
	def EquipSword():
		sword.gameObject.SetActive(true)
		fireSpeed.minValue = 2.5
		fireSpeed.maxValue = 10
		UpdateStats()

	def PickUp(item as IPickable):
		if item isa BonusItem:
			(item cast BonusItem).ApplyBonus(self)
			(item cast BonusItem).generated = false
		elif item isa Artifact:
			if not (item cast Artifact).pickedUp:
				(item cast Artifact).ApplyArtifact()

	def OnTriggerEnter2D(collision as Collider2D):
		if not teleporting:
			pickable as IPickable = collision.gameObject.GetComponent(IPickable)
			if pickable != null:
				PickUp(pickable)
		
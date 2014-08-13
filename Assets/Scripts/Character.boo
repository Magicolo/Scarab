import UnityEngine

class Character (MonoBehaviour, IDamageable, IKillable):
	
	public currentState as string
	
	public Health as single:
		get: return health
		set: health = value
	health as single
		
	public HealthBar as HealthBarGUI:
		get: return healthBar
		set: healthBar = value
	healthBar as HealthBarGUI	
		
	public Alive as bool:
		get: return alive
		set: alive = value
	alive as bool = true
		
	public NormalColor as Color:
		get: return normalColor
		set: normalColor = value
	normalColor as Color = Color.white
	
	public DamagedColor as Color:
		get: return damagedColor
		set: damagedColor = value
	damagedColor as Color = Color.red
		
	public DeadColor as Color:
		get: return deadColor
		set: deadColor = value
	deadColor as Color = Color.gray
		
	public MaxHealth as single:
		get: return maxHealth.value
		set: maxHealth.value = value
	[Separator] public maxHealth as Stat = Stat("MaxHealth", 100, 0)
	
	[HideInInspector] public stats as List[of Stat] = List[of Stat]()
	[HideInInspector] public teleporting as bool
	
	CurrentState as callable
	spriteRenderer as SpriteRenderer
	
	virtual def Awake():
		spriteRenderer = GetComponent(SpriteRenderer)
		NormalColor = spriteRenderer.color
		stats.Add(maxHealth)
		CurrentState = Normal
	
	virtual def OnEnable():
		Health = MaxHealth
		
	virtual def Start():
		HealthBar = hObjectPool.Instance.Spawn(References.HealthBarID).GetComponent(HealthBarGUI)
		HealthBar.owner = self
		HealthBar.transform.parent = References.Canvas.transform
	
	virtual def Update():
		CurrentState()
	
	virtual def ApplyDamage(amount as single):
		if CurrentState != Dead:
			amount = Mathf.Max(amount.Round(1), 1)
			Health -= amount
			CurrentState = Damaged
			SpawnInfoText(amount.ToString(), gameObject, Color.red)
			source as AudioSource = AudioPlayer.Play("SFX_Character_Hit")
			source.pitch += 2 / transform.localScale.magnitude**0.5 - 1
	
	virtual def Kill():
		Alive = false
		if HealthBar != null: 
			hObjectPool.Instance.Despawn(HealthBar.gameObject)
		spriteRenderer.color = DeadColor
		rigidbody2D.isKinematic = true
		gameObject.layer = 20
		CurrentState = Dead
		source as AudioSource = AudioPlayer.Play("SFX_Character_Die")
		source.pitch += 2 / transform.localScale.magnitude**0.5 - 1
	
	virtual def Normal():
		currentState = "Normal"
		
		spriteRenderer.color = Color.Lerp(spriteRenderer.color, NormalColor, 5 * Time.deltaTime)

	virtual def Damaged():
		currentState = "Damaged"
		
		spriteRenderer.color = DamagedColor
		if Health <= 0:
			Kill()
		else:
			CurrentState = Normal

	virtual def Dead():
		currentState = "Dead"
		
		spriteRenderer.color.a = Mathf.Lerp(spriteRenderer.color.a, 0, 3 * Time.deltaTime)
		if spriteRenderer.color.a.Round(0.05) == 0:
			Destroy(gameObject)
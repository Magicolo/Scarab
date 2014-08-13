import UnityEngine

class Rock (MonoBehaviour):
	
	public currentState as string
	public baseDamage as single
	public velocityThreshold as single = 1
	public scaleDamageWithVelocity as bool = true
	public maxVelocity as single = 10
	public lifeTime as single = 5
	
	[HideInInspector] public owner as Fighter
	[HideInInspector] public damage as single
	[HideInInspector] public initScale as Vector2
	[HideInInspector] public initMass as single
	[HideInInspector] public CurrentState as callable
	
	lifeCounter as single
	normalColor as Color = Color.white
	spriteRenderer as SpriteRenderer

	def Awake():
		CurrentState = Normal
		spriteRenderer = GetComponent(SpriteRenderer)
		initScale = transform.localScale
		initMass = rigidbody2D.mass
		normalColor = spriteRenderer.color
		
	def OnEnable():
		collider2D.enabled = true

	def Update():
		lifeCounter += Time.deltaTime
		CurrentState()
		
	def Normal():
		currentState = "Normal"
		
		if lifeCounter >= lifeTime or rigidbody2D.IsSleeping():
			CurrentState = FadingOut

	def FadingOut():
		currentState = "FadingOut"
		
		spriteRenderer.color.a = Mathf.Lerp(spriteRenderer.color.a, 0, 6 * Time.deltaTime)
		if spriteRenderer.color.a.Round(0.01) == 0:
			CurrentState = Despawning
		
	def Despawning():
		currentState = "Despawning"
		lifeCounter = 0
		spriteRenderer.color = normalColor
		CurrentState = Normal
		hObjectPool.Instance.Despawn(gameObject)

	def OnCollisionEnter2D(collision as Collision2D):
		if CurrentState == Normal:
			damageable as IDamageable = collision.gameObject.GetComponent(IDamageable)
			if damageable:
				if rigidbody2D.velocity.magnitude > velocityThreshold:
					if scaleDamageWithVelocity: damageable.ApplyDamage(damage * Mathf.Clamp01(rigidbody2D.velocity.magnitude / (maxVelocity - velocityThreshold)))
					else: damageable.ApplyDamage(damage)
					CurrentState = Despawning
			else:
				collider2D.enabled = false
				gameObject.layer = 15
				CurrentState = FadingOut

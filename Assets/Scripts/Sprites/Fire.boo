import UnityEngine

class Fire (MonoBehaviour): 

	public currentState as string
	public damage as single = 10
	public lifeTime as single = 5
	public destroyOnRoomChange as bool = true
	public hitEnemies as bool = false
	
	CurrentState as callable
	lifeCounter as single
	collisionCounter as single
	collisionDelay as single = 0.25
	normalColor as Color = Color.white
	spriteRenderer as SpriteRenderer
	initScale as Vector2
	
	def Awake():
		CurrentState = Normal
		spriteRenderer = GetComponent(SpriteRenderer)
		normalColor = spriteRenderer.color
		initScale = transform.localScale
		
	def Update():
		CurrentState()
		collisionCounter += Time.deltaTime
		
	def Normal():
		currentState = "Normal"
		
		if lifeTime > 0:
			if lifeCounter >= lifeTime:
				CurrentState = FadingOut
			else:
				lifeCounter += Time.deltaTime
				
	def FadingOut():
		currentState = "FadingOut"
		
		spriteRenderer.color.a = Mathf.Lerp(spriteRenderer.color.a, 0, 3 * Time.deltaTime)
		transform.localScale = Vector2.Lerp(transform.localScale, Vector2.zero, 3 * Time.deltaTime)
		if spriteRenderer.color.a.Round(0.01) == 0:
			CurrentState = Despawning
		
	def Despawning():
		currentState = "Despawning"
		
		lifeCounter = 0
		spriteRenderer.color = normalColor
		transform.localScale = initScale
		CurrentState = Normal
		hObjectPool.Instance.Despawn(gameObject)
	
	def OnRoomExit(room as Room):
		if destroyOnRoomChange:
			hObjectPool.Instance.Despawn(gameObject)
	
	def OnTriggerStay2D(collision as Collider2D):
		if collisionCounter >= collisionDelay:
			character as Character = collision.GetComponent(Character)
			if character != null:
				collisionCounter = 0
				if hitEnemies:
					if character.gameObject.layer == 9:
						character.ApplyDamage(damage)
				if character.gameObject == References.Player.gameObject:
					character.ApplyDamage(damage)

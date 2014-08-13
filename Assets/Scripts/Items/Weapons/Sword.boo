import UnityEngine

class Sword (MonoBehaviour): 

	public currentState as string
	public damage as single = 150
	public lifeSteal as single = 5
	public slashArc as single = 120
	
	[HideInInspector] public owner as Fighter
	
	CurrentState as callable
	initAngle as single
	swordCollider as PolygonCollider2D
	
	def Awake():
		CurrentState = Idle
		initAngle = transform.localEulerAngles.z
		swordCollider = GetComponent(PolygonCollider2D)
		
	def Update():
		CurrentState()
		
	def Idle():
		currentState = "Idle"
	
	def Swing():
		transform.localEulerAngles.z = initAngle + slashArc / 2
		CurrentState = Swinging

	def Swinging():
		currentState = "Swinging"
		
		transform.localEulerAngles.z -= slashArc * owner.FireSpeed * Time.deltaTime
		
		if transform.localEulerAngles.z <= initAngle - slashArc / 2:
			transform.localEulerAngles.z = initAngle
			CurrentState = Idle

	def OnTriggerEnter2D(collision as Collider2D):
		if CurrentState != Swinging:
			rock as Rock = collision.gameObject.GetComponent(Rock)
			if rock != null:
				rock.CurrentState = rock.Despawning
				AudioPlayer.Play("SFX_Weapon_Sword_Block")
		
	def OnTriggerStay2D(collision as Collider2D):
		if CurrentState == Swinging:
			if owner != null:
				damageable as IDamageable = collision.gameObject.GetComponent(IDamageable)
				if damageable != null:
					damageable.ApplyDamage(damage + owner.Damage)
					lifeStealAmount = (Mathf.Min(owner.Health + (damage + owner.Damage) * (lifeSteal / 100), owner.MaxHealth) - owner.Health).Round(1)
					if lifeStealAmount > 0:
						owner.Health += lifeStealAmount
						SpawnInfoText(lifeStealAmount.ToString(), gameObject, Color(0, 0.85, 0, 1), ColorMode.Normal, 2)
					
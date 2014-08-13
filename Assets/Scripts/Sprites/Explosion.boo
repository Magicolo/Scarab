import UnityEngine

class Explosion (MonoBehaviour): 

	public damage as single
	public offset as Vector2

	[HideInInspector] public owner as GameObject
	
	circleColllider as CircleCollider2D
	animator as Animator
	lifeCounter as single
	
	def Awake():
		circleColllider = GetComponent[of CircleCollider2D]()
		animator = GetComponent[of Animator]()

	def OnEnable():
		circleColllider.enabled = true
		animator.Play("Explosion")
		
	def Update():
		if owner != null:
			transform.position = owner.transform.position + Vector2(offset.x * owner.transform.localScale.x, offset.y * owner.transform.localScale.y)
		
		if lifeCounter > animator.GetCurrentAnimatorStateInfo(0).length:
			lifeCounter = 0
			circleColllider.enabled = false
		else:
			lifeCounter += Time.deltaTime
	
	def OnRoomExit(room as Room):
		hObjectPool.Instance.Despawn(gameObject)
	
	def OnTriggerEnter2D(collision as Collider2D):
		damageable as IDamageable = collision.GetComponent(IDamageable)
		if damageable != null:
			damageable.ApplyDamage(damage)
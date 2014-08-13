import UnityEngine

class FakeWall (MonoBehaviour): 

	public fadeSpeed as single = 5

	isVisible as bool = true
	spriteRenderer as SpriteRenderer
	boxCollider as BoxCollider2D

	def OnDisable():
		isVisible = true
		spriteRenderer.color.a = 1

	def Awake():
		spriteRenderer = GetComponent[of SpriteRenderer]()
		boxCollider = GetComponent[of BoxCollider2D]()
		boxCollider.isTrigger = true

	def Update():
		if isVisible:
			spriteRenderer.color.a = Mathf.Lerp(spriteRenderer.color.a, 1, fadeSpeed * Time.deltaTime)
		else:
			spriteRenderer.color.a = Mathf.Lerp(spriteRenderer.color.a, 0.25, fadeSpeed * Time.deltaTime)

	def OnTriggerEnter2D(collision as Collider2D):
		if collision.gameObject == References.Player.gameObject:
			isVisible = false
			
	def OnTriggerExit2D(collision as Collider2D):
		if collision.gameObject == References.Player.gameObject:
			isVisible = true
		
import UnityEngine

class InfoText (MonoBehaviour): 

	public offset as Vector2 = Vector2(0, 0.75)

	[HideInInspector] public owner as GameObject
	[HideInInspector] public colorMode as ColorMode
	[HideInInspector] public textMesh as TextMesh
	
	meshObject as GameObject
	animator as Animator
	lifeCounter as single

	def Awake():
		meshObject = transform.GetChild(0).gameObject
		animator = meshObject.GetComponent(Animator)
		textMesh = meshObject.GetComponent(TextMesh)

	def OnEnable():
		animator.Play("InfoText")

	def Update():
		if owner != null:
			transform.position = owner.transform.position + Vector2(offset.x * owner.transform.localScale.x, offset.y * owner.transform.localScale.y)
			if colorMode == ColorMode.Random:
				textMesh.color = Color(Random.value, 1, 1).ToRGB()
		
		if lifeCounter > animator.GetCurrentAnimatorStateInfo(0).length:
			lifeCounter = 0
			hObjectPool.Instance.Despawn(gameObject)
		else:
			lifeCounter += Time.deltaTime

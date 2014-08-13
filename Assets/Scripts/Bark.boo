import UnityEngine
import System.Collections

class Bark (MonoBehaviour): 

	public typeSpeed as single = 10
	public offset as Vector2 = Vector2(0, 0.75)

	[HideInInspector] public currentText as string
	[HideInInspector] public owner as Character
	
	meshObject as GameObject
	textMesh as TextMesh

	def Awake():
		meshObject = transform.FindChild("Mesh").gameObject
		textMesh = meshObject.GetComponent[of TextMesh]()
		
	def OnEnable():
		textMesh.text = ""
		StartCoroutine(TypeText())
	
	def Update():
		if owner != null:
			transform.position = owner.transform.position + Vector2(offset.x * owner.transform.localScale.x, offset.y * owner.transform.localScale.y)
			
	def TypeText() as IEnumerator:
		yield WaitForEndOfFrame()
		for letter in currentText:
			if owner.Alive:
				textMesh.text += letter
				yield WaitForSeconds(1.0 / typeSpeed)
			else:
				hObjectPool.Instance.Despawn(gameObject)
				return
		yield WaitForSeconds(currentText.Length / 10)
		hObjectPool.Instance.Despawn(gameObject)


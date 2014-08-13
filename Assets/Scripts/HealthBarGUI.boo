import UnityEngine

class HealthBarGUI (MonoBehaviour): 

	public offset as Vector2 = Vector2(0, 0.5)

	[HideInInspector] public owner as Character
	[HideInInspector] public healthBar as UI.Slider

	def Awake():
		healthBar = gameObject.GetComponent(UI.Slider)

	def Update():
		if owner != null:
			for child as Transform in transform.GetChildren():
				child.gameObject.SetActive(owner.gameObject.activeSelf)
			transform.position = References.MainCamera.WorldToScreenPoint(owner.transform.position + Vector2(offset.x * owner.transform.localScale.x, offset.y * owner.transform.localScale.y))
			healthBar.value = owner.Health / owner.MaxHealth
		else:
			hObjectPool.Instance.Despawn(gameObject)

import UnityEngine

class Artifact (MonoBehaviour, IPickable): 

	public oscillateAmplitude as single = 0.5
	public oscillateFrequency as single = 1

	[HideInInspector] public pickedUp as bool
	[HideInInspector] public spriteTransform as Transform
	[HideInInspector] public spriteRenderer as SpriteRenderer

	initSpritePosition as Vector2
	CurrentState as callable

	def Awake():
		spriteTransform = transform.FindChild("Sprite")
		spriteRenderer = spriteTransform.GetComponent[of SpriteRenderer]()
		initSpritePosition = spriteTransform.localPosition
		CurrentState = Idle
		
	def Update():
		CurrentState()

	virtual def ApplyArtifact():
		pickedUp = true

	def Idle():
		spriteTransform.localPosition.y = initSpritePosition.y + oscillateAmplitude * (Mathf.Sin(Time.time * oscillateFrequency) + 1)

	def PickedUp():
		spriteTransform.position = Vector2.Lerp(spriteTransform.position, References.Player.transform.position, 5 * Time.deltaTime)
		spriteTransform.localScale = Vector2.Lerp(spriteTransform.localScale, Vector2.zero, 2 * Time.deltaTime)
		
		if spriteTransform.localScale.magnitude.Round(0.25) == 0:
			ApplyArtifact()
			Destroy(spriteTransform.gameObject)
			Destroy(self)
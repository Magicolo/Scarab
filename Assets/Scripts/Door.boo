import UnityEngine

[ExecuteInEditMode]
class Door (MonoBehaviour): 
	enum DoorState:
		Opened
		Closed
	enum Triggers:
		None
		OnPlayerTriggerEnter
		OnPlayerTriggerExit
		OnButtonPressEnter
		OnButtonPressExit

	public color as Color = Color(0, 0, 0, 1)
	public startState as DoorState = DoorState.Closed
	public openTrigger as Triggers = Triggers.OnPlayerTriggerEnter
	public openSpeed as single = 1
	public closeTrigger as Triggers = Triggers.OnPlayerTriggerExit
	public closeSpeed as single = 1
	
	[HideInInspector] public doorState as DoorState
	[HideInInspector] public animator as Animator
	[HideInInspector] public upDoor as Transform
	[HideInInspector] public upDoorRenderer as SpriteRenderer
	[HideInInspector] public downDoor as Transform
	[HideInInspector] public downDoorRenderer as SpriteRenderer
	
	player as Player
	playerTrigger as BoxCollider2D
	material as Material
	colorValue as single
	pColorValue as single

	def Awake():
		if Application.isPlaying:
			player = References.Player
			animator = GetComponent(Animator)
			upDoor = transform.FindChild("Up")
			upDoorRenderer = upDoor.GetComponent[of SpriteRenderer]()
			downDoor = transform.FindChild("Down")	
			downDoorRenderer = downDoor.GetComponent[of SpriteRenderer]()
			doorState = startState
		else:
			Preview()

	def Update():
		if Application.isPlaying:
			if upDoorRenderer.color != color or downDoorRenderer.color != color:
				upDoorRenderer.color = color
				downDoorRenderer.color = color
				
			if doorState == DoorState.Opened and upDoor.localPosition.y == upDoor.localScale.y / 2:
				animator.SetTrigger("Open")
			if doorState == DoorState.Closed and upDoor.localPosition.y == upDoor.localScale.y + upDoor.localScale.y / 2:
				animator.SetTrigger("Close")
			
			if doorState == DoorState.Opened:
				animator.speed = openSpeed
			elif doorState == DoorState.Closed:
				animator.speed = closeSpeed
		else:
			ifdef UNITY_EDITOR:
				if gameObject in UnityEditor.Selection.gameObjects:
					Preview()
	
	def OnTriggerEnter2D(collision as Collider2D):
		if openTrigger == Triggers.OnPlayerTriggerEnter:
			if doorState != DoorState.Opened:
				if collision.gameObject == player.gameObject:
					animator.SetTrigger("Open")
					doorState = DoorState.Opened
		elif closeTrigger == Triggers.OnPlayerTriggerEnter:
			if doorState != DoorState.Closed:
				if collision.gameObject == player.gameObject:
					animator.SetTrigger("Close")
					doorState = DoorState.Closed
	
	def OnTriggerExit2D(collision as Collider2D):
		if openTrigger == Triggers.OnPlayerTriggerExit:
			if doorState != DoorState.Opened:
				if collision.gameObject == player.gameObject:
					animator.SetTrigger("Open")
					doorState = DoorState.Opened
		elif closeTrigger == Triggers.OnPlayerTriggerExit:
			if doorState != DoorState.Closed:
				if collision.gameObject == player.gameObject:
					animator.SetTrigger("Close")
					doorState = DoorState.Closed
	
	def OnButtonPressEnter():
		if openTrigger == Triggers.OnButtonPressEnter:
			if doorState != DoorState.Opened:
				animator.SetTrigger("Open")
				doorState = DoorState.Opened
		elif closeTrigger == Triggers.OnButtonPressEnter:
			if doorState != DoorState.Closed:
				animator.SetTrigger("Close")
				doorState = DoorState.Closed
	
	def OnButtonPressExit():
		if openTrigger == Triggers.OnButtonPressExit:
			if doorState != DoorState.Opened:
				animator.SetTrigger("Open")
				doorState = DoorState.Opened
		elif closeTrigger == Triggers.OnButtonPressExit:
			if doorState != DoorState.Closed:
				animator.SetTrigger("Close")
				doorState = DoorState.Closed
			
	def Preview():
		upDoor = transform.FindChild("Up")
		upDoorRenderer = upDoor.GetComponent[of SpriteRenderer]()
		downDoor = transform.FindChild("Down")	
		downDoorRenderer = downDoor.GetComponent[of SpriteRenderer]()
		
		if upDoorRenderer.color != color or downDoorRenderer.color != color:
			upDoorRenderer.color = color
			downDoorRenderer.color = color
		
		if startState == DoorState.Opened:
			upDoor.localPosition.y = upDoor.localScale.y + upDoor.localScale.y / 2
			downDoor.localPosition.y = -(downDoor.localScale.y + downDoor.localScale.y / 2)
		else:
			upDoor.localPosition.y = upDoor.localScale.y / 2
			downDoor.localPosition.y = -downDoor.localScale.y / 2
			
		if collider2D:
			playerTrigger = collider2D
		if openTrigger == Triggers.OnPlayerTriggerEnter or openTrigger == Triggers.OnPlayerTriggerExit or closeTrigger == Triggers.OnPlayerTriggerEnter or closeTrigger == Triggers.OnPlayerTriggerExit:
			if not playerTrigger:
				playerTrigger = gameObject.AddComponent(BoxCollider2D)
				playerTrigger.isTrigger = true
				playerTrigger.size.x = 1
				playerTrigger.size.y = upDoor.localScale.y + downDoor.localScale.y
		else:
			if playerTrigger:
				DestroyImmediate(playerTrigger)

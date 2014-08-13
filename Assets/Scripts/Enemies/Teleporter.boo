import UnityEngine

class Teleporter (Enemy): 

	[Separator]
	public teleportSpeed as single = 10
	public teleportMinRange as single = 5
	public teleportMaxRange as single = 15
	
	override def OnEnable():
		super.OnEnable()
		
	override def Fire():
		CurrentState = TeleportDepart
		source as AudioSource = AudioPlayer.Play("SFX_Character_Teleport")
		source.pitch += 10 / teleportSpeed - 1
		
	def TeleportDepart():
		currentState = "TeleportDepart"
		
		if References.Player != null:
			transform.localScale = Vector2.Lerp(transform.localScale, Vector2.zero, teleportSpeed * Time.deltaTime)
		
			if transform.localScale.magnitude.Round(0.25) <= 0:
				transform.localScale = Vector2.zero
				randomPoint as Vector2 = Random.insideUnitCircle * teleportMaxRange
				randomPoint /= Mathf.Min(randomPoint.magnitude / teleportMinRange, 1)
				randomPoint += References.Player.transform.position
				currentRoomPosition as Vector2 = References.RoomManager.currentRoom.transform.position
				randomPoint.x = Mathf.Clamp(randomPoint.x, currentRoomPosition.x - 10, currentRoomPosition.x + 10)
				randomPoint.y = Mathf.Clamp(randomPoint.y, currentRoomPosition.y - 8, currentRoomPosition.y + 8)
				transform.position = randomPoint
				transform.rigidbody2D.MoveRotation(transform.LookingAt2D(References.Player.transform).eulerAngles.z)
				CurrentState = TeleportArrive
			
	def TeleportArrive():
		currentState = "TeleportArrive"
		
		transform.localScale = Vector2.Lerp(transform.localScale, currentScale, teleportSpeed * Time.deltaTime)
		
		if transform.localScale.magnitude.Round(0.25) >= currentScale.magnitude.Round(0.25):
			transform.localScale = currentScale
			
			super.Fire()
			CurrentState = Normal
		
			
	

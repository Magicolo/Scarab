import UnityEngine

class RandomBlock (MonoBehaviour): 

	public moveSpeed as single = 300
	
	targetChangeCounter as single
	targetChangeDelay as single = 2
	target as Vector3
	
	def OnEnable():
		ChangeTarget()
	
	def Update():
		if targetChangeCounter >= targetChangeDelay or (target - transform.position).magnitude < 1:
			targetChangeCounter = 0
			ChangeTarget()
		else:
			targetChangeCounter += Time.deltaTime
		rigidbody2D.velocity = (target - transform.position).normalized * moveSpeed * Time.deltaTime
		
	def ChangeTarget():
		if References.RoomManager.currentRoom != null:
			currentRoomPosition as Vector2 = References.RoomManager.currentRoom.transform.position
			targetX = Random.Range((currentRoomPosition.x - 11 + transform.localScale.x / 2) cast single, (currentRoomPosition.x + 11 - transform.localScale.x / 2) cast single)
			targetY = Random.Range((currentRoomPosition.y - 8.5 + transform.localScale.y / 2) cast single, (currentRoomPosition.y + 8.5 - transform.localScale.y / 2) cast single)
			target = Vector3(targetX, targetY, transform.position.z)
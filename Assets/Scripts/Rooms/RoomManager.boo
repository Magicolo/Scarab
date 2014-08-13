import UnityEngine

class RoomManager (MonoBehaviour): 

	public roomsX as int = 11
	public roomsY as int = 11
	
	public startRoom as Vector2 = Vector2(4, 4)
	public roomMatrix as (Room, 2) = matrix(Room, roomsY, roomsX)
	public currentRoom as Room
	
	[HideInInspector] public lastCompletedRoom as Room

	def Start():
		ChangeCurrentRoom(startRoom, false)
		lastCompletedRoom = currentRoom

	def UpdateRoomOrder():
		for y in range(roomsY):
			for x in range(roomsX):
				room as Room = roomMatrix[y, x]
				if room != null:
					if room == currentRoom:
						room.order = -10
						for door as Door in room.doors:
							door.animator.enabled = true
					elif GetAdjacentRooms(currentRoom).Contains(room):
						room.order = -30
						for door as Door in room.doors:
							door.animator.enabled = true
					else:
						room.order = -50
						for door as Door in room.doors:
							door.animator.enabled = false
					for door as Door in room.doors:
						door.upDoorRenderer.sortingOrder = -31
						door.downDoorRenderer.sortingOrder = -31
					room.UpdateSprites()
					
	def ChangeCurrentRoom(roomCoord as Vector2):
		ChangeCurrentRoom(roomCoord, true)
		
	def ChangeCurrentRoom(roomCoord as Vector2, pushPlayer as bool):
		if currentRoom != null:
			gameObject.BroadcastMessage("OnRoomExit", currentRoom, SendMessageOptions.DontRequireReceiver)
			References.Rooms.BroadcastMessage("OnRoomExit", currentRoom, SendMessageOptions.DontRequireReceiver)
		
		currentRoom = roomMatrix[roomCoord.y, roomCoord.x]
		if pushPlayer:
			currentRoom.PushPlayerInside()
		UpdateRoomOrder()
		
		gameObject.BroadcastMessage("OnRoomEnter", currentRoom, SendMessageOptions.DontRequireReceiver)
		References.Rooms.BroadcastMessage("OnRoomEnter", currentRoom, SendMessageOptions.DontRequireReceiver)
	
	def GetAdjacentRooms(room as Room) as List[of Room]:
		adjacentRooms as List[of Room] = List[of Room]()
		coords as Vector2 = room.roomCoord
		
		if coords.x > 0:
			adjacentRoom = roomMatrix[coords.y, coords.x - 1]
			if adjacentRoom != null:
				adjacentRooms.Add(adjacentRoom)
		if coords.x < roomsX - 1:
			adjacentRoom = roomMatrix[coords.y, coords.x + 1]
			if adjacentRoom != null:
				adjacentRooms.Add(adjacentRoom)
		if coords.y > 0:
			adjacentRoom = roomMatrix[coords.y - 1, coords.x]
			if adjacentRoom != null:
				adjacentRooms.Add(adjacentRoom)
		if coords.y < roomsY - 1:
			adjacentRoom = roomMatrix[coords.y + 1, coords.x]
			if adjacentRoom != null:
				adjacentRooms.Add(adjacentRoom)
		
		return adjacentRooms
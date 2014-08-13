import UnityEngine

class MapUI (MonoBehaviour): 

	public flashSpeed as single = 2

	image as UI.Image
	sprite as Sprite
	texture as Texture2D
	flashCounter as single
	isWhite as bool
	showMap as bool

	def Awake():
		image = GetComponent[of UI.Image]()
		GenerateSprite()

	def Update():
		if Input.GetKeyDown(KeyCode.M):
			showMap = not showMap
		image.enabled = showMap
			
		if flashCounter >= 1.0 / flashSpeed:
			flashCounter = 0
			if isWhite:
				texture.SetPixel(References.RoomManager.currentRoom.roomCoord.x, References.RoomManager.currentRoom.roomCoord.y, References.RoomManager.currentRoom.color)
			else:
				texture.SetPixel(References.RoomManager.currentRoom.roomCoord.x, References.RoomManager.currentRoom.roomCoord.y, Color.white)
			isWhite = not isWhite
			texture.Apply()
		else:
			flashCounter += Time.deltaTime
	
	def UpdateMap():
		for y in range(11):
			for x in range(11):
				room as Room = References.RoomManager.roomMatrix[y, x]
				if References.Player != null:
					if References.Player.hasMap and References.Player.hasCompass:
						if room.visited:
							texture.SetPixel(x, y, room.color)
						else:
							texture.SetPixel(x, y, room.color / 2)
					elif References.Player.hasMap:
						if room.visited:
							texture.SetPixel(x, y, Color.grey)
						else:
							texture.SetPixel(x, y, Color.black)
					elif References.Player.hasCompass:
						if room.isGate:
							if room.visited:
								texture.SetPixel(x, y, room.color)
							else:
								texture.SetPixel(x, y, room.color / 2)
						else:
							texture.SetPixel(x, y, Color.black)
					else:
						texture.SetPixel(x, y, Color.black)
						
		texture.SetPixel(References.RoomManager.currentRoom.roomCoord.x, References.RoomManager.currentRoom.roomCoord.y, Color.white)
		isWhite = true
		texture.Apply()
	
	def GenerateSprite():
		texture = Texture2D(11, 11)
		texture.filterMode = FilterMode.Point
		sprite = Sprite.Create(texture, Rect(0, 0, 11, 11), Vector2.zero, 11)
		image.sprite = sprite
	
	def OnRoomEnter(room as Room):
		if room.enemies.Count > 0:
			showMap = false
		UpdateMap()
		
	def OnRoomExit(room as Room):
		UpdateMap()
		

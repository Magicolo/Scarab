import UnityEngine
import System.Collections

[ExecuteInEditMode]
class Room (MonoBehaviour): 

	[Separator]
	[Button("Reparent Objects", "ReparentObjects", NoPrefixLabel : true)] public reparentObjects as bool
	public roomSection as RoomSection.RoomSections
	public color as Color = Color.white
	public isGate as bool
	public order as int = 1
	public roomMatrixSize as int = 10
	public layerMask as LayerMask
	
	[HideInInspector] public roomCoord as Vector2
	[HideInInspector] public cameraSocket as GameObject
	[HideInInspector] public doors as (Door)
	[HideInInspector] public enemies as List[of Enemy]
	[HideInInspector] public visited as bool
	
	sprites as (SpriteRenderer)
	boxCollider as BoxCollider2D
	ejectPlayerEnumerator as IEnumerator
	
	def Awake():
		if Application.isPlaying:
			cameraSocket = transform.FindChild("CameraSocket").gameObject
			doors = GetComponentsInChildren[of Door]()
			boxCollider = GetComponent[of BoxCollider2D]()
			roomCoord.x = (transform.position.x / 25) cast int
			roomCoord.y = (transform.position.y / 20) cast int
			SetSprites()
			References.RoomManager.roomMatrix[roomCoord.y, roomCoord.x] = self
			CheckForEnemies()
			UpdateSprites()
		else:
			Preview()
			
	def Update():
		if Application.isPlaying:
			pass
		else:
			ifdef UNITY_EDITOR:
				if gameObject in UnityEditor.Selection.gameObjects:
					Preview()
	
	def UpdateSprites():
		for sprite as SpriteRenderer in sprites:
			sprite.color = color
			if sprite.gameObject.name == "Up" or sprite.gameObject.name == "Down":
				Logger.Log("DOOR")
				sprite.sortingOrder = -31
			else:
				sprite.sortingOrder = order
	
	def SetSprites():
		spriteList = List[of SpriteRenderer]()
		for sprite as SpriteRenderer in GetComponentsInChildren[of SpriteRenderer]():
			if sprite.sprite.name == "Orange_Box32x32":
				spriteList.Add(sprite)
		sprites = array(spriteList)
	
	def EnemyHasDied(enemy as Enemy):
		enemies.Remove(enemy)
		if enemies.Count <= 0:
			if References.Player != null:
				References.Player.ApplyRegeneration()
			SetDoorsOpen(true)
			References.RoomManager.lastCompletedRoom = self
			SpawnRandomBonusItem(transform.position)
			AudioPlayer.Play("SFX_Room_Door_Open")
	
	def CheckForEnemies():
		enemies = List[of Enemy]()
		for child as Transform in transform.GetChildren():
			if child.gameObject.layer == 9:
				enemy as Enemy = child.GetComponent(Enemy)
				if enemy != null:
					enemies.Add(enemy)
					enemy.roomBonus = roomSection cast int
		if References.RoomManager.currentRoom == self:
			if enemies.Count > 0:
				SetDoorsOpen(false)
				AudioPlayer.Play("SFX_Room_Door_Close")
		else:
			SetRoomObjectsActive(false)

	def IsBorder(coord as single) as bool:
		if coord == 0 or coord == roomMatrixSize:
			return true
		else:
			return false

	def SetDoorsOpen(open as bool):
		for door as Door in doors:
			if open:
				door.doorState = Door.DoorState.Opened
			else:
				door.doorState = Door.DoorState.Closed
				
	def SetRoomObjectsActive(active as bool):
		for child as Transform in transform.GetChildren():
			if child.gameObject.layer != 10:
				child.gameObject.SetActive(active)

	def Preview():
		SetSprites()
		
		transform.position.x = Mathf.Clamp(transform.position.x.Round(25), 0, roomMatrixSize * 25)
		transform.position.y = Mathf.Clamp(transform.position.y.Round(20), 0, roomMatrixSize * 20)
		
		roomCoord.x = (transform.position.x / 25) cast int
		roomCoord.y = (transform.position.y / 20) cast int
		
		if IsBorder(roomCoord.x):
			if IsBorder(roomCoord.y):
				gameObject.name = "CornerRoom" + roomCoord.x + "_" + roomCoord.y
			else:
				gameObject.name = "BorderRoom" + roomCoord.x + "_" + roomCoord.y
		elif IsBorder(roomCoord.y):
			gameObject.name = "BorderRoom" + roomCoord.x + "_" + roomCoord.y
		else:
			gameObject.name = "Room" + roomCoord.x + "_" + roomCoord.y
			
		UpdateSprites()
		
	def ReparentObjects():
		for obj as Transform in FindObjectsOfType(Transform):
			if obj.parent == null:
				if obj.position.x > transform.position.x - 12 and obj.position.x < transform.position.x + 12:
					if obj.position.y > transform.position.y - 9.5 and obj.position.y < transform.position.y + 9.5:
						if obj.GetComponent(Enemy) != null or obj.GetComponent(BonusItem) or obj.GetComponent(Teleport) or obj.GetComponent(Fire) or obj.GetComponent(Artifact) or obj.name == "WallOrange" or obj.name == "RandomBlock":
							Debug.Log("Reparented: " + obj.name + " to: " + name)
							obj.parent = transform
	
	def PushPlayerInside():
		References.Player.transform.position += (transform.position - References.Player.transform.position).normalized * 1.5
		References.Player.mouseFollow.targetPosition = References.Player.transform.position
	
	virtual def OnRoomEnter(room as Room):
		if room == self:
			visited = true
			SetRoomObjectsActive(true)
			CheckForEnemies()
			References.MainCameraFollow.target = cameraSocket.transform
			
	virtual def OnRoomExit(room as Room):
		if room == self:
			SetDoorsOpen(true)
			SetRoomObjectsActive(false)

	def OnTriggerStay2D(collision as Collider2D):
		if References.Player != null:
			if collision.gameObject == References.Player.gameObject:
				if References.RoomManager.currentRoom == self:
					if enemies.Count <= 0:
						References.RoomManager.lastCompletedRoom = self
				else:
					References.RoomManager.currentRoom.SetDoorsOpen(true)
					References.RoomManager.ChangeCurrentRoom(roomCoord)

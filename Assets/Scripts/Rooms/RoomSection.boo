import UnityEngine

[ExecuteInEditMode]
class RoomSection (MonoBehaviour): 

	enum RoomSections:
		Orange
		Green
		Red
		Yellow
		Special
		StartEnd

	[Separator]
	[Button("Update Rooms", "UpdateRooms", NoPrefixLabel : true)] public updateRooms as bool
	public section as RoomSections
	public color as Color = Color.white
	public roomCount as int
	
	def UpdateRooms():
		if not Application.isPlaying:
			ifdef UNITY_EDITOR:
				roomCount = 0
				for child as Transform in transform.GetChildren():
					room as Room = child.GetComponent[of Room]()
					if room != null:
						roomCount += 1
						room.roomSection = section
						room.color = color
						room.SetSprites()
						room.UpdateSprites()
						UnityEditor.EditorUtility.SetDirty(room)
import UnityEngine
import System.Collections

[ExecuteInEditMode]
class Teleport (MonoBehaviour): 

	public pair as Teleport
	public teleportSpeed as single = 10
	public affectsOnlyPlayer as bool
	
	[HideInInspector] public room as Room
	[HideInInspector] public isActive as bool = true
	[HideInInspector] public rotator as Rotator
	[HideInInspector] public toTeleportScale as Vector2
	[HideInInspector] public toNotTeleport as List[of GameObject]
	
	rotateSpeed as single
	
	def Awake():
		if Application.isPlaying:
			room = transform.parent.GetComponent[of Room]()
			rotator = GetComponent[of Rotator]()
			rotateSpeed = rotator.rotateSpeed
			toNotTeleport = List[of GameObject]()
			
			if pair == null:
				rotator.rotateSpeed = 0
			elif not pair.isActive:
				rotator.rotateSpeed = 0
			else:
				rotator.rotateSpeed = rotateSpeed
		
	def Update():
		if Application.isPlaying:
			if pair == null:
				rotator.rotateSpeed = 0
			elif not pair.isActive:
				rotator.rotateSpeed = 0
			else:
				rotator.rotateSpeed = rotateSpeed
		else:
			ifdef UNITY_EDITOR:
				if gameObject in UnityEditor.Selection.gameObjects:
					Preview()
	
	def TeleportStart(toTeleport as GameObject):
		StartCoroutine(TeleportDepart(toTeleport, toTeleport.transform.localScale))
		source as AudioSource = AudioPlayer.Play("SFX_Character_Teleport")
		source.pitch += 10 / teleportSpeed - 1
		
	def TeleportDepart(toTeleport as GameObject, initScale as Vector2) as IEnumerator:
		while(toTeleport.transform.localScale.magnitude.Round(0.25) > 0):
			toTeleport.transform.localScale = Vector2.Lerp(toTeleport.transform.localScale, Vector2.zero, teleportSpeed * Time.deltaTime)
			yield WaitForSeconds(0)
			
		pair.gameObject.SetActive(true)
		toTeleport.transform.localScale = Vector2.zero
		toTeleport.transform.position = pair.transform.position
		if References.Player != null:
			if toTeleport == References.Player.gameObject:
				References.Player.teleporting = true
				if room != pair.room:
					References.RoomManager.ChangeCurrentRoom(pair.room.roomCoord, false)
					References.MainCamera.transform.position = pair.room.transform.position
		pair.toNotTeleport.Add(toTeleport)
		pair.StartCoroutine(TeleportArrive(toTeleport, initScale))
		
	def TeleportArrive(toTeleport as GameObject, initScale as Vector2) as IEnumerator:
		counter as single
		
		while(toTeleport.transform.localScale.magnitude.Round(0.25) < initScale.magnitude.Round(0.25) and not counter > 10 / teleportSpeed):
			toTeleport.transform.localScale = Vector2.Lerp(toTeleport.transform.localScale, initScale, teleportSpeed * Time.deltaTime)
			if References.Player != null:
				if toTeleport == References.Player.gameObject:
					initScale = References.Player.currentScale
					References.Player.mouseFollow.targetPosition = pair.transform.position
			counter += Time.deltaTime
			yield WaitForSeconds(0)
		
		if References.Player != null:
			if toTeleport == References.Player.gameObject:
				toTeleport.transform.localScale = References.Player.currentScale
				References.Player.teleporting = false
			else:
				toTeleport.transform.localScale = initScale
	
	def Preview():
		if pair != null:
			Debug.DrawLine(transform.position, pair.transform.position, Color.white)

	def OnTriggerEnter2D(collision as Collider2D):
		if pair != null:
			if not toNotTeleport.Contains(collision.gameObject):
				if affectsOnlyPlayer:
					if References.Player != null:
						if collision.gameObject == References.Player.gameObject:
							TeleportStart(collision.gameObject)
				else:
					TeleportStart(collision.gameObject)
			
	def OnTriggerExit2D(collision as Collider2D):
		if toNotTeleport.Contains(collision.gameObject):
			toNotTeleport.Remove(collision.gameObject)

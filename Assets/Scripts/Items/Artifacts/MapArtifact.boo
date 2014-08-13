import UnityEngine

class MapArtifact (Artifact): 

	override def ApplyArtifact():
		super.ApplyArtifact()
		
		if CurrentState != PickedUp:
			CurrentState = PickedUp
			AudioPlayer.Play("SFX_Artifact_Touch")
		else:
			References.Player.hasMap = true
			References.Map.UpdateMap()
			SpawnInfoText("THE MAP", References.Player.gameObject, Color.white, ColorMode.Normal, 4)
			AudioPlayer.Play("SFX_Artifact_Pickup")
		
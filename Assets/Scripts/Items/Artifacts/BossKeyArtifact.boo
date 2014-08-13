import UnityEngine

class BossKeyArtifact (Artifact): 

	override def ApplyArtifact():
		super.ApplyArtifact()
		
		if CurrentState != PickedUp:
			CurrentState = PickedUp
			AudioPlayer.Play("SFX_Artifact_Touch")
		else:
			References.Player.hasBossKey = true
			SpawnInfoText("THE BOSS KEY", References.Player.gameObject, Color.white, ColorMode.Normal, 4)
			AudioPlayer.Play("SFX_Artifact_Pickup")

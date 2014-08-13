//import UnityEngine;
//import System.Collections.Generic
//
//class AudioBridge (MonoBehaviour):
//
//	static Instance as AudioBridge
//	
//	def Awake():
//		Instance = self
//
//	static def Play(soundName as string, sourceObject as GameObject, delay as single, syncMode as string) as AudioSource:
//		audioSources as List[of AudioSource] = List[of AudioSource]()
//		Instance.gameObject.SendMessage("BeginInfo")
//		Instance.gameObject.SendMessage("AddSoundName", soundName)
//		Instance.gameObject.SendMessage("AddSourceObject", sourceObject)
//		Instance.gameObject.SendMessage("AddDelay", delay)
//		Instance.gameObject.SendMessage("AddSyncMode", syncMode)
//		Instance.gameObject.SendMessage("EndPlaySoundNameInfo", audioSources)
//		return audioSources[0]
//		
//	static def PlayContainer(containerName as string, sourceObject as GameObject, delay as single, syncMode as string) as List[of AudioSource]:
//		audioSources as List[of List[of AudioSource]] = List[of List[of AudioSource]]()
//		Instance.gameObject.SendMessage("BeginInfo")
//		Instance.gameObject.SendMessage("AddContainer", containerName)
//		Instance.gameObject.SendMessage("AddSourceObject", sourceObject)
//		Instance.gameObject.SendMessage("AddDelay", delay)
//		Instance.gameObject.SendMessage("AddSyncMode", syncMode)
//		Instance.gameObject.SendMessage("EndPlayContainerInfo", audioSources)
//		return audioSources[0]
//		
//	static def Stop(audioSource as AudioSource, delay as single):
//		Instance.gameObject.SendMessage("BeginInfo")
//		Instance.gameObject.SendMessage("AddDelay", delay)
//		Instance.gameObject.SendMessage("EndStopAudioSourceInfo", audioSource)

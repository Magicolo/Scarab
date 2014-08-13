//using UnityEngine;
//using System.Collections;
//using System.Collections.Generic;
//
//public class AudioBridge : MonoBehaviour {
//
//	AudioBridgeInfo info;
//	
//	public void BeginInfo(){
//		info = new AudioBridgeInfo();
//	}
//	
//	public void AddSoundName(string soundName){
//		info.soundName = soundName;
//	}
//	
//	public void AddContainer(string containerName){
//		info.container = AudioPlayer.Containers[containerName];
//	}
//	
//	public void AddSourceObject(GameObject sourceObject){
//		info.sourceObject = sourceObject;
//	}
//	
//	public void AddDelay(float delay){
//		info.delay = delay;
//	}
//	
//	public void AddSyncMode(string syncMode){
//		if (syncMode == "None") info.syncMode = AudioPlayer.SyncMode.None;
//		else if (syncMode == "Beat") info.syncMode = AudioPlayer.SyncMode.Beat;
//		else if (syncMode == "Measure") info.syncMode = AudioPlayer.SyncMode.Measure;
//	}
//	
//	public void EndPlaySoundNameInfo(List<AudioSource> audioSources){
//		audioSources.Add(AudioPlayer.Play(info.soundName, info.sourceObject, info.delay, info.syncMode));
//	}
//	
//	public void EndPlayContainerInfo(List<List<AudioSource>> audioSources){
//		audioSources.Add(AudioPlayer.Play(info.container, info.sourceObject, info.delay, info.syncMode));
//	}
//	
//	public void EndStopAudioSourceInfo(AudioSource audioSource){
//		AudioPlayer.Stop(audioSource, info.delay);
//	}
//}
//
//public class AudioBridgeInfo {
//	
//	public string soundName;
//	public AudioPlayer.Container container;
//	public GameObject sourceObject = null;
//	public float delay = 0;
//	public AudioPlayer.SyncMode syncMode = AudioPlayer.SyncMode.None;
//}

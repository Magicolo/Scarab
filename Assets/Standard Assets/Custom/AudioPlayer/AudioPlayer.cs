using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// TODO Get rid of the click when pausing/resuming sounds
// TODO Add a loop option on containers
// TODO Add automatic ducking
// TODO Make custom audio FX; or use open source ones if available?
// TODO Ability to make more than one version of each sound in the inspector
// TODO Test on other versions on Unity
// TODO Add an optionnal AnimationCurve parameter to the SetVolume() function
// TODO Add more RTPC parameters

[ExecuteInEditMode]
public class AudioPlayer : MonoBehaviour {

	public enum SyncMode{None, Beat, Measure};
	public string audioClipsPath;
	[Range(0, 100)] public float masterVolume = 100;
	[Range(1, 64)] public int maxVoices = 32;
	public TempoSettings tempoSettings;
	public bool showTempoSettings = false;
	public Container[] containers;
	public bool showContainers = false;
	public RTPC[] rTPCs;
	public bool showRTPCs = false;
	public AudioBus[] buses;
	public bool showBuses = false;
	
	public string[] keys;
	public AudioInfo[] values;
	public AudioClip[] audioClips;
	public AudioClip[] pAudioClips;
	public AudioCoroutineHolder coroutineHolder;
	
	Dictionary<string, AudioInfo> audioDict;
	float pMasterVolume;
	TempoSettings pTempoSettings;
	
	static public Dictionary<string, Container> Containers;
	static public Dictionary<string, float> Buses;
	static public Dictionary<string, float> RTPCs;
	static public Dictionary<string, AudioInfo> AudioInfos;
	static public AudioPlayer Instance;
	
	static Dictionary<GameObject, AudioCoroutineHolder> ActiveAudioSources;
	static List<AudioSource> ActiveVoices;
	static Dictionary<GameObject, AudioGainManager> GainManagerDict;
	static List<GameObject> InactiveAudioSources;
	static Dictionary<string, float> pBuses;
	static Dictionary<string, float> pRTPCs;
	static Dictionary<string, RTPC> RTPCDict;
	static Dictionary<string, List<AudioSource>> RTPCAudioDict;
	static List<ArrayList> ToPlayOnNextBeat;
	static List<ArrayList> ToPlayOnNextMeasure;
	
	void Awake(){
		if (Application.isPlaying){
			SetReferences();
		}
	}
	
	void Start(){
		if (!Application.isPlaying){
			this.SetExecutionOrder(-10);
			gameObject.DisconnectPrefab();
			
			if (FindObjectsOfType<AudioPlayer>().Length > 1){
				Debug.LogError("There can only be one AudioPlayer.");
				DestroyImmediate(gameObject);
			}
		}
		else StartMetronome();
	}
	
	void Update(){
		if (!Application.isPlaying){
			transform.position = Vector3.zero;
			transform.rotation = Quaternion.identity;
			transform.localScale = Vector3.one;
			
			SetReferences();
			DestroyHierarchy();
			
			if (audioClips != null){
				foreach (AudioClip audioClip in audioClips){
					if (audioClip != null) CreateHierarchy(audioClip, audioClip.name, transform);
				}
				FreezeHierarchy();
			}
			
			keys = new List<string>(audioDict.Keys).ToArray();
			values = new List<AudioInfo>(audioDict.Values).ToArray();
			BuildAudioDict();
			BuildContainerDict();
		}
		else {
			UpdateTempoSettings();
			UpdateBuses();
			UpdateRTPCs();
			LimitVoices();
		}
	}
	
	void LateUpdate(){
		if (!Application.isPlaying){
			SetReferences();
		}
	}
	
	void UpdateTempoSettings(){
		if (tempoSettings.changed){
			TempoSettings.BeatsPerMinute = tempoSettings.beatsPerMinute;
			TempoSettings.BeatsPerMeasure = tempoSettings.beatsPerMeasure;
			TempoSettings.BeatDuration = 60D / TempoSettings.BeatsPerMinute;
			TempoSettings.MeasureDuration = (60D / TempoSettings.BeatsPerMinute) * TempoSettings.BeatsPerMeasure;
			tempoSettings.changed = false;
		}
	}
	
	void UpdateBuses(){
		foreach (AudioBus bus in buses){
			if (bus.changed){
				Buses[bus.name] = bus.volume;
				bus.changed = false;
			}
			else {
				bus.volume = Buses[bus.name];
			}
		}
		
		bool updateBusVolume = false;
		if (pMasterVolume != masterVolume){
			updateBusVolume = true;
			pMasterVolume = masterVolume;
		}
		if (pBuses == null){
			pBuses = new Dictionary<string, float>();
			foreach (string key in Buses.Keys){
				pBuses[key] = Mathf.Infinity;
			}
		}
		foreach (string key in new List<string>(Buses.Keys)){
			if (pBuses.ContainsKey(key)){
				if (pBuses[key] != Buses[key]){
					Buses[key] = Mathf.Clamp(Buses[key], 0, 100);
					updateBusVolume = true;
				}
			}
		}
		if (updateBusVolume) UpdateBusVolume();
	}
	
	void UpdateRTPCs(){
		foreach (RTPC rtpc in rTPCs){
			if (rtpc.changed){
				RTPCs[rtpc.name] = rtpc.defaultValue;
				rtpc.changed = false;
			}
		}
		
		if (pRTPCs == null){
			pRTPCs = new Dictionary<string, float>();
			foreach (string key in RTPCs.Keys){
				pRTPCs[key] = Mathf.Infinity;
			}
		}
		foreach (string key in new List<string>(RTPCs.Keys)){
			if (pRTPCs.ContainsKey(key)){
				if (pRTPCs[key] != RTPCs[key]){
					RTPC rtpcInfo = RTPCDict[key];
					RTPCs[key] = Mathf.Clamp(RTPCs[key], rtpcInfo.minValue, rtpcInfo.maxValue);
					foreach (AudioSource audioSource in RTPCAudioDict[key]){
						if (audioSource.gameObject.activeSelf){
							AudioInfo audioInfo = AudioInfos[audioSource.clip.name];
							foreach (AudioInfo.RTPCParameter parameter in audioInfo.RTPCDict[key].parameters){
								if (parameter.enabled){
									ApplyRTPC(audioSource, parameter, RTPCs[key], rtpcInfo, audioInfo);
								}
							}
						}
					}
				}
		    }
			else {
				pRTPCs[key] = Mathf.Infinity;
			}
		}
		Dictionary<string, float> newDict = new Dictionary<string, float>();
		foreach (KeyValuePair<string, float> pair in RTPCs){
			newDict[pair.Key] = pair.Value;
		}
		pRTPCs = newDict;
	}
	
	void LimitVoices(){
		while (ActiveVoices.Count > maxVoices){
			for (int i = 0; i < ActiveVoices.Count; i++){
				AudioSource audioSource = ActiveVoices[i];
				AudioInfo audioInfo = AudioInfos[audioSource.clip.name];
				if (!audioInfo.doNotKill){
					float initFadeOut = audioInfo.fadeOut;
					audioInfo.fadeOut = 0.1F;
					Stop(audioSource);
					audioInfo.fadeOut = initFadeOut;
					ActiveVoices.RemoveAt(i);
					break;
				}
			}
		}
	}
	
	void CreateHierarchy(AudioClip audioClip, string name, Transform parent){
		GameObject newGameObject;
		string gameObjectName;
		string[] splitName = name.Split('_');

		if (!string.IsNullOrEmpty(splitName[0])) gameObjectName = splitName[0];
		else if (!string.IsNullOrEmpty(name)) gameObjectName = name;
		else return;
		
		foreach (Transform child in parent.GetChildren()){
			if (child.name == gameObjectName){
				if (splitName.Length > 1){
					CreateHierarchy(audioClip, name.TrimStart(gameObjectName.ToCharArray()).TrimStart('_'), child);
				}
				return;
			}
		}
		
		newGameObject = new GameObject();
		newGameObject.name = gameObjectName;
		newGameObject.transform.parent = parent;
		newGameObject.transform.localPosition = Vector3.zero;
		if (splitName.Length > 1){
			CreateHierarchy(audioClip, name.TrimStart(gameObjectName.ToCharArray()).TrimStart('_'), newGameObject.transform);
		}
		else {
			AudioInfo audioInfo = newGameObject.AddComponent<AudioInfo>();
			audioInfo.clip = audioClip;
			if (audioDict.ContainsKey(audioClip.name)){
				AudioInfo sound = audioDict[audioClip.name];
				
				audioInfo.fadeIn = sound.fadeIn;
				audioInfo.fadeOut = sound.fadeOut;
				audioInfo.randomVolume = sound.randomVolume;
				audioInfo.randomPitch = sound.randomPitch;
				audioInfo.delay = sound.delay;
				audioInfo.buses = sound.buses;
				audioInfo.effects = sound.effects;
				audioInfo.rTPCs = sound.rTPCs;
				
				audioInfo.clip = sound.clip;
				audioInfo.mute = sound.mute;
				audioInfo.bypassEffects = sound.bypassEffects;
				audioInfo.bypassListenerEffects = sound.bypassListenerEffects;
				audioInfo.bypassReverbZones = sound.bypassReverbZones;
				audioInfo.playOnAwake = sound.playOnAwake;
				audioInfo.loop = sound.loop;
				audioInfo.priority = sound.priority;
				audioInfo.volume = sound.volume;
				audioInfo.pitch = sound.pitch;
				audioInfo.dopplerLevel = sound.dopplerLevel;
				audioInfo.rolloffMode = sound.rolloffMode;
				audioInfo.minDistance = sound.minDistance;
				audioInfo.panLevel = sound.panLevel;
				audioInfo.spread = sound.spread;
				audioInfo.maxDistance = sound.maxDistance;
				audioInfo.pan = sound.pan;
			}
			audioDict[audioClip.name] = audioInfo;
		}
	}
	
	void DestroyHierarchy(){
		if (pAudioClips != null && audioClips != null){
			if (pAudioClips.Length != audioClips.Length){
				transform.DestroyImmediateChildren();
			}
			else {
				for (int i = 0; i < pAudioClips.Length; i++){
					if (pAudioClips[i] == null || audioClips[i] == null){
						transform.DestroyImmediateChildren();
						break;
					}
					else if (pAudioClips[i].name != audioClips[i].name){
						transform.DestroyImmediateChildren();
						break;
					}
				}
			}
		}
		pAudioClips = audioClips;
	}
	
	void FreezeHierarchy(){
		transform.hideFlags = HideFlags.HideInInspector;
		foreach (Transform child in transform.GetChildrenRecursive()){
			child.hideFlags = HideFlags.HideInInspector;
		}
	}
	
	void SetReferences(){
		Instance = this;
		GainManagerDict = new Dictionary<GameObject, AudioGainManager>();
		ActiveAudioSources = new Dictionary<GameObject, AudioCoroutineHolder>();
		ActiveVoices = new List<AudioSource>();
		InactiveAudioSources = new List<GameObject>();
		TempoSettings.BeatsPerMinute = tempoSettings.beatsPerMinute;
		TempoSettings.BeatsPerMeasure = tempoSettings.beatsPerMeasure;
		ToPlayOnNextBeat = new List<ArrayList>();
		ToPlayOnNextMeasure = new List<ArrayList>();
	
		if (!string.IsNullOrEmpty(audioClipsPath)){
			AudioClip[] clips;
			clips = Resources.LoadAll<AudioClip>(audioClipsPath);
			if (audioClips != clips && clips.Length != 0) audioClips = clips;
			audioClips = Sort(audioClips);
		}
		
		if (gameObject.GetComponent<AudioCoroutineHolder>() == null){
			coroutineHolder = gameObject.AddComponent<AudioCoroutineHolder>();
		}
		else coroutineHolder = gameObject.GetComponent<AudioCoroutineHolder>();
		
		BuildAudioDict();
		BuildContainerDict();
		BuildBusDict();
		BuildRTPCDict();
	}
	
	void BuildAudioDict(){
		audioDict = new Dictionary<string, AudioInfo>();
		if (keys != null){
			for (int i = 0; i < keys.Length; i++){
				if (keys[i] != null && values[i] != null){
					audioDict[keys[i]] = values[i];
				}
			}
		}
		AudioInfos = audioDict;
	}
	
	void BuildContainerDict(){
		Containers = new Dictionary<string, Container>();
		if (containers != null){
			for (int i = 0; i < containers.Length; i++){
				Container container = containers[i];
				Containers[container.name] = container;
				container.BuildIDDict();
			}
		}
	}
	
	void BuildBusDict(){
		Buses = new Dictionary<string, float>();
		if (buses != null){
			foreach (AudioBus bus in buses){
				Buses[bus.name] = bus.volume;
			}
		}
	}
	
	void BuildRTPCDict(){
		RTPCs = new Dictionary<string, float>();
		RTPCDict = new Dictionary<string, RTPC>();
		RTPCAudioDict = new Dictionary<string, List<AudioSource>>();
		
		if (rTPCs != null){
			foreach (RTPC rtpc in rTPCs){
				RTPCs[rtpc.name] = rtpc.defaultValue;
				RTPCDict[rtpc.name] = rtpc;
				RTPCAudioDict[rtpc.name] = new List<AudioSource>();
			}
		}
	}
	
	static public AudioSource Play(AudioSource audioSource, float delay = 0, AudioPlayer.SyncMode syncMode = AudioPlayer.SyncMode.None){
		AudioInfo audioInfo = null;
		
		if (audioSource.clip != null){
			if (AudioInfos.ContainsKey(audioSource.clip.name)) audioInfo = AudioInfos[audioSource.clip.name];
		}
		if (audioInfo != null) delay = (float) (GetDelayToSync(syncMode) + GetAdjustedDelay(audioInfo.delay, audioInfo.syncMode) + GetAdjustedDelay(delay, syncMode));
		else delay = (float) (GetDelayToSync(syncMode) + GetAdjustedDelay(delay, syncMode));
		
		if (ActiveAudioSources.ContainsKey(audioSource.gameObject)){
			SchedulePlay(audioSource, audioInfo, delay);
			
			if (audioInfo != null){
				if (!audioInfo.loop){
					ActiveAudioSources[audioSource.gameObject].AddCoroutine("StopAfterDelay", StopAfterDelay(audioSource, audioInfo, audioSource.clip.length / audioSource.pitch - audioInfo.fadeOut + delay));
				}
			}
		}
		
		return audioSource;
	}
	
	static public AudioSource Play(AudioClip audioClip, AudioInfo audioInfo, GameObject sourceObject = null, float delay = 0, AudioPlayer.SyncMode syncMode = AudioPlayer.SyncMode.None){
		AudioSource audioSource = GetAudioSource(audioInfo, sourceObject, audioClip);
		
		if (audioInfo.syncMode == AudioPlayer.SyncMode.Beat){
			ToPlayOnNextBeat.Add(new ArrayList(3){audioSource, delay, syncMode});
			return audioSource;
		}
		else if (audioInfo.syncMode == AudioPlayer.SyncMode.Measure){
			ToPlayOnNextMeasure.Add(new ArrayList(3){audioSource, delay, syncMode});
			return audioSource;
		}
		else {
			return Play(audioSource, delay, syncMode);
		}
	}
	
	static public AudioSource Play(AudioInfo audioInfo, GameObject sourceObject = null, float delay = 0, AudioPlayer.SyncMode syncMode = AudioPlayer.SyncMode.None){
		AudioSource audioSource = GetAudioSource(audioInfo, sourceObject);
		
		if (audioInfo.syncMode == AudioPlayer.SyncMode.Beat){
			ToPlayOnNextBeat.Add(new ArrayList(3){audioSource, delay, syncMode});
			return audioSource;
		}
		else if (audioInfo.syncMode == AudioPlayer.SyncMode.Measure){
			ToPlayOnNextMeasure.Add(new ArrayList(3){audioSource, delay, syncMode});
			return audioSource;
		}
		else {
			return Play(audioSource, delay, syncMode);
		}
	}		
	
	static public List<AudioSource> Play(AudioInfo[] audioInfos, GameObject sourceObject = null, float delay = 0, AudioPlayer.SyncMode syncMode = AudioPlayer.SyncMode.None){
		List<AudioSource> audioSources = new List<AudioSource>();
		for (int i = 0; i < audioInfos.Length; i++){
			audioSources.Add(Play(audioInfos[i], sourceObject, delay, syncMode));
		}
		return audioSources;
	}
	
	static public AudioSource Play(string soundName){
		return Play(AudioInfos[soundName], null, 0, AudioPlayer.SyncMode.None);
	}
	
	static public AudioSource Play(string soundName, GameObject sourceObject, float delay = 0, AudioPlayer.SyncMode syncMode = AudioPlayer.SyncMode.None){
		return Play(AudioInfos[soundName], sourceObject, delay, syncMode);
	}
	
	static public List<AudioSource> Play(string[] soundNames, GameObject sourceObject = null, float delay = 0, AudioPlayer.SyncMode syncMode = AudioPlayer.SyncMode.None){
		List<AudioSource> audioSources = new List<AudioSource>();
		for (int i = 0; i < soundNames.Length; i++){
			audioSources.Add(Play(soundNames[i], sourceObject, delay, syncMode));
		}
		return audioSources;
	}
	
	static public List<AudioSource> Play(Container container, GameObject sourceObject = null, float delay = 0, AudioPlayer.SyncMode syncMode = AudioPlayer.SyncMode.None){
		List<AudioSource> audioSources = new List<AudioSource>();
		
		if (container.containerType == Container.ContainerTypes.MixContainer){
			foreach (SubContainer subContainer in container.sources){
				PlaySubContainer(audioSources, subContainer, container, sourceObject, delay, syncMode);
			}
		}
		else if (container.containerType == Container.ContainerTypes.RandomContainer){
			PlaySubContainer(audioSources, GetRandomContainer(container.sources), container, sourceObject, delay, syncMode);
		}
		else if (container.containerType == Container.ContainerTypes.SequenceContainer){
			Instance.coroutineHolder.AddCoroutine("PlaySequence" + audioSources.GetHashCode(), PlaySequence(audioSources, container.sources, container, sourceObject, delay, syncMode));
		}
		return audioSources;
	}
	
	static public List<AudioSource> PlayRepeating(float repeatRate, AudioInfo audioInfo, GameObject sourceObject = null, float delay = 0, AudioPlayer.SyncMode syncMode = AudioPlayer.SyncMode.None){
		List<AudioSource> audioSources = new List<AudioSource>();
		Instance.coroutineHolder.AddCoroutine("PlayRepeatedly" + audioSources.GetHashCode(), PlayRepeatedly(audioSources, repeatRate, audioInfo, sourceObject, delay, syncMode));
		return audioSources;
	}
	
	static public List<AudioSource> PlayRepeating(float repeatRate, AudioInfo[] audioInfos, GameObject sourceObject = null, float delay = 0, AudioPlayer.SyncMode syncMode = AudioPlayer.SyncMode.None){
		List<AudioSource> audioSources = new List<AudioSource>();
		Instance.coroutineHolder.AddCoroutine("PlayRepeatedly" + audioSources.GetHashCode(), PlayRepeatedly(audioSources, repeatRate, audioInfos, sourceObject, delay, syncMode));
		return audioSources;
	}
	
	static public List<AudioSource> PlayRepeating(float repeatRate, string soundName){
		return PlayRepeating(repeatRate, AudioInfos[soundName], null, 0, SyncMode.None);
	}
	
	static public List<AudioSource> PlayRepeating(float repeatRate, string soundName, GameObject sourceObject, float delay = 0, AudioPlayer.SyncMode syncMode = AudioPlayer.SyncMode.None){
		return PlayRepeating(repeatRate, AudioInfos[soundName], sourceObject, delay, syncMode);
	}
	
	static public List<AudioSource> PlayRepeating(float repeatRate, string[] soundNames, GameObject sourceObject = null, float delay = 0, AudioPlayer.SyncMode syncMode = AudioPlayer.SyncMode.None){
		AudioInfo[] audioInfos = new AudioInfo[soundNames.Length];
		for (int i = 0; i < soundNames.Length; i++){
			audioInfos[i] = AudioInfos[soundNames[i]];
		}
		return PlayRepeating(repeatRate, audioInfos, sourceObject, delay, syncMode);
	}
	
	static public List<AudioSource> PlayRepeating(float repeatRate, Container container, GameObject sourceObject = null, float delay = 0, AudioPlayer.SyncMode syncMode = AudioPlayer.SyncMode.None){
		List<AudioSource> audioSources = new List<AudioSource>();
		Instance.coroutineHolder.AddCoroutine("PlayRepeatedly" + audioSources.GetHashCode(), PlayRepeatedly(audioSources, repeatRate, container, sourceObject, delay, syncMode));
		return audioSources;
	}
	
	static public List<AudioSource> PlayRepeating(float repeatRate, Sampler.Note note, GameObject sourceObject = null, float delay = 0, AudioPlayer.SyncMode syncMode = AudioPlayer.SyncMode.None){
		List<AudioSource> audioSources = new List<AudioSource>();
		Instance.coroutineHolder.AddCoroutine("PlayRepeatedly" + audioSources.GetHashCode(), PlayRepeatedly(audioSources, repeatRate, note, sourceObject, delay, syncMode));
		return audioSources;
	}
	
	static public void Pause(AudioSource audioSource, float delay = 0){
		if (audioSource != null){
			if (audioSource.gameObject.activeSelf){
				ActiveAudioSources[audioSource.gameObject].AddCoroutine("PauseAfterDelay", PauseAfterDelay(audioSource, delay));
			}
		}
	}
	
	static public void Pause(List<AudioSource> audioSources, float delay = 0){
		if (audioSources != null){
			Instance.coroutineHolder.AddCoroutine("PauseAfterDelay" + audioSources.GetHashCode(), PauseAfterDelay(audioSources, delay));
		}
	}
	
	static public void PauseAll(float delay = 0){
		foreach (GameObject GO in new List<GameObject>(ActiveAudioSources.Keys)){
			Pause(GO.audio, delay);
		}
		Instance.coroutineHolder.PauseAllCoroutinesBut("PlayMetronome");
	}
	
	static public void Resume(AudioSource audioSource, float delay = 0){
		if (audioSource != null){
			if (audioSource.gameObject.activeSelf){
				ActiveAudioSources[audioSource.gameObject].AddCoroutine("ResumeAfterDelay", ResumeAfterDelay(audioSource, delay));
			}
		}
	}
	
	static public void Resume(List<AudioSource> audioSources, float delay = 0){
		if (audioSources != null){
			Instance.coroutineHolder.AddCoroutine("ResumeAfterDelay" + audioSources.GetHashCode(), ResumeAfterDelay(audioSources, delay));
		}
	}
	
	static public void ResumeAll(float delay = 0){
		foreach (GameObject GO in new List<GameObject>(ActiveAudioSources.Keys)){
			Resume(GO.audio, delay);
		}
		Instance.coroutineHolder.ResumeAllCoroutines();
	}
	
	static public void Stop(AudioSource audioSource, float delay = 0){
		if (audioSource != null){
			if (audioSource.clip != null){
				if (audioSource.gameObject.activeSelf){
					AudioInfo audioInfo;
					if (AudioInfos.ContainsKey(audioSource.clip.name)) audioInfo = AudioInfos[audioSource.clip.name];
					else audioInfo = new AudioInfo();
					ActiveAudioSources[audioSource.gameObject].AddCoroutine("StopAfterDelay", StopAfterDelay(audioSource, audioInfo, delay, true));
				}
			}
		}
	}
	
	static public void Stop(List<AudioSource> audioSources, float delay = 0){
		if (audioSources != null){
			foreach (AudioSource audioSource in audioSources){
				Stop(audioSource, delay);
			}
			Instance.coroutineHolder.RemoveCoroutines("PlayRepeatedly" + audioSources.GetHashCode());
			Instance.coroutineHolder.RemoveCoroutines("PlaySequence" + audioSources.GetHashCode());
			audioSources.Clear();
		}
	}
	
	static public void StopAll(float delay = 0){
		foreach (GameObject GO in new List<GameObject>(ActiveAudioSources.Keys)){
			Stop(GO.audio, delay);
		}
		ToPlayOnNextBeat.Clear();
		ToPlayOnNextMeasure.Clear();
		Instance.coroutineHolder.RemoveAllCoroutinesBut("PlayMetronome");
	}
	
	static public void StopImmediate(AudioSource audioSource, float delay = 0){
		if (audioSource != null){
			if (audioSource.clip != null){
				AudioInfo audioInfo;
				if (AudioInfos.ContainsKey(audioSource.clip.name)) audioInfo = AudioInfos[audioSource.clip.name];
				else audioInfo = new AudioInfo();
				
				if (audioSource.gameObject.activeSelf){
					ActiveAudioSources[audioSource.gameObject].AddCoroutine("StopAfterDelay", StopAfterDelay(audioSource, audioInfo, delay, true, false));
			    }
			}
		}
	}
	
	static public void StopImmediate(List<AudioSource> audioSources, float delay = 0){
		if (audioSources != null){
			foreach (AudioSource audioSource in audioSources){
				StopImmediate(audioSource, delay);
			}
			Instance.coroutineHolder.RemoveCoroutines("PlayRepeatedly" + audioSources.GetHashCode());
			Instance.coroutineHolder.RemoveCoroutines("PlaySequence" + audioSources.GetHashCode());
			audioSources.Clear();
		}
	}
	
	static public void StopAllImmediate(float delay = 0){
		foreach (GameObject GO in new List<GameObject>(ActiveAudioSources.Keys)){
			StopImmediate(GO.audio, delay);
		}
		ToPlayOnNextBeat.Clear();
		ToPlayOnNextMeasure.Clear();
		Instance.coroutineHolder.RemoveAllCoroutinesBut("PlayMetronome");
	}
	
	static public void SetVolume(AudioSource audioSource, float targetVolume, float time = 0){
		if (audioSource != null){
			if (ActiveAudioSources.ContainsKey(audioSource.gameObject)){
				ActiveAudioSources[audioSource.gameObject].RemoveCoroutines("FadeVolume");
				ActiveAudioSources[audioSource.gameObject].AddCoroutine("FadeVolume", FadeVolume(audioSource, targetVolume / 100, time));
		    }
			else {
				Instance.coroutineHolder.RemoveCoroutines("FadeVolume" + audioSource.GetHashCode());
				Instance.coroutineHolder.AddCoroutine("FadeVolume" + audioSource.GetHashCode(), FadeVolume(audioSource, targetVolume / 100, time));
			}
		}
	}
	
	static public void SetVolume(List<AudioSource> audioSources, float targetVolume, float time = 0){
		if (audioSources != null){
			foreach (AudioSource audioSource in audioSources){
				SetVolume(audioSource, targetVolume, time);
			}
		}
	}
	
	static public void SetMasterVolume(float targetVolume, float time = 0){
		Instance.coroutineHolder.RemoveCoroutines("FadeMasterVolume");
		Instance.coroutineHolder.AddCoroutine("FadeMasterVolume", FadeMasterVolume(targetVolume, time));
	}
	
	static public void StartMetronome(){
		Instance.coroutineHolder.RemoveCoroutines("PlayMetronome");
		Instance.coroutineHolder.AddCoroutine("PlayMetronome", PlayMetronome());
	}
	
	static public void PauseMetronome(){
		Instance.coroutineHolder.PauseCoroutines("PlayMetronome");
	}
	
	static public void ResumeMetronome(){
		Instance.coroutineHolder.ResumeCoroutines("PlayMetronome");
	}
	
	static public void StopMetronome(){
		Instance.coroutineHolder.RemoveCoroutines("PlayMetronome");
	}
	
	static void SchedulePlay(AudioSource audioSource, AudioInfo audioInfo, float delay = 0, bool fadeIn = true){
		if (audioInfo == null){
			audioSource.enabled = true;
			audioSource.PlayScheduled(AudioSettings.dspTime + delay);
		}
		else {
			if (audioSource.gameObject.activeSelf){
				if (fadeIn && audioInfo.fadeIn > 0){
					float targetVolume = audioSource.volume;
					audioSource.volume = 0;
					ActiveAudioSources[audioSource.gameObject].AddCoroutine("FadeVolume", FadeVolume(audioSource, targetVolume, audioInfo.fadeIn, false, audioInfo.fadeInCurve, "In", delay));
				}
				audioSource.enabled = true;
				audioSource.PlayScheduled(AudioSettings.dspTime + delay);
			}
		}
	}
	
	static void PlaySubContainer(List<AudioSource> audioSources, SubContainer subContainer, Container container, GameObject sourceObject = null, float delay = 0, AudioPlayer.SyncMode syncMode = AudioPlayer.SyncMode.None){
		delay = (float) (GetDelayToSync(syncMode) + GetAdjustedDelay(delay, syncMode));
		
		if (subContainer.sourceType == SubContainer.ContainerTypes.AudioSource){
			audioSources.Add(Play(subContainer.audioInfo, sourceObject, subContainer.delay + delay, subContainer.syncMode));
		}
		else if (subContainer.sourceType == SubContainer.ContainerTypes.Sampler){
			audioSources.Add(Sampler.Play(subContainer.instrumentName, subContainer.midiNote, subContainer.velocity, sourceObject, subContainer.delay + delay, subContainer.syncMode));
		}
		else if (subContainer.sourceType == SubContainer.ContainerTypes.MixContainer){
			for (int i = 0; i < subContainer.childrenLink.Count; i++){
				PlaySubContainer(audioSources, container.idDict[subContainer.childrenLink[i]], container, sourceObject, subContainer.delay + delay, subContainer.syncMode);
			}
		}
		else if (subContainer.sourceType == SubContainer.ContainerTypes.RandomContainer){
			PlaySubContainer(audioSources, GetRandomContainer(subContainer.sources), container, sourceObject, subContainer.delay + delay, subContainer.syncMode);
		}
		else if (subContainer.sourceType == SubContainer.ContainerTypes.SequenceContainer){
			Instance.coroutineHolder.AddCoroutine("PlaySequence" + audioSources.GetHashCode(), PlaySequence(audioSources, subContainer.sources, container, sourceObject, subContainer.delay + delay, subContainer.syncMode));
		}
	}
	
	static IEnumerator PlaySequence(List<AudioSource> audioSources, SubContainer[] sequence, Container container, GameObject sourceObject, float delay = 0, AudioPlayer.SyncMode syncMode = AudioPlayer.SyncMode.None){
		delay = (float) (GetDelayToSync(syncMode) + GetAdjustedDelay(delay, syncMode));
		float delayCounter = 0;
		while (delayCounter < delay){
			yield return new WaitForSeconds(0);
			delayCounter += Time.deltaTime;
		}
		
		for (int i = 0; i < sequence.Length; i++){
			for (int j = 0; j < sequence[i].repeat; j++){
				RemoveInactiveAudioSources(audioSources);
				
				AudioSource audioSource = null;
				float waitTime = 0;
				
				if (sequence[i].sourceType == SubContainer.ContainerTypes.AudioSource){
					audioSource = Play(sequence[i].audioInfo, sourceObject, sequence[i].delay, sequence[i].syncMode);
					audioSources.Add(audioSource);
					
					AudioInfo audioInfo = AudioInfos[audioSource.clip.name];
					waitTime = (float) (audioSource.clip.length / audioSource.pitch - audioInfo.fadeIn + GetAdjustedDelay(audioInfo.delay, audioInfo.syncMode) + GetAdjustedDelay(sequence[i].delay, sequence[i].syncMode));
				}
				else if (sequence[i].sourceType == SubContainer.ContainerTypes.Sampler){
					audioSource = Sampler.Play(sequence[i].instrumentName, sequence[i].midiNote, sequence[i].velocity, sourceObject, sequence[i].delay, sequence[i].syncMode);
					audioSources.Add(audioSource);
					
					AudioInfo audioInfo = AudioInfos[audioSource.clip.name];
					waitTime = (float) (audioSource.clip.length / audioSource.pitch - audioInfo.fadeIn + GetAdjustedDelay(audioInfo.delay, audioInfo.syncMode) + GetAdjustedDelay(sequence[i].delay, sequence[i].syncMode));
				}
				else if (sequence[i].sourceType == SubContainer.ContainerTypes.MixContainer){
					List<AudioSource> tempAudioSources = new List<AudioSource>();
					for (int k = 0; k < sequence[i].childrenLink.Count; k++){
						PlaySubContainer(tempAudioSources, container.idDict[sequence[i].childrenLink[k]], container, sourceObject, sequence[i].delay, sequence[i].syncMode);
					}
					while (tempAudioSources.Count == 0){
						yield return new WaitForSeconds(0);
						if (audioSources.Count == 0) break;
					}
					foreach (AudioSource source in tempAudioSources){
						AudioInfo audioInfo = AudioInfos[source.clip.name];
						if ((float) (source.clip.length / source.pitch - audioInfo.fadeIn + GetAdjustedDelay(audioInfo.delay, audioInfo.syncMode) + GetAdjustedDelay(sequence[i].delay, sequence[i].syncMode)) > waitTime){
							waitTime = (float) (source.clip.length / source.pitch - audioInfo.fadeIn + GetAdjustedDelay(audioInfo.delay, audioInfo.syncMode) + GetAdjustedDelay(sequence[i].delay, sequence[i].syncMode));
						}
					}
					audioSources.AddRange(tempAudioSources);
				}
				else if (sequence[i].sourceType == SubContainer.ContainerTypes.RandomContainer){
					List<AudioSource> tempAudioSources = new List<AudioSource>();
					
					PlaySubContainer(tempAudioSources, GetRandomContainer(sequence[i].sources), container, sourceObject, sequence[i].delay, sequence[i].syncMode);
					
					foreach (AudioSource source in tempAudioSources){
						AudioInfo audioInfo = AudioInfos[source.clip.name];
						if ((float) (source.clip.length / source.pitch - audioInfo.fadeIn + GetAdjustedDelay(audioInfo.delay, audioInfo.syncMode) + GetAdjustedDelay(sequence[i].delay, sequence[i].syncMode)) > waitTime){
							waitTime = (float) (source.clip.length / source.pitch - audioInfo.fadeIn + GetAdjustedDelay(audioInfo.delay, audioInfo.syncMode) + GetAdjustedDelay(sequence[i].delay, sequence[i].syncMode));
						}
					}
					audioSources.AddRange(tempAudioSources);
				}
				else if (sequence[i].sourceType == SubContainer.ContainerTypes.SequenceContainer){
					IEnumerator coroutine = PlaySequence(audioSources, sequence[i].sources, container, sourceObject, sequence[i].delay, sequence[i].syncMode);
					while (coroutine.MoveNext()){
						yield return coroutine.Current;
						if (audioSources.Count == 0) break;
					}
				}
				
				float counter = 0;
				while (counter < waitTime){
					yield return new WaitForSeconds(0);
					counter += Time.deltaTime;
				}
				if (audioSources.Count == 0) break;
			}
		}
	}
	
	static IEnumerator PlayRepeatedly(List<AudioSource> audioSources, float repeatRate, object toPlay, GameObject sourceObject = null, float delay = 0, AudioPlayer.SyncMode syncMode = AudioPlayer.SyncMode.None){
		bool firstTime = true;
		double currentTime;
		double nextRepeatTime = 0;
		
		delay = (float) (GetDelayToSync(syncMode) + GetAdjustedDelay(delay, syncMode));
		
		while(true){
			currentTime = AudioSettings.dspTime;
			
			if (currentTime >= nextRepeatTime){
				if (firstTime){
					nextRepeatTime = AudioSettings.dspTime + delay;
					firstTime = false;
				}
				else {
					if (audioSources.Count == 0) break;
					while (nextRepeatTime < AudioSettings.dspTime){
						nextRepeatTime +=  GetAdjustedDelay(repeatRate, syncMode);
					}
				}
				
				RemoveInactiveAudioSources(audioSources);
				
				if (toPlay is AudioSource){
					audioSources.Add(Play(toPlay as AudioSource, (float) (nextRepeatTime - AudioSettings.dspTime)));
				}
				else if (toPlay is AudioInfo){
					audioSources.Add(Play(toPlay as AudioInfo, sourceObject, (float) (nextRepeatTime - AudioSettings.dspTime)));
				}
				else if (toPlay is AudioInfo[]){
					audioSources.AddRange(Play(toPlay as AudioInfo[], sourceObject, (float) (nextRepeatTime - AudioSettings.dspTime)));
				}
				else if (toPlay is string){
					audioSources.Add(Play(toPlay as string, sourceObject, (float) (nextRepeatTime - AudioSettings.dspTime)));
				}
				else if (toPlay is string[]){
					audioSources.AddRange(Play(toPlay as string[], sourceObject, (float) (nextRepeatTime - AudioSettings.dspTime)));
				}
				else if (toPlay is Container){
					audioSources.AddRange(Play(toPlay as Container, sourceObject, (float) (nextRepeatTime - AudioSettings.dspTime)));
				}
				else if (toPlay is Sampler.Note){
					audioSources.AddRange(Sampler.Play(toPlay as Sampler.Note, sourceObject, (float) (nextRepeatTime - AudioSettings.dspTime)));
				}
			}
			yield return new WaitForSeconds(0);
		}
	}
	
	static IEnumerator PauseAfterDelay(AudioSource audioSource, float delay = 0){
		float counter = 0;
		while (counter < delay){
			yield return new WaitForSeconds(0);
			counter += Time.deltaTime;
		}
		
		if (audioSource != null){
			if (audioSource.gameObject.activeSelf) audioSource.Pause();
			if (ActiveAudioSources.ContainsKey(audioSource.gameObject)) ActiveAudioSources[audioSource.gameObject].PauseAllCoroutines();
			Instance.coroutineHolder.PauseCoroutines("PlayRepeatedly" + audioSource.GetHashCode());
			Instance.coroutineHolder.PauseCoroutines("PlaySequence" + audioSource.GetHashCode());
		}
	}
	
	static IEnumerator PauseAfterDelay(List<AudioSource> audioSources, float delay = 0){
		float counter = 0;
		while (counter < delay){
			yield return new WaitForSeconds(0);
			counter += Time.deltaTime;
		}
		
		if (audioSources != null){
			foreach (AudioSource audioSource in audioSources){
				if (audioSource != null){
					if (audioSource.gameObject.activeSelf) audioSource.Pause();
					if (ActiveAudioSources.ContainsKey(audioSource.gameObject)) ActiveAudioSources[audioSource.gameObject].PauseAllCoroutines();
					Instance.coroutineHolder.PauseCoroutines("PlayRepeatedly" + audioSource.GetHashCode());
					Instance.coroutineHolder.PauseCoroutines("PlaySequence" + audioSource.GetHashCode());
				}
			}
			Instance.coroutineHolder.PauseCoroutines("PlayRepeatedly" + audioSources.GetHashCode());
			Instance.coroutineHolder.PauseCoroutines("PlaySequence" + audioSources.GetHashCode());
		}
	}
	
	static IEnumerator ResumeAfterDelay(AudioSource audioSource, float delay = 0){
		float counter = 0;
		while (counter < delay){
			yield return new WaitForSeconds(0);
			counter += Time.deltaTime;
		}
		
		if (audioSource != null){
			if (!audioSource.isPlaying){
				if (audioSource.gameObject.activeSelf) audioSource.Play();
				if (ActiveAudioSources.ContainsKey(audioSource.gameObject)) ActiveAudioSources[audioSource.gameObject].ResumeAllCoroutines();
				Instance.coroutineHolder.ResumeCoroutines("PlayRepeatedly" + audioSource.GetHashCode());
				Instance.coroutineHolder.ResumeCoroutines("PlaySequence" + audioSource.GetHashCode());
			}
		}
	}
	
	static IEnumerator ResumeAfterDelay(List<AudioSource> audioSources, float delay = 0){
		float counter = 0;
		while (counter < delay){
			yield return new WaitForSeconds(0);
			counter += Time.deltaTime;
		}
		
		if (audioSources != null){
			foreach (AudioSource audioSource in audioSources){
				if (audioSource != null){
					if (!audioSource.isPlaying){
						if (audioSource.gameObject.activeSelf) audioSource.Play();
						if (ActiveAudioSources.ContainsKey(audioSource.gameObject)) ActiveAudioSources[audioSource.gameObject].ResumeAllCoroutines();
						Instance.coroutineHolder.ResumeCoroutines("PlayRepeatedly" + audioSource.GetHashCode());
						Instance.coroutineHolder.ResumeCoroutines("PlaySequence" + audioSource.GetHashCode());
					}
				}
			}
			Instance.coroutineHolder.ResumeCoroutines("PlayRepeatedly" + audioSources.GetHashCode());
			Instance.coroutineHolder.ResumeCoroutines("PlaySequence" + audioSources.GetHashCode());
		}
	}
	
	static IEnumerator StopAfterDelay(AudioSource audioSource, AudioInfo audioInfo, float delay = 0, bool enforce = false, bool fadeOut = true){
		float delayCounter = 0;
		while (delayCounter < delay){
			yield return new WaitForSeconds(0);
			delayCounter += Time.deltaTime;
		}
		
		if (audioSource != null){
			if (fadeOut && audioInfo.fadeOut > 0){
				ActiveAudioSources[audioSource.gameObject].RemoveCoroutines("FadeVolume");
				ActiveAudioSources[audioSource.gameObject].AddCoroutine("FadeVolume", FadeVolume(audioSource, 0, audioInfo.fadeOut, enforce, audioInfo.fadeOutCurve, "Out"));
			}
			else {audioSource.volume = 0;}
		}
		while (audioSource != null){
			if (audioSource.volume == 0){
				audioSource.Stop();
				audioSource.enabled = false;
				RemoveAudioSource(audioSource);
				break;
			}
			else{
				yield return new WaitForSeconds(0);
			}
		}
	}
	
	static IEnumerator FadeVolume(AudioSource audioSource, float targetVolume, float time = 0, bool enforce = false, AnimationCurve curve = null, string fade = null, float delay = 0){
		float delayCounter = 0;
		while (delayCounter < delay){
			yield return new WaitForSeconds(0);
			delayCounter += Time.deltaTime;
		}
		
		if (audioSource != null){
			float counter = 0;
			float startVolume = audioSource.volume;
			float volumeDiff = targetVolume - startVolume;
			
			while (audioSource != null){
				if (enforce) yield return new WaitForEndOfFrame();
				if (audioSource != null){
					if (curve != null){
						float adjustedValue = 0;
						if (fade == "In"){
							adjustedValue = counter / time;
							if (adjustedValue < 1){
								float curveValue = curve.Evaluate(adjustedValue);
								audioSource.volume = startVolume + (curveValue * volumeDiff);
								counter += Time.deltaTime;
								yield return new WaitForSeconds(0);
							}
							else {
								audioSource.volume = targetVolume;
								break;
							}
						}
						else if (fade == "Out"){
							adjustedValue = 1 - (counter / time);
							if (adjustedValue > 0){
								float curveValue = curve.Evaluate(adjustedValue);
								audioSource.volume = startVolume + (curveValue * volumeDiff);
								counter += Time.deltaTime;
								yield return new WaitForSeconds(0);
							}
							else {
								audioSource.volume = targetVolume;
								break;
							}
						}
					}
					else if (Mathf.Abs(targetVolume - audioSource.volume) > Mathf.Abs((volumeDiff * Time.deltaTime) / time)){
						audioSource.volume += (volumeDiff * Time.deltaTime) / time;
						yield return new WaitForSeconds(0);
					}
					else {
						audioSource.volume = targetVolume;
						break;
					}
				}
			}
		}
	}
	
	static IEnumerator FadeMasterVolume(float targetVolume, float time = 0){
		float volumeDiff = targetVolume - Instance.masterVolume;
		
		while (true){
			if (Mathf.Abs(targetVolume - Instance.masterVolume) > Mathf.Abs((volumeDiff * Time.deltaTime) / time)){
				Instance.masterVolume += (volumeDiff * Time.deltaTime) / time;
				yield return new WaitForSeconds(0);
			}
			else {
				Instance.masterVolume = targetVolume;
				break;
			}
		}
	}
	
	static IEnumerator PlayMetronome(){
		TempoSettings.CurrentBeatTime = 0;
		TempoSettings.CurrentBeat = 0;
		TempoSettings.CurrentMeasureTime = 0;
		TempoSettings.NextBeatTime = AudioSettings.dspTime;
		TempoSettings.NextMeasureTime = AudioSettings.dspTime;
		TempoSettings.BeatDuration = 60D / TempoSettings.BeatsPerMinute;
		TempoSettings.MeasureDuration = (60D / TempoSettings.BeatsPerMinute) * TempoSettings.BeatsPerMeasure;
		double currentTime;
		
		while (true){
			currentTime = AudioSettings.dspTime;
			if (AudioSettings.dspTime >= TempoSettings.NextBeatTime){
				if (TempoSettings.CurrentBeat == 0){
					TempoSettings.CurrentMeasureTime = currentTime;
					TempoSettings.NextMeasureTime += TempoSettings.MeasureDuration;
					
					foreach (ArrayList playInfo in ToPlayOnNextMeasure){
						AudioSource audioSource = playInfo[0] as AudioSource;
						float delay = (float) playInfo[1];
						AudioPlayer.SyncMode syncMode = (AudioPlayer.SyncMode) playInfo[2];
						Play(audioSource, delay, syncMode);
					}
					ToPlayOnNextMeasure.Clear();
				}
				
				TempoSettings.CurrentBeatTime = currentTime;
				TempoSettings.CurrentBeat = (TempoSettings.CurrentBeat + 1) % TempoSettings.BeatsPerMeasure;
				TempoSettings.NextBeatTime += TempoSettings.BeatDuration;
				
				foreach (ArrayList playInfo in ToPlayOnNextBeat){
					AudioSource audioSource = playInfo[0] as AudioSource;
					float delay = (float) playInfo[1];
					AudioPlayer.SyncMode syncMode = (AudioPlayer.SyncMode) playInfo[2];
					Play(audioSource, delay, syncMode);
				}
				ToPlayOnNextBeat.Clear();
			}
			yield return new WaitForSeconds(0);
		}
	}
	
	static IEnumerator RemoveOnFinish(AudioSource audioSource){
		while (audioSource.isPlaying){
			yield return new WaitForSeconds(0);
		}
		Stop(audioSource);
	}
	
	static void RemoveAudioSource(AudioSource audioSource){
		if (!audioSource.isPlaying){
			audioSource.transform.parent = Instance.transform;
			audioSource.gameObject.SetActive(false);
			audioSource.gameObject.name = "Inactive AudioSource";
			InactiveAudioSources.Add(audioSource.gameObject);
			ActiveAudioSources[audioSource.gameObject].RemoveAllCoroutines();
			ActiveAudioSources.Remove(audioSource.gameObject);
			ActiveVoices.Remove(audioSource);
		}
		else {ActiveAudioSources[audioSource.gameObject].AddCoroutine("RemoveOnFinish", RemoveOnFinish(audioSource));}
	}
	
	static void RemoveInactiveAudioSources(List<AudioSource> audioSources){
		List<AudioSource> toRemove = new List<AudioSource>();
		foreach (AudioSource audioSource in audioSources){
			if (audioSource == null) toRemove.Add(audioSource);
			else if (audioSource.gameObject == null) toRemove.Add(audioSource);
			else if (!audioSource.gameObject.activeSelf) toRemove.Add(audioSource);
		}
		foreach (AudioSource audioSource in toRemove){
			audioSources.Remove(audioSource);
		}
	}
	
	static AudioSource GetAudioSource(AudioInfo audioInfo, GameObject sourceObject = null, AudioClip overrideClip = null){
		GameObject GO;
		AudioSource audioSource;
			
		GO = GetGameObject(sourceObject);
		if (GO.audio != null){audioSource = GO.audio;}
		else {audioSource = GO.AddComponent<AudioSource>();}
		GO.name = "AudioSource: " + audioInfo.clip.name;
		SetAudioSource(audioSource, audioInfo, overrideClip);
		ActiveVoices.Add(audioSource);
		
		return audioSource;
	}
		
	static void SetAudioSource(AudioSource audioSource, AudioInfo audioInfo, AudioClip overrideClip = null){
		if (overrideClip == null) audioSource.clip = audioInfo.clip;
		else audioSource.clip = overrideClip;
		audioSource.mute = audioInfo.mute;
		audioSource.bypassEffects = audioInfo.bypassEffects;
		audioSource.bypassListenerEffects = audioInfo.bypassListenerEffects;
		audioSource.bypassReverbZones = audioInfo.bypassReverbZones;
		audioSource.playOnAwake = audioInfo.playOnAwake;
		audioSource.loop = audioInfo.loop;
		audioSource.priority = audioInfo.priority;
		audioSource.volume = Random.Range(-audioInfo.randomVolume, audioInfo.randomVolume) + audioInfo.volume;
		audioSource.pitch = Random.Range(-audioInfo.randomPitch, audioInfo.randomPitch) + audioInfo.pitch;
		audioSource.dopplerLevel = audioInfo.dopplerLevel;
		audioSource.rolloffMode = audioInfo.rolloffMode;
		audioSource.minDistance = audioInfo.minDistance;
		audioSource.panLevel = audioInfo.panLevel;
		audioSource.spread = audioInfo.spread;
		audioSource.maxDistance = audioInfo.maxDistance;
		audioSource.pan = audioInfo.pan;
		
		if (audioInfo.effects.lowPassFilter){
			AudioLowPassFilter lowPassFilter = audioSource.gameObject.GetComponent<AudioLowPassFilter>();
			if (lowPassFilter == null){lowPassFilter = audioSource.gameObject.AddComponent<AudioLowPassFilter>();}
			lowPassFilter.enabled = true;
			lowPassFilter.cutoffFrequency = audioInfo.effects.lowPassCutoffFrequency;
			lowPassFilter.lowpassResonaceQ = audioInfo.effects.lowPassResonance;
		}
		else {Destroy(audioSource.gameObject.GetComponent<AudioLowPassFilter>());}
		
		if (audioInfo.effects.highPassFilter){
			AudioHighPassFilter highPassFilter = audioSource.gameObject.GetComponent<AudioHighPassFilter>();
			if (highPassFilter == null){highPassFilter = audioSource.gameObject.AddComponent<AudioHighPassFilter>();}
			highPassFilter.enabled = true;
			highPassFilter.cutoffFrequency = audioInfo.effects.highPassCutoffFrequency;
			highPassFilter.highpassResonaceQ = audioInfo.effects.highPassResonance;
		}
		else {Destroy(audioSource.gameObject.GetComponent<AudioHighPassFilter>());}
		
		if (audioInfo.effects.echoFilter){
			AudioEchoFilter echoFilter = audioSource.gameObject.GetComponent<AudioEchoFilter>();
			if (echoFilter == null){echoFilter = audioSource.gameObject.AddComponent<AudioEchoFilter>();}
			echoFilter.enabled = true;
			echoFilter.delay = audioInfo.effects.echoDelay;
			echoFilter.decayRatio = audioInfo.effects.echoDecayRatio;
			echoFilter.dryMix = audioInfo.effects.echoDryMix;
			echoFilter.wetMix = audioInfo.effects.echoWetMix;
		}
		else {Destroy(audioSource.gameObject.GetComponent<AudioEchoFilter>());}
		
		if (audioInfo.effects.distortionFilter){
			AudioDistortionFilter distortionFilter = audioSource.gameObject.GetComponent<AudioDistortionFilter>();
			if (distortionFilter == null){distortionFilter = audioSource.gameObject.AddComponent<AudioDistortionFilter>();}
			distortionFilter.enabled = true;
			distortionFilter.distortionLevel = audioInfo.effects.distortionLevel;
		}
		else {Destroy(audioSource.gameObject.GetComponent<AudioDistortionFilter>());}
		
		if (audioInfo.effects.reverbFilter){
			AudioReverbFilter reverbFilter = audioSource.gameObject.GetComponent<AudioReverbFilter>();
			if (reverbFilter == null){reverbFilter = audioSource.gameObject.AddComponent<AudioReverbFilter>();}
			reverbFilter.enabled = true;
			reverbFilter.reverbPreset = audioInfo.effects.reverbPreset;
			reverbFilter.dryLevel = audioInfo.effects.reverbDryLevel;
			reverbFilter.room = audioInfo.effects.reverbRoom;
			reverbFilter.roomHF = audioInfo.effects.reverbRoomHF;
			reverbFilter.roomLF = audioInfo.effects.reverbRoomLP;
			reverbFilter.decayTime = audioInfo.effects.reverbDecayTime;
			reverbFilter.decayHFRatio = audioInfo.effects.reverbDecayHFRatio;
			reverbFilter.reflectionsDelay = audioInfo.effects.reverbReflectionDelay;
			reverbFilter.reflectionsLevel = audioInfo.effects.reverbReflectionLevel;
			reverbFilter.reverbLevel = audioInfo.effects.reverbLevel;
			reverbFilter.reverbDelay = audioInfo.effects.reverbDelay;
			reverbFilter.hfReference = audioInfo.effects.reverbHFReference;
			reverbFilter.lFReference = audioInfo.effects.reverbLPReference;
			reverbFilter.diffusion = audioInfo.effects.reverbDiffusion;
			reverbFilter.density = audioInfo.effects.reverbDensity;
		}
		else {Destroy(audioSource.gameObject.GetComponent<AudioReverbFilter>());}
		
		if (audioInfo.effects.chorusFilter){
			AudioChorusFilter chorusFilter = audioSource.gameObject.GetComponent<AudioChorusFilter>();
			if (chorusFilter == null){chorusFilter = audioSource.gameObject.AddComponent<AudioChorusFilter>();}
			chorusFilter.enabled = true;
			chorusFilter.dryMix = audioInfo.effects.chorusDryMix;
			chorusFilter.wetMix1 = audioInfo.effects.chorusDryMix;
			chorusFilter.wetMix2 = audioInfo.effects.chorusWetMix1;
			chorusFilter.wetMix3 = audioInfo.effects.chorusWetMix2;
			chorusFilter.delay = audioInfo.effects.chorusDelay;
			chorusFilter.rate = audioInfo.effects.chorusRate;
			chorusFilter.depth = audioInfo.effects.chorusDepth;
		}
		else {Destroy(audioSource.gameObject.GetComponent<AudioChorusFilter>());}
		
		UpdateBusVolume();
		
		if (audioInfo.rTPCs != null){
			foreach (AudioInfo.RTPC rtpc in audioInfo.rTPCs){
				if (rtpc.parameters != null){
					foreach (AudioInfo.RTPCParameter parameter in rtpc.parameters){
						if (parameter != null){
							if (parameter.enabled){
								if (!RTPCAudioDict[rtpc.name].Contains(audioSource)){
									RTPCAudioDict[rtpc.name].Add(audioSource);
									break;
								}
							}
							else {
								if (RTPCAudioDict[rtpc.name].Contains(audioSource)){
									RTPCAudioDict[rtpc.name].Remove(audioSource);
									break;
								}
							}
						}
					}
				}
			}
		}
		foreach (string key in new List<string>(RTPCs.Keys)){
			if (RTPCDict.ContainsKey(key)){
				RTPC rtpcInfo = RTPCDict[key];
				RTPCs[key] = Mathf.Clamp(RTPCs[key], rtpcInfo.minValue, rtpcInfo.maxValue);
				if (audioInfo.RTPCDict != null){
					foreach (AudioInfo.RTPCParameter parameter in audioInfo.RTPCDict[key].parameters){
						if (parameter.enabled){
							ApplyRTPC(audioSource, parameter, RTPCs[key], rtpcInfo, audioInfo);
						}
					}
				}
		    }
		}
	}
	
	static GameObject GetGameObject(GameObject sourceObject){
		GameObject GO;
		AudioCoroutineHolder newCoroutineHolder;
		AudioGainManager newGainManager;
		
		if (InactiveAudioSources.Count > 0){
			GO = InactiveAudioSources[0];
			GO.SetActive(true);
			InactiveAudioSources.Remove(InactiveAudioSources[0]);
			newCoroutineHolder = GO.GetComponent<AudioCoroutineHolder>();
			newGainManager = GO.GetComponent<AudioGainManager>();
		}
		else{
			GO = new GameObject();
			newCoroutineHolder = GO.AddComponent<AudioCoroutineHolder>();
			newGainManager = GO.AddComponent<AudioGainManager>();
		}
		GainManagerDict[GO] = newGainManager;
		ActiveAudioSources[GO] = newCoroutineHolder;
		if(sourceObject != null){GO.transform.parent = sourceObject.transform;}
		else {GO.transform.parent = Instance.transform;}
		GO.transform.localPosition = Vector3.zero;
		
		return GO;
	}
	
	static SubContainer GetRandomContainer(SubContainer[] subContainers){
		float[] weights = new float[subContainers.Length];
		float weightSum = 0;
		float randomValue = 0;
		
		for (int i = 0; i < subContainers.Length; i++){
			SubContainer subContainer = subContainers[i];
			weightSum += subContainer.weight;
			weights[i] = weightSum;
		}
		randomValue = Random.Range(0, weightSum);
		for (int i = 0; i < subContainers.Length; i++){
			if (randomValue < weights[i]){
				return subContainers[i];
			}
		}
		return null;
	}
	
	static double GetDelayToSync(AudioPlayer.SyncMode syncMode){
		double delayToSync = 0;
		
		if (syncMode == AudioPlayer.SyncMode.Beat){delayToSync = TempoSettings.NextBeatTime - AudioSettings.dspTime;}
		else if (syncMode == AudioPlayer.SyncMode.Measure){delayToSync = TempoSettings.NextMeasureTime - AudioSettings.dspTime;}
		
		return delayToSync;
	}
	
	static void UpdateBusVolume(){
		foreach (GameObject GO in ActiveAudioSources.Keys){
			if (GO != null){
				if (GO.audio != null){
					AudioSource audioSource = GO.audio;
					if (audioSource.clip != null){
						if (AudioInfos.ContainsKey(audioSource.clip.name)){
							AudioInfo audioInfo = AudioInfos[audioSource.clip.name];
							AudioGainManager gainManager = GainManagerDict[GO];
							float volumeSum = 0;
							bool applyBuses = false;
							
							foreach (string busName in Buses.Keys){
								if (audioInfo.BusDict != null){
									if (audioInfo.BusDict.ContainsKey(busName)){
										if (audioInfo.BusDict[busName].sendVolume > 0){
											volumeSum += (audioInfo.BusDict[busName].sendVolume / 100) * (Buses[busName] / 100);
											applyBuses = true;
										}
								    }
								}
							}
							if (applyBuses) gainManager.volume = volumeSum * (Instance.masterVolume / 100);
							else gainManager.volume = Instance.masterVolume / 100;
					    }
					}
				}
			}
		}
	}
	
	static void ApplyRTPC(AudioSource audioSource, AudioInfo.RTPCParameter parameter, float rtpcValue, RTPC rtpcInfo, AudioInfo audioInfo){
		float adjustedValue = (rtpcValue - rtpcInfo.minValue) / (rtpcInfo.maxValue - rtpcInfo.minValue);
		float curveValue = parameter.curve.Evaluate(adjustedValue);
		
		if (parameter.name == "Volume"){
			audioSource.volume = audioInfo.volume * curveValue;
		}
		else if (parameter.name == "Pitch"){
			audioSource.pitch = audioInfo.pitch + (curveValue * 6 - 3);
		}
	}
	
	static public double GetAdjustedDelay(float delay, AudioPlayer.SyncMode syncMode){
		double adjustedDelay = 0;
		
		if (delay == 0){return delay;}
		
		if (syncMode == AudioPlayer.SyncMode.Beat){adjustedDelay = delay * TempoSettings.BeatDuration;}
		else if (syncMode == AudioPlayer.SyncMode.Measure){adjustedDelay = delay * TempoSettings.MeasureDuration;}
		else {adjustedDelay = delay;}
		return adjustedDelay;
	}
	
	static public string GetUniqueName(object[] array, string newName, string oldName = "", int suffix = 0){
		bool uniqueName = false;
		string currentName = "";
		
		while (!uniqueName){
			uniqueName = true;
			currentName = newName;
			if (suffix > 0) currentName += suffix.ToString();
			
			foreach (object obj in array){
				if (obj is Container){
					if (((Container) obj).name == currentName && ((Container) obj).name != oldName){
						uniqueName = false;
						break;
					}
				}
				else if (obj is SubContainer){
					if (((SubContainer) obj).name == currentName && ((SubContainer) obj).name != oldName){
						uniqueName = false;
						break;
					}
				}
				else if (obj is RTPC){
					if (((RTPC) obj).name == currentName && ((RTPC) obj).name != oldName){
						uniqueName = false;
						break;
					}
				}
				else if (obj is AudioBus){
					if (((AudioBus) obj).name == currentName && ((AudioBus) obj).name != oldName){
						uniqueName = false;
						break;
					}
				}
			}
			
			suffix += 1;
		}
		return currentName;
	}
	
	static public AudioInfo[] Sort(AudioInfo[] audioInfos){
		string[] names = new string[audioInfos.Length];
		AudioInfo[] sortedAudioInfos = new AudioInfo[audioInfos.Length];
		
		for (int i = 0; i < audioInfos.Length; i++){
			names[i] = audioInfos[i].clip.name;
		}
		
		System.Array.Sort(names);
		
		for (int i = 0; i < audioInfos.Length; i++){
			foreach (AudioInfo sound in audioInfos){
				if (sound.clip.name == names[i]){
					sortedAudioInfos[i] = sound;
					break;
				}
			}
		}
		return sortedAudioInfos;
	}
	
	static public AudioClip[] Sort(AudioClip[] audioClips){
		List<string> names = new List<string>();

		foreach (AudioClip audioClip in audioClips){
			if (audioClip != null){if (!names.Contains(audioClip.name)){names.Add(audioClip.name);}}
		}
		
		string[] namesArray = names.ToArray();
		System.Array.Sort(namesArray);
		AudioClip[] sortedAudioClips = new AudioClip[namesArray.Length];
		
		for (int i = 0; i < namesArray.Length; i++){
			foreach (AudioClip sound in audioClips){
				if (sound != null){
					if (sound.name == namesArray[i]){
						sortedAudioClips[i] = sound;
						break;
					}
				}
			}
		}
		return sortedAudioClips;
	}
	
	[System.Serializable]
	public class TempoSettings{
		[Range(1, 1000)] public float beatsPerMinute = 120;
		public int beatsPerMeasure = 4;
		public float pBeatsPerMinute;
		public float pBeatsPerMeasure;
		public bool changed;
		
		public static float BeatsPerMinute;
		public static int BeatsPerMeasure;
		
		public static double BeatDuration;
		public static double MeasureDuration;
		
		public static double CurrentBeatTime;
		public static int CurrentBeat;
		public static double CurrentMeasureTime;
		public static double NextBeatTime;
		public static double NextMeasureTime;
	}
	
	[System.Serializable]
	public class Container {
		public string Name;
		public string name {
			get {return Name;}
			set {Name = GetUniqueName(Instance.containers, value, Name);}
		}
		public enum ContainerTypes {MixContainer, RandomContainer, SequenceContainer};
		public ContainerTypes containerType;
		public bool loop;
		public Dictionary<int, SubContainer> idDict;
		
		public SubContainer[] sources = new SubContainer[0];
		public int currentId;
		public List<SubContainer> subContainers;
		public bool showing;
		public bool sourcesShowing;
		
		public void BuildIDDict(){
			idDict = new Dictionary<int, AudioPlayer.SubContainer>();
			foreach (SubContainer subContainer in sources){
				idDict[subContainer.id] = subContainer;
				subContainer.SetReferences(this);
			}
			foreach (SubContainer subContainer in subContainers){
				idDict[subContainer.id] = subContainer;
				subContainer.SetReferences(this);
			}
		}
		
		public int GetUniqueID(){
			currentId += 1;
			return currentId;
		}
		
		public SubContainer GetSourceWithID(int id){
			foreach (SubContainer source in sources){
				if (source.id == id){
					return source;
				}
			}
			foreach (SubContainer source in subContainers){
				if (source.id == id){
					return source;
				}
			}
			return null;
		}
		
		public void RemoveEmptyReferences(){
			for (int i = 0; i < subContainers.Count; i++){
				bool referenced = false;
				for (int j = 0; j < sources.Length; j++){
					if (sources[j].childrenLink.Contains(subContainers[i].id)){
				    	referenced = true; 
				    	break;
				    }
				}
				for (int j = 0; j < subContainers.Count; j++){
					if (subContainers[j].childrenLink.Contains(subContainers[i].id)){
				    	referenced = true; 
				    	break;
				    }
				}
				if (!referenced) subContainers.Remove(subContainers[i]);
			}
		}
		
		public List<AudioSource> Play(GameObject sourceObject = null, float delay = 0, AudioPlayer.SyncMode syncMode = AudioPlayer.SyncMode.None){
			return AudioPlayer.Play(this, sourceObject, delay, syncMode);
		}
		
		public List<AudioSource> PlayRepeating(float repeatRate, GameObject sourceObject = null, float delay = 0, AudioPlayer.SyncMode syncMode = AudioPlayer.SyncMode.None){
			return AudioPlayer.PlayRepeating(repeatRate, this, sourceObject, delay, syncMode);
		}
	}
	
	[System.Serializable]
	public class SubContainer {
		public string name;
		
		public enum ContainerTypes {AudioSource, Sampler, MixContainer, RandomContainer, SequenceContainer};
		public ContainerTypes sourceType;
		public AudioSource audioSource;
		public string audioClipName;
		public string instrumentName;
		public int instrumentIndex;
		[Range(0, 127)] public int midiNote = 60;
		[Range(0, 127)] public float velocity = 127;
		public AudioPlayer.SyncMode syncMode;
		public float delay;
		public float weight = 1;
		public int repeat = 1;
		
		public AudioInfo audioInfo;
		[System.NonSerialized] public SubContainer[] sources;
		public int id;
		public int parentLink;
		public List<int> childrenLink;
		public bool showing;
		public bool sourcesShowing;
		
		public void Initialize(Container container, int parentId, SubContainer subContainer = null){
			id = container.GetUniqueID();
			parentLink = parentId;
			childrenLink = new List<int>();
			
			if (subContainer != null){
				sourceType = subContainer.sourceType;
				audioSource = subContainer.audioSource;
				instrumentName = subContainer.instrumentName;
				instrumentIndex = subContainer.instrumentIndex;
				midiNote = subContainer.midiNote;
				velocity = subContainer.velocity;
				syncMode = subContainer.syncMode;
				delay = subContainer.delay;
				weight = subContainer.weight;
				repeat = subContainer.repeat;
			}
			if (parentId != 0) container.GetSourceWithID(parentId).childrenLink.Add(id);
		}
		
		public void SetReferences(Container container){
			sources = new SubContainer[childrenLink.Count];
			for (int i = 0; i < sources.Length; i++){
				sources[i] = container.GetSourceWithID(childrenLink[i]);
			}
			
			if (sourceType == ContainerTypes.AudioSource){
				if (audioSource == null && !string.IsNullOrEmpty(audioClipName)){
					if (AudioInfos.ContainsKey(audioClipName)){
						audioSource = AudioInfos[audioClipName].audio;
					}
				}
				if (audioSource != null){
					if (audioSource.clip != null){
						audioClipName = audioSource.clip.name;
						if (AudioInfos.ContainsKey(audioSource.clip.name)){
							audioInfo = AudioInfos[audioSource.clip.name];
					    }
					}
				}
			}
		}
	}
	
	[System.Serializable]
	public class AudioBus{
		public string Name;
		public string name {
			get {return Name;}
			set {Name = GetUniqueName(Instance.buses, value, Name);}
		}
		[Range(0, 100)] public float volume = 100;
		public float pVolume;
		public bool changed;
		public bool showing = false;
	}
	
	[System.Serializable]
	public class RTPC{
		public string Name;
		public string name {
			get {return Name;}
			set {Name = GetUniqueName(Instance.rTPCs, value, Name);}
		}
		public float defaultValue = 50;
		public float pDefaultValue = 50;
		public float minValue = 0;
		public float maxValue = 100;
		public bool changed;
		public bool showing = false;
	}
}
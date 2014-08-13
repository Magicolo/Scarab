using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// TODO Add crossfades between velocity layers

[ExecuteInEditMode]
public class Sampler : MonoBehaviour {

	public Instrument[] instruments;
	
	AudioCoroutineHolder coroutineHolder;
	
	public static Sampler Instance;
	
	static Dictionary<string, Instrument> Instruments;
	
	void Awake(){
		if (Application.isPlaying){
			Instance = this;
			coroutineHolder = gameObject.AddComponent<AudioCoroutineHolder>();
			if (instruments != null){
				foreach(Instrument instrument in instruments){
					instrument.BuildOriginalClips();
					instrument.GenerateMissingClips();
				}
			}
			BuildInstrumentDict();
		}
	}
	
	void Start(){
		if (!Application.isPlaying){
			gameObject.DisconnectPrefab();
			if (FindObjectsOfType<Sampler>().Length > 1){
				Debug.LogError("There can only be one Sampler.");
				DestroyImmediate(gameObject);
			}
		}
	}
	
	void Update(){
		if (!Application.isPlaying){
			Instance = this;
			transform.hideFlags = HideFlags.HideInInspector;
			transform.position = Vector3.zero;
			transform.rotation = Quaternion.identity;
			transform.localScale = Vector3.one;
		}
		else {
			foreach (Instrument instrument in instruments){
				instrument.LimitVoices();
			}
		}
	}
	
	void BuildInstrumentDict(){
		Instruments = new Dictionary<string, Instrument>();
		if (instruments != null){
			foreach (Instrument instrument in instruments){
				Instruments[instrument.name] = instrument;
			}
		}
	}
	
	static public AudioSource Play(string instrumentName, int midiNote, float velocity, GameObject sourceObject = null, float delay = 0, AudioPlayer.SyncMode syncMode = AudioPlayer.SyncMode.None){
		AudioSource audioSource;
		Instrument instrument = Instruments[instrumentName];
		AudioInfo audioInfo = AudioPlayer.AudioInfos[instrument.referenceClips[midiNote].name];
		AudioClip audioClip = null;
		float initVolume = audioInfo.volume;
		midiNote = GetAdjustedNote(midiNote, velocity, instrument);
		
		if (!string.IsNullOrEmpty(instrument.originalClips[midiNote])) audioClip = instrument.audioClips[midiNote];
		else {
			if (instrument.generateMode == Instrument.GenerateModes.None || instrument.generateMode == Instrument.GenerateModes.PreGenerateAll) audioClip = instrument.audioClips[midiNote];
			else if (instrument.generateMode == Instrument.GenerateModes.GenerateAtRuntime){
				if (instrument.audioClips[midiNote] != null) audioClip = instrument.audioClips[midiNote];
				
				if (audioClip == null){
					audioClip = CreateAudioClip(instrument, midiNote);
				}
				
				if (!audioInfo.loop && instrument.destroyIdle){
					Instance.coroutineHolder.RemoveCoroutines(instrumentName + midiNote);
					Instance.coroutineHolder.AddCoroutine(instrumentName + midiNote, RemoveAfterDelay(audioClip, instrument.idleThreshold, (float) (AudioPlayer.GetAdjustedDelay(audioInfo.delay, audioInfo.syncMode) + AudioPlayer.GetAdjustedDelay(delay, syncMode))));
				}
			}
		}
		
		if (instrument.velocityAffectsVolume) audioInfo.volume *= instrument.velocityCurve.Evaluate(velocity / 127);
		
		audioSource = AudioPlayer.Play(audioClip, audioInfo, sourceObject, delay, syncMode);
		instrument.activeVoices.Add(audioSource);
		
		audioInfo.volume = initVolume;
		return audioSource;
	}
	
	static public List<AudioSource> Play(Note note, GameObject sourceObject = null, float delay = 0, AudioPlayer.SyncMode syncMode = AudioPlayer.SyncMode.None){
		List<AudioSource> audioSources = new List<AudioSource>();
		audioSources.AddRange(Play(note.instrumentName, note.midiNotes, note.velocities, sourceObject, delay, syncMode));
		return audioSources;
	}
	
	static public List<AudioSource> Play(string instrumentName, int[] midiNotes, float velocity, GameObject sourceObject = null, float delay = 0, AudioPlayer.SyncMode syncMode = AudioPlayer.SyncMode.None){
		List<AudioSource> audioSources = new List<AudioSource>();
		for (int i = 0; i < midiNotes.Length; i++){
			audioSources.Add(Play(instrumentName, midiNotes[i], velocity, sourceObject, delay, syncMode));
		}
		return audioSources;
	}
	
	static public List<AudioSource> Play(string instrumentName, int[] midiNotes, float[] velocities, GameObject sourceObject = null, float delay = 0, AudioPlayer.SyncMode syncMode = AudioPlayer.SyncMode.None){
		List<AudioSource> audioSources = new List<AudioSource>();
		for (int i = 0; i < midiNotes.Length; i++){
			audioSources.Add(Play(instrumentName, midiNotes[i], velocities[i], sourceObject, delay, syncMode));
		}
		return audioSources;
	}
	
	static public List<AudioSource> PlayRepeating(float repeatRate, string instrumentName, int midiNote, float velocity, GameObject sourceObject = null, float delay = 0, AudioPlayer.SyncMode syncMode = AudioPlayer.SyncMode.None){
		return AudioPlayer.PlayRepeating(repeatRate, new Note(instrumentName, midiNote, velocity), sourceObject, delay, syncMode);
	}
	
	static public List<AudioSource> PlayRepeating(float repeatRate, string instrumentName, int[] midiNotes, float velocity, GameObject sourceObject = null, float delay = 0, AudioPlayer.SyncMode syncMode = AudioPlayer.SyncMode.None){
		return AudioPlayer.PlayRepeating(repeatRate, new Note(instrumentName, midiNotes, velocity), sourceObject, delay, syncMode);
	}
	
	static public List<AudioSource> PlayRepeating(float repeatRate, string instrumentName, int[] midiNotes, float[] velocities, GameObject sourceObject = null, float delay = 0, AudioPlayer.SyncMode syncMode = AudioPlayer.SyncMode.None){
		return AudioPlayer.PlayRepeating(repeatRate, new Note(instrumentName, midiNotes, velocities), sourceObject, delay, syncMode);
	}
	
	static public void Stop(AudioSource audioSource, float delay = 0){
		AudioPlayer.Stop(audioSource, delay);
	}
	
	static public void Stop(List<AudioSource> audioSources, float delay = 0){
		AudioPlayer.Stop(audioSources, delay);
	}
	
	static public void StopAll(float delay = 0){
		AudioPlayer.StopAll(delay);
		Instance.coroutineHolder.RemoveAllCoroutines();
	}
	
	static public void StopImmediate(AudioSource audioSource, float delay = 0){
		AudioPlayer.StopImmediate(audioSource, delay);
	}
	
	static public void StopImmediate(List<AudioSource> audioSources, float delay = 0){
		AudioPlayer.StopImmediate(audioSources, delay);
	}
	
	static public void StopAllImmediate(float delay = 0){
		AudioPlayer.StopAllImmediate(delay);
		Instance.coroutineHolder.RemoveAllCoroutines();
	}

	static IEnumerator RemoveAfterDelay(AudioClip audioClip, float idleThreshold, float delay){
		delay += audioClip.length + idleThreshold;
		
		float counter = 0;
		while (counter < delay){
			yield return new WaitForSeconds(0);
			counter += Time.deltaTime;
		}
		
		yield return new WaitForEndOfFrame();
		Destroy(audioClip);
	}

	static AudioClip CreateAudioClip(Instrument instrument, int midiNote){
		AudioClip referenceClip = instrument.referenceClips[midiNote];
		int referenceFrequency = instrument.referenceFrequencies[midiNote];
		AudioClip audioClip = AudioClip.Create(referenceClip.name, referenceClip.samples, referenceClip.channels, referenceFrequency, instrument.is3D, false);
		float[] data = new float[referenceClip.samples * referenceClip.channels];
		referenceClip.GetData(data, 0);
		audioClip.SetData(data, 0);
		instrument.audioClips[midiNote] = audioClip;
		
		return audioClip;
	}
	
	static int GetAdjustedNote(int midiNote, float velocity, Instrument instrument){
		return midiNote + (Mathf.Clamp((int) (instrument.velocityCurve.Evaluate(velocity / 127) * instrument.velocityLayers), 0, instrument.velocityLayers - 1) * 128);
	}

	static public string GetUniqueName(object[] array, string newName, string oldName = "", int suffix = 0){
		bool uniqueName = false;
		string currentName = "";
		
		while (!uniqueName){
			uniqueName = true;
			currentName = newName;
			if (suffix > 0) currentName += suffix.ToString();
			
			foreach (object obj in array){
				if (obj is Instrument){
					if (((Instrument) obj).name == currentName && ((Instrument) obj).name != oldName){
						uniqueName = false;
						break;
					}
				}
			}
			suffix += 1;
		}
		return currentName;
	}

	public class Note {
		public string instrumentName;
		public int[] midiNotes;
		public float[] velocities;
		
		public Note(string _instrumentName, int _midiNote, float _velocity){
			instrumentName = _instrumentName;
			midiNotes = new int[1]{_midiNote};
			velocities = new float[1]{_velocity};
		}
		public Note(string _instrumentName, int[] _midiNotes, float _velocity){
			instrumentName = _instrumentName;
			midiNotes = _midiNotes;
			velocities = new float[midiNotes.Length];
			for (int i = 0; i < velocities.Length; i++) velocities[i] = _velocity;
		}
		public Note(string _instrumentName, int[] _midiNotes, float[] _velocities){
			instrumentName = _instrumentName;
			midiNotes = _midiNotes;
			velocities = _velocities;
		}
	}
}

[System.Serializable]
public class Instrument {
	
	public string Name;
	public string name {
		get {return Name;}
		set {Name = Sampler.GetUniqueName(Sampler.Instance.instruments, value, Name);}
	}
	public enum GenerateModes {None, PreGenerateAll, GenerateAtRuntime};
	public GenerateModes generateMode = GenerateModes.GenerateAtRuntime;
	public bool destroyIdle = true;
	public float idleThreshold = 5;
	public List<AudioSource> activeVoices;
	[Range(1, 64)] public int maxVoices = 8;
	public bool is3D = true;
	public bool velocitySettingsShowing = false;
	public bool velocityAffectsVolume = true;
	public AnimationCurve velocityCurve = new AnimationCurve(new Keyframe[2]{new Keyframe(0, 0), new Keyframe(1, 1)});
	[Range(1, 16)] public int velocityLayers = 1;
	public AudioClip[] audioClips;
	public AudioClip[] referenceClips;
	public string[] originalClips;
	public int[] referenceFrequencies;
	public float minNote = 48;
	public float maxNote = 72;
	public bool velocityLayersChanged {
		get {bool changed = pVelocityLayers != velocityLayers;
			pVelocityLayers = velocityLayers;
			return changed;}
	}
	public bool showing = false;
	public bool notesShowing = false;
	public bool generated = false;
	public string[] noteNames = new string[12]{"C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B"};

	int pVelocityLayers;
	
	public void InitializeClips(){
		if (audioClips != null && audioClips.Length == 16 * 128) return;
		else audioClips = new AudioClip[16 * 128];
	}
	
	public void BuildOriginalClips(){
		activeVoices = new List<AudioSource>();
		
		originalClips = new string[16 * 128];
		if (audioClips != null){
			for (int i = 0; i < audioClips.Length; i++){
				if (audioClips[i] != null) originalClips[i] = audioClips[i].name;
			}
		}
	}
	
	public void Reset(){
		audioClips = new AudioClip[16 * 128];
	}
	
	public int GetClipCount(){
		int counter = 0;
		foreach (AudioClip audioClip in audioClips){
			if (audioClip != null) counter += 1;
		}
		return counter;
	}
	
	public void GenerateMissingClips(){
		referenceFrequencies = new int[16 * 128];
		referenceClips = new AudioClip[16 * 128];
		audioClips.CopyTo(referenceClips, 0);
		generated = true;
		
		if (GetClipCount() != 0){
			for (int i = 0; i < velocityLayers; i++){
				int emptyClipsCount = 0;
				int fromLastClipCount = 0;
				int clipsToAddCount = 0;
				for (int j = (int) minNote; j <= (int) maxNote + (128 * i); j++){
					if (audioClips[j] == null){
						if (clipsToAddCount > 0){
							fromLastClipCount += 1;
							
							float ratio = Mathf.Pow(2, (float) fromLastClipCount / 12);
							referenceClips[j] = audioClips[j - fromLastClipCount];
							referenceFrequencies[j] = Mathf.Max((int) (referenceClips[j].frequency * ratio), 1);
							
							if (generateMode == GenerateModes.PreGenerateAll){
								audioClips[j] = AudioClip.Create(referenceClips[j].name, referenceClips[j].samples, referenceClips[j].channels, referenceFrequencies[j], is3D, false);
								float[] data = new float[referenceClips[j].samples * referenceClips[j].channels];
								referenceClips[j].GetData(data, 0);
								audioClips[j].SetData(data, 0);
							}
	
							clipsToAddCount -= 1;
						}
						else emptyClipsCount += 1;
					}
					else {
						fromLastClipCount = 0;
						for (int k = 1; k <= emptyClipsCount; k++){
							float ratio = Mathf.Pow(2, (float) k / 12);
							referenceClips[j - k] = audioClips[j];
							referenceFrequencies[j - k] = Mathf.Max((int) (referenceClips[j - k].frequency / ratio), 1);
							
							if (generateMode == GenerateModes.PreGenerateAll){
								audioClips[j - k] = AudioClip.Create(referenceClips[j - k].name, referenceClips[j - k].samples, referenceClips[j - k].channels, referenceFrequencies[j - k], is3D, false);
								float[] data = new float[referenceClips[j - k].samples * referenceClips[j - k].channels];
								referenceClips[j - k].GetData(data, 0);
								audioClips[j - k].SetData(data, 0);
							}
						}
						emptyClipsCount = 0;
						for (int k = 1; k < 128 - (j % 129); k++){
							if (audioClips[j + k] == null) clipsToAddCount += 1;
							else if (audioClips[j + k] != null) {
								clipsToAddCount /= 2; 
								break;
							}
						}
					}
				}
			}
		}
	}

	public void LimitVoices(){
		while (activeVoices.Count > maxVoices){
			for (int i = 0; i < activeVoices.Count; i++){
				if (activeVoices[i] != null){
					if (activeVoices[i].clip != null){
						AudioInfo audioInfo = AudioPlayer.AudioInfos[activeVoices[i].clip.name];
						float initFadeOut = audioInfo.fadeOut;
						audioInfo.fadeOut = 0.1F;
						Sampler.Stop(activeVoices[0]);
						audioInfo.fadeOut = initFadeOut;
						activeVoices.RemoveAt(i);
						break;
					}
				}
			}
		}
	}
}
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class AudioInfo : MonoBehaviour{
	
	public bool init;
	public float fadeIn;
	public AnimationCurve fadeInCurve;
	public float fadeOut = 0.1F;
	public AnimationCurve fadeOutCurve;
	[Range(0, 1)] public float randomVolume;
	[Range(0, 6)] public float randomPitch;
	public float delay;
	public AudioPlayer.SyncMode syncMode;
	public bool doNotKill;
	public Effects effects;
	public RTPC[] rTPCs;
	public bool rTPCsShowing;
	public AudioBus[] buses;
	public bool busesShowing;
	public ClipInfo clipInfo;
	public Dictionary<string, AudioBus> BusDict;
	public Dictionary<string, RTPC> RTPCDict;
	
	public AudioSource source;
	public AudioClip clip;
	public bool mute;
	public bool bypassEffects;
	public bool bypassListenerEffects;
	public bool bypassReverbZones;
	public bool playOnAwake = false;
	public bool loop;
	public int priority = 128;
	public float initVolume = 1;
	public float volume = 1;
	public float pitch = 1;
	public float dopplerLevel = 0;
	public AudioRolloffMode rolloffMode = AudioRolloffMode.Linear;
	public float minDistance = 0;
	public float panLevel = 1;
	public float spread = 0;
	public float maxDistance = 500;
	public float pan;
	
	float pFadeIn;
	float pFadeOut;
	
	void Awake(){
		if (Application.isPlaying) UpdateInfo();
	}
	
	public void Start(){
		init = true;
		UpdateBuses();
		UpdateRTPCs();
		
		if (!Application.isPlaying){
			AudioSource audioSource = audio;
			
			if (audioSource == null){
				audioSource = gameObject.AddComponent<AudioSource>();
				audioSource.clip = clip;
				audioSource.mute = mute;
				audioSource.bypassEffects = bypassEffects;
				audioSource.bypassListenerEffects = bypassListenerEffects;
				audioSource.bypassReverbZones = bypassReverbZones;
				audioSource.playOnAwake = playOnAwake;
				audioSource.loop = loop;
				audioSource.priority = priority;
				audioSource.volume = volume;
				audioSource.pitch = pitch;
				audioSource.dopplerLevel = dopplerLevel;
				audioSource.rolloffMode = rolloffMode;
				audioSource.minDistance = minDistance;
				audioSource.panLevel = panLevel;
				audioSource.spread = spread;
				audioSource.maxDistance = maxDistance;
				audioSource.pan = pan;
			}
			
			if (effects != null){
				if (effects.lowPassFilter){
					if (effects.audioSourceLowPassFilter == null){
						effects.audioSourceLowPassFilter = gameObject.AddComponent<AudioLowPassFilter>();
						effects.audioSourceLowPassFilter.cutoffFrequency = effects.lowPassCutoffFrequency;
						effects.audioSourceLowPassFilter.lowpassResonaceQ = effects.lowPassResonance;
					}
				}
				if (effects.highPassFilter){
					if (effects.audioSourceHighPassFilter == null){
						effects.audioSourceHighPassFilter = gameObject.AddComponent<AudioHighPassFilter>();
						effects.audioSourceHighPassFilter.cutoffFrequency = effects.highPassCutoffFrequency;
						effects.audioSourceHighPassFilter.highpassResonaceQ = effects.highPassResonance;
					}
				}
				if (effects.echoFilter){
					if (effects.audioSourceEchoFilter == null){
						effects.audioSourceEchoFilter = gameObject.AddComponent<AudioEchoFilter>();
						effects.audioSourceEchoFilter.delay = effects.echoDelay;
						effects.audioSourceEchoFilter.decayRatio = effects.echoDecayRatio;
						effects.audioSourceEchoFilter.dryMix = effects.echoDryMix;
						effects.audioSourceEchoFilter.wetMix = effects.echoWetMix;
					}
				}
				if (effects.distortionFilter){
					if (effects.audioSourceDistortionFilter == null){
						effects.audioSourceDistortionFilter = gameObject.AddComponent<AudioDistortionFilter>();
						effects.audioSourceDistortionFilter.distortionLevel = effects.distortionLevel;
					}
				}
				if (effects.reverbFilter){
					if (effects.audioSourceReverbFilter == null){
						effects.audioSourceReverbFilter = gameObject.AddComponent<AudioReverbFilter>();
						effects.audioSourceReverbFilter.reverbPreset = effects.reverbPreset;
						effects.audioSourceReverbFilter.dryLevel = effects.reverbDryLevel;
						effects.audioSourceReverbFilter.room = effects.reverbRoom;
						effects.audioSourceReverbFilter.roomHF = effects.reverbRoomHF;
						effects.audioSourceReverbFilter.roomLF = effects.reverbRoomLP;
						effects.audioSourceReverbFilter.decayTime = effects.reverbDecayTime;
						effects.audioSourceReverbFilter.decayHFRatio = effects.reverbDecayHFRatio;
						effects.audioSourceReverbFilter.reflectionsDelay = effects.reverbReflectionDelay;
						effects.audioSourceReverbFilter.reflectionsLevel = effects.reverbReflectionLevel;
						effects.audioSourceReverbFilter.reverbLevel = effects.reverbLevel;
						effects.audioSourceReverbFilter.reverbDelay = effects.reverbDelay;
						effects.audioSourceReverbFilter.hfReference = effects.reverbHFReference;
						effects.audioSourceReverbFilter.lFReference = effects.reverbLPReference;
						effects.audioSourceReverbFilter.diffusion = effects.reverbDiffusion;
						effects.audioSourceReverbFilter.density = effects.reverbDensity;
					}
				}
				if (effects.chorusFilter){
					if (effects.audioSourceChorusFilter == null){
						effects.audioSourceChorusFilter = gameObject.AddComponent<AudioChorusFilter>();
						effects.audioSourceChorusFilter.dryMix = effects.chorusDryMix;
						effects.audioSourceChorusFilter.wetMix1 = effects.chorusDryMix;
						effects.audioSourceChorusFilter.wetMix2 = effects.chorusWetMix1;
						effects.audioSourceChorusFilter.wetMix3 = effects.chorusWetMix2;
						effects.audioSourceChorusFilter.delay = effects.chorusDelay;
						effects.audioSourceChorusFilter.rate = effects.chorusRate;
						effects.audioSourceChorusFilter.depth = effects.chorusDepth;
					}
				}
			}
			source = audioSource;
		}
	}
	
	public void UpdateInfo(){
		AudioSource audioSource = audio;
		
		clip = audioSource.clip;
		mute = audioSource.mute;
		bypassEffects = audioSource.bypassEffects;
		bypassListenerEffects = audioSource.bypassListenerEffects;
		bypassReverbZones = audioSource.bypassReverbZones;
		playOnAwake = audioSource.playOnAwake;
		loop = audioSource.loop;
		priority = audioSource.priority;
		volume = audioSource.volume;
		pitch = audioSource.pitch;
		dopplerLevel = audioSource.dopplerLevel;
		rolloffMode = audioSource.rolloffMode;
		minDistance = audioSource.minDistance;
		panLevel = audioSource.panLevel;
		spread = audioSource.spread;
		maxDistance = audioSource.maxDistance;
		pan = audioSource.pan;
		
		if (clipInfo != null){
			clipInfo.name = clip.name;
			clipInfo.channels = clip.channels;
			clipInfo.frequency = clip.frequency;
			clipInfo.length = Mathf.Abs(clip.length / audioSource.pitch);
			clipInfo.samples = clip.samples;
			if (pFadeOut != fadeOut){fadeIn = Mathf.Clamp(fadeIn, 0, clipInfo.length - fadeOut); pFadeOut = fadeOut;}
			if (pFadeIn != fadeIn){fadeOut = Mathf.Clamp(fadeOut, 0, clipInfo.length - fadeIn); pFadeIn = fadeIn;}
			fadeIn = Mathf.Clamp(fadeIn, 0, clipInfo.length);
			fadeOut = Mathf.Clamp(fadeOut, 0, clipInfo.length);
		}
		delay = Mathf.Max(delay, 0);
		
		if (AudioPlayer.Instance != null){
			if (AudioPlayer.Instance.buses != null){
				if (buses == null) UpdateBuses();
				else if (buses.Length != AudioPlayer.Instance.buses.Length) UpdateBuses();
				else {
					for (int i = 0; i < AudioPlayer.Instance.buses.Length; i++){
						if (buses[i].name != AudioPlayer.Instance.buses[i].name){UpdateBuses(); break;}
					}
				}
			}
			
			if (AudioPlayer.Instance.rTPCs != null){
				if (rTPCs == null) UpdateRTPCs();
				else if (rTPCs.Length != AudioPlayer.Instance.rTPCs.Length) UpdateRTPCs();
				else {
					for (int i = 0; i < AudioPlayer.Instance.rTPCs.Length; i++){
						if (rTPCs[i].name != AudioPlayer.Instance.rTPCs[i].name){UpdateRTPCs(); break;}
					}
				}
			}
		}
		if (rTPCs != null){
			foreach (RTPC rtpc in rTPCs){
				if (rtpc != null) rtpc.Update();
			}
		}
		
		if (fadeInCurve == null){
			fadeInCurve = new AnimationCurve(new Keyframe[2]{new Keyframe(0, 0), new Keyframe(1, 1)});}
		if (fadeOutCurve == null){fadeOutCurve = new AnimationCurve(new Keyframe[2]{new Keyframe(0, 1), new Keyframe(1, 0)});}
		UpdateCurve(fadeInCurve, "In");
		UpdateCurve(fadeOutCurve, "Out");
		
		if (effects != null){
			if (effects.lowPassFilter){
				if (effects.audioSourceLowPassFilter == null){
					effects.audioSourceLowPassFilter = gameObject.AddComponent<AudioLowPassFilter>();
					effects.audioSourceLowPassFilter.cutoffFrequency = effects.lowPassCutoffFrequency;
					effects.audioSourceLowPassFilter.lowpassResonaceQ = effects.lowPassResonance;
				}
				else if (effects.audioSourceLowPassFilter.cutoffFrequency == 5000 && effects.audioSourceLowPassFilter.cutoffFrequency != effects.lowPassCutoffFrequency){
					effects.audioSourceLowPassFilter.cutoffFrequency = effects.lowPassCutoffFrequency;
				}
				else {
					effects.lowPassCutoffFrequency = effects.audioSourceLowPassFilter.cutoffFrequency;
					effects.lowPassResonance = effects.audioSourceLowPassFilter.lowpassResonaceQ;
				}
			}
			else if(effects.audioSourceLowPassFilter != null){DestroyImmediate(effects.audioSourceLowPassFilter);}
			
			if (effects.highPassFilter){
				if (effects.audioSourceHighPassFilter == null){
					effects.audioSourceHighPassFilter = gameObject.AddComponent<AudioHighPassFilter>();
					effects.audioSourceHighPassFilter.cutoffFrequency = effects.highPassCutoffFrequency;
					effects.audioSourceHighPassFilter.highpassResonaceQ = effects.highPassResonance;
				}
				else {
					effects.highPassCutoffFrequency = effects.audioSourceHighPassFilter.cutoffFrequency;
					effects.highPassResonance = effects.audioSourceHighPassFilter.highpassResonaceQ;
				}
			}
			else if(effects.audioSourceHighPassFilter != null){DestroyImmediate(effects.audioSourceHighPassFilter);}
			
			if (effects.echoFilter){
				if (effects.audioSourceEchoFilter == null){
					effects.audioSourceEchoFilter = gameObject.AddComponent<AudioEchoFilter>();
					effects.audioSourceEchoFilter.delay = effects.echoDelay;
					effects.audioSourceEchoFilter.decayRatio = effects.echoDecayRatio;
					effects.audioSourceEchoFilter.dryMix = effects.echoDryMix;
					effects.audioSourceEchoFilter.wetMix = effects.echoWetMix;
				}
				else {
					effects.echoDelay = effects.audioSourceEchoFilter.delay;
					effects.echoDecayRatio = effects.audioSourceEchoFilter.decayRatio;
					effects.echoDryMix = effects.audioSourceEchoFilter.dryMix;
					effects.echoWetMix = effects.audioSourceEchoFilter.wetMix;
				}
			}
			else if(effects.audioSourceEchoFilter != null){DestroyImmediate(effects.audioSourceEchoFilter);}
			
			if (effects.distortionFilter){
				if (effects.audioSourceDistortionFilter == null){
					effects.audioSourceDistortionFilter = gameObject.AddComponent<AudioDistortionFilter>();
					effects.audioSourceDistortionFilter.distortionLevel = effects.distortionLevel;
				}
				else {
					effects.distortionLevel = effects.audioSourceDistortionFilter.distortionLevel;
				}
			}
			else if(effects.audioSourceDistortionFilter != null){DestroyImmediate(effects.audioSourceDistortionFilter);}
			
			if (effects.reverbFilter){
				if (effects.audioSourceReverbFilter == null){
					effects.audioSourceReverbFilter = gameObject.AddComponent<AudioReverbFilter>();
					effects.audioSourceReverbFilter.reverbPreset = effects.reverbPreset;
					effects.audioSourceReverbFilter.dryLevel = effects.reverbDryLevel;
					effects.audioSourceReverbFilter.room = effects.reverbRoom;
					effects.audioSourceReverbFilter.roomHF = effects.reverbRoomHF;
					effects.audioSourceReverbFilter.roomLF = effects.reverbRoomLP;
					effects.audioSourceReverbFilter.decayTime = effects.reverbDecayTime;
					effects.audioSourceReverbFilter.decayHFRatio = effects.reverbDecayHFRatio;
					effects.audioSourceReverbFilter.reflectionsDelay = effects.reverbReflectionDelay;
					effects.audioSourceReverbFilter.reflectionsLevel = effects.reverbReflectionLevel;
					effects.audioSourceReverbFilter.reverbLevel = effects.reverbLevel;
					effects.audioSourceReverbFilter.reverbDelay = effects.reverbDelay;
					effects.audioSourceReverbFilter.hfReference = effects.reverbHFReference;
					effects.audioSourceReverbFilter.lFReference = effects.reverbLPReference;
					effects.audioSourceReverbFilter.diffusion = effects.reverbDiffusion;
					effects.audioSourceReverbFilter.density = effects.reverbDensity;
				}
				else {
					effects.reverbPreset = effects.audioSourceReverbFilter.reverbPreset;
					effects.reverbDryLevel = effects.audioSourceReverbFilter.dryLevel;
					effects.reverbRoom = effects.audioSourceReverbFilter.room;
					effects.reverbRoomHF = effects.audioSourceReverbFilter.roomHF;
					effects.reverbRoomLP = effects.audioSourceReverbFilter.roomLF;
					effects.reverbDecayTime = effects.audioSourceReverbFilter.decayTime;
					effects.reverbDecayHFRatio = effects.audioSourceReverbFilter.decayHFRatio;
					effects.reverbReflectionDelay = effects.audioSourceReverbFilter.reflectionsDelay;
					effects.reverbReflectionLevel = effects.audioSourceReverbFilter.reflectionsLevel;
					effects.reverbLevel = effects.audioSourceReverbFilter.reverbLevel;
					effects.reverbDelay = effects.audioSourceReverbFilter.reverbDelay;
					effects.reverbHFReference = effects.audioSourceReverbFilter.hfReference;
					effects.reverbLPReference = effects.audioSourceReverbFilter.lFReference;
					effects.reverbDiffusion = effects.audioSourceReverbFilter.diffusion;
					effects.reverbDensity = effects.audioSourceReverbFilter.density;
				}
			}
			else if(effects.audioSourceReverbFilter != null){DestroyImmediate(effects.audioSourceReverbFilter);}
			
			if (effects.chorusFilter){
				if (effects.audioSourceChorusFilter == null){
					effects.audioSourceChorusFilter = gameObject.AddComponent<AudioChorusFilter>();
					effects.audioSourceChorusFilter.dryMix = effects.chorusDryMix;
					effects.audioSourceChorusFilter.wetMix1 = effects.chorusDryMix;
					effects.audioSourceChorusFilter.wetMix2 = effects.chorusWetMix1;
					effects.audioSourceChorusFilter.wetMix3 = effects.chorusWetMix2;
					effects.audioSourceChorusFilter.delay = effects.chorusDelay;
					effects.audioSourceChorusFilter.rate = effects.chorusRate;
					effects.audioSourceChorusFilter.depth = effects.chorusDepth;
				}
				else {
					effects.chorusDryMix = effects.audioSourceChorusFilter.dryMix;
					effects.chorusDryMix = effects.audioSourceChorusFilter.wetMix1;
					effects.chorusWetMix1 = effects.audioSourceChorusFilter.wetMix2;
					effects.chorusWetMix2 = effects.audioSourceChorusFilter.wetMix3;
					effects.chorusDelay = effects.audioSourceChorusFilter.delay;
					effects.chorusRate = effects.audioSourceChorusFilter.rate;
					effects.chorusDepth = effects.audioSourceChorusFilter.depth;
				}
			}
			else if(effects.audioSourceChorusFilter != null){DestroyImmediate(effects.audioSourceChorusFilter);}
		}
	}
	
	void UpdateCurve(AnimationCurve curve, string mode){
		if (curve != null){
			if (curve.keys.Length == 0){
				curve.AddKey(0, 0);
				curve.AddKey(1, 1);
			}
			for (int i = 0; i < curve.keys.Length; i++){
				if (i == 0){
					if (mode == "In"){
						if (curve.keys[i].time != 0 || curve.keys[i].value != 0){
					    	Keyframe key = new Keyframe(0, 0);
							key.tangentMode = curve.keys[i].tangentMode;
							key.inTangent = curve.keys[i].inTangent;
							key.outTangent = curve.keys[i].outTangent;
							curve.MoveKey(i, key);
						}
				    }
					else if (mode == "Out"){
						if (curve.keys[i].time != 0 || curve.keys[i].value != 1){
							Keyframe key = new Keyframe(0, 1);
							key.tangentMode = curve.keys[i].tangentMode;
							key.inTangent = curve.keys[i].inTangent;
							key.outTangent = curve.keys[i].outTangent;
							curve.MoveKey(i, key);
						}
					}
				}
				else if (i == curve.keys.Length - 1){
					if (mode == "In"){
						if (curve.keys[i].time != 1 || curve.keys[i].value != 1){
							Keyframe key = new Keyframe(1, 1);
							key.tangentMode = curve.keys[i].tangentMode;
							key.inTangent = curve.keys[i].inTangent;
							key.outTangent = curve.keys[i].outTangent;
							curve.MoveKey(i, key);
						}
					}
					else if (mode == "Out"){
						if (curve.keys[i].time != 1 || curve.keys[i].value != 0){
							Keyframe key = new Keyframe(1, 0);
							key.tangentMode = curve.keys[i].tangentMode;
							key.inTangent = curve.keys[i].inTangent;
							key.outTangent = curve.keys[i].outTangent;
							curve.MoveKey(i, key);
						}
					}
				}
			}
		}
	}
	
	void UpdateBuses(){
		BusDict = new Dictionary<string, AudioBus>();
		List<AudioBus> audioBuses = new List<AudioBus>();
		if (AudioPlayer.Instance != null){
			if (AudioPlayer.Instance.buses != null){
				if (buses == null){buses = new AudioBus[0];}
				for (int i = 0; i < AudioPlayer.Instance.buses.Length; i++){
					if (i < buses.Length){
						buses[i].name = AudioPlayer.Instance.buses[i].name;
						audioBuses.Add(buses[i]);
					}
					else {
						AudioBus newBus = new AudioBus();
						newBus.name = AudioPlayer.Instance.buses[i].name;
						audioBuses.Add(newBus);
					}
				}
				buses = audioBuses.ToArray();
				
				foreach (AudioBus bus in buses){
					BusDict[bus.name] = bus;
				}
			}
		}
	}
	
	void UpdateRTPCs(){
		RTPCDict = new Dictionary<string, RTPC>();
		List<RTPC> rtpcs = new List<RTPC>();
		
		if (AudioPlayer.Instance != null){
			if (AudioPlayer.Instance.rTPCs != null){
				if (rTPCs == null){rTPCs = new RTPC[0];}
				for (int i = 0; i < AudioPlayer.Instance.rTPCs.Length; i++){
					if (i < rTPCs.Length){
						rTPCs[i].name = AudioPlayer.Instance.rTPCs[i].name;
						
						rTPCs[i].volume.name = "Volume";
						rTPCs[i].pitch.name = "Pitch";
						rtpcs.Add(rTPCs[i]);
					}
					else {
						RTPC newRTPC = new RTPC();
						newRTPC.name = AudioPlayer.Instance.rTPCs[i].name;
						newRTPC.volume = new RTPCParameter();
						newRTPC.volume.name = "Volume";
						newRTPC.volume.curve = new AnimationCurve(new Keyframe[2]{new Keyframe(0, 0), new Keyframe(1, 1)});
						
						newRTPC.pitch = new RTPCParameter();
						newRTPC.pitch.name = "Pitch";
						newRTPC.pitch.curve = new AnimationCurve(new Keyframe[2]{new Keyframe(0, 0), new Keyframe(1, 1)});
						
						rtpcs.Add(newRTPC);
					}
				}
				rTPCs = rtpcs.ToArray();
				
				foreach (RTPC rtpc in rTPCs){
					RTPCDict[rtpc.name] = rtpc;
				}
			}
		}
	}
	
	[System.Serializable]
	public class Effects{
		public bool lowPassFilter;
		[HideInInspector] public AudioLowPassFilter audioSourceLowPassFilter;
		[HideInInspector] public float lowPassCutoffFrequency = 5000;
		[HideInInspector] public float lowPassResonance = 1;
		
		public bool highPassFilter;
		[HideInInspector] public AudioHighPassFilter audioSourceHighPassFilter;
		[HideInInspector] public float highPassCutoffFrequency = 5000;
		[HideInInspector] public float highPassResonance = 1;
		
		public bool echoFilter;
		[HideInInspector] public AudioEchoFilter audioSourceEchoFilter;
		[HideInInspector] public float echoDelay = 500;
		[HideInInspector] public float echoDecayRatio = 0.5F;
		[HideInInspector] public float echoWetMix = 1;
		[HideInInspector] public float echoDryMix = 1;
		
		public bool distortionFilter;
		[HideInInspector] public AudioDistortionFilter audioSourceDistortionFilter;
		[HideInInspector] public float distortionLevel = 0.5F;
		
		public bool reverbFilter;
		[HideInInspector] public AudioReverbFilter audioSourceReverbFilter;
		[HideInInspector] public AudioReverbPreset reverbPreset = AudioReverbPreset.User;
		[HideInInspector] public float reverbDryLevel;
		[HideInInspector] public float reverbRoom;
		[HideInInspector] public float reverbRoomHF;
		[HideInInspector] public float reverbRoomLP;
		[HideInInspector] public float reverbDecayTime = 1;
		[HideInInspector] public float reverbDecayHFRatio = 0.5F;
		[HideInInspector] public float reverbReflectionLevel = -10000;
		[HideInInspector] public float reverbReflectionDelay;
		[HideInInspector] public float reverbLevel;
		[HideInInspector] public float reverbDelay = 0.04F;
		[HideInInspector] public float reverbHFReference = 5000;
		[HideInInspector] public float reverbLPReference = 250;
		[HideInInspector] public float reverbDiffusion = 100;
		[HideInInspector] public float reverbDensity = 100;
		
		public bool chorusFilter;
		[HideInInspector] public AudioChorusFilter audioSourceChorusFilter;
		[HideInInspector] public float chorusDryMix = 0.5F;
		[HideInInspector] public float chorusWetMix1 = 0.5F;
		[HideInInspector] public float chorusWetMix2 = 0.5F;
		[HideInInspector] public float chorusWetMix3 = 0.5F;
		[HideInInspector] public float chorusDelay = 40F;
		[HideInInspector] public float chorusRate = 0.8F;
		[HideInInspector] public float chorusDepth = 0.03F;
	}
	
	[System.Serializable]
	public class AudioBus{
		public string name;
		public bool enabled;
		[Range(0, 100)] public float sendVolume;
		public bool showing;
	}
	
	[System.Serializable]
	public class RTPC{
		public string name;
		public RTPCParameter[] parameters;
		
		public RTPCParameter volume;
		public RTPCParameter pitch;
		public bool showing;
		
		public void Update(){
			parameters = new RTPCParameter[]{volume, pitch};
			
			foreach (RTPCParameter parameter in parameters){
				if (parameter != null){
					if (parameter.curve != null){
						for (int i = 0; i < parameter.curve.keys.Length; i++){
							if (parameter.curve.keys[i].time < 0 || parameter.curve.keys[i].time > 1 || parameter.curve.keys[i].value < 0 || parameter.curve.keys[i].value > 1){
								Keyframe key = new Keyframe(Mathf.Clamp01(parameter.curve.keys[i].time), Mathf.Clamp01(parameter.curve.keys[i].value));
								key.tangentMode = parameter.curve.keys[i].tangentMode;
								key.inTangent = parameter.curve.keys[i].inTangent;
								key.outTangent = parameter.curve.keys[i].outTangent;
								parameter.curve.MoveKey(i, key);
							}
						}
					}
				}
			}
		}
	}
	
	[System.Serializable]
	public class RTPCParameter{
		public string name;
		public bool enabled;
		public AnimationCurve curve;
		public bool showing;
	}
	
	[System.Serializable]
	public class ClipInfo {
		public string name;
		public int channels;
		public int frequency;
		public float length;
		public int samples;
	}
}

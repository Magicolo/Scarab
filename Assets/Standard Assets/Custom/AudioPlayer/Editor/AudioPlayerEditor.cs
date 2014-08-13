#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(AudioPlayer))]
public class AudioPlayerEditor : Editor {

	AudioPlayer audioPlayer;
	bool x;
	
	public override void OnInspectorGUI(){
		audioPlayer = (AudioPlayer) target;
		
		EditorGUI.BeginChangeCheck();
		
		serializedObject.Update();
		EditorGUILayout.Space();
		EditorGUILayout.PropertyField(serializedObject.FindProperty("audioClipsPath"));
		EditorGUILayout.PropertyField(serializedObject.FindProperty("masterVolume"));
		EditorGUILayout.PropertyField(serializedObject.FindProperty("maxVoices"));
		ShowTempoSettings();
		ShowSeparator();
		ShowContainers();
		ShowRTPCs();
		ShowBuses();
		serializedObject.ApplyModifiedProperties();
		
		if (EditorGUI.EndChangeCheck()) EditorUtility.SetDirty(target);
	}
	
	void ShowTempoSettings(){
		audioPlayer.showTempoSettings = EditorGUILayout.Foldout(audioPlayer.showTempoSettings, "Tempo Settings: " + audioPlayer.tempoSettings.beatsPerMinute + " | " + audioPlayer.tempoSettings.beatsPerMeasure);
		
		if (audioPlayer.showTempoSettings){
			EditorGUI.indentLevel += 1;
			audioPlayer.tempoSettings.beatsPerMinute = EditorGUILayout.Slider("Beats Per Minute", audioPlayer.tempoSettings.beatsPerMinute, 0.01F, 1000);
			audioPlayer.tempoSettings.beatsPerMeasure = Mathf.Max(EditorGUILayout.IntField("Beats Per Measure", audioPlayer.tempoSettings.beatsPerMeasure), 1);
			EditorGUI.indentLevel -= 1;
			
			if (audioPlayer.tempoSettings.pBeatsPerMinute != audioPlayer.tempoSettings.beatsPerMinute || audioPlayer.tempoSettings.pBeatsPerMeasure != audioPlayer.tempoSettings.beatsPerMeasure){
				audioPlayer.tempoSettings.changed = true;
				audioPlayer.tempoSettings.pBeatsPerMinute = audioPlayer.tempoSettings.beatsPerMinute;
				audioPlayer.tempoSettings.pBeatsPerMeasure = audioPlayer.tempoSettings.beatsPerMeasure;
			}
		}
	}
	
	void ShowRTPCs(){
		EditorGUILayout.BeginHorizontal();
		audioPlayer.showRTPCs = EditorGUILayout.Foldout(audioPlayer.showRTPCs, "RTPCs (" + audioPlayer.rTPCs.Length + ")");
		
		EditorGUI.BeginDisabledGroup(Application.isPlaying);
		if (ShowAddElementButton(serializedObject.FindProperty("rTPCs"))){
			audioPlayer.rTPCs[audioPlayer.rTPCs.Length - 1] = new AudioPlayer.RTPC();
			audioPlayer.rTPCs[audioPlayer.rTPCs.Length - 1].name = AudioPlayer.GetUniqueName(audioPlayer.rTPCs, "default");
		}
		EditorGUI.EndDisabledGroup();
		EditorGUILayout.EndHorizontal();
		
		if (audioPlayer.showRTPCs){
			EditorGUI.indentLevel += 1;
			
			for (int i = 0; i < audioPlayer.rTPCs.Length; i++){
				AudioPlayer.RTPC rtpc = audioPlayer.rTPCs[i];
				
				EditorGUILayout.BeginHorizontal();
				rtpc.showing = EditorGUILayout.Foldout(rtpc.showing, rtpc.name);
				GUILayout.Space(40);
				if (!rtpc.showing) rtpc.defaultValue = EditorGUILayout.Slider(rtpc.defaultValue, rtpc.minValue, rtpc.maxValue);
				if (ShowDeleteElementButton(serializedObject.FindProperty("rTPCs"), i)) break;
				EditorGUILayout.EndHorizontal();
				
				if (rtpc.showing){
					EditorGUI.indentLevel += 1;
					rtpc.name = EditorGUILayout.TextField(rtpc.name);
					rtpc.defaultValue = EditorGUILayout.Slider("Value", rtpc.defaultValue, rtpc.minValue, rtpc.maxValue);
					rtpc.minValue = Mathf.Min(EditorGUILayout.FloatField("Min Value", rtpc.minValue), rtpc.maxValue);
					rtpc.maxValue = Mathf.Max(EditorGUILayout.FloatField("Max Value", rtpc.maxValue), rtpc.minValue);
					ShowSeparator(false);
					EditorGUI.indentLevel -= 1;
				}
				if (rtpc.pDefaultValue != rtpc.defaultValue){
					rtpc.changed = true;
					rtpc.pDefaultValue = rtpc.defaultValue;
				}
			}
			EditorGUI.indentLevel -= 1;
		}
	}
	
	void ShowBuses(){
		EditorGUILayout.BeginHorizontal();
		audioPlayer.showBuses = EditorGUILayout.Foldout(audioPlayer.showBuses, "Buses (" + audioPlayer.buses.Length + ")");
		
		EditorGUI.BeginDisabledGroup(Application.isPlaying);
		if (ShowAddElementButton(serializedObject.FindProperty("buses"))){
			audioPlayer.buses[audioPlayer.buses.Length - 1] = new AudioPlayer.AudioBus();
			audioPlayer.buses[audioPlayer.buses.Length - 1].name = AudioPlayer.GetUniqueName(audioPlayer.buses, "default");
		}
		EditorGUI.EndDisabledGroup();
		EditorGUILayout.EndHorizontal();
		
		if (audioPlayer.showBuses){
			EditorGUI.indentLevel += 1;
			
			for (int i = 0; i < audioPlayer.buses.Length; i++){
				AudioPlayer.AudioBus bus = audioPlayer.buses[i];
				
				EditorGUILayout.BeginHorizontal();
				bus.showing = EditorGUILayout.Foldout(bus.showing, bus.name);
				GUILayout.Space(40);
				if (!bus.showing) bus.volume = EditorGUILayout.Slider(bus.volume, 0, 100);
				if (ShowDeleteElementButton(serializedObject.FindProperty("buses"), i)) break;
				EditorGUILayout.EndHorizontal();
				
				if (bus.showing){
					EditorGUI.indentLevel += 1;
					bus.name = EditorGUILayout.TextField(bus.name);
					bus.volume = EditorGUILayout.Slider("Volume", bus.volume, 0, 100);
					ShowSeparator(false);
					EditorGUI.indentLevel -= 1;
				}
				
				if (bus.pVolume != bus.volume){
					bus.changed = true;
					bus.pVolume = bus.volume;
				}
			}
			EditorGUI.indentLevel -= 1;
		}
	}
	
	void ShowContainers(){
		if (audioPlayer.containers != null){
			EditorGUILayout.BeginHorizontal();
			audioPlayer.showContainers = EditorGUILayout.Foldout(audioPlayer.showContainers, "Containers (" + audioPlayer.containers.Length + ")");
			
			EditorGUI.BeginDisabledGroup(Application.isPlaying);
			if (ShowAddElementButton(serializedObject.FindProperty("containers"))){
				audioPlayer.containers[audioPlayer.containers.Length - 1] = new AudioPlayer.Container();
				audioPlayer.containers[audioPlayer.containers.Length - 1].name = AudioPlayer.GetUniqueName(audioPlayer.containers, "default");
				audioPlayer.containers[audioPlayer.containers.Length - 1].subContainers = new List<AudioPlayer.SubContainer>();
			}
			EditorGUI.EndDisabledGroup();
			EditorGUILayout.EndHorizontal();
			
			if (audioPlayer.showContainers){
				EditorGUI.indentLevel += 1;
				
				for (int i = 0; i < audioPlayer.containers.Length; i++){
					AudioPlayer.Container container = audioPlayer.containers[i];
					SerializedProperty containerProperty = serializedObject.FindProperty("containers").GetArrayElementAtIndex(i);
					
					EditorGUILayout.BeginHorizontal();
					container.showing = EditorGUILayout.Foldout(container.showing, container.name);
					EditorGUI.BeginDisabledGroup(Application.isPlaying);
					if (ShowDeleteElementButton(serializedObject.FindProperty("containers"), i)) break;
					EditorGUI.EndDisabledGroup();
					EditorGUILayout.EndHorizontal();
					
					if (container.showing){
						EditorGUI.indentLevel += 1;
						
						EditorGUI.BeginDisabledGroup(Application.isPlaying);
						container.name = EditorGUILayout.TextField(container.name);
						EditorGUI.EndDisabledGroup();
						container.containerType = (AudioPlayer.Container.ContainerTypes) EditorGUILayout.EnumPopup(container.containerType);
						
						ShowSources(container, containerProperty);
						
						EditorGUI.indentLevel -= 1;
					}
				}
				EditorGUI.indentLevel -= 1;
			}
		}
	}
	
	void ShowSources(AudioPlayer.Container container, SerializedProperty containerProperty){
		EditorGUILayout.BeginHorizontal();
		container.sourcesShowing = EditorGUILayout.Foldout(container.sourcesShowing, "Sources (" + container.sources.Length + ")");
		
		EditorGUI.BeginDisabledGroup(Application.isPlaying);
		if (ShowAddElementButton(containerProperty.FindPropertyRelative("sources"))){
			container.sources[container.sources.Length - 1] = new AudioPlayer.SubContainer();
			if (container.sources.Length > 1) container.sources[container.sources.Length - 1].Initialize(container, 0, container.sources[container.sources.Length - 2]);
			else container.sources[container.sources.Length - 1].Initialize(container, 0);
		}
		
		EditorGUI.EndDisabledGroup();
		EditorGUILayout.EndHorizontal();
		
		if (container.sourcesShowing){
			EditorGUI.indentLevel += 1;
			if (container.sources != null){
				for (int i = 0; i < container.sources.Length; i++){
					AudioPlayer.SubContainer source = container.sources[i];
					
					EditorGUILayout.BeginHorizontal();
					source.showing = EditorGUILayout.Foldout(source.showing, source.name);
					EditorGUI.BeginDisabledGroup(Application.isPlaying);
					if (ShowDeleteElementButton(containerProperty.FindPropertyRelative("sources"), i)) {
						container.RemoveEmptyReferences();
						break;
					}
					EditorGUILayout.EndHorizontal();
					EditorGUI.EndDisabledGroup();
					
					if (source.sourceType == AudioPlayer.SubContainer.ContainerTypes.AudioSource){
						ShowAudioSource(source, container, containerProperty);
					}
					else if (source.sourceType == AudioPlayer.SubContainer.ContainerTypes.Sampler){
						ShowSampler(source, container, containerProperty);
					}
					else if (source.sourceType == AudioPlayer.SubContainer.ContainerTypes.MixContainer){
						ShowMixContainer(source, container, containerProperty);
					}
					else if (source.sourceType == AudioPlayer.SubContainer.ContainerTypes.RandomContainer){
						ShowRandomContainer(source, container, containerProperty);
					}
					else if (source.sourceType == AudioPlayer.SubContainer.ContainerTypes.SequenceContainer){
						ShowSequenceContainer(source, container, containerProperty);
					}
				}
			}
			EditorGUI.indentLevel -= 1;
		}
		ShowSeparator(false);
	}
	
	void ShowChildrenSources(AudioPlayer.SubContainer source, AudioPlayer.Container container, SerializedProperty containerProperty){
		EditorGUILayout.BeginHorizontal();
		source.sourcesShowing = EditorGUILayout.Foldout(source.sourcesShowing, "Sources (" + source.childrenLink.Count + ")");
		
		EditorGUI.BeginDisabledGroup(Application.isPlaying);
		if (ShowAddElementButton(containerProperty.FindPropertyRelative("subContainers"))){
			container.subContainers[container.subContainers.Count - 1] = new AudioPlayer.SubContainer();
			if (container.subContainers.Count > 1) container.subContainers[container.subContainers.Count - 1].Initialize(container, source.id, container.subContainers[container.subContainers.Count - 2]);
			else container.subContainers[container.subContainers.Count - 1].Initialize(container, source.id);
		}
		EditorGUI.EndDisabledGroup();
		EditorGUILayout.EndHorizontal();
		
		if (source.sourcesShowing){
			EditorGUI.indentLevel += 1;
			if (source.childrenLink.Count != 0){
				for (int i = 0; i < source.childrenLink.Count; i++){
					AudioPlayer.SubContainer childSource = container.GetSourceWithID(source.childrenLink[i]);
					int index = container.subContainers.IndexOf(childSource);
					
					EditorGUILayout.BeginHorizontal();
					childSource.showing = EditorGUILayout.Foldout(childSource.showing, childSource.name);
					EditorGUI.BeginDisabledGroup(Application.isPlaying);
					if (ShowDeleteElementButton(containerProperty.FindPropertyRelative("subContainers"), index)){
						container.GetSourceWithID(source.id).childrenLink.Remove(childSource.id);
						container.RemoveEmptyReferences();
						break;
					}
					EditorGUILayout.EndHorizontal();
					EditorGUI.EndDisabledGroup();
					
					if (childSource.sourceType == AudioPlayer.SubContainer.ContainerTypes.AudioSource){
						ShowAudioSource(childSource, container, containerProperty);
					}
					else if (childSource.sourceType == AudioPlayer.SubContainer.ContainerTypes.Sampler){
						ShowSampler(childSource, container, containerProperty);
					}
					else if (childSource.sourceType == AudioPlayer.SubContainer.ContainerTypes.MixContainer){
						ShowMixContainer(childSource, container, containerProperty);
					}
					else if (childSource.sourceType == AudioPlayer.SubContainer.ContainerTypes.RandomContainer){
						ShowRandomContainer(childSource, container, containerProperty);
					}
					else if (childSource.sourceType == AudioPlayer.SubContainer.ContainerTypes.SequenceContainer){
						ShowSequenceContainer(childSource, container, containerProperty);
					}
				}
			}
			EditorGUI.indentLevel -= 1;
		}
		ShowSeparator(false);
	}
	
	void ShowAudioSource(AudioPlayer.SubContainer source, AudioPlayer.Container container, SerializedProperty containerProperty){
		if (source.audioSource == null) AdjustName("Audio Source: null", source, container);
		else AdjustName("Audio Source: " + source.audioSource.clip.name, source, container);
		
		if (source.showing){
			EditorGUI.indentLevel += 1;
			ShowSourceParentSettings(source, container);
			source.audioSource = (AudioSource) EditorGUILayout.ObjectField("Audio Source", source.audioSource, typeof(AudioSource), true);
			ShowGeneralSourceSettings(source, container);
			
			EditorGUI.indentLevel -= 1;
		}
	}
	
	void ShowSampler(AudioPlayer.SubContainer source, AudioPlayer.Container container, SerializedProperty containerProperty){
		
		if (string.IsNullOrEmpty(source.instrumentName)) AdjustName("Sampler: null", source, container);
		else AdjustName("Sampler: " + source.instrumentName + " " + source.midiNote + "/" + source.velocity, source, container);
			
		if (source.showing){
			EditorGUI.indentLevel += 1;
			ShowSourceParentSettings(source, container);
			
			string[] displayedOptions = new string[0];
			if (Sampler.Instance != null) if (Sampler.Instance.instruments != null){
				displayedOptions = new string[Sampler.Instance.instruments.Length];
				for (int i = 0; i < Sampler.Instance.instruments.Length; i++){
					displayedOptions[i] = Sampler.Instance.instruments[i].name;
				}
			}
			
			if (displayedOptions.Length > 0){
				source.instrumentIndex = Mathf.Min(EditorGUILayout.Popup("Instrument", source.instrumentIndex, displayedOptions), Sampler.Instance.instruments.Length - 1);
				source.instrumentName = Sampler.Instance.instruments[source.instrumentIndex].name;
				source.midiNote = EditorGUILayout.IntSlider("Midi Note", source.midiNote, (int) Sampler.Instance.instruments[source.instrumentIndex].minNote, (int) Sampler.Instance.instruments[source.instrumentIndex].maxNote);
				source.velocity = EditorGUILayout.Slider("Velocity", source.velocity, 0, 127);
				ShowGeneralSourceSettings(source, container);
			}
			else {
				EditorGUILayout.HelpBox("Add Instruments in the Sampler.", MessageType.Info);
				source.instrumentName = "";
			}
			
			EditorGUI.indentLevel -= 1;
		}
	}
	
	void ShowMixContainer(AudioPlayer.SubContainer source, AudioPlayer.Container container, SerializedProperty containerProperty){
		AdjustName("Mix Container", source, container);
		
		if (source.showing){
			EditorGUI.indentLevel += 1;
			ShowSourceParentSettings(source, container);
			ShowGeneralSourceSettings(source, container);
			ShowChildrenSources(source, container, containerProperty);
			EditorGUI.indentLevel -= 1;
		}
	}
	
	void ShowRandomContainer(AudioPlayer.SubContainer source, AudioPlayer.Container container, SerializedProperty containerProperty){
		AdjustName("Random Container", source, container);
		
		if (source.showing){
			EditorGUI.indentLevel += 1;
			ShowSourceParentSettings(source, container);
			ShowGeneralSourceSettings(source, container);
			ShowChildrenSources(source, container, containerProperty);
			EditorGUI.indentLevel -= 1;
		}
	}
	
	void ShowSequenceContainer(AudioPlayer.SubContainer source, AudioPlayer.Container container, SerializedProperty containerProperty){
		AdjustName("Sequence Container", source, container);
		
		if (source.showing){
			EditorGUI.indentLevel += 1;
			ShowSourceParentSettings(source, container);
			ShowGeneralSourceSettings(source, container);
			ShowChildrenSources(source, container, containerProperty);
			EditorGUI.indentLevel -= 1;
		}
	}
	
	void ShowGeneralSourceSettings(AudioPlayer.SubContainer source, AudioPlayer.Container container){
		source.delay = Mathf.Max(EditorGUILayout.FloatField("Delay", source.delay), 0);
		source.syncMode = (AudioPlayer.SyncMode) EditorGUILayout.EnumPopup("Sync Mode", source.syncMode);
	}
	
	void AdjustName(string prefix, AudioPlayer.SubContainer source, AudioPlayer.Container container){
		source.name = prefix;
		
		if (source.sourceType == AudioPlayer.SubContainer.ContainerTypes.MixContainer || source.sourceType == AudioPlayer.SubContainer.ContainerTypes.RandomContainer || source.sourceType == AudioPlayer.SubContainer.ContainerTypes.SequenceContainer){
			source.name += " | Sources: " + source.childrenLink.Count;
		}
		
		if (GetParentContainerType(source, container) == AudioPlayer.SubContainer.ContainerTypes.RandomContainer){
			source.name += " | Weight: " + source.weight;
		}
		else if (GetParentContainerType(source, container) == AudioPlayer.SubContainer.ContainerTypes.SequenceContainer){
			source.name += " | Repeat: " + source.repeat;
		}
	}
	
	void ShowSourceParentSettings(AudioPlayer.SubContainer source, AudioPlayer.Container container){
		if (GetParentContainerType(source, container) == AudioPlayer.SubContainer.ContainerTypes.RandomContainer){
			source.weight = Mathf.Max(EditorGUILayout.FloatField("Weight", source.weight), 0);
			EditorGUILayout.Space();
		}
		else if (GetParentContainerType(source, container) == AudioPlayer.SubContainer.ContainerTypes.SequenceContainer){
			source.repeat = Mathf.Max(EditorGUILayout.IntField("Repeat", source.repeat), 1);
			EditorGUILayout.Space();
		}
		source.sourceType = (AudioPlayer.SubContainer.ContainerTypes) EditorGUILayout.EnumPopup(source.sourceType);
	}
	
	AudioPlayer.SubContainer.ContainerTypes GetParentContainerType(AudioPlayer.SubContainer source, AudioPlayer.Container container){
		AudioPlayer.SubContainer.ContainerTypes containerType = AudioPlayer.SubContainer.ContainerTypes.AudioSource;
		
		if (source.parentLink != 0){
			containerType = container.GetSourceWithID(source.parentLink).sourceType;
		}
		else if (container.containerType == AudioPlayer.Container.ContainerTypes.MixContainer){
			containerType = AudioPlayer.SubContainer.ContainerTypes.MixContainer;
		}
		else if (container.containerType == AudioPlayer.Container.ContainerTypes.RandomContainer){
			containerType = AudioPlayer.SubContainer.ContainerTypes.RandomContainer;
		}
		else if (container.containerType == AudioPlayer.Container.ContainerTypes.SequenceContainer){
			containerType = AudioPlayer.SubContainer.ContainerTypes.SequenceContainer;
		}
		
		return containerType;
	}
	
	void AddElement(SerializedProperty property){
		property.arraySize += 1;
		serializedObject.ApplyModifiedProperties();
		EditorUtility.SetDirty(target);
	}
	
	void DeleteElement(SerializedProperty property, int indexToRemove){
		property.DeleteArrayElementAtIndex(indexToRemove);
		serializedObject.ApplyModifiedProperties();
		EditorUtility.SetDirty(target);
	}
	
	void ShowSeparator(bool showAboveSpace = true, bool showBelowSpace = true){
		if (showAboveSpace) EditorGUILayout.Space();
		EditorGUILayout.LabelField("", new GUIStyle("RL DragHandle"), GUILayout.Height(4));
		if (showBelowSpace) EditorGUILayout.Space();
	}
	
	bool ShowAddElementButton(SerializedProperty property){
		bool pressed = false;
		if (GUILayout.Button("+", EditorStyles.toolbarButton, GUILayout.Width(20))){
			AddElement(property);
			pressed = true;
		}
		return pressed;
	}
	
	bool ShowLargeAddElementButton(SerializedProperty property, string name){
		bool pressed = false;
		GUILayout.BeginHorizontal();
		GUILayout.Space(EditorGUI.indentLevel * 16);
		if (GUILayout.Button(name, EditorStyles.toolbarButton)){
			AddElement(property);
			pressed = true;
		}
		GUILayout.EndHorizontal();
		return pressed;
	}
	
	bool ShowDeleteElementButton(SerializedProperty property, int indexToRemove){
		bool pressed = false;
		if (GUILayout.Button("-", EditorStyles.toolbarButton, GUILayout.Width(20))){
			DeleteElement(property, indexToRemove);
			pressed = true;
		}
		return pressed;
	}
}
#endif

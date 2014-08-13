using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SamplerExample : MonoBehaviour {

	[Range(0, 127)] public float velocity = 127;
	
	List<AudioSource> audioSources;
	
	void Awake(){
		audioSources = new List<AudioSource>();
	}
	
	void OnGUI() {
		GUILayout.BeginHorizontal();
		GUILayout.Space(Screen.width * 0.6F);
		GUILayout.BeginVertical();
			
		if (GUILayout.Button(" Play Multiple ")){
			audioSources.Add(Sampler.Play("Piano", 60, velocity, gameObject));
			audioSources.Add(Sampler.Play("Piano", 64, velocity, gameObject, 0.05F));
			audioSources.Add(Sampler.Play("Piano", 67, velocity, gameObject, 0.1F));
			audioSources.Add(Sampler.Play("Piano", 71, velocity, gameObject, 0.15F));
	    }
		
		if (GUILayout.Button(" Play Repeatedly ")){
			audioSources = Sampler.PlayRepeating(0.125F, "Piano", new int[3]{60, 64, 67}, velocity, gameObject);
	    }
	    
		if (GUILayout.Button(" Stop Last Sounds With Fade Out ")){
			Sampler.Stop(audioSources);
		}
		
		if (GUILayout.Button(" Stop All Without Fade Out ")){
			Sampler.StopAllImmediate();
		}
		
		GUILayout.EndVertical();
		GUILayout.EndHorizontal();
	}
}

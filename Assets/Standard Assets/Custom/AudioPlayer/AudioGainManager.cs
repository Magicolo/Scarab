using UnityEngine;
using System.Collections;

public class AudioGainManager : MonoBehaviour {

	public float volume = 0;
	float pVolume = 0;

	void Awake(){
		this.hideFlags = HideFlags.HideInInspector;
	}
	
	void OnAudioFilterRead (float[] data, int channels){
		for (int i = 0; i < data.Length; i++){
			pVolume = Mathf.Lerp(pVolume, volume, 0.001F);
			data[i] *= pVolume;
		}
	}
}

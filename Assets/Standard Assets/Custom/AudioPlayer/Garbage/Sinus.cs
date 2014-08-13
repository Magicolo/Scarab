//using UnityEngine;
//using System.Collections;
//using System;
//
//[Serializable]
//public class Sinus {
//
//	public float frequency;
//	int sampleRate;
//	float[] waveArray;
//	int currentPhase;
//	
//	public void Awake(){
//		sampleRate = AudioSettings.outputSampleRate;
//		frequency = UnityEngine.Random.Range(200, 3000);
//	}
//	
//	public void Initialize(){
//		waveArray = new float[(int) (sampleRate / frequency)];
//		double currentPhase = 0;
//		double incr = (Math.PI * 2) / waveArray.Length;
//		for (int i = 0; i < waveArray.Length; i++){
//			waveArray[i] = Mathf.Sin((float) currentPhase);
//			currentPhase += incr;
//		}
//	}
//	
//	public float Process(float data){
//		currentPhase = (currentPhase + 1) % waveArray.Length;
//		return (data + waveArray[currentPhase]);
//	}
//}

//using UnityEngine;
//using System.Collections;
//
//public class Sinuses : MonoBehaviour {
//
//	public Sinus[] sinuses;
//	
//	void Awake(){
//		foreach (Sinus sinus in sinuses){
//			sinus.Awake();
//			sinus.Initialize();
//		}
//	}
//	
//	void OnAudioFilterRead(float[] data, int channels){
//		for (int i = 0; i < data.Length; i++){
//			foreach (Sinus sinus in sinuses){
//				data[i] = sinus.Process(data[i]);
//			}
//			data[i] *= 0.01F;
//		}
//	}
//}

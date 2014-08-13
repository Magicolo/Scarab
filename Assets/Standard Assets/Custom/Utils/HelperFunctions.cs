using UnityEngine;
using System.Collections;
using System.Collections.Generic;

static public class HF {

	static public float MidiToFrequency(float note){
		return Mathf.Pow(2, (note - 69) / 12) * 440;
	}
	
	static public float Hypotenuse(float a){
		return Hypotenuse(a, a);
	}
	
	static public float Hypotenuse(float a, float b){
		return Mathf.Sqrt(Mathf.Pow(a, 2) + Mathf.Pow(b, 2));
	}
	
	static public object WeightedRandom(Dictionary<object, float> objectsAndWeights){
		object[] objectList = new object[objectsAndWeights.Keys.Count];
		float[] weightList = new float[objectsAndWeights.Values.Count];
		int counter = 0;
		
		foreach (object key in objectsAndWeights.Keys){
			objectList[counter] = key;
			weightList[counter] = objectsAndWeights[key];
			counter += 1;
		}
		return WeightedRandom(objectList, weightList);
	}
	
	static public T WeightedRandom<T>(Dictionary<T, float> objectsAndWeights){
		T[] objectList = new T[objectsAndWeights.Keys.Count];
		float[] weightList = new float[objectsAndWeights.Values.Count];
		int counter = 0;
		
		foreach (T key in objectsAndWeights.Keys){
			objectList[counter] = key;
			weightList[counter] = objectsAndWeights[key];
			counter += 1;
		}
		return WeightedRandom<T>(objectList, weightList);
	}
	
	static public object WeightedRandom(List<object> objectList, List<float> weightList){
		return WeightedRandom(objectList.ToArray(), weightList.ToArray());
	}
	
	static public T WeightedRandom<T>(List<T> objectList, List<float> weightList){
		return WeightedRandom<T>(objectList.ToArray(), weightList.ToArray());
	}
	
	static public object WeightedRandom(object[] objectList, float[] weightList){
		return WeightedRandom<object>(objectList, weightList);
	}
	
	static public T WeightedRandom<T>(T[] objectList, float[] weightList){
		float[] weights = new float[weightList.Length];
		float weightSum = 0;
		float randomValue = 0;
		
		for (int i = 0; i < weights.Length; i++){
			weightSum += weightList[i];
			weights[i] = weightSum;
		}
		randomValue = Random.Range(0, weightSum);
		for (int i = 0; i < weights.Length; i++){
			if (randomValue < weights[i]){
				return objectList[i];
			}
		}
		return default(T);
	}

	static public float ProportionalRandomRange(float minValue, float maxValue){
		return Mathf.Pow(2, Random.Range(Mathf.Log(minValue, 2), Mathf.Log(maxValue, 2)));
	}
}

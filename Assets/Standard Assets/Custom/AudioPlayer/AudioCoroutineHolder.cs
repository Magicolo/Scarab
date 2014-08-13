using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class AudioCoroutineHolder : MonoBehaviour {
	
	public enum States {Active, Inactive};
	public Dictionary<string, List<IEnumerator>> coroutines;
	public Dictionary<string, List<States>> coroutineStates;
	
	void Awake(){
		coroutines = new Dictionary<string, List<IEnumerator>>();
		coroutineStates = new Dictionary<string, List<States>>();
		this.hideFlags = HideFlags.HideInInspector;
		
		if (Application.isPlaying){
			AddCoroutine("RemoveCompletedCoroutines", RemoveCompletedCoroutines());
		}
	}
	
	public Coroutine AddCoroutine(string name, IEnumerator coroutine){
		if (!coroutines.ContainsKey(name)) coroutines[name] = new List<IEnumerator>();
		if (!coroutineStates.ContainsKey(name)) coroutineStates[name] = new List<States>();
		coroutines[name].Add(coroutine);
		coroutineStates[name].Add(States.Active);
		return StartCoroutine(coroutine);
	}
	
	public void PauseCoroutine(string name, int index){
		if (coroutines.ContainsKey(name)){
			StopCoroutine(coroutines[name][index]);
			coroutineStates[name][index] = States.Inactive;
		}
	}
	
	public void PauseCoroutines(string name){
		if (coroutines.ContainsKey(name)){
			for (int i = 0; i < coroutines[name].Count; i++){
				PauseCoroutine(name, i);
			}
		}
	}
	
	public void PauseAllCoroutines(){
		List<string> keys = new List<string>(coroutines.Keys);
		foreach (string key in keys){
			PauseCoroutines(key);
		}
	}
	
	public void PauseAllCoroutinesBut(params string[] names){
		List<string> nameList = new List<string>(names);
		List<string> keys = new List<string>(coroutines.Keys);
		foreach (string key in keys){
			if (key != "RemoveCompletedCoroutines") if (!nameList.Contains(key)) PauseCoroutines(key);
		}
	}
		
	public void ResumeCoroutine(string name, int index){
		if (coroutines.ContainsKey(name)){
			if (coroutineStates[name][index] == States.Inactive){
				coroutineStates[name][index] = States.Active;
				StartCoroutine(coroutines[name][index]);
			}
		}
	}
	
	public void ResumeCoroutines(string name){
		if (coroutines.ContainsKey(name)){
			int count = coroutines[name].Count;
			for (int i = 0; i < count; i++){
				ResumeCoroutine(name, i);
			}
		}
	}
	
	public void ResumeAllCoroutines(){
		List<string> keys = new List<string>(coroutines.Keys);
		foreach (string key in keys){
			ResumeCoroutines(key);
		}
	}
	
	public void RemoveCoroutine(string name, int index){
		if (coroutines.ContainsKey(name)){
			StopCoroutine(coroutines[name][index]);
			coroutines[name].RemoveAt(index);
			coroutineStates[name].RemoveAt(index);
			if (coroutines[name].Count == 0){
				coroutines.Remove(name);
				coroutineStates.Remove(name);
			}
		}
	}
	
	public void RemoveCoroutines(string name){
		if (coroutines.ContainsKey(name)){
			int count = coroutines[name].Count;
			for (int i = count - 1; i >= 0; i--){
				RemoveCoroutine(name, i);
			}
		}
	}
	
	public void RemoveAllCoroutines(){
		List<string> keys = new List<string>(coroutines.Keys);
		foreach (string key in keys){
			if (key != "RemoveCompletedCoroutines") RemoveCoroutines(key);
		}
	}
	
	public void RemoveAllCoroutinesBut(params string[] names){
		List<string> nameList = new List<string>(names);
		List<string> keys = new List<string>(coroutines.Keys);
		foreach (string key in keys){
			if (key != "RemoveCompletedCoroutines") if (!nameList.Contains(key)) RemoveCoroutines(key);
		}
	}
	
	public IEnumerator RemoveCompletedCoroutines(){
		while (true){
			List<string> keys = new List<string>(coroutines.Keys);
			foreach (string key in keys){
				if (key != "RemoveCompletedCoroutines"){
					if (coroutines.ContainsKey(key)){
						int count = coroutines[key].Count;
						for (int i = count - 1; i >= 0; i--){
							if (coroutines[key][i].Current == null || !gameObject.activeSelf){
								RemoveCoroutine(key, i);
							}
					    }
					}
				}
			}
			yield return new WaitForSeconds(3);
		}
	}
	
	override public string ToString(){
		string str = "{";
		
		List<string> keys = new List<string>(coroutines.Keys);
		for (int i = 0; i < keys.Count; i++){
			for (int j = 0; j < coroutines[keys[i]].Count; j++){
				str += keys[i] + " : " + coroutineStates[keys[i]][j].ToString();
				if (j < coroutines[keys[i]].Count - 1) str += ", ";
			}
			if (i < keys.Count - 1) str += ", ";
		}
		
		return str + "}";
	}

	
	
}
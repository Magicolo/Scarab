using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class Grid : MonoBehaviour {

	public float positionGrid;
	public float rotationGrid;
	public float scaleGrid;

	void Awake(){
		if (Application.isPlaying){
			this.hideFlags = HideFlags.NotEditable;
		}
	}

	void Update(){
		if (!Application.isPlaying){
			#if UNITY_EDITOR
			foreach (Transform t in UnityEditor.Selection.transforms){
				if (positionGrid > 0) t.localPosition = t.localPosition.Round(positionGrid);
				if (rotationGrid > 0) t.localEulerAngles = t.localEulerAngles.Round(rotationGrid);
				if (scaleGrid > 0) t.localScale = t.localScale.Round(scaleGrid);
			}
			#endif
		}
	}
}

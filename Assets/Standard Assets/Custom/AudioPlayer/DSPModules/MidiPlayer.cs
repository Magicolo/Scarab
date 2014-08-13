using UnityEngine;
using System.Collections;

public class MidiPlayer : MonoBehaviour {
	
	[Range(0, 8)] public int octave = 4;
	public GameObject[] voices;
	int polyCounter = 0;
	
	public void Play(float note, float velocity = 127, int channel = 0){
		if (voices != null) if (voices.Length > 0){
			polyCounter = (polyCounter + 1) % voices.Length;
			if (voices[polyCounter] != null) if (voices[polyCounter].audio != null){
				voices[polyCounter].SendMessage("OnMidiNote", new MidiInfo(note, velocity, channel), SendMessageOptions.DontRequireReceiver);
				voices[polyCounter].audio.Play();
			}
		}
		else gameObject.SendMessage("OnMidiNote", new MidiInfo(note, velocity, channel), SendMessageOptions.DontRequireReceiver);
	}
}

public class MidiInfo {
	public float note;
	public float velocity;
	public int channel;
	
	public MidiInfo(float _note, float _velocity, int _channel){
		note = _note;
		velocity = _velocity;
		channel = _channel;
	}
}

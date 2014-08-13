//using UnityEngine;
//using System.Collections;
//using System;
//
//[System.Serializable]
//public class Source {
//
//	public enum Modules {
//		None,
//		Oscillator,
//		Noise,
//		Sampler
//	};
//	
//	public Modules module;
//	
//	// General
//	public int sampleRate;
//	bool initialized;
//	
//	// Oscillator
//	public enum WaveShapes {Sinus, Custom}
//	public WaveShapes waveShape;
//	public AnimationCurve customShape;
//	public float frequency;
//	[Range(0, 1)] public float phase;
//	float pPhase;
//	float pFrequency;
//	double currentPhase;
//	double increment;
//	
//	// Noise
//	System.Random randomNumber = new System.Random();
//	
//	// Sampler
//	public Notes audioClips;
//	
//	void Initialize(){
//		// Oscillator
//		customShape = new AnimationCurve(new Keyframe[5]{new Keyframe(0, 0), new Keyframe(0.25F, 1), new Keyframe(0.5F, 0), new Keyframe(0.75F, -1), new Keyframe(1, 0)});
//		frequency = 440;
//		
//		initialized = true;
//	}
//	
//	public void Awake(){
//		sampleRate = AudioSettings.outputSampleRate;
//	}
//	
//	public void Update(){
//		if (!Application.isPlaying) if (!initialized) Initialize();
//		customShape.Clamp(0, 1, -1, 1);
//	}
//	
//	public void Process(float[] data, int channels){
//		if (module == Modules.None) return;
//		else if (module == Modules.Oscillator) OscillatorProcess(data, channels);
//		else if (module == Modules.Noise) NoiseProcess(data, channels);
//	}
//	
//	void OscillatorProcess(float[] data, int channels){
//		if (waveShape == WaveShapes.Sinus){
//			if (pFrequency != frequency) increment = frequency * 2 * Math.PI / sampleRate; pFrequency = frequency;
//			if (pPhase != phase) currentPhase = phase * 2 * Math.PI; pPhase = phase;
//			for (var i = 0; i < data.Length; i++){
//				currentPhase += increment;
//				data[i] += (float) Math.Sin(currentPhase);
//				currentPhase %= 2 * Math.PI;
//			}
//		}
//		else if (waveShape == WaveShapes.Custom){
//			if (pFrequency != frequency) increment = frequency / sampleRate; pFrequency = frequency;
//			if (pPhase != phase) currentPhase = phase; pPhase = phase;
//			for (var i = 0; i < data.Length; i++){
//				currentPhase += increment;
//				data[i] += customShape.Evaluate((float) currentPhase);
//				currentPhase %= 1;
//			}
//		}
//		
//	}
//	
//	void NoiseProcess(float[] data, int channels){
//		for (int i = 0; i < data.Length; i++){
//			data[i] += (float) randomNumber.NextDouble() * 2 - 1;
//		}
//	}
//}
//
//[System.Serializable]
//public class Notes {
//	public NoteOctave C0;
//	public NoteOctave C1;
//	public NoteOctave C2;
//	public NoteOctave C3;
//	public NoteOctave C4;
//	public NoteOctave C5;
//}
//
//[System.Serializable]
//public class NoteOctave {
//	public AudioClip C;
//	public AudioClip Db;
//	public AudioClip D;
//	public AudioClip Eb;
//	public AudioClip E;
//	public AudioClip F;
//	public AudioClip Gb;
//	public AudioClip G;
//	public AudioClip Ab;
//	public AudioClip A;
//	public AudioClip Bb;
//	public AudioClip B;
//}
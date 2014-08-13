using UnityEngine;
using System.Collections;
using System;

// FIXME Clipping and weird frequency change delay when playing the midikeyboard
// TODO Smoother of wave (Mathf.Lerp)
// TODO Set the sustaining in the envelope

public class DSPModule : MonoBehaviour {

	public enum Modules {
		None,
		Oscillator,
		Noise,
		Sampler,
		___________,
		Delay,
		Envelope,
		Gain
	};
	
	// General
	public Modules module;
	public int sampleRate;
	Modules pModule;
	
	// Oscillator
	public enum WaveShapes {Sinus, Custom}
	public WaveShapes waveShape;
	public AnimationCurve oscCustomShape = new AnimationCurve(new Keyframe[5]{new Keyframe(0, 0), new Keyframe(0.25F, 1), new Keyframe(0.5F, 0), new Keyframe(0.75F, -1), new Keyframe(1, 0)});
	public float frequency = 440;
	[Range(0, 1)] public float phase;
//	float[] waveArray;
	double oscCurrentPhase;
	double oscIncrement;
	WaveShapes pWaveShape;
	float pFrequency;
	float pPhase;
	
	// Noise
	System.Random randomNumber = new System.Random();
	
	// Delay
	[Range(0, 0.999F)] public float feedback = 0.5F;
	[Range(10, 3000)] public float delayTime = 250;
	int bufferSize;
	float[] dataBuffer;
	int dataBufferPointer = 0;
	int sampleDelay;
	float pDelayTime;
	
	// Enveloppe
	public enum EnvelopeModes {ADSR, Custom};
	public EnvelopeModes mode = EnvelopeModes.ADSR;
	public AnimationCurve envCustomShape = new AnimationCurve(new Keyframe[5]{new Keyframe(0, 0), new Keyframe(0.03F, 1), new Keyframe(0.3F, 0.5F), new Keyframe(0.6F, 0.5F), new Keyframe(1, 0)});
	[Range(0, 1)] public float smooth = 0.01F;
	public float duration = 1;
	public float attack = 0.05F;
	public float decay = 0.25F;
	public float sustain = 0.75F;
	public float release = 1;
	public float maxVolume = 1;
	bool sustaining = false;
	double envCounter;
	double envIncrement;
	EnvelopeModes pMode;
	float pDuration;
	float pEnv;
	
	// Gain
	[Range(0, 5)] public float gain = 1;
	[Range(0, 1)] public float rampSpeed = 0.5F;
	float pGain;
	
	public void Awake(){
		// General
		sampleRate = AudioSettings.outputSampleRate;
		
		// Oscillator
//		OscillatorInitialize();
		
		// Delay
		bufferSize = 3 * sampleRate;
		dataBuffer = new float[bufferSize];
		sampleDelay = Mathf.Max((int) (delayTime / 1000 * sampleRate), 1);
		
		if (module == Modules.Sampler) InitializeSampler();
	}
	
	public void OnMidiNote(MidiInfo info){
		if (module == Modules.Oscillator){
			oscCurrentPhase = 0;
			frequency = HF.MidiToFrequency(info.note);
		}
		else if (module == Modules.Envelope){
			envCounter = 0;
			maxVolume = info.velocity / 127;
		}
	}
	
	public void OnAudioFilterRead(float[] data, int channels){
		if (module == Modules.None) return;
		
		else if (module == Modules.Oscillator) OscillatorProcess(data, channels);
		else if (module == Modules.Noise) NoiseProcess(data, channels);
		
		else if (module == Modules.Delay) DelayProcess(data, channels);
		else if (module == Modules.Envelope) EnvelopeProcess(data, channels);
		else if (module == Modules.Gain) GainProcess(data, channels);
	}
	
//	void OscillatorInitialize(){
//		if (waveShape == WaveShapes.Sinus){
//			waveArray = new float[(int) (sampleRate / frequency)];
//			double currentPhase = 0;
//			double incr = (Math.PI * 2) / waveArray.Length;
//			for (int i = 0; i < waveArray.Length; i++){
//				waveArray[i] = Mathf.Sin((float) currentPhase);
//				currentPhase += incr;
//			}
//		}
//	}
	
	void OscillatorProcess(float[] data, int channels){
		if (waveShape == WaveShapes.Sinus){
			if (pFrequency != frequency || pWaveShape != waveShape || pModule != module) {
				oscIncrement = frequency * 2 * Math.PI / sampleRate;
//				OscillatorInitialize();
				pFrequency = frequency;
				pWaveShape = waveShape;
				pModule = module;
			}
			if (pPhase != phase) oscCurrentPhase = phase * 2 * Math.PI; pPhase = phase;
			for (int i = 0; i < data.Length; i++){
				oscCurrentPhase += oscIncrement;
				data[i] += (float) Math.Sin(oscCurrentPhase);
				oscCurrentPhase %= 2 * Math.PI;
//				oscCurrentPhase = (oscCurrentPhase + 1) % waveArray.Length;
//				data[i] += waveArray[oscCurrentPhase];
			}
		}
		else if (waveShape == WaveShapes.Custom){
			if (pFrequency != frequency || pWaveShape != waveShape || pModule != module) {
				oscIncrement = frequency / sampleRate;
				pFrequency = frequency;
				pWaveShape = waveShape;
				pModule = module;
			}
			if (pPhase != phase) oscCurrentPhase = phase; pPhase = phase;
			for (int i = 0; i < data.Length; i++){
				oscCurrentPhase += oscIncrement;
				data[i] += oscCustomShape.Evaluate((float) oscCurrentPhase);
				oscCurrentPhase %= 1;
			}
		}
	}
	
	void NoiseProcess(float[] data, int channels){
		for (int i = 0; i < data.Length; i++){
			data[i] += (float) randomNumber.NextDouble() * 2 - 1;
		}
	}
	
	void InitializeSampler(){
		
	}
	
	void SamplerProcess(){
		
	}

	void DelayProcess(float[] data, int channels){
		if (pDelayTime != delayTime || pModule != module) {
			sampleDelay = Mathf.Max((int) (delayTime / 1000 * sampleRate), 1);
			pDelayTime = delayTime;
			pModule = module;
		}
		for (int i = 0; i < data.Length; i++){
			data[i] += dataBuffer[dataBufferPointer] * feedback;
			dataBuffer[dataBufferPointer] = data[i];
			dataBufferPointer = (dataBufferPointer + 1) % sampleDelay;
		}
	}
	
	void EnvelopeProcess(float[] data, int channels){
		if (mode == EnvelopeModes.ADSR){
			if (pDuration != duration || pMode != mode || pModule != module){
				if (pModule != module) envCounter = 0;
				envIncrement = 1F / sampleRate;
				pDuration = duration;
				pModule = module;
				pMode = mode;
			}
			for (int i = 0; i < data.Length; i++){
				float env;
				float mult = 0;
				if (envCounter <= attack){mult = (float) (envCounter / attack); envCounter += envIncrement;}
				else if (envCounter > attack && envCounter <= attack + decay){mult = (float) (1 - ((envCounter - attack) / decay) * (1 - sustain)); envCounter += envIncrement;}
				else if (envCounter > attack + decay && sustaining) mult = sustain;
				else if (envCounter > attack + decay && !sustaining && envCounter <= attack + decay + release){mult = (float) (sustain - (envCounter - (attack + decay) / release) * sustain); envCounter += envIncrement;}
				
				if (smooth < 1) env = Mathf.Lerp(pEnv, mult, smooth);
				else env = mult;
				data[i] *= env * maxVolume;
				pEnv = env;
			}
		}
		else {
			if (pDuration != duration || pMode != mode || pModule != module){
				if (pModule != module) envCounter = 0;
				envIncrement = (1F / sampleRate) * (1F / duration);
				pDuration = duration;
				pModule = module;
				pMode = mode;
			}
			for (int i = 0; i < data.Length; i++){
				float env;
				if (smooth < 1) env = Mathf.Lerp(pEnv, envCustomShape.Evaluate((float) envCounter), smooth);
				else env = envCustomShape.Evaluate((float) envCounter);
				data[i] *= env * maxVolume;
				envCounter += envIncrement;
				pEnv = env;
			}
		}
	}
	
	void GainProcess(float[] data, int channels){
		if (rampSpeed < 1){
			pGain = Mathf.Lerp(pGain, gain, rampSpeed);
			for (int i = 0; i < data.Length; i++){
				data[i] *= pGain;
			}
		}
		else {
			for (int i = 0; i < data.Length; i++){
				data[i] *= gain;
			}
		}
	}
}

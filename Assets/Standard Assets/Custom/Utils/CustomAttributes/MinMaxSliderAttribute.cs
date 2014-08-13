
public sealed class MinMaxSliderAttribute : CustomAttributeBase {
	
	public float min;
	public float max;
	public string minLabel;
	public string maxLabel;
	
	public MinMaxSliderAttribute(){
	}
	
	public MinMaxSliderAttribute(float min, float max){
		this.min = min;
		this.max = max;
	}
	
	public MinMaxSliderAttribute(string minLabel, string maxLabel){
		this.minLabel = minLabel;
		this.maxLabel = maxLabel;
	}
	
	public MinMaxSliderAttribute(float min, float max, string minLabel, string maxLabel){
		this.min = min;
		this.max = max;
		this.minLabel = minLabel;
		this.maxLabel = maxLabel;
	}
}

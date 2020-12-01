using UnityEngine;
using UnityEngine.UI;

public class SliderWithEcho : MonoBehaviour {

    public Slider TheSlider = null;
    public Text TheEcho = null;
    public Text TheLabel = null;

    public delegate void SliderCallbackDelegate(float v);      // defined a new data type
    private SliderCallbackDelegate mCallBack = null;           // private instance of the data type


	// Use this for initialization
	void Start () {
        Debug.Assert(TheSlider != null, name);
        Debug.Assert(TheEcho != null, name);
        Debug.Assert(TheLabel != null, name);

        TheSlider.onValueChanged.AddListener(SliderValueChange);
    }

    public void SetSliderListener(SliderCallbackDelegate listener)
    {
        mCallBack = listener;
    }
	
    // GUI element changes the object
	void SliderValueChange(float v)
    {
        TheEcho.text = v.ToString("0");
        mCallBack?.Invoke(v);
    }

    public float GetSliderValue() { return TheSlider.value; }
    public void SetSliderLabel(string l) { TheLabel.text = l; }
    public void SetSliderValue(float v) { TheSlider.value = v; SliderValueChange(v); }
    public void InitSliderRange(float min, float max, float v)
    {
        TheSlider.minValue = min;
        TheSlider.maxValue = max;
        SetSliderValue(v);
    }

}
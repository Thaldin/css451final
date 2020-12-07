using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class sliderEcho : MonoBehaviour
{
    public Text echotext;

    public void EchoText(float _value) {
        echotext.text = _value.ToString();
    }
}

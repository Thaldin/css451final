using UnityEngine;

public class LoadLight : MonoBehaviour
{
    public Transform LightPosition;
    public float LightNear;
    public float LightFar;
    public Color LightColor;

    void OnPreRender()
    {
        Shader.SetGlobalVector("LightPosition", LightPosition.localPosition);
        Shader.SetGlobalFloat("LightFar", LightFar);
        Shader.SetGlobalFloat("LightNear", LightNear);
        Shader.SetGlobalColor("LightColor", LightColor);
    }
}

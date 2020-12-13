using UnityEngine;

public class LoadLight : MonoBehaviour
{
    public Transform LightPosition;

    void OnPreRender()
    {
        Shader.SetGlobalVector("LightPosition", LightPosition.localPosition);
    }
}

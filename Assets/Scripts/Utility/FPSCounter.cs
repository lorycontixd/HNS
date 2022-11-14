using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FPSCounter : MonoBehaviour
{
    private int avgFrameRate;
    public Text fpsText;

    public float threshold1 = 20f;
    public float threshold2 = 50f;

    public void Update()
    {
        float current = 0;
        current = (int)(1f / Time.unscaledDeltaTime);
        avgFrameRate = (int)current;
        fpsText.text = avgFrameRate.ToString() + " FPS";
        if (avgFrameRate < threshold1)
        {
            fpsText.color = Color.red;
        }else if (avgFrameRate >= threshold1 && avgFrameRate < threshold2)
        {
            fpsText.color = Color.yellow;
        }
        else
        {
            fpsText.color = Color.green;
        }
    }
}

﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FPS : MonoBehaviour
{
    /*public float updateRateSeconds = 4.0F;

    int frameCount = 0;
    float dt = 0.0F;
    float fps = 0.0F;
    public Text txtFps;
    void Update()
    {
        frameCount++;
        dt += Time.unscaledDeltaTime;
        if (dt > 1.0 / updateRateSeconds)
        {
            fps = frameCount / dt;
            frameCount = 0;
            dt -= 1.0F / updateRateSeconds;
        }

        txtFps.text = fps.ToString();
    }*/
    float deltaTime = 0.0f;
 
    void Update()
    {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
    }
 
    void OnGUI()
    {
        int w = Screen.width, h = Screen.height;
 
        GUIStyle style = new GUIStyle();
 
        Rect rect = new Rect(0, 0, w, h * 2 / 100);
        style.alignment = TextAnchor.UpperLeft;
        style.fontSize = h * 2 / 100;
        style.normal.textColor = new Color (0.5f, 0.0f, 0.5f, 1.0f);
        float msec = deltaTime * 1000.0f;
        float fps = 1.0f / deltaTime;
       string text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps );
        GUI.Label(rect, text, style);
    }
}
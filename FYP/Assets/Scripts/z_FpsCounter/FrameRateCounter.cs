using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;
using System;

public class FrameRateCounter : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI display = default;

    public enum DisplayMode { FPS, MS};

    private List<float> time = new List<float>();
    private List<float> bestTime = new List<float>();


    [SerializeField]
    DisplayMode displayMode = DisplayMode.FPS;

    [SerializeField, Range(.1f, 2f)]
    float sampleDuration = 1f; // sample durtation for avarage FPS 


    int frames;
    float duration;
    float bestDuration = float.MaxValue;
    float worstDuration;

    private void Update()
    {
        float frameDuration = Time.unscaledDeltaTime; // get the time elapesed between previous frame and the current one
        frames += 1;
        duration += frameDuration;

        // check if current frame duration is < bestDuration, if so make it the bestDuration.
        if(frameDuration < bestDuration) { bestDuration = frameDuration; }
        // check if current frame duration is > bestDuration, if so make it the worstDuration.
        if (frameDuration > worstDuration) { worstDuration = frameDuration; }

        if(duration >= sampleDuration)
        {
            if(displayMode == DisplayMode.FPS)
            {
                var best = 1f / bestDuration;
                var avg = frames / duration;
                var wrst = 1f / worstDuration;
                
                 display.SetText(
                    "FPS\n{0:0}\n{1:0}\n{2:0}\n  {3:1}",
                    
                    best,
                    avg,
                    wrst
                    //sampleDuration
                    );
                bestTime.Add(best);
                time.Add(frames);
                //display.SetText(
                //    "FPS\n{0:0}\n{1:0}\n{2:0}\n  {3:1}",
                //    1f / bestDuration, 
                //    frames / duration, 
                //    1f / worstDuration,
                //    sampleDuration
                //    ); 
            }
            else
            {
                display.SetText(
                   "MS\n{0:1}\n{1:1}\n{2:1}\n  {3:1}",
                   1000f * bestDuration,
                   1000f * duration / frames,
                   1000f * worstDuration
                   //sampleDuration
                   );
            }


            //reset frame count and duration
            frames = 0;
            duration = 0f;
            bestDuration = float.MaxValue;
            worstDuration = 0f;

            // WRITE TO FILE THE VALUES. https://forum.unity.com/threads/write-data-from-list-to-csv-file.643561/.
            string filePath = GetPath();
            StreamWriter writer = new StreamWriter(filePath);
            writer.WriteLine("Best Time , Frames ");

            for(int i = 0; i < Mathf.Max(bestTime.Count, time.Count); ++i)
{
                if (i < bestTime.Count) writer.Write( bestTime[i]);
                writer.Write(",");
                if (i < time.Count) writer.Write(time[i]);
                writer.Write(System.Environment.NewLine);
            }
            writer.Flush();
            writer.Close();
        }
    }

    private string GetPath()
    {
        string path = Application.dataPath + "/Data/" + "Results.csv";
        Debug.Log(path);

        return path;
    }
}

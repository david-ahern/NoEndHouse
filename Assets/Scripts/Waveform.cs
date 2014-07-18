using UnityEngine;
using System.Collections;

[RequireComponent(typeof(GUITexture))]
[RequireComponent(typeof(AudioSource))]

public class Waveform : MonoBehaviour
{
    public int width = 500;
    public int height = 100;
    public Color backgroundColor = Color.black;
    public Color waveformColor = Color.green;
    public int size = 4096;

    Color[] blank;
    Texture2D texture;
    float[] samples;

    void Start()
    {
        samples = new float[size];

        texture = new Texture2D(width, height);
        guiTexture.texture = texture;

        blank = new Color[width * height];

        for (var i = 0; i < blank.Length; i++)
        {
            blank[i] = backgroundColor;
        }

        // refresh the display each 100mS
        StartCoroutine(UpdateWaveForm());
    }

    IEnumerator UpdateWaveForm()
    {
        while (true)
        {
            GetCurWave();
            yield return new WaitForSeconds(0.1f);
        }
    }

    void GetCurWave()
    {
        texture.SetPixels(blank, 0);

        audio.GetOutputData(samples, 0);

        for (var i = 0; i < size; i++)
        {
            texture.SetPixel((int)((width * i) / size), (int)(height * (samples[i] + 1f) / 2), waveformColor);
        }

        texture.Apply();
    }

}
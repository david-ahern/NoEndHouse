using UnityEngine;
using System.Collections;

public class SubtitleController : MonoBehaviour 
{
    static public SubtitleController instance;

    static private TextMesh textMesh;

    public Renderer Background;

    public int LineWidth = 10;

    private int NumLines = 0;

    [Range(0, 1)]
    public float BackgroundAlpha = 1;
    public float FadeTime = 1.0f;

    private float BaseBGScaleX = 11.70765f;
    private float BaseBGScaleY = 2.934343f;
    void Awake()
    {
        if (instance == null)
        {
            instance = this;

            textMesh = gameObject.GetComponent<TextMesh>();
            Color textColor = textMesh.color;
            textColor.a = 0.0f;
            textMesh.color = textColor;
            renderer.enabled = false;

            Vector3 tempScale = Background.gameObject.transform.localScale;
            tempScale.x = LineWidth / 10 * BaseBGScaleX;
            Background.gameObject.transform.localScale = tempScale;

            textColor = Background.material.GetColor("_Color");
            textColor.a = 0.0f;
            Background.material.SetColor("_Color", textColor);

            Background.enabled = false;
        }
        else
            Destroy(gameObject);
    }

    static private string CalculateText(string input)
    {
        string[] words = input.Split(' ');

        string result = "";
        string curLine = "";

        instance.NumLines = 1;

        foreach (string word in words)
        {
            string temp = curLine + " " + word;

            if (temp.Length > instance.LineWidth)
            {
                result += curLine + "\n";

                curLine = word;
                instance.NumLines++;
            }
            else
                curLine = temp;
        }

        result += curLine;
        return result;
    }

    static public void ShowSubtitle(string subtitle, float delay = 0.0f)
    {
        if (instance != null)
        {
            instance.StopAllCoroutines();
            instance.StartCoroutine(instance.coShowSubtitle(subtitle, delay));
        }
    }

    IEnumerator coShowSubtitle(string subtitle, float delay)
    {
        yield return new WaitForSeconds(delay);

        textMesh.text = CalculateText(subtitle);

        Vector3 tempScale = Background.gameObject.transform.localScale;
        tempScale.y = NumLines * BaseBGScaleY;
        Background.gameObject.transform.localScale = tempScale;

        float StartTime = Time.time;

        Color curBGColor = Background.material.GetColor("_Color");
        float StartBGAlpha = curBGColor.a;

        Color curTextColor = textMesh.color;
        float StartTextAlpha = curTextColor.a;

        renderer.enabled = true;
        Background.renderer.enabled = true;

        while (Time.time < StartTime + FadeTime)
        {
            curBGColor.a = Mathf.Lerp(StartBGAlpha, BackgroundAlpha, MiscMethods.GetLerpTimeValue(Time.time, StartTime, FadeTime));
            Background.material.SetColor("_Color", curBGColor);

            curTextColor.a = Mathf.Lerp(StartTextAlpha, 1.0f, MiscMethods.GetLerpTimeValue(Time.time, StartTime, FadeTime));
            textMesh.color = curTextColor;

            yield return new WaitForEndOfFrame();
        }

        curBGColor.a = BackgroundAlpha;
        Background.material.SetColor("_Color", curBGColor);

        curTextColor.a = 1.0f;
        textMesh.color = curTextColor;
    }

    static public void Clear()
    {
        if (instance != null)
        {
            instance.StopAllCoroutines();
            instance.StartCoroutine(instance.coClear());
        }
    }

    IEnumerator coClear()
    {
        Debug.Log("Clearing");
        float StartTime = Time.time;

        Color curBGColor = Background.material.GetColor("_Color");
        float StartBGAlpha = curBGColor.a;

        Color curTextColor = textMesh.color;
        float StartTextAlpha = curTextColor.a;

        while (Time.time < StartTime + FadeTime)
        {
            curBGColor.a = Mathf.Lerp(StartBGAlpha, 0.0f, MiscMethods.GetLerpTimeValue(Time.time, StartTime, FadeTime));
            Background.material.SetColor("_Color", curBGColor);

            curTextColor.a = Mathf.Lerp(StartTextAlpha, 0.0f, MiscMethods.GetLerpTimeValue(Time.time, StartTime, FadeTime));
            textMesh.color = curTextColor;

            yield return new WaitForEndOfFrame();
        }

        curBGColor.a = 0.0f;
        Background.material.SetColor("_Color", curBGColor);

        curTextColor.a = 0.0f;
        textMesh.color = curTextColor;

        Background.renderer.enabled = false;
        renderer.enabled = false;
    }
}

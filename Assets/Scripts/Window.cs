using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider))]
public class Window : MonoBehaviour 
{
    public Transform MovableWindow;

    public float MoveHeight;

    public MiscAudioClip OpenSound;
    public MiscAudioClip CloseSound;

    public float OpenSoundEndDelay;
    public float CloseSoundEndDelay;

    private bool moving;
    private bool _isOpen;
    public bool IsOpen
    {
        get { return _isOpen; }
    }

    void Awake()
    {
        Vector3 temp = MovableWindow.localPosition;
        temp.y = 0;
        MovableWindow.localPosition = temp;

        _isOpen = false;
        moving = false;
    }

	public void ToggleOpen()
    {
        if (!moving)
            StartCoroutine(coToggleOpen(!IsOpen));
    }

    private IEnumerator coToggleOpen(bool Open)
    {
        moving = true;
        _isOpen = Open;

        float startTime = Time.time;
        float endTime = Time.time + 1;

        if (Open)
        {
            if (OpenSound.Clip != null)
            {
                endTime = Time.time + OpenSound.Clip.length - OpenSoundEndDelay;
                SoundController.PlayClip(OpenSound, gameObject.audio);
            }

            Vector3 tempPos = MovableWindow.localPosition;

            while (Time.time < endTime)
            {
                tempPos.y = MoveHeight * ((Time.time - startTime) / (endTime - startTime)); 
                MovableWindow.localPosition = tempPos;
                yield return new WaitForEndOfFrame();
            }

            tempPos.y = MoveHeight;
            MovableWindow.localPosition = tempPos;
        }
        else
        {
            if (CloseSound.Clip != null)
            {
                endTime = Time.time + CloseSound.Clip.length - CloseSoundEndDelay;
                SoundController.PlayClip(CloseSound, gameObject.audio);
            }

            Vector3 tempPos = MovableWindow.localPosition;

            while (Time.time < endTime)
            {
                tempPos.y = MoveHeight - (MoveHeight * ((Time.time - startTime) / (endTime - startTime))); 
                MovableWindow.localPosition = tempPos;
                yield return new WaitForEndOfFrame();
            }

            tempPos.y = 0;
            MovableWindow.localPosition = tempPos;
        }
        moving = false;
    }
}

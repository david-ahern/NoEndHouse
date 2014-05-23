using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(MeshRenderer))]
public class Message : MonoBehaviour 
{
    public bool PlayOnAwake;
    public List<MessageHolder> Messages;

    private bool IsPlaying = false;
    private TextMesh textMesh;

    void Awake()
    {
        textMesh = gameObject.GetComponent<TextMesh>();

        if (PlayOnAwake)
        {
            ShowMessages();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && !IsPlaying)
        {
            ShowMessages();
        }
    }

    void OnTrigerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            if (!IsPlaying && !PlayOnAwake)
                textMesh.text = "";
        }
    }

    private void ShowMessages()
    {
        if (Messages[0].Time > 0)
            StartCoroutine(coShowMessages());
        else
            textMesh.text = Messages[0].Text;
    }

    private IEnumerator coShowMessages()
    {
        IsPlaying = true;
        for (int i = 0; i < Messages.Count; ++i)
        {
            textMesh.text = Messages[i].Text;

            yield return new WaitForSeconds(Messages[i].Time);

            if (Messages[i].NextDelay > 0)
            {
                textMesh.text = "";
                yield return new WaitForSeconds(Messages[i].NextDelay);
            }
        }
        IsPlaying = false;
        if (Messages[Messages.Count - 1].Time > 0)
            textMesh.text = "";
    }

    [System.Serializable]
    public class MessageHolder
    {
        [SerializeField]
        public string Text;
        [SerializeField]
        public float Time = 1.0f;
        [SerializeField]
        public float NextDelay = 0.0f;
    }
}

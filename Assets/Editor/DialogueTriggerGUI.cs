using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(DialogueTrigger))]
public class DialogueTriggerGUI : Editor
{

	private DialogueTrigger Target
    {
        get { return (DialogueTrigger)target; }
    }

    static bool ShowDialogue = true;

    static List<bool> DialogueFoldouts = new List<bool>();

    public override void OnInspectorGUI()
    {
        if (Target.ShowDefaultInspector)
        {
            DrawDefaultInspector();
            return;
        }

        Texture AddButton = GUIIconEditor.GetIcon("Add Icon");
        Texture RemoveButton = GUIIconEditor.GetIcon("Remove Icon");
        Texture UpArrow = GUIIconEditor.GetIcon("Up Icon");
        Texture DownArrow = GUIIconEditor.GetIcon("Down Icon");

        Repaint();

        GUILayout.Label("Hello world");

        GUIStyle FoldoutStyle = new GUIStyle(EditorStyles.foldout);
        FoldoutStyle.fontStyle = FontStyle.Bold;

        EditorGUILayout.BeginHorizontal();
        ShowDialogue = EditorGUILayout.Foldout(ShowDialogue, "Dialogue List", FoldoutStyle);
        if (GUILayout.Button(AddButton, GUILayout.Width(20), GUILayout.Height(20))) Target.Dialogues.Add(new Dialogue());
        EditorGUILayout.EndHorizontal();

        Dialogue removeDialogue = null;

        if (DialogueFoldouts.Count < Target.Dialogues.Count)
            for (int i = DialogueFoldouts.Count; i < Target.Dialogues.Count; i++)
                DialogueFoldouts.Add(false);
        if (ShowDialogue)
        {
            foreach (Dialogue dialogue in Target.Dialogues)
            {
                EditorGUILayout.BeginHorizontal();
                DialogueFoldouts[Target.Dialogues.IndexOf(dialogue)] = EditorGUILayout.Foldout(DialogueFoldouts[Target.Dialogues.IndexOf(dialogue)], (dialogue.Clip ? dialogue.Clip.name : "No Clip"));
                if (GUILayout.Button(UpArrow, GUILayout.Width(20), GUILayout.Height(20))) MoveUp(Target.Dialogues.IndexOf(dialogue));
                if (GUILayout.Button(DownArrow, GUILayout.Width(20), GUILayout.Height(20))) MoveDown(Target.Dialogues.IndexOf(dialogue));
                if (GUILayout.Button(RemoveButton, GUILayout.Width(20), GUILayout.Height(20))) removeDialogue = dialogue;
                EditorGUILayout.EndHorizontal();

                if (DialogueFoldouts[Target.Dialogues.IndexOf(dialogue)])
                {
                    dialogue.Clip = (AudioClip)EditorGUILayout.ObjectField(dialogue.Clip, typeof(AudioClip));
                    dialogue.Subtitle = GUILayout.TextArea(dialogue.Subtitle);
                }
            }
        }

        if (removeDialogue != null)
        {
            DialogueFoldouts.RemoveAt(Target.Dialogues.IndexOf(removeDialogue));
            Target.Dialogues.Remove(removeDialogue);
        }

        GUILayout.BeginHorizontal();
        Target.SkipQueue = EditorGUILayout.Toggle(Target.SkipQueue);
        Target.StopCurrent = EditorGUILayout.Toggle(Target.StopCurrent);
        Target.ClearQueue = EditorGUILayout.Toggle(Target.ClearQueue);
        GUILayout.EndHorizontal();
        EditorUtility.SetDirty(Target);
    }

    void MoveUp(int index)
    {
        if (index == 0) return;

        var temp = Target.Dialogues[index - 1];
        Target.Dialogues[index - 1] = Target.Dialogues[index];
        Target.Dialogues[index] = temp;
    }

    void MoveDown(int index)
    {
        if (index == Target.Dialogues.Count - 1) return;

        var temp = Target.Dialogues[index + 1];
        Target.Dialogues[index + 1] = Target.Dialogues[index];
        Target.Dialogues[index] = temp;
    }
}

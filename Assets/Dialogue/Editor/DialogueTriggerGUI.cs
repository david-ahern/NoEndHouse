﻿using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System;

[CustomEditor(typeof(DialogueTrigger))]
public class DialogueTriggerGUI : Editor
{
	private DialogueTrigger Target
    {
        get { return (DialogueTrigger)target; }
    }

    static bool ShowDialogue = true;

    static List<bool> DialogueFoldouts = new List<bool>();
    static List<bool> OptionsFoldouts = new List<bool>();

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
        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Use Trigger:");
        Target.Trigger = EditorGUILayout.Toggle(Target.Trigger);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();

        GUIStyle FoldoutStyle = new GUIStyle(EditorStyles.foldout);
        FoldoutStyle.fontStyle = FontStyle.Bold;

        EditorGUILayout.BeginHorizontal();
        ShowDialogue = EditorGUILayout.Foldout(ShowDialogue, "Dialogue List", FoldoutStyle);
        if (GUILayout.Button(AddButton, GUILayout.Width(20), GUILayout.Height(20))) AddDialogueWindow.ShowWindow(Target);
        EditorGUILayout.EndHorizontal();

        string removeDialogue = null;

        if (DialogueFoldouts.Count < Target.Keys.Count)
            for (int i = DialogueFoldouts.Count; i < Target.Keys.Count; i++)
                DialogueFoldouts.Add(false);
        if (OptionsFoldouts.Count < Target.Keys.Count)
            for (int i = OptionsFoldouts.Count; i < Target.Keys.Count; i++)
                OptionsFoldouts.Add(false);

                if (ShowDialogue)
                {
                    foreach (string dialogue in Target.Keys)
                    {
                        EditorGUILayout.BeginHorizontal();
                        GUILayout.Space(20);
                        DialogueFoldouts[Target.Keys.IndexOf(dialogue)] = EditorGUILayout.Foldout(DialogueFoldouts[Target.Keys.IndexOf(dialogue)], DialogueEditor._DialogeHolder.dialogue(dialogue).Key);
                        if (GUILayout.Button(UpArrow, GUILayout.Width(20), GUILayout.Height(20)))
                            MoveUp(Target.Keys.IndexOf(dialogue));

                        if (GUILayout.Button(DownArrow, GUILayout.Width(20), GUILayout.Height(20))) 
                            MoveDown(Target.Keys.IndexOf(dialogue));

                        if (GUILayout.Button(RemoveButton, GUILayout.Width(20), GUILayout.Height(20)))
                            removeDialogue = dialogue;

                        EditorGUILayout.EndHorizontal();

                        if (DialogueFoldouts[Target.Keys.IndexOf(dialogue)])
                        {
                            EditorGUILayout.BeginHorizontal();
                            GUILayout.Space(40);
                            OptionsFoldouts[Target.Keys.IndexOf(dialogue)] = EditorGUILayout.Foldout(OptionsFoldouts[Target.Keys.IndexOf(dialogue)], "Audio Options:");
                            EditorGUILayout.EndHorizontal();

                            if (OptionsFoldouts[Target.Keys.IndexOf(dialogue)])
                            {
                                EditorGUILayout.BeginHorizontal();
                                GUILayout.Space(40);
                                GUILayout.Label("Clip:", GUILayout.Width(70));
                                DialogueEditor._DialogeHolder.dialogue(dialogue).Clip = (AudioClip)EditorGUILayout.ObjectField(DialogueEditor._DialogeHolder.dialogue(dialogue).Clip, typeof(AudioClip), false);
                                EditorGUILayout.EndHorizontal();

                                EditorGUILayout.BeginHorizontal();
                                GUILayout.Space(40);
                                GUILayout.Label("Volume:", GUILayout.Width(70));
                                DialogueEditor._DialogeHolder.dialogue(dialogue).Volume = GUILayout.HorizontalSlider(DialogueEditor._DialogeHolder.dialogue(dialogue).Volume, 0, 1);
                                GUILayout.Space(20);
                                EditorGUILayout.EndHorizontal();
                                EditorGUILayout.BeginHorizontal();
                                GUILayout.Space(40);
                                GUILayout.Label("Delay:", GUILayout.Width(70));
                                string temp = GUILayout.TextField(DialogueEditor._DialogeHolder.dialogue(dialogue).Delay.ToString(), GUILayout.Width(50));

                                temp = Regex.Replace(temp, @"[^a-zA-Z0-9 ]", "");
                                if (temp == "")
                                    DialogueEditor._DialogeHolder.dialogue(dialogue).Delay = 0;
                                else
                                    DialogueEditor._DialogeHolder.dialogue(dialogue).Delay = System.Convert.ToSingle(temp);
                                EditorGUILayout.EndHorizontal();
                                EditorGUILayout.Space();
                            }

                            EditorGUILayout.BeginHorizontal();
                            GUILayout.Space(40); 
                            GUILayout.Label("Subtitle:");
                            EditorGUILayout.EndHorizontal();
                            EditorGUILayout.BeginHorizontal();
                            GUILayout.Space(40);
                            DialogueEditor._DialogeHolder.dialogue(dialogue).Subtitle = GUILayout.TextArea(DialogueEditor._DialogeHolder.dialogue(dialogue).Subtitle);
                            EditorGUILayout.EndHorizontal();
                            EditorGUILayout.Space();
                            GUILayout.Box(GUIContent.none, new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(1) });
                            EditorGUILayout.Space();
                        }
                    }

                    EditorUtility.SetDirty(Target);
                }

        if (removeDialogue != null)
        {
            DialogueFoldouts.RemoveAt(Target.Keys.IndexOf(removeDialogue));
            Target.Keys.Remove(removeDialogue);
        }
        EditorGUILayout.Space();
        GUILayout.BeginHorizontal();
        GUILayout.Label("Skip Queue:");
        Target.SkipQueue = EditorGUILayout.Toggle(Target.SkipQueue);
        GUILayout.Label("Stop Current:");
        Target.StopCurrent = EditorGUILayout.Toggle(Target.StopCurrent);
        GUILayout.Label("Clear Queue:");
        Target.ClearQueue = EditorGUILayout.Toggle(Target.ClearQueue);
        GUILayout.EndHorizontal();
        EditorUtility.SetDirty(Target);
        EditorUtility.SetDirty(DialogueEditor._DialogeHolder);
    }

    void MoveUp(int index)
    {
        if (index == 0) return;

        var temp = Target.Keys[index - 1];
        bool tempb = DialogueFoldouts[index - 1];
        bool tempc = OptionsFoldouts[index - 1];
        Target.Keys[index - 1] = Target.Keys[index];
        DialogueFoldouts[index - 1] = DialogueFoldouts[index];
        OptionsFoldouts[index - 1] = OptionsFoldouts[index];
        Target.Keys[index] = temp;
        DialogueFoldouts[index] = tempb;
        OptionsFoldouts[index] = tempc;

    }

    void MoveDown(int index)
    {
        if (index == Target.Keys.Count - 1) return;

        var temp = Target.Keys[index + 1];
        bool tempb = DialogueFoldouts[index + 1];
        bool tempc = OptionsFoldouts[index + 1];
        Target.Keys[index + 1] = Target.Keys[index];
        DialogueFoldouts[index + 1] = DialogueFoldouts[index];
        OptionsFoldouts[index + 1] = OptionsFoldouts[index];
        Target.Keys[index] = temp;
        DialogueFoldouts[index] = tempb;
        OptionsFoldouts[index] = tempc;
    }
}

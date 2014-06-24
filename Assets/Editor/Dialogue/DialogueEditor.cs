using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

[InitializeOnLoad]
public class DialogueEditor : EditorWindow 
{
    static private string HolderPath = "Assets/Editor/Dialogue/Holder/DialogueHolder.asset";

    [SerializeField]
    static public DialogueHolder _DialogeHolder;

    [MenuItem("Dialogue/Dialogue Editor")]
    public static void ShowWindow()
    {
        EditorWindow window = EditorWindow.GetWindow(typeof(DialogueEditor));
        window.name = "Dialogue";
        GetAllDialogueTriggers();
    }

    static DialogueEditor()
    {
        if (_DialogeHolder == null)
            _DialogeHolder = (DialogueHolder)AssetDatabase.LoadAssetAtPath(HolderPath, typeof(DialogueHolder));

        if (_DialogeHolder == null)
        {
            _DialogeHolder = ScriptableObject.CreateInstance<DialogueHolder>();
            _DialogeHolder.Dialogues = new List<Dialogue>();
            AssetDatabase.CreateAsset(_DialogeHolder, HolderPath);

            if (_DialogeHolder == null)
            {
                Debug.Log("Failed to load or create DialogueHolder");
                return;
            }
        }

        GetAllDialogueTriggers();

        if (_DialogeHolder.Dialogues.Count > 0)
        {
            _DialogeHolder.selected = _DialogeHolder.Dialogues[0];
            TempKey = _DialogeHolder.selected.Key;
        }
    }

    enum PopupSelections { Dialogue, Area };
    PopupSelections CurrentPopup = PopupSelections.Dialogue;

    string searchString = "";

    Vector2 DialogueListScrollPos = new Vector2(0, 0);
    Vector2 AreaListScrollPos = new Vector2(0, 0);
    Vector2 AreaTriggerScrollPos = new Vector2(0, 0);
    Vector2 DialoguesScrollPos = new Vector2(0, 0);

    static string TempKey = "";

    static List<GameObject> Areas = new List<GameObject>();
    static List<List<DialogueTrigger>> DialogueAreaList = new List<List<DialogueTrigger>>();

    GameObject SelectedArea;
    DialogueTrigger SelectedTrigger;

    void OnGUI()
    {
        Texture AddButton = GUIIconEditor.GetIcon("Add Icon");
        Texture TrashButton = GUIIconEditor.GetIcon("Trash Icon");
        Texture UpArrow = GUIIconEditor.GetIcon("Up Icon");
        Texture DownArrow = GUIIconEditor.GetIcon("Down Icon");
        Texture SaveIcon = GUIIconEditor.GetIcon("Save Icon");

        GUILayout.BeginHorizontal(GUI.skin.FindStyle("Toolbar"));
        GUILayout.FlexibleSpace();
        searchString = GUILayout.TextField(searchString, GUI.skin.FindStyle("ToolbarSeachTextField"), GUILayout.Width(this.position.width - 150));
        if (GUILayout.Button("", GUI.skin.FindStyle("ToolbarSeachCancelButton")))
        {
            searchString = "";
            GUI.FocusControl(null);
        }
        GUILayout.EndHorizontal();
        CurrentPopup = (PopupSelections)EditorGUI.EnumPopup(new Rect(1,1,100, 15), "", CurrentPopup);

        if (CurrentPopup == PopupSelections.Dialogue)
        {
            EditorGUILayout.BeginHorizontal();
            DialogueListScrollPos = EditorGUILayout.BeginScrollView(DialogueListScrollPos, GUILayout.Width(120), GUILayout.Height(this.position.height - 20));

            foreach (Dialogue dialogue in _DialogeHolder.Dialogues)
                if (dialogue.Key.Contains(searchString) || dialogue.Subtitle.Contains(searchString) || (dialogue.Clip != null && dialogue.Clip.name.Contains(searchString)))
                    if (GUILayout.Button(dialogue.Key, GUILayout.Width(100)))
                    {
                        _DialogeHolder.selected = dialogue;
                        TempKey = dialogue.Key;
                    }

            if (GUILayout.Button(AddButton, GUILayout.Height(20), GUILayout.Width(100))) AddNewDialogue();

            EditorGUILayout.EndScrollView();

            EditorGUILayout.BeginVertical();

            if (_DialogeHolder.selected != null)
            {
                EditorGUILayout.Space();
                EditorGUILayout.BeginHorizontal();

                GUILayout.Label("Key:", GUILayout.Width(70));
                TempKey = GUILayout.TextField(TempKey);

                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("Clip:", GUILayout.Width(70));
                _DialogeHolder.selected.Clip = (AudioClip)EditorGUILayout.ObjectField(_DialogeHolder.selected.Clip, typeof(AudioClip), false);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("Volume:", GUILayout.Width(70));
                _DialogeHolder.selected.Volume = GUILayout.HorizontalSlider(_DialogeHolder.selected.Volume, 0, 1);
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("Delay:", GUILayout.Width(70));
                string temp = GUILayout.TextField(_DialogeHolder.selected.Delay.ToString(), GUILayout.Width(50));

                temp = Regex.Replace(temp, @"[^a-zA-Z0-9 ]", "");
                if (temp == "")
                    _DialogeHolder.selected.Delay = 0;
                else
                    _DialogeHolder.selected.Delay = System.Convert.ToSingle(temp);
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Space();

                GUILayout.Label("Subtitle:");
                _DialogeHolder.selected.Subtitle = GUILayout.TextArea(_DialogeHolder.selected.Subtitle);

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.Space();
                if (GUILayout.Button(SaveIcon, GUILayout.Width(30), GUILayout.Height(30)))
                {
                    ApplyNewDialogueData();
                }
                if (GUILayout.Button(TrashButton, GUILayout.Width(30), GUILayout.Height(30)))
                {
                    RemoveDialogue(_DialogeHolder.selected);

                }

                EditorGUILayout.Space();
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();

            EditorGUILayout.EndHorizontal();
        }
        else if (CurrentPopup == PopupSelections.Area)
        {
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.BeginVertical(GUILayout.Width(120));
            GUILayout.Label("Areas");
            AreaListScrollPos = EditorGUILayout.BeginScrollView(AreaListScrollPos, GUILayout.Width(120), GUILayout.Height(this.position.height - 20));
            foreach (GameObject area in Areas)
                if (GUILayout.Button(area.name, GUILayout.Width(100)))
                {
                    SelectedArea = area;
                    SelectedTrigger = null;
                }
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();

            if (SelectedArea != null)
            {
                EditorGUILayout.BeginVertical(GUILayout.Width(120));
                GUILayout.Label("Triggers", GUILayout.Width(80));
                AreaTriggerScrollPos = EditorGUILayout.BeginScrollView(AreaTriggerScrollPos, GUILayout.Width(120), GUILayout.Height(this.position.height - 20));
                
                foreach (DialogueTrigger trigger in DialogueAreaList[Areas.IndexOf(SelectedArea)])
                    if (GUILayout.Button(trigger.name, GUILayout.Width(100))) SelectedTrigger = trigger;

                EditorGUILayout.EndScrollView();
                EditorGUILayout.EndVertical();

                if (SelectedTrigger != null)
                {
                    DialoguesScrollPos = EditorGUILayout.BeginScrollView(DialoguesScrollPos, GUILayout.Height(this.position.height - 20));
                    if (SelectedTrigger.Dialogues.Count > 0)
                    {
                        Dialogue removeDialogue = null;
                        GUILayout.Label("Dialogues", GUILayout.Width(80));
                        foreach (Dialogue dialogue in SelectedTrigger.Dialogues)
                        {
                            EditorGUILayout.BeginHorizontal();
                            GUILayout.Label("Key: " + dialogue.Key);
                            EditorGUILayout.Space();
                            if (GUILayout.Button(UpArrow, GUILayout.Width(20), GUILayout.Height(20))) MoveUp(SelectedTrigger, SelectedTrigger.Dialogues.IndexOf(dialogue));
                            if (GUILayout.Button(DownArrow, GUILayout.Width(20), GUILayout.Height(20))) MoveDown(SelectedTrigger, SelectedTrigger.Dialogues.IndexOf(dialogue));
                            if (GUILayout.Button(TrashButton, GUILayout.Width(20), GUILayout.Height(20))) removeDialogue = dialogue;
                            EditorGUILayout.EndHorizontal();

                            EditorGUILayout.BeginHorizontal();
                            GUILayout.Label("Clip:", GUILayout.Width(70));
                            dialogue.Clip = (AudioClip)EditorGUILayout.ObjectField(dialogue.Clip, typeof(AudioClip), false);
                            EditorGUILayout.EndHorizontal();

                            EditorGUILayout.BeginHorizontal();
                            GUILayout.Label("Volume:", GUILayout.Width(70));
                            dialogue.Volume = GUILayout.HorizontalSlider(dialogue.Volume, 0, 1);
                            EditorGUILayout.EndHorizontal();
                            EditorGUILayout.BeginHorizontal();
                            GUILayout.Label("Delay:", GUILayout.Width(70));
                            string temp = GUILayout.TextField(dialogue.Delay.ToString(), GUILayout.Width(50));

                            temp = Regex.Replace(temp, @"[^a-zA-Z0-9 ]", "");
                            if (temp == "")
                                dialogue.Delay = 0;
                            else
                                dialogue.Delay = System.Convert.ToSingle(temp);
                            EditorGUILayout.EndHorizontal();
                            EditorGUILayout.Space();

                            GUILayout.Label("Subtitle:");
                            dialogue.Subtitle = GUILayout.TextArea(dialogue.Subtitle);

                        }

                        if (removeDialogue != null)
                            SelectedTrigger.Dialogues.Remove(removeDialogue);
                    }
                    else
                        GUILayout.Label("No dialogues in this trigger");

                    EditorGUILayout.EndScrollView();

                    EditorGUILayout.BeginVertical(GUILayout.Width(120));

                    GUILayout.Label("Availible Dialogues");

                    DialogueListScrollPos = EditorGUILayout.BeginScrollView(DialogueListScrollPos, GUILayout.Width(120), GUILayout.Height(this.position.height - 20));

                    foreach (Dialogue dialogue in _DialogeHolder.Dialogues)
                        if (dialogue.Key.Contains(searchString) || dialogue.Subtitle.Contains(searchString) || (dialogue.Clip != null && dialogue.Clip.name.Contains(searchString)))
                            if (GUILayout.Button(dialogue.Key, GUILayout.Width(100)))
                            {
                                SelectedTrigger.Dialogues.Add(dialogue);
                                EditorUtility.SetDirty(SelectedTrigger);
                            }

                    EditorGUILayout.EndScrollView();
                    EditorGUILayout.EndVertical();
                }
            }
            EditorGUILayout.EndHorizontal();
        }
        Repaint();
    }

    void ApplyNewDialogueData()
    {
        bool allow = true;
        foreach (Dialogue d in _DialogeHolder.Dialogues)
            if (d.Key == TempKey && (_DialogeHolder.Dialogues.IndexOf(d) != _DialogeHolder.Dialogues.IndexOf(_DialogeHolder.selected)))
                allow = false;

        if (allow)
            _DialogeHolder.selected.Key = TempKey;
        else
            EditorUtility.DisplayDialog("Invalid Dialogue Key", "The key entered already exists, please enter a different key.", "Ok");

        EditorUtility.SetDirty(_DialogeHolder);
        AssetDatabase.SaveAssets();
    }
    void AddNewDialogue()
    {
        Dialogue d = new Dialogue();
        bool gotNewKey = true;
        string newKey = "Dialogue";

        foreach (Dialogue dia in _DialogeHolder.Dialogues)
            if (dia.Key == newKey)
                gotNewKey = false;

        int attempts = 1;
        while (!gotNewKey)
        {
            gotNewKey = true;
            newKey = "Dialogue " + attempts.ToString();
            attempts++;
            foreach (Dialogue dia in _DialogeHolder.Dialogues)
                if (dia.Key == newKey)
                    gotNewKey = false;
        }

        d.Key = newKey;

        _DialogeHolder.Dialogues.Add(d);

        _DialogeHolder.selected = _DialogeHolder.Dialogues[_DialogeHolder.Dialogues.Count - 1];
        TempKey = _DialogeHolder.selected.Key;

        EditorUtility.SetDirty(_DialogeHolder);
        AssetDatabase.SaveAssets();
    }

    static void GetAllDialogueTriggers()
    {
        Areas = MiscEditorMethods.LoadAllPrefabsWithComponent(typeof(AreaController));

        foreach (GameObject Area in Areas)
        {
            List<DialogueTrigger> TriggerList = new List<DialogueTrigger>(Area.GetComponentsInChildren<DialogueTrigger>(true));

            DialogueAreaList.Add(TriggerList);

            foreach(DialogueTrigger trigger in TriggerList)
            {
                foreach(Dialogue dialogue in trigger.Dialogues)
                {
                    bool containes = false;
                    foreach (Dialogue d in _DialogeHolder.Dialogues)
                        if (d.Key == dialogue.Key)
                            containes = true;
                    if (!containes)
                        _DialogeHolder.Dialogues.Add(dialogue);
                }
            }
            EditorUtility.SetDirty(_DialogeHolder);
            AssetDatabase.SaveAssets();
        }
    }

    void MoveUp(DialogueTrigger trigger, int index)
    {
        if (index == 0) return;

        var temp = trigger.Dialogues[index - 1];
        trigger.Dialogues[index - 1] = trigger.Dialogues[index];
        trigger.Dialogues[index] = temp;

    }

    void MoveDown(DialogueTrigger trigger, int index)
    {
        if (index == trigger.Dialogues.Count - 1) return;

        var temp = trigger.Dialogues[index + 1];
        trigger.Dialogues[index + 1] = trigger.Dialogues[index];
        trigger.Dialogues[index] = temp;

    }

    void RemoveDialogue(Dialogue dialogue)
    {
        Dialogue remove = null;

        foreach (Dialogue d in _DialogeHolder.Dialogues)
            if (d.Key == dialogue.Key)
                remove = d;

        int newInt = Mathf.Clamp(_DialogeHolder.Dialogues.IndexOf(_DialogeHolder.selected), 0, _DialogeHolder.Dialogues.Count - 2);
        _DialogeHolder.Dialogues.Remove(remove);
        if (_DialogeHolder.Dialogues.Count > 0)
        {
            _DialogeHolder.selected = _DialogeHolder.Dialogues[newInt];
            TempKey = _DialogeHolder.selected.Key;
        }

        EditorUtility.SetDirty(_DialogeHolder);
        AssetDatabase.SaveAssets();

        foreach (GameObject Area in Areas)
        {
            foreach (DialogueTrigger trigger in Area.GetComponentsInChildren<DialogueTrigger>(true))
            {
                Dialogue remove2 = null;

                foreach (Dialogue d in trigger.Dialogues)
                    if (d.Key == dialogue.Key)
                        remove2 = d;

                if (remove2 != null)
                    trigger.Dialogues.Remove(remove2);
            }
        }
    }
}

public class AddDialogueWindow : EditorWindow
{
    static DialogueTrigger SelectedTrigger;
    Vector2 ScrollPos = new Vector2(0, 0);

    static public void ShowWindow(DialogueTrigger trigger)
    {
        SelectedTrigger = trigger;
        EditorWindow window = EditorWindow.GetWindow<AddDialogueWindow>();
        window.position = new Rect(window.position.x, window.position.height, 443, 188);
    }

    Dialogue selected = null;

    void OnGUI()
    {
        Texture AddIcon = GUIIconEditor.GetIcon("Add Icon");
        EditorGUILayout.BeginHorizontal();
        ScrollPos = EditorGUILayout.BeginScrollView(ScrollPos, GUILayout.Width(120), GUILayout.Height(this.position.height - 20));
        foreach(Dialogue dialogue in DialogueEditor._DialogeHolder.Dialogues)
        {
            if (GUILayout.Button(dialogue.Key)) selected = dialogue;
        }
        EditorGUILayout.EndScrollView();

        if (selected != null)
        {
            EditorGUILayout.BeginVertical();
            if (SelectedTrigger.gameObject != Selection.activeGameObject)
                this.Close();

            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();

            GUILayout.Label("Key:", GUILayout.Width(70));
            GUILayout.TextField(selected.Key);

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Clip:", GUILayout.Width(70));
            EditorGUILayout.ObjectField(selected.Clip, typeof(AudioClip), false);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Volume:", GUILayout.Width(70));
            GUILayout.HorizontalSlider(selected.Volume, 0, 1);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Delay:", GUILayout.Width(70));
            GUILayout.TextField(selected.Delay.ToString(), GUILayout.Width(50));

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();

            GUILayout.Label("Subtitle:");
            GUILayout.TextArea(selected.Subtitle);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space();
            if (GUILayout.Button(AddIcon, GUILayout.Width(30), GUILayout.Height(30))) SelectedTrigger.Dialogues.Add(selected);
            EditorGUILayout.Space();
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.EndHorizontal();
        Debug.Log("W:" + this.position.width);
        Debug.Log("H:" + this.position.height);
    }
}
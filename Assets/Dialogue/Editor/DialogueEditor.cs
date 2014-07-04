using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

[InitializeOnLoad]
public class DialogueEditor : EditorWindow 
{
    static private string HolderPath = "Assets/Dialogue/Holder/DialogueHolder.asset";

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

    static public List<AreaFoldout> Areas = new List<AreaFoldout>();

    AreaController SelectedArea;
    DialogueTrigger SelectedTrigger;

    void OnGUI()
    {
        try
        {

            GUIStyle BoldButton = new GUIStyle(EditorStyles.miniButton);
            BoldButton.fontStyle = FontStyle.Bold;

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
            CurrentPopup = (PopupSelections)EditorGUI.EnumPopup(new Rect(1, 1, 100, 15), "", CurrentPopup);

            if (CurrentPopup == PopupSelections.Dialogue)
            {
                EditorGUILayout.BeginHorizontal();
                DialogueListScrollPos = EditorGUILayout.BeginScrollView(DialogueListScrollPos, MyGUIStyles.Options(150, this.position.height - 20));

                foreach (Dialogue dialogue in _DialogeHolder.Dialogues)
                    if (dialogue.Key.ToLower().Contains(searchString.ToLower()) || dialogue.Subtitle.ToLower().Contains(searchString.ToLower()) || dialogue.Clip.name.ToLower().Contains(searchString.ToLower()) || (dialogue.Clip != null && dialogue.Clip.name.Contains(searchString)))
                        if (GUILayout.Button(dialogue.Key, (dialogue == _DialogeHolder.selected ? MyGUIStyles.BoldButton : MyGUIStyles.Button), MyGUIStyles.Options(130, 20)))
                        {
                            _DialogeHolder.selected = dialogue;
                            TempKey = dialogue.Key;
                        }

                if (GUILayout.Button(AddButton, MyGUIStyles.Button, MyGUIStyles.Options(130, 20))) AddNewDialogue();

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
                    if (GUILayout.Button(SaveIcon, MyGUIStyles.Options(30, 30)))
                    {
                        ApplyNewDialogueData();
                    }
                    if (GUILayout.Button(TrashButton, MyGUIStyles.Options(30, 30)))
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

                EditorGUILayout.BeginVertical(GUILayout.Width(150));
                GUILayout.Label("Areas");
                AreaListScrollPos = EditorGUILayout.BeginScrollView(AreaListScrollPos, GUILayout.Width(150), GUILayout.Height(this.position.height - 20));

                foreach (AreaFoldout area in Areas)
                {
                    area.Foldout = EditorGUILayout.Foldout(area.Foldout, area.Area.name);

                    if (area.Foldout)
                    {
                        foreach (DialogueTrigger trigger in area.Triggers)
                        {
                            if (GUILayout.Button(trigger.name, (SelectedTrigger == trigger ? MyGUIStyles.BoldButton : MyGUIStyles.Button), GUILayout.Width(130))) { SelectedTrigger = trigger; SelectedArea = area.Area; }
                        }
                        if (GUILayout.Button(AddButton, MyGUIStyles.Button, MyGUIStyles.Options(130, 15))) AddTriggerToArea(area.Area);
                    }
                }
                EditorGUILayout.EndScrollView();
                EditorGUILayout.EndVertical();


                if (SelectedTrigger != null)
                {
                    bool changed = false;

                    DialoguesScrollPos = EditorGUILayout.BeginScrollView(DialoguesScrollPos, GUILayout.Height(this.position.height - 20));
                    if (SelectedTrigger.Keys.Count > 0)
                    {
                        string removeDialogue = null;
                        GUILayout.Label("Dialogues", GUILayout.Width(80));

                        foreach (string d in SelectedTrigger.Keys)
                        {
                            EditorGUILayout.BeginHorizontal();
                            GUILayout.Label("Key", GUILayout.Width(70));
                            GUILayout.Label(_DialogeHolder.dialogue(d).Key);

                            EditorGUILayout.Space();
                            if (GUILayout.Button(UpArrow, MyGUIStyles.Button, MyGUIStyles.Options(20, 20)))
                            {
                                MoveUp(SelectedTrigger, SelectedTrigger.Keys.IndexOf(d));
                                changed = true;
                            }
                            if (GUILayout.Button(DownArrow, MyGUIStyles.Button, MyGUIStyles.Options(20, 20)))
                            {
                                MoveDown(SelectedTrigger, SelectedTrigger.Keys.IndexOf(d));
                                changed = true;
                            }
                            if (GUILayout.Button(TrashButton, MyGUIStyles.Button, MyGUIStyles.Options(20, 20)))
                            {
                                removeDialogue = _DialogeHolder.dialogue(d).Key;
                                changed = true;
                            }
                            EditorGUILayout.EndHorizontal();
                            GUILayout.Space(5);
                            EditorGUILayout.BeginHorizontal();
                            GUILayout.Label("Clip:", GUILayout.Width(70));
                            _DialogeHolder.dialogue(d).Clip = (AudioClip)EditorGUILayout.ObjectField(_DialogeHolder.dialogue(d).Clip, typeof(AudioClip), false);
                            EditorGUILayout.EndHorizontal();

                            EditorGUILayout.BeginHorizontal();
                            GUILayout.Label("Volume:", GUILayout.Width(70));
                            _DialogeHolder.dialogue(d).Volume = GUILayout.HorizontalSlider(_DialogeHolder.dialogue(d).Volume, 0, 1);
                            EditorGUILayout.EndHorizontal();
                            EditorGUILayout.BeginHorizontal();
                            GUILayout.Label("Delay:", GUILayout.Width(70));
                            string temp = GUILayout.TextField(_DialogeHolder.dialogue(d).Delay.ToString(), GUILayout.Width(50));

                            temp = Regex.Replace(temp, @"[^a-zA-Z0-9 ]", "");
                            if (temp == "")
                                _DialogeHolder.dialogue(d).Delay = 0;
                            else
                                _DialogeHolder.dialogue(d).Delay = System.Convert.ToSingle(temp);
                            EditorGUILayout.EndHorizontal();
                            EditorGUILayout.Space();

                            GUILayout.Label("Subtitle:");
                            _DialogeHolder.dialogue(d).Subtitle = GUILayout.TextArea(_DialogeHolder.dialogue(d).Subtitle);

                            if (SelectedTrigger.Keys.Count - 1 > SelectedTrigger.Keys.IndexOf(d))
                                MyGUIStyles.Seperator();
                        }

                        if (removeDialogue != null)
                            SelectedTrigger.Keys.Remove(removeDialogue);
                    }
                    else
                        GUILayout.Label("No dialogues in this trigger");

                    EditorGUILayout.EndScrollView();

                    EditorGUILayout.BeginVertical(GUILayout.Width(120));

                    GUILayout.Label("Availible Dialogues");

                    DialogueListScrollPos = EditorGUILayout.BeginScrollView(DialogueListScrollPos, MyGUIStyles.Options(150, this.position.height - 20));

                    foreach (Dialogue dialogue in _DialogeHolder.Dialogues)
                        if (dialogue.Key.ToLower().Contains(searchString.ToLower()) || dialogue.Subtitle.ToLower().Contains(searchString.ToLower()) || dialogue.Clip.name.ToLower().Contains(searchString.ToLower()) || (dialogue.Clip != null && dialogue.Clip.name.Contains(searchString)))
                            if (GUILayout.Button(dialogue.Key, MyGUIStyles.Button, GUILayout.Width(130)))
                            {
                                SelectedTrigger.Keys.Add(dialogue.Key);
                                changed = true;
                            }

                    EditorGUILayout.EndScrollView();
                    EditorGUILayout.EndVertical();

                    if (changed)
                        EditorUtility.SetDirty(SelectedTrigger);
                }
                EditorGUILayout.EndHorizontal();
            }
            Repaint();
        }
        catch { GetAllDialogueTriggers(); }
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

    static public void AddTriggerToArea(AreaController Area)
    {
        GameObject areaGO = (GameObject)GameObject.Instantiate(Area.gameObject);

        GameObject newTrigger = new GameObject();
        newTrigger.name = "New Dialogue Trigger";
        newTrigger.transform.parent = areaGO.transform;
        newTrigger.AddComponent<DialogueTrigger>();
        newTrigger.collider.isTrigger = true;
        newTrigger.transform.localPosition = Vector3.zero;

        PrefabUtility.ReplacePrefab(areaGO, Area.gameObject);

        DestroyImmediate(areaGO);

        GetAllDialogueTriggers();
    }

    static public void GetAllDialogueTriggers()
    {
        List<bool> foldouts = new List<bool>();

        if (Areas != null)
            foreach (AreaFoldout area in Areas)
                foldouts.Add(area.Foldout);

        int areaProgress = 0;
        int triggerProgress = 0;

        Areas = new List<AreaFoldout>();
        List<GameObject> foundAreas = MiscEditorMethods.LoadAllPrefabsWithComponent(typeof(AreaController));

        bool end = false;

        foreach (GameObject NewArea in foundAreas)
        {
            AreaFoldout area = new AreaFoldout();

            area.Area = NewArea.GetComponent<AreaController>();

            area.Triggers = new List<DialogueTrigger>(area.Area.GetComponentsInChildren<DialogueTrigger>(true));

            triggerProgress = 0;

            float Prog = ((float)areaProgress / foundAreas.Count);

            if (EditorUtility.DisplayCancelableProgressBar("Hold on", NewArea.name, Prog)) { end = true; break; }

            foreach(DialogueTrigger trigger in area.Triggers)
            {
                float barProg = ((float)areaProgress / foundAreas.Count) + (((float)triggerProgress / area.Triggers.Count) / foundAreas.Count);
                if (EditorUtility.DisplayCancelableProgressBar("Hold on", NewArea.name + "/" + trigger.name, barProg)) { end = true; break; }
                triggerProgress++;
                foreach(string k in trigger.Keys)
                {
                    bool containes = false;
                    foreach (Dialogue d in _DialogeHolder.Dialogues)
                        if (d.Key == k)
                            containes = true;
                    if (!containes)
                        trigger.Keys.Remove(k);
                }
            }
            areaProgress++;

            if (end) break;

            Areas.Add(area);
            EditorUtility.SetDirty(_DialogeHolder);
            AssetDatabase.SaveAssets();
        }
        EditorUtility.ClearProgressBar();

        for (int i = 0; i < Areas.Count; i++)
        {
            if (i < foldouts.Count)
                Areas[i].Foldout = foldouts[i];
            else
                Areas[i].Foldout = false;
        }
    }

    void MoveUp(DialogueTrigger trigger, int index)
    {
        if (index == 0) return;

        var temp = trigger.Keys[index - 1];
        trigger.Keys[index - 1] = trigger.Keys[index];
        trigger.Keys[index] = temp;

    }

    void MoveDown(DialogueTrigger trigger, int index)
    {
        if (index == trigger.Keys.Count - 1) return;

        var temp = trigger.Keys[index + 1];
        trigger.Keys[index + 1] = trigger.Keys[index];
        trigger.Keys[index] = temp;

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

        foreach (AreaFoldout Area in Areas)
        {
            foreach (DialogueTrigger trigger in Area.Area.GetComponentsInChildren<DialogueTrigger>(true))
            {
                string remove2 = null;

                foreach (string k in trigger.Keys)
                    if (k == dialogue.Key)
                        remove2 = k;

                if (remove2 != null)
                    trigger.Keys.Remove(remove2);
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
            if (GUILayout.Button(AddIcon, GUILayout.Width(30), GUILayout.Height(30))) SelectedTrigger.Keys.Add(selected.Key);
            EditorGUILayout.Space();
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.EndHorizontal();
        Debug.Log("W:" + this.position.width);
        Debug.Log("H:" + this.position.height);
    }
}

public class AreaFoldout
{
    public AreaController Area;

    public List<DialogueTrigger> Triggers;

    public bool Foldout;
}
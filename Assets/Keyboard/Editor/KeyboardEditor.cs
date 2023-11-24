using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TMPro;
using UnityEngine.UI;
using System.Linq;
using TheHangingHouse.Utility.Extensions;

[CustomEditor(typeof(Keyboard))]
public class KeyboardEditor : Editor
{
    public Keyboard keyboard;

    private bool m_foldOut = true;
    private Color m_color = Color.white;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (m_foldOut = EditorGUILayout.Foldout(m_foldOut, "Keyboard Editor"))
        {
            if (GUILayout.Button("Assign Button Texts"))
                AssignButtonTexts();
            if (GUILayout.Button("Assign Button Texts From Name"))
                AssignButtonFromNames();
            if (GUILayout.Button("Assign Key Code From Text"))
                AssignKeyCodeFromText();
            GUILayout.Space(5);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Key Color");
            m_color = EditorGUILayout.ColorField(m_color);
            if (GUILayout.Button("Apply")) ApplyKeysColor();
            GUILayout.EndHorizontal();
        }
    }

    private void OnClickKey()
    {

    }

    private void OnEnable()
    {
        keyboard = (Keyboard)target;
    }

    private void AssignButtonTexts()
    {
        var keys = keyboard.transform.GetComponentsInChildren<KeyButton>();
        foreach(var key in keys)
        {
            var tmpText = key.transform.GetChild(0).GetComponent<TMP_Text>();
            Undo.RecordObject(tmpText, "Set Text");
            tmpText.text = key.key;
        }
        Repaint();
    }

    private void AssignButtonFromNames()
    {
        var keys = keyboard.transform.GetComponentsInChildren<KeyButton>();
        Debug.Log(keys.Length);
        foreach (var key in keys)
        {
            var tmpText = key.transform.GetChild(0).GetComponent<TMP_Text>();
            Undo.RecordObject(tmpText, "Set Text From Name");
            Undo.RecordObject(key, "Set Text From Name");
            tmpText.text = key.gameObject.name;
            key.key = tmpText.text;
        }
        Repaint();
    }

    private void AssignKeyCodeFromText()
    {
        var keys = keyboard.transform.GetComponentsInChildren<KeyButton>();
        Undo.RecordObjects(keys, "Set Key Code From Text");
        foreach(var key in keys)
        {
            var tmpText = key.transform.GetChild(0).GetComponent<TMP_Text>();
            key.key = tmpText.text;
        }
        Repaint();
    }

    private void ApplyKeysColor()
    {
        var images = keyboard.transform.GetComponentsInChildren<Button>().Map(btn => btn.GetComponent<Image>());
        Undo.RecordObjects(images, "Apply Key Board Keys Color");
        foreach(var image in images)
        {
            image.color = m_color;
        }
        Repaint();
    }
}

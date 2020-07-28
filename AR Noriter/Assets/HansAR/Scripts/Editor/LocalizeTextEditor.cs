using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LocalizeText), true), CanEditMultipleObjects]
public class LocalizeTextEditor : Editor
{
    public override void OnInspectorGUI()
    {
        LocalizeText localize = target as LocalizeText;
        serializedObject.Update();

        SerializedProperty property = serializedObject.GetIterator();
        property.NextVisible(true);

        EditorGUILayout.Space();

        while (property.NextVisible(false))
        {
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(property, true);
            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                if (EditorApplication.isPlaying)
                {
                    localize.ApplyLanguage();
                }
            }
            if (string.Compare(property.name, "ValueName") == 0)
            {
                LocalizeManager localizeManager = FindObjectOfType<LocalizeManager>();
                if (localizeManager == null)
                {
                    EditorGUILayout.HelpBox("LocalizeManager does not exist in the current scene.", MessageType.Warning);
                }
                else if (!serializedObject.isEditingMultipleObjects)
                {
                    SystemLanguage language = localizeManager.CurrentLanguage;
                    if (language == SystemLanguage.Unknown)
                    {
                        language = LocalizeText.CurrentLanguage;
                    }
                    if (LocalizeValue.GetLocalizeType(language) == null)
                    {
                        EditorGUILayout.HelpBox(string.Format("Language type not found : {0}", language), MessageType.Warning);
                    }
                    else
                    {
                        EditorGUI.BeginDisabledGroup(!localizeManager.isActiveAndEnabled);
                        if (LocalizeText.Value.GetValueType(language, localize.ValueName) != typeof(string))
                        {
                            EditorGUILayout.HelpBox("Please input a valid value name.", MessageType.Info);
                        }
                        else
                        {
                            EditorGUILayout.HelpBox(LocalizeText.Value.GetString(language, localize.ValueName), MessageType.None);
                        }
                        EditorGUI.EndDisabledGroup();
                    }
                }
            }
        }
    }
}

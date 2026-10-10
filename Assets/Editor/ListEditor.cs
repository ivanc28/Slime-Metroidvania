using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[CustomEditor(typeof(DialogueGraph))] // Your target MonoBehaviour
public class ListEditor : Editor 
{
    public override void OnInspectorGUI() 
    {
        serializedObject.Update();
        serializedObject.ApplyModifiedProperties();

        DrawPropertiesExcluding(serializedObject, new string[] { "nodes" });

        EditorGUILayout.Space(15);

        // 1. Find your ScriptableObject list
        SerializedProperty listProp = serializedObject.FindProperty("nodes");

        EditorGUILayout.LabelField("Nodes", EditorStyles.boldLabel);
        
        // 2. Control the size of the list manually
        int newSize = EditorGUILayout.IntField("Size", listProp.arraySize);
        if (newSize != listProp.arraySize) listProp.arraySize = newSize;

        EditorGUI.indentLevel++;

        // 3. Loop through each element slot in the list
        for (int i = 0; i < listProp.arraySize; i++) 
        {
            SerializedProperty elementSlot = listProp.GetArrayElementAtIndex(i);

            EditorGUILayout.Space(5);
            // Draw the default drag-and-drop file slot first
            EditorGUILayout.PropertyField(elementSlot, new GUIContent($"Element {i}"));

            // 4. If a ScriptableObject file is actually dragged into this slot...
            if (elementSlot.objectReferenceValue != null) 
            {
                EditorGUI.indentLevel++;

                // 5. Create a temporary serialized link directly into that file asset
                SerializedObject nestedSO = new SerializedObject(elementSlot.objectReferenceValue);
                nestedSO.Update();

                // 6. Draw the specific fields inside that specific ScriptableObject asset
                // Make sure these strings match the variables inside your ScriptableObject script
                SerializedProperty nodeId = nestedSO.FindProperty("nodeId");
                SerializedProperty rawInput = nestedSO.FindProperty("rawInput");
                SerializedProperty lineList = nestedSO.FindProperty("lineList");

                if (nodeId != null) EditorGUILayout.PropertyField(nodeId);
                if (rawInput != null) EditorGUILayout.PropertyField(rawInput);
                if (lineList != null) EditorGUILayout.PropertyField(lineList);

                // Save any changes back to the actual asset file on your hard drive
                nestedSO.ApplyModifiedProperties();

                EditorGUI.indentLevel--;
            }
            else 
            {
                // If the slot is empty, show a small warning or note
                EditorGUI.indentLevel++;
                EditorGUILayout.HelpBox("Drag an ItemData ScriptableObject file here to view fields.", MessageType.None);
                EditorGUI.indentLevel--;
            }
        }

        EditorGUI.indentLevel--;

        serializedObject.ApplyModifiedProperties();
    }
}
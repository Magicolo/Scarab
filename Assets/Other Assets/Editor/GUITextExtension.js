@CustomEditor (GUIText)
class GUITextExtension extends Editor{
    function OnInspectorGUI () {
        target.text = EditorGUILayout.TextArea(target.text);
        DrawDefaultInspector();
    }
}
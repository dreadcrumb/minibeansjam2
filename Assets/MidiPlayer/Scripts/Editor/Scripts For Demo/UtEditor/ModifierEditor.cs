using Assets.MidiPlayer.Scenes_For_Demo.Demo_InfinityMusic.Script;
using Assets.MidiPlayer.Scripts.MPTKGameObject;
using UnityEditor;
using UnityEngine;

namespace Assets.MidiPlayer.Scripts.Editor.Scripts_For_Demo.UtEditor
{
    /// <summary>
    /// Useful to change on live some parameters of others components
    /// Work in progress
    /// </summary>
    [CanEditMultipleObjects]
    [CustomEditor(typeof(UtModifier))]
    public class ModifierEditor : UnityEditor.Editor
    {
        private static UtModifier instance;
        private GUIStyle styleBold;

        void OnEnable()
        {
            try
            {
                instance = (UtModifier)target;
            }
            catch (System.Exception ex)
            {
                MidiPlayerGlobal.ErrorDetail(ex);
            }
        }

        public override void OnInspectorGUI()
        {
            try
            {
                GUI.changed = false;
                GUI.color = Color.white;
                styleBold = new GUIStyle("Label")
                {
                    fontSize = 12,
                    fontStyle = FontStyle.Bold
                };

                EditorGUILayout.Space();
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button(new GUIContent("+", ""), GUILayout.ExpandWidth(false)))
                    instance.Components.Add(null);
                EditorGUILayout.EndHorizontal();

                for (int i=0; i< instance.Components.Count;i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    instance.Components[i] = (UtComponent)EditorGUILayout.ObjectField(new GUIContent("Component"), instance.Components[i], typeof(UtComponent), true);
                    if (GUILayout.Button(new GUIContent("-", ""), GUILayout.ExpandWidth(false)))
                        instance.Components.Add(null);
                    EditorGUILayout.EndHorizontal();

                }
                //for (int i = 0; i < instance.Components.Count;)
                //    if (instance.Components[i] == null)
                //        instance.Components.RemoveAt(i);
                //    else
                //        i++;

                //var reorderableList = new UnityEditorInternal.ReorderableList(instance.Components, typeof(UtMathMotif), true, true, true, true);
                ////reorderableList.DoList(rect);
                ////or
                //reorderableList.DoLayoutList();

                //
                // Running
                //
                if (Application.isPlaying)
                {
                    EditorGUILayout.LabelField(new GUIContent("Measure", ""),
                        new GUIContent(string.Format("{0,3:000}.{1,2:00}.{2,2:00}",
                        InfinityMusic.instance.IndexMeasure + 1, InfinityMusic.instance.IndexQuarterMeasure + 1, InfinityMusic.instance.IndexSixteenthMeasure + 1), ""), styleBold);
                }

                //
                // Actions
                //
                if (Application.isPlaying)
                {
                    EditorGUILayout.Space();
                    EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button(new GUIContent("Generate", ""), GUILayout.ExpandWidth(false)))
                        instance.Generate(false);
                    EditorGUILayout.EndHorizontal();
                }

                showDefault = EditorGUILayout.Foldout(showDefault, "Show default editor");
                if (showDefault) DrawDefaultInspector();

                if (GUI.changed) EditorUtility.SetDirty(instance);
            }
            catch (System.Exception ex)
            {
                MidiPlayerGlobal.ErrorDetail(ex);
            }
        }
        private static bool showDefault = true;


    }

}

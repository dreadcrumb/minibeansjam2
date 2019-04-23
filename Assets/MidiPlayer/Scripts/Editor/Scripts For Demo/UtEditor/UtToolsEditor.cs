using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Assets.MidiPlayer.Scripts.Editor.Scripts_For_Demo.UtEditor
{
    class UtToolsEditor
    {
        public const BindingFlags flags = BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static;

        static public int IntSlider(string title, string help, string name, int value, Type type)
        {
            bool ok = false;
            FieldInfo field= type.GetField(name, flags);
            Attribute[] atts = (System.Attribute[])field.GetCustomAttributes(false);
            foreach (Attribute att in atts)
            {
                if (att is RangeAttribute)
                {
                    RangeAttribute range = (RangeAttribute)att;
                    value = EditorGUILayout.IntSlider(new GUIContent (title, help), value, (int)range.min, (int)range.max);
                    //Debug.Log(field.Name + " " + range.min + " " + range.max);
                    ok = true;
                    break;
                }
            }
            if (!ok)
                Debug.LogWarning("Range not defined for " + name);
            return value;
        }
    }
}

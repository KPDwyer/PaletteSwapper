using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace KPD.PaletteSwapper
{

    [System.Serializable]
    /// <summary>
/// Class Handles ColorOperation (replace) UI and functionality.
/// Next Step would be to split this into a BaseOperation class and a Replace Class.
/// </summary>
public class ColorOperation
    {
        [SerializeField]
        public Color sampledColor;

        [SerializeField]
        public Color targetColor;

        public bool valChanged = false;

        private Color reuseme;


        public void DrawColorOperationGUI()
        {
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUIUtility.labelWidth = 60.0f;
                EditorGUILayout.LabelField("Color Replace");
                sampledColor = EditorGUILayout.ColorField(sampledColor);
                targetColor = EditorGUILayout.ColorField(targetColor);
            }
            EditorGUILayout.EndHorizontal();

        }

        public Color OperateOnColor(Color source)
        {
            reuseme = source;
            valChanged = false;
            if (!Dif(source, sampledColor))
            {

                reuseme.r = targetColor.r;
                reuseme.g = targetColor.g;
                reuseme.b = targetColor.b;
                valChanged = true;
            }
           
            return reuseme;
        }

        public bool Dif(Color one, Color two)
        {

            if (one.r > two.r + 0.01f || one.r < two.r - 0.01f)
                return true;

            if (one.g > two.g + 0.01f || one.g < two.g - 0.01f)
                return true;

            if (one.b > two.b + 0.01f || one.b < two.b - 0.01f)
                return true;


            return false;
       
        }
    }
}
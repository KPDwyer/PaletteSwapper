using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace KPD.PaletteSwapper
{
    [CustomEditor(typeof(PaletteSwapObject))]
    public class PalettSwapObjectEditor : Editor
    {

        TextureBuilder texBuilder;

        SpritePreviewEditor spritePreview;
        PaletteSwapObject swapObject;

        private bool previewFocus = false;


        public override void OnInspectorGUI()
        {

            swapObject = target as PaletteSwapObject;

            if (texBuilder == null)
            {
                texBuilder = new TextureBuilder();
            }

            GUIPaletteSwapper();


            if (previewFocus)
            {
                spritePreview.Focus();
                previewFocus = false;
            }
        }

        /// <summary>
        /// Shows the preview Texture.
        /// </summary>
        /// <param name="_tex">The texture to preview</param>
        private void ShowPreview(Texture2D _tex)
        {
            if (spritePreview == null)
            {
                spritePreview = (SpritePreviewEditor)EditorWindow.GetWindow(typeof(SpritePreviewEditor), false, "Sprite Preview");
                spritePreview.Show();
            }
            spritePreview.SetTexture(_tex);
        }

        /// <summary>
        /// Previews the texture.
        /// </summary>
        private void PreviewTexture()
        {
            ShowPreview(texBuilder.GetTexture(swapObject.ColorOps));
        }


        /// <summary>
        /// Triggers a preview than asks the TextureBuilder to save the texture
        /// </summary>
        private void SaveTexture()
        {
            PreviewTexture();
            texBuilder.SaveTexture();
        }

        /// <summary>
        /// Menu Option to Create a PaletteSwapObject
        /// </summary>
        [MenuItem("Tools/New PaletteSwap")]
        static void CreateNewPaletteSwapObject()
        {
            PaletteSwapObject temp = ScriptableObject.CreateInstance<PaletteSwapObject>();
            temp.Filename = "New Palette Swap Object";
            temp.ColorOps = new System.Collections.Generic.List<ColorOperation>();

            string AssetPath = AssetDatabase.GenerateUniqueAssetPath("Assets/" + temp.Filename + ".asset");
            AssetDatabase.CreateAsset(temp, AssetPath);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = temp;

        }


        private void GUIPaletteSwapper()
        {
            if (swapObject != null)
            {
                swapObject.SourceAsset = (Texture2D)EditorGUILayout.ObjectField("Sprite", swapObject.SourceAsset, typeof(Texture2D), false);

                if (swapObject.SourceAsset != null && texBuilder.SourceAsset != swapObject.SourceAsset)
                {
                    //this code ensures the texture is readable
                    string assetpath = AssetDatabase.GetAssetPath(swapObject.SourceAsset);
                    TextureImporter tImporter = AssetImporter.GetAtPath(assetpath) as TextureImporter;
                    if (tImporter != null)
                    {
                        tImporter.isReadable = true;
                        AssetDatabase.ImportAsset(assetpath);
                        AssetDatabase.Refresh();
                    }


                    texBuilder.SetTextureSource(swapObject.SourceAsset);
                }

                if (swapObject.SourceAsset == null)
                    return;

                if (GUILayout.Button("Reset/Load Source Palette"))
                {
                    List<Color> colors = texBuilder.GetSourceTextureColors();
                    swapObject.ColorOps.Clear();
                    foreach (Color c in colors)
                    {
                        ColorOperation op = new ColorOperation();
                        op.sampledColor = c;

                        swapObject.ColorOps.Add(op);
                    }

                }

                ColorOperation deleteOp = null;
                foreach (ColorOperation co in swapObject.ColorOps)
                {
                    EditorGUILayout.BeginHorizontal();
                    {
                        GUI.color = Color.red;
                        if (GUILayout.Button("-"))
                        {
                            deleteOp = co;
                        }
                        GUI.color = Color.white;

                        co.DrawColorOperationGUI();
                    }
                    EditorGUILayout.EndHorizontal();
                }

                if (GUILayout.Button("Add Swap"))
                {
                    swapObject.ColorOps.Add(new ColorOperation());
                }

                if (deleteOp != null)
                {
                    swapObject.ColorOps.Remove(deleteOp);
                }
                swapObject.Filename = texBuilder.fileName = EditorGUILayout.TextField("Filename", swapObject.Filename);
            }


            GUILayout.BeginHorizontal();
            {

                if (GUILayout.Button("Preview"))
                {
                    PreviewTexture();
                }

                if (GUILayout.Button("Save Texture"))
                {
                    SaveTexture();
                }
            }
            GUILayout.EndHorizontal();



        }


    }

}
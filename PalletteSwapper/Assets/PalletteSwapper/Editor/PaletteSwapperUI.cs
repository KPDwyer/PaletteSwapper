using UnityEngine;
using UnityEditor;

namespace PaletteSwapper
{

    /// <summary>
    /// Handles UI for PaletteSwapper's main window.
    /// Might be a bit messy, loosely based on SpriteMaker.
    /// </summary>
    public class PaletteSwapperUI : EditorWindow
    {

        TextureBuilder texBuilder;

        SpritePreviewEditor spritePreview;

        public PaletteSwapObject swapObject;



        private bool previewFocus = false;




        [MenuItem("Window/PaletteSwapper")]
        static void Init()
        {
            PaletteSwapperUI window = (PaletteSwapperUI)EditorWindow.GetWindow(typeof(PaletteSwapperUI), false, "Palette Swapper");
            window.texBuilder = new TextureBuilder();
            window.Show();
        }

        void OnGUI()
        {

            //if the editor window was left open on a new Unity bootup, init doesn't get called.
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


        private void GUIPaletteSwapper()
        {
            //loadAsset = (SpriteMakerAsset)EditorGUILayout.ObjectField(activeAsset, typeof(SpriteMakerAsset), false);

            swapObject = (PaletteSwapObject)EditorGUILayout.ObjectField(swapObject, typeof(PaletteSwapObject), false);

            if (GUILayout.Button("New Swap Object"))
            {
                PaletteSwapObject temp = ScriptableObject.CreateInstance<PaletteSwapObject>();
                temp.Filename = "New Palette Swap Object";
                temp.ColorOps = new System.Collections.Generic.List<ColorOperation>();

                string AssetPath = AssetDatabase.GenerateUniqueAssetPath("Assets/" + temp.Filename + ".asset");
                AssetDatabase.CreateAsset(temp, AssetPath);

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                swapObject = temp;
                EditorUtility.FocusProjectWindow();
                Selection.activeObject = temp;


            }


            if (swapObject != null)
            {
                swapObject.Filename = texBuilder.fileName = EditorGUILayout.TextField("Asset File Name", swapObject.Filename);
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
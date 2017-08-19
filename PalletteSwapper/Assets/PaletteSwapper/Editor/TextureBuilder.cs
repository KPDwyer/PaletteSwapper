using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;

namespace KPD.PaletteSwapper
{
    /// <summary>
    /// A modified version of SpriteMaker's TextureBuilder.  
    /// Next Steps would be to create a baseclass with shared code, then build tool-specific subclasses/wrappers
    /// </summary>
    public class TextureBuilder
    {

        // arbitrary default values
        public string fileName = "Asset";
        public Texture2D SourceAsset;
        public int pixelWidth = 64;
        public int pixelHeight = 64;



        //Texture2D that we are manipulating
        private Texture2D tex;

        private Color[] sourcePixels;


        public TextureBuilder()
        {
        }

        public void SetTextureSource(Texture2D source)
        {
            SourceAsset = source;
            tex = new Texture2D(source.width, source.height, TextureFormat.ARGB32, false);


            sourcePixels = source.GetPixels(); 
        }

        /// <summary>
        /// returns a listof all the unique colors in the source image.
        /// </summary>
        /// <returns>The source texture colors.</returns>
        public List<Color> GetSourceTextureColors()
        {
            List<Color> result = new List<Color>();
            for (int i = 0; i < sourcePixels.Length; i++)
            {
                if (sourcePixels[i].a == 0)
                    continue;
                
                if (!result.Contains(sourcePixels[i]))
                {
                    result.Add(new Color(sourcePixels[i].r, sourcePixels[i].g, sourcePixels[i].b, sourcePixels[i].a));
                }
            }   
            result.Sort(delegate(Color x, Color y)
                {
                    return y.maxColorComponent.CompareTo(x.maxColorComponent);                
                });


            return result;
        }


        /// <summary>
        /// Gets the texture in its current state
        /// </summary>
        /// <returns>The texture.</returns>
        public Texture2D GetTexture(List<ColorOperation> ops)
        {
            
            Color[] colorArray = new Color[sourcePixels.Length];

            List<int> operatedIndices = new List<int>();


            for (int i = 0; i < sourcePixels.Length; i++)
            {
                colorArray[i] = new Color(sourcePixels[i].r, sourcePixels[i].g, sourcePixels[i].b, sourcePixels[i].a);
            }


            for (int i = 0; i < colorArray.Length; i++)
            {
           
                //ignore ones we'vealready operated on
                if (operatedIndices.Contains(i))
                {
                    continue;
                }

                foreach (ColorOperation op in ops)
                {
                    
                    colorArray[i] = op.OperateOnColor(colorArray[i]);
 
                    if (op.valChanged)
                    {
                        //using list here is bad.
                        //operatedIndices.Add(i);
                        break;
                    }

                }


            }


            tex.SetPixels(colorArray);

            tex.Apply();

            return tex;
        }



        /// <summary>
        /// Saves the current Texture2D to a PNG
        /// </summary>
        public void SaveTexture()
        {

            byte[] bytes = tex.EncodeToPNG();

            CheckVars();

            File.WriteAllBytes(Application.dataPath + "/" + fileName + ".png", bytes);


            AssetDatabase.Refresh();
        }


        /// <summary>
        /// Checks to make sure some inportant variables aren't null or empty
        /// </summary>
        private void CheckVars()
        {
            if (pixelHeight == pixelWidth && pixelHeight == 0)
            {
                pixelWidth = pixelHeight = 64; //default to 64x64
            }

            if (string.IsNullOrEmpty(fileName))
            {
                fileName = "Asset";
				
            }
        }
    }
}

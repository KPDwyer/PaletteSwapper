using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KPD.PaletteSwapper
{

    /// <summary>
    /// ScriptableObject that retains palette swapping data (once you've set it up once, you can load this object
    /// to make further tweaks without having to redefine your palette).
    /// </summary>
    public class PaletteSwapObject : ScriptableObject
    {
        [SerializeField]
        public Texture2D SourceAsset;
        [SerializeField]
        public string Filename;
        [SerializeField]
        public List<ColorOperation> ColorOps;
    }
}
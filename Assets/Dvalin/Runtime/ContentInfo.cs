using System.Collections.Generic;
using UnityEngine;

namespace Dvalin
{
    public class ContentInfo
    {
        public PieceInfo[] Pieces { get; protected set; }
        public TextAsset Localization { get; protected set; }

        public ContentInfo(List<PieceInfo> pieces, TextAsset localization)
        {
            Pieces = pieces.ToArray();
            Localization = localization; 
        }
    }
}
using System.Collections.Generic;
using UnityEngine;

namespace Dvalin
{
    public class ContentInfo
    {
        public PieceInfo[] Pieces { get; protected set; }
        public ItemInfo[] Items { get; protected set; }
        public RecipeWrapper[] Recipes { get; protected set; }
        public SmelterItemConversionWrapper[] SmelterItemConversions { get; protected set; }
        public TextAsset Localization { get; protected set; }

        public ContentInfo(
            List<PieceInfo> pieces,
            List<ItemInfo> items,
            List<RecipeWrapper> recipes,
            List<SmelterItemConversionWrapper> smelterItemConversions,
            TextAsset localization)
        {
            Pieces = pieces.ToArray();
            Items = items.ToArray();
            Recipes = recipes.ToArray();
            SmelterItemConversions = smelterItemConversions.ToArray();
            Localization = localization;
        }
    }

    public class ItemInfo : IDestroyable
    {
        public ItemDropWrapper Prefab { get; protected set; }

        public ItemInfo(ItemDropWrapper prefab)
        {
            Prefab = prefab;
        }

        public void Destroy()
        {
            Object.Destroy(Prefab);

            Logger.LogInfoFormat("{0} destroyed.", this);
        }

        public override string ToString()
        {
            return string.Format("{0} (Item)", Prefab.name);
        }
    }
}
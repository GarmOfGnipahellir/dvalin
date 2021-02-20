using System.IO;
using System.Collections.Generic;
using UnityEngine;
using Dvalin.Core;

namespace Dvalin.API
{
    public class DvalinPiece
    {
        public Piece Prefab { get; protected set; }

        private DvalinPiece(Piece prefab)
        {
            Prefab = prefab;
        }

        public static bool TryCreate(string pieceName, out DvalinPiece dvalinPiece)
        {
            try
            {
                var assetBundlePath = Path.Combine(DvalinPlugin.PluginDir, pieceName);
                var assetBundle = AssetBundle.LoadFromFile(assetBundlePath);
                var gameObject = assetBundle.LoadAsset<GameObject>(pieceName);
                var piece = gameObject.GetComponent<Piece>();

                Debug.Assert(piece); // make sure piece is valid

                assetBundle.Unload(false);

                dvalinPiece = new DvalinPiece(piece);
                return true;
            }
            catch (System.NullReferenceException e)
            {
                DvalinLogger.LogError(e);
            }

            dvalinPiece = null;
            return false;
        }

        public void Destroy()
        {
            Object.Destroy(Prefab);

            DvalinLogger.LogInfoFormat("{0} destroyed.", this);
        }

        public void AddToTable(Player player)
        {
            if (player != Player.m_localPlayer) return;

            PieceTable hammerTable = null;
            var tables = new List<PieceTable>();
            player.GetInventory().GetAllPieceTables(tables);
            foreach (var table in tables)
            {
                if (table.name.StartsWith("_Hammer"))
                {
                    hammerTable = table;
                }
            }

            if (!hammerTable)
            {
                DvalinLogger.LogErrorFormat("Couldn't find hammer table for: {0}", player);
                return;
            }

            var shouldAdd = true;
            foreach (var go in hammerTable.m_pieces)
            {
                var piece = go.GetComponent<Piece>();
                if (piece.m_name == Prefab.m_name)
                {
                    DvalinLogger.LogInfoFormat("{0} already in m_pieces", piece.name);
                    shouldAdd = false;
                    break;
                }
            }
            if (shouldAdd)
            {
                hammerTable.m_pieces.Add(Prefab.gameObject);
                DvalinLogger.LogInfoFormat("{0} added to m_pieces", Prefab.m_name);

                var known = player.IsRecipeKnown(Prefab.m_name);
                DvalinLogger.LogInfoFormat("{0} known? {1}", Prefab.m_name, known);
                if (!known)
                {
                    player.AddKnownPiece(Prefab);
                    DvalinLogger.LogInfoFormat("{0} added to known", Prefab.m_name);
                }
            }
        }

        public override string ToString()
        {
            return string.Format("{0} (DvalinPiece)", Prefab.name);
        }
    }
}

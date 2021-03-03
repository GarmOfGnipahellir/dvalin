using System.IO;
using System.Collections.Generic;
using UnityEngine;

namespace Dvalin
{
    /// <summary>
    /// Handles the loading and adding of pieces to the game.
    /// </summary>
    [System.Serializable]
    public class PieceInfo : IDestroyable
    {
        public PieceWrapper Prefab { get; protected set; }

        public PieceInfo(PieceWrapper prefab)
        {
            Prefab = prefab;
        }

        public void Destroy()
        {
            Object.Destroy(Prefab);

            Logger.LogInfoFormat("{0} destroyed.", this);
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
                Logger.LogErrorFormat("Couldn't find hammer table for: {0}", player);
                return;
            }

            var shouldAdd = true;
            foreach (var go in hammerTable.m_pieces)
            {
                var piece = go.GetComponent<Piece>();
                if (piece.m_name == Prefab.m_name)
                {
                    Logger.LogInfoFormat("{0} already in m_pieces", piece.name);
                    shouldAdd = false;
                    break;
                }
            }
            if (shouldAdd)
            {
                hammerTable.m_pieces.Add(Prefab.gameObject);
                Logger.LogInfoFormat("{0} added to m_pieces", Prefab.m_name);

                // var known = player.IsRecipeKnown(Prefab.m_name);
                // Logger.LogInfoFormat("{0} known? {1}", Prefab.m_name, known);
                // if (!known)
                // {
                //     player.AddKnownPiece(Prefab);
                //     Logger.LogInfoFormat("{0} added to known", Prefab.m_name);
                // }
            }
        }

        public override string ToString()
        {
            return string.Format("{0} (Piece)", Prefab.name);
        }
    }
}
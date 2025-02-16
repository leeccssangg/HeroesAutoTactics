using System;
using System.Collections.Generic;
using UnityEngine;
using TW.Reactive.CustomComponent;

namespace Game.Data
{
    [Serializable]
    public class HeroUpgradeData
    {
        [field: SerializeField] public List<EachHeroUpgradeData> Data { get; private set; }
    }
    [Serializable]
    public class EachHeroUpgradeData
    {
        [field: SerializeField] public int HeroId { get; private set; }
        [field: SerializeField] public ReactiveValue<int> Level { get; private set; }
        [field: SerializeField] public ReactiveValue<int> Piece { get; private set; }

        public EachHeroUpgradeData(int heroId, int level, int piece)
        {
            HeroId = heroId;
            Level = new(level);
            Piece = new(piece);
        }
        public void AddLevel(int level)
        {
            Level.Value += level;
        }
        public void AddPiece(int piece)
        {
            Piece.Value += piece;
        }
        public void RemovePiece(int piece) 
        {
            Piece.Value -= piece;
        }
    }
}

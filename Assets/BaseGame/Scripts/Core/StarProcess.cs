using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace Game.Core
{
    public class StarProcess : MonoBehaviour
    {
        private static string[] Star { get; set; } = {"Lvl1", "Lvl1", "Lvl2", "Lvl2", "Lvl2", "Lvl3"};
        private static string[] Process { get; set; } = {"000", "100", "000", "100", "110", "111"};
        [field: SerializeField] public TextMeshPro TextStar { get; private set; }
        [field: SerializeField] public GameObject[] ProcessBackground { get; private set; }
        [field: SerializeField] public GameObject[] StarStroke { get; private set; }
        [field: SerializeField] public GameObject[] ProcessIcon { get; private set; }
    
        [Button]
        public void SetPieces(int piece)
        {
            TextStar.SetText(Star[piece - 1]);
            ProcessBackground[2].SetActive(piece > 2);
        
            // LevelStroke[0].SetActive(Level[piece - 1] == "1");
            // LevelStroke[1].SetActive(Level[piece - 1] == "2");
            // LevelStroke[2].SetActive(Level[piece - 1] == "3");

            ProcessIcon[0].SetActive(Process[piece - 1][0] == '1');
            ProcessIcon[1].SetActive(Process[piece - 1][1] == '1');
            ProcessIcon[2].SetActive(Process[piece - 1][2] == '1');
        }
    }
}
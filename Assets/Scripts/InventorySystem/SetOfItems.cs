using System.Collections.Generic;
using ImportedScripts;
using UnityEngine;

namespace CharacterCustomNGO
{
    [CreateAssetMenu(fileName = "New " + nameof(SetOfItems), menuName = "Items/" + nameof(SetOfItems))]
    public class SetOfItems : ScriptableObject
    {
        [SerializeField] private List<ItemAsset> itemSet;

        public List<ItemAsset> ItemSet => itemSet;
        public int AmountOfItems => itemSet.SafeCount();
    }
}
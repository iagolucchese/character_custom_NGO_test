using NaughtyAttributes;
using UnityEngine;

namespace CharacterCustomNGO
{
    [CreateAssetMenu(fileName = "New " + nameof(ItemAsset), menuName = "Items/" + nameof(ItemAsset))]
    public class ItemAsset : ScriptableObject
    {
        [Header("Currency Values")]
        [SerializeField, Min(0)] protected int itemCost;
        [SerializeField, Min(0)] protected int itemSellValue;
        [Header("Visual & UI")]
        [SerializeField] protected string itemName;
        [SerializeField, ShowAssetPreview(width:64, height:64)] protected Sprite itemIcon;

        public int ItemCost => itemCost;
        public int ItemSellValue => itemSellValue;
        public string ItemName => itemName;
        public Sprite ItemIcon => itemIcon;
    }
}
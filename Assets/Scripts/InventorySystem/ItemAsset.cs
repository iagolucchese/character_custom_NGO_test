using NaughtyAttributes;
using Unity.Netcode;
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
    
    [System.Serializable]
    public class ItemReference : INetworkSerializable
    {
        public ItemAsset item;

        public ItemReference()
        {
            item = null;
        }
        public ItemReference(ItemAsset item)
        {
            this.item = item;
        }
        
        public static implicit operator ItemAsset(ItemReference itemRef) => itemRef.item;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            if (serializer.IsReader)
            {
                serializer.GetFastBufferReader().ReadValueSafe(out string assetName);
                item = Resources.Load<ItemAsset>("Items/" + assetName); //new AssetReference(assetName);
            }
            else 
            {
                serializer.GetFastBufferWriter().WriteValueSafe(item.name);
            }
            /*ItemEquipmentAsset assetRef = this;
            serializer.SerializeValue(ref assetRef);*/
        }
    }
}
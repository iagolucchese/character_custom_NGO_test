using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace CharacterCustomNGO
{
    [CreateAssetMenu(fileName = "New " + nameof(ItemEquipmentAsset), menuName = "Items/" + nameof(ItemEquipmentAsset))]
    public class ItemEquipmentAsset : ItemAsset
    {
        [Header("Outfit")]
        [SerializeField] private Mesh outfitMesh;
        [SerializeField] private Material outfitMaterial;
        [SerializeField] private Vector2 outfitShaderOffset;
        [SerializeField] private List<EquipmentSlot> validSlots;
        [SerializeField] private List<EquipmentSlot> hidesOtherSlots;

        public Mesh OutfitMesh => outfitMesh;
        public Material OutfitMaterial => outfitMaterial;
        public Vector2 OutfitShaderOffset => outfitShaderOffset;
        public List<EquipmentSlot> ValidSlots => validSlots;
        public List<EquipmentSlot> HidesOtherSlots => hidesOtherSlots;
    }

    [System.Serializable]
    public class EquipmentReference : INetworkSerializable
    {
        public ItemEquipmentAsset equipment;

        public EquipmentReference()
        {
            equipment = null;
        }
        public EquipmentReference(ItemEquipmentAsset equipment)
        {
            this.equipment = equipment;
        }
        
        public static implicit operator ItemEquipmentAsset(EquipmentReference equipRef) => equipRef.equipment;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            if (serializer.IsReader)
            {
                serializer.GetFastBufferReader().ReadValueSafe(out string assetName);
                equipment = Resources.Load<ItemEquipmentAsset>("Equipments/" + assetName); //new AssetReference(assetName);
            }
            else 
            {
                serializer.GetFastBufferWriter().WriteValueSafe(equipment.name);
            }
            /*ItemEquipmentAsset assetRef = this;
            serializer.SerializeValue(ref assetRef);*/
        }
    }
}
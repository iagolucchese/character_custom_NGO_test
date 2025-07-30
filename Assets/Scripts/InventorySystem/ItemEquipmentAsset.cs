using System.Collections.Generic;
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
}
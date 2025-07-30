using System.Collections.Generic;
using ImportedScripts;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Assertions;

namespace CharacterCustomNGO
{
    public class EquippedOutfitVisuals : MonoBehaviour
    {
        [System.Serializable]
        public class EquipSlotVisual
        {
            public EquipmentSlot slot;
            public Renderer outfitRenderer;
            //public Material outfitMaterial;
        }
        
        [SerializeField] private EquipmentHolder equipmentHolder;
        [SerializeField] private List<EquipSlotVisual> slotPairs;
        
        #region Unity Messages
        private void Awake()
        {
            Assert.IsNotNull(equipmentHolder);

            equipmentHolder.OnItemEquipped += ItemEventCallback;
            equipmentHolder.OnItemUnequipped += ItemEventCallback;
        }

        private void OnEnable()
        {
            UpdateAllOutfits();
        }

        private void OnDestroy()
        {
            if (equipmentHolder != null)
            {
                equipmentHolder.OnItemEquipped -= ItemEventCallback;
                equipmentHolder.OnItemUnequipped -= ItemEventCallback;
            }
        }
        #endregion

        #region Private Methods
        [Button]
        private void UpdateAllOutfits()
        {
            List<EquipmentSlot> slotsToHide = new();
            foreach (SlotEquipmentPair slotPair in equipmentHolder.EquipmentSlots)
            {
                if (slotPair == null) continue;
                UpdateOutfitVisualForSlot(slotPair.slot, slotPair.item);
                if (slotPair.item != null)
                    slotsToHide.AddIfNew(slotPair.item.HidesOtherSlots);
            }

            HideSlots(slotsToHide);
            this.SetAsDirty();
        }
        
        private void UpdateOutfitVisualForSlot(EquipmentSlot slot, ItemEquipmentAsset equipmentAsset)
        {
            if (slot == null) return;

            foreach (EquipSlotVisual slotPair in slotPairs)
            {
                if (slotPair == null) continue;
                if (slotPair.slot != slot) continue;

                SetOutfit(slotPair, equipmentAsset);
            }
        }
        
        private void HideSlots(List<EquipmentSlot> slotsToHide)
        {
            foreach (EquipSlotVisual pair in slotPairs)
            {
                if (slotsToHide.Contains(pair.slot)) 
                    SetOutfit(pair, null);
            }
        }

        private void ItemEventCallback(EquipmentHolder holder, EquipmentSlot slot, ItemEquipmentAsset equipmentAsset)
        {
            UpdateAllOutfits();
        }

        private static void SetOutfit(EquipSlotVisual pair, ItemEquipmentAsset equipment)
        {
            if (equipment == null) return;
            
            if (pair.outfitRenderer is SkinnedMeshRenderer skinnedMesh)
            {
                skinnedMesh.sharedMesh = equipment.OutfitMesh;
            }
            else if (pair.outfitRenderer.TryGetComponent(out MeshFilter filter))
            {
                filter.sharedMesh = equipment.OutfitMesh;
            }
            pair.outfitRenderer.material = equipment.OutfitMaterial;
        }
        #endregion
    }
}
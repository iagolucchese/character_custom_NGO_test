using System;
using System.Collections.Generic;
using ImportedScripts;
using UnityEngine;
using UnityEngine.Assertions;

namespace CharacterCustomNGO
{
    public class EquipmentHolder : MonoBehaviour
    {
        public delegate void EquipmentEvent(EquipmentHolder holder, EquipmentSlot slot, ItemEquipmentAsset equipmentAsset);
        public event EquipmentEvent OnItemEquipped;
        public event EquipmentEvent OnItemUnequipped;

        [SerializeField] private InventoryHolder inventoryHolder;
        [SerializeField] private List<SlotEquipmentPair> equipmentSlots;

        public List<SlotEquipmentPair> EquipmentSlots => equipmentSlots;

        #region Unity Messages
        private void OnEnable()
        {
            Assert.IsTrue(equipmentSlots.IsValidAndNotEmpty());

            inventoryHolder.OnItemRemoved += InventoryItemRemovedCallback;
        }

        private void OnValidate()
        {
            if (Application.isPlaying) return;
            ValidateEquipment();
        }
        #endregion

        #region Public Methods
        public bool TryEquipItem(ItemEquipmentAsset newItem)
        {
            if (newItem == null) return false;
            foreach (SlotEquipmentPair slotPair in equipmentSlots)
            {
                if (newItem.ValidSlots.Contains(slotPair.slot) == false) continue;

                return EquipItemToSlot(newItem, slotPair.slot);
            }

            return false;
        }
        
        public bool EquipItemToSlot(ItemEquipmentAsset newItem, EquipmentSlot slot)
        {
            if (newItem == null || slot == null) return false;
            foreach (SlotEquipmentPair slotPair in equipmentSlots)
            {
                if (slotPair.slot != slot) continue;
                if (newItem.ValidSlots.Contains(slot) == false) return false;
                
                if (slotPair.item != null) 
                    OnItemUnequipped?.Invoke(this, slotPair.slot, slotPair.item);
                slotPair.item = newItem;
                OnItemEquipped?.Invoke(this, slot, newItem);
                return true;
            }

            return false;
        }

        public bool UnequipItemAtSlot(EquipmentSlot slot)
        {
            if (slot == null) return false;
            foreach (SlotEquipmentPair slotPair in equipmentSlots)
            {
                if (slotPair.slot != slot) continue;
                if (slotPair.item != null)
                {
                    slotPair.item = null;
                    OnItemUnequipped?.Invoke(this, slotPair.slot, slotPair.item);
                    return true;
                }
                return false;
            }
            return false;
        }

        public bool UnequipItem(ItemEquipmentAsset item)
        {
            if (item == null) return false;
            foreach (SlotEquipmentPair slotPair in equipmentSlots)
            {
                if (slotPair.item != item) continue;
                slotPair.item = null;
                OnItemUnequipped?.Invoke(this, slotPair.slot, slotPair.item);
                return true;
            }
            return false;
        }

        public bool IsItemEquipped(ItemAsset item)
        {
            if (item == null) return false;
            foreach (SlotEquipmentPair slotPair in equipmentSlots)
            {
                if (slotPair.item == item)
                    return true;
            }
            return false;
        }

        public bool ToggleItemEquipped(ItemEquipmentAsset item)
        {
            bool isEquipped = IsItemEquipped(item);
            return isEquipped ? UnequipItem(item) : TryEquipItem(item);
        }
        #endregion
        
        #region Private Methods
        private void ValidateEquipment()
        {
            foreach (SlotEquipmentPair pair in equipmentSlots)
            {
                if (pair.item.ValidSlots.Contains(pair.slot) == false) 
                    pair.item = null;
            }
        }
        
        private void InventoryItemRemovedCallback(InventoryHolder holder, ItemAsset item)
        {
            if (inventoryHolder != holder) return;

            UnequipItem(item as ItemEquipmentAsset);
        }
        #endregion
    }
}
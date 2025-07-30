using System.Collections.Generic;
using ImportedScripts;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Assertions;

namespace CharacterCustomNGO
{
    [System.Serializable]
    public class SlotEquipmentPair
    {
        public EquipmentSlot slot;
        public EquipmentReference itemRef;

        public SlotEquipmentPair(EquipmentSlot slot, ItemEquipmentAsset item)
        {
            this.slot = slot;
            itemRef = new(item);
        }
    }
    
    public class EquipmentHolder : NetworkBehaviour
    {
        public delegate void EquipmentEvent(EquipmentHolder holder, EquipmentSlot slot, ItemEquipmentAsset equipmentAsset);
        public event EquipmentEvent OnItemEquipped;
        public event EquipmentEvent OnItemUnequipped;

        [SerializeField] private InventoryHolder inventoryHolder;
        [SerializeField] private List<SlotEquipmentPair> equipmentSlots;
        
        public List<SlotEquipmentPair> EquipmentSlots => equipmentSlots;

        #region Unity Messages
        private void Awake()
        {
            Assert.IsTrue(equipmentSlots.IsValidAndNotEmpty());
        }

        protected override void OnNetworkPostSpawn()
        {
            base.OnNetworkPostSpawn();
            if (IsOwner)
                inventoryHolder.OnItemRemoved += InventoryItemRemovedCallback;
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
            if (inventoryHolder != null)
                inventoryHolder.OnItemRemoved -= InventoryItemRemovedCallback;
        }

        private void OnValidate()
        {
            if (Application.isPlaying) return;
            if (gameObject.IsAPrefab()) return;
            ValidateEquipment();
        }
        #endregion

        #region Public Methods
        public bool TryEquipItem(ItemEquipmentAsset newItem)
        {
            if (newItem == null) return false;
            
            RequestEquipItemRpc(new(newItem));
            return true;
            /*foreach (SlotEquipmentPair slotPair in equipmentSlots)
            {
                if (newItem.ValidSlots.Contains(slotPair.slot) == false) continue;

                return EquipItemToSlot(newItem, slotPair.slot);
            }

            return false;*/
        }

        //[Rpc(SendTo.Server)]
        [Rpc(SendTo.ClientsAndHost)]
        private void RequestEquipItemRpc(EquipmentReference itemRef, RpcParams rpcParams = default)
        {
            Debug.Log($"Requested Equip Item. Sender: {rpcParams.Receive.SenderClientId}");

            if (itemRef == null || itemRef.equipment == null)
            {
                Debug.Log($"Received item is null. Sender: {rpcParams.Receive.SenderClientId}");
                return;
            }
            foreach (SlotEquipmentPair slotPair in equipmentSlots)
            {
                if (itemRef.equipment.ValidSlots.Contains(slotPair.slot) == false) continue;

                EquipItemToSlot(itemRef, slotPair.slot);
                Debug.Log($"Item Equipped. Sender: {rpcParams.Receive.SenderClientId}");
            }
        }
        
        private bool EquipItemToSlot(ItemEquipmentAsset newItem, EquipmentSlot slot)
        {
            if (newItem == null || slot == null) return false;
            foreach (SlotEquipmentPair slotPair in equipmentSlots)
            {
                if (slotPair.slot != slot) continue;
                if (newItem.ValidSlots.Contains(slot) == false) return false;
                
                if (slotPair.itemRef != null) 
                    OnItemUnequipped?.Invoke(this, slotPair.slot, slotPair.itemRef);
                slotPair.itemRef = new(newItem);
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
                if (slotPair.itemRef != null)
                {
                    slotPair.itemRef = null;
                    OnItemUnequipped?.Invoke(this, slotPair.slot, slotPair.itemRef);
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
                if (slotPair.itemRef.equipment != item) continue;
                slotPair.itemRef = null;
                OnItemUnequipped?.Invoke(this, slotPair.slot, slotPair.itemRef);
                return true;
            }
            return false;
        }

        public bool IsItemEquipped(ItemAsset item)
        {
            if (item == null) return false;
            foreach (SlotEquipmentPair slotPair in equipmentSlots)
            {
                if (slotPair.itemRef.equipment == item)
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
                if (pair == null) continue;
                if (pair.itemRef.equipment.ValidSlots.Contains(pair.slot) == false) 
                    pair.itemRef = null;
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
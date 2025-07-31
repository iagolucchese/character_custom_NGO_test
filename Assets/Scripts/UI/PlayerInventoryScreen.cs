using System;
using System.Collections.Generic;
using ImportedScripts;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Assertions;

namespace CharacterCustomNGO.UI
{
    public class PlayerInventoryScreen : ScreenManagerBase
    {
        public enum ItemClickCallback{Nothing, Equip, Sell}
        
        [SerializeField] private ItemClickCallback itemClickCallback;
        [SerializeField] private List<InventoryItemUI> allInventorySlots;
        private InventoryHolder inventory;
        private EquipmentHolder equipmentHolder;
        
        #region Unity Messages
        protected override void Awake()
        {
            base.Awake();
            /*Assert.IsNotNull(inventory);
            Assert.IsNotNull(equipmentHolder);*/
            Assert.IsTrue(allInventorySlots.IsValidAndNotEmpty());
            
            /*inventory.OnItemAdded += InventoryItemEventCallback;
            inventory.OnItemRemoved += InventoryItemEventCallback;
            InventoryItemUI.OnItemClicked += ItemClickedCallback;*/
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            /*InventoryItemUI.OnItemClicked -= ItemClickedCallback;
            if (inventory != null)
            {
                inventory.OnItemAdded += InventoryItemEventCallback;
                inventory.OnItemRemoved += InventoryItemEventCallback;
            }*/
        }
        #endregion
        
        #region Public Methods
        public void UpdateInventoryIcons()
        {
            List<ItemAsset> allItems = inventory.ItemsOnInventory;
            for (int index = 0; index < allInventorySlots.Count; index++)
            {
                InventoryItemUI itemUI = allInventorySlots[index];
                if (index >= allItems.SafeCount())
                {
                    itemUI.gameObject.SetActive(false);
                    continue;
                }
                
                ItemAsset item = allItems[index];
                if (item == null) continue;
                
                itemUI.SetIcon(item.ItemIcon);
                itemUI.SetEquipped( equipmentHolder.IsItemEquipped(item) );
                itemUI.SetSellValueFrame(itemClickCallback == ItemClickCallback.Sell, item);
                itemUI.gameObject.SetActive(true);
            }
        }
        #endregion
        
        #region Private Methods
        private void FindLocalPlayerInventory()
        {
            var localPlayer = NetworkManager.Singleton.LocalClient.PlayerObject;
            inventory = localPlayer.GetComponent<InventoryHolder>();
            equipmentHolder = localPlayer.GetComponent<EquipmentHolder>();
        }
        
        protected override void ToggleScreen(bool show)
        {
            if (show)
            {
                FindLocalPlayerInventory();
                
                inventory.OnItemAdded += InventoryItemEventCallback;
                inventory.OnItemRemoved += InventoryItemEventCallback;
                InventoryItemUI.OnItemClicked += ItemClickedCallback;
                
                UpdateInventoryIcons();
            }
            else
            {
                InventoryItemUI.OnItemClicked -= ItemClickedCallback;
                if (inventory != null)
                {
                    inventory.OnItemAdded -= InventoryItemEventCallback;
                    inventory.OnItemRemoved -= InventoryItemEventCallback;
                }
            }
            
            base.ToggleScreen(show);
        }

        private void ItemClickedCallback(InventoryItemUI itemUI)
        {
            int clickedItemIndex = allInventorySlots.IndexOf(itemUI);
            if (clickedItemIndex < 0) return;
            
            if (clickedItemIndex >= inventory.AmountOfItemsInInventory)
            {
                throw new IndexOutOfRangeException("Tried to click an item, but index came out of bounds.");
            }

            ItemAsset clickedItem = inventory.ItemsOnInventory[clickedItemIndex];
            if (clickedItem == null) return;
            
            switch (itemClickCallback)
            {
                case ItemClickCallback.Equip:
                    ItemEquipmentAsset clickedEquipment = clickedItem as ItemEquipmentAsset;
                    if (clickedEquipment != null)
                        equipmentHolder.ToggleItemEquipped(clickedEquipment);
                    break;
                case ItemClickCallback.Sell:
                    inventory.TrySellItem(clickedItem);
                    break;
                case ItemClickCallback.Nothing:
                default:
                    break;
            }
            
            UpdateInventoryIcons();
        }

        private void InventoryItemEventCallback(InventoryHolder holder, ItemAsset item)
        {
            UpdateInventoryIcons();
        }
        #endregion
    }
}
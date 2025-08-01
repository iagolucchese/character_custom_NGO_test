using System;
using System.Collections.Generic;
using ImportedScripts;
using NaughtyAttributes;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Assertions;

namespace CharacterCustomNGO.UI
{
    public class ShopScreen : ScreenManagerBase
    {
        [SerializeField] private SetOfItems shopInventory;
        [SerializeField] private List<ShopItemUI> allShopItems;
        [SerializeField, ReadOnly] private InventoryHolder playerInventory;

        #region Unity Messages
        protected override void Awake()
        {
            base.Awake();
            Assert.IsNotNull(shopInventory);
            Assert.IsTrue(allShopItems.IsValidAndNotEmpty());

            ShopItemUI.OnItemClicked += ItemClickedCallback;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            ShopItemUI.OnItemClicked -= ItemClickedCallback;
        }
        #endregion

        #region Private Methods
        private void ItemClickedCallback(ShopItemUI shopItemUI)
        {
            int clickedItemIndex = allShopItems.IndexOf(shopItemUI);
            if (clickedItemIndex < 0) return;
            
            if (clickedItemIndex >= shopInventory.AmountOfItems)
            {
                throw new IndexOutOfRangeException("Tried to click an item, but index came out of bounds.");
            }

            ItemAsset clickedItem = shopInventory.ItemSet[clickedItemIndex];
            playerInventory.TryBuyItemRpc(clickedItem);
            UpdateShopInventoryOptions();
        }

        private void FindLocalPlayerInventory()
        {
            var localPlayer = NetworkManager.Singleton.LocalClient.PlayerObject;
            playerInventory = localPlayer.GetComponent<InventoryHolder>();
        }
        
        protected override void ToggleScreen(bool show)
        {
            if (show)
            {
                FindLocalPlayerInventory();
                UpdateShopInventoryOptions();
            }
            base.ToggleScreen(show);
        }

        private void UpdateShopInventoryOptions()
        {
            for (int index = 0; index < allShopItems.Count; index++)
            {
                ShopItemUI shopItem = allShopItems[index];
                if (index >= shopInventory.AmountOfItems)
                {
                    shopItem.UpdateUIForItem(null);
                    shopItem.SetButtonInteractable(false);
                    continue;
                }
                
                ItemAsset item = shopInventory.ItemSet[index];
                shopItem.UpdateUIForItem(item);
                shopItem.SetButtonInteractable( playerInventory.HasMoneyToBuyItem(item) );
            }
        }
        #endregion
    }
}
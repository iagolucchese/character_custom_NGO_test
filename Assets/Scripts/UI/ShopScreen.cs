using System;
using System.Collections.Generic;
using ImportedScripts;
using UnityEngine;
using UnityEngine.Assertions;

namespace CharacterCustomNGO.UI
{
    public class ShopScreen : ScreenManagerBase
    {
        [SerializeField] private SetOfItems shopInventory;
        [SerializeField] private InventoryHolder playerInventory;
        [SerializeField] private List<ShopItemUI> allShopItems;
        
        #region Unity Messages
        protected override void Awake()
        {
            base.Awake();
            Assert.IsNotNull(shopInventory);
            Assert.IsNotNull(playerInventory);
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
            playerInventory.TryBuyItem(clickedItem);
            UpdateShopInventoryOptions();
        }

        protected override void ToggleScreen(bool show)
        {
            if (show)
                UpdateShopInventoryOptions();
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
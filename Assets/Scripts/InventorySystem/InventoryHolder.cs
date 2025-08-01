using System.Collections.Generic;
using ImportedScripts;
using NaughtyAttributes;
using Unity.Netcode;
using UnityEngine;

namespace CharacterCustomNGO
{
    public class InventoryHolder : MonoBehaviour
    {
        public delegate void InventoryItemEvent(InventoryHolder holder, ItemAsset item);
        public delegate void MoneyEvent(InventoryHolder holder);
        public event InventoryItemEvent OnItemAdded;
        public event InventoryItemEvent OnItemRemoved;
        public event MoneyEvent OnMoneyValueChanged;
        
        [SerializeField] private List<ItemAsset> itemsOnInventory;
        [SerializeField, Min(0)] private int startingCash = 100;
        [SerializeField, ReadOnly] private int moneyOnHand;
        
        public List<ItemAsset> ItemsOnInventory => itemsOnInventory;
        public int AmountOfItemsInInventory => itemsOnInventory.SafeCount();
        public int MoneyOnHand
        {
            get => moneyOnHand;
            set
            {
                moneyOnHand = value < 0 ? 0 : value;
                OnMoneyValueChanged?.Invoke(this);
            }
        }

        #region Unity Messages
        private void OnEnable()
        {
            itemsOnInventory ??= new List<ItemAsset>();
        }

        private void Start()
        {
            MoneyOnHand = startingCash;
        }
        #endregion

        #region Public Methods
        [Rpc(SendTo.ClientsAndHost)]
        public bool TrySellItemRpc(ItemAsset item, RpcParams rpcParams = default)
        {
            if (item == null) return false;
            
            Debug.Log($"Requested Sell Item. Sender: {rpcParams.Receive.SenderClientId}");
            if (RemoveItemFromInventory(item))
            {
                MoneyOnHand += item.ItemSellValue;
                return true;
            }
            return false;
        }

        [Rpc(SendTo.ClientsAndHost)]
        public bool TryBuyItemRpc(ItemAsset item, RpcParams rpcParams = default)
        {
            if (item == null) return false;
            if (HasMoneyToBuyItem(item) == false) return false;
            
            Debug.Log($"Requested Buy Item. Sender: {rpcParams.Receive.SenderClientId}");
            if (AddItemToInventory(item))
            {
                MoneyOnHand -= item.ItemCost;
                return true;
            }
            return false;
        }
        
        public bool HasMoneyToBuyItem(ItemAsset item)
        {
            return item.ItemCost <= MoneyOnHand;
        }
        
        public bool AddItemToInventory(ItemAsset newItem)
        {
            itemsOnInventory.Add(newItem);
            OnItemAdded?.Invoke(this, newItem);
            return true;
        }

        public bool RemoveItemFromInventory(ItemAsset removeItem)
        {
            if (itemsOnInventory.IsInvalidOrEmpty()) return false;
            if (itemsOnInventory.Remove(removeItem))
            {
                OnItemRemoved?.Invoke(this, removeItem);
                return true;
            }
            return false;
        }
        #endregion
    }
}
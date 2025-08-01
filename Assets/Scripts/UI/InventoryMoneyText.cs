using NaughtyAttributes;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Assertions;

namespace CharacterCustomNGO.UI
{
    public class InventoryMoneyText : ScreenManagerBase
    {
        [SerializeField] private TMP_Text moneyText;
        [SerializeField] private string moneyPrefix;
        [SerializeField, ReadOnly] private InventoryHolder inventoryHolder;

        #region Unity Messages
        protected override void Awake()
        {
            base.Awake();
            //Assert.IsNotNull(inventoryHolder);
            Assert.IsNotNull(moneyText);

            PlayerNetwork.OnPlayerSpawned += HandlePlayerSpawn;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            PlayerNetwork.OnPlayerSpawned -= HandlePlayerSpawn;
            if (inventoryHolder != null) 
                inventoryHolder.OnMoneyValueChanged -= MoneyValueChangedCallback;
        }
        #endregion

        #region Private Methods
        private void HandlePlayerSpawn(NetworkObject playerNO)
        {
            if (inventoryHolder)
                inventoryHolder.OnMoneyValueChanged -= MoneyValueChangedCallback;

            if (playerNO.TryGetComponent(out inventoryHolder)) 
                inventoryHolder.OnMoneyValueChanged += MoneyValueChangedCallback;
        }
        
        private void MoneyValueChangedCallback(InventoryHolder holder)
        {
            moneyText.text = $"{moneyPrefix}{holder.MoneyOnHand}";
        }
        #endregion
    }
}
using System;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;

namespace CharacterCustomNGO.UI
{
    public class InventoryMoneyText : MonoBehaviour
    {
        [SerializeField] private InventoryHolder inventoryHolder;
        [SerializeField] private TMP_Text moneyText;
        [SerializeField] private string moneyPrefix;
        
        #region Unity Messages
        private void Awake()
        {
            Assert.IsNotNull(inventoryHolder);
            Assert.IsNotNull(moneyText);

            inventoryHolder.OnMoneyValueChanged += MoneyValueChangedCallback;
        }
        private void OnDestroy()
        {
            if (inventoryHolder != null)
            {
                inventoryHolder.OnMoneyValueChanged -= MoneyValueChangedCallback;
            }
        }
        #endregion

        #region Private Methods
        private void MoneyValueChangedCallback(InventoryHolder holder)
        {
            moneyText.text = $"{moneyPrefix}{holder.MoneyOnHand}";
        }
        #endregion
    }
}
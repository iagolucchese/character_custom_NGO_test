using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CharacterCustomNGO.UI
{
    public class ShopItemUI : MonoBehaviour
    {
        public delegate void ShopItemUIEvent(ShopItemUI shopItemUI);
        public static event ShopItemUIEvent OnItemClicked;
        
        [SerializeField] private Button itemButton;
        [SerializeField] private Image itemIcon;
        [SerializeField] private TMP_Text itemNameText;
        [SerializeField] private TMP_Text itemCostText;
        [SerializeField] private string itemCostPrefix;
        
        #region Unity Messages
        private void OnEnable()
        {
            if (itemButton != null)
                itemButton.onClick.AddListener(RaiseOnItemClicked);
        }

        private void OnDisable()
        {
            if (itemButton != null)
                itemButton.onClick.RemoveListener(RaiseOnItemClicked);
        }
        #endregion

        public void UpdateUIForItem(ItemAsset item)
        {
            if (item == null)
            {
                gameObject.SetActive(false);
                return;
            }
            
            if (itemIcon != null)
                itemIcon.sprite = item == null ? null : item.ItemIcon;
            if (itemNameText != null)
                itemNameText.text = item == null ? string.Empty : item.ItemName;
            if (itemCostText != null)
                itemCostText.text = item == null ? string.Empty : $"{itemCostPrefix}{item.ItemCost}";
            gameObject.SetActive(true);
        }

        public void SetButtonInteractable(bool interactable)
        {
            if (itemButton != null)
                itemButton.interactable = interactable;
        }
        
        private void RaiseOnItemClicked()
        {
            OnItemClicked?.Invoke(this);
        }
    }
}
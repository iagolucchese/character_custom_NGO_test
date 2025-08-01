using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CharacterCustomNGO.UI
{
    public class InventoryItemUI : MonoBehaviour
    {
        public delegate void ItemUIEvent(InventoryItemUI itemUI);
        public static event ItemUIEvent OnItemClicked;
        
        [SerializeField] private Image itemIconImage;
        [SerializeField] private Image equippedOverlay;
        [SerializeField] private Button itemButton;
        [SerializeField] private GameObject sellValueFrame;
        //[SerializeField] private TMP_Text itemSellValueText;
        [SerializeField] private TMP_Text itemNameText;

        private void OnEnable()
        {
            itemButton.onClick.AddListener(RaiseOnItemClicked);
        }

        private void OnDisable()
        {
            itemButton.onClick.RemoveListener(RaiseOnItemClicked);
        }

        private void RaiseOnItemClicked()
        {
            OnItemClicked?.Invoke(this);
        }

        public void SetSellValueFrame(bool showFrame, ItemAsset item)
        {
            if (item != null)
            {
                //itemSellValueText.text = item.ItemSellValue.ToString();
                itemNameText.text = item.ItemName;
            }
            if (sellValueFrame)
                sellValueFrame.SetActive(showFrame);
        }

        public void SetIcon(Sprite newIcon)
        {
            itemIconImage.sprite = newIcon;
        }

        public void SetEquipped(bool isEquipped)
        {
            equippedOverlay.enabled = isEquipped;
        }
    }
}
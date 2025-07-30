namespace CharacterCustomNGO
{
    [System.Serializable]
    public class SlotEquipmentPair
    {
        public EquipmentSlot slot;
        public ItemEquipmentAsset item;

        public SlotEquipmentPair(EquipmentSlot slot, ItemEquipmentAsset item)
        {
            this.slot = slot;
            this.item = item;
        }
    }
}
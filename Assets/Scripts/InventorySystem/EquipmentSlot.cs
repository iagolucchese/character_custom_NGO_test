using UnityEngine;

namespace CharacterCustomNGO
{
    [CreateAssetMenu(fileName = "New " + nameof(EquipmentSlot), menuName = "Items/" + nameof(EquipmentSlot))]
    public class EquipmentSlot : ScriptableObject
    {
        [Header("Visual & UI")]
        [SerializeField] private string slotName;

        public string SlotName => slotName;
    }
}
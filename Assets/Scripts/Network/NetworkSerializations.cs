using Unity.Netcode;
using UnityEngine;

namespace CharacterCustomNGO
{
    public static class NetworkSerializations
    {
        public static void WriteValueSafe(this FastBufferWriter writer, in ItemAsset itemAsset)
        {
            writer.WriteValueSafe(itemAsset.name);
        }

        public static void ReadValueSafe(this FastBufferReader reader, out ItemAsset itemAsset)
        {
            reader.ReadValueSafe(out string assetName);
            itemAsset = Resources.Load<ItemAsset>("Equipments/" + assetName);
        }
        
        /*public static void SerializeValue<T>(this BufferSerializer<T> reader, ref ItemEquipmentAsset equipmentRef) where T: IReaderWriter
        {
            if (reader.IsReader)
            {
                reader.GetFastBufferReader().ReadValueSafe(out string assetName);
                equipmentRef = Resources.Load<ItemEquipmentAsset>("Equipments/" + assetName); //new AssetReference(assetName);
            }
            else 
            {
                reader.GetFastBufferWriter().WriteValueSafe(equipmentRef.name);
            }
        }*/
    }
}
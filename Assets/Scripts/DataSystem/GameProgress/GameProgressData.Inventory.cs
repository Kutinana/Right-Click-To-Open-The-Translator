using System.Collections;
using System.Collections.Generic;
using QFramework;
using System;

namespace DataSystem
{
    public partial class GameProgressData
    {
        public static void IncreaseInventory(string _id)
        {
            var inventory = Instance.Save.Inventory;
            var data = GameDesignData.GetObtainableObjectDataById(_id);

            if (!inventory.TryGetValue(_id, out var value))
            {
                value = 0;
            }
            if (data.MaxAmount == 0) inventory[_id]++;
            else inventory[_id] = Math.Clamp(value + 1, 0, data.MaxAmount);

            Instance.Serialization();
            TypeEventSystem.Global.Send(new OnInventoryIncreasedEvent(new Dictionary<string, int>() {{_id, 1}}));
        }

        public static void IncreaseInventory(string _id, int _delta)
        {
            var inventory = Instance.Save.Inventory;
            var data = GameDesignData.GetObtainableObjectDataById(_id);

            if (!inventory.TryGetValue(_id, out var value))
            {
                value = 0;
            }
            if (data.MaxAmount == 0) inventory[_id] = _delta + value;
            else inventory[_id] = Math.Clamp(value + _delta, 0, data.MaxAmount);

            Instance.Serialization();
            TypeEventSystem.Global.Send(new OnInventoryIncreasedEvent(new Dictionary<string, int>() {{_id, _delta}}));
        }

        public static void IncreaseInventory(Dictionary<string, int> _items)
        {
            var inventory = Instance.Save.Inventory;
            foreach (var item in _items)
            {
                var data = GameDesignData.GetObtainableObjectDataById(item.Key);

                if (!inventory.TryGetValue(item.Key, out var value))
                {
                    value = 0;
                }
                if (data.MaxAmount == 0) inventory[item.Key] = item.Value + value;
                else inventory[item.Key] = Math.Clamp(value + item.Value, 0, data.MaxAmount);
            }

            Instance.Serialization();
            TypeEventSystem.Global.Send(new OnInventoryIncreasedEvent(_items));
        }

        public static bool TryDecreaseInventory(string _id, int _delta)
        {
            var inventory = Instance.Save.Inventory;
            if (inventory.TryGetValue(_id, out var value))
            {
                if (value >= _delta)
                {
                    inventory[_id] -= _delta;

                    Instance.Serialization();
                    return true;
                }
            }
            return false;
        }

        public static bool TryDecreaseInventory(Dictionary<string, int> _items)
        {
            var inventory = Instance.Save.Inventory;

            foreach (var item in _items)
            {
                if (!inventory.TryGetValue(item.Key, out var value) || value < item.Value)
                {
                    return false;
                }
            }
            
            foreach (var item in _items)
            {
                inventory[item.Key] -= item.Value;
            }
            Instance.Serialization();
            return true;
        }

        public static Dictionary<string, int> GetInventory()
        {
            return Instance.Save.Inventory;
        }
    }
}
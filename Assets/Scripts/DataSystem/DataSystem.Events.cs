using System.Collections.Generic;

namespace DataSystem
{
    public struct OnInventoryIncreasedEvent
    {
        public Dictionary<string, int> items;
        public OnInventoryIncreasedEvent(Dictionary<string, int> _items)
        {
            items = _items;
        }
    }
}
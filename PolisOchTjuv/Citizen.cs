using TjuvochPolis;
using System;

namespace PoliceAndThieves
{
    public class Citizen : Person
    {
        // When the citizen moved into the poorhouse.
        public DateTime PoorHouseSince { get; set; }

        public Citizen(int x, int y, int dirX, int dirY)
            : base(x, y, dirX, dirY)
        {
            GiveStartingItems();
        }

        // Gives the citizen the four starting items.
        public void GiveStartingItems()
        {
            Inventory.Add(new Item("Keys"));
            Inventory.Add(new Item("Mobile Phone"));
            Inventory.Add(new Item("Money"));
            Inventory.Add(new Item("Watch"));
        }

        public override char GetSymbol() => 'C';
    }
}
using TjuvochPolis;

namespace PoliceAndThieves
{
    public class Citizen : Person
    {
        public Citizen(int x, int y, int dirX, int dirY)
            : base(x, y, dirX, dirY)
        {
            Inventory.Add(new Item("Keys"));
            Inventory.Add(new Item("Mobile Phone"));
            Inventory.Add(new Item("Money"));
            Inventory.Add(new Item("Watch"));
        }

        public override char GetSymbol() => 'C';
    }
}
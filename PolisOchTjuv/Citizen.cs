namespace TjuvochPolis
{
    public class Citizen : Person
    {
        public Citizen(int x, int y, int dirX, int dirY) : base(x, y, dirX, dirY)
        {
            Inventory.Add(new Item("Nycklar"));
            Inventory.Add(new Item("Mobiltelefon"));
            Inventory.Add(new Item("Pengar"));
            Inventory.Add(new Item("Klocka"));
        }

        public override char GetSymbol() => 'M';
    }
}
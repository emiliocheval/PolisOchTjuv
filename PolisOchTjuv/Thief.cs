namespace TjuvochPolis
{
    public class Thief : Person
    {
        public Thief(int x, int y, int dirX, int dirY) : base(x, y, dirX, dirY)
        {
        }

        public override char GetSymbol() => 'T';
    }
}
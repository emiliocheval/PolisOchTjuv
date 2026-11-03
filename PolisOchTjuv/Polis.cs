
namespace PoliceAndThieves
{
    public class Police : Person
    {
        public Police(int x, int y, int dirX, int dirY)
            : base(x, y, dirX, dirY)
        {
        }

        // Returns the symbol representing a police officer.
        public override char GetSymbol() => 'P';
    }
}


using System;

namespace PoliceAndThieves
{
    public class Thief : Person
    {
        public DateTime ReleaseTime { get; set; }

        public Thief(int x, int y, int dirX, int dirY)
            : base(x, y, dirX, dirY)
        {
        }

        public override char GetSymbol() => 'T';
    }
}

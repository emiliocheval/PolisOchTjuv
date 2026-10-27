using System;
using System.Collections.Generic;
using System.Text;

namespace PolisOchTjuv
{
    internal class Polis
    {
    }
}
namespace TjuvochPolis
{
    public class Polis : Person
    {
        public Polis(int x, int y, int dirX, int dirY) : base(x, y, dirX, dirY)
        {
        }

        public override char GetSymbol() => 'P';
    }
}

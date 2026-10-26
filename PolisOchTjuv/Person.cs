using System.Collections.Generic;

namespace TjuvochPolis
{
    public abstract class Person
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int DirX { get; set; }
        public int DirY { get; set; }
        public List<Item> Inventory { get; set; }

        public Person(int startX, int startY, int dirX, int dirY)
        {
            X = startX;
            Y = startY;
            DirX = dirX;
            DirY = dirY;
            Inventory = new List<Item>();
        }

        // Metod för att röra sig och dyka upp på andra sidan om man går utanför[cite: 10]
        public void Move(int width, int height)
        {
            X += DirX;
            Y += DirY;

            if (X < 0) X = width - 1;
            else if (X >= width) X = 0;

            if (Y < 0) Y = height - 1;
            else if (Y >= height) Y = 0;
        }

        public abstract char GetSymbol();
    }
}
using System.Collections.Generic;

namespace PoliceAndThieves
{
    public abstract class Place
    {
        public int Width { get; }
        public int Height { get; }
        public List<Person> People { get; } = new List<Person>();

        protected Place(int width, int height)
        {
            Width = width;
            Height = height;
        }

        // Moves all people within the current location.
        public void MoveAll()
        {
            foreach (var person in People)
            {
                person.Move(Width, Height);
            }
        }
    }

    public class City : Place
    {
        public City(int width, int height) : base(width, height)
        {
        }
    }

    public class Prison : Place
    {
        public Prison(int width, int height) : base(width, height)
        {
        }
    }

    public class PoorHouse : Place
    {
        public PoorHouse(int width, int height) : base(width, height)
        {
        }
    }
}
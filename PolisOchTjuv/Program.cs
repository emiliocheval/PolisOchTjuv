using System;
using System.Collections.Generic;
using System.Threading;

namespace TjuvochPolis
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.CursorVisible = false;
            int width = 100;
            int height = 25;
            List<Person> people = new List<Person>();
            Random rnd = new Random();

            int robbedCitizens = 0;
            int arrestedThieves = 0;

            // Skapa 30 medborgare, 20 tjuvar, 10 poliser
            for (int i = 0; i < 30; i++) people.Add(new Citizen(rnd.Next(width), rnd.Next(height), GetDir(rnd), GetDir(rnd)));
            for (int i = 0; i < 20; i++) people.Add(new Thief(rnd.Next(width), rnd.Next(height), GetDir(rnd), GetDir(rnd)));
            for (int i = 0; i < 10; i++) people.Add(new Polis(rnd.Next(width), rnd.Next(height), GetDir(rnd), GetDir(rnd)));
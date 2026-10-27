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
            
            while (true)
            {
                // 1. Flytta alla
                foreach (var p in people)
                {
                    p.Move(width, height);
                }

                // 2. Rita upp staden med FÄRG
                Console.SetCursorPosition(0, 0);

                // Vi använder en array av Person istället för char för att veta vem som är vem
                Person[,] city = new Person[height, width];
                foreach (var p in people) city[p.Y, p.X] = p;

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        if (city[y, x] == null)
                        {
                            Console.Write(' ');
                        }
                        else
                        {
                            // Sätt färg baserat på kategori
                            if (city[y, x] is Citizen) Console.ForegroundColor = ConsoleColor.Green;
                            else if (city[y, x] is Thief) Console.ForegroundColor = ConsoleColor.Red;
                            else if (city[y, x] is Polis) Console.ForegroundColor = ConsoleColor.Blue;

                            Console.Write(city[y, x].GetSymbol());
                            Console.ResetColor(); // Återställ färgen till vit
                        }
                    }
                    Console.WriteLine();
                }

                Console.WriteLine($"\nAntal rånade medborgare: {robbedCitizens}    ");
                Console.WriteLine($"Antal gripna tjuvar: {arrestedThieves}      ");

                // 3. Kolla kollisioner
                for (int i = 0; i < people.Count; i++)
                {
                    for (int j = i + 1; j < people.Count; j++)
                    {
                        if (people[i].X == people[j].X && people[i].Y == people[j].Y)
                        {
                            Person p1 = people[i];
                            Person p2 = people[j];

                            // Tjuv möter Medborgare
                            if ((p1 is Thief && p2 is Citizen) || (p1 is Citizen && p2 is Thief))
                            {
                                Citizen c = (p1 is Citizen) ? (Citizen)p1 : (Citizen)p2;
                                Thief t = (p1 is Thief) ? (Thief)p1 : (Thief)p2;
                                if (c.Inventory.Count > 0)
                                {
                                    int itemIndex = rnd.Next(c.Inventory.Count);
                                    Item stolenItem = c.Inventory[itemIndex];
                                    c.Inventory.RemoveAt(itemIndex);
                                    t.Inventory.Add(stolenItem);
                                    robbedCitizens++;

                                    Console.ForegroundColor = ConsoleColor.Red;
                                    Console.WriteLine($"\nTJUV RÅNAR MEDBORGARE på {stolenItem.Name}!       ");
                                    Console.ResetColor();
                                    Thread.Sleep(2000); // Pausa i 2 sekunder
                                }
                            }

                            // Polis möter Tjuv
                            else if ((p1 is Polis && p2 is Thief) || (p1 is Thief && p2 is Polis))
                            {
                                Polis p = (p1 is Polis) ? (Polis)p1 : (Polis)p2;
                                Thief t = (p1 is Thief) ? (Thief)p1 : (Thief)p2;

                                if (t.Inventory.Count > 0)
                                {
                                    p.Inventory.AddRange(t.Inventory);
                                    t.Inventory.Clear();
                                    arrestedThieves++;

                                    Console.ForegroundColor = ConsoleColor.Blue;
                                    Console.WriteLine("\nPOLIS GRIPER TJUV och beslagtar bytet!           ");
                                    Console.ResetColor();
                                    Thread.Sleep(2000); // Pausa i 2 sekunder
                                }
                            }
                        }
                    }
                }
                Thread.Sleep(200);
            }
        }

        static int GetDir(Random rnd)
        {
            return rnd.Next(-1, 2);
        }
    }
}
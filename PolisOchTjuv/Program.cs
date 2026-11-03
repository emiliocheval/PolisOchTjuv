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
            int height = 15;
            City city = new City(width, height);
            Prison prison = new Prison(10, 10);
            Random rnd = new Random();
            List<string> newsFeed = new List<string>();
            
            static void AddNews(List<string> newsFeed, string text)
            {
                newsFeed.Insert(0, text);
                if (newsFeed.Count > 5) newsFeed.RemoveAt(newsFeed.Count - 1);
            }

            int robbedCitizens = 0;
            int arrestedThieves = 0;

            if (Console.WindowWidth < width + 2 || Console.WindowHeight < 25)
            {
                Console.WriteLine($"Console window too small. Needs at least {width + 2} x 25.");
                return;
            }


            // Skapa 30 medborgare, 20 tjuvar, 10 poliser
            for (int i = 0; i < 30; i++) city.People.Add(new Citizen(rnd.Next(width), rnd.Next(height), GetDir(rnd), GetDir(rnd)));
            for (int i = 0; i < 20; i++) city.People.Add(new Thief(rnd.Next(width), rnd.Next(height), GetDir(rnd), GetDir(rnd)));
            for (int i = 0; i < 10; i++) city.People.Add(new Polis(rnd.Next(width), rnd.Next(height), GetDir(rnd), GetDir(rnd)));

            try
            {
                while (true)
                {
                    while (Console.KeyAvailable)
                    {
                        ConsoleKey key = Console.ReadKey(true).Key;
                        if (key == ConsoleKey.C)
                        {
                            city.People.Add(new Citizen(rnd.Next(width), rnd.Next(height), GetDir(rnd), GetDir(rnd)));
                            AddNews(NewsFeed, "A new citizen moves into the city");
                        }
                        else
                        }

                        if (key == ConsoleKey.T)
                        {
                            city.People.Add(new Thief(rnd.Next(width), rnd.Next(height), GetDir(rnd), GetDir(rnd)));
                            AddNews(newsFeed, ("A new thief moves into the city");
                        }
                        else if (key == ConsoleKey.P)
                        {
                            city.People.Add(new Polis(rnd.Next(width), rnd.Next(height), GetDir(rnd), GetDir(rnd)));
                            AddNews(newsFeed, "A new police officer moves into the city");
                        }
                    }

                    // Frisläpp tjuvar vars tid är slut
                    for (int i = prison.People.Count - 1; i >= 0; i--)
                    {
                        Thief released = (Thief)prison.People[i];
                        if (DateTime.Now >= released.ReleaseTime)
                        {
                            prison.People.RemoveAt(i);
                            released.X = rnd.Next(width);
                            released.Y = rnd.Next(height);
                            city.People.Add(released);
                        }
                    }

                    // 1. Flytta alla
                    city.MoveAll();
                    prison.MoveAll();

                    // 2. Rita upp staden med FÄRG
                    Console.SetCursorPosition(0, 0);
                    DrawPlace(city, "CITY");

                    Console.WriteLine($"\nAntal rånade medborgare: {robbedCitizens}    ");
                    Console.WriteLine($"Antal gripna tjuvar: {arrestedThieves}      ");

                    List<Thief> toPrison = new List<Thief>();

                    // 3. Kolla kollisioner
                    for (int i = 0; i < city.People.Count; i++)
                    {
                        for (int j = i + 1; j < city.People.Count; j++)
                        {
                            if (city.People[i].X == city.People[j].X && city.People[i].Y == city.People[j].Y)
                            {
                                Person p1 = city.People[i];
                                Person p2 = city.People[j];

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
                                        int count = t.Inventory.Count;
                                        p.Inventory.AddRange(t.Inventory);
                                        t.Inventory.Clear();
                                        t.ReleaseTime = DateTime.Now.AddSeconds(count * 10);   // 4 saker = 40 s
                                        arrestedThieves++;
                                        toPrison.Add(t);

                                        Console.ForegroundColor = ConsoleColor.Blue;
                                        Console.WriteLine("\nPOLIS GRIPER TJUV och beslagtar bytet!           ");
                                        Console.ResetColor();
                                        Thread.Sleep(2000);
                                    }
                                }
                            }
                        }
                    }
                    foreach (Thief thief in toPrison)
                    {
                        city.People.Remove(thief);
                        thief.X = rnd.Next(prison.Width);
                        thief.Y = rnd.Next(prison.Height);
                        prison.People.Add(thief);
                    }
                    Thread.Sleep(200);
                }
            }
            catch (Exception ex)
            {
                Console.ResetColor();
                Console.CursorVisible = true;
                Console.Clear();
                Console.WriteLine($"An error occurred: {ex.Message}");
            }

           
        }

        static int GetDir(Random rnd)
        {
            return rnd.Next(-1, 2);
        }

        static void DrawPlace(Place place, string title)
        {
            Console.WriteLine(("= " + title + " ").PadRight(place.Width + 2, '='));

            Person[,] grid = new Person[place.Height, place.Width];
            foreach (var p in place.People) grid[p.Y, p.X] = p;

            for (int y = 0; y < place.Height; y++)
            {
                Console.Write('|');
                for (int x = 0; x < place.Width; x++)
                {
                    Person p = grid[y, x];
                    if (p == null)
                    {
                        Console.Write(' ');
                        continue;
                    }
                    if (p is Citizen) Console.ForegroundColor = ConsoleColor.Green;
                    else if (p is Thief) Console.ForegroundColor = ConsoleColor.Red;
                    else if (p is Polis) Console.ForegroundColor = ConsoleColor.Blue;
                    Console.Write(p.GetSymbol());
                    Console.ResetColor();
                }
                Console.WriteLine('|');
            }
            Console.WriteLine("+" + new string('-', place.Width) + "+");
        }

    }
}
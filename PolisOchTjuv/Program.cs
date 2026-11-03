
using System;
using System.Collections.Generic;
using System.Threading;
using TjuvochPolis;

namespace PoliceAndThieves
{
    internal class Program
    {
        static void Main(string[] args)
        {
            const int width = 100;
            const int height = 15;

            City city = new City(width, height);
            Prison prison = new Prison(10, 10);

            Random random = new Random();
            List<string> newsFeed = new List<string>();

            int robbedCitizens = 0;
            int arrestedThieves = 0;

            if (Console.WindowWidth < width + 2 ||
                Console.WindowHeight < 25)
            {
                Console.WriteLine(
                    $"Console window too small. Minimum size: {width + 2} x 25.");
                return;
            }

            Console.CursorVisible = false;

            // Create 30 citizens.
            for (int i = 0; i < 30; i++)
            {
                city.People.Add(new Citizen(
                    random.Next(width),
                    random.Next(height),
                    GetDirection(random),
                    GetDirection(random)));
            }

            // Create 20 thieves.
            for (int i = 0; i < 20; i++)
            {
                city.People.Add(new Thief(
                    random.Next(width),
                    random.Next(height),
                    GetDirection(random),
                    GetDirection(random)));
            }

            // Create 10 police officers.
            for (int i = 0; i < 10; i++)
            {
                city.People.Add(new Police(
                    random.Next(width),
                    random.Next(height),
                    GetDirection(random),
                    GetDirection(random)));
            }

            AddNews(newsFeed, "City simulation started.");

            try
            {
                while (true)
                {
                    // Handle keyboard input.
                    while (Console.KeyAvailable)
                    {
                        ConsoleKey key = Console.ReadKey(true).Key;

                        if (key == ConsoleKey.Escape)
                        {
                            return;
                        }

                        if (key == ConsoleKey.C)
                        {
                            city.People.Add(new Citizen(
                                random.Next(width),
                                random.Next(height),
                                GetDirection(random),
                                GetDirection(random)));

                            AddNews(newsFeed,
                                "A new citizen has moved into the city.");
                        }
                        else if (key == ConsoleKey.T)
                        {
                            city.People.Add(new Thief(
                                random.Next(width),
                                random.Next(height),
                                GetDirection(random),
                                GetDirection(random)));

                            AddNews(newsFeed,
                                "A new thief has entered the city.");
                        }
                        else if (key == ConsoleKey.P)
                        {
                            city.People.Add(new Police(
                                random.Next(width),
                                random.Next(height),
                                GetDirection(random),
                                GetDirection(random)));

                            AddNews(newsFeed,
                                "A new police officer has joined the city.");
                        }
                    }

                    // Release thieves whose prison sentences have ended.
                    for (int i = prison.People.Count - 1; i >= 0; i--)
                    {
                        Thief thief = (Thief)prison.People[i];

                        if (DateTime.Now >= thief.ReleaseTime)
                        {
                            prison.People.RemoveAt(i);

                            thief.X = random.Next(width);
                            thief.Y = random.Next(height);

                            city.People.Add(thief);

                            AddNews(newsFeed,
                                "A thief has been released from prison.");
                        }
                    }

                    // Move all people.
                    city.MoveAll();
                    prison.MoveAll();

                    List<Thief> thievesToPrison = new List<Thief>();

                    // Check collisions between people.
                    for (int i = 0; i < city.People.Count; i++)
                    {
                        for (int j = i + 1; j < city.People.Count; j++)
                        {
                            Person firstPerson = city.People[i];
                            Person secondPerson = city.People[j];

                            if (firstPerson.X != secondPerson.X ||
                                firstPerson.Y != secondPerson.Y)
                            {
                                continue;
                            }

                            // A thief encounters a citizen.
                            if ((firstPerson is Thief &&
                                 secondPerson is Citizen) ||
                                (firstPerson is Citizen &&
                                 secondPerson is Thief))
                            {
                                Citizen citizen = firstPerson is Citizen
                                    ? (Citizen)firstPerson
                                    : (Citizen)secondPerson;

                                Thief thief = firstPerson is Thief
                                    ? (Thief)firstPerson
                                    : (Thief)secondPerson;

                                if (citizen.Inventory.Count > 0)
                                {
                                    int itemIndex = random.Next(
                                        citizen.Inventory.Count);

                                    Item stolenItem =
                                        citizen.Inventory[itemIndex];

                                    citizen.Inventory.RemoveAt(itemIndex);
                                    thief.Inventory.Add(stolenItem);

                                    robbedCitizens++;

                                    AddNews(newsFeed,
                                        $"ROBBERY: A thief stole {stolenItem.Name}!");

                                    Thread.Sleep(2000);
                                }
                            }

                            // A police officer encounters a thief.
                            else if ((firstPerson is Police &&
                                      secondPerson is Thief) ||
                                     (firstPerson is Thief &&
                                      secondPerson is Police))
                            {
                                Police officer = firstPerson is Police
                                    ? (Police)firstPerson
                                    : (Police)secondPerson;

                                Thief thief = firstPerson is Thief
                                    ? (Thief)firstPerson
                                    : (Thief)secondPerson;

                                if (thief.Inventory.Count > 0 &&
                                    !thievesToPrison.Contains(thief))
                                {
                                    int stolenItemCount =
                                        thief.Inventory.Count;

                                    // Confiscate stolen items.
                                    officer.Inventory.AddRange(thief.Inventory);
                                    thief.Inventory.Clear();

                                    // Ten seconds in prison per stolen item.
                                    int sentenceSeconds =
                                        stolenItemCount * 10;

                                    thief.ReleaseTime =
                                        DateTime.Now.AddSeconds(sentenceSeconds);

                                    arrestedThieves++;
                                    thievesToPrison.Add(thief);

                                    AddNews(newsFeed,
                                        $"ARREST: Thief sentenced to {sentenceSeconds} seconds!");

                                    Thread.Sleep(2000);
                                }
                            }
                        }
                    }

                    // Move arrested thieves to prison.
                    foreach (Thief thief in thievesToPrison)
                    {
                        city.People.Remove(thief);

                        thief.X = random.Next(prison.Width);
                        thief.Y = random.Next(prison.Height);

                        prison.People.Add(thief);
                    }

                    // Draw the city and statistics.
                    Console.SetCursorPosition(0, 0);

                    DrawPlace(city, "CITY");

                    Console.WriteLine();
                    Console.WriteLine(
                        $"Citizens robbed: {robbedCitizens}          ");
                    Console.WriteLine(
                        $"Thieves arrested: {arrestedThieves}          ");
                    Console.WriteLine(
                        $"Thieves in prison: {prison.People.Count}          ");

                    // Display the latest news.
                    Console.WriteLine();
                    Console.WriteLine("LATEST NEWS");
                    Console.WriteLine(new string('-', 70));

                    for (int i = 0; i < 5; i++)
                    {
                        string message = i < newsFeed.Count
                            ? newsFeed[i]
                            : "";

                        Console.WriteLine(
                            message.PadRight(width));
                    }

                    Console.WriteLine();
                    Console.WriteLine(
                        "CONTROLS: [C] Citizen  [T] Thief  [P] Police  [ESC] Exit");

                    Thread.Sleep(200);
                }
            }
            catch (Exception ex)
            {
                Console.Clear();
                Console.WriteLine(
                    $"An error occurred: {ex.Message}");
            }
            finally
            {
                Console.ResetColor();
                Console.CursorVisible = true;
            }
        }

        // Add a message to the news feed.
        static void AddNews(
            List<string> newsFeed,
            string message)
        {
            newsFeed.Insert(0, message);

            if (newsFeed.Count > 5)
            {
                newsFeed.RemoveAt(newsFeed.Count - 1);
            }
        }

        // Generate a random movement direction.
        static int GetDirection(Random random)
        {
            return random.Next(-1, 2);
        }

        // Draw a location and all its inhabitants.
        static void DrawPlace(Place place, string title)
        {
            Console.WriteLine(
                ("= " + title + " ").PadRight(
                    place.Width + 2, '='));

            Person[,] grid = new Person[
                place.Height, place.Width];

            foreach (Person person in place.People)
            {
                grid[person.Y, person.X] = person;
            }

            for (int y = 0; y < place.Height; y++)
            {
                Console.Write('|');

                for (int x = 0; x < place.Width; x++)
                {
                    Person person = grid[y, x];

                    if (person == null)
                    {
                        Console.Write(' ');
                        continue;
                    }

                    if (person is Citizen)
                    {
                        Console.ForegroundColor =
                            ConsoleColor.Green;
                    }
                    else if (person is Thief)
                    {
                        Console.ForegroundColor =
                            ConsoleColor.Red;
                    }
                    else if (person is Police)
                    {
                        Console.ForegroundColor =
                            ConsoleColor.Blue;
                    }

                    Console.Write(person.GetSymbol());
                    Console.ResetColor();
                }

                Console.WriteLine('|');
            }

            Console.WriteLine(
                "+" + new string('-', place.Width) + "+");
        }
    }
}

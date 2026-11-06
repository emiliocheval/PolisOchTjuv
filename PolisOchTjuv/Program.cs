using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using TjuvochPolis;

namespace PoliceAndThieves
{
    internal class Program
    {
        static void Main(string[] args)
        {
            const int width = 100;
            const int height = 6;

            City city = new City(width, height);
            Prison prison = new Prison(10, 10);
            PoorHouse poorHouse = new PoorHouse(50, 10);

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
            Console.Clear();

            for (int i = 0; i < 30; i++)
                city.People.Add(new Citizen(random.Next(width), random.Next(height),
                    GetDirection(random), GetDirection(random)));

            for (int i = 0; i < 20; i++)
                city.People.Add(new Thief(random.Next(width), random.Next(height),
                    GetDirection(random), GetDirection(random)));

            for (int i = 0; i < 10; i++)
                city.People.Add(new Police(random.Next(width), random.Next(height),
                    GetDirection(random), GetDirection(random)));

            AddNews(newsFeed, "City simulation started.");

            try
            {
                while (true)
                {
                    bool eventHappened = false;

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
                            city.People.Add(new Citizen(random.Next(width), random.Next(height),
                                GetDirection(random), GetDirection(random)));
                            AddNews(newsFeed, "A new citizen has moved into the city.");
                        }
                        else if (key == ConsoleKey.T)
                        {
                            city.People.Add(new Thief(random.Next(width), random.Next(height),
                                GetDirection(random), GetDirection(random)));
                            AddNews(newsFeed, "A new thief has entered the city.");
                        }
                        else if (key == ConsoleKey.P)
                        {
                            city.People.Add(new Police(random.Next(width), random.Next(height),
                                GetDirection(random), GetDirection(random)));
                            AddNews(newsFeed, "A new police officer has joined the city.");
                        }
                    }

                    // Release thieves whose prison sentences have ended.
                    for (int i = prison.People.Count - 1; i >= 0; i--)
                    {
                        Thief released = (Thief)prison.People[i];

                        if (DateTime.Now >= released.ReleaseTime)
                        {
                            prison.People.RemoveAt(i);
                            released.X = random.Next(width);
                            released.Y = random.Next(height);
                            city.People.Add(released);
                            AddNews(newsFeed, "A thief has been released from prison.");
                        }
                    }

                    // Release citizens from the poorhouse after 60 seconds.
                    for (int i = poorHouse.People.Count - 1; i >= 0; i--)
                    {
                        Citizen resident = (Citizen)poorHouse.People[i];

                        if (DateTime.Now >= resident.PoorHouseSince.AddSeconds(60))
                        {
                            poorHouse.People.RemoveAt(i);
                            resident.GiveStartingItems();
                            resident.X = random.Next(width);
                            resident.Y = random.Next(height);
                            city.People.Add(resident);
                            AddNews(newsFeed, "A citizen has left the poorhouse.");
                        }
                    }

                    // Move all people.
                    city.MoveAll();
                    prison.MoveAll();
                    poorHouse.MoveAll();

                    List<Thief> thievesToPrison = new List<Thief>();
                    List<Citizen> citizensToPoorHouse = new List<Citizen>();

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
                            if ((firstPerson is Thief && secondPerson is Citizen) ||
                                (firstPerson is Citizen && secondPerson is Thief))
                            {
                                Citizen citizen = firstPerson is Citizen
                                    ? (Citizen)firstPerson : (Citizen)secondPerson;
                                Thief thief = firstPerson is Thief
                                    ? (Thief)firstPerson : (Thief)secondPerson;

                                if (citizen.Inventory.Count > 0)
                                {
                                    int itemIndex = random.Next(citizen.Inventory.Count);
                                    Item stolenItem = citizen.Inventory[itemIndex];

                                    citizen.Inventory.RemoveAt(itemIndex);
                                    thief.Inventory.Add(stolenItem);

                                    robbedCitizens++;
                                    eventHappened = true;

                                    AddNews(newsFeed,
                                        $"ROBBERY: A thief stole {stolenItem.Name}!");

                                    if (citizen.Inventory.Count == 0 &&
                                        !citizensToPoorHouse.Contains(citizen))
                                    {
                                        citizensToPoorHouse.Add(citizen);
                                    }
                                }
                            }
                            // A police officer encounters a thief.
                            else if ((firstPerson is Police && secondPerson is Thief) ||
                                     (firstPerson is Thief && secondPerson is Police))
                            {
                                Police officer = firstPerson is Police
                                    ? (Police)firstPerson : (Police)secondPerson;
                                Thief thief = firstPerson is Thief
                                    ? (Thief)firstPerson : (Thief)secondPerson;

                                if (thief.Inventory.Count > 0 &&
                                    !thievesToPrison.Contains(thief))
                                {
                                    int stolenItemCount = thief.Inventory.Count;

                                    officer.Inventory.AddRange(thief.Inventory);
                                    thief.Inventory.Clear();

                                    // Ten seconds in prison per stolen item.
                                    int sentenceSeconds = stolenItemCount * 10;
                                    thief.ReleaseTime = DateTime.Now.AddSeconds(sentenceSeconds);

                                    arrestedThieves++;
                                    eventHappened = true;
                                    thievesToPrison.Add(thief);

                                    AddNews(newsFeed,
                                        $"ARREST: Thief sentenced to {sentenceSeconds} seconds!");
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

                    // Move citizens without belongings to the poorhouse.
                    foreach (Citizen poorCitizen in citizensToPoorHouse)
                    {
                        city.People.Remove(poorCitizen);
                        poorCitizen.PoorHouseSince = DateTime.Now;
                        poorCitizen.X = random.Next(poorHouse.Width);
                        poorCitizen.Y = random.Next(poorHouse.Height);
                        poorHouse.People.Add(poorCitizen);
                        AddNews(newsFeed, "A citizen lost everything and ended up in the poorhouse.");
                    }

                    int thievesFree = 0;
                    foreach (Person person in city.People)
                    {
                        if (person is Thief) thievesFree++;
                    }

                    // Draw everything in one go from the top of the screen.
                    Console.SetCursorPosition(0, 0);

                    DrawPlace(city, "CITY");
                    WriteLine($"Citizens robbed: {robbedCitizens}   Thieves arrested: {arrestedThieves}", width);

                    DrawPlace(prison, "PRISON");
                    WriteLine($"In prison: {prison.People.Count}   At large: {thievesFree}", width);

                    DrawPlace(poorHouse, "POORHOUSE");

                    Console.WriteLine("= LATEST NEWS ".PadRight(width + 2, '='));
                    for (int i = 0; i < 3; i++)
                    {
                        string message = i < newsFeed.Count ? newsFeed[i] : "";
                        WriteLine(message, width);
                    }

                    Console.Write("[C] Citizen  [T] Thief  [P] Police  [ESC] Exit".PadRight(width + 2));

                    // One pause per round, after drawing, so events can be read.
                    Thread.Sleep(eventHappened ? 1500 : 200);
                }
            }
            catch (Exception ex)
            {
                Console.Clear();
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
            finally
            {
                Console.ResetColor();
                Console.CursorVisible = true;
            }
        }

        // Write a padded line so old characters are overwritten.
        static void WriteLine(string text, int width)
        {
            Console.WriteLine(text.PadRight(width + 2));
        }

        static void AddNews(List<string> newsFeed, string message)
        {
            newsFeed.Insert(0, message);

            if (newsFeed.Count > 3)
            {
                newsFeed.RemoveAt(newsFeed.Count - 1);
            }
        }

        static int GetDirection(Random random)
        {
            return random.Next(-1, 2);
        }

        // Draw a place. Each row is built in memory and written in colored chunks,
        // which is much faster than writing one character at a time.
        static void DrawPlace(Place place, string title)
        {
            Console.WriteLine(("= " + title + " ").PadRight(place.Width + 2, '='));

            Person[,] grid = new Person[place.Height, place.Width];

            foreach (Person person in place.People)
            {
                grid[person.Y, person.X] = person;
            }

            StringBuilder buffer = new StringBuilder();

            for (int y = 0; y < place.Height; y++)
            {
                Console.Write('|');

                ConsoleColor current = ConsoleColor.Gray;
                buffer.Clear();

                for (int x = 0; x < place.Width; x++)
                {
                    Person person = grid[y, x];

                    ConsoleColor color = ConsoleColor.Gray;
                    char symbol = ' ';

                    if (person != null)
                    {
                        symbol = person.GetSymbol();

                        if (person is Citizen) color = ConsoleColor.Green;
                        else if (person is Thief) color = ConsoleColor.Red;
                        else if (person is Police) color = ConsoleColor.Blue;
                    }

                    if (color != current)
                    {
                        Console.Write(buffer.ToString());
                        buffer.Clear();
                        Console.ForegroundColor = color;
                        current = color;
                    }

                    buffer.Append(symbol);
                }

                Console.Write(buffer.ToString());
                Console.ResetColor();
                Console.WriteLine('|');
            }

            Console.WriteLine("+" + new string('-', place.Width) + "+");
        }
    }
}
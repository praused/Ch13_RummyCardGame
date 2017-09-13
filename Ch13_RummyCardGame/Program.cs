using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CardLib;

namespace Ch13_RummyCardGame
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("KarliCards: a new and exciting card game.");
            Console.WriteLine("To win you must have 7 cards of the same suit in your hand.");
            Console.WriteLine();

            bool inputOK = false;
            int choice = -1;
            do
            {
                Console.WriteLine("How many players (2-7)?");
                string input = Console.ReadLine();
                try
                {
                    choice = Convert.ToInt32(input);
                    if (choice >= 2 && choice <= 7) inputOK = true;
                }
                catch
                {
                    //Ignore failed conversions, just continue prompting
                }
            } while (inputOK == false);

            Player[] players = new Player[choice];
            for (int p = 0; p < players.Length; p++)
            {
                Console.WriteLine($"Player {p + 1}, enter your name.");
                string playerName = Console.ReadLine();
                players[p] = new Player(playerName);
            }

            Game newGame = new Game();
            newGame.SetPlayers(players);
            int whoWon = newGame.PlayGame();
            Console.WriteLine($"{players[whoWon].Name} has won the game!");
            Console.ReadKey();
        }
    }
}

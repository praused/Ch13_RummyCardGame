using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CardLib;

namespace Ch13_RummyCardGame
{
    public class Game
    {
        private int currentCard;
        private Deck playDeck;
        private Player[] players;
        private Cards discardedCards;

        public Game()
        {
            currentCard = 0;
            playDeck = new Deck(true);
            playDeck.LastCardDrawn += new LastCardDrawnHandler(Reshuffle);
            playDeck.Shuffle();
            discardedCards = new Cards();
        }

        private void Reshuffle(Deck currentDeck)
        {
            Console.WriteLine("Discarded cards reshuffled into deck.");
            currentDeck.Shuffle();
            discardedCards.Clear();
            currentCard = 0;
        }

        public void SetPlayers(Player[] newPlayers)
        {
            if (newPlayers.Length > 7)
            {
                throw new ArgumentException("A maximum of seven players may play this game.");

            }
            if (newPlayers.Length < 2)
            {
                throw new ArgumentException("A minimum of 2 players may play this game.");
            }
            players = newPlayers;
        }

        private void DealHands()
        {
            for (int p = 0; p < players.Length; p++)
            {
                for (int c = 0; c < 7; c++)
                {
                    players[p].PlayHand.Add(playDeck.GetCard(currentCard++));
                }
            }
        }

        public int PlayGame()
        {
            if (players == null) return -1;

            DealHands();

            bool gameWon = false;
            int currentPlayer;
            Card playCard = playDeck.GetCard(currentCard++); //playCard is the card faceup on the stack of discarded cards
            discardedCards.Add(playCard);

            do
            {
                for (currentPlayer = 0; currentPlayer < players.Length; currentPlayer++)
                {
                    Console.WriteLine($"{players[currentPlayer].Name}'s turn.");
                    Console.WriteLine("Current hand:");
                    foreach (Card card in players[currentPlayer].PlayHand)
                    {
                        Console.WriteLine(card);
                    }
                    Console.WriteLine($"Card in play: {playCard}");

                    bool inputOK = false;
                    do
                    {
                        Console.WriteLine("Press T to take card in play or D to draw:");
                        string input = Console.ReadLine();
                        if (input.ToLower() == "t")
                        {
                            Console.WriteLine($"Drawn: {playCard}");
                            if (discardedCards.Contains(playCard)) //if deck was just reshuffled it won't be there.
                            {
                                discardedCards.Remove(playCard);
                            }
                            players[currentPlayer].PlayHand.Add(playCard);
                            inputOK = true;
                        }
                        if (input.ToLower() == "d")
                        {
                            Card newCard;
                            bool cardIsAvailable; //make sure card is not already in a player's hand or discarded.
                            do
                            {
                                newCard = playDeck.GetCard(currentCard++);
                                //newCard is in discardedCards if it was in a player's hand during last reshuffle but has since been discarded.
                                cardIsAvailable = !discardedCards.Contains(newCard);
                                if (cardIsAvailable)
                                {
                                    foreach (Player testPlayer in players)
                                    {
                                        if (testPlayer.PlayHand.Contains(newCard)) //happens when newCard has been in a player's hand since last reshuffle.
                                        {
                                            cardIsAvailable = false;
                                            break; //don't need to check anymore
                                        }
                                    }
                                }
                            } while (!cardIsAvailable);
                            Console.WriteLine($"Drawn: {newCard}");
                            players[currentPlayer].PlayHand.Add(newCard);
                            inputOK = true;
                        }
                    } while (inputOK == false);

                    Console.WriteLine("New hand:");
                    for (int i = 0; i < players[currentPlayer].PlayHand.Count; i++)
                    {
                        Console.WriteLine($"{i + 1}: {players[currentPlayer].PlayHand[i]}");
                    }

                    inputOK = false;
                    int choice = -1;
                    do
                    {
                        Console.WriteLine("Choose a card to discard.");
                        string input = Console.ReadLine();
                        try
                        {
                            choice = Convert.ToInt32(input);
                            if (choice > 0 && choice <= 8) inputOK = true;
                        }
                        catch (Exception)
                        {
                            //ignore failed conversions, just keep prompting.
                        }
                    } while (inputOK == false);

                    playCard = players[currentPlayer].PlayHand[choice - 1];
                    players[currentPlayer].PlayHand.RemoveAt(choice - 1);
                    discardedCards.Add(playCard);
                    Console.WriteLine($"Discarded: {playCard}");

                    Console.WriteLine();

                    gameWon = players[currentPlayer].HasWon();
                    if (gameWon == true) break; //stop taking turns

                }
            } while (gameWon == false);

            return currentPlayer;
        }

    }
}

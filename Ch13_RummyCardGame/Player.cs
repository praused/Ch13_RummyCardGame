using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CardLib;

namespace Ch13_RummyCardGame
{
    public class Player
    {
        public string Name { get; private set; }

        public Cards PlayHand { get; private set; }

        private Player()
        {
        }

        public Player(string name)
        {
            Name = name;
            PlayHand = new Cards();
        }

        public bool HasWon()
        {

            Cards tempHand = (Cards)PlayHand.Clone();

            //start by finding three-of-a-kind and four-of-a-kind sets.
            bool fourOfAKind = false;
            bool threeOfAKind = false;
            int fourRank = -1;
            int threeRank = -1;
            int cardsOfRank;
            for (int matchRank = 0; matchRank < 13; matchRank++)
            {
                cardsOfRank = 0;
                foreach (Card c in tempHand)
                {
                    if (c.rank == (Rank)matchRank)
                    {
                        cardsOfRank++;
                    }
                }
                if (cardsOfRank == 4)
                {
                    fourRank = matchRank;
                    fourOfAKind = true;
                }
                if (cardsOfRank == 3)
                {
                    if (threeOfAKind == true)//true only if there was three of another rank.
                    {
                        return false;//two three-of-a-kind is not a winning hand.
                    }
                    threeRank = matchRank;
                    threeOfAKind = true;
                }
            }

            if (threeOfAKind && fourOfAKind)
            {
                return true; //player wins.
            }

            //simplify hand by removing cards already being used a winning condition.
            //BUT: This could cheat you out of a winning condition if one of you four-of-a-kind cards  was needed for a run!
            if (fourOfAKind || threeOfAKind)
            {
                for (int cardIndex = tempHand.Count - 1; cardIndex >= 0; cardIndex--)
                {
                    if (tempHand[cardIndex].rank == (Rank)fourRank || tempHand[cardIndex].rank == (Rank)threeRank)
                    {
                        tempHand.RemoveAt(cardIndex);
                    }
                }
            }

            //if method hasn't already returned then one of the following is true:
            //- tempHand contains 7 cards because no sets were found. (Need a run of 4 and a run of three to win.)
            //- tempHand contains 4 cards because a three-of-a-kind was found. (Need a run of 3 to win.)
            //- tempHand contains 3 cards because a four-of-a-kind was found. (Need a run of 4 to win.)

            //next look for runs.
            bool fourOfASuit = false;
            bool threeOfASuit = false;
            int fourSuit = -1;
            int threeSuit = -1;
            int cardsOfSuit;
            for (int matchSuit = 0; matchSuit < 4; matchSuit++)
            {
                cardsOfSuit = 0;
                foreach (Card c in tempHand)
                {
                    if (c.suit == (Suit)matchSuit)
                    {
                        cardsOfSuit++;
                    }
                }
                //cardOfSuite values of 6, 5, 2, 1 and 0 can't satisfy winning conditions.
                if (cardsOfSuit == 7)//hand might have 2 runs.
                {
                    fourSuit = matchSuit;
                    fourOfASuit = true;
                    threeSuit = matchSuit;
                    threeOfASuit = true;
                }
                if (cardsOfSuit == 4)
                {
                    fourSuit = matchSuit;
                    fourOfASuit = true;
                }
                if (cardsOfSuit == 3)
                {
                    threeSuit = matchSuit;
                    threeOfASuit = true;
                }
            }

            if (!(threeOfASuit || fourOfASuit))
            {
                return false;
            }

            if (tempHand.Count == 7)
            {
                if (!(threeOfASuit && fourOfASuit))
                {
                    return false;
                }

                Cards set1 = new Cards();
                Cards set2 = new Cards();
                if (threeSuit == fourSuit)//all 7 cards have the same suit
                {
                    int maxVal, minVal;
                    GetLimits(tempHand, out maxVal, out minVal);
                    for (int cardIndex = tempHand.Count - 1; cardIndex >= 0; cardIndex--)
                    {
                        if (((int)tempHand[cardIndex].rank) < (minVal + 3) || ((int)tempHand[cardIndex].rank) > (maxVal - 3))
                        {
                            tempHand.RemoveAt(cardIndex);//remove all cards that could be in three set runs with the highest and lowest cards (inclusive).
                        }
                    }
                    if (tempHand.Count != 1)
                    {
                        return false;//can't have 2 runs if more than one card is left
                    }
                    if (((int)tempHand[0].rank) == (minVal + 3) || ((int)tempHand[0].rank) == (maxVal - 3))
                    {
                        return true;//remaining card makes a run of four with one of the removed runs of three.
                    }
                    else
                    {
                        return false;//only had two runs of three.
                    }
                }

                //if three and four card suits are different
                foreach (Card card in tempHand)
                {
                    if (card.suit == (Suit)threeSuit)
                    {
                        set1.Add(card);
                    }
                    else
                    {
                        set2.Add(card);
                    }
                }
                if (isSequential(set1) && isSequential(set2))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            //if four cards remain in tempHand, because a three-of-a-kind was found
            if (tempHand.Count == 4)
            {
                if (!fourOfASuit)
                {
                    return false;
                }
                if (isSequential(tempHand))
                {
                    return true;
                }
            }

            //if three cards remain in tempHand, because a four-of-a-kind was found
            if (tempHand.Count == 3)
            {
                if (!threeOfASuit)
                {
                    return false;
                }
                if (isSequential(tempHand))
                {
                    return true;
                }
            }

            return false;
        }

        private bool isSequential(Cards cards)//same suit is assumed
        {
            int maxVal, minVal;
            GetLimits(cards, out maxVal, out minVal);
            if ((maxVal - minVal) == (cards.Count - 1))
            {
                return true;//cards are a run
            }
            else
            {
                return false;
            }
        }

        private void GetLimits(Cards cards, out int maxVal, out int minVal)//same suit is assumed
        {
            maxVal = 0;
            minVal = 14;
            foreach (Card card in cards)
            {
                if ((int)card.rank > maxVal)
                {
                    maxVal = (int)card.rank;
                }
                if ((int)card.rank < minVal)
                {
                    minVal = (int)card.rank;
                }
            }
        }

    }
}

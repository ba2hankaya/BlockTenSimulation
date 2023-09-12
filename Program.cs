using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices;
namespace Block10
{
    internal class Block10
    {
        public static List<Card>? Deck;
        public static Card[]? Table;

        public static string[]? signs;
        static void Main(string[] args)
        {
            //set string[] to get the signs from while creating the deck
            signs = new string[] 
            { 
                "Clubs",
                "Hearts",
                "Diamonds",
                "Spades"
            };
            
            //define a list to store the results in
            List<int> results_list = new List<int>();

            //for loop for simulating the number of games the user wants to simulate(given in args[0])
            for(int expNum =0; expNum<Convert.ToInt32(args[0]);expNum++){
                //Splits games
                Log("Game: " + expNum.ToString() + "----------------------------------------------------------------------------------");
                // define/reset counter to count the number of pairs eliminated
                int count = 0;

                // set the game
                Readythegame();

                //print the table at the start of each game
                PrintTable();
                //starts with while true since we want to go as long as we can
                while(true){
                    try
                    {
                        //find a pair
                        List<int[]> pairs_list = FindPairs();
                        //if there are no pairs, break out of the while loop, game ends
                        if(pairs_list.Count()==0){break;}

                        //if there is a pair, print the indexes so it can be observed
                        string s = string.Format("Remove Pair:{0},{1}",pairs_list[0][0].ToString(),pairs_list[0][1].ToString());
			Log(s);
			
			//foreach(int a in pairs_list[0]){
                        //    Log(a.ToString());
                        //}

                        //Remove the pair and replace it with cards from the deck
                        RemoveOnePairAndReplaceIt(pairs_list[0]);

                        //print the table after changes
                        PrintTable();
                        
                    }
                    catch (Exception ex)
                    {
                        //if an error occurs, print it(for testing and debugging)
                        Log(ex.ToString());
                        break;
                    
                    }
                    //increase the pairs eliminated count every round the game doesnt add
                    count++;                
                }

                //after the game ends, print the number of pairs eleminated
                Log("Total Pairs Removed: " + count.ToString());

                //add the number of pairs eliminated to the list to compare results in the future
                results_list.Add(count);
            }

            //put the results into a list ordered by value and grouped by value
            var elementCount = results_list.OrderBy(x=>x).GroupBy(x => x).ToDictionary(x => x.Key,x => x.Count());

            //print the values in the list
            foreach(var entry in elementCount){
                Log($"{entry.Key}:{entry.Value}");
            }
        }

        public static void Readythegame(){
            //instantiate Deck defined publicly
            Deck = new List<Card>();
            //instantiate table which was defined publicly
            Table = new Card[9];

            //Fill the deck with cards
            for(int cardValue = 1; cardValue <= 13; cardValue++){
                for(int signNum = 0; signNum< 4; signNum++){
                    Deck.Add(new Card(signs[signNum],cardValue));
                }
            }

            //Set table
            SetTheTableInit();
        }

        //set the table when it is empty
        public static void SetTheTableInit(){
            //pick 9 cards from the deck and put them on the table
            for(int index = 0;index<9;index++){
                Table[index] = PickCardFromDeck();
            }
            
        }

        //remove a pair of cards by index and replace them from the deck
        public static void RemoveOnePairAndReplaceIt(int[] pair)
        {
            Table[pair[0]] = PickCardFromDeck();
            Table[pair[1]] = PickCardFromDeck();
        }


        // print the table
        public static void PrintTable()
        {
            int i = 0;
            for(int index = 0;index<9;index++){
                if(Table[index]!=null){
                    //print index of the card + its value + its sign
                    string s = string.Format("{0}){1,-10}{2}",i.ToString(),Table[index].Num().ToString(),Table[index].Sign());
		    Log(s);
		    //Log(i.ToString()+") " + Table[index].Num().ToString() + "\t\t" + Table[index].Sign());
                }else{
                    //if the card is null, print empty(a card is null when there is no card in the deck and you try to draw a card)
                    Log("Empty");
                }
                i++;
            }
        }

        public static List<int[]> FindPairs(){
            //make a list for matching pairs(kind of unnecessary because only one is going to be used)
            List<int[]> matchingPairs = new List<int[]>();

            //enum the list starting from first to second from last, since the last card cant be paired with a card after it
            for(int currentnum = 0; currentnum<Table.Count()-1;currentnum++){
                //from the selected card to the end of the list, see if there is a card you can eliminate the selected card with
                for(int toCompare = currentnum;toCompare < Table.Count();toCompare++){
                    //if that pair can be eliminated and the pair isnt with the card itself, add it to the list
                    if(pairCanBeEliminated(Table[currentnum],Table[toCompare]) && currentnum != toCompare){matchingPairs.Add(new int[]{currentnum,toCompare});}
                }
            }
            //return the list
            return matchingPairs;
        }


        //see if a pair can be eliminated
        public static bool pairCanBeEliminated(Card card1, Card card2){
            //if either card is null(cards can be null on the table if there are no more cards in the deck) return false since they can't be matched
            if(card1==null || card2==null){
                return false;
            }

            //since 10s can't be paired per the game rules, if either card is 10 return false.
            if(card1.Value() == 10 || card2.Value() == 10){
                return false;
            }
            
            //if the card value is greater than 10, it can be paired with a card of the same value per the rules of the game, so if the card1 value is greater than 10 and equal to card2 return true 
            if(card1.Value() > 10){
                if(card1.Value() == card2.Value()){
                    return true;
                }else{
                    return false;
                }
            }
            
            //if the card1 value is less than 10, the card can be paired with a card that completes it to 10. if card 2 completes card1 to 10 return true
            if(card1.Value() < 10){
                if(card1.Value()+card2.Value() == 10){
                    return true;
                }else{
                    return false;
                }
            }


            return false;
        }


        //I used console.writeline a lot while compiling and debugging, so I shortened it.
        public static void Log(string str){
            Console.WriteLine(str);
        }

        //Picks a random card from the deck and returns it
        public static Card PickCardFromDeck(){
            //define random
            Random random = new Random();

            //choose card index from 0 to number of cards left in the deck
            int cardIndex = (int)random.NextInt64(0,Deck.Count);

            //if there are no cards in the deck to pick from, return null
            if(Deck.Count == 0){
                return null;
            }

            //if not, return the card and remove it from the deck
            Card pickedCard = Deck[cardIndex];
            Deck.RemoveAt(cardIndex);
            return pickedCard;
        }
    }


    //card class
    class Card{
        private string sign = "";

        //num and value are a bit confusing, value is the raw number of the card while num is the name of the card you would see in a real deck
        private string num = "";
        private int value = 0;
        public Card(string _sign, int _value){
            sign = _sign;
            value = _value;
            //according to the value assigned in construction, assign real life card numbers/names to num variable 
            switch (value){
                case 1:
                    num = "Ace";
                    break;
                case 11:
                    num = "Jack";
                    break;
                case 12:
                    num = "Queen";
                    break;
                case 13:
                    num = "King";
                    break;
                default:
                    num = value.ToString();
                    break;
            }
        }

        //return variables of cards
        public string Sign(){
            return sign;
        }
        
        public string Num(){
            return num;
        }

        public int Value(){
            return value;
        }

    }
}

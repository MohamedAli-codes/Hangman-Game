using System.IO;

namespace Hangman_game
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("------- Welcome to Hangman game ---------");
            string name = GetStringInput("Enter your name: ");
            int noOftries;
            Console.Write("Enter the number of tries you want: ");
            while (!int.TryParse(Console.ReadLine(), out noOftries) || noOftries <= 0)
            {
                Console.WriteLine("Invalid input. Please enter a positive number for tries.");
                Console.Write("Enter the number of tries you want: ");
            }

            HangmanGame newGame = new HangmanGame(name,noOftries);

            Console.WriteLine("--------------------------");
            while (true)
            {
                newGame.StartGame();  // Start a new game
                Console.WriteLine("----------------------------");
                Console.WriteLine();

                // Ask the user if they want to play again
                char input;
                do
                {
                    Console.Write("Do you want a new game (y/n)? ");
                    string userInput = Console.ReadLine();
                    if (userInput.Length == 1 && (userInput[0] == 'y' || userInput[0] == 'n'))
                    {
                        input = userInput[0];
                        break;
                    }
                    else
                    {
                        Console.WriteLine("Invalid input. Please enter 'y' or 'n'.");
                    }
                } while (true);

                if (input == 'n')
                {
                    Console.WriteLine("Goodbye! see you later. ^_^ ");
                    break;
                }
                else
                {
                    newGame.ResetTries(noOftries);
                    newGame.GenerateRandomWord(); // Add this line to reset tries for a new round
                }
            }

        }

        public static string GetStringInput(string s)
        {
            while (true)
            {
                Console.Write(s);
                string input = Console.ReadLine();
                if( input != null && input != string.Empty ) return input;
            }
        }
    }
    internal class Player
    {
        public string Name { get; set; }

        public Player(string name)
        {
            Name = name;
        }

        public char GuessLetter()
        {
            char c;
            while (true)
            {
                c = Console.ReadKey().KeyChar; 

                if (char.IsLetter(c))
                {
                    return char.ToLower(c); 
                }
                // Inform the user about invalid input
                Console.WriteLine();
                Console.Write("Invalid input. Please enter a single letter: ");
            }
        }

        public override string ToString()
        {
            return $"{Name}" ;
        }
    }

    internal class WordsStoreList : List<string> 
    {
        private string filePath;

        public WordsStoreList(string path)
        {
            filePath = path;
        }

        public List<string> GenerateWords()
        {
            List<string> wordsList = new List<string>();
            try
            {
                using (StreamReader reader = new StreamReader(filePath))
                {
                    string line;
                    while ( (line = reader.ReadLine())!= null )
                    {
                        wordsList.Add(line);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            return wordsList;
        }
    }

    internal class HangmanGame
    {
        private Player p1;
        private WordsStoreList wordsStore;
        private int noOfTries;
        private List<string> words;
        private string wordToGuess;

        public HangmanGame(string playerName,int _noOfTries )
        {
            p1 = new Player(playerName);
            string projectRoot = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName;
            wordsStore = new WordsStoreList(Path.Combine(projectRoot, "WordsStorage.txt"));
            words = wordsStore.GenerateWords();
            noOfTries = _noOfTries;
        }

        public string GenerateRandomWord()
        {
            Random random = new Random();
            int wIndex = random.Next(words.Count);
            wordToGuess = words[wIndex];
            return wordToGuess;
        }
        public void ResetTries(int tries)
        {
            noOfTries = tries;
        }

        public void StartGame()
        {

            GenerateRandomWord(); //initialize random wordToGuess
            string userGuess = new string('_', wordToGuess.Length);

            while (noOfTries > 0)
            {
                Console.Write($"{userGuess} | tries left:{noOfTries}| what is your guess: ");
                char letter = p1.GuessLetter();
                if (wordToGuess.Contains(letter))
                {
                    // Replace underscores with the guessed letter at the correct positions
                    char[] userGuessArray = userGuess.ToCharArray();
                    for (int i = 0; i < wordToGuess.Length; i++)
                    {
                        if (wordToGuess[i] == letter)
                        {
                            userGuessArray[i] = letter;
                        }
                    }
                    userGuess = new string(userGuessArray);
                }
                else
                {
                    noOfTries--;
                }
                Console.WriteLine();
                if (userGuess == wordToGuess)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"Congratulation {p1.Name} you have won!");
                    break;
                }
            }
            if (userGuess != wordToGuess)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Game over! The word was: {wordToGuess}");
            }
            Console.ForegroundColor = ConsoleColor.White; //reset colors
        }
    }
}

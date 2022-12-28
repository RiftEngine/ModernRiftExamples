using Rift.ModernRift.Core;

namespace Challenge04
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Engine.GameReference = new GameClass();

            Engine.Initialize();

            Engine.Start();
        }
    }

    public class LoopSubroutine : Subroutine
    {
        public int targetNumber;
        private bool _isDone = false;
        public int maxGuesses;
        public int guessCount;

        public override bool IsDone()
        {
            return _isDone;
        }

        public override void Initialize() { }

        public override void Start() { }

        public override void Update()
        {
            Console.Write("Enter a guess between 1 and 10: ");
            string guess = Console.ReadLine();
            if (!ValidateGuess(guess))
            {
                Console.WriteLine("The guess was not a valid number. ");
                return;
            }
            int value = int.Parse(guess);
            MessageHandler.AddMessage("count");
            MessageHandler.SendMessages();
            if (guessCount == maxGuesses)
            {
                Console.WriteLine("You ran out of guesses. Better luck next time.");
                _isDone = true;
                return;
            }
            if (value < targetNumber)
            {
                Console.WriteLine("Your guess was too low.");
            }
            else if (value > targetNumber)
            {
                Console.WriteLine("Your guess was too high.");
            }
            else
            {
                Console.WriteLine("Your guess was correct! Congratulations!");
                _isDone = true;
                return;
            }
            if (guessCount == maxGuesses)
            {
                Console.WriteLine("You ran out of guesses. Better luck next time.");
                _isDone = true;
            }
        }
        public override void End()
        {
            _isDone = false;
        }

        public bool ValidateGuess(string guess)
        {
            try
            {
                int.Parse(guess);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public void SetTargetNumber(int targetNumber)
        {
            this.targetNumber = targetNumber;
        }

        public void SetMaxGuesses(int maxGuesses)
        {
            this.maxGuesses = maxGuesses;
        }

        public void SetGuessCount(int guessCount)
        {
            this.guessCount = guessCount;
        }
    }

    public class GameClass : Game
    { 
        // Main Game
        Directive GenerateDirective;
        Directive UpdateSubroutineDirective;
        Directive StartMainSubroutineDirective;
        Command StartCommand;
        LoopSubroutine mainSubroutine;
        Directive CountDirective;
        int GuessCount = 0;
        int MaxGuesses = 5;
        Directive ListCountDirective;
        Directive Reset;

        public int number;

        // High Scores
        int? HighScore = null;
        Directive ListHighScoreDirective;
        Command ListHighScoreCommand;

        public override void Initialize()
        { 
            configuration = new Configuration.ConfigurationBuilder().FromConfigFile("configuration.json").Build();

            mainSubroutine = new LoopSubroutine();

            GenerateDirective = new Directive(() => { GenerateNumber(1, 10); });

            UpdateSubroutineDirective = new Directive(() => {
                mainSubroutine.SetTargetNumber(number);
                mainSubroutine.SetMaxGuesses(MaxGuesses);
            });

            StartMainSubroutineDirective = new Directive(() => { SubroutineManager.StartSubroutine(mainSubroutine); });

            ListCountDirective = new Directive(() => { if (GuessCount != MaxGuesses) Console.WriteLine($"You took {GuessCount.ToString()} guesses to guess the number."); });

            Reset = new Directive(() => {
                if (GuessCount != MaxGuesses)
                {
                    MaxGuesses--;
                }
                if (GuessCount < HighScore)
                {
                    HighScore = GuessCount;
                }
                if (HighScore == null)
                {
                    HighScore = GuessCount;
                }
                GuessCount = 0;
                mainSubroutine.SetGuessCount(0);
            });

            StartCommand = new Command("start", "Starts a new game.", GenerateDirective, UpdateSubroutineDirective, StartMainSubroutineDirective, ListCountDirective, Reset);

            CountDirective = new Directive(() => {
                GuessCount++;
                mainSubroutine.SetGuessCount(GuessCount);
            });
            CountDirective.AddTrigger("count");

            ListHighScoreDirective = new Directive(() => { if (HighScore != null) Console.WriteLine(HighScore); else Console.WriteLine("No games have been played."); });

            ListHighScoreCommand = new Command("score", "Lists the best score that has been aquired.", ListHighScoreDirective);
        }

        public override void Start()
        {
            AddCommand(StartCommand);
            AddCommand(ListHighScoreCommand);
            MessageHandler.AddDirective(CountDirective);
        }


        public void GenerateNumber(int min, int max)
        {
            Random rnd = new Random();
            number = rnd.Next(min, max + 1);
        }
    }
}
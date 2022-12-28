using Rift.ModernRift.Core;

namespace Challenge01
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
            MessageHandler.AddMessage("count");
            MessageHandler.SendMessages();
            int value = int.Parse(guess);
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
    }

    public class GameClass : Game
    {
        Directive GenerateDirective;
        Directive UpdateSubroutineDirective;
        Directive StartMainSubroutineDirective;
        Command StartCommand;
        LoopSubroutine mainSubroutine;
        Directive CountDirective;
        int GuessCount = 0;
        Directive ListCountDirective;
        Directive ResetCountDirective;

        public int number;

        public override void Initialize()
        {
            configuration = new Configuration.ConfigurationBuilder().FromConfigFile("configuration.json").Build();

            mainSubroutine = new LoopSubroutine();

            GenerateDirective = new Directive(() => { GenerateNumber(1, 10); });

            UpdateSubroutineDirective = new Directive(() => { mainSubroutine.SetTargetNumber(number); });

            StartMainSubroutineDirective = new Directive(() => { SubroutineManager.StartSubroutine(mainSubroutine); });

            ListCountDirective = new Directive(() => { Console.WriteLine($"You took {GuessCount.ToString()} guesses to guess the number."); });

            ResetCountDirective = new Directive(() => { GuessCount = 0; });

            StartCommand = new Command("start", "Starts a new game.", GenerateDirective, UpdateSubroutineDirective, StartMainSubroutineDirective, ListCountDirective, ResetCountDirective);

            CountDirective = new Directive(() => { GuessCount++; });
            CountDirective.AddTrigger("count");
        }

        public override void Start()
        {
            AddCommand(StartCommand);
            MessageHandler.AddDirective(CountDirective);
        }


        public void GenerateNumber(int min, int max)
        {
            Random rnd = new Random();
            number = rnd.Next(min, max + 1);
        }
    }
}
using Rift.ModernRift.Core;

namespace Tutorial
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

        public int number;

        public override void Initialize()
        {
            configuration = new Configuration.ConfigurationBuilder().FromConfigFile("configuration.json").Build();

            mainSubroutine = new LoopSubroutine();

            GenerateDirective = new Directive(() => { GenerateNumber(1, 10); });

            UpdateSubroutineDirective = new Directive(() => { mainSubroutine.SetTargetNumber(number); });

            StartMainSubroutineDirective = new Directive(() => { SubroutineManager.StartSubroutine(mainSubroutine); });

            StartCommand = new Command("start", "Starts a new game.", GenerateDirective, UpdateSubroutineDirective, StartMainSubroutineDirective);
        }

        public override void Start()
        {
            AddCommand(StartCommand);
        }


        public void GenerateNumber(int min, int max)
        {
            Random rnd = new Random();
            number = rnd.Next(min, max + 1);
        }
    }
}
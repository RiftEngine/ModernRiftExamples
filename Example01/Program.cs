using Rift.ModernRift.Core;

namespace Example01
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

    public class GameClass : Game
    {
        public override void Initialize()
        {
            configuration = new Configuration.ConfigurationBuilder().FromConfigFile("configuration.json").Build();
        }
    }
}
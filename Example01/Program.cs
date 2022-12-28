using Rift.ModernRift.Core;

namespace Example01
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Configuration configuration = new Configuration.ConfigurationBuilder()
                .FromConfigFile("configuration.json")
                .Build();
            Engine.GameReference = new GameClass(configuration);

            Engine.Initialize();

            Engine.Start();
        }
    }

    public class GameClass : Game
    {
        public GameClass(Configuration config) : base(config) { }
    }
}
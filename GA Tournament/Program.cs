using System;

namespace GA_Tournament
{
    internal static class Program
    {        
        public static readonly Random _rand = new Random();


        public static void Main(string[] args)
        {
            var pop = new Population(500, 0.1);
            for (var i = 0; i < 100; ++i)
            {
                pop.CreateAndMergePop();
            }
            
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using static GA_Tournament.Program;
namespace GA_Tournament
{
    public class Population
    {
        private List<DNA>population = new List<DNA>();
        private List<DNA> tournamentMembers = new List<DNA>();
        private readonly List<DNA> newPopulation = new List<DNA>();
        
        private readonly double m_mutationRate;

        //Constructs a population of DNA objects
        public Population(int maxPopulation, double mutation)
        {
            m_mutationRate = mutation;
            for (var i = 0; i < maxPopulation; ++i)
            {
                population.Add(new DNA());
            }
        }
        
        public double GetResults(List<double> weights)
        {
            Network net = new Network();

            net.SetWeights(weights);

            PendulumMaths p = new PendulumMaths();
            p.initialise(1);

            Network v = new Network();
            v.SetWeights(net.GetWeights());

            double[][] motor_vals = new double[p.getcrabnum()][];

            for (int i = 0; i < motor_vals.Length; i++)
            {
                motor_vals[i] = new double[2];
            }

            do
            {
                double[][] sval = (p.getSensorValues());

                double[] inputs = new double[10];

                for (int i = 0; i < p.getcrabnum(); i++)
                {

                    for (int x = 0; x < sval[0].Length; x++)
                    {
                        inputs[x] = ((sval[i][x]) / (127) * (1 - 0)) + 1;
                    }

                    v.SetInputs(inputs);

                    v.Execute();

                    double[] outputs = v.GetOutputs();

                    motor_vals[i][0] = ((outputs[0])) * 127;
                    motor_vals[i][1] = ((outputs[1])) * 127;

                }

            }
            while (p.performOneStep(motor_vals) == 1);

            return p.getFitness();
        }

        //Calculates the fitness of the entire population
        //and assigns a fitness score to each chromosome/weight
        private void CalcFitnessOfPop(IEnumerable<DNA> originalOrNewPop)
        {
            double highestFitness = 0;
            foreach (var chromosome in originalOrNewPop)
            {
                var fitness = GetResults(chromosome.GetWeights());
                if (fitness > highestFitness)
                {
                    highestFitness = fitness; 
                }
                chromosome.SetFitness(fitness);
                //Console.WriteLine(fitness);
            }
            Console.WriteLine("Highest fitness in the population " + highestFitness);
        }
        
        //Tournament selection strategy.
        //Creates a list of 4 members ordered by their fitness.
        //The best and second best values are returned
        private (DNA,DNA) Tournament()
        {
            //Create Tournament Members
            for (var i = 0; i < 4; ++i)
            {
                //Ensure candidates are all different
                var index = _rand.Next(0, population.Count);
                if (tournamentMembers.Contains(population[index]))
                {
                    index = _rand.Next(0, population.Count);
                    tournamentMembers.Add(population[index]);
                }
                else
                {
                    tournamentMembers.Add(population[index]);
                }
            }

            tournamentMembers = tournamentMembers.OrderByDescending(x => x.GetFitness()).ToList();
            return (tournamentMembers[0], tournamentMembers[1]);
        }

        //Takes two DNA objects and performs Crossover and Mutation to produce a child.
        //The child is added to a new population
        private void GenerateChild(DNA parentA, DNA parentB)
        {
            var child = parentA.Crossover(parentB);
            var rate = RandomNumberBetween(0, m_mutationRate);
            if (rate < m_mutationRate)
            {
                child.Mutation();
            }
            newPopulation.Add(child);
        }

        private static double RandomNumberBetween(double minValue, double maxValue)
        {
            var next = _rand.NextDouble();

            return minValue + (next * (maxValue - minValue));
        }
        
        //Merges the new population into the old population.
        //Orders the list to have the best members of both populations
        //Culls the rest to retain a population of maxPopulation
        public void CreateAndMergePop()
        {
            //Calculates the fitness of the initial population
            CalcFitnessOfPop(population);
            for (var i = 0; i < population.Count; ++i)
            {
                var (parentA, parentB) = Tournament();
                /* TODO: Improve tournament clearing 
                    Hacky but works
                    Ideal: Tournament members are deleted inside Tournament method and members returned
                    are based on the index of the population, not the index of the tournament members
                */
                tournamentMembers.Clear();
                GenerateChild(parentA, parentB);
            }
            population.AddRange(newPopulation);
            CalcFitnessOfPop(population);
            population = population.OrderByDescending(x => x.GetFitness()).ToList();
            //Always cull the lower half of the list
            population.RemoveRange(population.Count/2,population.Count/2);
            newPopulation.Clear();
        }
    }
}

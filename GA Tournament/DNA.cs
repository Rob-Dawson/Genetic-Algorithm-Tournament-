using System.Collections.Generic;
using static GA_Tournament.Program;

namespace GA_Tournament
{
    public class DNA
    {   
        private readonly List<double> genes = new List<double>();
        private double m_genesFitness;
        public DNA()
        {
            for (var i = 0; i < 60; ++i)
            {
                genes.Add((_rand.NextDouble() * 2) - 1);
            }
        }

        public double GetFitness()
        {
            return m_genesFitness;
        }

        public void SetFitness(double genesFitness)
        {
            m_genesFitness = genesFitness;
        }
        public List<double> GetWeights()
        {
            return genes;
        }
        
        public DNA Crossover(DNA parentB)
        {
            DNA child = new DNA();
            for (var i = 0; i < genes.Count / 2; ++i)
            {
                child.genes[i] = this.genes[i];
            }
            
            for (var i = genes.Count / 2; i < genes.Count; ++i)
            {
                child.genes[i] = parentB.genes[i];
            }

            return child;
        }

        public void Mutation()
        {
            var index = _rand.Next(0, genes.Count);
            genes[index] = (_rand.NextDouble() * 2) - 1;
        }
    }
}
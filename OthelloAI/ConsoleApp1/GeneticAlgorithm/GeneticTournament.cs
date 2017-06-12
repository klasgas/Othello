using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using OthelloAI;

namespace GeneticAlgorithm
{
    class GeneticTournament
    {
        public void Run()
        {
            //string fileName = "c:\\temp\\population.xml";
            string fileNameTemplate = "c:\\temp\\population_{0}.xml";

            int populationSize = 15;
            int generationCount = 200;

            List<DoubleStrategy> population = LoadPopulationFromDisk(String.Format(fileNameTemplate, 19));

            //List<DoubleStrategy> population = GenerateInitialPopulation(populationSize);

            for (int g = 0; g < generationCount; g++)
            {
                Console.WriteLine("Generation: " + g);
                population = RunTournament(population);

                SavePopulationToDisk(population, String.Format(fileNameTemplate, g));

                population = GenerateNextGeneration(population);
            }
        }

        private void SavePopulationToDisk(List<DoubleStrategy> population, string fileName)
        {
            var ser = new DataContractSerializer(typeof(List<DoubleStrategy>));
            using (var output = new StringWriter())
            {
                using (var writer = new XmlTextWriter(output) { Formatting = Formatting.Indented })

                {
                    ser.WriteObject(writer, population);
                }

                var s = output.GetStringBuilder().ToString();

                File.WriteAllText(fileName, s);
            }
        }

        private List<DoubleStrategy> LoadPopulationFromDisk(string fileName)
        {
            var s = File.ReadAllText(fileName);
            var population = Deserialize<List<DoubleStrategy>>(s);
            return population;
        }

        public static T Deserialize<T>(string xml)
        {
            using (Stream stream = new MemoryStream())
            {

                byte[] data = System.Text.Encoding.UTF8.GetBytes(xml);

                stream.Write(data, 0, data.Length);

                stream.Position = 0;

                //var dataContractSerializer = new DataContractSerializer(typeof(T), EntityUtilities.GetAllKnownTypes(), int.MaxValue, true, true, null);

                var dataContractSerializer = new DataContractSerializer(typeof(T));

                return (T)dataContractSerializer.ReadObject(stream);
            }
        }

        static Random rand = new Random();

        private List<DoubleStrategy> GenerateNextGeneration(List<DoubleStrategy> oldPopulation)
        {
            int winnerCount = oldPopulation.Count / 3;
            double mutationProbability = 0.2;
            var winners = oldPopulation.GetRange(0, winnerCount);

            List<DoubleStrategy> newPopulation = new List<DoubleStrategy>();

            var winner = winners[0];
            winner.tournamentWins++;

            newPopulation.Add(winner); // include previous winner in new population

            int populationSize = oldPopulation.Count();
            while(newPopulation.Count < populationSize)
            {
                int indexA = rand.Next(winnerCount);

                int indexB = 0; 

                while(indexA == indexB)
                {
                    indexB = rand.Next(winnerCount);
                }

                var parentA = winners[indexA];
                var parentB = winners[indexB];
                var offspring = parentA.Crossover(parentB);

                var randomValue = RandomHelper.rand.NextDouble();
                if (randomValue < RandomHelper.HeavyMutationProbability)
                {
                    offspring.Mutate();
                }
                else if (randomValue < RandomHelper.SlightMutationProbability)
                {
                    offspring.MutateSlightly();
                }

                newPopulation.Add(offspring);
            }

            return newPopulation;
        }

        public List<DoubleStrategy> GenerateInitialPopulation(int populationSize)
        {
            var population = new List<DoubleStrategy>(populationSize);

            for (int i = 0; i < populationSize; i++)
            {
                var individual = new DoubleStrategy();
                individual.InitRandomWeights();
                population.Add(individual);
            }

            return population; 
        }

        public List<DoubleStrategy> RunTournament(List<DoubleStrategy> population)
        {
            foreach (var individual in population)
            {
                individual.tournamentScore = 0;
            }

            foreach (var i1 in population)
            {
                foreach (var i2 in population)
                {
                    if (i1.guid != i2.guid)
                    {
                        Console.WriteLine(i1.guid.ToString() + " vs " + i2.guid.ToString());
                        PlayGame(i1, i2);
                        //PlayGame(i2, i1);
                    }
                }
                //GC.Collect();
            }

            population = population.OrderByDescending(i => i.tournamentScore).ToList();

            return population;
        }

        private static void PlayGame(DoubleStrategy blackStrategy, DoubleStrategy whiteStrategy)
        {
            UInt64 black = 0x810000000;
            UInt64 white = 0x1008000000;

            bool blackFoundMove = true;
            bool whiteFoundMove = false;

            MoveFinder blackMoveFinder = new MoveFinder(blackStrategy);
            MoveFinder whiteMoveFinder = new MoveFinder(whiteStrategy);

            while (blackFoundMove || whiteFoundMove)
            {
                UInt64 blackMove = blackMoveFinder.FindMove(white, black);
                blackFoundMove = (blackMove != 0);
                if (blackFoundMove)
                {
                    MoveFinder.MakeMove(ref white, ref black, blackMove);
                }

                UInt64 whiteMove = whiteMoveFinder.FindMove(black, white);
                whiteFoundMove = (whiteMove != 0);
                if (whiteFoundMove)
                {
                    MoveFinder.MakeMove(ref black, ref white, whiteMove);
                }
            }

            int blackDiscsCount = MoveFinder.PopulationCount(black);
            int whiteDiscsCount = MoveFinder.PopulationCount(white);

            int scoreForWin = 3;
            int scoreForDraw = 1;

            if (blackDiscsCount > whiteDiscsCount)
            {
                blackStrategy.tournamentScore += scoreForWin;
            }
            else if(blackDiscsCount < whiteDiscsCount)
            {
                whiteStrategy.tournamentScore += scoreForWin;
            }
            else
            {
                blackStrategy.tournamentScore += scoreForDraw;
                whiteStrategy.tournamentScore += scoreForDraw;
            }

        }
    }

}

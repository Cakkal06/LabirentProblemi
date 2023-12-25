using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

namespace Qlearning
{
    internal class QAgent
    {
        QTable Table = new QTable();
        char[,] Maze;

        double epsilon = 0.1;

        State StartState = new State() { X = 8, Y = 0 };
        State FinishState = new State() { X = 5, Y = 9 };

        public QAgent(char[,] maze)
        {
            Maze = maze;
            MazeV();
        }
        public void Train(int Epsidoes = 1_000_000)
        {

            var startState = StartState;
            var goalState = FinishState;

            bool goalOkay = false;
            int theGoalEpisodeNumber = 0;

            double learningRate = 0.1;
            double discountFactor = 0.9;
            int episodes = Epsidoes;

            // Eğitim döngüsü
            for (int episode = 0; episode < episodes; episode++)
            {
                if (episode % 100_000 == 0)
                    Console.WriteLine("Yapılan eğitim : " + episode);

                var currentState = startState;
                bool gameOver = false;
                //hedefe ulaşana kadar dön
                while (!currentState.Equals(goalState) && !gameOver)// duvara çarpmak bitirir
                {

                    int selectAction = SelectAction(currentState);
                    var nextState = TakeAction(currentState, selectAction);


                    double reward = CalculateReward(currentState, nextState, goalState);// ödülü hesapla
                    double maxQValue = Table.GetMax(nextState).value;// 

                    double value = Table.Get(currentState, selectAction);// currentstate in q değerini al
                                                                               //yeni Q değerini hesapla formülle 
                    double newQValue = (1 - learningRate) * value + learningRate * (reward + discountFactor * maxQValue);

                    Table.Update(currentState, selectAction, newQValue);// yeni Q değerini table'de günceller



                    currentState = nextState;// durumu güncelle
                    if (currentState.X < 0 || currentState.Y < 0 || currentState.Y > 9 || currentState.Y > 29 || Maze[currentState.X,currentState.Y]== '█')
                    {
                        gameOver = true;
                    }
                }

                if (currentState.Equals(goalState) && !goalOkay)
                {
                    
                    theGoalEpisodeNumber = episode;
                    goalOkay = true;
                }

            }

            if (goalOkay)
            {
                Console.WriteLine("çıkışa ulaşıldı" + theGoalEpisodeNumber + ". episode'da");
            }
        }

        State TakeAction(State state,int action)
        {
            switch (action)
            {
                case 0: // Yukarı git
                    state.X--;
                    break;
                case 1: // Aşağı git
                    state.X++; 
                    break;
                case 2: // Sola git
                    state.Y--;
                    break;
                case 3: // Sağa git
                    state.Y++;
                    break;
            }
            return state;
        }

        int SelectAction(State currentState)
        {
            Random rand = new Random();
            if (rand.NextDouble() < epsilon) // epsilon-greedy stratejisi
            {
                int selectAction = rand.Next(4);
                return selectAction; // rastgele bir eylem seç
            }
            else
            {
                int selectAction = Table.GetMax(currentState).action;
                return selectAction; // rastgele bir eylem seç         
            }
        }

        double CalculateReward(State currState,State nextState,State goalState)
        {
            if (nextState.X > 9 || nextState.X < 0 || nextState.Y > 9 || nextState.Y < 0)
                return -5.0; // Sınırların dışına çıkma cezası

            if (Maze[nextState.X,nextState.Y] == '█')
                return -5.0; // Duvara çarpma cezası

            if (nextState.Equals(goalState))
            {
                Console.WriteLine("Çıkışa ulaşıldı");
                return 100.0; // Hedefe ulaşma ödülü
            }

            // Hedefe olan mesafeyi daha basit bir şekilde hesapla
            double distanceToGoal = Math.Abs(nextState.X - goalState.X) + Math.Abs(nextState.Y - goalState.Y);

            // Mesafe ne kadar küçükse, ödülü o kadar arttır
            double reward = 10.0 - distanceToGoal;

            return reward;
        }
        public void PrintMaze()
        {
            // Labirenti ekrana yazdırma
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    Console.Write(Maze[i, j]);
                }
                Console.WriteLine();
            }

        }
        void MazeV()
        {
            // labirent görselleştirme █ duvar boşluk yol
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    Maze[i, j] = Maze[i, j] == '1' ? '█' : ' ';
                }
            }

        }

        public void Watch()
        {
            foreach (var item in Table.Table)
            {

            }
        }


    }
}

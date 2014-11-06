using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace WI2 {
    class Program {
        static void Main(string[] args) {
            //string dir = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\Semester7\Web Intelligence\Git\WI2\friendships.txt";
            string sentiments = @"C:\Users\Casper\Documents\sw7_webintelligence\SentimentTrainingData.txt";
            //List<User> users = GetData(dir);
            SentimentAnalyzer analyzer = new SentimentAnalyzer(sentiments);
            analyzer.LoadReviewsQuickAndDirty();
            analyzer.learnModel();
            var hej = 0;
            //analyzer.FindTypes();
            /*
            int[,] adjacencyMatrix = MakeAdjacencyMatrix(users);
            int test = adjacencyMatrix.GetUpperBound(0);
            for (int i = 0; i <= adjacencyMatrix.GetUpperBound(0); i++) {
                FindUserClique(adjacencyMatrix, i);
            }
            PrintUsers(users);
            */
        }

        static List<User> GetData(string dir) {

            List<string> lines = new List<string>();
            List<User> users = new List<User>();
            bool isTwoNames = false;

            // Read lines from the file until the end of the file is reached.
            using (StreamReader sr = new StreamReader(dir)) {
                string line;
                while ((line = sr.ReadLine()) != null) {
                    lines.Add(line);
                }
            }


            // Sort data into a proper structure
            for (int i = 0; i < lines.Count(); i+=5) {
                string name = lines[i].Remove(0, 6);
                string friendLine = lines[i + 1].Remove(0, 9);
                List<string> friends = friendLine.Split(null).ToList();
                string summary = lines[i + 2].Split(null)[1];
                string review = lines[i + 3].Split(null)[1];

                User user = new User(name, friends, summary, review);
                users.Add(user);
            }

            foreach (User user in users) {
                List<string> names = user.GetFriends();
                List<string> replaceNames = new List<string>();

                for (int i = 0; i < names.Count() - 1; i++) {

                    isTwoNames = false;
                    string twoNames = names[i] + " " + names[i + 1];

                    foreach (User u in users) {
                        if (u.GetName() == twoNames) {
                            replaceNames.Add(twoNames);
                            isTwoNames = true;
                            break;
                        }
                    }

                    if (!isTwoNames) {
                        replaceNames.Add(names[i]);
                    }
                }

                //Handle last user in friendlist, if any
                if (!isTwoNames) {
                    replaceNames.Add(names[names.Count-1]);
                }

            }
            return users;
        }

        static void PrintUsers(List<User> users) {

            foreach (User user in users) {

                Console.WriteLine("User: " + user.GetName());
                Console.Write("Friends: ");

                foreach (string friend in user.GetFriends()) {
                    Console.Write(friend + " ");
                }
                Console.WriteLine();
                Console.WriteLine("Summary: " + user.GetSummary());
                Console.WriteLine("Review: " + user.GetReview());
                Console.WriteLine();
                Console.ReadLine();
            }
        }

        static int[,] MakeAdjacencyMatrix(List<User> users) {

            int size = users.Count;
            int[,] adjacencyMatrix = new int[users.Count, users.Count];
            Array.Clear(adjacencyMatrix, 0, adjacencyMatrix.Length);

            for (int i = 0; i < users.Count; i++) {
                for (int j = 0; j < users[i].GetFriends().Count; j++) {
                    for (int k = 0; k < users.Count; k++) {
                        if (users[i].GetFriends()[j] == users[k].GetName()) {
                            adjacencyMatrix[i,k] = 1;
                        }
                    }
                }
            }

            return adjacencyMatrix;
        }

        static List<int> FindUserClique(int[,] adjacencyMatrix, int initialUser) {

            List<int> tempInitialUser = new List<int>();
            tempInitialUser.Add(initialUser);
            
            List<int> tempClique = new List<int>();
            List<int> friendsOfFriend = new List<int>();
            List<int> friendships = new List<int>();
            List<List<int>> processedCliques = new List<List<int>>();
            List<List<int>> tempCliques = new List<List<int>>();
            List<List<int>> oldCliques = new List<List<int>>();
            List<List<int>> newCliques = new List<List<int>>();
            List<int> maxClique = new List<int>();
            Stack<List<int>> cliqueStack = new Stack<List<int>>();
            bool contained = false;
            cliqueStack.Push(tempInitialUser);

            //As long as there's more cliques
            while (cliqueStack.Count > 0) {
                //Get new clique from stack
                List<int> clique = new List<int>(cliqueStack.Pop());
                int lastInClique = clique.Last();

                //Find friendships for last person in clique
                friendships.Clear();

                for (int i = 0; i <= adjacencyMatrix.GetUpperBound(0); i++) {
                    if (adjacencyMatrix[lastInClique, i] == 1) {
                        friendships.Add(i);
                    }
                }

                //For each friendship, find all friends (friends of friends)
                foreach (int friendship in friendships) {
                    friendsOfFriend.Clear();
                    for (int i = 0; i <= adjacencyMatrix.GetUpperBound(0); i++) {
                        if (adjacencyMatrix[friendship, i] == 1) {
                            friendsOfFriend.Add(i);
                        }
                    }

                    //Determine if friends of friends includes the clique. If so, they are part of the clique, and added to queue
                    if (ContainsAll(friendsOfFriend, clique)) {
                        tempClique = new List<int>(clique);
                        tempClique.Add(friendship);
                        contained = false;

                        foreach (List<int> x in processedCliques) {
                            if (listContainsList(tempClique, x)) {
                                contained = true;
                                break;
                            }
                        }
                        if (!contained) {
                            foreach (List<int> x in cliqueStack) {
                                if (listContainsList(tempClique, x)) {
                                    contained = true;
                                    break;
                                }
                            }
                        }

                        if (!contained) {
                            cliqueStack.Push(tempClique);
                        }
                    } 
                }
                processedCliques.Add(clique);
            }

            //Remove list subsets for duplication elimination
            tempCliques = new List<List<int>>(processedCliques);
            foreach (List<int> outerList in tempCliques) {
                foreach (List<int> innerList in tempCliques) {
                    if (!innerList.Equals(outerList)) {
                        if (ContainsAll(outerList, innerList)) {
                            processedCliques.Remove(innerList);
                        }
                    }
                }
            }


            //http://bit.ly/1wrNgD4
            //While cliques change, check if it's possible to add people to them,
            //given the criteria that everyone in the clique is still friends with over 50% of people in clique
            newCliques = new List<List<int>>(processedCliques);

            while (!oldCliques.SequenceEqual(newCliques)) {
                oldCliques = new List<List<int>>(newCliques);

                bool okToAdd = true;
                List<int> intersection = new List<int>();
                double result;

                foreach (List<int> clique in processedCliques) {

                    for (int i = 0; i <= adjacencyMatrix.GetUpperBound(0); i++) {
                        if (!clique.Contains(i)) {
                            okToAdd = true;
                            tempClique.Clear();
                            tempClique = new List<int>(clique);
                            tempClique.Add(i);

                            foreach (int person in clique) {
                                friendships.Clear();

                                for (int j = 0; j <= adjacencyMatrix.GetUpperBound(0); j++) {
                                    if (adjacencyMatrix[person, j] == 1) {
                                        friendships.Add(j);
                                    }
                                }

                                intersection = new List<int>(friendships.Intersect(tempClique).ToList());
                                result = (double)intersection.Count / (double)tempClique.Count;

                                if (result < 0.5) {
                                    okToAdd = false;
                                    break;
                                }
                            }
                            if (okToAdd) {
                                clique.Add(i);
                            }
                        }
                    }
                }
                newCliques = new List<List<int>>(processedCliques);
            }

           /* List<int> friendshipsInClique = new List<int>();
            List<List<int>> newProcessedCliques = new List<List<int>>();
            List<int> tempList = new List<int>();

            for (int i = 0; i < processedCliques.Count; i++) {
                for (int j = 0; j < adjacencyMatrix.GetUpperBound(0); j++) {
                    foreach (int person in processedCliques[i]) {
                        if (adjacencyMatrix[person, j] == 1) {
                            friendshipsInClique.Add(j);
                        }
                    }
                }

                for (int k = 0; k < processedCliques.Count; k++) {
                    if (!listContainsList(processedCliques[k], processedCliques[i]) && !listContainsList(processedCliques[i], processedCliques[k])) {

                        List<int> intersection = processedCliques[k].Intersect(friendshipsInClique).ToList();

                        if (processedCliques[i].Count > processedCliques[k].Count && intersection.Count != 0) {
                            double result = intersection.Count / (double)((processedCliques[k].Count * processedCliques[i].Count) - processedCliques[k].Count);
                            if (result > 0.45) {
                                tempList = new List<int>(processedCliques[k].Union(processedCliques[i]).ToList());
                                newProcessedCliques.Add(tempList);
                            }
                        } else if(intersection.Count != 0) {
                            double result = intersection.Count / (double)((processedCliques[i].Count * processedCliques[k].Count) - processedCliques[i].Count);
                            if (result > 0.45) {
                                tempList = new List<int>(processedCliques[k].Union(processedCliques[i]).ToList());
                                newProcessedCliques.Add(tempList);
                        } else { 
                            }

                        }
                    }
                }
            }

            processedCliques = processedCliques.Union(newProcessedCliques).ToList();*/


            foreach (List<int> list in processedCliques) {
                if(list.Count > maxClique.Count) {
                    maxClique = list;
                }
            }
            foreach (int i in maxClique) {
                Console.WriteLine(i);
            }

            Console.WriteLine("-----End-----");

            return maxClique;
        }

        static bool listContainsList(List<int> a, List<int> b) {
            bool contained = true;

            foreach (int element in a) {
                if (!b.Contains(element)) {
                    contained = false;
                }
            }
            return contained;
        }


        //Determines if all elements of list b is contained in a (A can have more elements still)
        static bool ContainsAll(List<int> a, List<int> b) {
            return !b.Except(a).Any();
        }
    }
}

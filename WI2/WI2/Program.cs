using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace WI2 {
    class Program {
        static void Main(string[] args) {
            string dir = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\Semester7\Web Intelligence\Handin1\Crawler\WI2\friendships.txt";
            List<User> users = GetData(dir);
            int[,] adjacencyMatrix = MakeAdjacencyMatrix(users);

            for (int i = 0; i < adjacencyMatrix.GetUpperBound(1); i++) {
                FindUserClique(adjacencyMatrix, i);
            }
            PrintUsers(users);
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
            List<int> maxClique = new List<int>();
            Stack<List<int>> cliqueStack = new Stack<List<int>>();
            cliqueStack.Push(tempInitialUser);

            while (cliqueStack.Count > 0) {
                //Get new clique from queue
                List<int> clique = new List<int>(cliqueStack.Pop());
                int lastInClique = clique.Last();

                //Find friendships for last in clique
                friendships.Clear();

                for (int i = 0; i < adjacencyMatrix.GetUpperBound(0); i++) {
                    if (adjacencyMatrix[lastInClique, i] == 1) {
                        friendships.Add(i);
                    }
                }

                //For each friendship, determine if seen before. If not, add to processed
                foreach (int friendship in friendships) {
                    friendsOfFriend.Clear();
                    for (int i = 0; i <= adjacencyMatrix.GetUpperBound(0); i++) {
                        if (adjacencyMatrix[i, friendship] == 1) {
                            friendsOfFriend.Add(i);
                        }
                    }

                    if (ContainsAllItems(friendsOfFriend, clique)) {
                        clique.Add(friendship);

                        if (!processedCliques.Contains(clique)) {
                            cliqueStack.Push(clique);
                        } 
                    } 
                }
                processedCliques.Add(clique);
            }

            foreach (List<int> list in processedCliques) {
                if(list.Count > maxClique.Count) {
                    maxClique = list;
                }
            }
            foreach (int i in maxClique) {
                Console.WriteLine(i);
            }

            Console.WriteLine("-----End-----");
            Console.ReadLine();

            return maxClique;
        }

        static bool ContainsAllItems(List<int> a, List<int> b) {
            return !b.Except(a).Any();
        }
    }
}

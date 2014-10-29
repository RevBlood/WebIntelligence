using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace WI2 {
    class Program {
        static void Main(string[] args) {
            string dir = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\Semester7\Web Intelligence\WI2\friendships.txt";
            List<User> users = GetData(dir);
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

                    string twoNames = names[i] + " " + names[i + 1];

                    foreach (User u in users) {
                        if (u.GetName() == twoNames) {
                            replaceNames.Add(twoNames);
                            isTwoNames = true;
                        }
                    }

                    if (!isTwoNames) {
                        replaceNames.Add(names[i]);
                    } else {
                        isTwoNames = false;
                    }
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
    }
}

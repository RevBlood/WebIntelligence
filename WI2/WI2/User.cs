using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WI2 {
    class User {

        string Name;
        List<string> Friends = new List<string>();
        List<string> Summary;
        List<string> Review;
        double ReviewScore;

        public User(string name, List<string> friends, List<string> summary, List<string> review) {
            this.Name = name;
            this.Friends = friends;
            this.Summary = summary;
            this.Review = review;
        }

        public string GetName() {
            return Name;
        }

        public List<string> GetFriends() {
            return Friends;
        }

        public void RemoveFriends(int i) {
            Friends.RemoveRange(i, 1);
        }

        public List<string> GetSummary() {
            return Summary;
        }

        public List<string> GetReview() {
            return Review;
        }

        public void SetScore(double score) {
            this.ReviewScore = score;
        }

        public double GetScore() {
            return this.ReviewScore;
        }
    }
}

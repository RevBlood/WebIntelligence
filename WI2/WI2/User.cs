using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WI2 {
    class User {

        string Name;
        List<string> Friends = new List<string>();
        string Summary;
        string Review;

        public User(string name, List<string> friends, string summary, string review) {
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

        public string GetSummary() {
            return Summary;
        }

        public string GetReview() {
            return Review;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WI2 {
    class ReviewData {

        private string ProductId;
        private string UserId;
        private string ProfileName;
        private double Helpful;
        private double NotHelpful;
        private double Score;
        private int Time;
        private List<string> Summary = new List<string>();
        private List<string> ReviewText = new List<string>();

        public ReviewData(string productId, string userId, string profileName, double helpful, double notHelpful, double score, int time, List<string> summary, List<string> reviewText) {
            this.ProductId = productId;
            this.UserId = userId;
            this.ProfileName = profileName;
            this.Helpful = helpful;
            this.NotHelpful = notHelpful;
            this.Score = score;
            this.Time = time;
            this.Summary = summary;
            this.ReviewText = reviewText;
        }

        public ReviewData(string userId, double score, List<string> summary, List<string> reviewText) {

            this.UserId = userId;
            this.Score = score;
            this.Summary = summary;
            this.ReviewText = reviewText;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace WI2 {
    class SentimentAnalyzer {

        private List<ReviewData> Reviews = new List<ReviewData>();
        private string Dir;

        public SentimentAnalyzer(string dir) {
            this.Dir = dir;
        }

        public void LoadReviewsFull() {

            List<string> lines = new List<string>();
            List<string> tempList = new List<string>();

            string productId;
            string userId;
            string profileName;
            double helpful;
            double notHelpful;
            double score;
            int time;
            List<string> summary = new List<string>();
            List<string> reviewText = new List<string>();

            using (StreamReader sr = new StreamReader(this.Dir)) {
                string line;
                while ((line = sr.ReadLine()) != null) {
                    lines.Add(line);
                }
            }
            for (int i = 0; i < lines.Count; i += 9) {
                productId = lines[i].Split(null).ToList()[1];
                userId = lines[i + 1].Split(null).ToList()[1];
                profileName = lines[i + 2].Split(null).ToList()[1];
                helpful = Int32.Parse(lines[i + 3].Split(null).ToList()[1].Split('/').ToList()[0]);
                notHelpful = Int32.Parse(lines[i + 3].Split(null).ToList()[1].Split('/').ToList()[1]);
                score = Double.Parse(lines[i + 4].Split(null).ToList()[1].Replace('.', ','));
                time = Int32.Parse(lines[i + 5].Split(null).ToList()[1]);
                summary = lines[i + 6].Split(null).ToList();
                summary.RemoveAt(0);
                reviewText = lines[i + 7].Split(null).ToList();
                reviewText.RemoveAt(0);

                this.Reviews.Add(new ReviewData(productId, userId, profileName, helpful, notHelpful, score, time, summary, reviewText));
            }
        }

        public void LoadReviewsSlim() {

            List<string> lines = new List<string>();
            List<string> tempList = new List<string>();

            string userId;
            double score;
            List<string> summary = new List<string>();
            List<string> reviewText = new List<string>();

            using (StreamReader sr = new StreamReader(this.Dir)) {
                string line;
                while ((line = sr.ReadLine()) != null) {
                    lines.Add(line);
                }
            }
            for (int i = 0; i < lines.Count; i += 9) {
                userId = lines[i + 1].Split(null).ToList()[1];
                score = Double.Parse(lines[i + 4].Split(null).ToList()[1].Replace('.', ','));
                summary = lines[i + 6].Split(null).ToList();
                summary.RemoveAt(0);
                reviewText = lines[i + 7].Split(null).ToList();
                reviewText.RemoveAt(0);

                this.Reviews.Add(new ReviewData(userId, score, summary, reviewText));
            }
        }

        public void LoadReviewsQuickAndDirty() {

            List<string> lines = new List<string>();
            List<string> tempList = new List<string>();

            string userId;
            double score;
            List<string> summary = new List<string>();
            List<string> reviewText = new List<string>();

            using (StreamReader sr = new StreamReader(this.Dir)) {
                string line;
                while ((line = sr.ReadLine()) != null) {
                    lines.Add(line);

                    //If we have 500 reviews from the file, stop. We don't want more.
                    if (lines.Count == 4500) {
                        break;
                    }
                }
            }
            for (int i = 0; i < lines.Count; i += 9) {
                userId = lines[i + 1].Split(null).ToList()[1];
                score = Double.Parse(lines[i + 4].Split(null).ToList()[1].Replace('.', ','));
                summary = lines[i + 6].Split(null).ToList();
                summary.RemoveAt(0);
                reviewText = lines[i + 7].Split(null).ToList();
                reviewText.RemoveAt(0);

                this.Reviews.Add(new ReviewData(userId, score, summary, reviewText));
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;

namespace WI2 {
    class SentimentAnalyzer {

        private List<ReviewData> Reviews = new List<ReviewData>();
        private List<ReviewData> TestReviews = new List<ReviewData>();
        private string Dir;
        string regexNeg = @"(?:^(?:never|no|nothing|nowhere|noone|none|not|havent|hasnt|hadnt|cant|couldnt|shouldnt|wont|wouldnt|dont|doesnt|didnt|isnt|arent|aint)$)|n't";
        string regexEndSentence = @"^[.:;!?]$";

        public SentimentAnalyzer(string dir) {
            this.Dir = dir;
        }

        public ReviewData GetRandomReview() {
            return TestReviews[0];
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
                    if (lines.Count == 90000) {
                        break;
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

                while ((line = sr.ReadLine()) != null) {
                    lines.Add(line);

                    //If we have 500 reviews from the file, stop. We don't want more.
                    if (lines.Count == 90018) {
                        break;
                    }
                }
            }

            for (int i = 4500; i < lines.Count; i += 9) {
                userId = lines[i + 1].Split(null).ToList()[1];
                score = Double.Parse(lines[i + 4].Split(null).ToList()[1].Replace('.', ','));
                summary = lines[i + 6].Split(null).ToList();
                summary.RemoveAt(0);
                reviewText = lines[i + 7].Split(null).ToList();
                reviewText.RemoveAt(0);

                this.TestReviews.Add(new ReviewData(userId, score, summary, reviewText));
            }
        }

        public void LoadEqualReviewAmounts(){

            List<string> lines = new List<string>();
            string userId;
            double score;
            List<string> summary = new List<string>();
            List<string> reviewText = new List<string>();

            List<ReviewData> one = new List<ReviewData>();
            List<ReviewData> two = new List<ReviewData>();
            List<ReviewData> three = new List<ReviewData>();
            List<ReviewData> four = new List<ReviewData>();
            List<ReviewData> five = new List<ReviewData>();

            using (StreamReader sr = new StreamReader(this.Dir)) {
                string line;
                while ((line = sr.ReadLine()) != null) {
                    lines.Add(line);

                    if (lines.Count == 90000) {
                        break;
                    }
                }
                for (int i = 0; i < lines.Count; i += 9) {
                    userId = lines[i + 1].Split(null).ToList()[1];
                    score = Double.Parse(lines[i + 4].Split(null).ToList()[1].Replace('.', ','));
                    summary = lines[i + 6].Split(null).ToList();
                    summary.RemoveAt(0);
                    reviewText = lines[i + 7].Split(null).ToList();
                    reviewText.RemoveAt(0);

                    switch (score.ToString()) {
                        case "1":
                            if (one.Count < 500) {
                                one.Add(new ReviewData(userId, score, summary, reviewText));
                            }
                            break;
                        case "2":
                            if (two.Count < 500) {
                                two.Add(new ReviewData(userId, score, summary, reviewText));
                            }
                            break;
                        case "3":
                            if (three.Count < 500) {
                                three.Add(new ReviewData(userId, score, summary, reviewText));
                            }
                            break;
                        case "4":
                            if (four.Count < 500) {
                                four.Add(new ReviewData(userId, score, summary, reviewText));
                            }
                            break;
                        case "5":
                            if (five.Count < 500) {
                                five.Add(new ReviewData(userId, score, summary, reviewText));
                            }
                            break;
                    }
                }

                while ((line = sr.ReadLine()) != null) {
                    lines.Add(line);

                    //If we have 500 reviews from the file, stop. We don't want more.
                    if (lines.Count == 90018) {
                        break;
                    }
                }
            }

            for (int i = 4500; i < lines.Count; i += 9) {
                userId = lines[i + 1].Split(null).ToList()[1];
                score = Double.Parse(lines[i + 4].Split(null).ToList()[1].Replace('.', ','));
                summary = lines[i + 6].Split(null).ToList();
                summary.RemoveAt(0);
                reviewText = lines[i + 7].Split(null).ToList();
                reviewText.RemoveAt(0);

                this.TestReviews.Add(new ReviewData(userId, score, summary, reviewText));
            }

            this.Reviews.AddRange(one);
            this.Reviews.AddRange(two);
            this.Reviews.AddRange(three);
            this.Reviews.AddRange(four);
            this.Reviews.AddRange(five);
        }

        public void FindTypes() {
            List<string> regex = new List<string>();
            List<int> reviewTextAsTypes = new List<int>();

            // 1: Phone Numbers
            regex.Add(@"(?:(?:\+?[01][\-\s.]*)?(?:[\(]?\d{3}[\-\s.\)]*)?\d{3}[\-\s.]*\d{4})");

            // 2: Emoticons
            regex.Add(@"(?:[<>]?[:;=8][\-o\*\']?[\)\]\(\[dDpP/\:\}\{@\|\\]|[\)\]\(\[dDpP/\:\}\{@\|\\][\-o\*\']?[:;=8][<>]?)");

            // 3: HTML Tags
            regex.Add(@"<[^>]+>");

            // 4: Twitter Usernames
            regex.Add(@"(?:@[\w_]+)");

            // 5: Hashtags
            regex.Add(@"(?:\#+[\w_]+[\w\'_\-]*[\w_]+)");

            // 6: Everything else: Words with apostrophes or dashes | numbers, including fractions, decimals | words without apostrophes or dashes | ellipsis dots | everything else that isn't whitespace.
            regex.Add(@"(?:[a-z][a-z'\-_]+[a-z])|(?:[+\-]?\d+[,/.:-]\d+[+\-]?)|(?:[\w_]+)|(?:\.(?:\s*\.){1,})|(?:\S)");

            foreach (ReviewData review in Reviews) {
                foreach (string word in review.GetReviewText()) {
                    for (int i = 0; i < regex.Count; i++) {
                        if (Regex.IsMatch(word, regex[i])) {
                            reviewTextAsTypes.Add(i);
                            break;
                        }
                    }
                }
                review.SetReviewTextAsTypes(reviewTextAsTypes);
                reviewTextAsTypes.Clear();
            }
        }

        Dictionary<int, int> n_c = new Dictionary<int, int>();
        Dictionary<int, double> p_c = new Dictionary<int, double>();
        public void learnModel() {
            int N = Reviews.Count();
            create_p_c_and_n_c(N);
            findWordSentimentWeight();
            findWordProbability();

        }
        int counter = 0;
        public void create_p_c_and_n_c(int N) {
            for (int i = 1; i < 6; i++) {
                n_c.Add(i, 0);
            }
            foreach (ReviewData review in Reviews) {
                switch (review.getReviewScore().ToString()) {
                    case "1":
                        n_c[1] += 1;
                        break;
                    case "2":
                        n_c[2] += 1;
                        break;
                    case "3":
                        n_c[3] += 1;
                        break;
                    case "4":
                        n_c[4] += 1;
                        break;
                    case "5":
                        n_c[5] += 1;
                        break;
                    default:
                        throw new ArgumentException("no bucket, sad day");
                        break;
                }
            }

            for (int i = 1; i < 6; i++) {
                p_c.Add(i, (double)n_c[i] / (double)N);
            }
        }

        Dictionary<Tuple<string, double>, double> weightedWords = new Dictionary<Tuple<string, double>, double>();
        public void findWordSentimentWeight() {

            bool inMatch = false;

            foreach (ReviewData review in Reviews) {
                foreach (string word in review.GetReviewText()) {

                    if (Regex.IsMatch(word, regexNeg) && !inMatch) {
                        inMatch = true;
                    } else if (Regex.IsMatch(word, regexNeg) && inMatch) {
                        inMatch = false;
                    }

                    if (Regex.IsMatch(word, regexEndSentence)) {
                        inMatch = false;
                    }

                    Tuple<string, double> curKey = new Tuple<string, double>(word, review.getReviewScore());
                    if (inMatch) {
                        curKey = new Tuple<string, double>("NEG_" + word, review.getReviewScore());
                    }
                    
                    if (!weightedWords.ContainsKey(curKey)) {
                        weightedWords.Add(curKey, 1);
                    } else {
                        weightedWords[curKey]++;
                    }
                }
            }
        }

        Dictionary<Tuple<string, double>, double> wordProbability = new Dictionary<Tuple<string, double>, double>();
        public void findWordProbability() {
            foreach (Tuple<string, double> weightedWord in weightedWords.Keys) {
                if (!wordProbability.ContainsKey(weightedWord)) {
                    int test = (int)weightedWord.Item2;
                    wordProbability.Add(weightedWord, weightedWords[weightedWord] / n_c[(int)weightedWord.Item2]);
                } else {
                    throw new ArgumentException("oh my god this is some weird shit");
                }
            }
        }

        public void predictionModel(User user) {

            Dictionary<int, double> predictions = new Dictionary<int, double>();
            bool inMatch = false;
            string word;

            foreach (string workWord in user.GetReview()) {
                word = workWord;

                if (Regex.IsMatch(word, regexNeg) && !inMatch) {
                    inMatch = true;
                } else if (Regex.IsMatch(word, regexNeg) && inMatch) {
                    inMatch = false;
                }

                if (Regex.IsMatch(word, regexEndSentence)) {
                    inMatch = false;
                }

                if (inMatch) {
                    word = "NEG_" + word;
                }


                for (int i = 1; i < 6; i++) {
                    if (wordProbability.ContainsKey(new Tuple<string, double>(word, i))) {
                        if (!predictions.ContainsKey(i)) {
                            predictions.Add(i, wordProbability[new Tuple<string, double>(word, i)] * p_c[i]);
                        } else {

                            double a = wordProbability[new Tuple<string, double>(word, i)] * p_c[i];
                            double b = predictions[i] + a;
                            predictions[i] = b;
                        }
                    }
                }
            }

            double predictionSum = 0;
            double result = 0;

            for (int i = 1; i < 6; i++) {
                if (predictions.ContainsKey(i)) {
                    predictionSum += predictions[i];
                    result += i * predictions[i];
                }
            }
            // RUN IT now plz
            double realresultAMOK = result / predictionSum;
            user.SetScore(realresultAMOK);

            //MINDRE AMOK....

            /*if (predictionSum / 5 >= 3) {
                reviewGrade = true;
            } else {
                reviewGrade = false;
            }*/
            //Givet x,
            // udregn:
            // score(x, 1)
            // score(x, 2)
            // score(x, 3)
            // score(x, 4)
            // score(x, 5)
            //return scores.max(); as prediction 
            //   POSITIVE ELER NEGATIVE REVIEW
        }

    }
}
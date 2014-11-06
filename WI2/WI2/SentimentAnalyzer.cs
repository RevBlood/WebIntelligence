﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;

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

            foreach(ReviewData review in Reviews) {
                foreach(string word in review.GetReviewText()) {
                    for(int i = 0; i < regex.Count; i++) {
                        if(Regex.IsMatch(word, regex[i])) {
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
            foreach (ReviewData review in Reviews) {
                foreach (string word in review.GetReviewText()) {

                    Tuple<string, double> curKey = new Tuple<string, double>(word, review.getReviewScore());

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
            foreach(Tuple<string, double> weightedWord in weightedWords.Keys) {
                if (!wordProbability.ContainsKey(weightedWord)) {
                    int test = (int)weightedWord.Item2;
                    wordProbability.Add(weightedWord, weightedWords[weightedWord] / n_c[(int)weightedWord.Item2]);
                } else {
                    throw new ArgumentException("oh my god this is some weird shit");
                }
            }
        }

    }
}
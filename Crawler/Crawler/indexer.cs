using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;


namespace Crawler {
    class indexer {
        List<string> wordsNoDuplicates = new List<string>();
        List<int> timesSeen = new List<int>();
        string mydocpath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\pagefolder\";
        public Dictionary<string, LinkedList<int>> invertedIndex = new Dictionary<string, LinkedList<int>>();

        public indexer() {


            StringBuilder sb = new StringBuilder();
            List<string> myStrings = new List<string>();
            string[] dirs = Directory.GetFiles(mydocpath);

            foreach (string dir in dirs) {
                Console.WriteLine(dir);
                if (dir.ToString() == "data.txt") {

                } else {
                    using (StreamReader sr = new StreamReader(dir)) {
                        String line;
                        // Read and display lines from the file until the end of 
                        // the file is reached.
                        while ((line = sr.ReadLine()) != null) {
                            sb.AppendLine(line);
                        }
                    }
                    string allines = sb.ToString();
                    myStrings.Add(allines);
                    sb.Clear();
                }
            }


            List<string[]> result = new List<string[]>();

            foreach (string content in myStrings) {
                char[] delimiters = new char[] { ' ' };
                string[] res = content.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < res.Count(); i++) {
                    res[i] = Regex.Replace(res[i], @"^\s+$[\r\n]*", "", RegexOptions.Multiline);
                    res[i] = Regex.Replace(res[i], @"\t|\n|\r", "");
                }


                result.Add(res);
            }
            StreamReader counter = new StreamReader(mydocpath + "data.txt");
            int urlCounter = Convert.ToInt32(counter.ReadLine());

            List<string> cleaningBlanks = new List<string>();
            foreach (string[] stringArray in result) {
                cleaningBlanks.Clear();

                foreach (string word in stringArray) {
                    if (word != "") {
                        cleaningBlanks.Add(word);
                        invertedIndexMethod(word, urlCounter);
                    }
                }
                indexFreqTable(cleaningBlanks);
                Console.WriteLine(urlCounter);
                using (StreamWriter outfile = new StreamWriter(mydocpath + "freqTable" + urlCounter + ".txt")) {
                    
                    for (int i = 0; i < wordsNoDuplicates.Count(); i++) {
                        outfile.WriteLine(wordsNoDuplicates.ElementAt(i) + " " + timesSeen.ElementAt(i));

                    }
                    urlCounter++;
                    wordsNoDuplicates.Clear();
                    timesSeen.Clear();
                    outfile.Close();
                }
            }

            saveInvertedIndex();
        }


        public void invertedIndexMethod(string word, int docId) {
            if(invertedIndex.ContainsKey(word)) {
                if (!(invertedIndex[word].Last() == docId)) {
                    invertedIndex[word].AddLast(docId);
                }
            } else {
            invertedIndex.Add(word, new LinkedList<int>());
            invertedIndex[word].AddLast(docId);
            }
        }


        public void indexFreqTable(List<string> input) {

            input.RemoveAt(0);
            int index;

            foreach (string word in input) {
                if (wordsNoDuplicates.Contains(word)) {
                    index = wordsNoDuplicates.IndexOf(word);
                    timesSeen.Insert(index, timesSeen.ElementAt(index) + 1);
                } else {
                    wordsNoDuplicates.Add(word);
                    timesSeen.Add(1);
                }

            }
        }

        public void saveInvertedIndex() {
            using (StreamWriter outfile = new StreamWriter(mydocpath + "invertedIndex.txt")) {
                foreach (KeyValuePair<string, LinkedList<int>> res in invertedIndex) {
                    string lldocIds = "";
                    foreach(int i in res.Value) {
                        lldocIds += i.ToString() + ',';
                    }

                    outfile.WriteLine(res.Key + " " + lldocIds);
                }
            }
        }

        //read files
        //create/update dictionary
        //add word frequency
        public void saveIndex() {

            using (StreamWriter outfile = new StreamWriter(mydocpath + "result.txt")) {
                for (int i = 0; i < wordsNoDuplicates.Count(); i++) {
                    outfile.WriteLine(wordsNoDuplicates.ElementAt(i) + " " + timesSeen.ElementAt(i));

                }
                outfile.Close();
            }

        }

    }
}

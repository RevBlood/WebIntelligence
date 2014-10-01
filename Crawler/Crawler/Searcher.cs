using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace Crawler {
    class Searcher {

        List<String> searchTerms = new List<String>();
        List<String> invertedIndex = new List<String>();
        Dictionary<string, LinkedList<int>> documentswithTerms = new Dictionary<string, LinkedList<int>>();
        static string myPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\pagefolder\";
        string invertedIndexLocation = myPath + "invertedIndex.txt";


        public void startSearch() {

            clearVariables();

            //Get query from user
            Console.WriteLine("Please enter your query");
            //searchTerms = Console.ReadLine().Split(null).ToList();
            searchTerms.Add("Dr.");
            searchTerms.Add("ShowBe");
            findDocumentsWithTerms();
            idfMethod();
            printtf_IdfResult();
            
        }

        private void clearVariables() {

            searchTerms = new List<String>();
            invertedIndex = new List<String>();
            documentswithTerms = new Dictionary<string, LinkedList<int>>();
        }

        private void printtf_IdfResult() {
            foreach (KeyValuePair<string, List<Tuple<int, double>>> termBased in tf_idfkWeighting) {
                foreach (Tuple<int, double> tfIdfRes in termBased.Value) {
                    Console.WriteLine("Term: " + termBased.Key + " docId: " + tfIdfRes.Item1.ToString() + " tfIdf_Value: " + tfIdfRes.Item2.ToString());
                }
            }
        }
        
        private void printDick() {
            foreach (KeyValuePair<string, LinkedList<int>> kv in documentswithTerms) {
                foreach (int i in kv.Value) {
                    Console.WriteLine(kv.Key + " has value: " + i.ToString());
                }
            }
            Console.WriteLine("test, what was found????");
        }

        private void findDocumentsWithTerms() {

            //Fill up dictionary with all searchTerms. Values are blank so far
            foreach (string searchTerm in searchTerms) {
                documentswithTerms.Add(searchTerm, new LinkedList<int>());
            }

            //Open the inverted index
            using (StreamReader sr = new StreamReader(invertedIndexLocation)) {

                // Read and display lines from the file until the end of the file is reached.
                string line;
                while ((line = sr.ReadLine()) != null) {
                    invertedIndex.Add(line);
                }
            }
            //Go through the invertedIndex and compare each searchTerm to all terms found in the invertedIndex
            List<string> docIds = new List<string>();

            foreach (string indexTerm in invertedIndex) {
                foreach (string searchTerm in searchTerms) {
                    //When a match is found, extract all documents containing the match and fill them into dictionary as values for the terms
                    if (indexTerm.Substring(0, indexTerm.IndexOf(' ')) == searchTerm) {
                        int test = indexTerm.IndexOf(' ');

                        docIds = indexTerm.Substring(test + 1, indexTerm.Length - (test + 1)).Split(',').ToList();

                        foreach (string docId in docIds) {
                            Console.WriteLine(docId);
                            if (docId != "" && docId != null) {
                                documentswithTerms[searchTerm].AddLast(Convert.ToInt32(docId));
                            }
                        }

                    }
                }
            }
            printDick();
        }


        Dictionary<string, List<Tuple<int, double>>> tfWeighting = new Dictionary<string, List<Tuple<int, double>>>();
        Dictionary<string, double> idfWeighting = new Dictionary<string, double>();
        List<string> resultBuilding = new List<string>();

        Dictionary<string, List<Tuple<int, double>>> tf_idfkWeighting = new Dictionary<string, List<Tuple<int, double>>>();

        private void idfMethod() {
            int documentCount = countDocuments();
            foreach (KeyValuePair<string, LinkedList<int>> term in documentswithTerms) {
                
                
                tfWeighting.Add(term.Key, new List<Tuple<int, double>>());
                foreach (int docId in term.Value) {
                    string mydocpath = myPath + @"freqTable" + docId + ".txt";
                    resultBuilding.Clear();
                    using (StreamReader sr = new StreamReader(mydocpath)) {
                        string line;
                        while ((line = sr.ReadLine()) != null) {
                            resultBuilding.Add(line);
                        }
                    }

                    foreach (string resultBuildingString in resultBuilding) {
                        if (resultBuildingString.Substring(0, resultBuildingString.IndexOf(' ')) == term.Key) {
                            int hits = Convert.ToInt32(resultBuildingString.Substring(resultBuildingString.IndexOf(' ')));
                            double tf = 1 + Math.Log(hits,10);
                            
                            tfWeighting[term.Key].Add(new Tuple<int,double>(docId, tf));
                        }
                    }
                }

                // ANTAL DOKUMENTER documentCount N
                // ANTAL DOKUMENT-HITS           df
                Console.WriteLine(documentswithTerms[term.Key].Count());
                Console.ReadLine();

                double tempResult = (double)documentCount / (double)documentswithTerms[term.Key].Count();

                double idfResult = (double)Math.Log(tempResult,10);
                idfWeighting.Add(term.Key, idfResult);
            }


            foreach(KeyValuePair<string, double> idfVal in idfWeighting) {

                foreach (KeyValuePair<string, List<Tuple<int, double>>> tfRow in tfWeighting) {
                    if (idfVal.Key == tfRow.Key) {
                        tf_idfkWeighting.Add(idfVal.Key, new List<Tuple<int, double>>());
                        foreach (Tuple<int, double> tfVal in tfRow.Value) {

                            double tf_idfVal = tfVal.Item2 * idfVal.Value;
                            tf_idfkWeighting[idfVal.Key].Add(new Tuple<int,double>(tfVal.Item1, tf_idfVal));
                        }
                    }
                }


            }



        }

        private int countDocuments() {
            int docCounter = -1;
            string[] dirs = Directory.GetFiles(myPath);
            foreach (string dir in dirs) {
                if(dir.Contains("freqTable")) {
                    docCounter++;
                }
            }


            return docCounter;
        }


    }
}

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
        string invertedIndexLocation = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\pagefolder\invertedIndex.txt";

        public void startSearch() {

            clearVariables();

            //Get query from user
            Console.WriteLine("Please enter your query");
            //searchTerms = Console.ReadLine().Split(null).ToList();
            searchTerms.Add("on");
            searchTerms.Add("ShowBe");
            findDocumentsWithTerms();
        }

        private void clearVariables() {

            searchTerms = new List<String>();
            invertedIndex = new List<String>();
            documentswithTerms = new Dictionary<string, LinkedList<int>>();
        }

        private void printDick() {
            /*
            List<int> results = documentswithTerms[searchTerms.First()].ToList();
            foreach (int res in results) {
                Console.WriteLine(res.ToString());
            }
             */
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

                        docIds = indexTerm.Substring(test+1, indexTerm.Length - (test+1)).Split(',').ToList();

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
    }
}

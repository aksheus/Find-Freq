using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;

namespace FrequentPatternMiner
{

    class FrequentPatternGen 
    {

        static void Main(string[] args)
        {
            //#################################################
            // checking input and taking it
            if (args.Length != 4)
            {
                Console.WriteLine("command line arguments are missing  : min_sup K input_file_path output_file_path");
                return;
            }
            if (!File.Exists(args[2]))
            {
                Console.WriteLine("Path to input file is not valid : FILE DOES NOT EXIST");
            }
            int min_sup = int.Parse(args[0]);
            int K = int.Parse(args[1]);
            string input_file = args[2];
            string output_file = args[3];
            int largest_transaction = -1;
            using (StreamReader sr = File.OpenText(input_file))
            {
                while (!sr.EndOfStream)
                {
                    List<string> items = new List<string>(sr.ReadLine().Split(new char[] { ' ' })); //get items seperately
                    if (items.Count > largest_transaction)
                    {
                        largest_transaction = items.Count;
                    }
                }
            }
            largest_transaction += 1;
            // checking input and taking it 
            //#######################################################
            //####################################################################
            // scan file get support of k item sets and store it
            Dictionary<string, int> itemsetSupportK = new Dictionary<string, int>();
            Dictionary<Tuple<int, int>, List<string>> DataBase = new Dictionary<Tuple<int, int>, List<string>>();
            Dictionary<int, int> theEnd = new Dictionary<int, int>();
            for (int z = 1; z < largest_transaction; z++) // 795 max length for given input file
            {
                theEnd[z] = 0;
            }
            Dictionary<string, int> levelTwoItems = new Dictionary<string, int>();
            // int array or tuple ? tuple is generic and shit we don't need that so int [] for now
            // Dictionary<string, int> itemCombinations = new Dictionary<string, int>();
            // NOTE : if needed later replace dictionary with hashtable slightly more
            // efficient but you have to handle TYPE SAFETY yourself !!!
     //       int totalNumberOfItemsets = 0;
            using (StreamReader sr = File.OpenText(input_file))
            {
                while (!sr.EndOfStream)
                {
                    List<string> items = new List<string>(sr.ReadLine().Split(new char[] { ' ' })); //get items seperately
                    items.Sort();
                    if (items.Count == 2)
                    {
                        
                        string temp = items[0] + " " + items[1];
                        if (!levelTwoItems.ContainsKey(temp))
                        {
                            levelTwoItems[temp] = 1;
                        }
                        else
                        {
                            levelTwoItems[temp] += 1;
                        } 
                    }

                    if (items.Count >= 3) // only interested in 3 and above for db
                    {
                        Tuple<int, int> access = new Tuple<int, int>(items.Count, theEnd[items.Count]);
                        DataBase[access] = items;
                        theEnd[items.Count] += 1;
                        // now we need combinations of size two
                        for(int a = 0; a < items.Count; a++)
                        {
                            for(int b = a+1; b < items.Count; b++)
                            {
                                string temp = items[a] + " " + items[b];
                                if (!levelTwoItems.ContainsKey(temp))
                                {
                                    levelTwoItems[temp] = 1;
                                }
                                else
                                {
                                    levelTwoItems[temp] += 1;
                                }
                            }
                        } 

                    }
                    for (int i = 0; i < items.Count; i++)
                    {
                        if (!itemsetSupportK.ContainsKey(items[i]))
                        {
                            itemsetSupportK[items[i]] = 1;
                        }
                        else
                        {
                            itemsetSupportK[items[i]] += 1;
                        }
                    }
          //          totalNumberOfItemsets += 1;

                }
            }
                
            HashSet<string> itemsToBeRemoved = new HashSet<string>();
            foreach (KeyValuePair<string, int> kvp in itemsetSupportK)
            {
                if (kvp.Value < min_sup)
                {
                    itemsToBeRemoved.Add(kvp.Key);
                }
            }
            foreach (string s in itemsToBeRemoved)
            {
                itemsetSupportK.Remove(s);
            }
            itemsToBeRemoved = null; // get it garbage collected asap
            HashSet<string> invalidLevelTwoPpl = new HashSet<string>();
            foreach (KeyValuePair<string, int> kvp in levelTwoItems)
            {
                if (kvp.Value < min_sup)
                {
                    invalidLevelTwoPpl.Add(kvp.Key);
                }
            }
            foreach (string w in invalidLevelTwoPpl)
            {
                levelTwoItems.Remove(w);
            }
            invalidLevelTwoPpl = null;
            //#######################################################################
            //#######################################################################
            // if k==1 then output frequent 1-itemset before entering main loop
            // main loop section , strategy while itemSuppork is not empty
            // generate candidates for next level
            // verify wether they are actually frequent
            // output them if they are K+ itemset
            // not sure how deletion of previous level will go
            if (K == 1)
            {
                using (StreamWriter sw = File.AppendText(output_file))
                {
                    foreach (KeyValuePair<string, int> kvp in itemsetSupportK)
                    {
                        sw.WriteLine(kvp.Key + " " + "(" + kvp.Value + ")");
                    }
                }

            }

         
             itemsetSupportK.Clear(); // ditch the prev level already outputted
            foreach(KeyValuePair<string,int> kvp in levelTwoItems)
            {
                itemsetSupportK[kvp.Key] = kvp.Value;
            }
            levelTwoItems.Clear(); // ditch this dictionary use itemSupport k from here
            levelTwoItems = null;
            if (K <= 2 )
            {
                using (StreamWriter sw = File.AppendText(output_file))
                {
                    foreach (KeyValuePair<string, int> kvp in itemsetSupportK)
                    {
                        sw.WriteLine(kvp.Key + " " + "(" + kvp.Value + ")");
                    }
                }

            }
            int whereAt = 3;
            while (itemsetSupportK.Count != 0) // the big bad loop :p
            {
                string[] candidateInput = new string[itemsetSupportK.Count];
                int index = 0;
                foreach (string s in itemsetSupportK.Keys)
                {
                    candidateInput[index] = s;
                    index += 1;
                }
                Array.Sort(candidateInput);
                HashSet<string> validCandidates = generateCandidates(candidateInput);
                itemsetSupportK.Clear(); // ditch the previous level
                foreach (string s in validCandidates)
                {
                    itemsetSupportK[s] = 0; // initialize counts to zero
                }
                for (int u = whereAt; u < largest_transaction; u++)
                {
                    for (int v = 0; v < theEnd[u]; v++)
                    {
                        Tuple<int, int> accessKey = new Tuple<int, int>(u, v);
                        if (DataBase.ContainsKey(accessKey))
                        {
                            List<string> fullSet = DataBase[accessKey];
                            foreach (string candidate in validCandidates)
                            {
                                List<string> subSet = new List<string>(candidate.Split(new char[] { ' ' }));
                                if (!subSet.Except(fullSet).Any())
                                {
                                    itemsetSupportK[candidate] += 1;
                                }

                            }
                        }
                    }
                }
                validCandidates = null;
                HashSet<string> invalidCandidates = new HashSet<string>();
                foreach (KeyValuePair<string, int> kvp in itemsetSupportK)
                {
                    if (kvp.Value < min_sup)
                    {
                        invalidCandidates.Add(kvp.Key);
                    }
                }
                foreach (string invalid in invalidCandidates)
                {
                    itemsetSupportK.Remove(invalid);
                }
                invalidCandidates = null;
                if (whereAt >= K)
                {
                    using (StreamWriter sw = File.AppendText(output_file))
                    {
                        foreach (KeyValuePair<string, int> kvp in itemsetSupportK)
                        {
                            sw.WriteLine(kvp.Key + " " + "(" + kvp.Value + ")");
                        }
                    }
                }
                whereAt++;
            }



        }

        private static HashSet<string> generateCandidates(string[] freqItemsAtPrevLevel)
        {
            // STRONG NOTE : it is a must to properly maintain THE SAME TOTAL ORDERING
            // else this shit will not work , test accordingly
            // NOTE:  assumes appropriate and sorted input 
            // generates nextValidCandidates for/at nextLevelOfFreqItems 
            HashSet<string> candidates = new HashSet<string>();
            
                // { "a b","b c", "a c" , "a d" } => { "a b c", "a b d", "a c d" }
                // idea for join step
                // itemsets which differ only in last item are worth joining 
                // joining done  by adding each others last item to each other
                for (int i = 0; i < freqItemsAtPrevLevel.Length; i++)
                {

                    string[] itemSet1 = freqItemsAtPrevLevel[i].Split(new char[] { ' ' });
                    for (int j = i + 1; j < freqItemsAtPrevLevel.Length; j++)
                    {
                        string[] itemSet2 = freqItemsAtPrevLevel[j].Split(new char[] { ' ' });

                        if (differOnlyInLastItem(itemSet1, itemSet2))
                        {
                            string addToCandidates = string.Empty;
                            for (int x = 0; x < itemSet1.Length; x++)
                            {
                                addToCandidates += itemSet1[x] + " ";
                            }

                            addToCandidates += itemSet2[itemSet1.Length - 1];
                            candidates.Add(addToCandidates);
                        }
                    }
                }
                // join step ends 
                // prune step begins
                // { "a b","b c", "a c" , "a d" } => { "a b c", "a b d", "a c d" }
                // { "a b c", "a b d","a c d" } => { "a b c"}
                // idea for prune step nlogn
                // we chop off "a b c" char and space at a time 
                // use binary search to check if combination is there
                HashSet<string> invalidCandidates = new HashSet<string>();
                foreach (string s in candidates)
                {
                    HashSet<string> combinations = getLowerLevelCombinations(s);
                    foreach (string searchThis in combinations)
                    {
                        if (Array.BinarySearch(freqItemsAtPrevLevel, searchThis) < 0)
                        {

                            invalidCandidates.Add(s);
                        }
                    }

                }
                foreach (string s in invalidCandidates)
                {
                    candidates.Remove(s);
                }
                invalidCandidates = null; //force garbage collection
                return candidates;
            
        
        }

        private static bool differOnlyInLastItem(string[] itemset1, string[] itemset2)
        {
            // their lengths will be same there is no doubt about it
            //  string[] itemset1 = s1.Split(new char[] {' '}); wiser to do these two steps
            //  string[] itemset2 = s2.Split(new char[] {' '}); outside and send them as args 
            // might move this function into calling code outside now for clean purposes
            if (string.Equals(itemset1[itemset1.Length - 1], itemset2[itemset1.Length - 1]))
            {
                return false;
            }
            else
            {
                for (int x = 0; x < itemset1.Length - 1; x++)
                {
                    if (!string.Equals(itemset1[x], itemset2[x]))
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        private static HashSet<string> getLowerLevelCombinations(string toBeChopped)
        {
            HashSet<string> choppedOff = new HashSet<string>();
            string[] splitWords = toBeChopped.Split(new char[] { ' ' });
            int x = 0, y = 0;
            while (x < toBeChopped.Length)
            {
                try
                {
                    StringBuilder sb = new StringBuilder(toBeChopped);
                    if (x != (toBeChopped.Length - splitWords[y].Length))
                    {
                        sb.Remove(x, splitWords[y].Length + 1);
                    }
                    else
                    {
                        sb.Remove(x - 1, splitWords[y].Length + 1);
                    }
                    choppedOff.Add(sb.ToString());
                    x += splitWords[y].Length + 1;
                    y += 1;
                }
                catch (OutOfMemoryException e)
                {
                    Console.WriteLine(e.StackTrace);

                }
            }
            splitWords = null; //force garbage collection
            return choppedOff;
        }


    }
}


# FrequentPatternIdentifier

Introduction:
Implements apriori algorithm efficiently and generates all K+ frequent itemsets . Where K is the size of an itemset . 

Requirements:
Input File :

> item1 item2 itemN

Each line should be a transaction with items delimited by a space 

Where each line is a transaction with space seperated items

Usage:

> C:\Users\YourUserName> csc Program.cs

> C:\Users\YourUserName>Program min_sup K path_to_input_file path_to_output_file

NOTE:  min_sup is the support value in frequency not percentage

Benchmark:

transactionDB.txt (27000+ transactions)
with min_sup = 3 and K = anything takes just over 2 minutes to compute  K+ frequent itemsets



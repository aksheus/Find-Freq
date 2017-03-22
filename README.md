# FrequentPatternIdentifier

Introduction:
Implements apriori algorithm efficiently and generates all K+ frequent itemsets . Where K is the size of an itemset . 

Requirements:
Input File :
transactionDB.txt (27000+ transactions)
(will be extended later to support different files )

Where each line is a transaction with space seperated items

Usage:

> C:\Users\YourUserName> csc Program.cs

> C:\Users\YourUserName>Program min_sup K path_to_input_file path_to_output_file

NOTE:  min_sup is the support value in frequency not percentage

Benchmark:

with min_sup = 3 and K = anything takes just over 2 minutes to compute  K+ frequent itemsets



# kmers

The sequence k-mer tools
> This R# package module provides the toolkit for make the k-mer based 
>  sequence data analysis:
>  
>  + ``kmers``: generate the k-mer sequence fragments from a given sequence 
>    data in a sliding window manner;
>  + ``kmers_matrix``: generate the k-mer count matrix of a given sequence 
>    collection;
>  + ``tfidf_vectorizer`` and ``onehot_vectorizer``: make the sequence 
>    embedding via the bag-of-k-mers model, the TF-IDF weight or the one-hot 
>    encoding vector;
>  + ``cdhit_nr`` and ``cdhit_clusters``: run the CD-HIT like sequence 
>    clustering for get the non-redundant sequence set or the cluster 
>    family table.

+ [kmers](kmers/kmers.1) Create kmers from a given sequence
+ [kmers_matrix](kmers/kmers_matrix.1) generate sequence k-mer count data matrix
+ [tfidf_vectorizer](kmers/tfidf_vectorizer.1) make the sequence embedding via the TF-IDF weight of the bag-of-k-mers 
+ [onehot_vectorizer](kmers/onehot_vectorizer.1) make the sequence embedding via the one-hot encoding(Bag-of-n-grams) of 
+ [cdhit_nr](kmers/cdhit_nr.1) run the CD-HIT like sequence clustering for get the non-redundant 
+ [cdhit_clusters](kmers/cdhit_clusters.1) run the CD-HIT like sequence clustering and then export the cluster 

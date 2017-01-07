#!/bin/bash

# The query proteins' fasta file path
query="path/to/query.fasta"
# The COG database
cog="path/to/prot2003-2014.fasta"
  
# The output directory
EXPORT="path/to/save"

makeblastdb -in $query -dbtype prot
makeblastdb -in $cog -dbtype prot

queryName=$(basename $query)
echo " ----> $queryName"

out=" ==> $EXPORT/$queryName._vs_COG.txt"
echo $out
blastp -query $query -db $cog -num_threads 20 -evalue 1e-5 -out $out &

out=" ==> $EXPORT/COG._vs_$queryName.txt"
echo $out
blastp -query $cog -db $query -num_threads 20 -evalue 1e-5 -out $out &
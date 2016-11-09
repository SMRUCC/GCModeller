## COG

### 1. Download database from NCBI

> FTP location: [ftp://ftp.ncbi.nlm.nih.gov/pub/COG/COG2014/data/](ftp://ftp.ncbi.nlm.nih.gov/pub/COG/COG2014/data/)
>
> + ftp://ftp.ncbi.nlm.nih.gov/pub/COG/COG2014/data/cog2003-2014.csv
> + ftp://ftp.ncbi.nlm.nih.gov/pub/COG/COG2014/data/cognames2003-2014.tab
> + ftp://ftp.ncbi.nlm.nih.gov/pub/COG/COG2014/data/fun2003-2014.tab
> + ftp://ftp.ncbi.nlm.nih.gov/pub/COG/COG2014/data/genomes2003-2014.tab
> + ftp://ftp.ncbi.nlm.nih.gov/pub/COG/COG2014/data/prot2003-2014.gi2gbk.tab
> + ftp://ftp.ncbi.nlm.nih.gov/pub/COG/COG2014/data/prot2003-2014.tab
> + ftp://ftp.ncbi.nlm.nih.gov/pub/COG/COG2014/data/prot2003-2014.fa.gz
> + ftp://ftp.ncbi.nlm.nih.gov/pub/COG/COG2014/data/Readme.201610.txt

### 2. BBH blastp

```bash
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
```

### 3. Using GCModeller for result exports

```bash
#!/bin/bash


```

### 4. Directly annotate COG using GCModeller tools

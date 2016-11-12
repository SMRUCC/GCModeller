## COG

###### Introduction

Rational classification of proteins encoded in sequenced genomes is critical for making the genome sequences maximally useful for functional and evolutionary studies. The database of Clusters of Orthologous Groups of proteins (COGs) is an attempt on a phylogenetic classification of the proteins encoded in 21 complete genomes of bacteria, archaea and eukaryotes (http://www.ncbi.nlm.nih.gov/COG ). The COGs were constructed by applying the criterion of consistency of genome-specific best hits to the results of an exhaustive comparison of all protein sequences from these genomes. The database comprises 2091 COGs that include 56–83% of the gene products from each of the complete bacterial and archaeal genomes and ~35% of those from the yeast Saccharomyces cerevisiae genome. The COG database is accompanied by the COGNITOR program that is used to fit new proteins into the COGs and can be applied to functional and phylogenetic annotation of newly sequenced genomes.

> The COG database: a tool for genome-scale analysis of protein functions and evolution
> [10.1093/nar/28.1.33](http://nar.oxfordjournals.org/content/28/1/33.full)

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

### 2. COG BBH manually

###### Run bbh blastp

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

###### Using GCModeller for result exports

Here is the example of how to construct the **bbh** result from the raw blastp output:

```bash
#!/bin/bash

# sbh1, sbh2 -> bbh
qvs="path/to/qvs.txt"
svg="path/to/svq.txt"

# sbh commands

# bbh commands
# bbh was generated from the qvs/svq sbh result.

```

###### COG catalog &amp; profiling



### 3. Directly annotate COG using GCModeller tools

###### Install COG database

First, using localblast tool, ``/install.cog2003-2014`` command for COG database install:
```
Help for command '/install.cog2003-2014':

  Information:  Config the ``prot2003-2014.fasta`` database for GCModeller localblast tools. This database
                will be using for the COG annotation.
                This command required of the blast+ install first.
  Usage:        G:\GCModeller\GCModeller\bin\localblast.exe /install.cog2003-2014 /db <prot2003-2014.fasta>
  Example:      localblast /install.cog2003-2014 /db /data/fasta/prot2003-2014.fasta

  Arguments:
  ============================

   /db    Description:  The fasta database using for COG annotation, which can be download
                        from NCBI ftp:
                        > ftp://ftp.ncbi.nlm.nih.gov/pub/COG/COG2014/data/prot2003-2014.fa.gz

          Example:      /db <file/directory>
```

For example:

```bash
localblast /install.cog2003-2014 /db /data/fasta/prot2003-2014.fasta
```

And then, using command ``/query.cog2003-2014`` for COG annotation automatically:

```bash
Help for command '/query.cog2003-2014':

  Information:  Protein COG annotation by using NCBI cog2003-2014.fasta database.
  Usage:        G:\GCModeller\GCModeller\bin\localblast.exe /query.cog2003-2014 /query <query.fasta> [/evalue 1e-5 /coverage 0.65 /identities 0.85 /all /out <out.DIR> /db <cog2003-2014.fasta> /blast+ <blast+/bin>]
  Example:      CLI usage example not found!

  Arguments:
  ============================

  [/db]       Description:  The file path to the database fasta file.
                            If you have config the cog2003-2014 database previously,
                            then this argument can be omitted.

              Example:      /db <file/directory>

  [/blast+]   Description:  The directory to the NCBI blast+ suite ``bin`` directory. If
                            you have config this path before, then this argument can be omitted.

              Example:      /blast+ <file/directory>

  [/all]      Description:  For export the bbh result, export all match or only the top best?
                            default is only top best.

              Example:      /all
                            (bool flag does not require of argument value)

  [/evalue]   Description:  blastp e-value cutoff.

              Example:      /evalue <float>

  [/out]      Description:  The output directory for the work files.

              Example:      /out <file/directory>
```

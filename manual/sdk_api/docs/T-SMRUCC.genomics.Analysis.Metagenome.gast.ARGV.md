---
title: ARGV
---

# ARGV
_namespace: [SMRUCC.genomics.Analysis.Metagenome.gast](N-SMRUCC.genomics.Analysis.Metagenome.gast.html)_

gast -in input_fasta -ref reference_uniques_fasta -rtax reference_dupes_taxonomy [-mp min_pct_id] [-m majority] -out output_file



### Methods

#### #ctor
```csharp
SMRUCC.genomics.Analysis.Metagenome.gast.ARGV.#ctor(Microsoft.VisualBasic.CommandLine.CommandLine)
```
```bash
 gast -in input_fasta -ref reference_uniques_fasta -rtax reference_dupes_taxonomy [-mp min_pct_id] [-m majority] -out output_file
 ```

|Parameter Name|Remarks|
|--------------|-------|
|args|-|



### Properties

#### db_host
-host, mysql server host name
#### db_name
-db, database name
#### full
-full, input data will be compared against full length 16S reference sequences [default: not full length]
#### in
input_fasta, input fasta file
#### m
[majority]
#### maj
-maj, percent majority required for taxonomic consensus [default: 66]
#### maxa
-maxa, [Optional] usearch --max_accepts parameter [default: 15]
#### maxr
-maxr, [Optional] usearch --max_rejects parameter [default: 200]
#### minp
-minp, [Optional] minimum percent identity match to a reference.
 If the best match Is less Then min_pct_id, it Is Not considered a match
 Default = 0.80
#### mp
[min_pct_id]
#### out
output_file, output filename
#### ref
reference_uniques_fasta, reference fasta file containing unique sequences of known taxonomy
 The definition line should include the ID used In the reference taxonomy file.
 Any other information On the definition line should be separated by a space Or a ``|`` symbol.
#### rtax
reference_dupes_taxonomy, reference taxa file with taxonomy for all copies of the sequences in 
 the reference fasta file 
 This Is a tab-delimited file, three columns, describing the taxonomy of the reference sequences
 The ID matching the reference fasta, the taxonomy And the number Of reference sequences With this 
 this same taxonomy.
#### table
-table, database table to receive data
#### terse
-terse minimal output, includes only ID, taxonomy, and distance
 See GAST manual For description Of other fields
#### udb
-udb, use a USearch formatted udb indexed version of the reference for speed. 
 (see http://drive5.com/usearch/manual/udb_files.html)
#### wdb
-wdb, use a USearch formatted wdb indexed version of the reference for speed. 
 [NO LONGER AVAILABLE with usearch6.0+]

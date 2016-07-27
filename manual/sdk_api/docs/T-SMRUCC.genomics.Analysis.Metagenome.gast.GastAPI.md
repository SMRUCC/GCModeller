---
title: GastAPI
---

# GastAPI
_namespace: [SMRUCC.genomics.Analysis.Metagenome.gast](N-SMRUCC.genomics.Analysis.Metagenome.gast.html)_

compares trimmed sequences against a reference database for assigning taxonomy, reads 
 a fasta file of trimmed 16S sequences, compares each sequence to a Set Of similarly 
 trimmed ( Or full-length ) 16S reference sequences And assigns taxonomy



### Methods

#### assign_taxonomy
```csharp
SMRUCC.genomics.Analysis.Metagenome.gast.GastAPI.assign_taxonomy(System.IO.StreamWriter,System.String,System.Collections.Generic.Dictionary{System.String,System.String[][]},System.Collections.Generic.Dictionary{System.String,System.String[]},System.Double,System.Boolean,System.String)
```
get dupes from the names file and calculate consensus taxonomy

|Parameter Name|Remarks|
|--------------|-------|
|names_file|-|
|results_ref|-|
|ref_taxa_ref|-|


#### Invoke
```csharp
SMRUCC.genomics.Analysis.Metagenome.gast.GastAPI.Invoke(Microsoft.VisualBasic.CommandLine.CommandLine)
```
```bash
 gast -in input_fasta -ref reference_uniques_fasta -rtax reference_dupes_taxonomy [-mp min_pct_id] [-m majority] -out output_file
 ```

|Parameter Name|Remarks|
|--------------|-------|
|args|-|


#### load_reftaxa
```csharp
SMRUCC.genomics.Analysis.Metagenome.gast.GastAPI.load_reftaxa(System.String)
```
Get dupes Of the reference sequences And their taxonomy

|Parameter Name|Remarks|
|--------------|-------|
|tax_file|-|


#### parse_uclust
```csharp
SMRUCC.genomics.Analysis.Metagenome.gast.GastAPI.parse_uclust(System.String,System.Boolean,System.Boolean,System.Boolean,System.Int32)
```
Parse the USearch results And grab the top hit

|Parameter Name|Remarks|
|--------------|-------|
|uc_file|-|




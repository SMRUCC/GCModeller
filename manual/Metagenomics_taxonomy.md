## Annotation of the taxonomy using GCModeller

### Database
+ NCBI nt
  ftp://ftp.ncbi.nih.gov/blast/db/FASTA/nt.gz
+ NCBI taxonomy
  ftp://ftp.ncbi.nih.gov/pub/taxonomy/
  + For legacy GI tools, download: ftp://ftp.ncbi.nih.gov/pub/taxonomy/gi_taxid_nucl.zip
  + For new tools without GI, download all of the files in directory: ftp://ftp.ncbi.nih.gov/pub/taxonomy/accession2taxid 
  + Download taxonomy data: ftp://ftp.ncbi.nih.gov/pub/taxonomy/taxdmp.zip
  
### Blastn query of NCBI nt

This is the standard blastn steps by using ncbi blast+ suite:

+ First, format ncbi nt fasta database by
```bash
makeblastdb -in "/path/to/nt" -dbtype nucl
```
+ Then, performance blastn query operation on your query fasta
```bash
blastn -query "/path/to/query" -db "/path/to/nt" -out "/path/to/output.txt" -evalue 1e-5 -num_threads <int/cpu_cores> ...[additionals]
```

### GCModeller tools

##### localblast
Then you can export the blastn query result by using command ``/Export.blastnMaps``

```bash
localblast ? /Export.blastnMaps

# Help for command '/Export.blastnMaps':
#
#   Information:
#   Usage:        /home/biostack/GCModeller/localblast /Export.blastnMaps /in <blastn.txt> [/best /out <out.csv>]
#   Example:      CLI usage example not found!
#
#   Arguments:
#   ============================
#
#   [/best]   Description:  Only output the first hit result for each query as best?
#
#             Example:      /best <term_string>
```

The parameter ``/best`` if is enable, then localblast tool will export **only the top best** hit of each query, if not, then all of the hits result will be export.

##### ncbi_tools 

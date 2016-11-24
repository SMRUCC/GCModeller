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

This is the standard blastn steps by using ncbi blast+ suite query your 16S/18S rRNA against nt database for the taxonomy annotations:

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

Once you have export the blastn query result, then you can export the **gi list** or **accession id list** for subset the x2taxid(``gi2taxid``, ``accessionid2taxid``) database:

```bash
localblast ? /ref.acc.list
# Help for command '/ref.acc.list':
#
#   Information:
#   Usage:        /home/biostack/GCModeller/localblast /ref.acc.list /in <blastnMaps.csv/DIR> [/out <out.csv>]
#   Example:      CLI usage example not found!

localblast ? /ref.gi.list
# Help for command '/ref.gi.list':
#
#   Information:
#   Usage:        /home/biostack/GCModeller/localblast /ref.gi.list /in <blastnMaps.csv/DIR> [/out <out.csv>]
#   Example:      CLI usage example not found!
```

##### ncbi_tools 

After you have the **gi list** or **accession id list**, then you can subset the ``gi2taxid`` or ``accessionid2taxid`` database for save your time by using command in ``ncbi_tools``:

```bash
ncbi_tools ? /gi.Match
# Help for command '/gi.Match':
#
#   Information:
#   Usage:        /home/biostack/GCModeller/NCBI_tools /gi.Match /in <nt.parts.fasta/list.txt> /gi2taxid <gi2taxid.dmp> [/out <gi_match.txt>]
#   Example:      CLI usage example not found!

ncbi_tools ? /accid2taxid.Match
# Help for command '/accid2taxid.Match':
#
#   Information:
#   Usage:        /home/biostack/GCModeller/NCBI_tools /accid2taxid.Match /in <nt.parts.fasta/list.txt> /acc2taxid <acc2taxid.dmp/DIR> [/gb_priority /out <acc2taxid_match.txt>]
#   Example:      CLI usage example not found!
```

At last, you have enough data for annotates the taxonomy details:
```bash
localblast ? /Blastn.Maps.Taxid
# Help for command '/Blastn.Maps.Taxid':
#
#   Information:
#   Usage:        /home/biostack/GCModeller/localblast /Blastn.Maps.Taxid /in <blastnMapping.csv> /2taxid <acc2taxid.tsv/gi2taxid.dmp> [/gi2taxid /trim /tax <NCBI_taxonomy:nodes/names> /out <out.csv>]
#   Example:      CLI usage example not found!
#
#   Arguments:
#   ============================
#
#   [/gi2taxid]   Description:  The 2taxid data source is comes from gi2taxid, by default is
#                               acc2taxid.
#
#                 Example:      /gi2taxid <term_string>
```

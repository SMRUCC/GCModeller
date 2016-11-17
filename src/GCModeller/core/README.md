# GCModeller.Core
GCModeller base core assembly library on common biological database read and write I/O, is a set of individual components for the [GCModeller](https://github.com/SMRUCC/GCModeller).

The library was public release avaliable at nuget via:

```bash
# https://www.nuget.org/packages/GCModeller.Core/
PM>  Install-Package GCModeller.Core
```

###### Development
Development of this library required of ``sciBASIC#`` runtime, which is available download at github repository:

> https://github.com/xieguigang/sciBASIC

or install via nuget:

```bash
# github: https://github.com/xieguigang/sciBASIC#
# nuget: https://www.nuget.org/packages/sciBASIC#

PM> Install-Package sciBASIC -Pre
```

### Library overviews

This project is the core lib of GCModeller, it provides the common components in the GCModeller analysis tools such as sequence model and protein structure models, some necessary interface and component class for build the bio-system model in the GCModeller.

```vb.net
Imports SMRUCC.genomics.ComponentModel
Imports SMRUCC.genomics.ContextModel
Imports SMRUCC.genomics.SequenceModel
```

This Projects includes some common used biological database reader, the database includes:

1. NCBI GenBank database
2. KEGG DBGET API
3. MetaCyc Database
4. FASTA sequence database

All of these good staff is in the namespace:

```vb.net
Imports SMRUCC.genomics.Assembly.Bac_sRNA.org
Imports SMRUCC.genomics.Assembly.DOMINE
Imports SMRUCC.genomics.Assembly.DOOR
Imports SMRUCC.genomics.Assembly.KEGG
Imports SMRUCC.genomics.Assembly.MetaCyc
Imports SMRUCC.genomics.Assembly.NCBI
Imports SMRUCC.genomics.Assembly.Uniprot
```

### Quick guide for Read database

```vb.net
Dim path As String = "/path/to/database/file"

' Read NCBI genbank database file.
Dim gb As GBFF.File = GBFF.File.Load(path)
Dim gbs As IEnumerable(Of GBFF.File) = GBFF.File.LoadDatabase(path)
Dim PTT As PTT = PTT.Load(path)
Dim GFF As GFF = GFF.LoadDocument(path)

' Read fasta sequence file
Dim Fasta As New FASTA.FastaFile(path)
Dim nt As New FASTA.FastaToken(path)

nt = FastaToken.Load(path)
nt = FastaToken.LoadNucleotideData(path)
```

### NCBI taxonomy annotation data

```vbnet
Imports SMRUCC.genomics.Assembly.NCBI.Taxonomy

' Build taxonomy tree from NCBI taxonomy data
' tax$ directory should contains files: names.dmp and nodes.dmp, 
' which are avaliable download from NCBI ftp
Dim tax$ = <DIR>
Dim taxonomy As New NcbiTaxonomyTree(tax)

' For gi mapping to taxid
Dim giMapTaxid As BucketDictionary(Of Integer, Integer) =
    Taxonomy.AcquireAuto(gi2taxid)
    
' For accid mapping to taxid
Dim accMapTaxid As BucketDictionary(Of String, Integer) = 
    Accession2Taxid.LoadAll(DIR)
    
' OR processing auto
Dim mapping As TaxidMaps.Mapping = If(
    is_gi2taxid,
    TaxidMaps.MapByGI(x2taxid),
    TaxidMaps.MapByAcc(x2taxid))
```

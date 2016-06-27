# GCModeller.Core
GCModeller base core assembly library on common biological database read and write I/O

The library was public release avaliable at nuget site: https://www.nuget.org/packages/GCModeller.Core/

>  PM>  Install-Package GCModeller.Core

### Library overviews

This project is the core lib of GCModeller, it provides the common components in the GCModeller analysis tools such as sequence model and protein structure models, some necessary interface and component class for build the bio-system model in the GCModeller.

```vb.net
Imports LANS.SystemsBiology.ComponentModel
Imports LANS.SystemsBiology.ContextModel
Imports LANS.SystemsBiology.SequenceModel
```

This Projects includes some common used biological database reader, the database includes:

1. NCBI GenBank database
2. KEGG DBGET API
3. MetaCyc Database
4. FASTA sequence database

All of these good staff is in the namespace:

```vb.net
Imports LANS.SystemsBiology.Assembly.Bac_sRNA.org
Imports LANS.SystemsBiology.Assembly.DOMINE
Imports LANS.SystemsBiology.Assembly.DOOR
Imports LANS.SystemsBiology.Assembly.KEGG
Imports LANS.SystemsBiology.Assembly.MetaCyc
Imports LANS.SystemsBiology.Assembly.NCBI
Imports LANS.SystemsBiology.Assembly.Uniprot
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
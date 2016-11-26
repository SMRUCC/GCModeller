![](./images/metagenome-network.png)
> **Figure 2** Metavirome network. **A visualization of the all-verses-all BLAST network.** Viral groups are assigned a unique color and were visualized in Gephi. Separation of the viral groups was accomplished using Gephi’s Force Atlas 2 plugin, a force-directed layout algorithm. Ovals highlight viral groups whose members matched against seeded viruses in a parallel network analysis. Despite the large number of seeded viruses (1812), only 26 viruses were assigned to viral groups. All 26 viruses were crenarchaeal viruses. Edge connections are lines connecting members within a viral group and between members of differing viral groups. **Care should be taken to not associate distance between viral groups as indicative of sequence similarity.** To enhance clarity and distinctions between viral groups, only those containing 50 contigs or more are included in the figure. Viral groups numbering: ``group 0 = SIRV-1,2``; ``group 23 = ASV-1, SSV-1-2, 4-9``; ``group 26 = ATV``; ``group 28 = AFV-1``; ``group 29 = STIV-1,2``; ``group 31 = AFV2-3, 6-9, SIFV``; ``group 32 = STST-1,2 and ARSV-1, SRV``.

## Creates metagenome network using BLAST on 16S/18S sequencing

### Protocol in Cytoscape CLI tools

```vbnet
<ExportAPI("/BLAST.Metagenome.SSU.Network",
           Info:="> Viral assemblage composition in Yellowstone acidic hot springs assessed by network analysis, DOI: 10.1038/ismej.2015.28",
           Usage:="/BLAST.Metagenome.SSU.Network /net <blastn.self.txt> /tax <ssu-nt.blastnMaps.csv> /x2taxid <x2taxid.dmp/DIR> /taxonomy <ncbi_taxonomy:names,nodes> [/gi2taxid /identities <default:0.3> /coverage <default:0.3> /out <out-net.DIR>]")>
<Group(CLIGrouping.Metagenomics)>
Public Function SSU_MetagenomeNetwork(args As CommandLine) As Integer
    Dim net$ = args("/net")
    Dim tax$ = args("/tax")
    Dim taxonomy$ = args("/taxonomy")
    Dim identities As Double = args.GetValue("/identities", 0.3)
    Dim coverage As Double = args.GetValue("/coverage", 0.3)
    Dim EXPORT$ = args.GetValue("/out", net.TrimSuffix & "-" & tax.BaseName & "-metagenome-network/")
    Dim out As New Value(Of String)
    Dim netdata As v228 = BlastPlus.Parser.ParsingSizeAuto(net)
    Dim taxdata As BlastnMapping() = tax.LoadCsv(Of BlastnMapping)
    Dim gi2taxid As Boolean = args.GetBoolean("/gi2taxid")
    Dim x2taxid As String = args("/x2taxid")
    Dim xid$() = taxdata _
        .Select(Function(x) x.Reference) _
        .ToArray(TaxidMaps.GetParser(gi2taxid))

    Call xid.FlushAllLines(out = EXPORT & "/reference_xid.txt")

    ' subset database
    Dim CLI$

    If gi2taxid Then
        CLI = $"/gi.Match /in {(+out).CLIPath} /gi2taxid {x2taxid.CLIPath} /out {(out = EXPORT & "/gi2taxid.txt").CLIPath}"
    Else
        CLI = $"/accid2taxid.Match /in {(+out).CLIPath} /acc2taxid {x2taxid.CLIPath} /gb_priority /out {(out = EXPORT & "/gi2taxid.txt").CLIPath}"
    End If

    Call New IORedirectFile(Apps.NCBI_tools, CLI).Run()

    ' step1
    Dim notFound As New List(Of String)
    Dim ssuTax As IEnumerable(Of BlastnMapping) = taxdata.TaxonomyMaps(
        x2taxid:=+out,
        is_gi2taxid:=gi2taxid,
        notFound:=notFound,
        taxonomy:=New NcbiTaxonomyTree(taxonomy))

    Call notFound.FlushAllLines(EXPORT & "/taxonomy_notfound.txt")

    ' step2
    Dim matrix As IEnumerable(Of DataSet) =
        netdata.BuildMatrix(identities, coverage)

    ' step3
    Dim network As Network = BuildNetwork(matrix, ssuTax)

    Return network.Save(EXPORT, Encodings.ASCII).CLICode
End Function
```

### CLI usage Tutorials

#### 1. Preparing the 16S/18S database
About screen all of the 16S/18S sequence from nt database using command tool:

```bash
localblast ? /Fasta.Filters
# Help for command '/Fasta.Filters':
#
#   Information:  Filter the fasta sequence subset from a larger fasta database by using the regexp for match
#                 on the fasta title.
#   Usage:        /home/biostack/GCModeller/localblast /Fasta.Filters /in <nt.fasta> /key <regex/list.txt> [/tokens /out <out.fasta> /p]
#   Example:      CLI usage example not found!
#
#   Arguments:
#   ============================
#
#    /key    Description:  A regexp string term that will be using for title search or file
#                          path of a text file contains lines of regexp.
#
#            Example:      /key <file/directory>
#                          (This argument can accept the std_out from upstream app as input)
#
#   [/p]     Description:  Using the parallel edition?? If GCModeller running in a 32bit
#                          environment, do not use this option. This option only works in
#                          single key mode.
#
#            Example:      /p <term_string>

# Example as

# For screen all of the 18S and 16S sequence, using
localblast /Fasta.Filters /in "/biostack/database/nt" /key "18S|16S"

# For screen only the 16S sequence, using
localblast /Fasta.Filters /in "/biostack/database/nt" /key "16S"
```

#### 2. Taxonomy reference mapping

The you can mapping you metagenome data on the ``18S/16S`` fasta for achieve of the taxonomy information, for mapping **OTU/contig/Scaftigs/scaffold** data, using NCBI blast+ ``blastn`` mapping, example as:

```bash
makeblastdb -in "/biostack/database/nt-16S.fasta" -dbtype nucl
blastn -query "/home/gx-guilin.contigs.fasta" -db "/biostack/database/nt-16S.fasta" -out "/home/gx-guilin.contigs.txt" -evalue 1e-5
```

#### 3. Export mapping result using GCModeller tools

Using command:

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

# Example as
localblast /Export.blastnMaps /in "/home/gx-guilin.16s.txt" /best
```

#### 4. All-verses-all BLAST

```bash
makeblastdb -in "/home/gx-guilin.contigs.fasta" -dbtype nucl
blastn -query "/home/gx-guilin.contigs.fasta" -db "/home/gx-guilin.contigs.fasta" -out "/home/gx-guilin.contigs.network.txt"
```

#### 5. Build model for Cytoscape

Required of NCBI taxonomy database, which can be download from: ftp://ftp.ncbi.nlm.nih.gov/pub/taxonomy/taxdmp.zip

```bash
Cytoscape ? /BLAST.Metagenome.SSU.Network
# Help for command '/BLAST.Metagenome.SSU.Network':
#
#   Information:  > Viral assemblage composition in Yellowstone acidic hot springs assessed by network analysis,
#                 DOI: 10.1038/ismej.2015.28
#   Usage:        /home/biostack/GCModeller/Cytoscape /BLAST.Metagenome.SSU.Network /net <blastn.self.txt> /tax <ssu-nt.blastnMaps.csv> /x2taxid <x2taxid.dmp/DIR> /taxonomy <ncbi_taxonomy:names,nodes> [/skip-exists /gi2taxid /identities <default:0.3> /coverage <default:0.3> /out <out-net.DIR>]
#   Example:      CLI usage example not found!
```

###### For legacy GI nt headers

Download database: ftp://ftp.ncbi.nlm.nih.gov/pub/taxonomy/gi_taxid_nucl.zip

```bash
Cytoscape /BLAST.Metagenome.SSU.Network /net "/home/gx-guilin.contigs.network.txt" /tax "/home/gx-guilin.16s.Best.Csv" /x2taxid "./gi_taxid_nucl.dmp" /taxonomy "./taxdmp" /gi2taxid
```

###### For new accession nt header

Download all database files in directory: ftp://ftp.ncbi.nlm.nih.gov/pub/taxonomy/accession2taxid/

```bash
Cytoscape /BLAST.Metagenome.SSU.Network /net "/home/gx-guilin.contigs.network.txt" /tax "/home/gx-guilin.16s.Best.Csv" /x2taxid "/biostack/database/accession2taxid/" /taxonomy "./taxdmp"
```


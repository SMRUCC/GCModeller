Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.Assembly.NCBI
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application
Imports SMRUCC.genomics.Metagenomics

Partial Module CLI

    <ExportAPI("/Reads.OTU.Taxonomy",
               Usage:="/Reads.OTU.Taxonomy /in <blastnMaps.csv> /OTU <OTU_data.csv> /tax <taxonomy:nodes/names> [/out <out.csv>]")>
    Public Function ReadsOTU_Taxonomy(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim OTU As String = args("/OTU")
        Dim tax As String = args("/tax")
        Dim out As String = args.GetValue("/out", [in].TrimSuffix & "-" & OTU.BaseName & ".Taxonomy.csv")
        Dim maps = [in].LoadCsv(Of BlastnMapping)
        Dim data = OTU.LoadCsv(Of OTUData)
        Dim taxonomy As New NcbiTaxonomyTree(tax)
        Dim readsTable = (From x As BlastnMapping
                          In maps
                          Select x
                          Group x By x.ReadQuery Into Group) _
            .ToDictionary(Function(x) x.ReadQuery,
                          Function(x) x.Group.ToArray)
        Dim output As New List(Of OTUData)

        For Each r In data
            Dim reads = readsTable(r.OTU)

            For Each o In reads
                Dim copy As New OTUData(r)
                Dim taxid% = CInt(o.Extensions("taxid"))
                Dim nodes = taxonomy.GetAscendantsWithRanksAndNames(taxid, True)
                copy.Data("taxid") = taxid
                copy.Data("Taxonomy") = TaxonomyNode.BuildBIOM(nodes)
                copy.Data("ref") = o.Reference
            Next
        Next

        Return output.SaveTo(out).CLICode
    End Function
End Module
Imports System.ComponentModel
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.Analysis.Metagenome
Imports SMRUCC.genomics.Analysis.Metagenome.gast
Imports SMRUCC.genomics.Metagenomics

Partial Module CLI

    <ExportAPI("/gast.stat.names")>
    <Usage("/gast.stat.names /in <*.names> /gast <gast.out> [/out <out.Csv>]")>
    Public Function StateNames(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim out As String = args.GetValue("/out", [in].TrimSuffix & ".stat.Csv")
        Dim gastOut As String = args("/gast")
        Dim result As Names() = ParseNames([in]).FillTaxonomy(gastOut).ToArray
        Call BIOM.Imports(result, 1000).ToJSON.SaveTo(out.TrimSuffix & ".Megan.biom")
        Call MeganImports.Out(result).Save(out.TrimSuffix & ".Megan.Csv", Encodings.ASCII)
        Return result.SaveTo(out).CLICode
    End Function

    <ExportAPI("/Export.Megan.BIOM")>
    <Usage("/Export.Megan.BIOM /in <relative.table.csv> [/rebuildBIOM.tax /out <out.biom.json>]")>
    <Description("Export v1.0 biom json file for data visualize in Megan program.")>
    <Argument("/in", False, AcceptTypes:={GetType(OTUData)})>
    Public Function ExportToMegan(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim out As String = args.GetValue("/out", [in].TrimSuffix & ".biom.json")
        Dim rebuildBIOM As Boolean = args("/rebuildBIOM.tax")
        Dim data As OTUData() = [in].LoadCsv(Of OTUData)()
        Dim result = data.EXPORT(alreadyBIOMTax:=Not rebuildBIOM)

        Return result.ToJSON.SaveTo(out).CLICode
    End Function
End Module

Imports LANS.SystemsBiology.Assembly.KEGG.DBGET.bGetObject
Imports LANS.SystemsBiology.Assembly.KEGG.WebServices
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.DocumentFormat.Csv
Imports Microsoft.VisualBasic.Scripting.MetaData

<PackageNamespace("KEGG.Ortholog.Export",
                  Category:=APICategories.ResearchTools,
                  Description:="KEGG ortholog annotation database.",
                  Publisher:="xie.guigang@gcmodeller.org",
                  Url:="http://www.kegg.jp/ssdb-bin/ssdb_best?org_gene={sp}:{locus_tag}")>
Public Module OrthologExport

    <ExportAPI("Export")>
    Public Function Export(ortholog As SSDB.OrthologREST) As DocumentStream.File
        Dim CsvData As DocumentStream.File =
            New DocumentStream.File +
            {"KEGG_Entry", "Definition", "KO", "Length", "Sw-Score", "Margin", "bits", "identity", "overlap", "best(all)"} +
            From resultRow As SSDB.SShit In ortholog.Orthologs Select __createRowObject(resultRow)
        Return CsvData
    End Function

    Private Function __createRowObject(resultRow As SSDB.SShit) As DocumentStream.RowObject
        Dim ArrayData As String() = New String() {
            resultRow.Entry.ToString,
            resultRow.Entry.Description,
            resultRow.KO.Value,
            resultRow.Length,
            resultRow.SWScore,
            resultRow.Margin,
            resultRow.Bits,
            resultRow.Identity,
            resultRow.Overlap,
            String.Format("{0} {1}", resultRow.Best.Key, resultRow.Best.Value)
        }
        Return New DocumentStream.RowObject(ArrayData)
    End Function

    <ExportAPI("HandleQuery")>
    Public Sub HandleQuery(lstId As String(), outDIR As String)
        For Each GeneId As String In lstId
            Call HandleQuery(GeneId, outDIR)
        Next
    End Sub

    <ExportAPI("HandleQuery")>
    Public Sub HandleQuery(lstId As String(), outDIR As String, sp As String)
        For Each locusId As String In lstId
            If $"{outDIR}/{locusId}.xml".FileExists AndAlso $"{outDIR}/{locusId}.csv".FileExists Then
                Continue For
            End If

            Call HandleQuery(locusId, sp, outDIR)
        Next
    End Sub

    <ExportAPI("HandleQuery")>
    Public Sub HandleQuery(locusId As String, sp As String, outDIR As String)
        Dim query As New QueryEntry With {
            .LocusId = locusId,
            .SpeciesId = sp
        }
        Call HandleQuery(query, outDIR)
    End Sub

    <ExportAPI("HandleQuery")>
    Public Sub HandleQuery(query As QueryEntry, outDIR As String)
        Dim Ortholog = SSDB.OrthologREST.Download(query)

        Call Ortholog.GetXml.SaveTo($"{outDIR}/{query.LocusId}.xml")
        Call OrthologExport.Export(Ortholog).Save($"{outDIR}/{query.LocusId}.csv", Text.Encoding.ASCII)
    End Sub

    <ExportAPI("HandleQuery")>
    Public Sub HandleQuery(locusId As String, outDIR As String)
        Dim Entries = WebRequest.HandleQuery(keyword:=locusId)

        If Entries.IsNullOrEmpty Then
            Call Console.WriteLine($"[KEGG_ENTRY_NOT_FOUND] KEYWORD:={locusId}")
            Return
        End If

        Dim LQuery = (From entry As QueryEntry
                      In Entries
                      Where String.Equals(entry.LocusId, locusId, StringComparison.OrdinalIgnoreCase)
                      Select entry).FirstOrDefault
        If LQuery Is Nothing Then
            Call Console.WriteLine($"[ENTRY_IS_NULL] GeneId:={locusId}")
        Else
            Call HandleQuery(LQuery, outDIR)
        End If
    End Sub
End Module

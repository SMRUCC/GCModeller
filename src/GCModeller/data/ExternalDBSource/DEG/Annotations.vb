Imports SMRUCC.genomics.NCBI.Extensions.LocalBLAST.Application
Imports Microsoft.VisualBasic.DocumentFormat.Csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.DocumentFormat.Csv.Extensions
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports SMRUCC.genomics.NCBI.Extensions.LocalBLAST.BLASTOutput
Imports Microsoft.VisualBasic.DocumentFormat.Csv
Imports Microsoft.VisualBasic.Text

Namespace DEG

    <PackageNamespace("DEG.Annotations")>
    Public Module Workflows

        <ExportAPI("Reports")>
        Public Function CreateReportView(LogFile As IBlastOutput, Annotations As DEG.Annotations()) As DocumentStream.File
            Call LogFile.Grep(Nothing, TextGrepScriptEngine.Compile("match DEG\d+").Method)
            Dim BestHit = LogFile.ExportAllBestHist '.AsDataSource(Of SMRUCC.genomics.NCBI.Extensions.LocalBLAST.Application.BBH.BestHit)(False)
            Dim CsvData As DocumentStream.File = New DocumentFormat.Csv.DocumentStream.File
            Dim QueriesId As String() = (From item In BestHit Select item.QueryName Distinct Order By QueryName Ascending).ToArray
            Dim SpeciesIdCollection = DEG.Annotations.GetSpeciesId(Annotations)

            Dim SpeciesIdRow As DocumentStream.RowObject = New DocumentStream.RowObject From {"SpeciesId"}
            Dim BestHitTitleRow As DocumentStream.RowObject = New DocumentStream.RowObject From {"QueryName", "QueryLength"}

            For Each SpeciesId As String In SpeciesIdCollection
                Call SpeciesIdRow.Add("")
                Call SpeciesIdRow.AddRange(New String() {"", SpeciesId, "", "", "", "", "", "", "", "", "", ""})
                Call BestHitTitleRow.Add("")
                Call BestHitTitleRow.AddRange(New String() {"#DEG_AC", "COG", "GeneName", "Gene_Ref", "hit_length", "e-value", "identities", "Score", "positive", "length_query", "length_hit", "length_hsp"})
            Next

            Call CsvData.Add(SpeciesIdRow)
            Call CsvData.Add(BestHitTitleRow)

            For Each Id As String In QueriesId
                Dim RowCollection = BBH.BestHit.FindByQueryName(Id, BestHit)
                Dim LQuery = (From item In RowCollection Let obj = item.HitName.GetItem(Annotations) Where Not obj Is Nothing Select New With {.BestHit = item, .Annotiation = obj}).ToArray
                Dim Row As DocumentStream.RowObject = New DocumentStream.RowObject From {Id, RowCollection.First.query_length}

                If Not LQuery.IsNullOrEmpty Then
                    For Each SpeciesId As String In SpeciesIdCollection
                        Dim GetItems = (From item In LQuery Where String.Equals(item.Annotiation.Organism, SpeciesId) Select item Order By item.BestHit.evalue Ascending).ToArray

                        If Not GetItems.IsNullOrEmpty Then
                            Dim item = GetItems.First
                            Dim ChunkBuffer As String() = New String() {item.Annotiation.DEG_AC, item.Annotiation.COG, item.Annotiation.GeneName, item.Annotiation.Gene_Ref, item.BestHit.hit_length, item.BestHit.evalue, item.BestHit.identities, item.BestHit.Score, item.BestHit.Positive, item.BestHit.length_query, item.BestHit.length_hit, item.BestHit.length_hsp}

                            Call Row.Add("")
                            Call Row.AddRange(ChunkBuffer)
                        Else
                            Call Row.Add("")
                            Call Row.AddRange(New String() {IBlastOutput.HITS_NOT_FOUND, "-", "-", "-", "-", "-", "-", "-", "-", "-", "-", "-"})
                        End If
                    Next
                End If

                Call CsvData.Add(Row)
            Next

            Return CsvData
        End Function
    End Module
End Namespace
#Region "Microsoft.VisualBasic::290a71b2fa308c57c22decf78d911d6d, ..\GCModeller\data\ExternalDBSource\DEG\Annotations.vb"

' Author:
' 
'       asuka (amethyst.asuka@gcmodeller.org)
'       xieguigang (xie.guigang@live.com)
' 
' Copyright (c) 2016 GPL3 Licensed
' 
' 
' GNU GENERAL PUBLIC LICENSE (GPL3)
' 
' This program is free software: you can redistribute it and/or modify
' it under the terms of the GNU General Public License as published by
' the Free Software Foundation, either version 3 of the License, or
' (at your option) any later version.
' 
' This program is distributed in the hope that it will be useful,
' but WITHOUT ANY WARRANTY; without even the implied warranty of
' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
' GNU General Public License for more details.
' 
' You should have received a copy of the GNU General Public License
' along with this program. If not, see <http://www.gnu.org/licenses/>.

#End Region

Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.DocumentStream
Imports Microsoft.VisualBasic.Data.csv.Extensions
Imports Microsoft.VisualBasic.Data.csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.BLASTOutput

Namespace DEG

    <PackageNamespace("DEG.Annotations")>
    Public Module Workflows

        <ExportAPI("Reports")>
        Public Function CreateReportView(LogFile As IBlastOutput, Annotations As DEG.Annotations()) As DocumentStream.File
            Call LogFile.Grep(Nothing, TextGrepScriptEngine.Compile("match DEG\d+").Method)
            Dim BestHit = LogFile.ExportAllBestHist '.AsDataSource(Of SMRUCC.genomics.NCBI.Extensions.LocalBLAST.Application.BBH.BestHit)(False)
            Dim CsvData As File
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

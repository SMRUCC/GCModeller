﻿#Region "Microsoft.VisualBasic::4093df00b1ea72b17115693a5df3e7ba, data\ExternalDBSource\DEG\Annotations.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xie (genetics@smrucc.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
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



    ' /********************************************************************************/

    ' Summaries:

    '     Module Workflows
    ' 
    '         Function: CreateReportView
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Scripting
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.BLASTOutput

Namespace DEG

    <Package("DEG.Annotations")>
    Public Module Workflows

        <ExportAPI("Reports")>
        Public Function CreateReportView(LogFile As IBlastOutput, Annotations As DEG.Annotations()) As IO.File
            Call LogFile.Grep(Nothing, TextGrepScriptEngine.Compile("match DEG\d+").PipelinePointer)
            Dim BestHit = LogFile.ExportAllBestHist '.AsDataSource(Of SMRUCC.genomics.NCBI.Extensions.LocalBLAST.Application.BBH.BestHit)(False)
            Dim csv As New File
            Dim QueriesId$() = LinqAPI.Exec(Of String) <=
                From hit As BBH.BestHit
                In BestHit
                Select hit.QueryName
                Distinct
                Order By QueryName Ascending
            Dim SpeciesIdCollection = DEG.Annotations.GetSpeciesId(Annotations)

            Dim SpeciesIdRow As New RowObject From {"SpeciesId"}
            Dim BestHitTitleRow As New RowObject From {"QueryName", "QueryLength"}

            For Each SpeciesId As String In SpeciesIdCollection
                Call SpeciesIdRow.Add("")
                Call SpeciesIdRow.AddRange(New String() {"", SpeciesId, "", "", "", "", "", "", "", "", "", ""})
                Call BestHitTitleRow.Add("")
                Call BestHitTitleRow.AddRange(New String() {"#DEG_AC", "COG", "GeneName", "Gene_Ref", "hit_length", "e-value", "identities", "Score", "positive", "length_query", "length_hit", "length_hsp"})
            Next

            Call csv.Add(SpeciesIdRow)
            Call csv.Add(BestHitTitleRow)

            For Each Id As String In QueriesId
                Dim RowCollection = BBH.BestHit.FindByQueryName(Id, BestHit)
                Dim LQuery = (From item In RowCollection Let obj = item.HitName.GetItem(Annotations) Where Not obj Is Nothing Select New With {.BestHit = item, .Annotiation = obj}).ToArray
                Dim Row As New IO.RowObject From {Id, RowCollection.First.query_length}

                If Not LQuery.IsNullOrEmpty Then
                    For Each SpeciesId As String In SpeciesIdCollection
                        Dim GetItems = (From item
                                        In LQuery
                                        Where String.Equals(item.Annotiation.Organism, SpeciesId)
                                        Select item
                                        Order By item.BestHit.evalue Ascending).ToArray

                        If Not GetItems.IsNullOrEmpty Then
                            Dim item = GetItems.First
                            Dim ChunkBuffer As String() = {
                                item.Annotiation.DEG_AC,
                                item.Annotiation.COG,
                                item.Annotiation.GeneName,
                                item.Annotiation.Gene_Ref,
                                item.BestHit.hit_length,
                                item.BestHit.evalue,
                                item.BestHit.identities,
                                item.BestHit.Score,
                                item.BestHit.Positive,
                                item.BestHit.length_query,
                                item.BestHit.length_hit,
                                item.BestHit.length_hsp
                            }

                            Call Row.Add("")
                            Call Row.AddRange(ChunkBuffer)
                        Else
                            Call Row.Add("")
                            Call Row.AddRange(New String() {IBlastOutput.HITS_NOT_FOUND, "-", "-", "-", "-", "-", "-", "-", "-", "-", "-", "-"})
                        End If
                    Next
                End If

                Call csv.Add(Row)
            Next

            Return csv
        End Function
    End Module
End Namespace

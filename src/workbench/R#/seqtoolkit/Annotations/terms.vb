#Region "Microsoft.VisualBasic::384b5df159833b117e5acd310e689986, R#\seqtoolkit\Annotations\terms.vb"

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


    ' Code Statistics:

    '   Total Lines: 147
    '    Code Lines: 104 (70.75%)
    ' Comment Lines: 22 (14.97%)
    '    - Xml Docs: 95.45%
    ' 
    '   Blank Lines: 21 (14.29%)
    '     File Size: 6.22 KB


    ' Module terms
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: COGannotations, COGtable, geneNames, GOannotations, KOannotations
    '               Pfamannotations, printIDSolver, readIdMappings, readMyvaCOG, saveIdMappings
    '               Synonyms
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.Framework
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics
Imports SMRUCC.genomics.ComponentModel
Imports SMRUCC.genomics.ComponentModel.DBLinkBuilder
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH
Imports SMRUCC.genomics.Interops.NCBI.Extensions.Pipeline
Imports SMRUCC.genomics.Interops.NCBI.Extensions.Pipeline.COG
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop
Imports SMRUCC.Rsharp.Runtime.Vectorization
Imports dataframe = SMRUCC.Rsharp.Runtime.Internal.Object.dataframe
Imports RInternal = SMRUCC.Rsharp.Runtime.Internal

<Package("annotation.terms", Category:=APICategories.ResearchTools, Publisher:="xie.guigang@gcmodeller.org")>
Module terms

    Sub New()
        Call RInternal.ConsolePrinter.AttachConsoleFormatter(Of SecondaryIDSolver)(AddressOf printIDSolver)

        Call RInternal.Object.Converts.makeDataframe.addHandler(GetType(MyvaCOG()), AddressOf COGtable)
    End Sub

    Private Function COGtable(myva As MyvaCOG(), args As list, env As Environment) As dataframe
        Dim text = myva.ToCsvDoc
        Dim table As New dataframe With {.columns = New Dictionary(Of String, Array)}

        For Each column As String() In text.Columns
            Call table.columns.Add(column(Scan0), column.Skip(1).ToArray)
        Next

        Return table
    End Function

    Private Function printIDSolver(solver As SecondaryIDSolver) As String
        Dim sb As New StringBuilder
        Dim synonym As String()
        Dim summary As String

        Call sb.AppendLine($"{NameOf(SecondaryIDSolver)} for {solver.Count} id entities:")
        Call sb.AppendLine($"[{solver.Count}] {solver.ALL.Take(10).JoinBy(" ")}...")
        Call sb.AppendLine()

        For Each id As String In solver.ALL.Take(6)
            synonym = solver.GetSynonym(id).alias
            summary = If(synonym.Length <= 5, synonym.JoinBy(vbTab), synonym.Take(5).JoinBy(vbTab) & "...")
            sb.AppendLine($"${id}: {summary}")
        Next

        Call sb.AppendLine("...")

        Return sb.ToString
    End Function

    ''' <summary>
    ''' try parse gene names from the product description strings
    ''' </summary>
    ''' <param name="descriptions">
    ''' the gene functional product description strings.
    ''' </param>
    ''' <returns></returns>
    <ExportAPI("geneNames")>
    Public Function geneNames(<RRawVectorArgument> descriptions As Object) As vector
        Return CLRVector.asCharacter(descriptions) _
            .Select(AddressOf ObjectQuery.geneName.TryParseGeneName) _
            .DoCall(AddressOf vector.asVector)
    End Function

    ''' <summary>
    ''' do KO number assign based on the bbh alignment result.
    ''' </summary>
    ''' <param name="forward"></param>
    ''' <param name="reverse"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("assign.KO")>
    Public Function KOannotations(forward As pipeline, reverse As pipeline, Optional env As Environment = Nothing) As pipeline
        If forward Is Nothing Then
            Return RInternal.debug.stop("forward data stream is nothing!", env)
        ElseIf reverse Is Nothing Then
            Return RInternal.debug.stop("reverse data stream is nothing!", env)
        ElseIf Not forward.elementType.raw Is GetType(BestHit) Then
            Return RInternal.debug.stop($"forward is invalid data stream type: {forward.elementType.fullName}!", env)
        ElseIf Not reverse.elementType.raw Is GetType(BestHit) Then
            Return RInternal.debug.stop($"reverse is invalid data stream type: {reverse.elementType.fullName}!", env)
        End If

        Return KOAssignment.KOassignmentBBH(forward.populates(Of BestHit)(env), reverse.populates(Of BestHit)(env)).DoCall(AddressOf pipeline.CreateFromPopulator)
    End Function

    <ExportAPI("assign.COG")>
    Public Function COGannotations(<RRawVectorArgument> alignment As Object, Optional env As Environment = Nothing) As Object
        If TypeOf alignment Is MyvaCOG() Then
            Return DirectCast(alignment, MyvaCOG()) _
                .GroupBy(Function(hit) hit.QueryName) _
                .Select(Function(hits)
                            Return hits.OrderByDescending(Function(hit) hit.Identities).First
                        End Function) _
                .ToArray
        Else
            Return RInternal.debug.stop(Message.InCompatibleType(GetType(MyvaCOG), alignment.GetType, env), env)
        End If
    End Function

    <ExportAPI("assign_terms")>
    Public Function TermAnnotations(<RRawVectorArgument> alignment As Object, Optional env As Environment = Nothing) As Object

    End Function

    <ExportAPI("assign.Pfam")>
    Public Function Pfamannotations()
        Throw New NotImplementedException
    End Function

    <ExportAPI("assign.GO")>
    Public Function GOannotations()
        Throw New NotImplementedException
    End Function

    <ExportAPI("write.id_maps")>
    Public Function saveIdMappings(maps As SecondaryIDSolver, file As String) As Boolean
        Return maps.Save(path:=file)
    End Function

    <ExportAPI("read.MyvaCOG")>
    Public Function readMyvaCOG(file As String) As MyvaCOG()
        Return file.LoadCsv(Of MyvaCOG).ToArray
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="file"></param>
    ''' <param name="skip2ndMaps">
    ''' set this parameter value to ``true`` for fixed for build the ``kegg2go`` mapping model.
    ''' </param>
    ''' <returns></returns>
    <ExportAPI("read.id_maps")>
    Public Function readIdMappings(file As String, Optional skip2ndMaps As Boolean = False) As SecondaryIDSolver
        Return DBLinkBuilder.LoadMappingText(file, skip2ndMaps)
    End Function

    <ExportAPI("synonym")>
    Public Function Synonyms(idlist As String(), idmap As SecondaryIDSolver, Optional excludeNull As Boolean = False) As Synonym()
        Return idmap.PopulateSynonyms(idlist, excludeNull:=excludeNull).ToArray()
    End Function
End Module

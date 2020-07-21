#Region "Microsoft.VisualBasic::a66c7badf9ebf93f5a590f187695feb0, seqtoolkit\Annotations\workflows.vb"

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

    ' Module workflows
    ' 
    '     Function: ExportBBHHits, ExportSBHHits, FilterBesthitStream, flush, grepNames
    '               openBlastReader, openWriter, parseBlastnMaps
    ' 
    '     Sub: writeStreamHelper
    ' 
    ' Enum TableTypes
    ' 
    '     BBH, Mapping, SBH
    ' 
    '  
    ' 
    ' 
    ' 
    ' Enum BBHAlgorithm
    ' 
    '     BHR, Naive, TaxonomySupports
    ' 
    '  
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ApplicationServices.Debugging.Logging
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv.IO.Linq
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.NtMapping
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.BLASTOutput
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.BLASTOutput.BlastPlus
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports REnv = SMRUCC.Rsharp.Runtime

''' <summary>
''' A pipeline collection for proteins' biological function 
''' annotation based on the sequence alignment.
''' </summary>
<Package("annotation.workflow", Category:=APICategories.ResearchTools, Publisher:="xie.guigang@gcmodeller.org")>
Module workflows

    ''' <summary>
    ''' Open the blast output text file for parse data result.
    ''' </summary>
    ''' <param name="file"></param>
    ''' <param name="type">``nucl`` or ``prot``</param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("read.blast")>
    Public Function openBlastReader(file As String, Optional type As String = "nucl", Optional fastMode As Boolean = True, Optional env As Environment = Nothing) As pipeline
        If Not file.FileExists(True) Then
            Return REnv.Internal.debug.stop($"invalid data source: '{file.ToFileURL}'!", env)
        End If

        If LCase(type) = "nucl" Then
            Return BlastPlus.TryParseUltraLarge(file).Queries.DoCall(AddressOf pipeline.CreateFromPopulator)
        ElseIf LCase(type) = "prot" Then
            Return pipeline.CreateFromPopulator(BlastpOutputReader.RunParser(file, fast:=fastMode))
        Else
            Return Internal.debug.stop($"invalid program type: {type}!", env)
        End If
    End Function

    <ExportAPI("blastn.maphit")>
    Public Function parseBlastnMaps(query As pipeline, Optional env As Environment = Nothing) As pipeline
        If query Is Nothing Then
            Return Nothing
        ElseIf Not query.elementType Like GetType(Query) Then
            Return REnv.Internal.debug.stop($"invalid pipeline data type: {query.elementType.ToString}", env)
        End If

        Return query.populates(Of Query)(env) _
            .Export(parallel:=False) _
            .DoCall(AddressOf pipeline.CreateFromPopulator)
    End Function

    <ExportAPI("blasthit.sbh")>
    <Extension>
    Public Function ExportSBHHits(query As pipeline,
                                  Optional idetities As Double = 0.3,
                                  Optional coverage As Double = 0.5,
                                  Optional topBest As Boolean = False,
                                  Optional keepsRawName As Boolean = False,
                                  Optional env As Environment = Nothing) As pipeline

        If query Is Nothing Then
            Return Nothing
        ElseIf Not query.elementType Like GetType(Query) Then
            Return REnv.Internal.debug.stop($"invalid pipeline data type: {query.elementType.ToString}", env)
        End If

        Dim hitsPopulator As Func(Of IEnumerable(Of BestHit()))

        If topBest Then
            hitsPopulator = Iterator Function() As IEnumerable(Of BestHit())
                                For Each q As Query In query.populates(Of Query)(env)
                                    Yield {
                                        v228.SBHLines(q, coverage, idetities, keepsRawQueryName:=keepsRawName)(Scan0)
                                    }
                                Next
                            End Function
        Else
            hitsPopulator = Iterator Function() As IEnumerable(Of BestHit())
                                For Each q As Query In query.populates(Of Query)(env)
                                    Yield v228.SBHLines(q, coverage, idetities, keepsRawQueryName:=keepsRawName)
                                Next
                            End Function
        End If

        Return New pipeline(hitsPopulator().IteratesALL, GetType(BestHit))
    End Function

    <ExportAPI("blasthit.bbh")>
    Public Function ExportBBHHits(forward As pipeline, reverse As pipeline, Optional algorithm As BBHAlgorithm = BBHAlgorithm.Naive, Optional env As Environment = Nothing) As pipeline
        If forward Is Nothing Then
            Return REnv.Internal.debug.stop("No forward alignment data!", env)
        ElseIf reverse Is Nothing Then
            Return REnv.Internal.debug.stop("No reversed alignment data!", env)
        End If

        If forward.elementType Like GetType(Query) Then
            forward = forward.ExportSBHHits(env:=env)
            env.AddMessage($"Best hit result from raw query in forward direction with default parameters.", MSG_TYPES.WRN)
        End If
        If reverse.elementType Like GetType(Query) Then
            reverse = reverse.ExportSBHHits(env:=env)
            env.AddMessage($"Best hit result from raw query in reverse direction with default parameters.", MSG_TYPES.WRN)
        End If

        If Not forward.elementType Like GetType(BestHit) Then
            Return REnv.Internal.debug.stop($"Invalid data type {forward.ToString} in forward direction for create bbh result!", env)
        ElseIf Not reverse.elementType Like GetType(BestHit) Then
            Return REnv.Internal.debug.stop($"Invalid data type {forward.ToString} in reverse direction for create bbh result!", env)
        End If

        Select Case algorithm
            Case BBHAlgorithm.Naive
                Return pipeline.CreateFromPopulator(BBHParser.GetBBHTop(forward.populates(Of BestHit)(env), reverse.populates(Of BestHit)(env)))
            Case BBHAlgorithm.BHR
            Case BBHAlgorithm.TaxonomySupports
            Case Else
                Return REnv.Internal.debug.stop("invalid algorithm supports!", env)
        End Select

        Return REnv.Internal.debug.stop(New NotImplementedException, env)
    End Function

    <ExportAPI("grep.names")>
    Public Function grepNames(query As pipeline, operators As Object,
                              Optional applyOnHits As Boolean = False,
                              Optional env As Environment = Nothing) As pipeline

        If query Is Nothing Then
            Return Nothing
        ElseIf Not query.elementType Like GetType(Query) Then
            Return REnv.Internal.debug.stop($"Invalid pipeline data type: {query.elementType.ToString}", env)
        End If

        If operators Is Nothing Then
            env.AddMessage("No operations provided!", MSG_TYPES.WRN)
            Return query
        ElseIf TypeOf operators Is String Then
            operators = TextGrepScriptEngine.Compile(operators)
        ElseIf Not TypeOf operators Is TextGrepScriptEngine Then
            Return REnv.Internal.debug.stop($"Invalid program: {operators.GetType.FullName}", env)
        End If

        Dim queryPopulator As Func(Of IEnumerable(Of Query))
        Dim grep As TextGrepMethod = DirectCast(operators, TextGrepScriptEngine).PipelinePointer

        If applyOnHits Then
            queryPopulator = Iterator Function() As IEnumerable(Of Query)
                                 For Each q As Query In query.populates(Of Query)(env)
                                     For Each hit In q.SubjectHits
                                         hit.Name = grep(hit.Name)
                                     Next

                                     Yield q
                                 Next
                             End Function
        Else
            queryPopulator = Iterator Function() As IEnumerable(Of Query)
                                 For Each q As Query In query.populates(Of Query)(env)
                                     q.QueryName = grep(q.QueryName)
                                     Yield q
                                 Next
                             End Function
        End If

        Return New pipeline(queryPopulator(), GetType(Query))
    End Function

    <ExportAPI("stream.flush")>
    Public Function flush(stream As Object, data As pipeline, Optional env As Environment = Nothing) As Object
        If stream Is Nothing Then
            Return Internal.debug.stop("No output stream device!", env)
        ElseIf data Is Nothing Then
            Return Internal.debug.stop("No content data provided!", env)
        ElseIf data.elementType Like GetType(BestHit) AndAlso Not TypeOf stream Is WriteStream(Of BestHit) Then
            Return Internal.debug.stop("Unmatched stream device with the incoming data type!", env)
        ElseIf data.elementType Like GetType(BiDirectionalBesthit) AndAlso Not TypeOf stream Is WriteStream(Of BiDirectionalBesthit) Then
            Return Internal.debug.stop("Unmatched stream device with the incoming data type!", env)
        ElseIf data.elementType Like GetType(BlastnMapping) AndAlso Not TypeOf stream Is WriteStream(Of BlastnMapping) Then
            Return Internal.debug.stop("Unmatched stream device with the incoming data type!", env)
        End If

        Select Case data.elementType.raw
            Case GetType(BestHit)
                Call writeStreamHelper(Of BestHit)(stream, data, env)
            Case GetType(BiDirectionalBesthit)
                Call writeStreamHelper(Of BiDirectionalBesthit)(stream, data, env)
            Case GetType(BlastnMapping)
                Call writeStreamHelper(Of BlastnMapping)(stream, data, env)
            Case Else
                Return Internal.debug.stop(New NotImplementedException, env)
        End Select

        Return True
    End Function

    Private Sub writeStreamHelper(Of T As Class)(stream As Object, data As pipeline, env As Environment)
        With DirectCast(stream, WriteStream(Of T))
            For Each hit As T In data.populates(Of T)(env)
                Call .Flush(hit)
            Next

            Call .Flush()
        End With
    End Sub

    <ExportAPI("besthit.filter")>
    Public Function FilterBesthitStream(besthits As pipeline, Optional evalue# = 0.00001, Optional delNohits As Boolean = True, Optional env As Environment = Nothing) As pipeline
        If besthits Is Nothing Then
            Return REnv.Internal.debug.stop("The input stream data is nothing!", env)
        ElseIf Not besthits.elementType Like GetType(BestHit) Then
            Return REnv.Internal.debug.stop($"could not handle the stream data: {besthits.elementType.fullName}", env)
        End If

        Dim filter As Func(Of BestHit, Boolean) =
            Function(hit)
                If delNohits AndAlso hit.HitName = "HITS_NOT_FOUND" Then
                    Return False
                Else
                    Return hit.evalue <= evalue
                End If
            End Function

        Return besthits _
            .populates(Of BestHit)(env) _
            .Where(filter) _
            .DoCall(AddressOf pipeline.CreateFromPopulator)
    End Function

    ''' <summary>
    ''' Open result table stream writer
    ''' </summary>
    ''' <param name="file"></param>
    ''' <param name="type"></param>
    ''' <param name="encoding"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("open.stream")>
    Public Function openWriter(file As String,
                               Optional type As TableTypes = TableTypes.SBH,
                               Optional encoding As Encodings = Encodings.ASCII,
                               Optional ioRead As Boolean = False,
                               Optional env As Environment = Nothing) As Object
        Select Case type
            Case TableTypes.SBH
                If ioRead Then
                    Return file _
                        .OpenHandle(encoding.CodePage) _
                        .AsLinq(Of BestHit) _
                        .DoCall(AddressOf pipeline.CreateFromPopulator)
                Else
                    Return New WriteStream(Of BestHit)(file, encoding:=encoding)
                End If
            Case TableTypes.BBH
                If ioRead Then
                    Return file _
                        .OpenHandle(encoding.CodePage) _
                        .AsLinq(Of BiDirectionalBesthit) _
                        .DoCall(AddressOf pipeline.CreateFromPopulator)
                Else
                    Return New WriteStream(Of BiDirectionalBesthit)(file, encoding:=encoding)
                End If
            Case TableTypes.Mapping
                If ioRead Then
                    Return file _
                       .OpenHandle(encoding.CodePage) _
                       .AsLinq(Of BlastnMapping) _
                       .DoCall(AddressOf pipeline.CreateFromPopulator)
                Else
                    Return New WriteStream(Of BlastnMapping)(file, encoding:=encoding, metaKeys:={})
                End If
            Case Else
                Return REnv.Internal.debug.stop($"Invalid stream formatter: {type.ToString}", env)
        End Select
    End Function
End Module

Public Enum TableTypes
    SBH
    BBH
    ''' <summary>
    ''' blastn mapping of the short reads
    ''' </summary>
    Mapping
End Enum

Public Enum BBHAlgorithm
    Naive
    BHR
    TaxonomySupports
End Enum

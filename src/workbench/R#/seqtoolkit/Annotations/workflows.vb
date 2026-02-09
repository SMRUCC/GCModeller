#Region "Microsoft.VisualBasic::efeecf06cd055cce3005fd3ce1f0a853, R#\seqtoolkit\Annotations\workflows.vb"

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

    '   Total Lines: 338
    '    Code Lines: 268 (79.29%)
    ' Comment Lines: 35 (10.36%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 35 (10.36%)
    '     File Size: 15.77 KB


    ' Module workflows
    ' 
    '     Function: ExportBBHHits, ExportSBHHits, FilterBesthitStream, flush, grepNames
    '               openBlastReader, openWriter, parseBlastnMaps
    ' 
    '     Sub: writeStreamHelper
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ApplicationServices.Debugging.Logging
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.Framework.IO.Linq
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.Interops.NCBI.Extensions
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.NtMapping
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.BLASTOutput
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.BLASTOutput.BlastPlus
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop
Imports REnv = SMRUCC.Rsharp.Runtime
Imports RInternal = SMRUCC.Rsharp.Runtime.Internal

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
    ''' <returns>a collection of the query hits result details</returns>
    <ExportAPI("read.blast")>
    <RApiReturn(GetType(Query))>
    Public Function openBlastReader(file As String,
                                    Optional type As String = "nucl",
                                    Optional fastMode As Boolean = True,
                                    Optional env As Environment = Nothing) As pipeline

        If Not file.FileExists(True) Then
            Return REnv.Internal.debug.stop($"invalid data source: '{file.ToFileURL}'!", env)
        End If

        If LCase(type) = "nucl" Then
            Return BlastPlus.TryParseUltraLarge(file).Queries.DoCall(AddressOf pipeline.CreateFromPopulator)
        ElseIf LCase(type) = "prot" Then
            Return pipeline.CreateFromPopulator(BlastpOutputReader.RunParser(file, fast:=fastMode))
        Else
            Return RInternal.debug.stop($"invalid program type: {type}!", env)
        End If
    End Function

    ''' <summary>
    ''' export results of fastq reads mapping to genome sequence. 
    ''' </summary>
    ''' <param name="query"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("blastn.maphit")>
    Public Function parseBlastnMaps(query As pipeline,
                                    Optional top_best As Boolean = False,
                                    Optional env As Environment = Nothing) As pipeline
        If query Is Nothing Then
            Return Nothing
        ElseIf Not query.elementType Like GetType(Query) Then
            Return REnv.Internal.debug.stop($"invalid pipeline data type: {query.elementType.ToString}", env)
        End If

        Return query.populates(Of Query)(env) _
            .Export(parallel:=False, topBest:=top_best) _
            .DoCall(AddressOf pipeline.CreateFromPopulator)
    End Function

    ''' <summary>
    ''' Export single side besthit
    ''' </summary>
    ''' <param name="query">the blast reader result from the ``read.blast`` iterator function.</param>
    ''' <param name="idetities"></param>
    ''' <param name="coverage"></param>
    ''' <param name="topBest"></param>
    ''' <param name="keepsRawName"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("blasthit.sbh")>
    <Extension>
    <RApiReturn(GetType(BestHit))>
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
    Public Function ExportBBHHits(forward As pipeline, reverse As pipeline,
                                  Optional algorithm As BBHAlgorithm = BBHAlgorithm.Naive,
                                  Optional env As Environment = Nothing) As pipeline

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

                Return BBHParser _
                    .GetBBHTop(
                        qvs:=forward.populates(Of BestHit)(env),
                        svq:=reverse.populates(Of BestHit)(env)
                    ) _
                    .DoCall(AddressOf pipeline.CreateFromPopulator)

            Case BBHAlgorithm.BHR
                Throw New NotImplementedException
            Case BBHAlgorithm.TaxonomySupports
                Throw New NotImplementedException
            Case BBHAlgorithm.HybridBHR
                Return FastMatch _
                    .BinaryMatch(forward.populates(Of BestHit)(env), reverse.populates(Of BestHit)(env)) _
                    .DoCall(AddressOf pipeline.CreateFromPopulator)
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

    ''' <summary>
    ''' Save the annotation rawdata into the given stream file.
    ''' </summary>
    ''' <param name="data"></param>
    ''' <param name="stream">
    ''' a stream data handler that generated via the ``open.stream`` function.
    ''' </param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("stream.flush")>
    Public Function flush(data As pipeline, stream As Object, Optional env As Environment = Nothing) As Object
        If stream Is Nothing Then
            Return RInternal.debug.stop("No output stream device!", env)
        ElseIf data Is Nothing Then
            Return RInternal.debug.stop("No content data provided!", env)
        ElseIf data.elementType Like GetType(BestHit) AndAlso Not TypeOf stream Is WriteStream(Of BestHit) Then
            Return RInternal.debug.stop("Unmatched stream device with the incoming data type!", env)
        ElseIf data.elementType Like GetType(BiDirectionalBesthit) AndAlso Not TypeOf stream Is WriteStream(Of BiDirectionalBesthit) Then
            Return RInternal.debug.stop("Unmatched stream device with the incoming data type!", env)
        ElseIf data.elementType Like GetType(BlastnMapping) AndAlso Not TypeOf stream Is WriteStream(Of BlastnMapping) Then
            Return RInternal.debug.stop("Unmatched stream device with the incoming data type!", env)
        End If

        Select Case data.elementType.raw
            Case GetType(BestHit)
                Call writeStreamHelper(Of BestHit)(stream, data, env)
            Case GetType(BiDirectionalBesthit)
                Call writeStreamHelper(Of BiDirectionalBesthit)(stream, data, env)
            Case GetType(BlastnMapping)
                Call writeStreamHelper(Of BlastnMapping)(stream, data, env)
            Case Else
                Return RInternal.debug.stop(New NotImplementedException, env)
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

    ''' <summary>
    ''' make filter of the blast best hits via the given parameter combinations
    ''' </summary>
    ''' <param name="besthits">is a collection of the blastp/blastn parsed result: <see cref="BestHit"/></param>
    ''' <param name="evalue">new cutoff value of the evalue for make filter of the given hits collection</param>
    ''' <param name="delNohits">removes ``HITS_NOT_FOUND``? default is yes.</param>
    ''' <param name="pickTop">pick the top one hit for each query group?</param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("besthit_filter")>
    Public Function FilterBesthitStream(besthits As pipeline,
                                        Optional evalue As Double? = Nothing,
                                        Optional identities As Double? = Nothing,
                                        Optional delNohits As Boolean = True,
                                        Optional pickTop As Boolean = False,
                                        Optional env As Environment = Nothing) As pipeline
        If besthits Is Nothing Then
            Return REnv.Internal.debug.stop("The input stream data is nothing!", env)
        ElseIf Not besthits.elementType Like GetType(BestHit) Then
            Return REnv.Internal.debug.stop($"could not handle the stream data: {besthits.elementType.fullName}", env)
        End If

        Dim filter_identities As Boolean = Not identities Is Nothing
        Dim filter_evalue As Boolean = Not evalue Is Nothing
        Dim filter As Func(Of BestHit, Boolean) =
            Function(hit)
                If delNohits AndAlso hit.HitName = "HITS_NOT_FOUND" Then
                    Return False
                End If
                If filter_identities AndAlso hit.identities < identities.Value Then
                    Return False
                End If
                If filter_evalue AndAlso hit.evalue > evalue.Value Then
                    Return False
                End If

                Return True
            End Function
        Dim stream As IEnumerable(Of BestHit) = besthits _
            .populates(Of BestHit)(env) _
            .Where(filter)

        If pickTop Then
            Return stream _
                .GroupBy(Function(hit) hit.QueryName) _
                .Select(Function(group)
                            Return group _
                                .OrderByDescending(Function(hit) hit.score) _
                                .First
                        End Function) _
                .DoCall(AddressOf pipeline.CreateFromPopulator)
        Else
            Return pipeline.CreateFromPopulator(stream)
        End If
    End Function

    ''' <summary>
    ''' read the hits data in pipeline stream style
    ''' </summary>
    ''' <param name="file"></param>
    ''' <param name="encoding"></param>
    ''' <returns></returns>
    <ExportAPI("read.besthits")>
    <RApiReturn(GetType(BestHit))>
    Public Function read_besthits(file As String, Optional encoding As Encodings = Encodings.ASCII) As Object
        Return file _
            .OpenHandle(encoding.CodePage) _
            .AsLinq(Of BestHit) _
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
                    Return read_besthits(file, encoding)
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

    <ExportAPI("read_m8")>
    <RApiReturn(GetType(DiamondAnnotation))>
    Public Function read_m8(file As String) As Object
        Return DiamondM8Parser.ParseFile(file).ToArray
    End Function
End Module

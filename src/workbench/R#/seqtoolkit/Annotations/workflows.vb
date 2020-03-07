
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
            Return Internal.debug.stop(New NotImplementedException, env)
        ElseIf LCase(type) = "prot" Then
            Return pipeline.CreateFromPopulator(BlastpOutputReader.RunParser(file, fast:=fastMode))
        Else
            Return Internal.debug.stop($"invalid program type: {type}!", env)
        End If
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
        ElseIf Not query.elementType.raw Is GetType(Query) Then
            Return REnv.Internal.debug.stop($"Invalid pipeline data type: {query.elementType.ToString}", env)
        End If

        Dim hitsPopulator As Func(Of IEnumerable(Of BestHit()))

        If topBest Then
            hitsPopulator = Iterator Function() As IEnumerable(Of BestHit())
                                For Each q As Query In query.populates(Of Query)
                                    Yield {
                                        v228.SBHLines(q, coverage, idetities, keepsRawQueryName:=keepsRawName)(Scan0)
                                    }
                                Next
                            End Function
        Else
            hitsPopulator = Iterator Function() As IEnumerable(Of BestHit())
                                For Each q As Query In query.populates(Of Query)
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

        If forward.elementType Is GetType(Query) Then
            forward = forward.ExportSBHHits(env:=env)
            env.AddMessage($"Best hit result from raw query in forward direction with default parameters.", MSG_TYPES.WRN)
        End If
        If reverse.elementType Is GetType(Query) Then
            reverse = reverse.ExportSBHHits(env:=env)
            env.AddMessage($"Best hit result from raw query in reverse direction with default parameters.", MSG_TYPES.WRN)
        End If

        If Not forward.elementType Is GetType(BestHit) Then
            Return REnv.Internal.debug.stop($"Invalid data type {forward.ToString} in forward direction for create bbh result!", env)
        ElseIf Not reverse.elementType Is GetType(BestHit) Then
            Return REnv.Internal.debug.stop($"Invalid data type {forward.ToString} in reverse direction for create bbh result!", env)
        End If

        Return pipeline.CreateFromPopulator(BBHParser.GetBBHTop(forward.populates(Of BestHit), reverse.populates(Of BestHit)))
    End Function

    <ExportAPI("grep.names")>
    Public Function grepNames(query As pipeline, operators As Object,
                              Optional applyOnHits As Boolean = False,
                              Optional env As Environment = Nothing) As pipeline

        If query Is Nothing Then
            Return Nothing
        ElseIf Not query.elementType.raw Is GetType(Query) Then
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
                                 For Each q As Query In query.populates(Of Query)
                                     For Each hit In q.SubjectHits
                                         hit.Name = grep(hit.Name)
                                     Next

                                     Yield q
                                 Next
                             End Function
        Else
            queryPopulator = Iterator Function() As IEnumerable(Of Query)
                                 For Each q As Query In query.populates(Of Query)
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
        ElseIf data.elementType.raw Is GetType(BestHit) AndAlso Not TypeOf stream Is WriteStream(Of BestHit) Then
            Return Internal.debug.stop("Unmatched stream device with the incoming data type!", env)
        ElseIf data.elementType.raw Is GetType(BiDirectionalBesthit) AndAlso Not TypeOf stream Is WriteStream(Of BiDirectionalBesthit) Then
            Return Internal.debug.stop("Unmatched stream device with the incoming data type!", env)
        ElseIf data.elementType.raw Is GetType(BlastnMapping) AndAlso Not TypeOf stream Is WriteStream(Of BlastnMapping) Then
            Return Internal.debug.stop("Unmatched stream device with the incoming data type!", env)
        End If

        Select Case data.elementType.raw
            Case GetType(BestHit)
                With DirectCast(stream, WriteStream(Of BestHit))
                    For Each hit As BestHit In data.populates(Of BestHit)
                        Call .Flush(hit)
                    Next
                End With
            Case Else
                Return Internal.debug.stop(New NotImplementedException, env)
        End Select

        Return True
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
                               Optional env As Environment = Nothing) As Object
        Select Case type
            Case TableTypes.SBH
                Return New WriteStream(Of BestHit)(file, encoding:=encoding)
            Case TableTypes.BBH
                Return New WriteStream(Of BiDirectionalBesthit)(file, encoding:=encoding)
            Case TableTypes.Mapping
                Return New WriteStream(Of BlastnMapping)(file, encoding:=encoding)
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
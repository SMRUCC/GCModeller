
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ApplicationServices.Debugging.Logging
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv.IO.Linq
Imports Microsoft.VisualBasic.Scripting
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH
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

        Return New pipeline(hitsPopulator(), GetType(Query))
    End Function

    <ExportAPI("blasthit.bbh")>
    Public Function ExportBBHHits(forward As pipeline, reverse As pipeline, Optional env As Environment = Nothing) As pipeline
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
            Case Else
                Return REnv.Internal.debug.stop($"Invalid stream formatter: {type.ToString}", env)
        End Select
    End Function
End Module

Public Enum TableTypes
    SBH
    BBH
End Enum
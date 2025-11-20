
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.SequenceModel.FQ
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Internal.[Object]
Imports SMRUCC.Rsharp.Runtime.Interop
Imports SMRUCC.Rsharp.Runtime.Vectorization

<Package("QC")>
Module QC

    <ExportAPI("trim_low_quality")>
    Public Function TrimLowQuality(<RRawVectorArgument> reads As Object, Optional quality% = 20, Optional env As Environment = Nothing) As Object
        Dim pull As pipeline = pipeline.TryCreatePipeline(Of FQ.FastQ)(reads, env)

        If pull.isError Then
            Return pull.getError
        End If

        Return pipeline.CreateFromPopulator(pull.populates(Of FQ.FastQ)(env).TrimLowQuality)
    End Function

    ''' <summary>
    ''' trim the primers or adapters sequence at read header region
    ''' </summary>
    ''' <param name="reads"></param>
    ''' <param name="headers"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("trim_reads_headers")>
    Public Function TrimReadsHeaders(<RRawVectorArgument> reads As Object, <RRawVectorArgument> headers As Object,
                                     Optional exact_match As Boolean = True,
                                     Optional cutoff As Double = 0.85,
                                     Optional env As Environment = Nothing) As Object

        Dim pull As pipeline = pipeline.TryCreatePipeline(Of FQ.FastQ)(reads, env)

        If pull.isError Then
            Return pull.getError
        End If

        Dim readsPool As IEnumerable(Of FQ.FastQ) = pull.populates(Of FQ.FastQ)(env)

        If exact_match Then
            Return pipeline.CreateFromPopulator(readsPool.TrimPrimersAndAdapters(CLRVector.asCharacter(headers)))
        Else
            Return pipeline.CreateFromPopulator(readsPool.TrimPrimersSmithWaterman(CLRVector.asCharacter(headers), minScore:=0, cutoff:=cutoff))
        End If
    End Function

End Module

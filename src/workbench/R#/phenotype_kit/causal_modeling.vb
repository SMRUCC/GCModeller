Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.GraphTheory
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Internal.[Object]
Imports SMRUCC.Rsharp.Runtime.Interop
Imports SMRUCC.Rsharp.Runtime.Vectorization

<Package("causal_modeling")>
Module causal_modeling

    <ExportAPI("sem")>
    Public Function sem(data As Object, <RRawVectorArgument> path As Object, Optional env As Environment = Nothing) As Object
        Dim pull As pipeline = pipeline.TryCreatePipeline(Of SparseGraph.IInteraction)(path, env)

        If pull.isError Then
            Return pull.getError
        End If


    End Function

    <ExportAPI("make_path")>
    <RApiReturn(GetType(SparseGraph.Edge))>
    Public Function make_path(<RRawVectorArgument> from As Object, <RRawVectorArgument> [to] As Object, Optional env As Environment = Nothing) As Object
        Dim fromNodes As GetVectorElement = GetVectorElement.Create(Of String)(CLRVector.asCharacter(from))
        Dim toNodes As GetVectorElement = GetVectorElement.Create(Of String)(CLRVector.asCharacter([to]))
        Dim edges As New List(Of SparseGraph.Edge)

        For Each itr In GetVectorElement.Zip(fromNodes, toNodes)
            Call edges.Add(New SparseGraph.Edge With {
                .u = CStr(itr.Item1),
                .v = CStr(itr.Item2)
            })
        Next

        Return edges.ToArray
    End Function

End Module

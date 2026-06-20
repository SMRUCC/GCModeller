Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.GraphTheory
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Analysis.Microarray
Imports SMRUCC.Rsharp
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Internal.[Object]
Imports SMRUCC.Rsharp.Runtime.Interop
Imports SMRUCC.Rsharp.Runtime.Vectorization
Imports Matrix = SMRUCC.genomics.Analysis.HTS.DataFrame.Matrix

<Package("causal_modeling")>
Module causal_modeling

    Sub Main()

    End Sub

    Private Function castGraph()

    End Function

    <ExportAPI("as_causalmodel")>
    <RApiReturn(GetType(CausalModel))>
    Public Function as_causalmodel(data As Object, <RRawVectorArgument> path As Object, Optional env As Environment = Nothing) As Object
        Dim pull As pipeline = pipeline.TryCreatePipeline(Of SparseGraph.IInteraction)(path, env)
        Dim expr As Object = geneExpression.loadExpression(data, env:=env)

        If TypeOf expr Is Message Then
            Return expr
        End If
        If pull.isError Then
            Return pull.getError
        End If

        Dim paths = pull.populates(Of SparseGraph.IInteraction)(env)
        Dim model As CausalModel = CausalModel.Create(x:=DirectCast(expr, Matrix), paths)

        Return model
    End Function

    <ExportAPI("sem")>
    <RApiReturn("sem_result", "sem_boot")>
    Public Function sem_tool(model As CausalModel, Optional boot As Integer = 500, Optional env As Environment = Nothing) As Object
        Dim semResult As SEMResult = SEM.FitPathAnalysis(model)
        Dim semBoot As BootstrapResult = SEM.BootstrapSEM(model, numBoot:=boot, seed:=123)

        Call SEM.PrintSEMResult(semResult, semBoot)

        Return New list(slot("sem_result") = semResult,
                        slot("sem_boot") = semBoot)
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

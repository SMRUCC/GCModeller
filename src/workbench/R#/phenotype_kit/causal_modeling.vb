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

    <ExportAPI("path_coefficient")>
    <RApiReturn(GetType(dataframe), GetType(PathCoefficient))>
    Public Function path_coefficient(sem_result As SEMResult, Optional as_dataframe As Boolean = True) As Object
        Dim pc As PathCoefficient() = PathCoefficient.FromResult(sem_result).ToArray

        If as_dataframe Then
            Dim df As New dataframe With {.columns = New Dictionary(Of String, Array)}

            Call df.add(NameOf(PathCoefficient.fromName), From i As PathCoefficient In pc Select i.fromName)
            Call df.add(NameOf(PathCoefficient.toName), From i As PathCoefficient In pc Select i.toName)
            Call df.add(NameOf(PathCoefficient.coef), From i As PathCoefficient In pc Select i.coef)
            Call df.add(NameOf(PathCoefficient.se), From i As PathCoefficient In pc Select i.se)
            Call df.add(NameOf(PathCoefficient.t), From i As PathCoefficient In pc Select i.t)
            Call df.add(NameOf(PathCoefficient.p), From i As PathCoefficient In pc Select i.p)
            Call df.add(NameOf(PathCoefficient.sig), From i As PathCoefficient In pc Select i.sig)

            Return df
        Else
            Return pc
        End If
    End Function

    <ExportAPI("effect_decomposition")>
    <RApiReturn(GetType(dataframe), GetType(EffectDecomposition))>
    Public Function effect_decomposition(sem_result As SEMResult, Optional as_dataframe As Boolean = True) As Object
        Dim ed As EffectDecomposition() = EffectDecomposition.FromResult(sem_result).ToArray

        If as_dataframe Then
            Dim df As New dataframe With {.columns = New Dictionary(Of String, Array)}

            Call df.add(NameOf(EffectDecomposition.fromName), From i As EffectDecomposition In ed Select i.fromName)
            Call df.add(NameOf(EffectDecomposition.toName), From i As EffectDecomposition In ed Select i.toName)
            Call df.add(NameOf(EffectDecomposition.direct), From i As EffectDecomposition In ed Select i.direct)
            Call df.add(NameOf(EffectDecomposition.indirect), From i As EffectDecomposition In ed Select i.indirect)
            Call df.add(NameOf(EffectDecomposition.total), From i As EffectDecomposition In ed Select i.total)

            Return df
        Else
            Return ed
        End If
    End Function

    <ExportAPI("significance_test")>
    <RApiReturn(GetType(dataframe), GetType(BootstrapSignificanceTest))>
    Public Function significance_test(sem_result As SEMResult, boot_result As BootstrapResult, Optional as_dataframe As Boolean = True) As Object
        Dim bs As BootstrapSignificanceTest() = BootstrapSignificanceTest.FromResult(sem_result, boot_result).ToArray

        If as_dataframe Then
            Dim df As New dataframe With {.columns = New Dictionary(Of String, Array)}

            Call df.add(NameOf(BootstrapSignificanceTest.fromName), From i As BootstrapSignificanceTest In bs Select i.fromName)
            Call df.add(NameOf(BootstrapSignificanceTest.toName), From i As BootstrapSignificanceTest In bs Select i.toName)
            Call df.add(NameOf(BootstrapSignificanceTest.coef), From i As BootstrapSignificanceTest In bs Select i.coef)
            Call df.add(NameOf(BootstrapSignificanceTest.bse), From i As BootstrapSignificanceTest In bs Select i.bse)
            Call df.add(NameOf(BootstrapSignificanceTest.ci_lb), From i As BootstrapSignificanceTest In bs Select i.ci_lb)
            Call df.add(NameOf(BootstrapSignificanceTest.ci_ub), From i As BootstrapSignificanceTest In bs Select i.ci_ub)
            Call df.add(NameOf(BootstrapSignificanceTest.sig), From i As BootstrapSignificanceTest In bs Select i.sig)

            Return df
        Else
            Return bs
        End If
    End Function

    <ExportAPI("indirect_effect")>
    <RApiReturn(GetType(dataframe), GetType(IndirectEffectBootstrap))>
    Public Function indirect_effect(sem_result As SEMResult, boot_result As BootstrapResult, Optional as_dataframe As Boolean = True) As Object
        Dim ib As IndirectEffectBootstrap() = IndirectEffectBootstrap.FromResult(sem_result, boot_result).ToArray

        If as_dataframe Then
            Dim df As New dataframe With {.columns = New Dictionary(Of String, Array)}

            Call df.add(NameOf(IndirectEffectBootstrap.fromName), From i As IndirectEffectBootstrap In ib Select i.fromName)
            Call df.add(NameOf(IndirectEffectBootstrap.toName), From i As IndirectEffectBootstrap In ib Select i.toName)
            Call df.add(NameOf(IndirectEffectBootstrap.indirectEffect), From i As IndirectEffectBootstrap In ib Select i.indirectEffect)
            Call df.add(NameOf(IndirectEffectBootstrap.bse), From i As IndirectEffectBootstrap In ib Select i.bse)
            Call df.add(NameOf(IndirectEffectBootstrap.ci_lb), From i As IndirectEffectBootstrap In ib Select i.ci_lb)
            Call df.add(NameOf(IndirectEffectBootstrap.ci_ub), From i As IndirectEffectBootstrap In ib Select i.ci_ub)
            Call df.add(NameOf(IndirectEffectBootstrap.sig), From i As IndirectEffectBootstrap In ib Select i.sig)

            Return df
        Else
            Return ib
        End If
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
    Public Function make_path(<RListObjectArgument>
                              Optional paths As list = Nothing,
                              <RRawVectorArgument(TypeCodes.string)> Optional from As Object = Nothing,
                              <RRawVectorArgument(TypeCodes.string)> Optional [to] As Object = Nothing,
                              Optional env As Environment = Nothing) As Object

        Dim edges As New List(Of SparseGraph.Edge)

        If from Is Nothing AndAlso [to] Is Nothing Then
            For Each name As String In paths.getNames
                Dim uv As String() = CLRVector.asCharacter(paths.getByName(name))
                Dim path As New SparseGraph.Edge With {
                    .u = uv(0),
                    .v = uv(1)
                }

                Call edges.Add(path)
            Next
        Else
            Dim fromNodes As GetVectorElement = GetVectorElement.Create(Of String)(CLRVector.asCharacter(from))
            Dim toNodes As GetVectorElement = GetVectorElement.Create(Of String)(CLRVector.asCharacter([to]))

            For Each itr In GetVectorElement.Zip(fromNodes, toNodes)
                Call edges.Add(New SparseGraph.Edge With {
                    .u = CStr(itr.Item1),
                    .v = CStr(itr.Item2)
                })
            Next
        End If

        Return edges.ToArray
    End Function

End Module

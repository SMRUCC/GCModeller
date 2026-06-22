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
<RTypeExport("latent_var", GetType(LatentSymbol))>
Module causal_modeling

    Sub Main()

    End Sub

    <ExportAPI("measurement_model")>
    <RApiReturn(GetType(dataframe), GetType(MeasurementModel))>
    Public Function measurement_model(result As PLSPMResult, Optional as_dataframe As Boolean = True) As Object
        Dim mm = MeasurementModel.FromResult(result).ToArray

        If as_dataframe Then
            Dim df As New dataframe With {.columns = New Dictionary(Of String, Array)}

            Call df.add(NameOf(MeasurementModel.latentName), From i As MeasurementModel In mm Select i.latentName)
            Call df.add(NameOf(MeasurementModel.mode), From i As MeasurementModel In mm Select i.mode)
            Call df.add(NameOf(MeasurementModel.manifest_variable), From i As MeasurementModel In mm Select i.manifest_variable)
            Call df.add(NameOf(MeasurementModel.loading), From i As MeasurementModel In mm Select i.loading)
            Call df.add(NameOf(MeasurementModel.w), From i As MeasurementModel In mm Select i.w)
            Call df.add(NameOf(MeasurementModel.communality), From i As MeasurementModel In mm Select i.communality)
            Call df.add(NameOf(MeasurementModel.block_communality), From i As MeasurementModel In mm Select i.block_communality)

            Return df
        Else
            Return mm
        End If
    End Function

    <ExportAPI("endogenous_latents")>
    <RApiReturn(GetType(dataframe), GetType(EndogenousLatentVariable))>
    Public Function endogenous_latents(result As PLSPMResult, Optional as_dataframe As Boolean = True) As Object
        Dim el = EndogenousLatentVariable.FromResult(result).ToArray

        If as_dataframe Then
            Dim df As New dataframe With {.columns = New Dictionary(Of String, Array)}

            Call df.add(NameOf(EndogenousLatentVariable.latentName), From i As EndogenousLatentVariable In el Select i.latentName)
            Call df.add(NameOf(EndogenousLatentVariable.r2), From i As EndogenousLatentVariable In el Select i.r2)
            Call df.add(NameOf(EndogenousLatentVariable.communality), From i As EndogenousLatentVariable In el Select i.communality)
            Call df.add(NameOf(EndogenousLatentVariable.redundancy), From i As EndogenousLatentVariable In el Select i.redundancy)

            Return df
        Else
            Return el
        End If
    End Function

    <ExportAPI("path_coefficient")>
    <RApiReturn(GetType(dataframe), GetType(PathCoefficient))>
    Public Function path_coefficient(result As Object, Optional as_dataframe As Boolean = True, Optional env As Environment = Nothing) As Object
        Dim pc As PathCoefficient()

        If result Is Nothing Then
            Return Nothing
        End If

        If TypeOf result Is SEMResult Then
            pc = PathCoefficient.FromResult(DirectCast(result, SEMResult)).ToArray
        ElseIf TypeOf result Is PLSPMResult Then
            pc = PathCoefficient.FromResult(DirectCast(result, PLSPMResult)).ToArray
        Else
            Return Message.InCompatibleType(GetType(SEMResult), result.GetType, env)
        End If

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
    Public Function effect_decomposition(result As Object, Optional as_dataframe As Boolean = True, Optional env As Environment = Nothing) As Object
        Dim ed As EffectDecomposition()

        If TypeOf result Is SEMResult Then
            ed = EffectDecomposition.FromResult(DirectCast(result, SEMResult)).ToArray
        ElseIf TypeOf result Is PLSPMResult Then
            ed = EffectDecomposition.FromResult(DirectCast(result, PLSPMResult)).ToArray
        Else
            Return Message.InCompatibleType(GetType(SEMResult), result.GetType, env)
        End If

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
    Public Function significance_test(result As Object, boot_result As BootstrapResult,
                                      Optional as_dataframe As Boolean = True,
                                      Optional env As Environment = Nothing) As Object

        Dim bs As BootstrapSignificanceTest()

        If TypeOf result Is SEMResult Then
            bs = BootstrapSignificanceTest.FromResult(DirectCast(result, SEMResult), boot_result).ToArray
        ElseIf TypeOf result Is PLSPMResult Then
            bs = BootstrapSignificanceTest.FromResult(
                DirectCast(result, PLSPMResult),
                DirectCast(boot_result, PLSPMBootstrapResult)).ToArray
        Else
            Return Message.InCompatibleType(GetType(SEMResult), result.GetType, env)
        End If

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
    Public Function indirect_effect(result As Object, boot_result As BootstrapResult,
                                    Optional as_dataframe As Boolean = True,
                                    Optional env As Environment = Nothing) As Object

        Dim ib As IndirectEffectBootstrap()

        If TypeOf result Is SEMResult Then
            ib = IndirectEffectBootstrap.FromResult(DirectCast(result, SEMResult), boot_result).ToArray
        ElseIf TypeOf result Is PLSPMResult Then
            ib = IndirectEffectBootstrap.FromResult(
                DirectCast(result, PLSPMResult),
                DirectCast(boot_result, PLSPMBootstrapResult)).ToArray
        Else
            Return Message.InCompatibleType(GetType(SEMResult), result.GetType, env)
        End If

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
    Public Function as_causalmodel(data As Object, <RRawVectorArgument> path As Object, <RRawVectorArgument> Optional latents As Object = Nothing, Optional env As Environment = Nothing) As Object
        Dim pathList As PipeIterator(Of SparseGraph.IInteraction) = pipeline.Stream(Of SparseGraph.IInteraction)(path, env)
        Dim expr As Object = geneExpression.loadExpression(data, env:=env)
        Dim latentVars As PipeIterator(Of LatentDefinition) = pipeline.Stream(Of LatentDefinition)(latents, env, nullPipe:=True)

        If TypeOf expr Is Message Then
            Return expr
        End If
        If pathList.isError Then
            Return pathList.getError
        End If
        If latentVars.isError Then
            Return latentVars.getError
        End If

        Dim model As CausalModel

        If latentVars.IsNullOrEmpty Then
            model = CausalModel.Create(x:=DirectCast(expr, Matrix), pathList)
        Else
            model = CausalModel.Create(x:=DirectCast(expr, Matrix), pathList, latentVars)
        End If

        Return model
    End Function

    <ExportAPI("sem")>
    <RApiReturn("sem_result", "sem_boot")>
    Public Function sem_tool(model As CausalModel, Optional boot As Integer = 500, Optional env As Environment = Nothing) As Object
        Dim semResult As SEMResult = SEM.FitPathAnalysis(model)
        Dim semBoot As BootstrapResult = SEM.BootstrapSEM(model, numBoot:=boot, seed:=123)

        ' Call SEM.PrintSEMResult(semResult, semBoot)

        Return New list(slot("sem_result") = semResult,
                        slot("sem_boot") = semBoot)
    End Function

    <ExportAPI("plspm")>
    <RApiReturn("plspm_result", "plspm_boot")>
    Public Function plspm_tool(model As CausalModel, Optional boot As Integer = 500, Optional env As Environment = Nothing) As Object
        Dim plspmResult = PLSPM.FitPLSPM(model)
        Dim plspmBoot = PLSPM.BootstrapPLSPM(model, numBoot:=boot, seed:=456)

        Console.WriteLine("模型整体拟合：")
        Console.WriteLine($"  GoF (Goodness of Fit) = {plspmResult.GoF:F4}  (良好 > 0.36, 中等 > 0.25, 弱 > 0.10)")
        Console.WriteLine()

        Return New list(slot("plspm_result") = plspmResult,
                       slot("plspm_boot") = plspmBoot)
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

    ''' <summary>
    ''' make full connected path for PLS-PM latent symbols
    ''' </summary>
    ''' <param name="manifest"></param>
    ''' <param name="from">class name of the from node</param>
    ''' <param name="to">class name of the to node</param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("make_full_path")>
    Public Function make_full_path(<RRawVectorArgument(GetType(LatentSymbol))> manifest As Object,
                                   <RRawVectorArgument(TypeCodes.string)> from As Object,
                                   <RRawVectorArgument(TypeCodes.string)> [to] As Object,
                                   Optional env As Environment = Nothing) As Object

        Dim manifest_vars As PipeIterator(Of LatentSymbol) = pipeline.Stream(Of LatentSymbol)(manifest, env)

        If manifest_vars.isError Then
            Return manifest_vars.getError
        End If

        Dim cls_from As String() = CLRVector.asCharacter(from)
        Dim cls_to As String() = CLRVector.asCharacter([to])
        Dim network = LatentSymbol.MakeFullPath(manifest_vars, cls_from, cls_to).ToArray

        Return network
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="manifest_names">
    ''' a set of the feature id for make the manifest symbol to the latent definition or a vector of the <see cref="LatentSymbol"/>
    ''' </param>
    ''' <param name="latent_name"></param>
    ''' <param name="mode"></param>
    ''' <returns></returns>
    <ExportAPI("make_latent")>
    <RApiReturn(GetType(LatentDefinition))>
    Public Function make_latent(<RRawVectorArgument> manifest_names As Object,
                                Optional latent_name As String = Nothing,
                                Optional mode As MeasurementModels = MeasurementModels.A,
                                Optional env As Environment = Nothing) As Object

        If latent_name.StringEmpty(, True) Then
            Dim vars As PipeIterator(Of LatentSymbol) = pipeline.Stream(Of LatentSymbol)(manifest_names, env)

            If vars.isError Then
                Return vars.getError
            Else
                Return LatentSymbol.MakeLatents(vars.AsEnumerable).ToArray
            End If
        Else
            Return New LatentDefinition(latent_name, CLRVector.asCharacter(manifest_names), mode)
        End If
    End Function

End Module

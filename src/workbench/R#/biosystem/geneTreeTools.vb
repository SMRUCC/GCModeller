Imports System.IO
Imports Microsoft.VisualBasic.ApplicationServices.Debugging
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Analysis.GeneTree
Imports SMRUCC.genomics.Analysis.HTS.DataFrame
Imports SMRUCC.genomics.Model.Network.Regulons.ARACNE
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop
Imports RInternal = SMRUCC.Rsharp.Runtime.Internal

<Package("geneTree")>
<RTypeExport("xgboost_grn", GetType(XGBoostGRN))>
Module geneTreeTools

    Sub Main()
        Call RInternal.generic.add("writeBin", GetType(XGBoostGRN), AddressOf SaveModelZip)
        Call RInternal.generic.add("readBin.xgboost_grn", GetType(Stream), AddressOf LoadModelZip)
    End Sub

    Private Function SaveModelZip(model As XGBoostGRN, args As list, env As Environment) As Object
        Dim con As Stream = args!con
        Call model.Save(con)
        Call con.Flush()
        Return True
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="s"></param>
    ''' <param name="args"></param>
    ''' <param name="env"></param>
    ''' <returns>
    ''' <see cref="XGBoostGRN"/>
    ''' </returns>
    Private Function LoadModelZip(s As Stream, args As list, env As Environment) As Object
        Return XGBoostGRN.Load(s)
    End Function

    <ExportAPI("make_profile")>
    Public Function makeConfig(Optional profile As GeneTreeProfile = GeneTreeProfile.Fast,
                               Optional population As Integer = 100,
                               Optional iterations As Integer = 60,
                               Optional cells As Integer = 0,
                               Optional max_genes As Integer = 0,
                               Optional max_regulators As Integer = -1) As GeneTreeConfig

        Dim config = GeneTreeConfig.FromProfile(profile)

        config.PopulationSize = population
        config.MaxIterations = iterations
        config.Verbose = True

        If cells > 0 Then
            config.MaxSamplesPerModel = cells
        End If
        If max_genes > 0 Then
            config.MaxGenes = max_genes
        End If
        If max_regulators >= 0 Then
            config.MaxRegulators = max_regulators
        End If

        Call config.ValidateOrThrow()

        Return config
    End Function

    <ExportAPI("load_topology")>
    Public Function load_topology(aracne As AracneNetwork, Optional config As GeneTreeConfig = Nothing) As GeneRegulationGraph
        Return GeneRegulationGraph.FromAracne(aracne, config)
    End Function

    <ExportAPI("load_expression")>
    Public Function load_expression(graph As GeneRegulationGraph, expr As Matrix, Optional config As GeneTreeConfig = Nothing) As XGBoostGRN
        Call DebugAudit.Mark("expression", $"{expr.size} genes x {expr.sample_count} samples")
        Call DebugAudit.Test(expr.size > 0 AndAlso expr.sample_count > 0, "表达矩阵为空")

        ' ---- 3. 拟合 ----
        Dim grn As XGBoostGRN = XGBoostGRN.Open(graph, expr, config)
        Return grn
    End Function

    <ExportAPI("fit_tree")>
    Public Function fit_tree(tree As XGBoostGRN) As TrainingReport
        Dim report As TrainingReport = tree.Fit
        Call DebugAudit.Mark("fit", report.ToString)
        Call DebugAudit.Test(tree.IsFitted, "模型应当已经完成拟合")
        Call DebugAudit.Test(report.BoostedModels > 0, "应当至少训练出一个梯度提升树模型")
        Call DebugAudit.Test(report.InformativeModels > 0, "应当存在 R² > 0.05 的模型，否则说明网络拓扑与表达数据不匹配")
        Return report
    End Function

    <ExportAPI("write_report")>
    Public Function write_report(report As TrainingReport, folder As String)
        Call report.Write(folder)
    End Function
End Module

Imports SMRUCC.genomics.Analysis.BNLearn
Imports SMRUCC.genomics.Analysis.BNLearn.DBN

''' <summary>
''' 采用动态贝叶斯网络所构建的基因表达调控网络系统
''' </summary>
Public Class GeneRegulatoryNetwork : Inherits SubNetwork

    ReadOnly GRN As DynamicBayesianNetwork

    Sub New(cell As VirtualCella, network As IEnumerable(Of RegulatoryLink), Optional config As DBNConfig = Nothing)
        Call MyBase.New(cell)

        GRN = New DynamicBayesianNetwork(If(config, New DBNConfig))
        GRN.BuildFromTopology(network)
    End Sub

    Public Overrides Sub RunStep()
        Dim metabolites = cell.metabolic.GetStats
        Dim proteins = cell.translation.GetStats
        Dim statsNext = GRN.PredictNextState(metabolites, proteins)
        Dim transcriptionRates = statsNext.RNAAbundanceChanges

    End Sub

    Public Overrides Function GetStats() As Dictionary(Of String, Double)
        Throw New NotImplementedException()
    End Function
End Class

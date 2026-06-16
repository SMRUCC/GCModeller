
Imports SMRUCC.genomics.Analysis.BNLearn.Core

''' <summary>
''' 采用动态贝叶斯网络所构建的基因表达调控网络系统
''' </summary>
Public Class GeneRegulatoryNetwork : Inherits SubNetwork

    ReadOnly GRN As BNLearnWorkflow

    Public Overrides Sub RunStep(cell As VirtualCella)
        Throw New NotImplementedException()
    End Sub
End Class

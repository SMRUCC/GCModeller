Imports Microsoft.VisualBasic.Math.DataFrame
Imports SMRUCC.genomics.Analysis.HTS.DataFrame
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Math.LinearAlgebra.Matrix
Imports Microsoft.VisualBasic.Math.LinearAlgebra

Public Module Analysis

    Public Function Run(samples As Matrix) As Result
        Dim cor As CorrelationMatrix = samples.Correlation(Function(gene) gene.experiments)
        Dim betaList As Double() = seq(1, 30, 0.5).ToArray
        Dim beta As Double = BetaTest.Best(cor, betaList)
        Dim network As GeneralMatrix = cor.WeightedCorrelation(beta, pvalue:=False)
        Dim K As New Vector(network.RowApply(AddressOf WeightedNetwork.sumK))
        Dim tomMat As GeneralMatrix = TOM.Matrix(network, K)
    End Function
End Module

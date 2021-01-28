Imports Microsoft.VisualBasic.Math.DataFrame
Imports SMRUCC.genomics.Analysis.HTS.DataFrame

Public Module Analysis

    Public Function Run(samples As Matrix) As Result
        Dim cor As CorrelationMatrix = samples.Correlation(Function(gene) gene.experiments)

    End Function
End Module

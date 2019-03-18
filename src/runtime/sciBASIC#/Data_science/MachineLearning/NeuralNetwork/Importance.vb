Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Math.Quantile
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports Microsoft.VisualBasic.Math

Namespace NeuralNetwork

    Partial Module Helpers

        ''' <summary>
        ''' 计算输入节点的重要性
        ''' </summary>
        ''' <param name="model"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' 1. 首先将突触链接的权重的绝对值进行quantile计算
        ''' 2. 然后删除20% quantile以下的所有节点连接
        ''' 3. 从input开始添加权重直到某一个输出节点
        ''' 4. 某一个input的权重除以最大的权重,得到归一化的相对值
        ''' </remarks>
        <Extension>
        Public Iterator Function Importance(model As StoreProcedure.NeuralNetwork, Optional q# = 0.2) As IEnumerable(Of DataSet)
            Dim edges = model.connections.Shadows
            Dim allWeights As Vector = edges!w.Abs
            Dim quantile As QuantileEstimationGK = allWeights.GKQuantile
            Dim threshold# = quantile.Query(q)

            ' edge cutoff
            model.connections = edges(allWeights >= threshold)

            ' 枚举每一个output
            For i As Integer = 0 To model.outputlayer.size - 1

            Next
        End Function
    End Module
End Namespace
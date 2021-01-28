Imports Microsoft.VisualBasic.Math.DataFrame
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports Microsoft.VisualBasic.Math.LinearAlgebra.Matrix

Public Module WeighedNetwork

    ''' <summary>
    ''' 得到权重关联网络A
    ''' </summary>
    ''' <param name="cor">
    ''' ``cor(gi, gj)``
    ''' </param>
    ''' <param name="betaPow">
    ''' 权重
    ''' </param>
    ''' <returns></returns>
    Public Function WeightedCorrelation(cor As CorrelationMatrix, betaPow As Double) As GeneralMatrix
        Dim abs As GeneralMatrix = CType(cor, GeneralMatrix).Abs
        Dim A As GeneralMatrix = abs ^ betaPow

        Return A
    End Function

    ''' <summary>
    ''' 连通度K
    ''' </summary>
    ''' <param name="cor"></param>
    ''' <param name="betaPow"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' 连接度ki表示第 i 个基因和其他基因的α值加和
    ''' </remarks>
    Public Function Connectivity(cor As CorrelationMatrix, betaPow As Double) As Vector
        Dim A As GeneralMatrix = WeightedCorrelation(cor, betaPow)
        ' 基因自己与自己的相关度为1，1^betaRow的计算结果仍然是1
        ' 因为计算公式里面要求i<>j
        ' 在这里每一个基因的连通度减掉1表示排除掉自己与自己的相关度
        Dim K As New Vector(A.RowApply(Function(r) r.Sum))

        Return K - 1
    End Function
End Module

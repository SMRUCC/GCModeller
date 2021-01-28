Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Math.DataFrame
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports Microsoft.VisualBasic.Math.LinearAlgebra.Matrix

''' <summary>
''' Category 1: Functions for network construction
''' </summary>
Public Module WeightedNetwork

    <Extension>
    Friend Function Adjacency(cor As GeneralMatrix, threshold As Double) As GeneralMatrix
        Dim adj As GeneralMatrix = cor.Copy
        Dim X As Double()() = adj.Array

        For i As Integer = 0 To X.Length - 1
            For j As Integer = 0 To X(Scan0).Length - 1
                If X(i)(j) < threshold Then
                    X(i)(j) = 0
                End If
            Next
        Next

        Return adj
    End Function

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
    ''' <remarks>
    ''' 1. a trait-based node significance measure can be defined as the absolute 
    '''    value of the correlation between the i-th node profile x i and the 
    '''    sample trait
    ''' 2. alternatively, a correlation test p-value or a regression-based p-value for 
    '''    assessing the statistical significance between x i and the sample trait T 
    '''    can be used to define a p-value based node significance measure
    ''' </remarks>
    <Extension>
    Public Function WeightedCorrelation(cor As CorrelationMatrix, betaPow As Double, Optional pvalue As Boolean = False) As GeneralMatrix
        ' The default method defines the coexpression
        ' Similarity sij as the absolute value of the correlation
        ' coefficient between the profiles of nodes i And j
        Dim S As GeneralMatrix = If(pvalue, -(cor.GetPvalueMatrix.Log(newBase:=10)), CType(cor, GeneralMatrix)).Abs
        Dim A As GeneralMatrix = S ^ betaPow

        Return A
    End Function

    ''' <summary>
    ''' 连通度K
    ''' </summary>
    ''' <param name="cor">
    ''' A network is fully specified by its adjacency matrix aij, a
    ''' symmetric n × n matrix With entries In [0, 1] whose component
    ''' aij encodes the network connection strength
    ''' between nodes i And j.
    ''' </param>
    ''' <param name="betaPow"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' 连接度ki表示第 i 个基因和其他基因的α值加和
    ''' </remarks>
    Public Function Connectivity(cor As CorrelationMatrix, betaPow As Double, adjacency As Double, Optional pvalue As Boolean = False) As Vector
        Dim A As GeneralMatrix = cor.WeightedCorrelation(betaPow, pvalue).Adjacency(adjacency)
        Dim K As New Vector(A.RowApply(AddressOf sumK))

        Return K
    End Function

    Friend Function sumK(r As Double(), i As Integer) As Double
        Dim sum As Double = 0

        For j As Integer = 0 To r.Length - 1
            If i <> j Then
                sum += r(j)
            End If
        Next

        Return sum
    End Function
End Module

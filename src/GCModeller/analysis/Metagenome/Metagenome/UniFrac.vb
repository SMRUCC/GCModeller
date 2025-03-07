Imports System
Imports System.Collections.Generic
Imports System.Linq

Public Module UniFracCalculator

    ' 定义系统发育树分支结构
    Public Class PhylogeneticBranch
        Public Property BranchLength As Double   ' 分支的进化距离
        Public Property OTUIDs As List(Of Integer)  ' 该分支下的OTU索引列表
    End Class

    ''' <summary>
    ''' 计算未加权UniFrac距离
    ''' </summary>
    ''' <param name="sample1">样本1的OTU丰度数组</param>
    ''' <param name="sample2">样本2的OTU丰度数组</param>
    ''' <param name="branches">系统发育树分支列表</param>
    Public Function UnweightedUniFrac(sample1 As Double(), sample2 As Double(), branches As List(Of PhylogeneticBranch)) As Double
        ValidateInput(sample1, sample2, branches)

        Dim totalLength As Double = branches.Sum(Function(b) b.BranchLength)
        If totalLength = 0 Then Return 0.0

        Dim uniqueBranchLength As Double = 0.0

        For Each branch In branches
            Dim hasSample1 = branch.OTUIDs.Any(Function(otu) sample1(otu) > 0)
            Dim hasSample2 = branch.OTUIDs.Any(Function(otu) sample2(otu) > 0)

            ' 判断是否独占分支
            If (hasSample1 Xor hasSample2) Then
                uniqueBranchLength += branch.BranchLength
            End If
        Next

        Return uniqueBranchLength / totalLength
    End Function

    ''' <summary>
    ''' 计算加权UniFrac距离
    ''' </summary>
    Public Function WeightedUniFrac(sample1 As Double(), sample2 As Double(), branches As List(Of PhylogeneticBranch)) As Double
        ValidateInput(sample1, sample2, branches)

        Dim numerator As Double = 0.0
        Dim denominator As Double = 0.0

        For Each branch In branches
            ' 计算分支下的总丰度
            Dim sum1 As Double = branch.OTUIDs.Sum(Function(otu) sample1(otu))
            Dim sum2 As Double = branch.OTUIDs.Sum(Function(otu) sample2(otu))

            numerator += branch.BranchLength * Math.Abs(sum1 - sum2)
            denominator += branch.BranchLength * (sum1 + sum2)
        Next

        If denominator = 0 Then Return 0.0
        Return numerator / denominator
    End Function

    Private Sub ValidateInput(sample1 As Double(), sample2 As Double(), branches As List(Of PhylogeneticBranch))
        If sample1.Length <> sample2.Length Then
            Throw New ArgumentException("样本OTU数量必须相同")
        End If

        Dim maxOTUID = branches.SelectMany(Function(b) b.OTUIDs).Max()
        If maxOTUID >= sample1.Length Then
            Throw New ArgumentException("分支包含无效OTU索引")
        End If
    End Sub

End Module
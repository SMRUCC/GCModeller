#Region "Microsoft.VisualBasic::205f8cbf9ba43120d4cb54f1b6046ead, analysis\Metagenome\Metagenome\UniFrac.vb"

' Author:
' 
'       asuka (amethyst.asuka@gcmodeller.org)
'       xie (genetics@smrucc.org)
'       xieguigang (xie.guigang@live.com)
' 
' Copyright (c) 2018 GPL3 Licensed
' 
' 
' GNU GENERAL PUBLIC LICENSE (GPL3)
' 
' 
' This program is free software: you can redistribute it and/or modify
' it under the terms of the GNU General Public License as published by
' the Free Software Foundation, either version 3 of the License, or
' (at your option) any later version.
' 
' This program is distributed in the hope that it will be useful,
' but WITHOUT ANY WARRANTY; without even the implied warranty of
' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
' GNU General Public License for more details.
' 
' You should have received a copy of the GNU General Public License
' along with this program. If not, see <http://www.gnu.org/licenses/>.



' /********************************************************************************/

' Summaries:


' Code Statistics:

'   Total Lines: 73
'    Code Lines: 45 (61.64%)
' Comment Lines: 12 (16.44%)
'    - Xml Docs: 75.00%
' 
'   Blank Lines: 16 (21.92%)
'     File Size: 2.77 KB


' Module UniFracCalculator
' 
'     Function: UnweightedUniFrac, WeightedUniFrac
' 
'     Sub: ValidateInput
'     Class PhylogeneticBranch
' 
'         Properties: BranchLength, OTUIDs
' 
' 
' 
' /********************************************************************************/

#End Region


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

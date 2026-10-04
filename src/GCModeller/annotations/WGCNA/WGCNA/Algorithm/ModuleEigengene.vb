#Region "Microsoft.VisualBasic::ModuleEigengene, annotations\WGCNA\WGCNA\Algorithm\ModuleEigengene.vb"

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

#End Region

Imports std = System.Math

''' <summary>
''' 模块特征基因（Module Eigengene）的快速计算
''' </summary>
''' <remarks>
''' 模块特征基因定义为模块内基因表达矩阵的第一主成分（PC1）。
''' 
''' <para>
''' 直接对 g x s 的模块矩阵做完整 PCA / SVD 的代价是 O(g·s^2)。
''' 在 6 万基因 x 1888 样本的大型矩阵上，把所有模块加总起来是
''' <c>nGenes · s^2 ≈ 2.1×10^11</c> 次浮点运算，会成为整条流水线的瓶颈。
''' </para>
''' 
''' <para>
''' 这里改用<b>幂迭代</b>求 XᵀX 的主特征向量：每次迭代只需两次矩阵-向量乘
''' <c>u = X·v</c>、<c>v = Xᵀ·u</c>，代价 2·g·s。共表达模块的谱衰减很快，
''' 通常几十次迭代即可收敛，总体可以快一到两个数量级。
''' </para>
''' </remarks>
Public Module ModuleEigengene

    ''' <summary>幂迭代的最大次数</summary>
    Private Const MaxIter As Integer = 100
    ''' <summary>收敛判据：相邻两次迭代的方向变化</summary>
    Private Const Tol As Double = 1.0E-9

    ''' <summary>
    ''' 计算模块特征基因（PC1 得分，长度 = 样本数）
    ''' </summary>
    ''' <param name="expr">模块内基因的表达子矩阵（基因 x 样本）</param>
    ''' <returns>模块特征基因向量，已按「与模块平均表达正相关」对齐符号</returns>
    Public Function Compute(expr As Double()()) As Double()
        Dim g As Integer = expr.Length

        If g = 0 Then
            Return New Double() {}
        End If

        Dim s As Integer = expr(Scan0).Length

        If g = 1 Then
            Return CType(expr(Scan0).Clone(), Double())
        End If

        ' 逐基因（逐行）去均值，与 PCA 的 center=TRUE 一致
        Dim xc As Double()() = New Double(g - 1)() {}
        Dim meanProfile As Double() = New Double(s - 1) {}

        Call Parallel.For(0, g,
            Sub(i)
                Dim src As Double() = expr(i)
                Dim row(s - 1) As Double
                Dim mean As Double = 0
                Dim count As Integer = 0

                For j As Integer = 0 To s - 1
                    If Not Double.IsNaN(src(j)) Then
                        mean += src(j)
                        count += 1
                    End If
                Next

                mean = If(count = 0, 0, mean / count)

                For j As Integer = 0 To s - 1
                    row(j) = If(Double.IsNaN(src(j)), 0, src(j) - mean)
                Next

                xc(i) = row
            End Sub)

        For i As Integer = 0 To g - 1
            For j As Integer = 0 To s - 1
                meanProfile(j) += expr(i)(j)
            Next
        Next

        For j As Integer = 0 To s - 1
            meanProfile(j) /= g
        Next

        Dim v As Double() = New Double(s - 1) {}

        For j As Integer = 0 To s - 1
            v(j) = 1 / std.Sqrt(s)
        Next

        Dim u As Double() = New Double(g - 1) {}

        For iter As Integer = 1 To MaxIter
            ' u = X·v
            Call Parallel.For(0, g,
                Sub(i)
                    Dim row As Double() = xc(i)
                    Dim acc As Double = 0

                    For j As Integer = 0 To s - 1
                        acc += row(j) * v(j)
                    Next

                    u(i) = acc
                End Sub)

            ' v = Xᵀ·u
            Dim nextV(s - 1) As Double

            For i As Integer = 0 To g - 1
                Dim row As Double() = xc(i)
                Dim ui As Double = u(i)

                For j As Integer = 0 To s - 1
                    nextV(j) += ui * row(j)
                Next
            Next

            Dim norm As Double = 0

            For j As Integer = 0 To s - 1
                norm += nextV(j) * nextV(j)
            Next

            norm = std.Sqrt(norm)

            If norm <= 0 Then
                Exit For
            End If

            Dim delta As Double = 0

            For j As Integer = 0 To s - 1
                nextV(j) /= norm
                delta += std.Abs(nextV(j) - v(j))
                v(j) = nextV(j)
            Next

            If delta < Tol Then
                Exit For
            End If
        Next

        ' 符号对齐：让特征基因与模块的平均表达谱正相关
        Dim dot As Double = 0

        For j As Integer = 0 To s - 1
            dot += v(j) * meanProfile(j)
        Next

        If dot < 0 Then
            For j As Integer = 0 To s - 1
                v(j) = -v(j)
            Next
        End If

        Return v
    End Function

    ''' <summary>
    ''' 由 HTS 表达矩阵与基因 ID 列表计算模块特征基因
    ''' </summary>
    ''' <param name="samples">完整表达矩阵</param>
    ''' <param name="geneIds">模块内的基因 ID</param>
    ''' <returns>模块特征基因向量</returns>
    Public Function Compute(samples As SMRUCC.genomics.Analysis.HTS.DataFrame.Matrix, geneIds As String()) As Double()
        Dim rows As Double()() = New Double(geneIds.Length - 1)() {}

        For i As Integer = 0 To geneIds.Length - 1
            rows(i) = samples(geneIds(i)).experiments
        Next

        Return Compute(rows)
    End Function
End Module

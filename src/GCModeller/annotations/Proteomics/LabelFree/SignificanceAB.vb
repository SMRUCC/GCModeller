#Region "Microsoft.VisualBasic::2651fea6ca00af2d65a7ef2a1efdff4c, GCModeller\annotations\Proteomics\LabelFree\SignificanceAB.vb"

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

    '   Total Lines: 132
    '    Code Lines: 82
    ' Comment Lines: 35
    '   Blank Lines: 15
    '     File Size: 5.06 KB


    ' Module SignificanceAB
    ' 
    '     Function: SignificanceA, SignificanceA_VB, SignificantB
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.Distributions
Imports Microsoft.VisualBasic.Math.Distributions.BinBox
Imports Microsoft.VisualBasic.Math.LinearAlgebra
#If netcore5 = 0 Then
Imports RDotNET.Extensions.VisualBasic
Imports RDotNET.Extensions.VisualBasic.API
#End If

''' <summary>
''' 当T检验无法正常工作的时候，使用这个模块进行P值的计算
''' 仅适用于LabelFree蛋白组数据分析
''' 
''' > https://www.nature.com/articles/nbt.1511
''' </summary>
Public Module SignificanceAB

    ''' <summary>
    ''' #### Significance A
    ''' 
    ''' 函数返回FoldChange向量所对应的P值向量
    ''' </summary>
    ''' <param name="ratio">某一个比对组别的FoldChange值向量</param>
    ''' <returns></returns>
    ''' 
    <Extension>
    Public Function SignificanceA_VB(ratio As Vector) As Vector
        Dim log2 = Vector.Log(ratio, base:=2)
        Dim q#() = log2.Quantile(0.1587, 0.5, 0.8413)
        Dim rl = q(Scan0) ' r-1
        Dim rm = q(1)     ' r0
        Dim rh = q(2)     ' r1
        Dim p As Vector = log2 _
            .Select(Function(x)
                        Dim z#

                        If x > rm Then
                            z = (x - rm) / (rh - rm)
                        Else
                            z = (rm - x) / (rm - rl)
                        End If

                        Return pnorm.AboveStandardDistribution(z, 100000, m:=0, sd:=1)
                    End Function) _
            .AsVector

        Return p
    End Function

    ''' <summary>
    ''' R server based
    ''' </summary>
    ''' <param name="ratio"></param>
    ''' <returns></returns>
    Public Function SignificanceA(ratio As Vector) As Vector
#If netcore5 = 0 Then
        Dim quantile#()

        ratio = ratio.Log(base:=2)
        quantile = stats.quantile(ratio, {0.1587, 0.5, 0.8413}, narm:=True).Rvar.As(Of Double())

        Dim rl# = quantile(Scan0)
        Dim rm# = quantile(1)
        Dim rh# = quantile(2)
        Dim p As Vector = ratio _
            .Select(Function(x As Double) As String
                        If x.IsNaNImaginary Then
                            Return 0
                        Else
                            Dim z#

                            If x > rm Then
                                z = (x - rm) / (rh - rm)
                            Else
                                z = (rm - x) / (rm - rl)
                            End If

                            Return stats.pnorm(z, lowertail:=False)
                        End If
                    End Function) _
            .Select(Function(x)
                        Return var.EvaluateAs(Of Double)(x)
                    End Function) _
            .AsVector

        Return p
#Else
        Throw New NotImplementedException
#End If
    End Function

    ''' <summary>
    ''' For highly abundant proteins the statistical spread of unregulated proteins Is much more 
    ''' focused than For low abundant ones. Because Of this, a protein that shows, For instance, 
    ''' a ratio Of two should be very significant When it Is highly abundant, while at very low 
    ''' abundance it should only be marginally significant. To capture this effect we define another 
    ''' quantity, called significance B, which Is calculated in the same way as significance A, 
    ''' but on protein subsets obtained by grouping them into intensity bins. We divide the proteins 
    ''' into bins Of equal occupancy such that Each bin contains at least 300 proteins. The above 
    ''' calculation For significance A Is Then repeated In each bin to obtain significance B.
    ''' </summary>
    ''' <param name="ratio"></param>
    ''' <param name="n">将所有的蛋白FoldChange计算结果等分为n份</param>
    ''' <returns></returns>
    Public Function SignificantB(ratio As Vector, n%) As Vector
        Dim bins = CutBins.EqualFrequencyBins(ratio, k:=n, eval:=Function(x) x).ToArray
        ' 然后对每一个bin都做一次significantA计算即可
        Dim pblocks = bins _
            .Select(Function(b) SignificanceA(b.Raw)) _
            .ToArray
        ' 现在需要将pvalue放回原来的位置
        ' pindex = [ratio => pvalue]
        Dim pindex As New Dictionary(Of Double, Double)

        Call bins _
            .ForEach(Sub(b, i)
                         Dim pvalues As Vector = pblocks(i)
                         Dim ratioBock As Double() = b.Raw

                         For j As Integer = 0 To b.Count - 1
                             If Not pindex.ContainsKey(ratioBock(j)) Then
                                 Call pindex.Add(ratioBock(j), pvalues(j))
                             End If
                         Next
                     End Sub)

        Return ratio _
            .Select(Function(r) pindex(r)) _
            .AsVector
    End Function
End Module

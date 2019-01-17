Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Math.Distributions
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports RDotNET.Extensions.VisualBasic
Imports RDotNET.Extensions.VisualBasic.API

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
    End Function
End Module

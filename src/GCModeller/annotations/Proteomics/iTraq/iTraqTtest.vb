Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports Microsoft.VisualBasic.Math.SyntaxAPI.MathExtension
Imports Microsoft.VisualBasic.Math.SyntaxAPI.Vectors
Imports RDotNET.Extensions.VisualBasic.API

Public Module iTraqTtest

    ''' <summary>
    ''' iTraq结果的差异表达蛋白的检验计算
    ''' </summary>
    ''' <param name="data"></param>
    ''' <param name="level#"></param>
    ''' <param name="pvalue#"></param>
    ''' <param name="fdrThreshold#"></param>
    ''' <param name="includesZERO"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' https://github.com/xieguigang/GCModeller.cli2R/blob/master/GCModeller.cli2R/R/log2FC_t-test.R
    ''' </remarks>
    <Extension>
    Public Function logFCtest(data As IEnumerable(Of DataSet),
                              Optional level# = 1.5,
                              Optional pvalue# = 0.05,
                              Optional fdrThreshold# = 0.05,
                              Optional includesZERO As Boolean = False) As DEP_iTraq()

        Dim ZERO$ = base.rep(0, times:=data.First.Properties.Count)
        Dim result As New List(Of DEP_iTraq)
        Dim NA As DefaultValue(Of Double) = 1.0# _
            .AsDefault(Function(x)
                           Dim n# = DirectCast(x, Double)
                           Return n = 0R OrElse
                                  n.IsNaNImaginary
                       End Function)

        For Each row As DataSet In data

            Dim value As New DEP_iTraq With {
                .ID = row.ID,
                .Properties = row.Properties _
                    .ToDictionary(Function(x) x.Key,
                                  Function(x) Math.Log(x.Value, 2).ToString)
            }

            If row.Properties.Values.All(Function(x) x = 0R) Then
                ' 所有的值都是0的话，是无法进行假设检验的
                ' 但是这种情况可能是实验A之中没有表达量，但是在实验B之中被检测到了表达

                If includesZERO Then
                    value.FCavg = 0
                    value.pvalue = 0 ' 所有的实验重复都是这种情况，则重复性很好，pvalue非常非常小？？
                Else
                    value.FCavg = Double.NaN
                    value.pvalue = Double.NaN
                End If

            Else

                ' 使用1补齐NA/0
                Dim v As Vector = row.Properties _
                    .Values _
                    .Select(Function(x) x Or NA) _
                    .AsVector

                value.FCavg = v.Average
                value.log2FC = Math.Log(value.FCavg, 2)
                value.pvalue = stats.Ttest(x:=base.c(v), y:=ZERO).pvalue

            End If

            result += value
        Next

        With result.VectorShadows

            Dim test As BooleanVector
            Dim log2FC As Vector = DirectCast(.log2FC, VectorShadows(Of Double))
            Dim p As Vector = DirectCast(.pvalue, VectorShadows(Of Double))
            Dim FDR As Vector = stats.padjust(DirectCast(.FDR, Double()), n:= .FDR.Length)

            .FDR = FDR

            test = VectorMath.Abs(log2FC) >= Math.Log(level, 2)
            test = test & (p <= pvalue)

            If fdrThreshold < 1 Then
                test = test & (FDR <= fdrThreshold)
            End If

            .isDEP = test

            println("resulted %s DEPs from %s proteins!", Which.IsTrue(test).Count, result.Count)
        End With

        Return result
    End Function
End Module

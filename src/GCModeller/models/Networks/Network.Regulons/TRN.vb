#Region "Microsoft.VisualBasic::86523512facfda42d994c544403783cf, models\Networks\Network.Regulons\TRN.vb"

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

'   Total Lines: 28
'    Code Lines: 19 (67.86%)
' Comment Lines: 6 (21.43%)
'    - Xml Docs: 83.33%
' 
'   Blank Lines: 3 (10.71%)
'     File Size: 1.08 KB


' Module TRN
' 
'     Function: CorrelationNetwork
' 
' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.Framework.IO
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.Correlations
Imports SMRUCC.genomics.Analysis.HTS.DataFrame
Imports std = System.Math

Public Module TRN

    ''' <summary>
    ''' 打印相关性计算结果
    ''' </summary>
    <Extension>
    Public Sub PrintResult(result As CrossOmicsCorrelation)
        Dim nOtu As Integer = result.omics1.Length
        Dim nMet As Integer = result.omics2.Length

        ' 打印表头
        Console.Write(vbTab)
        For j As Integer = 0 To nMet - 1
            Console.Write(result.omics2(j) & vbTab)
        Next
        Console.WriteLine()

        ' 打印相关系数矩阵
        For i As Integer = 0 To nOtu - 1
            Console.Write(result.omics1(i) & vbTab)
            For j As Integer = 0 To nMet - 1
                Console.Write(result.Correlation(i, j).cor.ToString("F4") & vbTab)
            Next
            Console.WriteLine()
        Next

        ' 打印 p 值矩阵
        Console.WriteLine("p-values:")
        For i As Integer = 0 To nOtu - 1
            Console.Write(result.omics1(i) & vbTab)
            For j As Integer = 0 To nMet - 1
                Console.Write(result.Correlation(i, j).pval.ToString("F4") & vbTab)
            Next
            Console.WriteLine()
        Next
    End Sub

    Public Function ValidateSamples(ByRef expr1 As Matrix, ByRef expr2 As Matrix, Optional strict As Boolean = True) As Boolean
        ' 跨组学计算相关性要求两个矩阵的样本（实验/列）必须是对齐的！
        ' 即 expr1 的第 i 列和 expr2 的第 i 列必须是同一个样本。
        Dim intersects As String() = expr1.sampleID.Intersect(expr2.sampleID).ToArray

        If intersects.Length <> expr1.sampleID.Length Then
            Dim diffs = expr1.sampleID.Except(intersects) _
                .JoinIterates(expr2.sampleID.Except(intersects)) _
                .Distinct _
                .ToArray
            Dim msg = $"sample dimension of omics data 1(={expr1.sample_count}) is not equals to the sample dimension of the omics data 2(={expr2.sample_count}), {diffs.Length} experiment sample is mis-matched between two matrix data: {diffs.JoinBy(", ")}!"

            If strict Then
                Throw New InvalidDataException(msg)
            Else
                Call msg.warning

                expr1 = expr1.Project(intersects)
                expr2 = expr2.Project(intersects)

                Return False
            End If
        Else
            If Not expr1.sampleID.SequenceEqual(expr2.sampleID) Then
                expr1 = expr1.Project(intersects)
                expr2 = expr2.Project(intersects)

                Return False
            End If
        End If

        Return True
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="expression"></param>
    ''' <param name="cutoff">An absolute value for the correlation cutoff.</param>
    ''' <returns></returns>
    <Extension>
    Public Function CorrelationNetwork(expression As IEnumerable(Of DataSet), Optional cutoff As Double = 0.65) As IEnumerable(Of Connection)
        Dim matrix As DataSet() = expression.ToArray
        Dim samples As String() = matrix.PropertyNames

        Return matrix _
            .Select(Function(gene)
                        Return gene.CorrelationImpl(matrix, samples, isSelfComparison:=True, skipIndirect:=False, cutoff:=cutoff)
                    End Function) _
            .IteratesALL _
            .Where(Function(cnn)
                       Return std.Abs(cnn.cor) >= cutoff
                   End Function)
    End Function

    <Extension>
    Public Function CorrelationImpl(gene As DataSet, matrix As DataSet(), sampleNames$(), isSelfComparison As Boolean, skipIndirect As Boolean, cutoff#) As Connection()
        Dim fpkm As Double() = gene(sampleNames)
        Dim links As Connection() = matrix _
            .Where(Function(g)
                       If isSelfComparison Then
                           Return g.ID <> gene.ID
                       Else
                           Return True
                       End If
                   End Function) _
            .AsParallel _
            .Select(Function(g)
                        Dim fpkm2 As Double() = g(sampleNames)
                        Dim cor As Double = GetPearson(fpkm, fpkm2)

                        If std.Abs(cor) >= cutoff AndAlso skipIndirect Then
                            Return New Connection With {
                                .cor = cor,
                                .gene1 = gene.ID,
                                .gene2 = g.ID,
                                .is_directly = True
                            }
                        Else
                            Return New Connection With {
                                .cor = Spearman(fpkm, fpkm2),
                                .gene1 = gene.ID,
                                .gene2 = g.ID,
                                .is_directly = False
                            }
                        End If
                    End Function) _
            .ToArray

        Call gene.ID.info

        Return links
    End Function
End Module


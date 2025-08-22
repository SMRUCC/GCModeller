#Region "Microsoft.VisualBasic::059b17e665bbf104f7059d48161d6d99, analysis\HTS_matrix\Math.vb"

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

'   Total Lines: 126
'    Code Lines: 91 (72.22%)
' Comment Lines: 23 (18.25%)
'    - Xml Docs: 95.65%
' 
'   Blank Lines: 12 (9.52%)
'     File Size: 5.09 KB


' Module Math
' 
'     Function: log, rowSds, Sum, TRanking
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports Microsoft.VisualBasic.Math.Statistics.Hypothesis
Imports SMRUCC.genomics.GCModeller.Workbench.ExperimentDesigner
Imports std = System.Math

''' <summary>
''' math helper for HTS matrix
''' </summary>
Public Module Math

    <Extension>
    Public Function MinMaxNorm(x As Matrix) As Matrix
        Return New Matrix With {
            .sampleID = x.sampleID,
            .tag = $"minmax({x.tag})",
            .expression = x.expression _
                .Select(Function(gene)
                            Dim v As New Vector(gene.experiments)

                            Return New DataFrameRow With {
                                .geneID = gene.geneID,
                                .experiments = (v - v.Min) / (v.Max - v.Min)
                            }
                        End Function) _
                .ToArray
        }
    End Function

    ''' <summary>
    ''' sum multiple gene expression into a vector
    ''' </summary>
    ''' <param name="expr"></param>
    ''' <returns>expression sum value in a vector</returns>
    <Extension>
    Public Function Sum(expr As IEnumerable(Of DataFrameRow)) As Vector
        Dim v As Vector = Nothing

        For Each gene As DataFrameRow In expr.SafeQuery
            v = v + gene.CreateVector
        Next

        Return v
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <Extension>
    Public Function rowSds(expr As Matrix) As Dictionary(Of String, Double)
        Return expr.expression _
            .ToDictionary(Function(g) g.geneID,
                          Function(g)
                              Return g.experiments.SD
                          End Function)
    End Function

    ''' <summary>
    ''' make log transform of the expresion value
    ''' </summary>
    ''' <param name="expr"></param>
    ''' <param name="base">the base for log function.</param>
    ''' <returns></returns>
    ''' <remarks>
    ''' # 分布转换（处理非正态分布）
    ''' 
    ''' 对数转换是处理非正态分布数据的常用方法，尤其是右偏数据。通过对数转换，可以将数据的分布变得更接近正态分布，从而满足统计分析的假设。
    ''' 
    ''' 例如，对于基因表达数据，通常会使用对数转换来减少数据的偏态性，使得后续的统计分析（如t检验、方差分析等）更加可靠。
    ''' 
    ''' 对于右偏数据，可以使用以下代码进行对数转换：
    ''' 
    ''' # 对数转换（右偏数据）
    ''' log_transform &lt;- function(mat) {
    '''    log(mat + 1 - min(mat))  # 避免log(0)
    ''' }
    ''' </remarks>
    <Extension>
    Public Function log(expr As Matrix, base As Double) As Matrix
        Dim logMat As New Matrix With {
            .sampleID = expr.sampleID,
            .tag = $"log({expr.tag}, base={base})",
            .expression = expr.expression _
                .Select(Function(exp)
                            Dim min As Double = exp.experiments _
                                .Where(Function(v) v > 0 AndAlso Not v.IsNaNImaginary) _
                                .DefaultIfEmpty(0) _
                                .Min
                            Return New DataFrameRow With {
                                .geneID = exp.geneID,
                                .experiments = exp.experiments _
                                    .Select(Function(v)
                                                If v <= 0 Then
                                                    Return 0
                                                Else
                                                    Return std.Log(v + 1 - min, newBase:=base)
                                                End If
                                            End Function) _
                                    .ToArray
                            }
                        End Function) _
                .ToArray
        }

        Return logMat
    End Function

    ''' <summary>
    ''' evaluated expression ranking value
    ''' </summary>
    ''' <param name="expr">
    ''' data must be normalized!
    ''' </param>
    ''' <param name="sampleinfo"></param>
    ''' <returns></returns>
    <Extension>
    Public Iterator Function TRanking(expr As Matrix, sampleinfo As SampleInfo()) As IEnumerable(Of Ranking)
        Dim groups = DataGroup _
            .CreateDataGroups(sampleinfo) _
            .ToDictionary(Function(g) g.sampleGroup,
                          Function(group)
                              Return expr.IndexOf(group)
                          End Function)
        Dim max As Vector = expr.sampleID _
            .Select(Function(id) expr.sample(id).Max) _
            .AsVector
        Dim group_maxs = groups.ToDictionary(Function(g) g.Key, Function(g) max(g.Value).AsVector)
        Dim group_zero = groups.ToDictionary(Function(g) g.Key,
                                             Function(g)
                                                 Dim v = Replicate(0.0, g.Value.Length).ToArray
                                                 ' fix of the possible t.test constant error
                                                 v(0) = 0.0000000001
                                                 Return New Vector(v)
                                             End Function)

        For Each gene As DataFrameRow In expr.expression
            Dim ranking As New Dictionary(Of String, Double)
            Dim pvalue As New Dictionary(Of String, Double)
            Dim exprVals As New Dictionary(Of String, Double)

            For Each group In groups
                Dim index As Integer() = group.Value
                Dim group_max As Vector = group_maxs(group.Key)
                Dim group_val As Vector = gene(index).AsVector
                Dim zero As Vector = group_zero(group.Key)
                Dim trank As Vector = group_val / group_max + zero
                Dim test As TwoSampleResult = t.Test(trank, zero, Hypothesis.Less)

                exprVals.Add(group.Key, group_val.Average)
                pvalue.Add(group.Key, test.Pvalue)
                ranking.Add(group.Key, trank.Average)
            Next

            Yield New Ranking With {
                .geneID = gene.geneID,
                .pvalue = pvalue,
                .ranking = ranking,
                .expression = exprVals
            }
        Next
    End Function
End Module

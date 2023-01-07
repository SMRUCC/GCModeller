#Region "Microsoft.VisualBasic::ce5487e04665ba38059f633d0f308fc8, GCModeller\analysis\HTS_matrix\Math.vb"

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

'   Total Lines: 27
'    Code Lines: 21
' Comment Lines: 3
'   Blank Lines: 3
'     File Size: 858 B


' Module Math
' 
'     Function: AsNumeric, rowSds
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports Microsoft.VisualBasic.Math.Statistics.Hypothesis
Imports SMRUCC.genomics.GCModeller.Workbench.ExperimentDesigner
Imports stdNum = System.Math

''' <summary>
''' math helper for HTS matrix
''' </summary>
Public Module Math

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <Extension>
    Public Function rowSds(expr As Matrix) As Dictionary(Of String, Double)
        Return expr.expression _
            .ToDictionary(Function(g) g.geneID,
                          Function(g)
                              Return g.experiments.SD
                          End Function)
    End Function

    <DebuggerStepThrough>
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <Extension>
    Public Function AsNumeric(m As Matrix) As Double()()
        Return (From t As DataFrameRow
                In m.expression
                Select t.experiments).ToArray
    End Function

    <Extension>
    Public Function log(expr As Matrix, base As Double) As Matrix
        Dim logMat As New Matrix With {
            .sampleID = expr.sampleID,
            .tag = $"log({expr.tag}, base={base})",
            .expression = expr.expression _
                .Select(Function(exp)
                            Return New DataFrameRow With {
                                .geneID = exp.geneID,
                                .experiments = exp.experiments _
                                    .Select(Function(v)
                                                If v <= 0 Then
                                                    Return 0
                                                Else
                                                    Return stdNum.Log(v, newBase:=base)
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
        Dim groups = sampleinfo _
            .GroupBy(Function(s) s.sample_info) _
            .ToDictionary(Function(t) t.Key,
                          Function(t)
                              Dim group As New DataGroup With {
                                  .sampleGroup = t.Key,
                                  .sample_id = t _
                                      .Select(Function(s) s.ID) _
                                      .ToArray
                              }

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
                Dim test As TwoSampleResult = t.Test(trank, zero)

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

Public Class Ranking

    Public Property geneID As String
    Public Property ranking As Dictionary(Of String, Double)
    Public Property pvalue As Dictionary(Of String, Double)
    Public Property expression As Dictionary(Of String, Double)

    Public Overrides Function ToString() As String
        Return geneID
    End Function

End Class
#Region "Microsoft.VisualBasic::58f680e504c0786f468cb2924cea8ebd, GCModeller\analysis\Microarray\Enrichment\KOBAS\Measure.vb"

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

    '   Total Lines: 64
    '    Code Lines: 49
    ' Comment Lines: 7
    '   Blank Lines: 8
    '     File Size: 2.70 KB


    '     Module Measure
    ' 
    '         Function: AsVector, LoadTerms, Similarity
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Math

Namespace KOBAS

    Public Module Measure

        <Extension>
        Public Function LoadTerms(file$, Optional name$ = Nothing) As NamedValue(Of EnrichmentTerm())
            Return New NamedValue(Of EnrichmentTerm()) With {
                .Name = name Or file.BaseName.AsDefault,
                .Value = file _
                    .LoadCsv(Of EnrichmentTerm) _
                    .Where(Function(term) term.Pvalue <= 0.05) _
                    .ToArray
            }
        End Function

        ''' <summary>
        ''' 使用余弦相似度计算两次功能富集分析之间的结果的相似度
        ''' </summary>
        ''' <param name="group1"></param>
        ''' <param name="group2"></param>
        ''' <returns></returns>
        <Extension>
        Public Function Similarity(group1 As NamedValue(Of EnrichmentTerm()),
                                   group2 As NamedValue(Of EnrichmentTerm())) As (A As DataSet, B As DataSet, similarity#)

            Dim list1 = group1.Value.ToDictionary,
                list2 = group2.Value.ToDictionary
            Dim allTerms = (list1.Values.AsList + list2.Values) _
                .Select(Function(term) term.ID) _
                .Distinct _
                .OrderBy(Function(id) id) _
                .ToArray

            ' 结果使用P值来表示
            Dim A As New DataSet With {.ID = group1.Name, .Properties = list1.AsVector(allTerms)}
            Dim B As New DataSet With {.ID = group2.Name, .Properties = list2.AsVector(allTerms)}
            Dim cos# = Sim(A.AsVector(allTerms), B.AsVector(allTerms))

            Return (A, B, cos)
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Private Function AsVector(list As Dictionary(Of EnrichmentTerm), allTerms$()) As Dictionary(Of String, Double)
            Return allTerms _
                .ToDictionary(Function(id) id,
                              Function(key)
                                  If list.ContainsKey(key) Then
                                      Return -Math.Log10(list(key).Pvalue)
                                  Else
                                      Return 0
                                  End If
                              End Function)
        End Function
    End Module
End Namespace

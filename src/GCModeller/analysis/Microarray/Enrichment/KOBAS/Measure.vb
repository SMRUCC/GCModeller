Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Math

Namespace KOBAS

    Public Module Measure

        ''' <summary>
        ''' 使用余弦相似度计算两次功能富集分析之间的结果的相似度
        ''' </summary>
        ''' <param name="group1"></param>
        ''' <param name="group2"></param>
        ''' <returns></returns>
        <Extension>
        Public Function Similarity(group1 As NamedValue(Of IEnumerable(Of EnrichmentTerm)),
                                   group2 As NamedValue(Of IEnumerable(Of EnrichmentTerm))) As (A As DataSet, B As DataSet, similarity#)

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
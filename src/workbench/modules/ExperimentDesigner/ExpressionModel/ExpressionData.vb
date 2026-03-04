Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Linq

Public Module ExpressionData

    <Extension>
    Public Iterator Function GroupAverage(Of T As {New, INamedValue, DynamicPropertyBase(Of Double)})(genes As IEnumerable(Of T), sampleinfo As IEnumerable(Of SampleInfo)) As IEnumerable(Of T)
        Dim groups As New DataAnalysis(sampleinfo)

        For Each gene As T In genes
            Yield gene.GroupAverage(groups)
        Next
    End Function

    <Extension>
    Public Function GroupAverage(Of T As {New, INamedValue, DynamicPropertyBase(Of Double)})(gene As T, groups As DataAnalysis) As T
        Dim data As New Dictionary(Of String, Double)

        For Each group As DataGroup In groups.AsEnumerable
            Call data.Add(group.sampleGroup, group.GetData(gene).Average)
        Next

        Return New T With {
            .Key = gene.Key,
            .Properties = data
        }
    End Function
End Module

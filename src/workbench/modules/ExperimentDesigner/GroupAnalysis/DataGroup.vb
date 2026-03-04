
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic

''' <summary>
''' the group view of the <see cref="SampleInfo"/> data
''' </summary>
Public Class DataGroup : Implements INamedValue

    ''' <summary>
    ''' the sample group label: <see cref="SampleInfo.sample_info"/>
    ''' </summary>
    ''' <returns></returns>
    Public Property sampleGroup As String Implements INamedValue.Key
    ''' <summary>
    ''' a collection of the sample id that belongs to current sample data group
    ''' </summary>
    ''' <returns></returns>
    Public Property sample_id As String()
    Public Property color As String
    Public Property shape As String

    Default Public ReadOnly Property Item(index As Integer) As String
        Get
            Return _sample_id(index)
        End Get
    End Property

    ''' <summary>
    ''' create a collection of the <see cref="DataGroup"/> from the given <see cref="SampleInfo"/>
    ''' </summary>
    ''' <param name="samples"></param>
    ''' <returns></returns>
    Public Shared Iterator Function CreateDataGroups(samples As IEnumerable(Of SampleInfo)) As IEnumerable(Of DataGroup)
        For Each group As IGrouping(Of String, SampleInfo) In samples.GroupBy(Function(s) s.sample_info)
            Dim template As SampleInfo = group.First

            Yield New DataGroup With {
                .sampleGroup = group.Key,
                .sample_id = group _
                    .Select(Function(s) s.ID) _
                    .ToArray,
                .color = template.color,
                .shape = template.shape
            }
        Next
    End Function

    Public Shared Function NameGroups(samples As IEnumerable(Of SampleInfo)) As Dictionary(Of String, String())
        Return samples _
            .GroupBy(Function(s) s.sample_info) _
            .ToDictionary(Function(g) g.Key,
                          Function(g)
                              Return g.Select(Function(s) s.ID).ToArray
                          End Function)
    End Function
End Class
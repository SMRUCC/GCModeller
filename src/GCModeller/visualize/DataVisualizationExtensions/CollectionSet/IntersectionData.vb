Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Linq

Namespace CollectionSet

    Public Class IntersectionData

        Public Property groups As FactorGroup()

        Public ReadOnly Property size As Integer
            Get
                Return groups.Length
            End Get
        End Property

        Public Function GetSetSize() As NamedValue(Of Integer)()
            Dim allLabels = groups.Select(Function(i) i.data).IteratesALL.GroupBy(Function([set]) [set].name).ToArray
            Dim counts = allLabels _
                .Select(Function(d)
                            Dim name As String = d.Key
                            Dim num As Integer = d _
                                .Select(Function([set]) [set].value) _
                                .IteratesALL _
                                .Distinct _
                                .Count

                            Return New NamedValue(Of Integer) With {
                                .Name = name,
                                .Value = num
                            }
                        End Function) _
                .ToArray

            Return counts
        End Function

        ''' <summary>
        ''' get the labels of all collection set like ``a vs b``, etc
        ''' </summary>
        ''' <returns></returns>
        Public Function GetAllCollectionTags() As String()
            Return groups _
                .Select(Function(d) d.data.Select(Function(t) t.name)) _
                .IteratesALL _
                .Distinct _
                .ToArray
        End Function

    End Class

End Namespace
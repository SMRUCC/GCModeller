Imports System.Drawing
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Linq

Namespace CollectionSet

    Public Class IntersectionData

        Public Property groups As FactorGroup()

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

    Public Class FactorGroup

        Public Property factors As String
        Public Property data As NamedCollection(Of String)()
        Public Property color As Color

    End Class
End Namespace
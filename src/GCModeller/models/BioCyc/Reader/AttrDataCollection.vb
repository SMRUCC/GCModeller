Public Class AttrDataCollection(Of T As Model)

    Public ReadOnly Property fileMeta As FileMeta
    Public ReadOnly Property features As IEnumerable(Of T)
        Get
            Return models.Values
        End Get
    End Property

    Protected ReadOnly models As Dictionary(Of String, T)

    Default Public ReadOnly Property getFeature(i As String) As T
        Get
            Return models(i)
        End Get
    End Property

    Sub New(meta As FileMeta, objects As IEnumerable(Of T))
        fileMeta = meta
        models = objects.ToDictionary(Function(o) o.uniqueId)
    End Sub

    Public Overrides Function ToString() As String
        Return fileMeta.ToString
    End Function

End Class

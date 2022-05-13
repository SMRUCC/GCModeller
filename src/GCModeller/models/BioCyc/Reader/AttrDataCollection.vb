Imports System.IO
Imports Microsoft.VisualBasic.Linq

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
            Return models.TryGetValue(i)
        End Get
    End Property

    Sub New(meta As FileMeta, objects As IEnumerable(Of T))
        fileMeta = meta
        models = objects.ToDictionary(Function(o) o.uniqueId)
    End Sub

    Public Overrides Function ToString() As String
        Return fileMeta.ToString
    End Function

    Public Shared Function LoadFile(file As Stream) As AttrDataCollection(Of T)
        Dim dataFile As AttrValDatFile = AttrValDatFile.ParseFile(New StreamReader(file))
        Dim writer As ObjectWriter = ObjectWriter.LoadSchema(Of T)
        Dim data As T() = (From a As SeqValue(Of FeatureElement)
                           In dataFile.features _
                               .SeqIterator _
                               .AsParallel
                           Let obj As Object = writer.Deserize(a.value)
                           Order By a.i
                           Select DirectCast(obj, T)).ToArray

        Return New AttrDataCollection(Of T)(dataFile.fileMeta, data)
    End Function

End Class

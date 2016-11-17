
Namespace TrackDatas

    Public Interface Idata
        Property FileName As String
        Function GetDocumentText() As String
        Function GetEnumerator() As IEnumerable(Of ITrackData)
    End Interface
End Namespace
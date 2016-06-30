
Public Class MySQLService(Of T) : Inherits DataStorage(Of T, DataStorage.FileModel.DataSerials(Of T))

    Public Overloads Overrides Function WriteData(chunkbuffer As IEnumerable(Of DataStorage.FileModel.DataSerials(Of T)), url As String) As Boolean
        Throw New NotImplementedException
    End Function

    Public Overloads Overrides Function WriteData(url As String) As Boolean
        Throw New NotImplementedException
    End Function
End Class

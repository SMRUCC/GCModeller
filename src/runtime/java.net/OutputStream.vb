Imports System.Text

Public Class OutputStream

    Dim _outputfile As String
    Dim _streamData As StringBuilder

    Sub New(outputFile As String, streamData As StringBuilder)
        _outputfile = outputFile
        _streamData = streamData
        If _streamData Is Nothing Then
            _streamData = New StringBuilder
        End If
    End Sub

    Sub New(outputFile As String)
        _outputfile = outputFile
        _streamData = New Global.System.Text.StringBuilder
    End Sub

    Public Sub WriteStream(data As StringBuilder)
        Call _streamData.AppendLine(data.ToString)
    End Sub

    Public Sub WriteStream(data As String)
        Call _streamData.AppendLine(data)
    End Sub

    Public Sub Write()
        Dim parentDir As String = FileIO.FileSystem.GetParentPath(_outputfile)
        Call FileIO.FileSystem.CreateDirectory(parentDir)
        Call FileIO.FileSystem.WriteAllText(_outputfile, _streamData.ToString, append:=False)
    End Sub
End Class

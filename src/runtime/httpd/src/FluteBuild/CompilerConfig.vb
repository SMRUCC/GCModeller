Imports Microsoft.VisualBasic.MIME.application.json
Imports Microsoft.VisualBasic.MIME.application.json.Javascript

Public Class CompilerConfig

    Public Property markdown As MarkdownConfig
    Public Property variables As Dictionary(Of String, Object)

    Public Sub [set](key As String, value As Object)
        If variables Is Nothing Then
            variables = New Dictionary(Of String, Object)
        End If

        _variables(key) = value
    End Sub

    Public Function join(args As Dictionary(Of String, Object)) As CompilerConfig
        If variables Is Nothing Then
            variables = New Dictionary(Of String, Object)
        End If

        For Each key As String In args.Keys
            _variables(key) = args(key)
        Next

        Return Me
    End Function

    Public Shared Function Load(file As String) As CompilerConfig
        Dim json As String = file.ReadAllText
        Dim data As JsonObject = JsonParser.Parse(json, False)

        If data Is Nothing Then
            Return New CompilerConfig
        End If

        Return data.CreateObject(Of CompilerConfig)()
    End Function

End Class

Public Class MarkdownConfig

    ''' <summary>
    ''' html template file path
    ''' </summary>
    ''' <returns></returns>
    Public Property template As String
    ''' <summary>
    ''' a folder path that contains the markdown source files
    ''' </summary>
    ''' <returns></returns>
    Public Property source As String

End Class

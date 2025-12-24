Imports System.Text
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
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
    Public Property menu As Menu

    Public Shared Iterator Function LoadMenu(source As String) As IEnumerable(Of NamedCollection(Of String))
        For Each dir As String In source.ListDirectory
            Yield New NamedCollection(Of String)(dir.BaseName, dir.ListFiles("*.md").BaseName)
        Next
    End Function

    Public Function RenderMenuHtml(sections As IEnumerable(Of NamedCollection(Of String))) As String
        If menu Is Nothing Then
            Return ""
        End If

        Return (From li As NamedCollection(Of String)
                In sections
                Select menu.RenderMenuHtml(li)).JoinBy(vbCrLf)
    End Function

End Class

Public Class Menu

    Public Property section As String
    Public Property list As List

    Public Function RenderMenuHtml(section As NamedCollection(Of String)) As String
        Dim html As New StringBuilder(Me.section)

        Call html.Replace("@section", section.name)
        Call html.Append(list.list)

        Dim menuItems As New List(Of String)

        For Each item As String In section
            Call menuItems.Add(list.item.Replace("@item", item).Replace("@section", section.name))
        Next

        Call html.Replace("@list", menuItems.JoinBy(""))

        Return html.ToString
    End Function

End Class

Public Class List

    Public Property list As String
    Public Property item As String

End Class
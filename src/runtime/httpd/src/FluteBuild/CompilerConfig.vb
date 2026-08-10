#Region "Microsoft.VisualBasic::6eea9f5a83db12f166fcbdab665abb77, src\FluteBuild\CompilerConfig.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xie (genetics@smrucc.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program. If not, see <http://www.gnu.org/licenses/>.



    ' /********************************************************************************/

    ' Summaries:


    ' Code Statistics:

    '   Total Lines: 113
    '    Code Lines: 74 (65.49%)
    ' Comment Lines: 8 (7.08%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 31 (27.43%)
    '     File Size: 3.25 KB


    ' Class CompilerConfig
    ' 
    '     Properties: markdown, variables
    ' 
    '     Function: join, Load
    ' 
    '     Sub: [set], del
    ' 
    ' Class MarkdownConfig
    ' 
    '     Properties: menu, source, template
    ' 
    '     Function: LoadMenu, RenderMenuHtml
    ' 
    ' Class Menu
    ' 
    '     Properties: list, section
    ' 
    '     Function: RenderMenuHtml
    ' 
    ' Class List
    ' 
    '     Properties: item, list
    ' 
    ' /********************************************************************************/

#End Region

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

    Public Sub del(name As String)
        If variables Is Nothing Then
            variables = New Dictionary(Of String, Object)
        End If

        _variables.Remove(name)
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

#Region "Microsoft.VisualBasic::c8b25dae7a5ebd8f4cbb9861b62f429d, G:/GCModeller/src/runtime/httpd/src/WebCloud/SMRUCC.WebCloud.VBScript//VBHtml.vb"

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

    '   Total Lines: 153
    '    Code Lines: 85
    ' Comment Lines: 46
    '   Blank Lines: 22
    '     File Size: 5.78 KB


    ' Class VBHtml
    ' 
    '     Properties: encoding, filepath, html
    ' 
    '     Constructor: (+1 Overloads) Sub New
    ' 
    '     Function: GetString, GetSymbol, HasSymbol, ReadHTML, ToString
    ' 
    '     Sub: AddSymbol, Render, Replace
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Text
Imports System.Text.RegularExpressions
Imports Flute.Template.Interpolate
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Text
Imports any = Microsoft.VisualBasic.Scripting

''' <summary>
''' *.vbhtml
''' </summary>
Public Class VBHtml

    ''' <summary>
    ''' matches for the variable and its property reference
    ''' </summary>
    Friend Shared ReadOnly variable As New Regex("@[_a-z][_a-z0-9]*(\.[_a-z][_a-z0-9]*)*", RegexOptions.IgnoreCase Or RegexOptions.Multiline Or RegexOptions.Compiled)
    Friend Shared ReadOnly partialIncludes As New Regex("<%= [^>]+? %>", RegexOptions.IgnoreCase Or RegexOptions.Multiline Or RegexOptions.Compiled)
    Friend Shared ReadOnly foreach As New Regex("<foreach @.+?</foreach>", RegexOptions.IgnoreCase Or RegexOptions.Singleline Or RegexOptions.Compiled)

    ''' <summary>
    ''' set variable value from the template file
    ''' </summary>
    Friend Shared ReadOnly valueExpression As New Regex("<% @[_a-z][_a-z0-9]*.+? %>", RegexOptions.IgnoreCase Or RegexOptions.Singleline Or RegexOptions.Compiled)

    ''' <summary>
    ''' all of the key inside this dictionary is in lower case,
    ''' due to the reason of vb identifier is not case-sensitive
    ''' </summary>
    ''' <remarks>
    ''' syntax for declare a variable inside the template content is @xxx
    ''' </remarks>
    Friend ReadOnly variables As Dictionary(Of String, Object)
    Friend ReadOnly assigned As New Dictionary(Of String, String)

    Default Public Property Item(name As String) As String
        Get
            Return assigned.TryGetValue(name)
        End Get
        Set(value As String)
            assigned(name) = value
            html.Replace("@" & name, value)
        End Set
    End Property

    ''' <summary>
    ''' the file path full name of the template file
    ''' </summary>
    Public ReadOnly Property filepath As String
    Public ReadOnly Property html As StringBuilder
    Public ReadOnly Property encoding As Encoding

    Friend Sub New(path As String,
                   symbols As Dictionary(Of String, Object),
                   encodings As Encoding)

        filepath = path.GetFullPath
        html = New StringBuilder
        encoding = encodings

        ' make value copy at here
        ' for handling the case sensetity problem
        ' vb language is not case sensity, so converts all keys to lower case at here
        variables = If(symbols, New Dictionary(Of String, Object))
        variables = variables _
            .ToDictionary(Function(var) var.Key.ToLower,
                          Function(var)
                              Return var.Value
                          End Function)
    End Sub

    Public Function HasSymbol(name As String) As Boolean
        Return variables.ContainsKey(name.ToLower)
    End Function

    Public Function GetSymbol(name As String) As Object
        Return variables.TryGetValue(name.ToLower)
    End Function

    ''' <summary>
    ''' <see cref="GetSymbol(String)"/> as string
    ''' </summary>
    ''' <param name="name"></param>
    ''' <returns></returns>
    Public Function GetString(name As String) As String
        Dim key As String = name.ToLower

        If variables.ContainsKey(key) Then
            Return any.ToString(variables(key), "")
        Else
            Return ""
        End If
    End Function

    ''' <summary>
    ''' add variable value that which is parsed from the vbhtml template
    ''' </summary>
    ''' <param name="name"></param>
    ''' <param name="value"></param>
    ''' <remarks>
    ''' add value to <see cref="variables"/>
    ''' </remarks>
    Public Sub AddSymbol(name As String, value As Object)
        name = name.ToLower

        ' the variable from function input always overrides the
        ' variable from the vbhtml template parsed result,
        ' so we check of the variable name is existsed at here
        ' skip if the name already existed
        If Not variables.ContainsKey(name) Then
            Call variables.Add(name, value)
        End If
    End Sub

    ''' <summary>
    ''' do string replace on <see cref="html"/> directly.
    ''' </summary>
    ''' <param name="name"></param>
    ''' <param name="value"></param>
    Public Sub Replace(name As String, value As String)
        html.Replace(name, value)
    End Sub

    Private Sub Render()
        For Each symbol As NamedValue(Of Object) In VariableInterpolate.GetVariables(ToString)
            ' removes from the html template
            Call Replace(symbol.Description, "")
            Call AddSymbol(symbol.Name, symbol.Value)
        Next

        ' @xxx
        Call ForeachInterpolate.ForeachTemplate(Me)
        Call IncludeInterpolate.FillIncludes(Me)
        Call VariableInterpolate.FillVariables(Me)
    End Sub

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Overrides Function ToString() As String
        Return html.ToString
    End Function

    ''' <summary>
    ''' Do html template rendering, processing a single html template file rendering.
    ''' 
    ''' ``&lt;%= relative_path %>``
    ''' </summary>
    ''' <param name="path">target template file to rendering</param>
    ''' <param name="variables">Data symbols to fill onto the html template</param>
    ''' <returns></returns>
    Public Shared Function ReadHTML(path$,
                                    Optional variables As Dictionary(Of String, Object) = Nothing,
                                    Optional encoding As Encodings = Encodings.UTF8) As String

        Dim html As New VBHtml(path, variables, encoding.CodePage)
        html.html.Append(path.ReadAllText(html.encoding))
        html.Render()
        Return html.ToString
    End Function

End Class

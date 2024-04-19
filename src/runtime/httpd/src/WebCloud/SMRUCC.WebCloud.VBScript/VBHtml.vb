#Region "Microsoft.VisualBasic::f3913d8946a7ac8926f63aeffbe4f96d, WebCloud\SMRUCC.WebCloud.VBScript\vbhtml.vb"

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

' Module vbhtml
' 
'     Function: GetIncludesPath, LoadStrings, ParseVariables, ReadHTML, TemplateInterplot
' 
'     Sub: (+2 Overloads) ApplyStrings
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Text
Imports System.Text.RegularExpressions
Imports Flute.Template.Interpolate
Imports Microsoft.VisualBasic.ComponentModel.Collection
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
    Friend Shared ReadOnly valueExpression As New Regex("<% @[_a-z][_a-z0-9]*.+? %>", RegexOptions.IgnoreCase Or RegexOptions.Multiline Or RegexOptions.Compiled)

    ''' <summary>
    ''' all of the key inside this dictionary is in lower case,
    ''' due to the reason of vb identifier is not case-sensitive
    ''' </summary>
    ReadOnly variables As Dictionary(Of String, Object)
    ReadOnly assigned As New Dictionary(Of String, String)

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
    Public ReadOnly Property wwwroot As String
    Public ReadOnly Property html As StringBuilder
    Public ReadOnly Property encoding As Encoding

    Friend Sub New(path As String, workdir As String,
                   symbols As Dictionary(Of String, Object),
                   encodings As Encoding)

        filepath = path.GetFullPath
        wwwroot = If(workdir.StringEmpty, filepath.ParentPath, workdir.GetDirectoryFullPath)
        html = New StringBuilder
        encoding = encodings
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

    Public Function GetString(name As String) As String
        Dim key As String = name.ToLower

        If variables.ContainsKey(key) Then
            Return any.ToString(variables(key), "")
        Else
            Return ""
        End If
    End Function

    Public Sub Replace(name As String, value As String)
        html.Replace(name, value)
    End Sub

    Private Sub Render()
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
    ''' <param name="wwwroot">Using for reading strings resource json file.</param>
    ''' <param name="variables">Data symbols to fill onto the html template</param>
    ''' <returns></returns>
    Public Shared Function ReadHTML(path$,
                                    Optional variables As Dictionary(Of String, Object) = Nothing,
                                    Optional wwwroot$ = Nothing,
                                    Optional encoding As Encodings = Encodings.UTF8) As String

        Dim html As New VBHtml(path, wwwroot, variables, encoding.CodePage)
        html.html.Append(path.ReadAllText(html.encoding))
        html.Render()
        Return html.ToString
    End Function

End Class

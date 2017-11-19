Imports System.Runtime.CompilerServices
Imports System.Text
Imports System.Xml
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Scripting.Expressions
Imports Microsoft.VisualBasic.Text
Imports Microsoft.VisualBasic.Text.Xml.Linq
Imports SMRUCC.WebCloud.HTTPInternal.Platform.Plugins
Imports Map = System.Collections.Generic.KeyValuePair(Of String, String)
Imports r = System.Text.RegularExpressions.Regex

Public Module vbhtml

    Const PartialIncludes$ = "<%= [^>]+? %>"
    Const ValueExpression$ = "<\?vb\s+[$].+?=\s*""[^""]*""\s+\?>"

    ''' <summary>
    ''' ``&lt;%= relative_path %>``
    ''' </summary>
    ''' <param name="path$"></param>
    ''' <param name="wwwroot">Using for reading strings.XML resource file.</param>
    ''' <returns></returns>
    <Scripting("*.vbhtml")>
    <Extension> Public Function ReadHTML(wwwroot$, path$, Optional encoding As Encodings = Encodings.UTF8) As String
        Dim codepage As Encoding = encoding.CodePage
        Dim parent$ = path.ParentPath
        Dim html As New StringBuilder(path.ReadAllText(codepage))
        Dim strings = (wwwroot & "/includes/strings.XML").LoadStrings
        Return html.TemplateInterplot(parent, codepage, Nothing, strings)
    End Function

    Public Function ParseVariables(html As String) As (raw$, expr As NamedValue(Of String))()
        Dim values$() = r _
            .Matches(html, vbhtml.ValueExpression, RegexICMul) _
            .ToArray
        Dim table As (raw$, exp As NamedValue(Of String))() = values _
            .Select(Function(s)
                        Dim exp = Mid(s.Trim("<"c, ">"c, "?"c), 4) _
                            .Trim _
                            .GetTagValue("=", trim:=True)
                        With exp
                            .Value = .Value.GetStackValue("""", """")
                            .Name = .Name.Trim("$"c, " "c)
                        End With
                        Return (s, exp)
                    End Function) _
            .ToArray

        Return table
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="html"></param>
    ''' <param name="codepage"></param>
    ''' <param name="stringsXML">String resource from loader: <see cref="LoadStrings(String)"/></param>
    ''' <returns></returns>
    <Extension>
    Public Function TemplateInterplot(html As StringBuilder, parent$, codepage As Encoding, variables As Dictionary(Of String, String), stringsXML As Dictionary(Of String, String)) As String
        Dim includes$() = r _
            .Matches(html.ToString, vbhtml.PartialIncludes, RegexICSng) _
            .ToArray
        Dim table = ParseVariables(html.ToString)
        ' <%= @Key %>
        Dim strings As New Dictionary(Of String, String)

        ' <%= include_path %>
        For Each include As String In includes
            Dim rel_path$ = include.Trim("<"c, ">"c, "%"c)
            ' 去除等号
            rel_path = Mid(rel_path, 2).Trim

            If rel_path.First = "@"c Then
                ' 因为对资源的引用可能会在多处有重复的引用
                ' 所以在这里不能够直接进行添加
                strings(include) = rel_path.Substring(1)
                Continue For
            Else
                rel_path = parent & "/" & rel_path
                rel_path = FileIO.FileSystem.GetFileInfo(rel_path).FullName
            End If

            ' 在这里会产生一个递归树，将其余的模板也进行插值处理
            Dim content As StringBuilder = rel_path.ReadAllText(codepage).CreateBuilder
            Call content.TemplateInterplot(rel_path.ParentPath, codepage, variables, stringsXML)
            Call html.Replace(include, content.ToString)
        Next

        Dim getValue As Func(Of String, String)

        If table.Length > 0 Then
            Dim exp = table _
                .Select(Function(e) e.expr) _
                .ToDictionary

            getValue = Function(name$)
                           If exp.ContainsKey(name) Then
                               Return exp(name).Value
                           Else
                               Return ""
                           End If
                       End Function

            ' 替换操作应该放在插值操作前面，否则后面将无法清除掉这些原始
            ' 的插值标记， 因为标记里面的$变量都会被替换为值了
            For Each t In table
                Call html.Replace(t.raw, "")
            Next

            Call html.Interpolate(getValue)
        End If

        ' variables主要是为ForLoop表达式所准备的
        If Not variables.IsNullOrEmpty Then
            Call html.Interpolate(variables.GetValue)
        End If

        If strings.Count > 0 Then
            Call html.ApplyStrings(strings, stringsXML)
        End If

        Return html.ToString
    End Function

    <Extension> Public Function LoadStrings(pathXML As String) As Dictionary(Of String, String)
        Dim xml As XmlDocument = pathXML.LoadXmlDocument
        Dim XmlNodeList As XmlNodeList = xml.GetElementsByTagName("string")
        Dim values As New Dictionary(Of String, String)

        For Each xmlNode As XmlNode In XmlNodeList
            For Each a As XmlAttribute In xmlNode.Attributes
                If a.Name = "name" Then
                    values.Add(a.InnerText, xmlNode.InnerText)
                    Exit For
                End If
            Next
        Next

        Return values
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <Extension>
    Public Sub ApplyStrings(ByRef html As StringBuilder, strings As Dictionary(Of String, String), resource As Dictionary(Of String, String))
        For Each ref As Map In strings
            Call html.Replace(ref.Key, resource(ref.Value))
        Next
    End Sub

    ''' <summary>
    ''' 字符串资源默认是在 ``&lt;wwwroot>/includes/strings.XML``
    ''' </summary>
    ''' <param name="html"></param>
    ''' <param name="strings"></param>
    <Extension>
    Public Sub ApplyStrings(ByRef html As StringBuilder, wwwroot$, strings As Dictionary(Of String, String))
        With (wwwroot & "/includes/strings.XML").LoadStrings
            Call html.ApplyStrings(strings, resource:= .ref)
        End With
    End Sub
End Module
Imports System.Reflection
Imports System.Runtime.CompilerServices
Imports System.Text
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Text
Imports r = System.Text.RegularExpressions.Regex

Partial Module vbhtml

    Const ForEachLoop$ = "<\?vb\s+For\s+.+?\s+As\s+<%= [^>]+? %>\s+\?>"

    ''' <summary>
    ''' Returns tuple: ``[expression, template_path]``
    ''' </summary>
    ''' <param name="vbhtml$"></param>
    ''' <returns></returns>
    Public Function ParseForEach(vbhtml$) As NamedValue(Of String)()
        Dim expression = r _
            .Matches(vbhtml, ForEachLoop, RegexICSng) _
            .EachValue(AddressOf parserInternal) _
            .ToArray

        Return expression
    End Function

    Private Function parserInternal(input As String) As NamedValue(Of String)
        Dim value$ = input.GetStackValue("<?vb", "?>").Trim(" "c, ASCII.TAB, ASCII.CR, ASCII.LF)
        Dim t = r _
            .Split(value, "(For\s+)|(\s+As\s+)", RegexICSng) _
            .Where(Function(s) Not s.StringEmpty) _
            .ToArray

        Return New NamedValue(Of String) With {
            .Name = t(1),
            .Value = t(3),
            .Description = input
        }
    End Function

    <Extension>
    Public Function GetVariables(schema As Dictionary(Of String, PropertyInfo), name$, obj As Object) As NamedValueList(Of String)
        Dim list As New NamedValueList(Of String)

        For Each [property] In schema
            Dim propertyName$ = name & "." & [property].Key
            Dim o As Object = [property].Value.GetValue(obj)
            Dim value$ = Scripting.ToString(o, "null")

            list += New NamedValue(Of String) With {
                .Name = propertyName,
                .Value = value
            }
        Next

        Return list
    End Function

    <Extension>
    Public Function Iterates(html As StringBuilder, parent$, args As InterpolateArgs) As StringBuilder
        Dim expressions = ParseForEach(html.ToString)

        For Each exp As NamedValue(Of String) In expressions
            Dim path$ = parent & "/" & exp.Value.GetIncludesPath
            Dim pathParent$ = path.ParentPath
            Dim template$ = path.ReadAllText(args.codepage)
            Dim source As IEnumerable = args.data(exp.Name)
            Dim list As New List(Of String)
            Dim variables As New Dictionary(Of String, String)(args.variables)
            Dim type As Type = source.GetType.GetTypeElement(True)
            Dim schema = type.Schema(PropertyAccess.Readable, PublicProperty, True)

            For Each obj As Object In source
                Dim vbhtml As New StringBuilder(template)
                Dim stackArgvs As New InterpolateArgs With {
                    .codepage = args.codepage,
                    .data = args.data,
                    .resource = args.resource,
                    .variables = variables + schema.GetVariables(exp.Name, obj),
                    .wwwroot = args.wwwroot
                }

                list += vbhtml.TemplateInterplot(parent, args)
            Next

            ' 从模板生成html之后开始进行替换
            html.Replace(exp.Description, list.JoinBy(vbCrLf & vbCrLf))
        Next

        Return html
    End Function
End Module
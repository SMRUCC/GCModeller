Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text
Imports RDotNET.Extensions.VisualBasic.API
Imports RDotNET.Extensions.VisualBasic.SymbolBuilder

Public Module ArgumentList

    <Extension>
    Private Function argumentExpression(args As ArgumentReference()) As String
        Dim assigns$() = args _
            .Select(Function(f)
                        Dim exp = f.parameterValueAssign
                        Dim script = $"{exp.Name} = {exp.Value}"

                        Return script
                    End Function) _
            .ToArray

        Return assigns.JoinBy(", ")
    End Function

    <Extension>
    Private Function parameterValueAssign(f As ArgumentReference) As NamedValue(Of String)
        If Not f.Value Is Nothing AndAlso f Like GetType(var) Then
            ' do variable value assign at here
            Return New NamedValue(Of String) With {
                .Name = f.Name,
                .Value = DirectCast(f.Value, var).name
            }
        ElseIf f.Value Is Nothing Then
            ' value is NULL
            Return New NamedValue(Of String) With {
                .Name = f.Name,
                .Value = "NULL"
            }
        ElseIf f Like GetType(String) OrElse f Like GetType(Char) Then
            ' 2019-03-19
            ' 有些时候会因为二进制文件读取的原因导致字符串结尾出现0字节的字符
            ' 这会导致R报错
            ' 所以会需要删除一下最末尾的0字节的字符
            Dim str As String = CStr(f.Value).Trim(ASCII.NUL)

            If Not str.StringEmpty AndAlso base.exists(str) Then
                ' do variable value assign by variable name
                Return New NamedValue(Of String) With {
                    .Name = f.Name,
                    .Value = str
                }
            Else
                ' is string value
                Return New NamedValue(Of String) With {
                    .Name = f.Name,
                    .Value = Rstring(str)
                }
            End If
        ElseIf f Like GetType(IEnumerable(Of String)) Then
            Return New NamedValue(Of String) With {.Name = f.Name, .Value = base.c(f.Value, stringVector:=True)}
        ElseIf f Like GetType(Boolean) Then
            Return New NamedValue(Of String) With {.Name = f.Name, .Value = f.Value.ToString.ToUpper}
        ElseIf f Like GetType(IEnumerable(Of Boolean)) Then
            Return New NamedValue(Of String) With {.Name = f.Name, .Value = base.c(DirectCast(f.Value, IEnumerable(Of Boolean)).ToArray)}
        Else
            Return New NamedValue(Of String) With {.Name = f.Name, .Value = Scripting.ToString(f.Value, "NULL")}
        End If
    End Function
End Module

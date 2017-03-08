Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.ComponentModel.DBLinkBuilder

Namespace Assembly.KEGG

    Public Module DrugParser

        Public Iterator Function ParseStream(path$) As IEnumerable(Of Drug)
            Dim lines$() = path.ReadAllLines

            For Each pack As String() In lines.Split("///")
                Yield pack.ParseStream
            Next
        End Function

        <Extension> Private Function ParseStream(lines$()) As Drug
            Dim list As New Dictionary(Of NamedValue(Of List(Of String)))
            Dim tag$ = ""  ' 在这里使用空字符串，如果使用Nothing空值的话，添加字典的时候出发生错误
            Dim values As New List(Of String)
            Dim add = Sub()
                          ' 忽略掉original，bracket这些分子结构参数，因为可以很方便的从ChEBI数据库之中获取得到
                          If Not list.ContainsKey(tag) Then
                              list += New NamedValue(Of List(Of String)) With {
                                  .Name = tag,
                                  .Value = values
                              }
                          End If
                      End Sub

            For Each line As String In lines
                Dim s$ = Mid(line, 1, 12).StripBlank

                If s.StringEmpty Then
                    values += line.Trim
                Else
                    ' 切换到新的标签
                    Call add()

                    tag = s
                    values = New List(Of String)
                    values += Mid(line, 12).StripBlank
                End If
            Next

            ' 还会有剩余的数据的，在这里将他们添加上去
            Call add()

            Dim getValue = Function(KEY$) As String()
                               If list.ContainsKey(KEY) Then
                                   Return list(KEY).Value
                               Else
                                   Return {}
                               End If
                           End Function

            Return New Drug With {
                .Entry = getValue("ENTRY").FirstOrDefault.Split.First,
                .Names = getValue("NAME") _
                    .Where(Function(s) Not s.StringEmpty) _
                    .ToArray,
                .Formula = getValue("FORMULA").FirstOrDefault,
                .Exact_Mass = Val(getValue("EXACT_MASS").FirstOrDefault),
                .Mol_Weight = Val(getValue("MOL_WEIGHT").FirstOrDefault),
                .Remarks = getValue("REMARKS"),
                .DBLinks = getValue("DBLINKS") _
                    .Select(AddressOf DBLink.FromTagValue) _
                    .ToArray,
                .Activity = getValue("ACTIVITY").FirstOrDefault,
                .Atoms = __atoms(getValue("ATOM")),
                .Bounds = __bounds(getValue("BOUND")),
                .Comments = getValue("COMMENT"),
                .Targets = getValue("TARGET"),
                .Metabolism = getValue("METABOLISM") _
                    .ToArray(Function(s) s.GetTagValue(":", trim:=True)),
                .Interaction = getValue("INTERACTION") _
                    .ToArray(Function(s) s.GetTagValue(":", trim:=True)),
                .Source = getValue("SOURCE") _
                    .ToArray(Function(s) s.StringSplit(",\s+")) _
                    .IteratesALL _
                    .ToArray
            }
        End Function

        Private Function __atoms(lines$()) As Atom()
            If lines.IsNullOrEmpty OrElse
                lines(Scan0).ParseInteger = 0 Then
                Return {}
            End If

            Dim out As New List(Of Atom)

            For Each line$ In lines.Skip(1)
                Dim t$() = line.StringSplit("\s+", True)

                out += New Atom With {
                    .index = Val(t(0)),
                    .Formula = t(1),
                    .Atom = t(2),
                    .M = Val(t(3)),
                    .Charge = Val(t(4)),
                    .Edit = t.Get(5)
                }
            Next

            Return out
        End Function

        Private Function __bounds(lines$()) As Bound()
            If lines.IsNullOrEmpty OrElse
                lines(Scan0).ParseInteger = 0 Then
                Return {}
            End If

            Dim out As New List(Of Bound)

            For Each line$ In lines.Skip(1)
                Dim t$() = line.StringSplit("\s+", True)

                out += New Bound With {
                    .index = Val(t(0)),
                    .a = Val(t(1)),
                    .b = Val(t(2)),
                    .N = Val(t(3)),
                    .Edit = t.Get(4)
                }
            Next

            Return out
        End Function
    End Module
End Namespace
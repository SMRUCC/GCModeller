Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Language
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
            Dim tag$ = Nothing
            Dim values As New List(Of String)

            For Each line As String In lines
                Dim s$ = Mid(line, 1, 12).StripBlank

                If s.StringEmpty Then
                    values += line.Trim
                Else
                    ' 切换到新的标签
                    list += New NamedValue(Of List(Of String)) With {
                        .Name = tag,
                        .Value = values
                    }

                    tag = s
                    values = New List(Of String)
                    values += Mid(line, 12).StripBlank
                End If
            Next

            list += New NamedValue(Of List(Of String)) With {
                .Name = tag,
                .Value = values
            }

            Return New Drug With {
               .Entry = list("ENTRY").Value.FirstOrDefault,
               .Names = list("NAME").Value,
               .Formula = list("FORMULA").Value.FirstOrDefault,
               .Exact_Mass = Val(list("EXACT_MASS").Value.FirstOrDefault),
               .Mol_Weight = Val(list("MOL_WEIGHT").Value.FirstOrDefault),
               .Remarks = list("REMARKS").Value,
               .DBLinks = list("DBLINKS").Value _
                   .Select(AddressOf DBLink.FromTagValue) _
                   .ToArray,
               .Activity = list("ACTIVITY").Value.FirstOrDefault,
               .Atoms = __atoms(list("ATOM").Value),
               .Bounds = __bounds(list("BOUND").Value)
            }
        End Function

        Private Function __atoms(lines$()) As Atom()
            Dim n = lines(Scan0).ParseInteger

            If n = 0 Then
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
                    .Edit = t(5)
                }
            Next

            Return out
        End Function

        Private Function __bounds(lines$()) As Bound()
            Dim n = lines(Scan0).ParseInteger

            If n = 0 Then
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
                    .Edit = t(4)
                }
            Next

            Return out
        End Function
    End Module
End Namespace
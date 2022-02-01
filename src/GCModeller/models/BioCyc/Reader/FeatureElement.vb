Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Linq.Extensions

Public Class FeatureElement : Implements IReadOnlyId

    Public ReadOnly Property uniqueId As String Implements IReadOnlyId.Identity
        Get
            Return attributes("UNIQUE-ID").First.value
        End Get
    End Property

    Public Property attributes As Dictionary(Of String, ValueString())

    Default Public ReadOnly Property getValue(key As String) As ValueString()
        Get
            Return attributes.TryGetValue(key)
        End Get
    End Property

    Public Overrides Function ToString() As String
        Return uniqueId
    End Function

    Private Shared Function removeBreakLines(buffer As String()) As IEnumerable(Of String)
        Dim newBuf As New List(Of String)

        For i As Integer = 0 To buffer.Length - 1
            If buffer(i).StartsWith("/") Then
                newBuf(newBuf.Count - 1) &= buffer(i).Trim("/"c, " "c)
            Else
                newBuf.Add(buffer(i))
            End If
        Next

        Return newBuf
    End Function

    Friend Shared Function ParseBuffer(buffer As String()) As FeatureElement
        Dim attrs As New List(Of NamedValue(Of ValueString))
        Dim innerAttrs As New List(Of NamedValue(Of String))
        Dim temp As NamedValue(Of String) = Nothing

        buffer = removeBreakLines(buffer).ToArray

        For Each line As String In buffer
            If line.StartsWith("^") Then
                ' is attribute value of the previous string value
                Call innerAttrs.Add(line.Trim("^").GetTagValue(" - "))
            Else
                If Not temp.IsEmpty Then
                    Call attrs.Add(New NamedValue(Of ValueString) With {
                        .Name = temp.Name,
                        .Value = New ValueString With {
                            .value = temp.Value,
                            .attributes = innerAttrs.PopAll
                        }
                    })
                End If

                temp = line.GetTagValue(" - ")
            End If
        Next

        Call attrs.Add(New NamedValue(Of ValueString) With {
            .Name = temp.Name,
            .Value = New ValueString With {
                .value = temp.Value,
                .attributes = innerAttrs.PopAll
            }
        })

        Return New FeatureElement With {
            .attributes = attrs _
                .GroupBy(Function(s) s.Name) _
                .ToDictionary(Function(s) s.Key,
                              Function(s)
                                  Return (From a As NamedValue(Of ValueString) In s Select a.Value).ToArray
                              End Function)
        }
    End Function

End Class

Public Class ValueString

    Public Property value As String
    Public Property attributes As NamedValue(Of String)()

    Default Public ReadOnly Property getValue(key As String) As String
        Get
            Return attributes _
                .Where(Function(a) a.Name = key) _
                .FirstOrDefault _
                .Value
        End Get
    End Property

    Public Overrides Function ToString() As String
        Return value
    End Function

    Public Shared Operator &(val As ValueString, str As String) As ValueString
        val.value &= str
        Return val
    End Operator

End Class
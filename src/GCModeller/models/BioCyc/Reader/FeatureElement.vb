Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Linq.Extensions

Public Class FeatureElement : Implements IReadOnlyId

    Public ReadOnly Property uniqueId As String Implements IReadOnlyId.Identity
        Get
            Return attributes("UNIQUE-ID").First
        End Get
    End Property

    Public Property attributes As Dictionary(Of String, String())

    Default Public ReadOnly Property getValue(key As String) As String()
        Get
            Return attributes.TryGetValue(key)
        End Get
    End Property

    Public Overrides Function ToString() As String
        Return uniqueId
    End Function

    Friend Shared Function ParseBuffer(buffer As String()) As FeatureElement
        Dim newBuf As New List(Of String)

        For i As Integer = 0 To buffer.Length - 1
            If buffer(i).StartsWith("/") Then
                newBuf(newBuf.Count - 1) &= buffer(i).Trim("/"c, " "c)
            Else
                newBuf.Add(buffer(i))
            End If
        Next

        Return New FeatureElement With {
            .attributes = newBuf _
                .Select(Function(str) str.GetTagValue(" - ")) _
                .GroupBy(Function(s) s.Name) _
                .ToDictionary(Function(s) s.Key,
                              Function(s)
                                  Return (From a As NamedValue(Of String) In s Select a.Value).ToArray
                              End Function)
        }
    End Function

End Class
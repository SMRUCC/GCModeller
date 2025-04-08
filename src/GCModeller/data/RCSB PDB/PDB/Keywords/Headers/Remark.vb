Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel

Namespace Keywords

    Public Class Remark : Inherits Keyword

        Public Overrides ReadOnly Property Keyword As String
            Get
                Return Keywords.KEYWORD_REMARK
            End Get
        End Property

        Public Property IndexedText As Dictionary(Of String, String)

        Dim cache As New List(Of NamedValue(Of String))

        Friend Shared Function Append(ByRef remark As Remark, str As String) As Remark
            If remark Is Nothing Then
                remark = New Remark
            End If
            remark.cache.Add(str.GetTagValue(" ", trim:=True))
            Return remark
        End Function

        Friend Overrides Sub Flush()
            IndexedText = cache _
                .GroupBy(Function(line) line.Name) _
                .ToDictionary(Function(a) a.Key,
                              Function(a)
                                  Return a.Select(Function(t) t.Value).JoinBy(vbCrLf)
                              End Function)
        End Sub

    End Class
End Namespace
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel

Namespace Keywords

    Public Class Journal : Inherits Keyword

        Public Overrides ReadOnly Property Keyword As String
            Get
                Return Keywords.KEYWORD_JRNL
            End Get
        End Property

        Dim cache As New List(Of NamedValue(Of String))

        Public Property author As String()
        Public Property title As String
        Public Property ref As String
        Public Property refn As String
        Public Property pmid As String
        Public Property doi As String

        Friend Shared Function Append(ByRef jrnl As Journal, str As String) As Journal
            If jrnl Is Nothing Then
                jrnl = New Journal
            End If
            jrnl.cache.Add(str.GetTagValue(" ", trim:=True, failureNoName:=False))
            Return jrnl
        End Function

        Friend Overrides Sub Flush()
            Dim cache = Me.cache _
                .GroupBy(Function(a) a.Name) _
                .Select(Function(a) (a.Key, a _
                    .Select(Function(t)
                                Dim numprefix = t.Value.Match("\d+\s+")

                                If numprefix.Length > 0 AndAlso InStr(t.Value, numprefix) = 1 Then
                                    Return t.Value.Substring(numprefix.Length).Trim
                                End If

                                Return t.Value
                            End Function) _
                    .JoinBy(" "))) _
                .ToArray

            For Each tuple As (name$, value$) In cache
                Select Case tuple.name
                    Case "AUTH" : author = tuple.value.Split(","c)
                    Case "TITL" : title = tuple.value
                    Case "REF" : ref = tuple.value
                    Case "REFN" : refn = tuple.value
                    Case "PMID" : pmid = tuple.value
                    Case "DOI" : doi = tuple.value
                    Case Else
                        Throw New NotImplementedException(tuple.name)
                End Select
            Next
        End Sub
    End Class
End Namespace
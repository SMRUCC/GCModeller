Imports Microsoft.VisualBasic.Serialization.JSON

Namespace Keywords

    Public Class Revision : Inherits Keyword

        Public Overrides ReadOnly Property Keyword As String
            Get
                Return Keywords.KEYWORD_REVDAT
            End Get
        End Property

        Public Property Versions As New List(Of RevVersion)

        Public Shared Function Append(ByRef rev As Revision, str As String) As Revision
            If rev Is Nothing Then
                rev = New Revision With {
                    .Versions = New List(Of RevVersion)
                }
            End If
            Call rev.Versions.Add(RevVersion.Parse(str))
            Return rev
        End Function

    End Class

    Public Class RevVersion

        Public Property [date] As String
        Public Property modify As String()

        Public Overrides Function ToString() As String
            Return $"({[date]}) {modify.GetJson}"
        End Function

        Friend Shared Function Parse(str As String) As RevVersion
            Dim t = str.StringSplit("\s+")
            Dim dat = t(1)
            Dim modifys = t.Skip(4).ToArray

            Return New RevVersion With {
                .[date] = dat,
                .modify = modifys
            }
        End Function

    End Class
End Namespace
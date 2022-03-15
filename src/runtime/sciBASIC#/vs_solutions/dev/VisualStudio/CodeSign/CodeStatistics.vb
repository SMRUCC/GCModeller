Imports System.Text
Imports Microsoft.VisualBasic.Text

Namespace CodeSign

    Public Class CodeStatics

        Public Property totalLines As Integer
        Public Property commentLines As Integer
        Public Property blankLines As Integer
        Public Property size As Double

        Public ReadOnly Property lineOfCodes As Integer
            Get
                Return totalLines - commentLines - blankLines
            End Get
        End Property

        Public Shared Function StatVB(code As String) As CodeStatics
            Dim lines = code.LineTokens.Select(Function(str) str.Trim(" "c, ASCII.TAB)).ToArray
            Dim stat As New CodeStatics With {
                .totalLines = lines.Length,
                .blankLines = lines.Where(Function(str) str.StringEmpty).Count,
                .commentLines = lines.Where(Function(str) Not str.StringEmpty AndAlso str.StartsWith("'")).Count,
                .size = Encoding.UTF8.GetBytes(code).Length
            }

            Return stat
        End Function

    End Class
End Namespace
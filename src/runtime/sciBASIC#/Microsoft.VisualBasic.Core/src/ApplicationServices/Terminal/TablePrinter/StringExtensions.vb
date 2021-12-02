Imports System.Runtime.CompilerServices

Namespace ApplicationServices.Terminal.TablePrinter
    Public Module StringExtensions
        <Extension()>
        Public Function RealLength(ByVal value As String, ByVal withUtf8Characters As Boolean) As Integer
            If String.IsNullOrEmpty(value) Then Return 0
            If Not withUtf8Characters Then Return value.Length
            Dim i = 0 'count

            For Each newChar In value.Select(AddressOf AscW)
                If newChar >= &H4E00 AndAlso newChar <= &H9FBB Then
                    'utf-8 characters
                    i += 2
                Else
                    i += 1
                End If
            Next

            Return i
        End Function
    End Module
End Namespace

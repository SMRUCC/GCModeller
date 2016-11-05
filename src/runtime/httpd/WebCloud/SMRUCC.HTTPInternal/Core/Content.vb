
Imports System.IO
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace Core

    Public Structure Content

        Public Property Length As Integer

        ''' <summary>
        ''' 不需要在这里写入http头部
        ''' </summary>
        ''' <returns></returns>
        Public Property Type As String
        Public Property attachment As String

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function

        Public Sub WriteHeader(outputStream As StreamWriter)
            If Length > 0 Then
                Call outputStream.WriteLine("Content-Length: " & Length)
            End If
            If Not String.IsNullOrEmpty(attachment) Then
                Call outputStream.WriteLine($"Content-Disposition: attachment;filename=""{attachment}""")
            End If
        End Sub
    End Structure
End Namespace
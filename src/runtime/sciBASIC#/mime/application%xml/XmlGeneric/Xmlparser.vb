Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Text
Imports Microsoft.VisualBasic.Text.Parser

Public Class XmlParser

    Dim stringEscape As Boolean = False
    Dim text As CharPtr
    Dim buffer As New CharBuffer

    Sub New(text As CharPtr)
        Me.text = text
    End Sub

    ''' <summary>
    ''' Parse the xml tree
    ''' </summary>
    ''' <returns></returns>
    Public Function ParseXml() As XmlElement
        Dim flag As Value(Of Boolean) = True

        Do While text
            If (flag = walkChar(c:=++text)) = True Then

            End If
        Loop
    End Function

    Private Function walkChar(c As Char) As Boolean
        If stringEscape Then
            If c = ASCII.Quot Then
                stringEscape = False
                buffer += c

                Return True
            Else
                buffer += c
                Return False
            End If
        End If

        If c = "<"c Then
            If buffer = 0 Then

            End If
        End If
    End Function
End Class

Public Enum XmlTokens
    name
    text
    ''' <summary>
    ''' =
    ''' </summary>
    attrAssign
    ''' <summary>
    ''' &lt;
    ''' </summary>
    open
    ''' <summary>
    ''' &gt;
    ''' </summary>
    close
End Enum
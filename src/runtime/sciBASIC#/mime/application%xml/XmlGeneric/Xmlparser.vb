Imports Microsoft.VisualBasic.Text
Imports Microsoft.VisualBasic.Text.Parser

Public Class XmlParser

    Dim stringEscape As Boolean = False
    Dim text As CharPtr
    Dim buffer As List(Of Char)

    Sub New(text As CharPtr)
        Me.text = text
    End Sub

    ''' <summary>
    ''' Parse the xml tree
    ''' </summary>
    ''' <returns></returns>
    Public Function ParseXml() As XmlElement

    End Function

    Private Function walkChar(c As Char)
        If stringEscape Then
            If c = ASCII.Quot Then
                stringEscape = False

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
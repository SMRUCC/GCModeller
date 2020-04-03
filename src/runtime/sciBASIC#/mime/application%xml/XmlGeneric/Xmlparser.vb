Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Scripting.TokenIcer
Imports Microsoft.VisualBasic.Text
Imports Microsoft.VisualBasic.Text.Parser

Public Class XmlParser

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
        Dim flag As New Value(Of XmlToken)
        Dim node As New XmlElement

        Call moveToNextTagOpen()
        Call readCurrentTagName()

        node.name = buffer.ToString

        Call readCurrentAttributeName()

    End Function

    Private Sub moveToNextTagOpen()
        Dim c As Value(Of Char) = ASCII.NUL

        Do While text
            If (c = ++text) = "<"c Then
                Exit Do
            End If
        Loop
    End Sub

    Private Sub readCurrentAttributeName()
        Dim c As Value(Of Char) = ASCII.NUL

        Do While text
            If (c = ++text) <> "="c Then
                buffer += c
            Else
                Exit Do
            End If
        Loop
    End Sub

    Private Sub readCurrentAttributeValue()
        Dim c As Value(Of Char) = ASCII.NUL

        Do While text
            If (c = ++text) <> ASCII.Quot Then
                buffer += c
            Else
                Exit Do
            End If
        Loop
    End Sub

    Private Sub readCurrentElementText()
        Dim c As Value(Of Char) = ASCII.NUL

        ' > ..... <
        Do While text
            If (c = ++text) <> "<"c Then
                buffer += c
            Else
                Exit Do
            End If
        Loop
    End Sub

    Private Sub readCurrentTagName()
        Dim c As Value(Of Char) = ASCII.NUL

        ' read current xml tag name
        Do While text
            If (c = ++text) <> " "c Then
                buffer += c
            Else
                Exit Do
            End If
        Loop
    End Sub
End Class

Public Class XmlToken : Inherits CodeToken(Of XmlTokens)

    Sub New(token As XmlTokens, text As String)
        Call MyBase.New(token, text)
    End Sub
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
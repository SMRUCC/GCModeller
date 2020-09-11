Imports System.Drawing

Namespace HTML.CSS

    Public Class CSSEnvirnment

        Public ReadOnly Property baseFont As Font

        Sub New(basefont As Font)
            Me.baseFont = basefont
        End Sub

    End Class
End Namespace
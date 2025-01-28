Imports Microsoft.VisualBasic.Imaging.Drawing2D.Shapes

Namespace PostScript

    ''' <summary>
    ''' An postscript graphics element
    ''' </summary>
    Public MustInherit Class PSElement(Of S As Shape)

        Public Property shape As S

    End Class

End Namespace


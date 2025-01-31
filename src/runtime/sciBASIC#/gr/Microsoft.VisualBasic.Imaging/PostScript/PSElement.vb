Imports Microsoft.VisualBasic.Imaging.Drawing2D.Shapes

Namespace PostScript

    Public MustInherit Class PSElement

    End Class

    ''' <summary>
    ''' An postscript graphics element
    ''' </summary>
    Public MustInherit Class PSElement(Of S As Shape) : Inherits PSElement

        Public Property shape As S

    End Class

End Namespace


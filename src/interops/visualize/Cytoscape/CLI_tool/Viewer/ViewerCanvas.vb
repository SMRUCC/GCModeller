Imports System.Windows.Forms
Imports Microsoft.VisualBasic.DataVisualization.Network.Canvas

Public Class ViewerCanvas

    Dim WithEvents canvas As New Canvas With {
        .Dock = DockStyle.Fill
    }

    Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        Controls.Add(canvas)
        canvas.BringToFront()
    End Sub
End Class
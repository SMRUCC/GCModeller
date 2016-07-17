Imports SMRUCC.genomics.Visualize.PDB_canvas

Public Class Form_molView

    Dim WithEvents Canvas As Canvas

    Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        Canvas = New Canvas With {
            .Dock = DockStyle.Fill
        }
        Call Controls.Add(Canvas)
    End Sub

    Private Sub OpenToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles OpenToolStripMenuItem.Click
        Using file As New OpenFileDialog
            If file.ShowDialog = DialogResult.OK Then
                Call Canvas.LoadModel(file.FileName)
            End If
        End Using
    End Sub
End Class

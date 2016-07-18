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
        Canvas.BringToFront()
#If DEBUG Then
        Canvas.LoadModel("G:\GCModeller\GCModeller\Data\pdb_Draw\XC_1184_pdb.txt")
#End If
    End Sub

    Private Sub OpenToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles OpenToolStripMenuItem.Click
        Using file As New OpenFileDialog
            If file.ShowDialog = DialogResult.OK Then
                Call Canvas.LoadModel(file.FileName)
            End If
        End Using
    End Sub

    Private Sub AutoRotationToolStripMenuItem_CheckedChanged(sender As Object, e As EventArgs) Handles AutoRotationToolStripMenuItem.CheckedChanged
        If Not Canvas Is Nothing Then
            Canvas.AutoRotate = AutoRotationToolStripMenuItem.Checked
        End If
    End Sub
End Class

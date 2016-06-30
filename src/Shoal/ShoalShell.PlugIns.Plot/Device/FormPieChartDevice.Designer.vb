<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class FormPieChartDevice
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose( disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.PieChart1 = New Microsoft.VisualBasic.DataVisualization.Enterprise.Windows.Forms.Nexus.PieChart()
        Me.SuspendLayout()
        '
        'PieChart1
        '
        Me.PieChart1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PieChart1.Location = New System.Drawing.Point(0, 0)
        Me.PieChart1.Name = "PieChart1"
        Me.PieChart1.Radius = 200.0!
        Me.PieChart1.Size = New System.Drawing.Size(664, 549)
        Me.PieChart1.TabIndex = 0
        Me.PieChart1.Text = "PieChart1"
        Me.PieChart1.Thickness = 10.0!
        '
        'FormPieChart
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(664, 549)
        Me.Controls.Add(Me.PieChart1)
        Me.Name = "FormPieChart"
        Me.Text = "Form1"
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents PieChart1 As Microsoft.VisualBasic.DataVisualization.Enterprise.Windows.Forms.Nexus.PieChart
End Class

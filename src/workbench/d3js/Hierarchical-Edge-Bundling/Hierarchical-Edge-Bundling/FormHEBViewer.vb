Imports Microsoft.VisualBasic.Windows.Forms
Imports Microsoft.VisualBasic.Parallel
Imports SMRUCC.HTTPInternal.Core
Imports Microsoft.VisualBasic.Serialization
Imports System.Text

Public Class FormHEBViewer

    Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        ToolStripManager.Renderer = New ChromeUIRender
    End Sub

    Dim __services As New HttpFileSystem(8523, App.AppSystemTemp, False, AddressOf __getValue)
    Dim data As Byte() = {}

    Private Function __getValue(res As String) As Byte()
        If InStr(res, "readme-flare-imports.json", CompareMethod.Text) > 0 Then
            Return data
        End If
        Return __services.GetResource(res)
    End Function

    Private Sub FormHEBViewer_Load(sender As Object, e As EventArgs) Handles Me.Load
        Call My.Resources.index.SaveTo(App.AppSystemTemp & "/index.html")
        Call My.Resources.d3_v3_min.SaveTo(App.AppSystemTemp & "/d3.v3.min.js")
        ' Call RunTask(AddressOf __services.Run)
        Call WebBrowser1.Navigate("http://127.0.0.1")
    End Sub

    Private Sub OpenToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles OpenToolStripMenuItem.Click
        Using open As New OpenFileDialog With {.Filter = "Network data(*.csv)|*.csv"}
            If open.ShowDialog = DialogResult.OK Then
                Dim json As String = LoadAs(open.FileName).GetJson
                data = Encoding.UTF8.GetBytes(json)
                Call WebBrowser1.Navigate("http://127.0.0.1:8523/")

#If DEBUG Then
                Call json.SaveTo(App.GetAppSysTempFile(".json"))
#End If
            End If
        End Using
    End Sub
End Class

#Region "Microsoft.VisualBasic::988fc299981f1bd0bc8c96af1268137e, ..\workbench\devenv\Forms\FormMain\RWizard.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program. If not, see <http://www.gnu.org/licenses/>.

#End Region

Public Class RWizard

    Dim Target As String, TargetFormat As String
    Dim ExistsModel As String
    Dim Save As String

    Private Sub RWizard_Load(sender As Object, e As EventArgs) Handles Me.Load
        Me.TopMost = True
        Button2.Enabled = False
    End Sub

    Private Sub LinkLabel1_LinkClicked(sender As Object, e As System.Windows.Forms.LinkLabelLinkClickedEventArgs) Handles LinkLabel1.LinkClicked
        Using File As New System.Windows.Forms.OpenFileDialog
            File.Filter = "NCBI Genbank Flat File(*.gbk;*.gb)|*.gbk;*.gb|FASTA Protein Sequence File(*.fasta;*.fsa)|*.fasta;*.fsa"
            If File.ShowDialog = System.Windows.Forms.DialogResult.OK Then
                Target = File.FileName
                If InStr("*.gbk;*.gb", System.IO.Path.GetExtension(Target)) Then
                    TargetFormat = "gbk"
                Else
                    TargetFormat = "fsa"
                End If

                Label2.Text = String.Format("Select target from a data file of type ({0}):{1}  {2}", TargetFormat, vbCrLf, Target)
            End If
        End Using

        Call CheckCondition()
    End Sub

    Private Sub CheckCondition()
        If String.IsNullOrEmpty(Target) OrElse String.IsNullOrEmpty(ExistsModel) Then
            Return
        Else
            If String.IsNullOrEmpty(TextBox1.Text) OrElse String.IsNullOrEmpty(TextBox2.Text) Then
                Return
            End If

            If String.IsNullOrEmpty(Save) Then Return

            Button2.Enabled = True
        End If
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Close()
    End Sub

    Private Sub LinkLabel2_LinkClicked(sender As Object, e As System.Windows.Forms.LinkLabelLinkClickedEventArgs) Handles LinkLabel2.LinkClicked
        Dim Dir As String = Methods.SelectFolder

        If Not String.IsNullOrEmpty(Dir) Then
            Target = Dir
            TargetFormat = "metacyc"
            Label2.Text = String.Format("Select target from a data file of type ({0}):{1}  {2}", TargetFormat, vbCrLf, Target)
        End If

        Call CheckCondition()
    End Sub

    Private Sub LinkLabel3_LinkClicked(sender As Object, e As System.Windows.Forms.LinkLabelLinkClickedEventArgs) Handles LinkLabel3.LinkClicked
        Dim Dir As String = Methods.SelectFolder

        If Not String.IsNullOrEmpty(Dir) Then
            ExistsModel = Dir
            Label1.Text = String.Format("Selected MetaCyc Model, Load from data directory:{0}  '{1}'", vbCrLf, Dir)
        End If

        Call CheckCondition()
    End Sub

    Const BUILD2_COMMAND As String = "build2 -m ""{0}"" -o ""{1}"" -t ""{2}"" -grep_m ""{3}"" -grep_t ""{4}"" -f {5}"

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        LinkLabel1.Enabled = False
        LinkLabel2.Enabled = False
        LinkLabel3.Enabled = False
        Button2.Enabled = False

        LinkLabel4.Enabled = False
        TextBox1.Enabled = False
        TextBox2.Enabled = False

        Me.Text = "Performing the reengineering operation..."
        Dim CommandLine As String = String.Format(BUILD2_COMMAND, Target, Save, ExistsModel, TextBox1.Text, TextBox2.Text, TargetFormat)
        Dim c2 As Microsoft.VisualBasic.CommandLine.IORedirect =
            New Microsoft.VisualBasic.CommandLine.IORedirect(My.Application.Info.DirectoryPath & "/c2.exe", CommandLine)
        AddHandler c2.DataArrival, Sub(s As String) Call Out(s, ConsoleColor.White, "c2->build2", Microsoft.VisualBasic.Logging.MSG_TYPES.INF)
        AddHandler c2.ProcessExit, Sub()
                                       Close()
                                       MsgBox("Job done!", MsgBoxStyle.Information)
                                   End Sub
        c2.Start(False)
    End Sub

    Private Sub LinkLabel4_LinkClicked(sender As Object, e As System.Windows.Forms.LinkLabelLinkClickedEventArgs) Handles LinkLabel4.LinkClicked
        Using File As New System.Windows.Forms.SaveFileDialog
            File.Filter = "GCModel markup language file(*.gcml.xml;*.xml)|*.gcml.xml;*.xml"
            If File.ShowDialog = System.Windows.Forms.DialogResult.OK Then
                Save = File.FileName

                Label10.Text = String.Format("Save location was select to:{0}  {1}", vbCrLf, Save)
            End If
        End Using

        Call CheckCondition()
    End Sub

    Private Sub TextBox1_TextChanged(sender As Object, e As EventArgs) Handles TextBox1.TextChanged
        CheckCondition()
    End Sub

    Private Sub TextBox2_TextChanged(sender As Object, e As EventArgs) Handles TextBox2.TextChanged
        CheckCondition()
    End Sub
End Class

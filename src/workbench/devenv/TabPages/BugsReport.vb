Imports Microsoft.VisualBasic

Public Class BugsReport

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim GMail = Microsoft.VisualBasic.Net.Mailto.EMailClient.GmailClient("", "")
        Dim Message As New Microsoft.VisualBasic.Net.Mailto.MailContents

        Message.Subject = "GCModeller Bugs Report"
        Message.Attatchments = (From s As String In ListBox1.Items Select s).ToList

        Dim f As Boolean = GMail.SendEMail(Message, "xie.guigang@gmail.com")

        If f = False Then
            Call IDEStatueText("Bugs information feedback report send failure, please check your internet connection...", Drawing.Color.Yellow)
        End If
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        If OpenAttachmentFile.ShowDialog = System.Windows.Forms.DialogResult.OK Then
            ListBox1.Items.Add(OpenAttachmentFile.FileName)
        End If
    End Sub

    Private Sub BugsReport_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        OpenAttachmentFile.Filter = "All type file(*.*)|*.*"
    End Sub
End Class

#Region "Microsoft.VisualBasic::63f14daf580498f4d8065a827dc01584, ..\workbench\devenv\TabPages\BugsReport.vb"

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

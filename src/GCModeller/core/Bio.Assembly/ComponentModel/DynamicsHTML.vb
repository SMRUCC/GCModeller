#Region "Microsoft.VisualBasic::4d70dc57d7df888f52cbcd134639b399, ..\GCModeller\core\Bio.Assembly\ComponentModel\DynamicsHTML.vb"

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

Imports System.Windows.Forms
Imports Microsoft.VisualBasic.Terminal.Utility

Namespace ComponentModel

    Public NotInheritable Class DynamicsHTML : Inherits System.Windows.Forms.Form

        Public Shared Function DownloadDynamicsObject(url As String, DownloadedActionCallBack As Func(Of String, Object)) As DynamicsHTML
            Dim Instance = New DynamicsHTML With {
                .DownloadedAction = DownloadedActionCallBack,
                .DownloadedURL = url
            }
            Call Instance.ShowDialog()
            Return Instance.DownloadResult
        End Function

        Dim WithEvents WB As WebBrowser = New WebBrowser With {.ScriptErrorsSuppressed = True}
        Dim DownloadedAction As Func(Of String, Object)
        Dim DownloadedURL As String
        Dim DownloadResult As Object

        Protected Sub New()
        End Sub

        Public Function DownloadPage(url As String) As String
            Call WB.Navigate(url)
            Call Console.WriteLine("Downloading page....")

            Using pb As CBusyIndicator = New CBusyIndicator(_start:=True)
WAITE:          If WB.ReadyState = WebBrowserReadyState.Complete Then  ' decode the string returned
                    Dim htm As HtmlDocument = WB.Document
                    Call Console.WriteLine("[DEBUG] Webbrowser is fully loaded.")

                    Return htm.Window.Document.Body.InnerHtml
                Else
                    Call Console.WriteLine(WB.Document.Body.InnerHtml)
                    Call Threading.Thread.Sleep(1000)
                    GoTo WAITE
                End If
            End Using
        End Function

        Private Sub DynamicsHTML_Load(sender As Object, e As EventArgs) Handles Me.Load
            CheckForIllegalCrossThreadCalls = False
            Call Me.Controls.Add(WB)
            WB.Dock = DockStyle.Fill
            WB.Navigate(DownloadedURL)
        End Sub

        Private Sub WB_DocumentCompleted(sender As Object, e As WebBrowserDocumentCompletedEventArgs) Handles WB.DocumentCompleted
REPEAT:     DownloadResult = DownloadedAction(WB.Document.Body.InnerHtml)

            If DownloadResult Is Nothing Then
                Call Threading.Thread.Sleep(10000)
                GoTo REPEAT
            Else
                Me.Close()
            End If
        End Sub
    End Class
End Namespace

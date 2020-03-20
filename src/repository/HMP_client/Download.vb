#Region "Microsoft.VisualBasic::fa88faf74d663cd9f234a935c362bbdb, HMP_client\Download.vb"

' Author:
' 
'       asuka (amethyst.asuka@gcmodeller.org)
'       xie (genetics@smrucc.org)
'       xieguigang (xie.guigang@live.com)
' 
' Copyright (c) 2018 GPL3 Licensed
' 
' 
' GNU GENERAL PUBLIC LICENSE (GPL3)
' 
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



' /********************************************************************************/

' Summaries:

' Module Download
' 
'     Function: HandleFileDownloads
' 
' /********************************************************************************/

#End Region

Imports System.Net
Imports System.Runtime.CompilerServices
Imports System.Threading
Imports Microsoft.VisualBasic.ApplicationServices.Terminal.ProgressBar
Imports Microsoft.VisualBasic.SecurityString
Imports Microsoft.VisualBasic.Serialization.JSON
Imports ThinkVB.FileSystem.OSS

Public Module Download

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <Extension>
    Private Function createLocalFileName(file As manifest, save$) As String
        Return save & "/" & Strings.Split(file.urls.Split(","c).First, "//").Last
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="manifests"></param>
    ''' <param name="save$">文件所保存的文件夹的路径</param>
    ''' <returns></returns>
    <Extension>
    Public Iterator Function HandleFileDownloads(manifests As IEnumerable(Of manifest), save$, Optional aspera As Aspera = Nothing) As IEnumerable(Of String)
        Dim url As String
        Dim downloadAs As String

        If aspera Is Nothing Then
            Call "Aspera client is not installed, file only download via http method....".Warning
        End If

        For Each file As manifest In manifests
            downloadAs = file.createLocalFileName(save)

            If file.md5 = downloadAs.GetFileMd5 Then
                ' 已经下载过了，跳过
                Continue For
            End If

            If aspera Is Nothing OrElse file.AsperaURL.StringEmpty Then
                ' 只能够使用http协议下载
HTTP:           url$ = file.HttpURL

                If url.StringEmpty Then
                    Yield "Empty resource: " & file.GetJson
                Else
                    Call httpRequest(url, downloadAs)
                    Call Thread.Sleep(2000)
                End If
            Else
                Call aspera.Download(file.AsperaURL, downloadAs)
                Call Thread.Sleep(1000)
            End If
        Next
    End Function

    Private Function progressHandle(progress As ProgressBar) As DownloadProgressChangedEventHandler
        Dim msg$

        Return Sub(sender, args)
                   msg$ = $"{args.BytesReceived}/{args.TotalBytesToReceive} bytes"
                   progress.SetProgress(args.ProgressPercentage, msg)
               End Sub
    End Function

    Private Sub httpRequest(url$, downloadAs$)
        Using progress As New ProgressBar(url, 1, True)
            Call url.DownloadFile(
                save:=downloadAs,
                progressHandle:=progressHandle(progress)
            )
        End Using
    End Sub
End Module

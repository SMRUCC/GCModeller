#Region "Microsoft.VisualBasic::5250d45938ca020ddab3f6efbe18a8ba, HMP_client\Download.vb"

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
    '     Function: createLocalFileName, HandleFileDownloads, runFileDownloader
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Threading
Imports Microsoft.VisualBasic.Net.Http
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
        Dim downloadAs As String

        If aspera Is Nothing Then
            Call "Aspera client is not installed, file only download via http method....".Warning
        End If

        For Each file As manifest In manifests
            downloadAs = file.createLocalFileName(save)

            If file.md5 = downloadAs.GetFileMd5 Then
                ' 已经下载过了，跳过
                Continue For
            Else
                Yield file.runFileDownloader(downloadAs, aspera)
            End If
        Next
    End Function

    <Extension>
    Private Function runFileDownloader(file As manifest, downloadAs$, aspera As Aspera) As String
        If aspera Is Nothing OrElse file.AsperaURL.StringEmpty Then
            ' 只能够使用http协议下载
HTTP:       Dim url As String = file.HttpURL

            If url.StringEmpty Then
                Return "empty resource: " & file.GetJson
            Else
                Call wget.Download(url, save:=downloadAs)
                Call Thread.Sleep(2000)
            End If
        Else
            Call aspera.Download(file.AsperaURL, downloadAs)
            Call Thread.Sleep(1000)
        End If

        If file.md5 = downloadAs.GetFileMd5 Then
            Return $"{downloadAs} download success!"
        Else
            Return $"{downloadAs} download failure!"
        End If
    End Function
End Module

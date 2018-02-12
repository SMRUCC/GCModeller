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
Imports Microsoft.VisualBasic.Serialization.JSON
Imports Microsoft.VisualBasic.Terminal.ProgressBar

Public Module Download

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="manifests"></param>
    ''' <param name="save$">文件所保存的文件夹的路径</param>
    ''' <returns></returns>
    <Extension>
    Public Iterator Function HandleFileDownloads(manifests As IEnumerable(Of manifest), save$) As IEnumerable(Of String)
        For Each file As manifest In manifests
            Dim url$ = file.HttpURL

            If url.StringEmpty Then
                Yield "Empty resource: " & file.GetJson
            Else
                Dim downloadAs$ = save & "/" & Strings.Split(url, "//").Last

                Using progress As New ProgressBar(url, 1, True)
                    Dim progressHandle As DownloadProgressChangedEventHandler =
 _
                        Sub(sender, args)
                            Dim msg$ = $"{args.BytesReceived}/{args.TotalBytesToReceive} bytes"
                            Call progress.SetProgress(args.ProgressPercentage, msg)
                        End Sub

                    Call url.DownloadFile(
                        save:=downloadAs,
                        progressHandle:=progressHandle)
                End Using
            End If
        Next
    End Function
End Module

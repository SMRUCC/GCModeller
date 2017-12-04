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

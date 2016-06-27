Imports LANS.SystemsBiology.Assembly.NCBI.Entrez
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Parallel
Imports Microsoft.VisualBasic.Terminal.Utility
Imports Microsoft.VisualBasic.Linq

Public Module DownloaderWebAPI

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="list">AccessionID列表</param>
    ''' <param name="EXPORT">保存的文件夹</param>
    ''' <returns>返回下载成功的文件数目</returns>
    ''' <remarks></remarks>
    ''' 
    <ExportAPI("genbank.batch_download",
               Info:="This command required the bioperl package installed on your computer!")>
    Public Function DownloadGBK(list As IEnumerable(Of String), EXPORT As String, num_threads As Integer) As Integer
        Using pb As New CBusyIndicator(_start:=True)
            Dim downloads As New __genbankDownloadHelper With {.EXPORT = EXPORT}
            Dim tasks As Action() = list.ToArray(AddressOf downloads.Download)
            Return tasks.Invoke(numOfThreads:=num_threads)
        End Using
    End Function

    Private Structure __genbankDownloadHelper

        Dim EXPORT As String

        Public Function Download(id As String) As Action
            Return AddressOf New __innerHelper With {
                .EXPORT = EXPORT,
                .id = id
            }.Download
        End Function

        Private Structure __innerHelper

            Dim EXPORT As String
            Dim id As String

            Public Sub Download()
                Dim gb As String = $"{EXPORT}/{id}.gb".GetFullPath
                Dim St As Stopwatch = Stopwatch.StartNew

                Call $"Threading start for ""{gb.ToFileURL}""".__DEBUG_ECHO

                If gb.FileExists Then
                    Return
                Else
                    gb = QueryHandler.Entry.DownloadGBK(EXPORT, id)
                End If

                If gb.FileExists Then
                    Call $"{id} was download at ""{St.ElapsedMilliseconds}ms"".".__DEBUG_ECHO
                Else
                    Call $"{id} was download not successfully!".Warning
                End If
            End Sub
        End Structure
    End Structure
End Module

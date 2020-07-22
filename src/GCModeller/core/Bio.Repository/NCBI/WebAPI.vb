#Region "Microsoft.VisualBasic::56df4e22d0600ec4a5061765fe207e51, Bio.Repository\NCBI\WebAPI.vb"

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

    ' Module DownloaderWebAPI
    ' 
    '     Function: DownloadGBK
    '     Structure __genbankDownloadHelper
    ' 
    '         Function: Download
    '         Structure __innerHelper
    ' 
    '             Sub: Download
    ' 
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ApplicationServices.Terminal.Utility
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Parallel
Imports SMRUCC.genomics.Assembly.NCBI

Public Module DownloaderWebAPI

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="list">AccessionID列表</param>
    ''' <param name="EXPORT">保存的文件夹</param>
    ''' <returns>返回下载成功的文件数目</returns>
    ''' <remarks></remarks>
    ''' 
    Public Function DownloadGBK(list As IEnumerable(Of String), EXPORT As String, num_threads As Integer) As Integer
        Using pb As New CBusyIndicator(start:=True)
            Dim downloads As New __genbankDownloadHelper With {.EXPORT = EXPORT}
            Dim tasks As Action() = list.Select(AddressOf downloads.Download).ToArray
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
                    Call Entrez.Genbank.Fetch(id, gb, True, True)
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

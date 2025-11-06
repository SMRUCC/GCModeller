#Region "Microsoft.VisualBasic::9d500cb4f3428488b3372b1d3b5eda14, Bio.Repository\NCBI\WebAPI.vb"

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


    ' Code Statistics:

    '   Total Lines: 59
    '    Code Lines: 40 (67.80%)
    ' Comment Lines: 7 (11.86%)
    '    - Xml Docs: 71.43%
    ' 
    '   Blank Lines: 12 (20.34%)
    '     File Size: 1.89 KB


    ' Module DownloaderWebAPI
    ' 
    '     Sub: DownloadGBK
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
Imports SMRUCC.genomics.Assembly.NCBI

Public Module DownloaderWebAPI

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="list">AccessionID列表</param>
    ''' <param name="EXPORT">保存的文件夹</param>
    ''' <remarks></remarks>
    ''' 
    Public Sub DownloadGBK(list As IEnumerable(Of String), EXPORT As String, num_threads As Integer)
        Using pb As New CBusyIndicator(start:=True)
            Dim downloads As New __genbankDownloadHelper With {.EXPORT = EXPORT}

            For Each item As String In list
                Call downloads.Download(item)
            Next
        End Using
    End Sub

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

                Call $"Threading start for ""{gb.ToFileURL}""".debug

                If gb.FileExists Then
                    Return
                Else
                    Call Entrez.Genbank.Fetch(id, gb, True, True)
                End If

                If gb.FileExists Then
                    Call $"{id} was download at ""{St.ElapsedMilliseconds}ms"".".debug
                Else
                    Call $"{id} was download not successfully!".Warning
                End If
            End Sub
        End Structure
    End Structure
End Module

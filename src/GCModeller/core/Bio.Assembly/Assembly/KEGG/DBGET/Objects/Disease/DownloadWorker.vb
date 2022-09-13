#Region "Microsoft.VisualBasic::65da203a40766b5982edfa8c1384c498, GCModeller\core\Bio.Assembly\Assembly\KEGG\DBGET\Objects\Disease\DownloadWorker.vb"

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

    '   Total Lines: 51
    '    Code Lines: 36
    ' Comment Lines: 6
    '   Blank Lines: 9
    '     File Size: 2.02 KB


    '     Module DownloadWorker
    ' 
    '         Function: DownloadDisease
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ApplicationServices.Terminal.ProgressBar
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.BriteHEntry

Namespace Assembly.KEGG.DBGET.bGetObject

    Public Module DownloadWorker

        ''' <summary>
        ''' 批量下载KEGG之上的人类疾病的数据
        ''' </summary>
        ''' <param name="htext"></param>
        ''' <param name="EXPORT$"></param>
        ''' <returns></returns>
        <Extension>
        Public Function DownloadDisease(htext As htext, EXPORT$) As String()
            Dim all As BriteHText() = htext.Hierarchical.EnumerateEntries.ToArray
            Dim failures As New List(Of String)

            Using progress As New ProgressBar("Download KEGG disease data...", 1, CLS:=True)
                Dim tick As New ProgressProvider(progress, all.Length)
                Dim path As New Value(Of String)
                Dim disease As Disease

                For Each entry As BriteHText In all

                    If Not (path = entry.BuildPath(EXPORT)).FileExists(ZERO_Nonexists:=True) Then
                        Try
                            disease = DownloadDiseases.Download(entry.entryID)
                            disease.SaveAsXml(path, , Encodings.ASCII)
                        Catch ex As Exception
                            failures += entry.entryID
                            ex = New Exception(entry.entryID & " " & entry.description, ex)
                            App.LogException(ex)
                        End Try
                    End If

                    Dim ETA$ = tick.ETA().FormatTime

                    Call progress.SetProgress(
                        tick.StepProgress(),
                        $"{entry.description}, ETA={ETA}")
                Next
            End Using

            Return failures
        End Function
    End Module
End Namespace

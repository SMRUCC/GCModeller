#Region "Microsoft.VisualBasic::4e025d690fd3afcdaf89825e5969e6c0, ..\GCModeller\core\Bio.Assembly\Assembly\KEGG\DBGET\Objects\Disease\DownloadWorker.vb"

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

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Terminal
Imports Microsoft.VisualBasic.Terminal.ProgressBar
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
                Dim tick As New ProgressProvider(all.Length)
                Dim path As New Value(Of String)
                Dim disease As Disease

                For Each entry As BriteHText In all

                    If Not (path = entry.BuildPath(EXPORT)).FileExists(ZERO_Nonexists:=True) Then
                        Try
                            disease = DownloadDiseases.Download(entry.EntryId)
                            disease.SaveAsXml(path, , Encodings.ASCII)
                        Catch ex As Exception
                            failures += entry.EntryId
                            ex = New Exception(entry.EntryId & " " & entry.Description, ex)
                            App.LogException(ex)
                        End Try
                    End If

                    Dim ETA$ = tick _
                        .ETA(progress.ElapsedMilliseconds) _
                        .FormatTime

                    Call progress.SetProgress(
                        tick.StepProgress(),
                        $"{entry.Description}, ETA={ETA}")
                Next
            End Using

            Return failures
        End Function
    End Module
End Namespace

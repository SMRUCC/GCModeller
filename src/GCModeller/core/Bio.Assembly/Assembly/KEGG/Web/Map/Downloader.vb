#Region "Microsoft.VisualBasic::03ae3a414f933fcb5f8d3460f40e45f9, ..\GCModeller\core\Bio.Assembly\Assembly\KEGG\Web\Map\Downloader.vb"

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
Imports System.Text.RegularExpressions
Imports System.Threading
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Terminal
Imports Microsoft.VisualBasic.Terminal.ProgressBar
Imports Microsoft.VisualBasic.Text
Imports PathwayEntry = SMRUCC.genomics.Assembly.KEGG.DBGET.BriteHEntry.Pathway

Namespace Assembly.KEGG.WebServices

    ''' <summary>
    ''' KEGG reference map downloader API
    ''' </summary>
    Public Module MapDownloader

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Private Function loadEntryAuto(file As String) As PathwayEntry()
            If file.FileExists Then
                Return PathwayEntry.LoadData(file)
            Else
                Return PathwayEntry.LoadFromResource()
            End If
        End Function

        ''' <summary>
        ''' Download the KEGG reference map data.
        ''' </summary>
        ''' <param name="EXPORT$"></param>
        ''' <param name="briefFile$"></param>
        ''' <returns></returns>
        Public Function Downloads(EXPORT$, Optional briefFile$ = Nothing) As String()
            Dim briefEntries As PathwayEntry() = loadEntryAuto(briefFile)
            Dim failures As New List(Of String)
            Dim tick As New ProgressProvider(briefEntries.Length)
            Dim msg$
            Dim getID = Function(entry As PathwayEntry)
                            If briefFile Is Nothing Then
                                Return "map" & entry.EntryId
                            Else
                                Dim s = entry.Entry.Value
                                s = Regex.Match(s, "\[PATH:.+?\]", RegexICSng).Value
                                s = s.GetStackValue("[", "]").Split(":"c).Last
                                Return s
                            End If
                        End Function

            Using progress As New ProgressBar("Downloads KEGG pathway map data...", 1, CLS:=True)
                For Each entry In briefEntries
                    Dim id$ = getID(entry)
                    Dim url$ = $"http://www.genome.jp/kegg-bin/show_pathway?{id}"
                    Dim save$ = EXPORT & $"/{id}.XML"

                    If id.StringEmpty Then
                        Continue For
                    End If

                    Try
                        If Not save.FileExists(True) Then
                            Call Map.ParseHTML(url) _
                                .GetXml _
                                .SaveTo(save, TextEncodings.UTF8WithoutBOM)
                            Call Thread.Sleep(2500)
                        End If
                    Catch ex As Exception
                        failures += id
                        Call ex.PrintException
                    Finally
                        msg = tick.ETA(progress.ElapsedMilliseconds).FormatTime
                        msg = entry.EntryId & "  " & msg

                        progress.SetProgress(tick.StepProgress, msg)
                    End Try
                Next
            End Using

            Return failures
        End Function

        Public Function DownloadHumans(EXPORT$) As String()
            Dim url$ = "http://www.kegg.jp/kegg-bin/download_htext?htext=hsa00001.keg&format=htext&filedir="
            Dim temp = App.GetAppSysTempFile(".txt")

            If Not url.DownloadFile(temp) Then
                Throw New UnauthorizedAccessException
            End If

            Return MapDownloader.Downloads(EXPORT, briefFile:=temp)
        End Function
    End Module
End Namespace

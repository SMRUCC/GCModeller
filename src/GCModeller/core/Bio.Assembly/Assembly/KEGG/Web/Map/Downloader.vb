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
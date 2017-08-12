Imports System.Text.RegularExpressions
Imports System.Threading
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Terminal
Imports SMRUCC.genomics.Assembly.KEGG.DBGET

Namespace Assembly.KEGG.WebServices

    Public Module Downloader

        Public Function Downloads(EXPORT$, Optional briefFile$ = Nothing) As String()
            Dim briefEntries As BriteHEntry.Pathway() =
                If(String.IsNullOrEmpty(briefFile),
                    BriteHEntry.Pathway.LoadFromResource,
                    BriteHEntry.Pathway.LoadData(briefFile))
            Dim failures As New List(Of String)
            Dim tick As New ProgressProvider(briefEntries.Length)
            Dim msg$
            Dim getID = Function(entry As BriteHEntry.Pathway)
                            If briefFile Is Nothing Then
                                Return "map" & entry.EntryId
                            Else
                                Dim s = entry.Entry.Value
                                s = Regex.Match(s, "\[PATH:.+?\]", RegexICSng).Value
                                s = s.GetStackValue("[", "]").Split(":"c).Last
                                Return s
                            End If
                        End Function

            Using progress As New ProgressBar("Downloads KEGG pathway map data...", CLS:=True)
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
                                .SaveTo(save)
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

            Return Downloader.Downloads(EXPORT, briefFile:=temp)
        End Function
    End Module
End Namespace
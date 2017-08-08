Imports SMRUCC.genomics.Assembly.KEGG.DBGET
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Terminal

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

            Using progress As New ProgressBar("Downloads KEGG pathway map data...", CLS:=True)
                For Each entry In briefEntries
                    Dim id$ = "map" & entry.EntryId
                    Dim url$ = $"http://www.genome.jp/kegg-bin/show_pathway?{id}"
                    Dim save$ = EXPORT & $"/{id}.XML"

                    Try
                        Call Map.ParseHTML(url) _
                            .GetXml _
                            .SaveTo(save)
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
    End Module
End Namespace
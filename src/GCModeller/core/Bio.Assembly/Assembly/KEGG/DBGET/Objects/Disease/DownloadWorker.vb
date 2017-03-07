
Imports System.Runtime.CompilerServices
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.BriteHEntry
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Terminal
Imports Microsoft.VisualBasic.Text

Namespace Assembly.KEGG.DBGET.bGetObject

    Public Module DownloadWorker

        <Extension>
        Public Function DownloadDisease(htext As htext, EXPORT$) As String()
            Dim all As BriteHText() = htext.Hierarchical.EnumerateEntries.ToArray
            Dim failures As New List(Of String)

            Using progress As New ProgressBar("Download KEGG disease data...",, True)
                Dim tick As New ProgressProvider(all.Length)
                Dim path As New Value(Of String)
                Dim disease As Disease

                For Each entry As BriteHText In all

                    If Not (path = entry.BuildPath(EXPORT)).FileExists(ZERO_Nonexists:=True) Then
                        Try
                            disease = DownloadDiseases.Download(entry.EntryId)
                            disease.SaveAsXml(path, , Encodings.ASCII.CodePage)
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
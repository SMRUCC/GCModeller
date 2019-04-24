Imports System.Threading
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Net.Http
Imports Microsoft.VisualBasic.Terminal.ProgressBar
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.BriteHEntry
Imports SMRUCC.genomics.SequenceModel.FASTA

Namespace Assembly.KEGG.DBGET.WebQuery.Compounds

    Module DownloaderProcess

        ReadOnly thread_sleep% = 2000

        Sub New()
            With App.GetVariable("sleep")
                If Not .StringEmpty AndAlso Val(.ByRef) > 1 Then
                    thread_sleep = Val(.ByRef)
                Else
                    thread_sleep = 2000
                End If
            End With
        End Sub

        Public Sub downloadsInternal(key$, briteEntry As CompoundBrite(),
                                            ByRef failures As List(Of String),
                                            ByRef successList As Dictionary(Of String, String),
                                            EXPORT$,
                                            DirectoryOrganized As Boolean,
                                            structInfo As Boolean)
            ' 2017-3-12
            ' 有些entry的编号是空值？？？
            Dim keys As CompoundBrite() = briteEntry _
                .Where(Function(ID)
                           Return (Not ID Is Nothing) AndAlso
                                (Not ID.Entry Is Nothing) AndAlso
                                (Not ID.Entry.Key.StringEmpty)
                       End Function) _
                .ToArray

            Using progress As New ProgressBar("Downloads " & key, 1, CLS:=True)
                Dim tick As New ProgressProvider(keys.Length)
                Dim skip As Boolean = False

                For Each entry As CompoundBrite In keys
                    Dim EntryId As String = entry.Entry.Key
                    Dim saveDIR As String = entry.BuildPath(EXPORT, DirectoryOrganized, [class]:=key)
                    Dim xmlFile$ = $"{saveDIR}/{EntryId}.xml"

                    skip = False

                    If successList.ContainsKey(EntryId) Then
                        skip = successList(EntryId).FileCopy(xmlFile)
                    End If
                    If Not skip AndAlso Not Download(EntryId, xmlFile, structInfo) Then
                        failures += EntryId
                    End If

                    Dim ETA$ = $"ETA={tick.ETA(progress.ElapsedMilliseconds)}"

                    If Not skip Then
                        Call Thread.Sleep(thread_sleep)
                    End If

                    Call progress.SetProgress(tick.StepProgress, details:=ETA)
                Next
            End Using
        End Sub

        ''' <summary>
        ''' 将指定编号的代谢物数据下载下来然后保存在指定的文件夹之中
        ''' gif图片是以base64编码放在XML文件里面的
        ''' </summary>
        ''' <param name="entryID"></param>
        ''' <param name="structInfo"></param>
        ''' <returns></returns>
        Private Function Download(entryID$, xmlFile$, structInfo As Boolean) As Boolean
            If entryID.First = "G"c Then
                Dim gl As Glycan = Glycan.Download(entryID)

                If gl.IsNullOrEmpty Then
                    Call $"[{entryID}] is not exists in the kegg!".Warning
                    Return False
                Else
                    Call gl.GetXml.SaveTo(xmlFile)
                End If
            Else
                Dim cpd As bGetObject.Compound = MetaboliteWebApi.DownloadCompound(entryID)

                If cpd.IsNullOrEmpty Then
                    Call $"[{entryID}] is not exists in the kegg!".Warning
                    Return False
                Else
                    If structInfo Then
                        Dim KCF$ = App.GetAppSysTempFile(".txt", App.PID)
                        Dim gif = App.GetAppSysTempFile(".gif", App.PID)

                        Call cpd.DownloadKCF(KCF)
                        Call cpd.DownloadStructureImage(gif)

                        If KCF.FileExists Then
                            cpd.KCF = KCF.ReadAllText
                        End If

                        ' gif分子二维结构图是以base64
                        ' 字符串的形式写在XML文件之中的
                        If gif.FileExists Then
                            cpd.Image = FastaSeq.SequenceLineBreak(200, New DataURI(gif).ToString)
                        End If
                    End If

                    Call cpd.GetXml.SaveTo(xmlFile)
                End If
            End If

            Return True
        End Function
    End Module
End Namespace
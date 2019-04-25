Imports System.Runtime.CompilerServices
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

        ''' <summary>
        ''' 将指定编号的代谢物数据下载下来然后保存在指定的文件夹之中
        ''' gif图片是以base64编码放在XML文件里面的
        ''' </summary>
        ''' <param name="key"></param>
        ''' <param name="entries"></param>
        ''' <param name="EXPORT"></param>
        ''' <param name="directoryOrganized"></param>
        ''' <param name="structInfo"></param>
        ''' 
        <Extension>
        Public Sub ExecuteDownloads(entries As CompoundBrite(), key$, EXPORT$, directoryOrganized As Boolean, structInfo As Boolean)
            ' 2017-3-12
            ' 有些entry的编号是空值？？？
            Dim keys As CompoundBrite() = entries _
                .Where(Function(ID)
                           Return (Not ID Is Nothing) AndAlso
                                (Not ID.Entry Is Nothing) AndAlso
                                (Not ID.Entry.Key.StringEmpty)
                       End Function) _
                .ToArray

            Using progress As New ProgressBar("Downloads " & key, 1, CLS:=True)
                Dim tick As New ProgressProvider(keys.Length)
                Dim query As New DbGetWebQuery($"{EXPORT}/.cache")
                Dim skip As Boolean = False

                For Each entry As CompoundBrite In keys
                    Dim entryId As String = entry.Entry.Key
                    Dim saveDIR As String = entry.BuildPath(EXPORT, directoryOrganized, [class]:=key)
                    Dim xmlFile$ = $"{saveDIR}/{entryId}.xml"
                    Dim ETA$ = $"ETA={tick.ETA(progress.ElapsedMilliseconds)}"

                    Call query.Download(entryId, xmlFile, structInfo)
                    Call progress.SetProgress(tick.StepProgress, details:=ETA)
                Next
            End Using
        End Sub

        <Extension>
        Friend Sub Download(query As DbGetWebQuery, entryID$, xmlFile$, structInfo As Boolean)
            If entryID.StartsWith("G") Then

                Call query.Query(Of Glycan)(entryID, ".html") _
                    .GetXml _
                    .SaveTo(xmlFile)

            ElseIf entryID.StartsWith("C") Then
                Dim compound As Compound = query.Query(Of Compound)(entryID, ".html")

                If structInfo Then
                    Dim KCF$ = xmlFile.ChangeSuffix("txt")
                    Dim gif = xmlFile.ChangeSuffix("gif")

                    With App.GetAppSysTempFile(".txt", App.PID)
                        If KCF.FileExists Then
                            compound.KCF = KCF.ReadAllText
                        Else
                            compound.DownloadKCF(.ByRef)
                            compound.KCF = .ReadAllText

                            ' make a local copy
                            Call .FileCopy(KCF)
                        End If
                    End With

                    ' gif分子二维结构图是以base64
                    ' 字符串的形式写在XML文件之中的
                    With App.GetAppSysTempFile(".gif", App.PID)
                        Dim base64$

                        If gif.FileExists Then
                            base64 = New DataURI(gif).ToString
                        Else
                            compound.DownloadStructureImage(.ByRef)
                            base64 = New DataURI(.ByRef).ToString

                            ' make a local copy
                            Call .FileCopy(gif)
                        End If

                        compound.Image = FastaSeq.SequenceLineBreak(200, base64)
                    End With
                End If

                Call compound.GetXml.SaveTo(xmlFile)
            Else
                Throw New NotImplementedException
            End If
        End Sub
    End Module
End Namespace
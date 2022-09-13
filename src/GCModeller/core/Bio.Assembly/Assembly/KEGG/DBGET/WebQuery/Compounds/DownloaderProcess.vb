#Region "Microsoft.VisualBasic::3854a48e8a51923f058bc9a04c142e84, GCModeller\core\Bio.Assembly\Assembly\KEGG\DBGET\WebQuery\Compounds\DownloaderProcess.vb"

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

    '   Total Lines: 132
    '    Code Lines: 97
    ' Comment Lines: 16
    '   Blank Lines: 19
    '     File Size: 5.72 KB


    '     Module DownloaderProcess
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Sub: Download, ExecuteDownloads
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ApplicationServices
Imports Microsoft.VisualBasic.ApplicationServices.Terminal.ProgressBar
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Net.Http
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
        Public Sub ExecuteDownloads(entries As BriteTerm(), key$, EXPORT$, directoryOrganized As Boolean, structInfo As Boolean)
            ' 2017-3-12
            ' 有些entry的编号是空值？？？
            Dim keys As BriteTerm() = entries _
                .Where(Function(ID)
                           Return (Not ID Is Nothing) AndAlso
                                (Not ID.entry Is Nothing) AndAlso
                                (Not ID.entry.Key.StringEmpty)
                       End Function) _
                .ToArray

            Using progress As New ProgressBar("Downloads " & key, 1, CLS:=True)
                Dim tick As New ProgressProvider(progress, keys.Length)
                Dim query As New DbGetWebQuery($"{EXPORT}/.cache")

                For Each entry As BriteTerm In keys
                    Dim entryId As String = entry.entry.Key
                    Dim saveDIR As String = entry.BuildPath(EXPORT, directoryOrganized, [class]:=key)
                    Dim xmlFile$ = $"{saveDIR}/{entryId}.xml"
                    Dim ETA$ = $"ETA={tick.ETA().FormatTime}"
                    Dim category As New NamedValue(Of BriteTerm)(key, entry)

                    Call query.Download(entryId, xmlFile, structInfo, category)
                    Call progress.SetProgress(tick.StepProgress, details:=ETA)
                Next
            End Using
        End Sub

        <Extension>
        Friend Sub Download(query As DbGetWebQuery, entryID$, xmlFile$, structInfo As Boolean, category As NamedValue(Of BriteTerm))
            If entryID.StartsWith("G") Then

                Call query.Query(Of Glycan)(entryID, ".html") _
                    .With(Sub(glycan)
                              If Not category.Value Is Nothing AndAlso Not glycan Is Nothing Then
                                  glycan.category = {category.Value}
                              End If
                          End Sub) _
                    .GetXml _
                    .SaveTo(xmlFile)

            ElseIf entryID.StartsWith("C") Then
                Dim compound As Compound = query _
                    .Query(Of Compound)(entryID, ".html") _
                    .With(Sub(metabolite)
                              If Not category.Value Is Nothing AndAlso Not metabolite Is Nothing Then
                                  metabolite.category = {category.Value}
                              End If
                          End Sub)

                If Not (compound Is Nothing OrElse compound.entry.StringEmpty) AndAlso structInfo Then
                    Dim KCF$ = xmlFile.ChangeSuffix("txt")
                    Dim gif = xmlFile.ChangeSuffix("gif")

                    With TempFileSystem.GetAppSysTempFile(".txt", App.PID)
                        If KCF.FileExists Then
                            compound.KCF = KCF.ReadAllText
                        Else
                            DownloadKCF(compound, .ByRef)
                            compound.KCF = .ReadAllText

                            ' make a local copy
                            Call .FileCopy(KCF)
                        End If
                    End With

                    ' gif分子二维结构图是以base64
                    ' 字符串的形式写在XML文件之中的
                    With TempFileSystem.GetAppSysTempFile(".gif", App.PID)
                        Dim base64$

                        If gif.FileExists Then
                            base64 = New DataURI(gif).ToString
                        Else
                            Call .DoCall(Sub(file)
                                             DownloadStructureImage(compound, file)
                                         End Sub)
                            base64 = New DataURI(.ByRef).ToString

                            ' make a local copy
                            Call .FileCopy(gif)
                        End If

                        compound.Image = FastaSeq.SequenceLineBreak(200, base64)
                    End With
                End If

                Call compound.GetXml.SaveTo(xmlFile)
            Else
                Call entryID.Warning
            End If
        End Sub
    End Module
End Namespace

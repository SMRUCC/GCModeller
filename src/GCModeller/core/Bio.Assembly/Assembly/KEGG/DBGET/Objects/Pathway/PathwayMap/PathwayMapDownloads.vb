#Region "Microsoft.VisualBasic::9c53a9498ad11d150da752b3dfcf5779, GCModeller\core\Bio.Assembly\Assembly\KEGG\DBGET\Objects\Pathway\PathwayMap\PathwayMapDownloads.vb"

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

    '   Total Lines: 153
    '    Code Lines: 121
    ' Comment Lines: 9
    '   Blank Lines: 23
    '     File Size: 7.33 KB


    '     Module PathwayMapDownloads
    ' 
    '         Function: DownloadAll, mapParserInternal, SolveEntries
    ' 
    '         Sub: SetMapImage
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports System.Threading
Imports Microsoft.VisualBasic.ApplicationServices.Terminal.ProgressBar
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Language.Default
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Net.Http
Imports Microsoft.VisualBasic.Serialization.JSON
Imports Microsoft.VisualBasic.Text.Parser.HtmlParser
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.LinkDB
Imports SMRUCC.genomics.Assembly.KEGG.WebServices.InternalWebFormParsers

Namespace Assembly.KEGG.DBGET.bGetObject

    Module PathwayMapDownloads

        <Extension>
        Public Sub SetMapImage(pathwayMap As PathwayMap, image As Image)
            Dim base64$ = Base64Codec.ToBase64String(image)
            Dim s As New List(Of String)

            For i As Integer = 1 To base64.Length Step 120
                s.Add(Mid(base64, i, 120))
            Next

            pathwayMap.Map = s.JoinBy(vbCrLf)
        End Sub

        Friend Function mapParserInternal(webForm As WebForm, entry As BriteHEntry.Pathway, cache$, offline As Boolean) As PathwayMap
            Dim pathwayMap As New PathwayMap With {
                .brite = entry,
                .EntryId = entry.EntryId,
                .name = webForm.GetValue("Name").FirstOrDefault.Strip_NOBR.StripHTMLTags.StripBlank,
                .description = .name
            }

            pathwayMap.disease = __parseHTML_ModuleList(webForm.GetValue("Disease").FirstOrDefault, LIST_TYPES.Disease)
            pathwayMap.modules = PathwayWebParser.__parseHTML_ModuleList(webForm.GetValue("Module").FirstOrDefault, LIST_TYPES.Module)
            pathwayMap.KOpathway = webForm.GetValue("KO pathway") _
                .FirstOrDefault _
                .GetTablesHTML _
                .LastOrDefault _
                .GetRowsHTML _
                .Select(Function(row$)
                            Dim cols As String() = row.GetColumnsHTML
                            Return New NamedValue With {
                                .name = cols(0).StripHTMLTags.StripBlank,
                                .text = cols(1).StripHTMLTags.StripBlank
                            }
                        End Function) _
                .FirstOrDefault?.name

#Region "All links"

            With pathwayMap
                .KEGGOrthology = ("http://www.genome.jp/dbget-bin/get_linkdb?-t+orthology+path:map" & entry.EntryId).LinkDbEntries(cacheRoot:=cache, offline:=offline).parseOrthologyTerms
                .KEGGCompound = ("http://www.genome.jp/dbget-bin/get_linkdb?-t+compound+path:map" & entry.EntryId).LinkDbEntries(cacheRoot:=cache, offline:=offline).ToArray
                .KEGGGlycan = ("http://www.genome.jp/dbget-bin/get_linkdb?-t+glycan+path:map" & entry.EntryId).LinkDbEntries(cacheRoot:=cache, offline:=offline).ToArray
                .KEGGEnzyme = ("http://www.genome.jp/dbget-bin/get_linkdb?-t+enzyme+path:map" & entry.EntryId).LinkDbEntries(cacheRoot:=cache, offline:=offline).ToArray
                .KEGGReaction = ("http://www.genome.jp/dbget-bin/get_linkdb?-t+reaction+path:map" & entry.EntryId).LinkDbEntries(cacheRoot:=cache, offline:=offline).ToArray
            End With
#End Region
            Return pathwayMap
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Private Function SolveEntries(file As String) As BriteHEntry.Pathway()
            Return If(String.IsNullOrEmpty(file), BriteHEntry.Pathway.LoadFromResource, BriteHEntry.Pathway.LoadData(file))
        End Function

        ''' <summary>
        ''' 函数会返回失败的个数
        ''' </summary>
        ''' <param name="EXPORT"></param>
        ''' <param name="briteFile"></param>
        ''' <param name="directoryOrganized"></param>
        ''' <returns></returns>
        Public Function DownloadAll(EXPORT$,
                                    Optional briteFile$ = "",
                                    Optional directoryOrganized As Boolean = True,
                                    Optional [overrides] As Boolean = False) As Integer

            Dim entries As BriteHEntry.Pathway() = SolveEntries(briteFile)
            Dim rtvl% = Scan0
            Dim EXPORT_dir = New [Default](Of String)(EXPORT).When(Not directoryOrganized)
            Dim testSuccess = Function(xml As String) As Boolean
                                  If xml.FileLength <= 0 Then
                                      Return False
                                  End If

                                  ' 可能会有些页面下载失败了，导致数据缺失
                                  ' 在这里进行测试
                                  With xml.LoadXml(Of PathwayMap)
                                      If .KEGGCompound.IsNullOrEmpty OrElse
                                         .KEGGEnzyme.IsNullOrEmpty OrElse
                                         .KEGGReaction.IsNullOrEmpty Then

                                          Return False
                                      End If
                                  End With

                                  Return True
                              End Function

            Using progress As New ProgressBar("Download KEGG pathway reference map data...", 1, CLS:=True)
                Dim tick As New ProgressProvider(progress, entries.Length)

                Call tick.StepProgress()

                For Each entry As BriteHEntry.Pathway In entries
                    Dim id$ = entry.entry.name
                    Dim save$ = $"{EXPORT}/{entry.GetPathCategory}" Or EXPORT_dir
                    Dim xml As String = $"{save}/map{id}.xml"
                    Dim png As String = $"{save}/map{id}.png"

                    If testSuccess(xml) AndAlso png.FileLength > 0 Then
                        If Not [overrides] Then
                            GoTo EXIT_LOOP
                        End If
                    End If

                    Dim pathway As PathwayMap = Nothing

                    Try
                        pathway = PathwayMap.Download(entry, cache:=EXPORT)
                    Catch ex As Exception
                        ex = New Exception(entry.GetJson, ex)
                        Call App.LogException(ex)
                        Call ex.PrintException
                    End Try

                    If pathway Is Nothing Then
                        Call App.LogException($"{entry.ToString} is not exists in the kegg!")
                        rtvl -= 1
                        GoTo EXIT_LOOP
                    Else
                        Call PathwayMap.DownloadPathwayMap("map", id, EXPORT:=save)
                        Call pathway.SetMapImage(LoadImage(png))
                        Call pathway.SaveAsXml(xml)
                        Call Thread.Sleep(10000)
                    End If
EXIT_LOOP:
                    Dim ETA = tick.ETA().FormatTime
                    Call progress.SetProgress(tick.StepProgress, "ETA " & ETA)
                Next
            End Using

            Return rtvl
        End Function
    End Module
End Namespace

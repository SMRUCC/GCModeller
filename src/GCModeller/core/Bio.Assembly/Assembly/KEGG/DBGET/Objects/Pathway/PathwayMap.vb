#Region "Microsoft.VisualBasic::7a240f86618df0da0964e9d6694edefa, Bio.Assembly\Assembly\KEGG\DBGET\Objects\Pathway\PathwayMap.vb"

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

    '     Class PathwayMap
    ' 
    '         Properties: brite, disease, KEGGCompound, KEGGEnzyme, KEGGGlycan
    '                     KEGGOrthology, KEGGReaction, KOpathway, Map, modules
    '                     name
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '         Function: __parserInternal, (+2 Overloads) Download, DownloadAll, DownloadPathwayMap, GetMapImage
    '                   GetPathwayGenes, SolveEntries
    ' 
    '         Sub: SetMapImage
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports System.Threading
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Language.Default
Imports Microsoft.VisualBasic.Net.Http
Imports Microsoft.VisualBasic.Serialization.JSON
Imports Microsoft.VisualBasic.Terminal
Imports Microsoft.VisualBasic.Terminal.ProgressBar
Imports Microsoft.VisualBasic.Text.Parser.HtmlParser
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.LinkDB
Imports SMRUCC.genomics.Assembly.KEGG.WebServices.InternalWebFormParsers
Imports SMRUCC.genomics.ComponentModel.Annotation

Namespace Assembly.KEGG.DBGET.bGetObject

    ''' <summary>
    ''' <see cref="BriteHEntry.Pathway.LoadFromResource()"/>.
    ''' (相对于<see cref="Pathway"/>而言，这个对象是参考用的，并非某个特定的物种的)
    ''' </summary>
    Public Class PathwayMap : Inherits PathwayBrief

        ''' <summary>
        ''' The name value of this pathway object
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property name As String

        Public Property KOpathway As String
        Public Property disease As NamedValue()
        Public Property modules As NamedValue()
        Public Property brite As BriteHEntry.Pathway

        ''' <summary>
        ''' base64 image data.
        ''' </summary>
        ''' <returns></returns>
        Public Property Map As String

#Region "All links"

        ''' <summary>
        ''' http://www.genome.jp/dbget-bin/get_linkdb?-t+orthology+path:map01100
        ''' </summary>
        ''' <returns></returns>
        Public Property KEGGOrthology As OrthologyTerms
        ''' <summary>
        ''' http://www.genome.jp/dbget-bin/get_linkdb?-t+compound+path:map01100
        ''' </summary>
        ''' <returns></returns>
        Public Property KEGGCompound As NamedValue()
        ''' <summary>
        ''' http://www.genome.jp/dbget-bin/get_linkdb?-t+glycan+path:map01100
        ''' </summary>
        ''' <returns></returns>
        Public Property KEGGGlycan As NamedValue()
        ''' <summary>
        ''' http://www.genome.jp/dbget-bin/get_linkdb?-t+enzyme+path:map01100
        ''' </summary>
        ''' <returns></returns>
        Public Property KEGGEnzyme As NamedValue()
        ''' <summary>
        ''' http://www.genome.jp/dbget-bin/get_linkdb?-t+reaction+path:map01100
        ''' </summary>
        ''' <returns></returns>
        Public Property KEGGReaction As NamedValue()
#End Region

        <XmlNamespaceDeclarations()>
        Public xmlnsImports As XmlSerializerNamespaces

        Public Sub New()
            xmlnsImports = New XmlSerializerNamespaces
            xmlnsImports.Add("KO", OrthologyTerms.Xmlns)
        End Sub

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Overrides Function GetPathwayGenes() As String()
            Return KEGGOrthology.EntityList
        End Function

        Public Sub SetMapImage(image As Image)
            Dim base64$ = Base64Codec.ToBase64String(image)
            Dim s As New List(Of String)

            For i As Integer = 1 To base64.Length Step 120
                s.Add(Mid(base64, i, 120))
            Next

            Map = s.JoinBy(vbCrLf)
        End Sub

        Public Function GetMapImage() As Bitmap
            If String.IsNullOrEmpty(Map) Then
                Return Nothing
            Else
                Dim lines$() = Map.LineTokens
                Dim base64$ = String.Join("", lines)
                Return Base64Codec.GetImage(base64)
            End If
        End Function

        Public Shared Function Download(entry As BriteHEntry.Pathway) As PathwayMap
            Dim url As String = "http://www.genome.jp/dbget-bin/www_bget?pathway:map" & entry.EntryId
            Dim WebForm As New WebForm(url)

            If WebForm.Count = 0 Then
                Return Nothing
            Else
                Return __parserInternal(WebForm, entry)
            End If
        End Function

        ''' <summary>
        ''' 测试用的函数
        ''' </summary>
        ''' <param name="entryId$">mapxxxx</param>
        ''' <returns></returns>
        Public Shared Function Download(entryId As String) As PathwayMap
            Dim entry As New BriteHEntry.Pathway With {
                .entry = New NamedValue With {
                    .name = entryId.Match("\d+")
                }
            }

            Return Download(entry)
        End Function

        Private Shared Function __parserInternal(webForm As WebForm, entry As BriteHEntry.Pathway) As PathwayMap
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
                .KEGGOrthology = ("http://www.genome.jp/dbget-bin/get_linkdb?-t+orthology+path:map" & entry.EntryId).LinkDbEntries.parseOrthologyTerms
                .KEGGCompound = ("http://www.genome.jp/dbget-bin/get_linkdb?-t+compound+path:map" & entry.EntryId).LinkDbEntries.Select(Function(t) New NamedValue(t.Key, t.Value)).ToArray
                .KEGGGlycan = ("http://www.genome.jp/dbget-bin/get_linkdb?-t+glycan+path:map" & entry.EntryId).LinkDbEntries.Select(Function(t) New NamedValue(t.Key, t.Value)).ToArray
                .KEGGEnzyme = ("http://www.genome.jp/dbget-bin/get_linkdb?-t+enzyme+path:map" & entry.EntryId).LinkDbEntries.Select(Function(t) New NamedValue(t.Key, t.Value)).ToArray
                .KEGGReaction = ("http://www.genome.jp/dbget-bin/get_linkdb?-t+reaction+path:map" & entry.EntryId).LinkDbEntries.Select(Function(t) New NamedValue(t.Key, t.Value)).ToArray
            End With
#End Region
            Return pathwayMap
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Private Shared Function SolveEntries(file As String) As BriteHEntry.Pathway()
            Return If(String.IsNullOrEmpty(file), BriteHEntry.Pathway.LoadFromResource, BriteHEntry.Pathway.LoadData(file))
        End Function

        ''' <summary>
        ''' 函数会返回失败的个数
        ''' </summary>
        ''' <param name="EXPORT"></param>
        ''' <param name="briteFile"></param>
        ''' <param name="directoryOrganized"></param>
        ''' <returns></returns>
        Public Shared Function DownloadAll(EXPORT$,
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
                Dim tick As New ProgressProvider(entries.Length)

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
                        pathway = Download(entry)
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
                        Call DownloadPathwayMap("map", id, EXPORT:=save)
                        Call pathway.SetMapImage(LoadImage(png))
                        Call pathway.SaveAsXml(xml)
                        Call Thread.Sleep(10000)
                    End If
EXIT_LOOP:
                    Dim ETA = tick.ETA(progress.ElapsedMilliseconds).FormatTime
                    Call progress.SetProgress(tick.StepProgress, "ETA " & ETA)
                Next
            End Using

            Return rtvl
        End Function

        ''' <summary>
        ''' 下载pathway的图片
        ''' </summary>
        ''' <param name="sp$"></param>
        ''' <param name="entry$"></param>
        ''' <param name="EXPORT$"></param>
        ''' <returns></returns>
        Public Shared Function DownloadPathwayMap(sp$, entry$, EXPORT$) As Boolean
            Dim url As String = $"http://www.genome.jp/kegg/pathway/{sp}/{sp}{entry}.png"
            Dim path$ = String.Format("{0}/{1}{2}.png", EXPORT, sp, entry)
            Return url.DownloadFile(save:=path)
        End Function
    End Class
End Namespace

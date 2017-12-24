#Region "Microsoft.VisualBasic::f12823e56dabf3c66a4e779c45934bf5, ..\GCModeller\core\Bio.Assembly\Assembly\KEGG\DBGET\Objects\Pathway\PathwayMap.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
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

Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports System.Threading
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Net.Http
Imports Microsoft.VisualBasic.Serialization.JSON
Imports Microsoft.VisualBasic.Terminal
Imports Microsoft.VisualBasic.Terminal.ProgressBar
Imports Microsoft.VisualBasic.Text.HtmlParser
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject.Pathway
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.LinkDB
Imports SMRUCC.genomics.Assembly.KEGG.WebServices.InternalWebFormParsers

Namespace Assembly.KEGG.DBGET.bGetObject

    ''' <summary>
    ''' <see cref="BriteHEntry.Pathway.LoadFromResource()"/>.
    ''' (相对于<see cref="Pathway"/>而言，这个对象是参考用的，并非某个特定的物种的)
    ''' </summary>
    Public Class PathwayMap : Inherits ComponentModel.PathwayBrief

        ''' <summary>
        ''' The name value of this pathway object
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Name As String

        Public Property KOpathway As KeyValuePair()
        Public Property Disease As KeyValuePair()
        Public Property Modules As KeyValuePair()
        Public Property Brite As BriteHEntry.Pathway

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
        Public Property KEGGOrthology As KeyValuePair()
        ''' <summary>
        ''' http://www.genome.jp/dbget-bin/get_linkdb?-t+compound+path:map01100
        ''' </summary>
        ''' <returns></returns>
        Public Property KEGGCompound As KeyValuePair()
        ''' <summary>
        ''' http://www.genome.jp/dbget-bin/get_linkdb?-t+glycan+path:map01100
        ''' </summary>
        ''' <returns></returns>
        Public Property KEGGGlycan As KeyValuePair()
        ''' <summary>
        ''' http://www.genome.jp/dbget-bin/get_linkdb?-t+enzyme+path:map01100
        ''' </summary>
        ''' <returns></returns>
        Public Property KEGGEnzyme As KeyValuePair()
        ''' <summary>
        ''' http://www.genome.jp/dbget-bin/get_linkdb?-t+reaction+path:map01100
        ''' </summary>
        ''' <returns></returns>
        Public Property KEGGReaction As KeyValuePair()
#End Region

        Public Overrides Function GetPathwayGenes() As String()
            Throw New NotImplementedException()
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
                Dim lines$() = Map.lTokens
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
        ''' <param name="entryId$"></param>
        ''' <returns></returns>
        Public Shared Function Download(entryId$) As PathwayMap
            Dim url As String = "http://www.genome.jp/dbget-bin/www_bget?pathway:map" & entryId
            Dim WebForm As New WebForm(url)
            Dim entry As New BriteHEntry.Pathway With {
                .Entry = New KeyValuePair With {
                    .Key = entryId
                }
            }

            If WebForm.Count = 0 Then
                Return Nothing
            Else
                Return __parserInternal(WebForm, entry)
            End If
        End Function

        Private Shared Function __parserInternal(webForm As WebForm, entry As BriteHEntry.Pathway) As PathwayMap
            Dim pathwayMap As New PathwayMap With {
                .Brite = entry,
                .EntryId = entry.EntryId,
                .Name = webForm.GetValue("Name").FirstOrDefault.Strip_NOBR.StripHTMLTags.StripBlank,
                .Description = .Name
            }

            pathwayMap.Disease = __parseHTML_ModuleList(webForm.GetValue("Disease").FirstOrDefault, LIST_TYPES.Disease)
            pathwayMap.Modules = PathwayWebParser.__parseHTML_ModuleList(webForm.GetValue("Module").FirstOrDefault, LIST_TYPES.Module)
            pathwayMap.KOpathway = webForm.GetValue("KO pathway") _
                .FirstOrDefault _
                .GetTablesHTML _
                .LastOrDefault _
                .GetRowsHTML _
                .Select(Function(row$)
                            Dim cols As String() = row.GetColumnsHTML
                            Return New KeyValuePair With {
                                .Key = cols(0).StripHTMLTags.StripBlank,
                                .Value = cols(1).StripHTMLTags.StripBlank
                            }
                        End Function).ToArray

#Region "All links"

            With pathwayMap
                .KEGGOrthology = ("http://www.genome.jp/dbget-bin/get_linkdb?-t+orthology+path:map" & entry.EntryId).LinkDbEntries.ToArray
                .KEGGCompound = ("http://www.genome.jp/dbget-bin/get_linkdb?-t+compound+path:map" & entry.EntryId).LinkDbEntries.ToArray
                .KEGGGlycan = ("http://www.genome.jp/dbget-bin/get_linkdb?-t+glycan+path:map" & entry.EntryId).LinkDbEntries.ToArray
                .KEGGEnzyme = ("http://www.genome.jp/dbget-bin/get_linkdb?-t+enzyme+path:map" & entry.EntryId).LinkDbEntries.ToArray
                .KEGGReaction = ("http://www.genome.jp/dbget-bin/get_linkdb?-t+reaction+path:map" & entry.EntryId).LinkDbEntries.ToArray
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
        ''' <param name="DirectoryOrganized"></param>
        ''' <returns></returns>
        Public Shared Function DownloadAll(EXPORT$, Optional briteFile$ = "", Optional DirectoryOrganized As Boolean = True, Optional [overrides] As Boolean = False) As Integer
            Dim BriefEntries As BriteHEntry.Pathway() = SolveEntries(briteFile)
            Dim rtvl% = Scan0

            Using progress As New ProgressBar("Download KEGG pathway reference map data...", 1, CLS:=True)
                Dim tick As New ProgressProvider(BriefEntries.Length)

                Call tick.StepProgress()

                For Each entry As BriteHEntry.Pathway In BriefEntries
                    Dim EntryId As String = entry.Entry.Key
                    Dim SaveToDir As String = If(DirectoryOrganized, BriteHEntry.Pathway.CombineDIR(entry, EXPORT), EXPORT)
                    Dim XmlFile As String = $"{SaveToDir}/map{EntryId}.xml"
                    Dim PngFile As String = $"{SaveToDir}/map{EntryId}.png"

                    If XmlFile.FileLength > 0 AndAlso PngFile.FileLength > 0 Then
                        If Not [overrides] Then
                            GoTo EXIT_LOOP
                        End If
                    End If

                    Dim Pathway As PathwayMap = Nothing

                    Try
                        Pathway = Download(entry)
                    Catch ex As Exception
                        ex = New Exception(entry.GetJson, ex)
                        Call App.LogException(ex)
                        Call ex.PrintException
                    End Try

                    If Pathway Is Nothing Then
                        Call App.LogException($"{entry.ToString} is not exists in the kegg!")
                        rtvl -= 1
                        GoTo EXIT_LOOP
                    Else
                        Call DownloadPathwayMap("map", EntryId, EXPORT:=SaveToDir)
                        Call Pathway.SetMapImage(LoadImage(PngFile))
                        Call Pathway.SaveAsXml(XmlFile)
                        Call Thread.Sleep(10000)
                    End If
EXIT_LOOP:
                    Dim ETA = tick.ETA(progress.ElapsedMilliseconds).FormatTime
                    Call progress.SetProgress(tick.StepProgress, "ETA " & ETA)
                Next
            End Using

            Return rtvl
        End Function
    End Class
End Namespace

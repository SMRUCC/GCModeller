
Imports System.Drawing
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Net.Http
Imports Microsoft.VisualBasic.Text.HtmlParser
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject.Pathway
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

        Public Function GetMapImage() As Bitmap
            If String.IsNullOrEmpty(Map) Then
                Return Nothing
            Else
                Return Base64Codec.GetImage(Map)
            End If
        End Function

        Public Shared Function Download(entry As BriteHEntry.Pathway) As PathwayMap
            Dim url As String = "http://www.genome.jp/dbget-bin/www_bget?pathway:map" & entry.EntryId
            Dim WebForm As New WebForm(url)

            If WebForm.Count = 0 Then
                Return Nothing
            End If

            Dim pathwayMap As New PathwayMap With {
                .Brite = entry,
                .EntryId = entry.EntryId,
                .Name = WebForm.StripName(WebForm.GetValue("Name").FirstOrDefault).StripHTMLTags.StripBlank,
                .Description = .Name
            }

            pathwayMap.Disease = __parseHTML_ModuleList(WebForm.GetValue("Disease").FirstOrDefault, LIST_TYPES.Disease)
            pathwayMap.Modules = Pathway.__parseHTML_ModuleList(WebForm.GetValue("Module").FirstOrDefault, LIST_TYPES.Module)
            pathwayMap.KOpathway = WebForm.GetValue("KO pathway") _
                .FirstOrDefault _
                .GetTablesHTML _
                .LastOrDefault _
                .GetRowsHTML _
                .Select(Function(row$)
                            Dim cols = row.GetColumnsHTML
                            Return New KeyValuePair With {
                                .Key = cols(0).StripHTMLTags.StripBlank,
                                .Value = cols(1).StripHTMLTags.StripBlank
                            }
                        End Function).ToArray

            Return pathwayMap
        End Function

        Public Shared Function DownloadAll(EXPORT As String, Optional BriefFile As String = "", Optional DirectoryOrganized As Boolean = True) As Integer
            Dim BriefEntries As KEGG.DBGET.BriteHEntry.Pathway() =
                If(String.IsNullOrEmpty(BriefFile),
                   KEGG.DBGET.BriteHEntry.Pathway.LoadFromResource,
                   KEGG.DBGET.BriteHEntry.Pathway.LoadData(BriefFile))

            For Each Entry As KEGG.DBGET.BriteHEntry.Pathway In BriefEntries
                Dim EntryId As String = Entry.Entry.Key
                Dim SaveToDir As String = If(DirectoryOrganized, BriteHEntry.Pathway.CombineDIR(Entry, EXPORT), EXPORT)

                Dim XmlFile As String = $"{SaveToDir}/map{EntryId}.xml"
                Dim PngFile As String = $"{SaveToDir}/map{EntryId}.png"

                If XmlFile.FileLength > 0 AndAlso PngFile.FileLength > 0 Then
                    Continue For
                End If

                Dim Pathway As PathwayMap = Download(Entry)

                If Pathway Is Nothing Then
                    Call $"{Entry.ToString} is not exists in the kegg!".__DEBUG_ECHO
                    Continue For
                Else
                    Call DownloadPathwayMap("map", EntryId, SaveLocationDir:=SaveToDir)
                    Pathway.Map = Base64Codec.ToBase64String(LoadImage(PngFile))
                    Call Pathway.SaveAsXml(XmlFile)
                End If
            Next

            Return 0
        End Function
    End Class
End Namespace
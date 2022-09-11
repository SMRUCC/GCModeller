#Region "Microsoft.VisualBasic::5fbe103ead75139ca060868e95820f4d, GCModeller\core\Bio.Assembly\Assembly\KEGG\DBGET\Objects\Pathway\PathwayMap\PathwayMap.vb"

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

    '   Total Lines: 163
    '    Code Lines: 87
    ' Comment Lines: 53
    '   Blank Lines: 23
    '     File Size: 6.24 KB


    '     Class PathwayMap
    ' 
    '         Properties: brite, disease, KEGGCompound, KEGGEnzyme, KEGGGlycan
    '                     KEGGOrthology, KEGGReaction, KOpathway, Map, modules
    '                     name
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: (+2 Overloads) Download, DownloadAll, DownloadPathwayMap, GetCompounds, GetMapImage
    '                   GetPathwayGenes, ToPathway
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Net.Http
Imports Microsoft.VisualBasic.Text.Xml.Models
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

        Public Function GetCompounds(Optional includesGlycan As Boolean = True) As Index(Of String)
            Dim cids As New List(Of String)(KEGGCompound.SafeQuery.Select(Function(c) c.name))

            If includesGlycan Then
                cids.AddRange(KEGGGlycan.SafeQuery.Select(Function(c) c.name))
            End If

            Return cids.Distinct.Indexing
        End Function

        Public Function GetMapImage() As Bitmap
            If String.IsNullOrEmpty(Map) Then
                Return Nothing
            Else
                Dim lines$() = Map.LineTokens
                Dim base64$ = String.Join("", lines)
                Return Base64Codec.GetImage(base64)
            End If
        End Function

        Public Function ToPathway() As Pathway
            Return New Pathway With {
                .compound = KEGGCompound,
                .name = name,
                .EntryId = EntryId
            }
        End Function

        Public Shared Function Download(entry As BriteHEntry.Pathway, Optional cache$ = "./", Optional offline As Boolean = False) As PathwayMap
            Dim url As String = "http://www.genome.jp/dbget-bin/www_bget?pathway:map" & entry.EntryId
            Dim WebForm As New WebForm(url)

            If WebForm.Count = 0 Then
                Return Nothing
            Else
                Return WebForm.DoCall(Function(form) mapParserInternal(form, entry, cache, offline))
            End If
        End Function

        ''' <summary>
        ''' 测试用的函数
        ''' </summary>
        ''' <param name="entryId$">mapxxxx</param>
        ''' <returns></returns>
        Public Shared Function Download(entryId$, Optional cache$ = "./", Optional offline As Boolean = False) As PathwayMap
            Dim entry As New BriteHEntry.Pathway With {
                .entry = New NamedValue With {
                    .name = entryId.Match("\d+")
                }
            }

            Return Download(entry, cache, offline)
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

            Return PathwayMapDownloads.DownloadAll(EXPORT, briteFile, directoryOrganized, [overrides])
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

            Return wget.Download(url, save:=path)
        End Function
    End Class
End Namespace

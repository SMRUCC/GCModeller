#Region "Microsoft.VisualBasic::3942cfa08fa119235c2abc61e5aac7a7, GCModeller\core\Bio.Assembly\Assembly\KEGG\DBGET\Objects\Pathway\PathwayMap\PathwayMap.vb"

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

    '   Total Lines: 121
    '    Code Lines: 63
    ' Comment Lines: 41
    '   Blank Lines: 17
    '     File Size: 4.37 KB


    '     Class PathwayMap
    ' 
    '         Properties: brite, disease, KEGGCompound, KEGGEnzyme, KEGGGlycan
    '                     KEGGOrthology, KEGGReaction, KOpathway, Map, modules
    '                     name
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: DownloadPathwayMap, GetCompounds, GetMapImage, GetPathwayGenes, ToPathway
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

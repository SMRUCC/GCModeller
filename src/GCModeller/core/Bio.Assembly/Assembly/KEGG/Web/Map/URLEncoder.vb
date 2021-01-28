﻿#Region "Microsoft.VisualBasic::41b2f679bf172c4cdd7df627fd8353ad, Bio.Assembly\Assembly\KEGG\Web\Map\URLEncoder.vb"

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

    '     Module URLEncoder
    ' 
    '         Function: KEGGURLEncode, URLParser, VisualizePathwayMap
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Imaging

Namespace Assembly.KEGG.WebServices

    ''' <summary>
    ''' The kegg pathway map url encoder
    ''' 
    ''' pattern:
    ''' 
    ''' ```
    ''' http://www.genome.jp/kegg-bin/show_pathway?{pathway_ID}/{geneID}%09{color}/{geneID}%09{color}/{geneID}%09{color}
    ''' ```
    ''' </summary>
    ''' <remarks>
    ''' Generates the enrichment term result for kegg url example like:
    ''' http://www.genome.jp/kegg-bin/show_pathway?hsa03013/hsa:8563%09red/hsa:3837%09red/hsa:1983%09red/hsa:4686%09red/hsa:1974%09red/hsa:23165%09red/hsa:9688%09red/hsa:57510%09red/hsa:7514%09red/hsa:60528%09red/hsa:1981%09red/hsa:51068%09red/hsa:5905%09red/hsa:1917%09red/hsa:57187%09red/hsa:1977%09red/hsa:8894%09red/hsa:79023%09red/hsa:8891%09red/hsa:8890%09red/hsa:8893%09red/hsa:8892%09red/hsa:11260%09red/hsa:79902%09red/hsa:8669%09red/hsa:26986%09red/hsa:9984%09red/hsa:23191%09red/hsa:26019%09red/hsa:79228%09red/hsa:1965%09red/hsa:9669%09red
    ''' </remarks>
    Public Module URLEncoder

        ''' <summary>
        ''' ``{id -> color}``
        ''' </summary>
        ''' <param name="url$"></param>
        ''' <returns></returns>
        Public Function URLParser(url$) As NamedCollection(Of NamedValue(Of String))
            Dim args$ = url.Split("?"c).LastOrDefault
            Dim t = args.Split("/"c)
            Dim pathwayID$ = t.First
            Dim genes As NamedValue(Of String)() = t _
                .Skip(1) _
                .Select(Function(gene)
                            With Strings.Split(gene, "%09")
                                Return New NamedValue(Of String)(.First, .Last)
                            End With
                        End Function) _
                .ToArray

            Return New NamedCollection(Of NamedValue(Of String)) With {
                .Name = pathwayID,
                .Value = genes,
                .Description = url
            }
        End Function

        ''' <summary>
        ''' 用于可视化差异表达基因或者富集结果
        ''' </summary>
        ''' <param name="profiles"></param>
        ''' <returns></returns>
        <Extension> Public Function KEGGURLEncode(profiles As NamedCollection(Of NamedValue(Of String))) As String
            Dim url$ = PathwayMapping.KEGG_show_pathway & profiles.Name
            Dim genes$ = profiles _
                .Select(Function(gene) $"{gene.Name}%09{gene.Value}") _
                .JoinBy("/")

            url = url & "/" & genes
            Return url
        End Function

        ''' <summary>
        ''' 可视化差异表达基因的代谢途径或者功能富集分析结果的代谢途径
        ''' </summary>
        ''' <param name="profiles"></param>
        ''' <param name="save$"></param>
        ''' <returns></returns>
        <Extension> Public Function VisualizePathwayMap(profiles As NamedCollection(Of NamedValue(Of String)), save$) As Image
            Dim url$ = profiles.KEGGURLEncode
            Call PathwayMapping.ShowEnrichmentPathway(url, save)
            Return save.LoadImage
        End Function
    End Module
End Namespace

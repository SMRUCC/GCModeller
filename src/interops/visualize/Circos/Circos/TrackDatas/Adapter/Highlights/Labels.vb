#Region "Microsoft.VisualBasic::ac662168d1b0f8511d09a0d62a283fdd, visualize\Circos\Circos\TrackDatas\Adapter\Highlights\Labels.vb"

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

    '     Class HighlightLabel
    ' 
    '         Constructor: (+4 Overloads) Sub New
    '         Function: (+3 Overloads) __textSource
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text.RegularExpressions
Imports SMRUCC.genomics.ComponentModel.Annotation
Imports SMRUCC.genomics.ComponentModel.Loci

Namespace TrackDatas.Highlights

    Public Class HighlightLabel : Inherits data(Of TextTrackData)

        Sub New(annoData As IEnumerable(Of IGeneBrief), Optional chr$ = "chr1")
            Call MyBase.New(__textSource(annoData, chr))
        End Sub

        Sub New(annoData As IEnumerable(Of IMotifSite), Optional chr$ = "chr1")
            Call MyBase.New(__textSource(annoData, chr))
        End Sub

        Sub New(metas As IEnumerable(Of TextTrackData))
            Call MyBase.New(metas)
        End Sub

        Protected Sub New()
            Call MyBase.New(Nothing)
        End Sub

        Private Shared Function __textSource(annoData As IEnumerable(Of IGeneBrief), chr$) As IEnumerable(Of TextTrackData)
            Return __textSource(annoData, chr,
                   Function(g) g.Key,
                   Function(g) g.Location.Normalization)
        End Function

        Private Shared Function __textSource(annoData As IEnumerable(Of IMotifSite), chr$) As IEnumerable(Of TextTrackData)
            Return __textSource(annoData, chr,
                   Function(g) g.Name,
                   Function(g) g.Site.Normalization)
        End Function

        Private Shared Function __textSource(Of T)(annoData As IEnumerable(Of T), chr$,
                                                   getLabel As Func(Of T, String),
                                                   getSite As Func(Of T, Location)) As IEnumerable(Of TextTrackData)
            Return From gene As T
                   In annoData
                   Let name As String = getLabel(gene)
                   Let location As Location = getSite(gene)
                   Where Not (String.IsNullOrEmpty(name) OrElse
                       String.Equals("-", name) OrElse  '这些基因名都表示没有的空值，去掉
                       String.Equals("/", name) OrElse
                       String.Equals("\", name))
                   Select New TextTrackData With {
                       .chr = chr,
                       .start = CInt(location.Left),
                       .end = CInt(location.Right),
                       .text = Regex.Replace(name, "\s+", "_")  ' 空格会出现问题的，所以在这里替换掉
                   }
        End Function
    End Class
End Namespace

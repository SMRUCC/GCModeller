#Region "Microsoft.VisualBasic::96bbeec5c4f411ce51b62e02da22bad8, ..\interops\visualize\Circos\Circos\TrackDatas\Adapter\Highlights\Labels.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
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
Imports System.Text
Imports System.Text.RegularExpressions
Imports SMRUCC.genomics.Visualize.Circos.TrackDatas
Imports SMRUCC.genomics.Assembly.NCBI.GenBank
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat
Imports SMRUCC.genomics.ComponentModel
Imports Microsoft.VisualBasic.Linq.Extensions

Namespace TrackDatas.Highlights

    Public Class HighlightLabel : Inherits data(Of TextTrackData)

        Sub New(annoData As IEnumerable(Of IGeneBrief))
            Call MyBase.New(__textSource(annoData))
        End Sub

        Sub New(metas As IEnumerable(Of TextTrackData))
            Call MyBase.New(metas)
        End Sub

        Protected Sub New()
            Call MyBase.New(Nothing)
        End Sub

        Private Shared Iterator Function __textSource(annoData As IEnumerable(Of IGeneBrief)) As IEnumerable(Of TextTrackData)
            For Each text As TextTrackData In From gene As IGeneBrief
                                              In annoData
                                              Where Not (String.IsNullOrEmpty(gene.Identifier) OrElse
                                                  String.Equals("-", gene.Identifier) OrElse  '这些基因名都表示没有的空值，去掉
                                                  String.Equals("/", gene.Identifier) OrElse
                                                  String.Equals("\", gene.Identifier))
                                              Select New TextTrackData With {
                                                  .start = CInt(gene.Location.Left),
                                                  .end = CInt(gene.Location.Right),
                                                  .text = Regex.Replace(gene.Identifier, "\s+", "_")
                                              }  ' 空格会出现问题的，所以在这里替换掉
                Yield text
            Next
        End Function
    End Class
End Namespace

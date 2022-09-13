#Region "Microsoft.VisualBasic::ab7f4086ef34f4d4de69826f5268efbf, GCModeller\visualize\ChromosomeMap\PlasmidMap\PlasmidMap.vb"

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

    '   Total Lines: 14
    '    Code Lines: 7
    ' Comment Lines: 3
    '   Blank Lines: 4
    '     File Size: 390 B


    '     Class PlasmidMapDrawingModel
    ' 
    '         Properties: GeneObjects, genomeSize
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports SMRUCC.genomics.Visualize.ChromosomeMap.DrawingModels

Namespace PlasmidMap

    ''' <summary>
    ''' 全基因组图谱的绘制是线性的，而plasmid图谱则是圆环状的
    ''' </summary>
    Public Class PlasmidMapDrawingModel

        Public Property GeneObjects As SegmentObject()
        Public Property genomeSize As Integer

    End Class
End Namespace

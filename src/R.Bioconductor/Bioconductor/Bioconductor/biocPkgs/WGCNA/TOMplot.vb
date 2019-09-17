#Region "Microsoft.VisualBasic::d74cf5d1d68995e6b032e55f25ef0940, Bioconductor\Bioconductor\biocPkgs\WGCNA\TOMplot.vb"

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

    '     Class TOMplot
    ' 
    '         Properties: Colors, ColorsLeft, dendro, dissim, setLayout
    '                     terrainColors
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports RDotNET.Extensions.VisualBasic.SymbolBuilder
Imports RDotNET.Extensions.VisualBasic.SymbolBuilder.Rtypes
Imports RDotNET.Extensions.VisualBasic

Namespace WGCNA

    ''' <summary>
    ''' Graphical representation of the Topological Overlap Matrix using a heatmap plot combined with the corresponding hierarchical clustering dendrogram and module colors.
    ''' </summary>
    <RFunc("TOMplot")> Public Class TOMplot : Inherits WGCNAFunction

        ''' <summary>
        ''' a matrix containing the topological overlap-based dissimilarity
        ''' </summary>
        ''' <returns></returns>
        Public Property dissim As RExpression
        ''' <summary>
        ''' the corresponding hierarchical clustering dendrogram
        ''' </summary>
        ''' <returns></returns>
        Public Property dendro As RExpression
        ''' <summary>
        ''' optional specification of module colors to be plotted on top
        ''' </summary>
        ''' <returns></returns>
        Public Property Colors As RExpression = [NULL]
        ''' <summary>
        ''' optional specification of module colors on the left side. If NULL, Colors will be used.
        ''' </summary>
        ''' <returns></returns>
        Public Property ColorsLeft As RExpression = Colors
        ''' <summary>
        ''' logical: should terrain colors be used?
        ''' </summary>
        ''' <returns></returns>
        Public Property terrainColors As Boolean = False
        ''' <summary>
        ''' logical: should layout be set? If TRUE, standard layout for one plot will be used. Note that this precludes multiple plots on one page. 
        ''' If FALSE, the user is responsible for setting the correct layout.
        ''' </summary>
        ''' <returns></returns>
        Public Property setLayout As Boolean = True
    End Class
End Namespace

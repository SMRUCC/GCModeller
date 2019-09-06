#Region "Microsoft.VisualBasic::6745661293f4b40289a51af6c2402773, RDotNET.Extensions.VisualBasic\ScriptBuilder\RPackage\stats\heatmap.vb"

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

    '     Class heatmap
    ' 
    '         Properties: Colv, Rowv
    ' 
    '         Function: Puriney
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports RDotNET.Extensions.VisualBasic.SymbolBuilder.Abstract
Imports RDotNET.Extensions.VisualBasic.SymbolBuilder.Rtypes

Namespace SymbolBuilder.packages.stats

    ''' <summary>
    ''' A heat map is a false color image (basically image(t(x))) with a dendrogram added to the left side and to the top. Typically, reordering of the rows and columns according to some set of values (row or column means) within the restrictions imposed by the dendrogram is carried out.
    ''' </summary>
    ''' <remarks>
    ''' If either Rowv or Colv are dendrograms they are honored (and not reordered). Otherwise, dendrograms are computed as dd &lt;- as.dendrogram(hclustfun(distfun(X))) where X is either x or t(x).
    ''' If either Is a vector (Of 'weights’) then the appropriate dendrogram is reordered according to the supplied values subject to the constraints imposed by the dendrogram, by reorder(dd, Rowv), in the row case. If either is missing, as by default, then the ordering of the corresponding dendrogram is by the mean value of the rows/columns, i.e., in the case of rows, Rowv &lt;- rowMeans(x, na.rm = na.rm). If either is NA, no reordering will be done for the corresponding side.
    ''' By Default (scale = "row") the rows are scaled to have mean zero And standard deviation one. There Is some empirical evidence from genomic plotting that this Is useful.
    ''' The Default colors are Not pretty. Consider Using enhancements such As the RColorBrewer package.
    ''' </remarks>
    <RFunc("heatmap")> Public Class heatmap : Inherits heatmap_plot

        ''' <summary>
        ''' determines if and how the row dendrogram should be computed and reordered. Either a dendrogram or a vector of values used to reorder the row dendrogram or NA to suppress any row dendrogram (and reordering) or by default, NULL, see ‘Details’ below.
        ''' </summary>
        ''' <returns></returns>
        Public Property Rowv As RExpression = NULL

        ''' <summary>
        ''' determines if and how the column dendrogram should be reordered. Has the same options as the Rowv argument above and additionally when x is a square matrix, Colv = "Rowv" means that columns should be treated identically to the rows (and so if there is to be no row dendrogram there will not be a column one either).
        ''' </summary>
        ''' <returns></returns>
        Public Property Colv As RExpression = "if(symm)""Rowv"" else NULL"

        Public Shared Function Puriney(x As String) As heatmap
            Dim heatmap As New heatmap With {
                .x = x,
                .Rowv = NA,
                .Colv = NA,
                .revC = [TRUE],
                .scale = "'column'"
                }
            Return heatmap
        End Function
    End Class
End Namespace

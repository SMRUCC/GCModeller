#Region "Microsoft.VisualBasic::ebba8518fe8f41033006c3a32fb22a39, RDotNet.Extensions.Bioinformatics\Declares\pheatmap\API.vb"

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

    '     Module API
    ' 
    '         Function: pheatmap
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports RDotNET.Extensions.VisualBasic

Namespace pheatmap

    ''' <summary>
    ''' A function to draw clustered heatmaps.
    ''' </summary>
    Public Module API

        Const DefaultPattern As String = "colorRampPalette(rev(brewer.pal(n= 7, name= ""RdYlBu"")))(100)"

        ''' <summary>
        ''' A function to draw clustered heatmaps where one has better control over some graphical parameters such as cell size, etc. 
        ''' </summary>
        ''' <param name="mat">numeric matrix of the values to be plotted.</param>
        ''' <param name="color">vector of colors used in heatmap.</param>
        ''' <param name="kmeans_k">the number of kmeans clusters to make, if we want to agggregate the rows before drawing heatmap. If NA then the rows are not aggregated.</param>
        ''' <param name="breaks">a sequence of numbers that covers the range of values in mat and is one element longer than color vector. Used for mapping values to colors. Useful, if needed to map certain values to certain colors, to certain values. If value is NA then the breaks are calculated automatically.</param>
        ''' <param name="border_color">color of cell borders on heatmap, use NA if no border should be drawn.</param>
        ''' <param name="cellwidth"></param>
        ''' <param name="cellheight"></param>
        ''' <param name="scale"></param>
        ''' <param name="cluster_rows"></param>
        ''' <param name="cluster_cols"></param>
        ''' <param name="clustering_distance_rows"></param>
        ''' <param name="clustering_distance_cols"></param>
        ''' <param name="clustering_method"></param>
        ''' <param name="clustering_callback"></param>
        ''' <param name="cutree_rows"></param>
        ''' <param name="cutree_cols"></param>
        ''' <param name="treeheight_row"></param>
        ''' <param name="treeheight_col"></param>
        ''' <param name="legend"></param>
        ''' <param name="legend_breaks"></param>
        ''' <param name="legend_labels"></param>
        ''' <param name="annotation_row"></param>
        ''' <param name="annotation_col"></param>
        ''' <param name="annotation"></param>
        ''' <param name="annotation_colors"></param>
        ''' <param name="annotation_legend"></param>
        ''' <param name="annotation_names_row"></param>
        ''' <param name="annotation_names_col"></param>
        ''' <param name="drop_levels"></param>
        ''' <param name="show_rownames"></param>
        ''' <param name="show_colnames"></param>
        ''' <param name="main"></param>
        ''' <param name="fontsize"></param>
        ''' <param name="fontsize_row"></param>
        ''' <param name="fontsize_col"></param>
        ''' <param name="display_numbers"></param>
        ''' <param name="number_format"></param>
        ''' <param name="number_color"></param>
        ''' <param name="fontsize_number"></param>
        ''' <param name="gaps_row"></param>
        ''' <param name="gaps_col"></param>
        ''' <param name="labels_row"></param>
        ''' <param name="labels_col"></param>
        ''' <param name="filename"></param>
        ''' <param name="width"></param>
        ''' <param name="height"></param>
        ''' <param name="silent"></param>
        ''' <param name="additionals">graphical parameters for the text used in plot. Parameters passed to grid.text, see gpar.</param>
        ''' <returns>
        ''' Invisibly a list of components 
        '''
        ''' + tree_row the clustering of rows as hclust object 
        ''' + tree_col the clustering of columns as hclust object 
        ''' + kmeans the kmeans clustering of rows if parameter kmeans_k was specified 
        ''' </returns>
        ''' <remarks>
        ''' The function also allows to aggregate the rows using kmeans clustering. 
        ''' This is advisable if number of rows is so big that R cannot handle their 
        ''' hierarchical clustering anymore, roughly more than 1000. Instead of 
        ''' showing all the rows separately one can cluster the rows in advance and 
        ''' show only the cluster centers. The number of clusters can be tuned with 
        ''' parameter kmeans_k. 
        ''' </remarks>
        Public Function pheatmap(mat As String,
                                 Optional color As String = DefaultPattern,
                                 Optional kmeans_k As String = NA,
                                 Optional breaks As String = NA,
                                 Optional border_color As String = "grey60",
                                 Optional cellwidth As String = NA,
                                 Optional cellheight As String = NA,
                                 Optional scale As String = "none",
                                 Optional cluster_rows As Boolean = True,
                                 Optional cluster_cols As Boolean = True,
                                 Optional clustering_distance_rows As String = "euclidean",
                                 Optional clustering_distance_cols As String = "euclidean",
                                 Optional clustering_method As String = "complete",
                                 Optional clustering_callback As String = "identity2",
                                 Optional cutree_rows As String = NA,
                                 Optional cutree_cols As String = NA,
                                 Optional treeheight_row As String = "ifelse((class(cluster_rows) == ""hclust"") || cluster_rows, 50, 0)",
                                 Optional treeheight_col As String = "ifelse((class(cluster_cols) == ""hclust"") || cluster_cols, 50, 0)",
                                 Optional legend As Boolean = True,
                                 Optional legend_breaks As String = NA,
                                 Optional legend_labels As String = NA,
                                 Optional annotation_row As String = NA,
                                 Optional annotation_col As String = NA,
                                 Optional annotation As String = NA,
                                 Optional annotation_colors As String = NA,
                                 Optional annotation_legend As Boolean = True,
                                 Optional annotation_names_row As Boolean = True,
                                 Optional annotation_names_col As Boolean = True,
                                 Optional drop_levels As Boolean = True,
                                 Optional show_rownames As String = "t()",
                                 Optional show_colnames As String = "t()",
                                 Optional main As String = NA,
                                 Optional fontsize As Integer = 10,
                                 Optional fontsize_row As String = "fontsize",
                                 Optional fontsize_col As String = "fontsize",
                                 Optional display_numbers As String = "F",
                                 Optional number_format As String = "%.2f",
                                 Optional number_color As String = "grey30",
                                 Optional fontsize_number As String = "0.8 * fontsize",
                                 Optional gaps_row As String = NULL,
                                 Optional gaps_col As String = NULL,
                                 Optional labels_row As String = NULL,
                                 Optional labels_col As String = NULL,
                                 Optional filename As String = NA,
                                 Optional width As String = NA,
                                 Optional height As String = NA,
                                 Optional silent As Boolean = False,
                                 Optional additionals As String() = Nothing)
        End Function
    End Module
End Namespace

# API
_namespace: [RDotNET.Extensions.Bioinformatics.pheatmap](./index.md)_

A function to draw clustered heatmaps.



### Methods

#### pheatmap
```csharp
RDotNET.Extensions.Bioinformatics.pheatmap.API.pheatmap(System.String,System.String,System.String,System.String,System.String,System.String,System.String,System.String,System.Boolean,System.Boolean,System.String,System.String,System.String,System.String,System.String,System.String,System.String,System.String,System.Boolean,System.String,System.String,System.String,System.String,System.String,System.String,System.Boolean,System.Boolean,System.Boolean,System.Boolean,System.String,System.String,System.String,System.Int32,System.String,System.String,System.String,System.String,System.String,System.String,System.String,System.String,System.String,System.String,System.String,System.String,System.String,System.Boolean,System.String[])
```
A function to draw clustered heatmaps where one has better control over some graphical parameters such as cell size, etc.

|Parameter Name|Remarks|
|--------------|-------|
|mat|numeric matrix of the values to be plotted.|
|color|vector of colors used in heatmap.|
|kmeans_k|the number of kmeans clusters to make, if we want to agggregate the rows before drawing heatmap. If NA then the rows are not aggregated.|
|breaks|a sequence of numbers that covers the range of values in mat and is one element longer than color vector. Used for mapping values to colors. Useful, if needed to map certain values to certain colors, to certain values. If value is NA then the breaks are calculated automatically.|
|border_color|color of cell borders on heatmap, use NA if no border should be drawn.|
|cellwidth|-|
|cellheight|-|
|scale|-|
|cluster_rows|-|
|cluster_cols|-|
|clustering_distance_rows|-|
|clustering_distance_cols|-|
|clustering_method|-|
|clustering_callback|-|
|cutree_rows|-|
|cutree_cols|-|
|treeheight_row|-|
|treeheight_col|-|
|legend|-|
|legend_breaks|-|
|legend_labels|-|
|annotation_row|-|
|annotation_col|-|
|annotation|-|
|annotation_colors|-|
|annotation_legend|-|
|annotation_names_row|-|
|annotation_names_col|-|
|drop_levels|-|
|show_rownames|-|
|show_colnames|-|
|main|-|
|fontsize|-|
|fontsize_row|-|
|fontsize_col|-|
|display_numbers|-|
|number_format|-|
|number_color|-|
|fontsize_number|-|
|gaps_row|-|
|gaps_col|-|
|labels_row|-|
|labels_col|-|
|filename|-|
|width|-|
|height|-|
|silent|-|
|additionals|graphical parameters for the text used in plot. Parameters passed to grid.text, see gpar.|


_returns: 
 Invisibly a list of components 

 + tree_row the clustering of rows as hclust object 
 + tree_col the clustering of columns as hclust object 
 + kmeans the kmeans clustering of rows if parameter kmeans_k was specified 
 _
> 
>  The function also allows to aggregate the rows using kmeans clustering. 
>  This is advisable if number of rows is so big that R cannot handle their 
>  hierarchical clustering anymore, roughly more than 1000. Instead of 
>  showing all the rows separately one can cluster the rows in advance and 
>  show only the cluster centers. The number of clusters can be tuned with 
>  parameter kmeans_k. 
>  



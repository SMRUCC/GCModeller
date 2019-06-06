Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Analysis.PFSNet.DataStructure
Imports SMRUCC.genomics.Analysis.PFSNet.R

<HideModuleName> Module masked_ggi

    ''' <summary>
    ''' ```R
    ''' ggi_mask &lt;- apply(ggi, 1, func &lt;- function(i){
	'''	if ((i[2] %in% genelist1$gl) &amp;&amp; (i[3] %in% genelist1$gl))
	''' 		TRUE
	''' 	else FALSE
	''' })
    ''' ```
    ''' </summary>
    ''' <param name="ggi"></param>
    ''' <param name="genelist"></param>
    ''' <returns></returns>
    <Extension>
    Public Function ggi_mask(ggi As IEnumerable(Of GraphEdge), genelist As Index(Of String)) As Boolean()
        Return ggi _
            .Select(Function(i)
                        If i.g1 Like genelist AndAlso i.g2 Like genelist Then
                            Return True
                        Else
                            Return False
                        End If
                    End Function) _
            .ToArray
    End Function

    ''' <summary>
    ''' ```R
    ''' masked.ggi &lt;- ggi[ggi_mask, ]
    ''' colnames(masked.ggi) &lt;- c("pathway", "g1", "g2")
    '''
    ''' masked.ggi &lt;- masked.ggi[(masked.ggi[, "g1"] != masked.ggi[, "g2"]), ]
    ''' ```
    ''' </summary>
    ''' <param name="ggi"></param>
    ''' <param name="ggi_mask"></param>
    ''' <returns></returns>
    <Extension>
    Public Function getMasked_ggi(ggi As IEnumerable(Of GraphEdge), ggi_mask As Boolean()) As GraphEdge()
        Dim masked_ggi As GraphEdge() = ggi _
            .Takes(ggi_mask) _
            .ToArray
        masked_ggi = masked_ggi _
            .Where(Function(i) i.g1 <> i.g2) _
            .ToArray

        Return masked_ggi
    End Function

    ''' <summary>
    ''' ```R
    ''' ccs &lt;- sapply(unique(masked.ggi[, "pathway"]), bypathway &lt;- function(pathwayid) {
    '''     g &lt;- graph.data.frame(masked.ggi[ masked.ggi[,"pathway"]==pathwayid, c("g1", "g2", "pathway")],directed=FALSE)
    '''     
    '''     for(i in 1:length(V(g))){
    '''
    '''	        V(g)[V(g)$name[i]]$weight&lt;-sum(w1matrix1[V(g)$name[[i]],])/sum(!is.na(w1matrix1[V(g)$name[[1]],]))
    '''         V(g)[V(g)$name[i]]$weight2&lt;-sum(w1matrix2[V(g)$name[[i]],])/sum(!is.na(w1matrix2[V(g)$name[[1]],]))
    '''     }
    '''     g&lt;-simplify(g)
    '''     decompose.graph(g,min.vertices=5)
    ''' })
    '''
    ''' ccs &lt;- unlist(ccs, recursive=FALSE)
    ''' ```
    ''' </summary>
    ''' <param name="masked_ggi"></param>
    ''' <param name="w1matrix1"></param>
    ''' <param name="w1matrix2"></param>
    ''' <returns></returns>
    Public Function ccs(masked_ggi As GraphEdge(), w1matrix1 As DataFrameRow(), w1matrix2 As DataFrameRow()) As IEnumerable(Of PFSNetGraph)
        Dim pathwayList = (From gene As GraphEdge
                           In masked_ggi
                           Select gene
                           Group gene By gene.PathwayID Into Group) _
            .ToDictionary(Function(g) g.PathwayID,
                          Function(genes)
                              Return genes.Group.ToArray
                          End Function)

        Return From pathway In pathwayList
               Let genes As GraphEdge() = pathway.Value
               Let V = InternalVg(genes, w1matrix1, w1matrix2)
               Where Not V Is Nothing
               Select V
    End Function

    ''' <summary>
    ''' 函数会忽略掉边数目少于5的网络
    ''' </summary>
    ''' <param name="data"></param>
    ''' <param name="w1matrix1"></param>
    ''' <param name="w1matrix2"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function InternalVg(data As GraphEdge(), w1matrix1 As DataFrameRow(), w1matrix2 As DataFrameRow()) As PFSNetGraph
        ' 总的网络
        Dim g As PFSNetGraph = Graph.Data.Frame(data)

        For i As Integer = 0 To g.Nodes.Length - 1
            g.Nodes(i).weight = w1matrix1.Select(g.Nodes(i).Name).experiments.Sum / w1matrix1.Select(g.Nodes.First.Name).experiments.Sum
            g.Nodes(i).weight2 = w1matrix2.Select(g.Nodes(i).Name).experiments.Sum / w1matrix2.Select(g.Nodes.First.Name).experiments.Sum
        Next

        ' 计算出总的网络之后再将总的网络分解为小得网络对象
        g = Graph.simplify(g)
        Return g  '   Return Graph.decompose_graph(g, min_vertices:=5)
    End Function
End Module

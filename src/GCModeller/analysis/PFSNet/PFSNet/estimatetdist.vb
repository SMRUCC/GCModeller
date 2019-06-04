Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Math.Statistics.Hypothesis
Imports Microsoft.VisualBasic.Terminal.STDIO
Imports SMRUCC.genomics.Analysis.PFSNet.DataStructure
Imports SMRUCC.genomics.Analysis.PFSNet.R

<HideModuleName> Module estimatetdist

    Public Function estimatetdist(d1 As DataFrameRow(), d2 As DataFrameRow(), ggi As GraphEdge(), b As Double, t1 As Double, t2 As Double, n As Integer) As Double()
        Dim total = cbind(d1, d2) 'total<-cbind(d1,d2)
        Dim tdistribution As New List(Of Double) 'tdistribution<-c()

        For i As Integer = 0 To n - 1
            cat(".")

            Dim samplevector = Sample(total.First.Samples - 1, d1.First.Samples - 1)
            Dim expr1o = DataFrameRow.TakeSamples(total, samplevector, False)
            Dim expr2o = DataFrameRow.TakeSamples(total, samplevector, True)
            Dim w1matrix1 = PFSNet.computew1(expr1o, theta1:=t1, theta2:=t2)
            Dim w1matrix2 = PFSNet.computew1(expr2o, theta1:=t1, theta2:=t2)

            Dim genelist1 As Index(Of String) = computegenelist(w1matrix1, beta:=b)
            Dim genelist2 As Index(Of String) = computegenelist(w1matrix2, beta:=b)

            'ggi_mask <- apply(ggi, 1, func <- function(i){
            ' 	if ((i[2] %in% genelist1$gl) && (i[3] %in% genelist1$gl))
            ' 		TRUE
            ' 	else FALSE
            '})
            Dim ggi_mask = ggi.Select(Function(interaction)
                                          If interaction.g1 Like genelist1 AndAlso interaction.g2 Like genelist1 Then
                                              Return True
                                          Else
                                              Return False
                                          End If
                                      End Function).ToArray
            Dim masked_ggi As GraphEdge() = ggi.Takes(ggi_mask).ToArray

            ' masked.ggi <- masked.ggi[(masked.ggi[, "g1"] != masked.ggi[, "g2"]), ]
            masked_ggi = masked_ggi _
                .Where(Function(inter) inter.g1 <> inter.g2) _
                .ToArray

            Dim bypathway = Function(pathwayid As String)
                                ' 总的网络
                                Dim g As PFSNetGraph = Graph.Data.Frame(masked_ggi.Where(Function(p) p.PathwayID = pathwayid).ToArray)

                                For ivg As Integer = 0 To g.Nodes.Length - 1
                                    g.Nodes(ivg).weight = w1matrix1.Select(g.Nodes(ivg).Name).experiments.Sum / w1matrix1.Select(g.Nodes.First.Name).experiments.Where(Function(y) Not y.IsNaNImaginary).Count
                                    g.Nodes(ivg).weight2 = w1matrix2.Select(g.Nodes(ivg).Name).experiments.Sum / w1matrix2.Select(g.Nodes.First.Name).experiments.Where(Function(y) Not y.IsNaNImaginary).Count
                                Next

                                ' 计算出总的网络之后再将总的网络分解为小得网络对象
                                g = Graph.simplify(g)
                                Return g  '   Return Graph.decompose_graph(g, min_vertices:=5)
                            End Function

            Dim ccs = masked_ggi.Select(Function(p) p.PathwayID).Distinct.Select(bypathway).ToArray

            '	ccs <- unlist(ccs, recursive=FALSE)

            Dim pscore = sapply_pscore(ccs, w1matrix1)
            Dim nscore = sapply_nscore(ccs, w1matrix1)

            Dim statistics As Double() = rep(False, ccs.Length)
            Dim rand As New Random

            For p As Integer = 0 To ccs.Length - 1
                Dim x = pscore(p), y = nscore(p)
                Dim ttest = t.Test(x, y)

                statistics(i) = ttest.TestValue
            Next

            Call tdistribution.AddRange(statistics)

            If tdistribution.Count > 1000 Then
                Exit For
            End If
        Next

        Return tdistribution.ToArray
    End Function

    Private Function sapply_pscore(ccs As PFSNetGraph(), w1matrix1 As DataFrameRow()) As Double()()
        Return (From x In ccs Select pscoring(x, w1matrix1)).ToArray
    End Function

    Private Function pscoring(x As PFSNetGraph, w1matrix1 As DataFrameRow()) As Double()
        Dim vertices = (From ccs_node In x.Nodes Select ccs_node.Name).ToArray  'vertices<-get.vertex.attribute(x,name="name")
        Dim ws = w1matrix1.Select(vertices) 'ws<-w1matrix1[vertices,,drop=FALSE]
        Dim v = New List(Of Double) '	v<-c()
        Dim ncol = ws(Scan0).Samples

        For j As Integer = 0 To ncol - 1
            Dim p = j
            Dim sum = (From d In ws Select x.Node(d.Name).weight * d.experiments(p)).Sum

            Call v.Add(sum)
        Next
        'v()
        '#apply(ss,2,function(y){
        '#		sum(apply(ss[y,,drop=FALSE],1,sum)/ncol(expr1o))
        '#	})
        Return v.ToArray
    End Function

    Private Function sapply_nscore(ccs As PFSNetGraph(), w1matrix1 As DataFrameRow()) As Double()()
        Return (From node As PFSNetGraph In ccs Select nscoring(node, w1matrix1)).ToArray
    End Function

    Private Function nscoring(x As PFSNetGraph, w1matrix1 As DataFrameRow()) As Double()
        Dim vertices = (From ccs_node In x.Nodes Select ccs_node.Name).ToArray  'vertices<-get.vertex.attribute(x,name="name")
        Dim ws = w1matrix1.Select(vertices) 'ws<-w1matrix1[vertices,,drop=FALSE]
        Dim v = New List(Of Double) '	v<-c()
        Dim ncol = ws(Scan0).Samples

        For j As Integer = 0 To ncol - 1
            Dim p = j
            Dim sum = (From d In ws Select x.Node(d.Name).weight2 * d.experiments(p)).Sum

            Call v.Add(sum)
        Next
        'v()
        '#apply(ss,2,function(y){
        '#		sum(apply(ss[y,,drop=FALSE],1,sum)/ncol(expr1o))
        '#	})
        Return v.ToArray
    End Function
End Module

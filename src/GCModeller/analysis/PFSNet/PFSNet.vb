#Region "Microsoft.VisualBasic::74229f915b453da7c6ae45fc807b75ed, ..\GCModeller\analysis\PFSNet\PFSNet.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
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

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Terminal.STDIO
Imports SMRUCC.genomics.Analysis.PFSNet.DataStructure
Imports SMRUCC.genomics.Analysis.PFSNet.R

''' <summary>
''' Implements the pfsnet algorithm to calculates the significant and consist cellular network between two types of mutations.
''' (计算两种突变体的共有的表型变化的PfsNET算法)
''' </summary>
''' <remarks></remarks>
''' 
<Package("PfsNET.Parallel",
                    Description:="PfsNET algorithm implments in VisualBasic language for large scale network high-performance calculation.",
                    Publisher:="xie.guigang@gcmodeller.org")>
Public Module PFSNet

    ''' <summary>
    ''' The united PfsNET evaluate interface handler for the R version scripting engine and VisualBasic evaluation engine.
    ''' (R版本和VB版本的计算函数的统一接口(好像R的版本不能够并行化))
    ''' </summary>
    ''' <param name="file1">The expression data file of mutation for one phenotype.(基因表达数据1)</param>
    ''' <param name="file2">The expression data file of mutation for another phenotype.(基因表达数据2)</param>
    ''' <param name="file3">The gene interaction relationship table.(基因互作关系列表)</param>
    ''' <param name="b">The sub network screening cutoff value.(子网络基因筛选阈值)</param>
    ''' <param name="t1">The upbound value of the fuzzy weight calculation.(模糊权重计算上限)</param>
    ''' <param name="t2">The lower bound value of the fuzzy weight calculation.(模糊权重计算下限)</param>
    ''' <param name="n">The hypothesis testing count of the PfsNET calculation sub network.(假设检验的统计次数)</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Delegate Function PFSNetEvaluateHandle(file1 As String, file2 As String, file3 As String, b As Double, t1 As Double, t2 As Double, n As Double) As PFSNetResultOut

    '''<summary>
    ''' The original pfsnet algorithm implemented in language R.
    ''' 
    ''' ```R
    ''' require(igraph)
    ''' #require(rJava)
    '''
    ''' loaddata&lt;-function(file){
    '''	 a&lt;-read.table(file,row.names=1)
    '''	 a
    ''' }
    '''
    ''' computew1&lt;-function(expr,theta1,theta2){
    '''	 ranks&lt;-apply(expr,2,function(x){
    '''	 	rank(x)/length(x)
    '''	 })
    '''	 apply(ranks,2,function(x){
    '''		q2&lt;-quantile(x,theta2,na.rm=T)
    '''		q1&lt;-quantile(x,theta1,na.rm=T)
    '''		m&lt;-median(x)
    '''		mx&lt;-max(x)
    '''		sapply(x,function(y){
    '''			if(is.na(y)){
    '''				NA
    '''			}
    '''			else if(y&gt;=q1)
    '''				1
    '''			else if(y&gt;=q2)
    '''				(y-q2)/(q1-q2)
    '''			else
    '''				0
    '''		})
    '''	 })
    ''' }
    '''
    ''' pfsnet.computegenelist&lt;-function(w,beta){
    '''	# within [rest of string was truncated]&quot;;.
    '''	```
    '''</summary>
    Public ReadOnly Property OriginalRAlgorithm As String
        Get
            Return My.Resources.pfsnet
        End Get
    End Property

    ''' <summary>
    ''' ```R
    ''' ranks&lt;-apply(expr,2,function(x){
    '''	   rank(x)/length(x)
    ''' })
    ''' ```
    ''' 
    ''' apply函数之中的MARGIN参数的含义：
    ''' MARGIN	
    ''' a vector giving the subscripts which the function will be applied over. E.g., for a matrix 1 indicates rows, 2 indicates columns, c(1, 2) indicates rows and columns. 
    ''' Where X has named dimnames, it can be a character vector selecting dimension names.
    ''' 即上述的R函数是对矩阵之中的每一列进行计算
    ''' </summary>
    ''' <param name="expr"></param>
    ''' <param name="theta1"></param>
    ''' <param name="theta2"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function computew1(expr As DataFrameRow(), theta1 As Double, theta2 As Double) As DataFrameRow()
        Dim data = (From item In expr Select item.ExperimentValues).ToArray.ToMatrix
        Call ALGLIB.alglib.rankdata(data)
        Dim ldata = data.ToVectorList
        expr = (From i As Integer In expr.Sequence Select New DataFrameRow With {.Name = expr(i).Name, .ExperimentValues = ldata(i)}).ToArray
        Dim LQueryCache = DataFrameRow.CreateApplyFunctionCache(expr)
        Dim fuzzyWeights = (From line In LQueryCache.Value Select computew1(line, theta1, theta2)).ToArray
        Return DataFrameRow.CreateDataFrameFromCache(LQueryCache.Key, fuzzyWeights)
    End Function

    Public Function computew1(expr As Double(), theta1 As Double, theta2 As Double) As Double()
        Dim q1 As Double = R.Base.Quantile(expr, theta1)
        Dim q2 As Double = R.Base.Quantile(expr, theta2)
        Dim delta_q12 As Double = q1 - q2
        Dim Weights As Double() = (From x As Double In expr Select Internal_getFuzzyWeight(x, q1, q2, delta_q12)).ToArray
        Return Weights
    End Function

    ''' <summary>
    ''' 计算模糊权重
    ''' </summary>
    ''' <param name="y"></param>
    ''' <param name="q1"></param>
    ''' <param name="q2"></param>
    ''' <param name="delta_q12"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function Internal_getFuzzyWeight(y As Double, q1 As Double, q2 As Double, delta_q12 As Double) As Double
        If y >= q1 Then
            Return 1
        ElseIf y >= q2 Then
            Return (y - q2) / delta_q12
        ElseIf q1.IsNaNImaginary OrElse q2.IsNaNImaginary Then
            Return Double.NaN
        Else
            Return 0
        End If
    End Function

    ''' <summary>
    ''' ```R
    ''' list.mask&lt;-apply(w,1,function(x){
    '''	  sum(x,na.rm=T)/sum(!is.na(x)) >= beta
    ''' })
    ''' list(gl=rownames(w)[list.mask])
    ''' ```
    ''' 
    ''' apply函数是对行进行统计的
    ''' 
    ''' 函数返回行的编号的列表
    ''' </summary>
    ''' <param name="w"></param>
    ''' <param name="beta"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function computegenelist(w As DataFrameRow(), beta As Double) As String()
        Dim list_mask = (From w_row As DataFrameRow In w.AsParallel
                         Let x As Double() = w_row.ExperimentValues
                         Let d = x.Sum / (From n As Double In x Let b As Integer = If(n.IsNaNImaginary, 0, 1) Select b).Sum
                         Where d >= beta
                         Select w_row.Name, d).ToArray
        Return (From obj In list_mask Select obj.Name).ToArray
    End Function

    <ExportAPI("PfsNet.Evaluate")>
    Public Function pfsnet(file1 As String, file2 As String, file3 As String, Optional b As Double = 0.5, Optional t1 As Double = 0.95, Optional t2 As Double = 0.85, Optional n As Double = 1000) _
        As PFSNetResultOut

        cat("reading data files")
        Dim ggi As GraphEdge() = GraphEdge.LoadData(file3)
        cat(".")
        Dim expr1o As DataFrameRow() = DataFrameRow.LoadData(file1)
        cat(".")
        Dim expr2o As DataFrameRow() = DataFrameRow.LoadData(file2)
        cat(".")
        cat("\t[DONE]\n")

        Return pfsnet(expr1o, expr2o, ggi, b, t1, t2, n)
    End Function

    <ExportAPI("PfsNET.Evaluate")>
    Public Function pfsnet(expr1o As DataFrameRow(), expr2o As DataFrameRow(), ggi As GraphEdge(), Optional b As Double = 0.5, Optional t1 As Double = 0.95, Optional t2 As Double = 0.85, Optional n As Double = 1000) _
        As PFSNetResultOut

        Dim proc As Stopwatch = Stopwatch.StartNew

        cat("computing fuzzy weights")
        Dim w1matrix1 = computew1(expr1o, theta1:=t1, theta2:=t2)
        Dim w1matrix2 = computew1(expr2o, theta1:=t1, theta2:=t2)
        cat(".")
        Dim genelist1 As String() = computegenelist(w1matrix1, beta:=b)
        Dim genelist2 As String() = computegenelist(w1matrix2, beta:=b)
        cat("\t[DONE]\n")
        cat("computing subnetworks")

        Dim masked_ggi = InternalMaskGGI(ggi, genelist1, genelist2, False)
        Dim ccs = (From g In (From item In masked_ggi Select item Group item By item.PathwayID Into Group).ToArray
                   Let V = InternalVg(g.Group.ToArray, w1matrix1, w1matrix2)
                   Where Not V Is Nothing
                   Select V).ToArray
        'ccs <- unlist(ccs, recursive=FALSE)
        masked_ggi = InternalMaskGGI(ggi, genelist1, genelist2, True)
        Dim ccs2 = (From g In (From item In masked_ggi Select item Group item By item.PathwayID Into Group).ToArray
                    Let V = InternalVg(g.Group.ToArray, w1matrix1, w1matrix2)
                    Where Not V Is Nothing
                    Select V).ToArray
        ' ccs2 <- unlist(ccs2, recursive=FALSE)
        cat(".")
        cat("\t[DONE]\n")
        cat("computing subnetwork scores")

        Dim pscore = Internal_get_pscore(ccs, w1matrix1)
        cat(".")
        Dim pscore2 = Internal_get_pscore(ccs2, w1matrix2)
        cat(".")

        Dim nscore = Internal_get_nscore(ccs, w1matrix1)
        Dim nscore2 = Internal_get_nscore(ccs2, w1matrix2)

        cat(".")
        cat(".\t[DONE]\n")
        cat("computing permuation tests ")

        Dim tdist = estimatetdist(expr1o, expr2o, ggi, b, t1, t2, n)
        Dim tdist2 = estimatetdist(expr2o, expr1o, ggi, b, t1, t2, n)

        ccs = Internal_statics(ccs, pscore, nscore, tdist)
        ccs2 = Internal_statics(ccs2, pscore2, nscore2, tdist2)

        cat("\t[DONE]\n")
        cat("total time elapsed: ", proc.ElapsedMilliseconds / 1000, " seconds\n")

        Dim analysisResult As New PFSNetResultOut With {
            .Phenotype1 = (From item In ccs Where item.masked Select item).ToArray,
            .Phenotype2 = (From item In ccs2 Where item.masked Select item).ToArray
        }
        Return analysisResult
    End Function

    <ExportAPI("Write.Xml.PfsNET", Info:="Write the PfsNET calculation result into a xml model.")>
    Public Function WriteXMLPfsNET(data As PFSNetResultOut, saveto As String) As Boolean
        Dim xml As String = data.GetXml
        Return xml.SaveTo(saveto)
    End Function

    Private Function InternalMaskGGI(ggi As GraphEdge(), genelist1 As String(), genelist2 As String(), reversed As Boolean) As GraphEdge()
        Dim masked_ggi As GraphEdge()

        If Not reversed Then
            '# ggi_mask <- apply(ggi, 1, func <- function(i){
            '# 	if ((i[2] %in% genelist1$gl) && (i[3] %in% genelist1$gl))
            '# 		TRUE
            '# 	else FALSE
            '# })
            masked_ggi = (From item As GraphEdge In ggi
                          Where (Array.IndexOf(genelist1, item.g1) > -1 AndAlso Array.IndexOf(genelist1, item.g2) > -1) AndAlso Not String.Equals(item.g1, item.g2)
                          Select item).ToArray       '	masked.ggi <- masked.ggi[(masked.ggi[, "g1"] != masked.ggi[, "g2"]), ]
        Else
            masked_ggi = (From item As GraphEdge In ggi
                          Where (Array.IndexOf(genelist1, item.g2) > -1 AndAlso Array.IndexOf(genelist1, item.g1) > -1) AndAlso Not String.Equals(item.g1, item.g2)
                          Select item).ToArray
        End If

        Return masked_ggi
    End Function

    Private Function Internal_statics(ccs_nodes As PFSNetGraph(), _pscore As Double()(), _nscore As Double()(), _tdist As Double()) As PFSNetGraph()
        For i As Integer = 0 To ccs_nodes.Length - 1
            Dim ccs_node = ccs_nodes(i)
            Dim x = _pscore(i), y = _nscore(i)
            Dim b, l, r As Double
            Call ALGLIB.alglib.studentttests.studentttest2(x, x.Length, y, y.Length, b, l, r)
            ccs_node.statistics = b
            ccs_node.pvalue = (From p In _tdist.Sequence Where Math.Abs(_tdist(p)) >= ccs_node.statistics Select _tdist(p)).ToArray.Sum / _tdist.Length
            ccs_node.masked = ccs_node.pvalue < 0.05
        Next
        Return ccs_nodes
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
        If data.Count < 5 Then
            Return Nothing
        End If

        Dim g = Graph.Data.Frame(data)  '总的网络
        For p As Integer = 0 To g.Nodes.Length - 1
            g.Nodes(p).weight = w1matrix1.Select(g.Nodes(p).Name).ExperimentValues.Sum / w1matrix1.Select(g.Nodes.First.Name).ExperimentValues.Sum
            g.Nodes(p).weight2 = w1matrix2.Select(g.Nodes(p).Name).ExperimentValues.Sum / w1matrix2.Select(g.Nodes.First.Name).ExperimentValues.Sum
        Next
        g = Graph.simplify(g) '计算出总的网络之后再将总的网络分解为小得网络对象
        Return g  '   Return Graph.decompose_graph(g, min_vertices:=5)
    End Function

    Public Function estimatetdist(d1 As DataFrameRow(), d2 As DataFrameRow(), ggi As GraphEdge(), b As Double, t1 As Double, t2 As Double, n As Integer) As Double()
        Dim total = cbind(d1, d2) 'total<-cbind(d1,d2)
        Dim tdistribution As List(Of Double) = New List(Of Double) 'tdistribution<-c()

        For i As Integer = 0 To n
            cat(".")
            Dim samplevector = Sample(total.First.Samples - 1, d1.First.Samples - 1)
            Dim expr1o = DataFrameRow.TakeSamples(total, samplevector, False)
            Dim expr2o = DataFrameRow.TakeSamples(total, samplevector, True)
            Dim w1matrix1 = computew1(expr1o, theta1:=t1, theta2:=t2)
            Dim w1matrix2 = computew1(expr2o, theta1:=t1, theta2:=t2)

            Dim genelist1 = computegenelist(w1matrix1, beta:=b)
            Dim genelist2 = computegenelist(w1matrix2, beta:=b)

            'ggi_mask <- apply(ggi, 1, func <- function(i){
            ' 	if ((i[2] %in% genelist1$gl) && (i[3] %in% genelist1$gl))
            ' 		TRUE
            ' 	else FALSE
            '})
            Dim masked_ggi = InternalMaskGGI(ggi, genelist1, genelist2, False)
            Dim ccs = (From g In (From item In masked_ggi Select item Group item By item.PathwayID Into Group).ToArray
                       Let V = InternalVg(g.Group.ToArray, w1matrix1, w1matrix2)
                       Where Not V Is Nothing
                       Select V).ToArray

            '	ccs <- unlist(ccs, recursive=FALSE)

            Dim pscore = Internal_get_pscore(ccs, w1matrix1)
            Dim nscore = Internal_get_nscore(ccs, w1matrix1)

            Dim statistics As Double() = rep(False, ccs.Length)
            Dim rand As New Random

            For p As Integer = 0 To ccs.Length - 1
                Dim x = pscore(p), y = nscore(p)
                Dim bt, lt, rt As Double
                Dim lm As Integer = x.Length * rand.NextDouble()
                Dim ln As Integer = y.Length * rand.NextDouble()

                Call ALGLIB.alglib.studentttests.studentttest2(x, ln, y, lm, bt, lt, rt)
                statistics(p) = bt
            Next

            Call tdistribution.AddRange(statistics)

            If tdistribution.Count > 1000 Then
                Exit For
            End If
        Next

        Return tdistribution.ToArray
    End Function

    Private Function Internal_get_pscore(ccs_nodes As PFSNetGraph(), matrix As DataFrameRow()) As Double()()
        Dim ChunkBuffer = (From x In ccs_nodes Select Internal_pscoring(x, matrix)).ToArray
        Return ChunkBuffer
    End Function

    Private Function Internal_pscoring(ccs_nodes As PFSNetGraph, matrix As DataFrameRow()) As Double()
        Dim vertices = (From ccs_node In ccs_nodes.Nodes Select ccs_node.Name).ToArray  'vertices<-get.vertex.attribute(x,name="name")
        Dim ws = matrix.Select(vertices) 'ws<-w1matrix1[vertices,,drop=FALSE]
        Dim v = New List(Of Double) '	v<-c()
        For j As Integer = 0 To ws.First.ExperimentValues.Length - 1
            Dim p = j
            Call v.Add((From d In ws Select ccs_nodes.Node(d.Name).weight * d.ExperimentValues(p)).ToArray.Sum)
        Next
        'v()
        '#apply(ss,2,function(y){
        '#		sum(apply(ss[y,,drop=FALSE],1,sum)/ncol(expr1o))
        '#	})
        Return v.ToArray
    End Function

    Private Function Internal_get_nscore(ccs_nodes As PFSNetGraph(), matrix As DataFrameRow()) As Double()()
        Dim ChunkBuffer = (From node As PFSNetGraph In ccs_nodes Select Internal_nscoring(node, matrix)).ToArray
        Return ChunkBuffer
    End Function

    Private Function Internal_nscoring(ccs_nodes As PFSNetGraph, matrix As DataFrameRow()) As Double()
        Dim vertices = (From ccs_node In ccs_nodes.Nodes Select ccs_node.Name).ToArray  'vertices<-get.vertex.attribute(x,name="name")
        Dim ws = matrix.Select(vertices) 'ws<-w1matrix1[vertices,,drop=FALSE]
        Dim v = New List(Of Double) '	v<-c()
        For j As Integer = 0 To ws.First.ExperimentValues.Length - 1
            Dim p = j
            Call v.Add((From d In ws Select ccs_nodes.Node(d.Name).weight2 * d.ExperimentValues(p)).ToArray.Sum)
        Next
        'v()
        '#apply(ss,2,function(y){
        '#		sum(apply(ss[y,,drop=FALSE],1,sum)/ncol(expr1o))
        '#	})
        Return v.ToArray
    End Function
End Module

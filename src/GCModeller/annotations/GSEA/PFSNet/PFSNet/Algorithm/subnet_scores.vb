#Region "Microsoft.VisualBasic::ac6e98a4dd09c91b1aacea407bb069f1, GCModeller\annotations\GSEA\PFSNet\PFSNet\Algorithm\subnet_scores.vb"

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

    '   Total Lines: 78
    '    Code Lines: 37
    ' Comment Lines: 32
    '   Blank Lines: 9
    '     File Size: 2.82 KB


    ' Module subnet_scores
    ' 
    '     Function: nscoring, pscoring, sapply_nscore, sapply_pscore
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports SMRUCC.genomics.Analysis.HTS.DataFrame
Imports SMRUCC.genomics.Analysis.PFSNet.DataStructure
Imports SMRUCC.genomics.Analysis.PFSNet.R

<HideModuleName> Module subnet_scores

    ''' <summary>
    ''' ```R
    ''' pscore&lt;-sapply(ccs,function(x){
    '''	    vertices&lt;-get.vertex.attribute(x,name="name")
    '''     ws&lt;-w1matrix1[vertices,,drop=FALSE]
    '''     ws[is.na(ws)]&lt;-0
    '''     v&lt;-c()
    '''
    '''     for(i in 1:ncol(ws)){
    '''         v&lt;-c(v,sum(
    '''             (V(x)$weight*ws[,i])
    '''         ))
    '''	    }
    '''	    
    '''     v
    '''     # apply(ss,2,function(y){
    '''     #    sum(apply(ss[y,,drop=FALSE],1,sum)/ncol(expr1o))
    '''	    # })
    '''	})
    '''	```
    ''' </summary>
    ''' <param name="ccs"></param>
    ''' <param name="w1matrix1"></param>
    ''' <returns></returns>
    Public Function sapply_pscore(ccs As PFSNetGraph(), w1matrix1 As DataFrameRow()) As Double()()
        Return (From x In ccs Select pscoring(x, w1matrix1)).ToArray
    End Function

    Private Function pscoring(x As PFSNetGraph, w1matrix1 As DataFrameRow()) As Double()
        Dim vertices = (From ccs_node In x.nodes Select ccs_node.name).ToArray  'vertices<-get.vertex.attribute(x,name="name")
        Dim ws = w1matrix1.Select(vertices) 'ws<-w1matrix1[vertices,,drop=FALSE]
        Dim v = New List(Of Double) '	v<-c()
        Dim ncol = ws(Scan0).samples

        For j As Integer = 0 To ncol - 1
            Dim p = j
            Dim sum = (From d In ws Select x.Node(d.geneID).weight * d.experiments(p)).Sum

            Call v.Add(sum)
        Next
        'v()
        '#apply(ss,2,function(y){
        '#		sum(apply(ss[y,,drop=FALSE],1,sum)/ncol(expr1o))
        '#	})
        Return v.ToArray
    End Function

    Public Function sapply_nscore(ccs As PFSNetGraph(), w1matrix1 As DataFrameRow()) As Double()()
        Return (From node As PFSNetGraph In ccs Select nscoring(node, w1matrix1)).ToArray
    End Function

    Private Function nscoring(x As PFSNetGraph, w1matrix1 As DataFrameRow()) As Double()
        Dim vertices = (From ccs_node In x.nodes Select ccs_node.name).ToArray  'vertices<-get.vertex.attribute(x,name="name")
        Dim ws = w1matrix1.Select(vertices) 'ws<-w1matrix1[vertices,,drop=FALSE]
        Dim v = New List(Of Double) '	v<-c()
        Dim ncol = ws(Scan0).samples

        For j As Integer = 0 To ncol - 1
            Dim p = j
            Dim sum = (From d In ws Select x.Node(d.geneID).weight2 * d.experiments(p)).Sum

            Call v.Add(sum)
        Next
        'v()
        '#apply(ss,2,function(y){
        '#		sum(apply(ss[y,,drop=FALSE],1,sum)/ncol(expr1o))
        '#	})
        Return v.ToArray
    End Function
End Module

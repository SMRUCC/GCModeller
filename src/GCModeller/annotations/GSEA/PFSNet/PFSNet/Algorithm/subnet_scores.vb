#Region "Microsoft.VisualBasic::c721c6fa122e2743b52c0b30c05ba017, annotations\GSEA\PFSNet\PFSNet\Algorithm\subnet_scores.vb"

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
    '    Code Lines: 37 (47.44%)
    ' Comment Lines: 32 (41.03%)
    '    - Xml Docs: 53.12%
    ' 
    '   Blank Lines: 9 (11.54%)
    '     File Size: 2.90 KB


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

    Private Function pscoring(graph As PFSNetGraph, w1matrix1 As DataFrameRow()) As Double()
        Return Score(graph, w1matrix1, usePrimaryWeight:=True)
    End Function

    Public Function sapply_nscore(ccs As PFSNetGraph(), w1matrix1 As DataFrameRow()) As Double()()
        Return (From node As PFSNetGraph In ccs Select nscoring(node, w1matrix1)).ToArray
    End Function

    Private Function nscoring(graph As PFSNetGraph, w1matrix1 As DataFrameRow()) As Double()
        Return Score(graph, w1matrix1, usePrimaryWeight:=False)
    End Function

    Private Function Score(graph As PFSNetGraph,
                           matrix As DataFrameRow(),
                           usePrimaryWeight As Boolean) As Double()

        If matrix Is Nothing OrElse matrix.Length = 0 Then
            Return Array.Empty(Of Double)()
        End If

        Dim sampleCount As Integer = matrix(0).samples
        Dim rows As New System.Collections.Generic.Dictionary(Of String, DataFrameRow)(StringComparer.Ordinal)

        For Each row As DataFrameRow In matrix
            If row IsNot Nothing AndAlso Not rows.ContainsKey(row.geneID) Then
                rows.Add(row.geneID, row)
            End If
        Next

        Dim scores(sampleCount - 1) As Double

        For sampleIndex As Integer = 0 To sampleCount - 1
            Dim totalScore As Double = 0

            For Each node As PFSNetGraphNode In graph.nodes
                Dim row As DataFrameRow = Nothing

                If rows.TryGetValue(node.name, row) AndAlso sampleIndex < row.samples Then
                    Dim weight As Double = If(usePrimaryWeight, node.weight, node.weight2)
                    Dim expression As Double = row.experiments(sampleIndex)

                    If Double.IsFinite(weight) AndAlso Double.IsFinite(expression) Then
                        totalScore += weight * expression
                    End If
                End If
            Next

            scores(sampleIndex) = totalScore
        Next

        Return scores
    End Function
End Module

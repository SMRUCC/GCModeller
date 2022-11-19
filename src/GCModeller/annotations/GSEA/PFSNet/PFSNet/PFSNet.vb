#Region "Microsoft.VisualBasic::b699f46af5f007d402eba06d5286bbd9, GCModeller\annotations\GSEA\PFSNet\PFSNet\PFSNet.vb"

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

    '   Total Lines: 155
    '    Code Lines: 93
    ' Comment Lines: 35
    '   Blank Lines: 27
    '     File Size: 6.29 KB


    ' Module PFSNetAlgorithm
    ' 
    '     Function: computegenelist, (+2 Overloads) pfsnet
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ApplicationServices.Terminal.STDIO
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Language.Vectorization
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Analysis.HTS.DataFrame
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
<HideModuleName> Public Module PFSNetAlgorithm

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
        Dim geneIDs As New StringVector(From gene In w Select gene.geneID)
        Dim maskQuery = From row As DataFrameRow
                        In w
                        Let x As Double() = row.experiments
                        Let sumX = x.Where(Function(y) Not y.IsNaNImaginary).Sum
                        Let countX = x.Where(Function(y) Not y.IsNaNImaginary).Count
                        Select (sumX / countX) >= beta
        Dim list_mask As Boolean() = maskQuery.ToArray

        Return geneIDs(list_mask)
    End Function

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
    <ExportAPI("PfsNet.Evaluate")>
    Public Function pfsnet(file1 As String,
                           file2 As String,
                           file3 As String,
                           Optional b As Double = 0.5,
                           Optional t1 As Double = 0.95,
                           Optional t2 As Double = 0.85,
                           Optional n As Double = 1000) As PFSNetResultOut

        cat("reading data files")
        Dim ggi As GraphEdge() = GraphEdge.LoadData(file3)
        cat(".")
        Dim expr1o As DataFrameRow() = Matrix.LoadData(file1).expression
        cat(".")
        Dim expr2o As DataFrameRow() = Matrix.LoadData(file2).expression
        cat(".")
        cat("\t[DONE]\n")

        Return pfsnet(expr1o, expr2o, ggi, b, t1, t2, n)
    End Function

    <ExportAPI("PfsNET.Evaluate")>
    Public Function pfsnet(expr1o As DataFrameRow(),
                           expr2o As DataFrameRow(),
                           ggi As GraphEdge(),
                           Optional b As Double = 0.5,
                           Optional t1 As Double = 0.95,
                           Optional t2 As Double = 0.85,
                           Optional n As Integer = 1000) As PFSNetResultOut

        Dim proc As Stopwatch = Stopwatch.StartNew

        cat("computing fuzzy weights")

        Dim w1matrix1 = computew1(expr1o, theta1:=t1, theta2:=t2)
        Dim w1matrix2 = computew1(expr2o, theta1:=t1, theta2:=t2)

        cat(".")

        Dim genelist1 As Index(Of String) = computegenelist(w1matrix1, beta:=b)
        Dim genelist2 As Index(Of String) = computegenelist(w1matrix2, beta:=b)

        cat("\t[DONE]\n")
        cat("computing subnetworks")

        Dim ggi_mask = ggi.ggi_mask(genelist1)
        Dim masked_ggi = ggi.getMasked_ggi(ggi_mask)

        cat(".")

        Dim ccs = masked_ggi.ccs(w1matrix1, w1matrix2).ToArray

        ggi_mask = ggi.ggi_mask(genelist2)
        masked_ggi = ggi.getMasked_ggi(ggi_mask)

        Dim ccs2 = masked_ggi.ccs(w1matrix1, w1matrix2).ToArray

        cat(".")
        cat("\t[DONE]\n")
        cat("computing subnetwork scores")

        Dim pscore = sapply_pscore(ccs, w1matrix1)
        cat(".")
        Dim pscore2 = sapply_pscore(ccs2, w1matrix2)
        cat(".")

        Dim nscore = sapply_nscore(ccs, w1matrix1)
        Dim nscore2 = sapply_nscore(ccs2, w1matrix2)

        cat(".")
        cat(".\t[DONE]\n")
        cat("computing permuation tests ")

        Dim tdist = estimatetdist(expr1o, expr2o, ggi, b, t1, t2, n)
        Dim tdist2 = estimatetdist(expr2o, expr1o, ggi, b, t1, t2, n)

        ccs = ccs.doStatics(pscore, nscore, tdist)
        ccs2 = ccs2.doStatics(pscore2, nscore2, tdist2)

        cat("\t[DONE]\n")
        cat("total time elapsed: ", proc.ElapsedMilliseconds / 1000, " seconds\n")

        Dim class1 = (From item In ccs Where item.masked Select item).ToArray
        Dim class2 = (From item In ccs2 Where item.masked Select item).ToArray

        Return New PFSNetResultOut With {
            .phenotype1 = class1,
            .phenotype2 = class2
        }
    End Function
End Module

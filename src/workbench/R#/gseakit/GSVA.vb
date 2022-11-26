#Region "Microsoft.VisualBasic::7e67d318d7f156fb61b5c4f1dc973a3f, R#\gseakit\GSVA.vb"

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

    '   Total Lines: 163
    '    Code Lines: 105
    ' Comment Lines: 45
    '   Blank Lines: 13
    '     File Size: 6.55 KB


    ' Module GSVA
    ' 
    '     Function: createGene, diff, gsva, matrixToDiff
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Math.Statistics.Hypothesis
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports SMRUCC.genomics.Analysis.HTS.DataFrame
Imports SMRUCC.genomics.Analysis.HTS.GSEA
Imports SMRUCC.genomics.Analysis.HTS.GSVA
Imports SMRUCC.genomics.GCModeller.Workbench.ExperimentDesigner
Imports SMRUCC.genomics.Visualize
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop
Imports HTSMatrix = SMRUCC.genomics.Analysis.HTS.DataFrame.Matrix
Imports REnv = SMRUCC.Rsharp.Runtime

''' <summary>
''' Gene Set Variation Analysis for microarray and RNA-seq data
''' </summary>
<Package("GSVA")>
Module GSVA

    ''' <summary>
    ''' Gene Set Variation Analysis for microarray and RNA-seq data
    ''' 
    ''' Gene Set Variation Analysis (GSVA) is a non-parametric, unsupervised 
    ''' method for estimating variation of gene set enrichment through the
    ''' samples of a expression data set. GSVA performs a change in coordinate
    ''' systems, transforming the data from a gene by sample matrix to a gene-set
    ''' by sample matrix, thereby allowing the evaluation of pathway enrichment 
    ''' for each sample. This new matrix of GSVA enrichment scores facilitates
    ''' applying standard analytical methods like functional enrichment, 
    ''' survival analysis, clustering, CNV-pathway analysis or cross-tissue 
    ''' pathway analysis, in a pathway-centric manner.
    ''' 
    ''' main function of the package which estimates activity
    ''' scores For Each given gene-Set
    ''' </summary>
    ''' <param name="expr">A raw gene expression data matrix object</param>
    ''' <param name="geneSet">
    ''' A gsea enrichment background model
    ''' </param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' Hänzelmann S., Castelo R. and Guinney J. GSVA: gene set variation analysis
    ''' for microarray and RNA-Seq data. BMC Bioinformatics, 14:7, 2013.
    ''' </remarks>
    <ExportAPI("gsva")>
    <RApiReturn(GetType(HTSMatrix))>
    Public Function gsva(expr As Object, geneSet As Object, Optional env As Environment = Nothing) As Object
        Dim mat As HTSMatrix
        Dim background As Background

        If TypeOf expr Is dataframe Then
            mat = New HTSMatrix With {
                .sampleID = DirectCast(expr, dataframe).colnames,
                .expression = DirectCast(expr, dataframe) _
                    .forEachRow _
                    .Select(Function(r)
                                Return New DataFrameRow With {
                                    .geneID = r.name,
                                    .experiments = REnv.asVector(Of Double)(r.ToArray)
                                }
                            End Function) _
                    .ToArray
            }
        Else
            mat = expr
        End If

        If TypeOf geneSet Is list Then
            background = New Background With {
                .clusters = DirectCast(geneSet, list).slots _
                    .Select(Function(c)
                                Return New Cluster With {
                                    .ID = c.Key,
                                    .description = c.Key,
                                    .names = c.Key,
                                    .members = DirectCast(REnv.asVector(Of String)(c.Value), String()) _
                                        .Select(AddressOf createGene) _
                                        .ToArray
                                }
                            End Function) _
                    .ToArray
            }
        Else
            background = geneSet
        End If

        Return mat.gsva(background)
    End Function

    ''' <summary>
    ''' different analysis of the gsva result
    ''' </summary>
    ''' <param name="gsva">the gsva analysis result</param>
    ''' <param name="compares">
    ''' the analysis comparision
    ''' </param>
    ''' <returns></returns>
    <ExportAPI("diff")>
    Public Function diff(gsva As HTSMatrix, compares As DataAnalysis) As GSVADiff()
        If compares.size <> 2 Then
            Throw New InvalidProgramException
        End If

        Dim i1 As Integer() = gsva.IndexOf(compares.experiment)
        Dim i2 As Integer() = gsva.IndexOf(compares.control)

        Return gsva.expression _
            .Select(Function(expr)
                        Dim test As TwoSampleResult = t.Test(expr(i1), expr(i2))

                        Return New GSVADiff With {
                            .pathName = expr.geneID,
                            .t = test.TestValue,
                            .pvalue = test.Pvalue
                        }
                    End Function) _
            .ToArray
    End Function

    ''' <summary>
    ''' convert to diff data from dataframe
    ''' </summary>
    ''' <param name="diff"></param>
    ''' <param name="pathId"></param>
    ''' <param name="t"></param>
    ''' <param name="pvalue"></param>
    ''' <returns></returns>
    <ExportAPI("matrix_to_diff")>
    Public Function matrixToDiff(diff As dataframe,
                                 Optional pathId As String = "pathNames",
                                 Optional t As String = "t",
                                 Optional pvalue As String = "pvalue") As GSVADiff()

        Dim pathNames As String() = REnv.asVector(Of String)(diff(pathId))
        Dim testVal As Double() = REnv.asVector(Of Double)(diff(t))
        Dim pVal As Double() = REnv.asVector(Of Double)(diff(pvalue))

        Return pathNames _
            .Select(Function(ref, i)
                        Return New GSVADiff With {
                            .pathName = ref,
                            .pvalue = pVal(i),
                            .t = testVal(i)
                        }
                    End Function) _
            .ToArray
    End Function

    Private Function createGene(name As String) As BackgroundGene
        Return New BackgroundGene With {
            .accessionID = name,
            .name = name,
            .[alias] = {name},
            .locus_tag = New NamedValue With {
                .name = name,
                .text = name
            },
            .term_id = {name}
        }
    End Function
End Module

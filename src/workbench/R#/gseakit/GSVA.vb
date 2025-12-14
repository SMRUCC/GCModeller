#Region "Microsoft.VisualBasic::3dd5b4f8c4fe24ecda79ff47b948143d, R#\gseakit\GSVA.vb"

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

'   Total Lines: 164
'    Code Lines: 106 (64.63%)
' Comment Lines: 45 (27.44%)
'    - Xml Docs: 95.56%
' 
'   Blank Lines: 13 (7.93%)
'     File Size: 6.62 KB


' Module GSVA
' 
'     Function: createGene, diff, gsva, matrixToDiff
' 
' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Math.Statistics.Hypothesis
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Analysis.HTS.DataFrame
Imports SMRUCC.genomics.Analysis.HTS.GSEA
Imports SMRUCC.genomics.Analysis.HTS.GSVA
Imports SMRUCC.genomics.GCModeller.Workbench.ExperimentDesigner
Imports SMRUCC.genomics.Visualize
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop
Imports SMRUCC.Rsharp.Runtime.Vectorization
Imports HTSMatrix = SMRUCC.genomics.Analysis.HTS.DataFrame.Matrix
Imports RInternal = SMRUCC.Rsharp.Runtime.Internal

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
    ''' A gsea enrichment <see cref="Background"/> model
    ''' </param>
    ''' <param name="name_suffix">
    ''' append the pathway name to the map id as suffix?
    ''' </param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' Hänzelmann S., Castelo R. and Guinney J. GSVA: gene set variation analysis
    ''' for microarray and RNA-Seq data. BMC Bioinformatics, 14:7, 2013.
    ''' </remarks>
    <ExportAPI("gsva")>
    <RApiReturn(GetType(HTSMatrix))>
    Public Function gsva(expr As Object, geneSet As Object, Optional name_suffix As Boolean = False, Optional env As Environment = Nothing) As Object
        Dim mat As HTSMatrix
        Dim background As Background

        If geneSet Is Nothing Then
            Return RInternal.debug.stop("the required enrichment background model should not be nothing!", env)
        End If

        If TypeOf expr Is dataframe Then
            mat = New HTSMatrix With {
                .sampleID = DirectCast(expr, dataframe).colnames,
                .expression = DirectCast(expr, dataframe) _
                    .forEachRow _
                    .Select(Function(r)
                                Return New DataFrameRow With {
                                    .geneID = r.name,
                                    .experiments = CLRVector.asNumeric(r.ToArray)
                                }
                            End Function) _
                    .ToArray
            }
        Else
            mat = expr
        End If

        If TypeOf geneSet Is list Then
            background = backgroundFromList(DirectCast(geneSet, list), env)
        ElseIf TypeOf geneSet Is Background Then
            background = geneSet
        Else
            Return Message.InCompatibleType(GetType(Background), geneSet.GetType, env)
        End If

        Dim gsva_result As HTSMatrix = mat.gsva(background)

        If name_suffix Then
            For Each pathway As DataFrameRow In gsva_result.expression
                pathway.geneID = $"{pathway.geneID} - {background(pathway.geneID).names}"
            Next
        End If

        Return gsva_result
    End Function

    ''' <summary>
    ''' different analysis of the gsva result
    ''' </summary>
    ''' <param name="gsva">the gsva analysis result</param>
    ''' <param name="compares">
    ''' the <see cref="DataAnalysis"/> comparision collection
    ''' </param>
    ''' <returns></returns>
    <ExportAPI("diff")>
    <RApiReturn(GetType(GSVADiff))>
    Public Function diff(gsva As HTSMatrix, compares As DataAnalysis) As Object
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

        Dim pathNames As String() = CLRVector.asCharacter(diff(pathId))
        Dim testVal As Double() = CLRVector.asNumeric(diff(t))
        Dim pVal As Double() = CLRVector.asNumeric(diff(pvalue))

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
End Module

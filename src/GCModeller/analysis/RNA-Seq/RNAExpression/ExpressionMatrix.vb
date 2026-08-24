#Region "Microsoft.VisualBasic::040021d1db268f4feb03fc8d6731aab1, analysis\RNA-Seq\RNAExpression\ExpressionMatrix.vb"

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

    '   Total Lines: 77
    '    Code Lines: 61 (79.22%)
    ' Comment Lines: 8 (10.39%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 8 (10.39%)
    '     File Size: 3.39 KB


    ' Module ExpressionMatrix
    ' 
    '     Function: FeatureCountMatrix, FPKMExpression, GetGeneExpression, TPMExpression
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.Framework.IO
Imports Microsoft.VisualBasic.Scripting.Runtime
Imports SMRUCC.genomics.Analysis.HTS.DataFrame
Imports SMRUCC.genomics.SequenceModel.GeneQuantification
Imports SMRUCC.genomics.SequenceModel.SAM.featureCount

Public Module ExpressionMatrix

    <Extension>
    Public Function TPMExpression(dataset As IEnumerable(Of GeneSampleSet)) As Matrix
        Dim sampleData = dataset.ToArray
        Dim sample_ids As String() = sampleData.SelectMany(Function(gene) gene.FPKM.Keys).Distinct().ToArray()
        Dim matrix As New Matrix With {
            .sampleID = sample_ids,
            .tag = "TPM",
            .expression = sampleData _
                .GetGeneExpression(isFpkm:=False, sample_ids:=sample_ids) _
                .ToArray
        }

        Return matrix
    End Function

    <Extension>
    Public Function FPKMExpression(dataset As IEnumerable(Of GeneSampleSet)) As Matrix
        Dim sampleData = dataset.ToArray
        Dim sample_ids As String() = sampleData.SelectMany(Function(gene) gene.FPKM.Keys).Distinct().ToArray()
        Dim matrix As New Matrix With {
            .sampleID = sample_ids,
            .tag = "FPKM",
            .expression = sampleData _
                .GetGeneExpression(isFpkm:=True, sample_ids:=sample_ids) _
                .ToArray
        }

        Return matrix
    End Function

    <Extension>
    Private Iterator Function GetGeneExpression(dataset As IEnumerable(Of GeneSampleSet), isFpkm As Boolean, sample_ids As String()) As IEnumerable(Of DataFrameRow)
        For Each gene As GeneSampleSet In dataset
            Yield New DataFrameRow With {
                .geneID = gene.GeneID,
                .experiments = gene(sample_ids, isFpkm)
            }
        Next
    End Function

    ''' <summary>
    ''' Export the raw feature count matrix data for make data normalizatiomn via DeSeq2 or edgeR. 
    ''' The matrix will be in the format of geneID as row names and sampleID as column names, and the values are the raw counts. 
    ''' This function is useful for users who want to perform their own normalization and differential 
    ''' expression analysis using R packages like DESeq2 or edgeR.
    ''' </summary>
    ''' <param name="featureCounts"></param>
    ''' <returns></returns>
    <Extension>
    Public Function FeatureCountMatrix(featureCounts As IEnumerable(Of featureCounts)) As Matrix
        Dim sampleData = featureCounts.ToArray
        Dim sample_ids As String() = sampleData.SelectMany(Function(gene) gene.SampleCounts.Keys).Distinct().ToArray()
        Dim geneCounts As DataFrameRow() = (From gene As featureCounts
                                            In sampleData
                                            Let counts = gene(sample_ids).AsDouble
                                            Select New DataFrameRow With {
                                                .geneID = gene.Geneid,
                                                .experiments = counts
                                           }).ToArray
        Dim matrix As New Matrix With {
            .sampleID = sample_ids,
            .tag = "RawCounts",
            .expression = geneCounts
        }

        Return matrix
    End Function
End Module


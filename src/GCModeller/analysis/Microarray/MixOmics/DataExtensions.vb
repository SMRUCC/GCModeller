#Region "Microsoft.VisualBasic::063a91fe7d179b79e248ccc76cd4c18e, analysis\Microarray\MixOmics\DataExtensions.vb"

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

    '   Total Lines: 48
    '    Code Lines: 40 (83.33%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 8 (16.67%)
    '     File Size: 1.76 KB


    ' Module DataExtensions
    ' 
    '     Function: CreateDataView, CreateExpressionMatrix
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.MachineLearning.TensorFlow
Imports SMRUCC.genomics.Analysis.HTS.DataFrame
Imports SMRUCC.genomics.Analysis.Microarray.MultiOmics.MOFA

Public Module DataExtensions

    <Extension>
    Public Function CreateDataView(mat As Matrix, Optional name As String = Nothing) As DataView
        Dim nsamples As Integer = mat.sample_count
        Dim ngenes As Integer = mat.size
        Dim data As New Tensor(nsamples, ngenes)

        For i As Integer = 0 To nsamples - 1
            For d As Integer = 0 To ngenes - 1
                data(i, d) = mat(d)(i)
            Next
        Next

        Return New DataView(If(name, mat.tag), data, mat.sampleID, mat.rownames)
    End Function

    <Extension>
    Public Function CreateExpressionMatrix(t As Tensor, sampleIDs As String(), featureIDs As String(), Optional ref_tag As String = Nothing) As Matrix
        Dim nsamples As Integer = sampleIDs.Length
        Dim ngenes As Integer = featureIDs.Length
        Dim data As DataFrameRow() = New DataFrameRow(ngenes - 1) {}

        For i As Integer = 0 To ngenes - 1
            data(i) = New DataFrameRow With {
                .geneID = featureIDs(i),
                .experiments = New Double(nsamples - 1) {}
            }
        Next

        For i As Integer = 0 To nsamples - 1
            For d As Integer = 0 To ngenes - 1
                data(d).experiments(i) = t(i, d)
            Next
        Next

        Return New Matrix With {
            .expression = data,
            .sampleID = sampleIDs,
            .tag = If(ref_tag.StringEmpty, "MOFA_reconstruct", $"MOFA_reconstruct({ref_tag})")
        }
    End Function
End Module


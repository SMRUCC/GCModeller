#Region "Microsoft.VisualBasic::a379b83e75574e54f62b5f62de152c3d, analysis\HTS_matrix\Math\TPM.vb"

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

    '   Total Lines: 63
    '    Code Lines: 48 (76.19%)
    ' Comment Lines: 8 (12.70%)
    '    - Xml Docs: 87.50%
    ' 
    '   Blank Lines: 7 (11.11%)
    '     File Size: 2.35 KB


    ' Module TPM
    ' 
    '     Function: LibrarySize, (+3 Overloads) Normalize
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Math.Statistics.Linq
Imports std_vec = Microsoft.VisualBasic.Math.LinearAlgebra.Vector

Public Module TPM

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="countData"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' normalized scaled via the median of the library size
    ''' </remarks>
    <Extension>
    Public Function Normalize(countData As Matrix) As Matrix
        Dim libSizes As Double() = countData.LibrarySize.ToArray
        Dim scale As Double = libSizes.Median
        Return countData.Normalize(libSizes, scale)
    End Function

    <Extension>
    Private Function Normalize(countData As Matrix, librarySize As Double(), factor As Double) As Matrix
        Dim samples = countData.sampleID _
            .Select(Function(ref, i)
                        Dim v As std_vec = countData.sample(ref)
                        Dim total = librarySize(i)
                        Dim col As New NamedValue(Of std_vec)(ref, factor * v / total)

                        Return col
                    End Function) _
            .ToArray
        Dim norm As New Matrix With {
           .sampleID = countData.sampleID,
           .tag = $"totalSumNorm({countData.tag})",
           .expression = countData.expression _
               .Select(Function(gene, i)
                           Return New DataFrameRow With {
                               .geneID = gene.geneID,
                               .experiments = samples _
                                   .Select(Function(v) v.Value(i)) _
                                   .ToArray
                           }
                       End Function) _
               .ToArray
        }

        Return norm
    End Function

    <Extension>
    Public Function Normalize(countData As Matrix, factor As Double) As Matrix
        Return countData.Normalize(countData.LibrarySize.ToArray, factor)
    End Function

    <Extension>
    Private Iterator Function LibrarySize(countData As Matrix) As IEnumerable(Of Double)
        For Each id As String In countData.sampleID
            Yield countData.GetSampleArray(id).Sum
        Next
    End Function
End Module


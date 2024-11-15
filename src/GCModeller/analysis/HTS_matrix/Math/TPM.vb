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

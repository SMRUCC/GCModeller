Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.ComponentModel.DataStructures
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic
Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.genomics.SequenceModel.NucleotideModels.NucleicAcidStaticsProperty

''' <summary>
''' ``GC%``异常点分析
''' </summary>
Public Module GCOutlier

    <Extension>
    Public Iterator Function Outlier(mla As IEnumerable(Of FastaToken), quantiles As Double(),
                                     Optional winsize As Integer = 250,
                                     Optional steps As Integer = 50,
                                     Optional slideSize As Integer = 5,
                                     Optional method As NtProperty = Nothing) As IEnumerable(Of NamedValue(Of NucleotideLocation))

        Dim data As NamedValue(Of Double())() = GCData(mla, winsize, steps, method)
        Dim iSeq As Integer() = data(Scan0).x.Sequence.ToArray
        Dim seq As lociX()() = New lociX(slideSize - 1)() {}

        For i As Integer = 0 To slideSize - 1
            Dim a As lociX() = New lociX(iSeq.Length - 1) {}

            For Each x In data.SeqIterator
                a(x.i) = New lociX With {
                    .Title = x.obj.Name
                }
            Next

            seq(i) = a
        Next

        For Each index As SlideWindowHandle(Of Integer) In iSeq.SlideWindows(slideSize)
            Dim tmp As New List(Of lociX)

        Next
    End Function

    Public Class lociX
        Public Property Title As String
        Public Property loci As Integer
        Public Property value As Double
    End Class
End Module

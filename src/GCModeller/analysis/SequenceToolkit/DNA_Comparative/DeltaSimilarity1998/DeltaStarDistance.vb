Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Math.SIMD
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.genomics.SequenceModel.NucleotideModels

Namespace DeltaSimilarity1998

    ''' <summary>
    ''' MEASURES OF DIFFERENCES WITHIN AND BETWEEN GENOMES.
    ''' 
    ''' A measure of difference between two sequences f and g (from different organisms 
    ''' or from different regions of the same genome) is the average absolute dinucleotide 
    ''' relative abundance difference calculated as
    ''' 
    ''' ```
    '''    delta*(f, g) = (1/16) * SUM(XY) |rho*XY(f) - rho*XY(g)|
    ''' ```
    ''' 
    ''' where the sum extends over all 16 dinucleotides and ``rho*`` denotes the 
    ''' double-strand symmetrized odds ratio (see <see cref="NucleicAcid.RelativeAbundance"/>).
    ''' 
    ''' (compatible rename of the legacy ``DifferenceMeasurement.Sigma`` API, 
    ''' ``Sigma`` -> ``DeltaStar`` follows the paper's ``delta*`` symbol)
    ''' </summary>
    <Package("DeltaStar.Distance",
             Description:="MEASURES OF DIFFERENCES WITHIN AND BETWEEN GENOMES. (Karlin 1998 delta* distance)",
             Cites:="Karlin, S., et al. (1998). ""Comparative DNA analysis across diverse genomes."" Annu Rev Genet 32: 185-225.",
             Publisher:="amethyst.asuka@gcmodeller.org")>
    Public Module DeltaStarDistance

        ''' <summary>
        ''' The SIMD vectorized mean-absolute-difference is only profitable when the 
        ''' signature vector is large enough to amortize the allocation overhead. 
        ''' The 16-dim dinucleotide signature is evaluated on the scalar path.
        ''' </summary>
        Friend Const SimdVectorSizeThreshold As Integer = 64

        ''' <summary>
        ''' ``delta*(f, g) = (1/16) SUM |rho*XY(f) - rho*XY(g)|``
        ''' </summary>
        ''' <param name="f"></param>
        ''' <param name="g"></param>
        ''' <returns></returns>
        <ExportAPI("DeltaStar")>
        Public Function DeltaStar(f As NucleicAcid, g As NucleicAcid) As Double
            Dim sf As Double() = f.SignatureVector()
            Dim sg As Double() = g.SignatureVector()
            Dim sum As Double = 0

            ' 16 iterations scalar loop: faster than SIMD due to no allocation
            For i As Integer = 0 To 15
                sum += Math.Abs(sf(i) - sg(i))
            Next

            Return sum / 16
        End Function

        <ExportAPI("DeltaStar")>
        <Extension>
        Public Function DeltaStar(f As FastaSeq, g As FastaSeq) As Double
            Return DeltaStar(New NucleicAcid(f), New NucleicAcid(g))
        End Function

        <ExportAPI("DeltaStar")>
        Public Function DeltaStar(f As String, g As String) As Double
            Return DeltaStar(New NucleicAcid(f), New NucleicAcid(g))
        End Function

        ''' <summary>
        ''' Mean absolute difference of two arbitrary signature vectors of the same size 
        ''' (used for the codon signature vectors and the codon usage profiles). 
        ''' The SIMD vectorized path (<see cref="SimdEngine"/>) is applied when 
        ''' ``SIMDEnvironment.IsEnabled`` and the vector size is large enough, 
        ''' otherwise falls back to the scalar path.
        ''' </summary>
        ''' <param name="a"></param>
        ''' <param name="b"></param>
        ''' <returns></returns>
        Friend Function MeanAbsoluteDifference(a As Double(), b As Double()) As Double
            Dim n As Integer = a.Length

            If n >= SimdVectorSizeThreshold AndAlso SIMDEnvironment.IsEnabled Then
                ' SIMD path: AVX2/AdvSimd vectorized |a - b| summation
                Dim diff As Double() = SimdEngine.Subtract(a, b)
                Return SimdReduce.L1Norm(diff) / n
            End If

            ' scalar fallback
            Dim sum As Double = 0

            For i As Integer = 0 To n - 1
                sum += Math.Abs(a(i) - b(i))
            Next

            Return sum / n
        End Function

        ''' <summary>
        ''' Levels of delta-differences for some reference examples 
        ''' (all values multiplied by 1000), see <see cref="DeltaStarLevels"/>.
        ''' </summary>
        ''' <param name="deltaStar">value from the calculation of function <see cref="DeltaStarDistance.DeltaStar"/></param>
        ''' <returns></returns>
        <ExportAPI("DeltaStar.Level")>
        Public Function DeltaStarLevel(deltaStar As Double) As DeltaStarLevels
            Dim value As Double = deltaStar * 1000

            If value <= 50 Then
                Return DeltaStarLevels.Close
            ElseIf value <= 85 Then
                Return DeltaStarLevels.ModeratelySimilar
            ElseIf value <= 120 Then
                Return DeltaStarLevels.WeaklySimilar
            ElseIf value <= 145 Then
                Return DeltaStarLevels.DistantlySimilar
            ElseIf value <= 180 Then
                Return DeltaStarLevels.Distant
            Else
                Return DeltaStarLevels.VeryDistant
            End If
        End Function
    End Module
End Namespace

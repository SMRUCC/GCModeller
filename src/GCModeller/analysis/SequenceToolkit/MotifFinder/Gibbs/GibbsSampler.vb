#Region "Microsoft.VisualBasic::e9f1fa682b82c6156e328d6e210cc674, analysis\SequenceToolkit\MotifFinder\Gibbs\GibbsSampler.vb"

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

    '   Total Lines: 293
    '    Code Lines: 180 (61.43%)
    ' Comment Lines: 74 (25.26%)
    '    - Xml Docs: 85.14%
    ' 
    '   Blank Lines: 39 (13.31%)
    '     File Size: 13.85 KB


    ' Class GibbsSampler
    ' 
    '     Properties: SequenceCount, Sequences
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: calculateMotifProbability, calculateP, find, getMotifStrings, getRandomSites
    '               gibbsSample, informationContent, predictiveUpdateStep, samplingStep, smoothProbabilities
    '               weightedChooseIndex
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports Microsoft.VisualBasic.Math.GibbsSampling
Imports Microsoft.VisualBasic.My.JavaScript
Imports SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Motif.Matrix
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports randf = Microsoft.VisualBasic.Math.RandomExtensions

''' <summary>
''' 
''' </summary>
''' <remarks>
''' https://github.com/trueb2/cs466proj
''' </remarks>
Public Class GibbsSampler

    Shared ReadOnly LOG_2 As Double = Math.Log(2)

    Public Overridable ReadOnly Property Sequences As IList(Of String)
        Get
            Return m_sequences
        End Get
    End Property

    Friend motifLength As Integer
    Friend sequenceLength As Integer
    Friend m_sequenceCount As Integer

    Dim m_sequences As FastaSeq()

    ''' <returns> the size of the list sequences </returns>
    Public Overridable ReadOnly Property SequenceCount As Integer
        Get
            Return m_sequenceCount
        End Get
    End Property

    Sub New(fastaFile As IEnumerable(Of FastaSeq), Optional motifLength As Integer = 8)
        m_sequences = fastaFile.ToArray
        Me.motifLength = motifLength
        sequenceLength = m_sequences.Select(Function(a) a.Length).Min
        m_sequenceCount = m_sequences.Length
    End Sub

    ''' <summary>
    ''' Runs numSamples gibbsSamples to find a prediction on the sites
    ''' and motifs with the highest information content in the sequences </summary>
    ''' <param name="maxIterations">maximum number of times to iterate in a Gibbs Sample </param>
    Public Function find(Optional maxIterations As Integer = 1000) As MSAMotif
        Dim numSamples As Integer = SequenceCount

        Console.WriteLine("============= Input Sequences =============")
        ' sequences.ForEach(System.out.println);
        Console.WriteLine("============= Result of Gibbs Sampling Algorithm in each iteration =============")
        Dim predictedSites As IList(Of Integer) = New List(Of Integer)()
        Dim predictedMotifs As IList(Of String) = New List(Of String)()
        Dim sequences = Me.m_sequences.Select(Function(fa) fa.SequenceData).ToArray
        Dim maxInformationContent = New Double() {Double.NegativeInfinity}

        Call Enumerable.Range(0, numSamples) _
            .AsParallel() _
            .ForEach(Sub(j, i)
                         SyncLock maxInformationContent
                             If maxInformationContent(0) / motifLength = 2.0 Then
                                 Return
                             End If
                         End SyncLock

                         Dim sites As IList(Of Integer) = gibbsSample(maxIterations, New List(Of String)(sequences))
                         Dim motifs = getMotifStrings(sequences, sites)
                         Dim informationContent = Me.informationContent(motifs)
                         Dim newMax As Boolean
                         SyncLock maxInformationContent
                             newMax = informationContent >= maxInformationContent(0)
                         End SyncLock
                         If newMax Then
                             Dim s As String = sites.Select(Function(k) k.ToString).JoinBy(" ")

                             SyncLock maxInformationContent
                                 maxInformationContent(0) = informationContent
                             End SyncLock
                             SyncLock predictedSites
                                 predictedSites.Clear()
                                 CType(predictedSites, List(Of Integer)).AddRange(sites)
                             End SyncLock
                             SyncLock predictedMotifs
                                 predictedMotifs.Clear()
                                 CType(predictedMotifs, List(Of String)).AddRange(motifs)
                             End SyncLock

                             Console.WriteLine(informationContent.ToString() & " :: " & s)
                         End If
                     End Sub)

        Dim motifMatrix As WeightMatrix = New SequenceMatrix(predictedMotifs)
        Dim icpc As Double = maxInformationContent(0) / motifLength
        Console.WriteLine("======== Maximum Information Content :: " & icpc & " =========" & vbLf)
        Dim p As Double() = New Double(predictedMotifs.Count - 1) {}
        Dim q As Double() = New Double(predictedMotifs.Count - 1) {}
        Dim eval As New Gibbs(sequences, motifLength)

        For i As Integer = 0 To predictedMotifs.Count - 1
            Dim pq = eval.PQ(i)

            p(i) = pq.p.Average
            q(i) = pq.q.Average
        Next

        Return New MSAMotif With {
            .cost = icpc,
            .MSA = predictedMotifs.ToArray,
            .names = m_sequences.Select(Function(fa) fa.Title).ToArray,
            .start = predictedSites.ToArray,
            .countMatrix = motifMatrix.countsMatrix,
            .rowSum = motifMatrix.rowSum,
            .p = p,
            .q = q
        }
    End Function

    Private Function informationContent(motifs As IList(Of String)) As Double
        Dim sm As New SequenceMatrix(motifs)
        Dim sum As Double = 0
        Dim d As Double = 0

        For i As Integer = 0 To motifLength - 1
            For j As Integer = 0 To 3
                d = sm.probability(i, j) * (Math.Log(sm.probability(i, j) * 4) / LOG_2)

                If Not d.IsNaNImaginary Then
                    sum += d
                End If
            Next
        Next

        Return sum
    End Function

    ''' <summary>
    ''' Implements the Gibbs Sampling algorithm found in the lawrence93.pdf </summary>
    ''' <param name="maxIterations">, maximum number of iterations sampling may take </param>
    ''' <returns> Sets of int predicting the position motifs located in each sequence </returns>
    Private Function gibbsSample(maxIterations As Integer, S As IList(Of String)) As IList(Of Integer)
        Dim A = getRandomSites().ToList

        For i As Integer = 0 To maxIterations
            ' Choose the next sequence
            Dim idx = randf.Next(m_sequenceCount)
            Dim z = S(idx)

            ' Remove the sequence from the sequences and sites
            S.RemoveAt(idx)
            A.RemoveAt(idx)

            ' Run the predictive step on z
            Dim q_ij = predictiveUpdateStep(S, A)
            Dim P = calculateP(S)

            ' Run the sampling step on q_ij
            Dim a_z = samplingStep(q_ij, z, P)

            ' Add z back into the set of sequences and sites
            S.Insert(idx, z)
            A.Insert(idx, a_z)
        Next

        Return A
    End Function

    ''' <summary>
    ''' Calculates the background probabilities for each base </summary>
    ''' <param name="S">, sequenceCount sequences </param>
    ''' <returns> List of Double length 4 </returns>
    Private Function calculateP(S As IList(Of String)) As IList(Of Double)
        Dim P = New Double() {0, 0, 0, 0}

        For i As Integer = 0 To m_sequenceCount - 1
            Call Enumerable.Range(0, sequenceLength) _
                .ForEach(Sub(jj, ii)
                             P(Utils.indexOfBase(S(ii)(jj))) += 1
                         End Sub)
        Next

        Dim sum As Double = P.Sum()

        Return P.Select(Function(d) d / sum).ToList()
    End Function

    ''' <summary>
    ''' One of the sequenceLength sequences, z,
    ''' is chosen either at random
    ''' The pattern description q_{i,j} frequency is
    ''' then calculated from the current positions a_k
    ''' in all sequences excluding z </summary>
    ''' <param name="S">, the sequences other than z </param>
    ''' <param name="A">, the sites for the sequences other than z </param>
    Private Function predictiveUpdateStep(S As IList(Of String), A As IList(Of Integer)) As SequenceMatrix
        ' Compute q_{i,j} from the current positions a_k
        Return New SequenceMatrix(getMotifStrings(S, A))
    End Function

    ''' <summary>
    ''' Grabs the motif strings of length motifLength
    ''' from each sequence and site </summary>
    ''' <param name="S">, sequences </param>
    ''' <param name="A">, sites </param>
    ''' <returns> sequenceCount motif strings </returns>
    Private Function getMotifStrings(S As IList(Of String), A As IList(Of Integer)) As IList(Of String)
        Return Enumerable.Range(0, S.Count) _
            .Select(Function(i)
                        Dim site = A(i)
                        Dim sequence = S(i)
                        Return sequence.Substring(site, motifLength)
                    End Function) _
            .ToList()
    End Function

    ''' <summary>
    ''' Every possible segment of width motifLength within sequence z
    ''' is considered as a possible instance of the pattern. The
    ''' probabilities Q_x of generating each segment x according to
    ''' the current pattern probabilities q_{i,j} are calculated
    ''' The weight A_x = Q_x/P_x is assigned to segment x, and
    ''' with each segment so weighted, a random one is selected.
    ''' Its position then becomes the new a_z. </summary>
    ''' <param name="z">, sequence we are iterating through </param>
    Private Function samplingStep(q_ij As SequenceMatrix, z As String, P As IList(Of Double)) As Integer
        Dim A As IList(Of Double) = Enumerable.Range(CInt(0), CInt(sequenceLength - motifLength)) _
            .AsParallel() _
            .Select(Function(x)
                        Return calculateMotifProbability(q_ij, z, x, P)
                    End Function) _
            .AsList()
        Dim weightDistribution = smoothProbabilities(A)
        Dim choice = weightedChooseIndex(weightDistribution)

        Return Enumerable.Range(0, sequenceLength - motifLength) _
            .reduce(Function(i, b)
                        Return If(A(i).Equals(choice), i, b)
                    End Function, 0)
    End Function

    ''' <summary>
    ''' calculates the log probability of a character appearing at a specific index in a motif </summary>
    ''' <param name="q_ij">, motif weight matrix </param>
    ''' <param name="z">, string of characters </param>
    ''' <param name="x">, index of site in z </param>
    ''' <param name="P">, background frequencies </param>
    ''' <returns> log probability </returns>
    Private Function calculateMotifProbability(q_ij As SequenceMatrix, z As String, x As Integer, P As IList(Of Double)) As Double
        Dim sum As Double = 0

        For i As Integer = 0 To motifLength - 1
            Dim baseIdx = Utils.indexOfBase(z(x + i))
            Dim q = q_ij.probability(i, baseIdx)
            Dim lP = 1 / P(baseIdx)

            sum += Math.Log(q / lP)
        Next

        Return sum
    End Function

    ''' <summary>
    ''' Takes Q a list of log probabilities
    ''' Replaces negative infinities with 1 less than the minimum log probability </summary>
    ''' <param name="A">, log probabilities </param>
    ''' <returns> list of smoothed probabilities </returns>
    Private Function smoothProbabilities(A As IList(Of Double)) As IList(Of Double)
        ' Find the smallest probability greater than 0
        Dim minExceptInfinity As Func(Of Double, Double, Double) = Function(i, b) If(i < b AndAlso Not b.Equals(Double.NegativeInfinity), b, i)
        Dim min As Double = A.Aggregate(Double.NegativeInfinity, minExceptInfinity)

        ' Assert that there is some non zero probability so that we may smooth
        If min <= Double.NegativeInfinity + 1 Then
            Return A.Select(Function(i) -100.0).ToList()
        Else
            ' Replace the 0 probability indices with (min - 1) log probability
            Return A.Select(Function(i) If(i.Equals(Double.NegativeInfinity), min - 1, i)).ToList()
        End If
    End Function

    ''' <summary>
    ''' Currently picks the one with the greatest probability but should be
    ''' picking randomly from a weighted distribution </summary>
    ''' <param name="weightDistribution">, sequenceCount smoothed log probabilities </param>
    ''' <returns> new index of the site </returns>
    Private Function weightedChooseIndex(weightDistribution As IList(Of Double)) As Double
        Return weightDistribution.Aggregate(Double.NegativeInfinity, Function(a, b) If(a > b, a, b))
    End Function

    ''' <summary>
    ''' Creates a list of sequenceLength random numbers
    ''' using the random object supplied
    ''' the numbers are from 0 to sequenceLength-motifLength-1 inclusive </summary>
    ''' <returns> sequenceLength random ints </returns>
    Private Function getRandomSites() As IEnumerable(Of Integer)
        Return Enumerable.Range(0, m_sequenceCount).Select(Function(i) randf.Next(sequenceLength - motifLength))
    End Function
End Class

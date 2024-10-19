
Imports System.IO
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

    Private Shared ReadOnly LOG_2 As Double = Math.Log(2)

    Public Overridable ReadOnly Property Sequences As IList(Of String)
        Get
            Return sequencesField
        End Get
    End Property

    Friend motifLength As Integer
    Friend sequenceLength As Integer
    Friend sequenceCountField As Integer
    Friend outputDirectory As String
    Friend sequencesField As IList(Of String)

    Friend Sub New(fastaFileName As String, motifLength As Integer, outputDirectory As String)
        sequencesField = readFastaFile(fastaFileName)
        Me.motifLength = motifLength
        sequenceLength = sequencesField(0).Length
        sequenceCountField = sequencesField.Count
        Me.outputDirectory = outputDirectory
    End Sub

    ''' <summary>
    ''' Load all sequences from *.fa </summary>
    ''' <param name="fileName"> path of fa file </param>
    ''' <returns> Set of Gene Sequence String </returns>
    Private Function readFastaFile(fileName As String) As IList(Of String)
        Try
            Return FastaFile.Read(fileName).Select(Function(fa) fa.SequenceData).ToList
        Catch e As FileNotFoundException
            Console.WriteLine(e.ToString())
            Console.Write(e.StackTrace)
            Return New List(Of String)()
        End Try
    End Function

    ''' <returns> the size of the list sequences </returns>
    Public Overridable ReadOnly Property SequenceCount As Integer
        Get
            Return sequenceCountField
        End Get
    End Property

    ''' <summary>
    ''' Runs numSamples gibbsSamples to find a prediction on the sites
    ''' and motifs with the highest information content in the sequences </summary>
    ''' <param name="maxIterations">maximum number of times to iterate in a Gibbs Sample </param>
    Public Overridable Overloads Sub find(Optional maxIterations As Integer = 1000)
        Dim numSamples As Integer = SequenceCount

        Console.WriteLine("============= Input Sequences =============")
        ' sequences.ForEach(System.out.println);
        Console.WriteLine("============= Result of Gibbs Sampling Algorithm in each iteration =============")
        Dim predictedSites As IList(Of Integer) = New List(Of Integer)()
        Dim predictedMotifs As IList(Of String) = New List(Of String)()

        Dim maxInformationContent = New Double() {Double.NegativeInfinity}
        Call Enumerable.Range(0, numSamples).AsParallel().ForEach(Sub(j, i)
                                                                      SyncLock maxInformationContent
                                                                          If maxInformationContent(0) / motifLength = 2.0 Then
                                                                              Return
                                                                          End If
                                                                      End SyncLock
                                                                      Dim sites As IList(Of Integer) = gibbsSample(maxIterations, New List(Of String)(sequencesField))
                                                                      Dim s As String = Enumerable.Select(Of Integer, Global.System.[String])(sites, CType(Function(k) CStr(k.ToString()), Func(Of Integer, String))).JoinBy(" ")
                                                                      Dim motifs = getMotifStrings(sequencesField, sites)
                                                                      Dim informationContent = Me.informationContent(motifs)
                                                                      Dim newMax As Boolean
                                                                      SyncLock maxInformationContent
                                                                          newMax = informationContent >= maxInformationContent(0)
                                                                      End SyncLock
                                                                      If newMax Then
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
                                                                      End If
                                                                      Console.WriteLine(informationContent.ToString() & " :: " & s)
                                                                  End Sub)

        Dim motifMatrix As WeightMatrix = New SequenceMatrix(predictedMotifs)
        Dim icpc As Double? = maxInformationContent(0) / motifLength
        Console.WriteLine("======== Maximum Information Content :: " & maxInformationContent(0) / motifLength.ToString() & " =========" & vbLf)

        Try
            Writer.writeSites(sequenceCountField, predictedMotifs, predictedSites, outputDirectory & "predictedsites.txt")
            Writer.writeMotif(sequenceCountField, motifMatrix, outputDirectory & "predictedmotif.txt", icpc.Value)
        Catch e As Exception When TypeOf e Is FileNotFoundException
            ' e.printStackTrace();
        End Try
    End Sub

    Private Function informationContent(motifs As IList(Of String)) As Double
        Dim sm As SequenceMatrix = New SequenceMatrix(motifs)
        Return Enumerable.Select(Of Integer, Global.System.[Double])(Enumerable.Range(CInt(0), CInt(motifLength)), CType(Function(i) Enumerable.Sum(Enumerable.Where(Of Double)(Enumerable.Select(Of Integer, Global.System.[Double])(Enumerable.Range(CInt(0), CInt(4)), CType(Function(j) sm.probability(CInt(i), CInt(j)) * (Math.Log(sm.probability(CInt(i), CInt(j)) * 4) / LOG_2), Func(Of Integer, Double))), CType(Function(d) CBool(Not Double.IsNaN(d)), Func(Of Double, Boolean)))), Func(Of Integer, Double))).Sum()
    End Function

    ''' <summary>
    ''' Implements the Gibbs Sampling algorithm found in the lawrence93.pdf </summary>
    ''' <param name="maxIterations">, maximum number of iterations sampling may take </param>
    ''' <returns> Sets of int predicting the position motifs located in each sequence </returns>
    Private Function gibbsSample(maxIterations As Integer, S As IList(Of String)) As IList(Of Integer)
        Dim A = getRandomSites()
        Dim i = 0
        While Math.Min(Threading.Interlocked.Increment(i), i - 1) < maxIterations
            ' Choose the next sequence
            Dim idx = randf.Next(sequenceCountField)
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
        End While
        Return A
    End Function

    ''' <summary>
    ''' Calculates the background probabilities for each base </summary>
    ''' <param name="S">, sequenceCount sequences </param>
    ''' <returns> List of Double length 4 </returns>
    Private Function calculateP(S As IList(Of String)) As IList(Of Double)
        Dim P = New Double() {0, 0, 0, 0}
        Enumerable.Range(0, sequenceCountField - 1).ForEach(Sub(i, j)
                                                                Enumerable.Range(0, sequenceLength).ForEach(Sub(jj, ii) P(Utils.indexOfBase(S(ii)(jj))) += 1)
                                                            End Sub)

        Dim sum As Double = P.Sum()

        Return P.[Select](Function(d) d / sum).ToList()
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
        Dim lA = getMotifStrings(S, A)
        Return New SequenceMatrix(lA)
    End Function

    ''' <summary>
    ''' Grabs the motif strings of length motifLength
    ''' from each sequence and site </summary>
    ''' <param name="S">, sequences </param>
    ''' <param name="A">, sites </param>
    ''' <returns> sequenceCount motif strings </returns>
    Private Function getMotifStrings(S As IList(Of String), A As IList(Of Integer)) As IList(Of String)
        Return Enumerable.Range(0, S.Count).[Select](Function(i)
                                                         Dim site = A(i)
                                                         Dim sequence = S(i)
                                                         Return sequence.Substring(site, motifLength)
                                                     End Function).ToList()
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
        Dim A As IList(Of Double) = ParallelEnumerable.Select(Of Integer, Global.System.[Double])(ParallelEnumerable.AsParallel(Enumerable.Range(CInt(0), CInt(sequenceLength - motifLength))), CType(Function(x) calculateMotifProbability(CType(q_ij, SequenceMatrix), CStr(z), CInt(x), CType(P, IList(Of Double))), Func(Of Integer, Double))).AsList()
        Dim weightDistribution = smoothProbabilities(A)
        Dim choice = weightedChooseIndex(weightDistribution)
        Return Enumerable.Range(0, sequenceLength - motifLength).reduce(Function(i, b) If(A(i).Equals(choice), i, b), 0)
    End Function

    ''' <summary>
    ''' calculates the log probability of a character appearing at a specific index in a motif </summary>
    ''' <param name="q_ij">, motif weight matrix </param>
    ''' <param name="z">, string of characters </param>
    ''' <param name="x">, index of site in z </param>
    ''' <param name="P">, background frequencies </param>
    ''' <returns> log probability </returns>
    Private Function calculateMotifProbability(q_ij As SequenceMatrix, z As String, x As Integer, P As IList(Of Double)) As Double
        Return Enumerable.Select(Enumerable.Range(CInt(0), CInt(motifLength)), CType(Function(i)
                                                                                         Dim baseIdx = Utils.indexOfBase(CChar(z(CInt(x + i))))
                                                                                         Dim q = q_ij.probability(CInt(i), CInt(baseIdx))
                                                                                         Dim lP = 1 / P(CInt(baseIdx))
                                                                                         Return Math.Log(q / lP)
                                                                                     End Function, Func(Of Integer, Double))).Sum
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
            Return A.[Select](Function(i) -100.0).ToList()
        Else
            ' Replace the 0 probability indices with (min - 1) log probability
            Return A.[Select](Function(i) If(i.Equals(Double.NegativeInfinity), min - 1, i)).ToList()
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
    Private Function getRandomSites() As IList(Of Integer)
        Return Enumerable.Range(0, sequenceCountField).[Select](Function(i) randf.Next(sequenceLength - motifLength)).ToList()
    End Function
End Class
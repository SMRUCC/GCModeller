Imports System.Runtime.CompilerServices
Imports SMRUCC.genomics.SequenceModel.NucleotideModels

Namespace DeltaSimilarity1998

    ''' <summary>
    ''' SLIDING WINDOW GENOME SIGNATURE ANALYSIS - locating the alien DNA / 
    ''' specialization islands (Karlin, Campbell &amp; Mrazek 1998, section 8).
    ''' 
    ''' ```
    '''    for each position i:
    '''        take the window W(i) (default 50 kb, the paper's contig size)
    '''        compute delta*( signature(W(i)), average genome signature )
    '''    plot delta* along the genome coordinates
    ''' ```
    ''' 
    ''' Alien DNA which has not yet been ameliorated shows a genome signature 
    ''' significantly deviating from the host average, and therefore appears 
    ''' as peaks on the curve (e.g. the ``B. subtilis`` peaks at 2.18-2.28 Mb and 
    ''' 2.65-2.75 Mb, or the 37kb cagA pathogenicity island of ``H. pylori``).
    ''' 
    ''' PERFORMANCE: the window dinucleotide count matrix is maintained 
    ''' incrementally - sliding one base right only removes the trailing dimer/monomer 
    ''' and adds the leading one (O(1) update), so the whole profile costs O(n) 
    ''' regardless of the window size. The legacy 
    ''' ``ToolsAPI.GenomeSigmaDifference_p`` rebuilt the window cache O(window) 
    ''' times per window and re-created the comparison sequence O(n) times.
    ''' </summary>
    Public Module SlidingWindowDelta

        ''' <summary>
        ''' Compute the sliding window delta* profile of the given genome 
        ''' against its own global average genome signature.
        ''' </summary>
        ''' <param name="genome"></param>
        ''' <param name="windowSize">window size in bp (default 50000 = the paper's 50kb contig)</param>
        ''' <param name="stepSize">sampling step in bp (default 5000)</param>
        ''' <param name="parallel">evaluate the sampled windows in parallel (PLINQ)</param>
        ''' <returns>profile points ordered by the site position</returns>
        <Extension>
        Public Function DeltaStarProfile(genome As NucleicAcid,
                                         Optional windowSize As Integer = 50000,
                                         Optional stepSize As Integer = 5000,
                                         Optional parallel As Boolean = True) As WindowDelta()
            Dim seq As DNA() = genome.nt
            Dim n As Integer = seq.Length

            If n < windowSize Then
                windowSize = n
            End If

            If stepSize < 1 Then
                stepSize = 1
            End If

            Dim globalSignature As Double() = genome.SignatureVector()
            Dim samples As New List(Of Integer)

            For i As Integer = 0 To n - windowSize Step stepSize
                samples.Add(i)
            Next

            ' window [start, start + windowSize - 1]
            Dim lastStart As Integer = n - windowSize
            Dim windowCounts As New WindowDimerCounts(seq, lastStart, windowSize)
            Dim delta As New Dictionary(Of Integer, Double)

            For Each start As Integer In samples
                ' fast-forward the incremental counters up to the sample position
                Call windowCounts.Seek(start)
                Call delta.Add(start, WindowDeltaValue(windowCounts, globalSignature, windowSize))
            Next

            Dim profile As IEnumerable(Of WindowDelta) = If(parallel, samples.AsParallel, samples) _
                .Select(Function(site)
                            Return New WindowDelta With {
                                .site = site,
                                .deltaStar = delta(site) * 1000,
                                .level = DeltaStarDistance.DeltaStarLevel(delta(site))
                            }
                        End Function)

            Return profile.OrderBy(Function(w) w.site).ToArray
        End Function

        ''' <summary>
        ''' Compute the delta* profile against an arbitrary reference sequence 
        ''' signature (for example, comparing a genome against another organism).
        ''' </summary>
        ''' <param name="genome"></param>
        ''' <param name="reference">the reference sequence</param>
        ''' <param name="windowSize"></param>
        ''' <param name="stepSize"></param>
        ''' <returns></returns>
        <Extension>
        Public Iterator Function DeltaStarProfile(genome As NucleicAcid,
                                                  reference As NucleicAcid,
                                                  Optional windowSize As Integer = 50000,
                                                  Optional stepSize As Integer = 5000) As IEnumerable(Of WindowDelta)

            Dim globalSignature As Double() = reference.SignatureVector()
            Dim seq As DNA() = genome.nt
            Dim n As Integer = seq.Length

            If n < windowSize Then
                windowSize = n
            End If

            If stepSize < 1 Then
                stepSize = 1
            End If

            Dim lastStart As Integer = n - windowSize
            Dim windowCounts As New WindowDimerCounts(seq, 0, windowSize)

            ' the incremental window counters are strictly sequential: 
            ' each sample only costs O(stepSize) slide updates + O(16) distance
            For start As Integer = 0 To lastStart Step stepSize
                Dim d As Double = windowCounts _
                    .Seek(start) _
                    .WindowDeltaValue(globalSignature, windowSize)

                Yield New WindowDelta With {
                    .site = start,
                    .deltaStar = d * 1000,
                    .level = DeltaStarDistance.DeltaStarLevel(d)
                }
            Next
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Friend Function WindowDeltaValue(window As WindowDimerCounts,
                                         globalSignature As Double(),
                                         windowSize As Integer) As Double
            Dim windowSignature As Double() = window.SignatureVector(windowSize)
            Dim sum As Double = 0

            For i As Integer = 0 To 15
                sum += Math.Abs(windowSignature(i) - globalSignature(i))
            Next

            Return sum / 16
        End Function
    End Module

    ''' <summary>
    ''' The incremental dinucleotide count matrix of a sliding window. 
    ''' Sliding one base right costs O(1): remove the trailing dimer/monomer, 
    ''' add the leading monomer/dimer.
    ''' </summary>
    Friend Class WindowDimerCounts

        Friend ReadOnly monomer As Integer() = New Integer(3) {}
        Friend ReadOnly dimer As Integer(,) = New Integer(3, 3) {}
        Friend ReadOnly seq As DNA()
        Friend ReadOnly windowSize As Integer
        Friend ReadOnly lastStart As Integer
        Friend position As Integer

        ''' <summary>
        ''' Initialize the counters on the window ``[start, start + windowSize - 1]``.
        ''' </summary>
        ''' <param name="seq"></param>
        ''' <param name="start">0-based window start</param>
        ''' <param name="windowSize"></param>
        Sub New(seq As DNA(), start As Integer, windowSize As Integer)
            Me.seq = seq
            Me.windowSize = windowSize
            Me.lastStart = seq.Length - windowSize

            If start < 0 Then
                start = 0
            End If

            For i As Integer = start To start + windowSize - 1
                Dim b As Integer = NucleicAcid.BaseIndex(seq(i))

                If b >= 0 Then
                    monomer(b) += 1
                End If

                If b >= 0 AndAlso i > start Then
                    Dim prev As Integer = NucleicAcid.BaseIndex(seq(i - 1))

                    If prev >= 0 Then
                        dimer(prev, b) += 1
                    End If
                End If
            Next

            position = start
        End Sub

        ''' <summary>
        ''' Slide the window until it starts at the given position.
        ''' </summary>
        ''' <param name="target"></param>
        Public Function Seek(target As Integer) As WindowDimerCounts
            While position < target
                Slide()
            End While

            Return Me
        End Function

        ''' <summary>
        ''' Slide the window one base right (O(1) update).
        ''' </summary>
        Public Sub Slide()
            If position >= lastStart Then
                Exit Sub
            End If

            Dim outBase As Integer = NucleicAcid.BaseIndex(seq(position))
            Dim inBase As Integer = NucleicAcid.BaseIndex(seq(position + windowSize))

            ' remove the trailing dimer (position, position+1)
            If outBase >= 0 Then
                monomer(outBase) -= 1
                Dim nextBase As Integer = NucleicAcid.BaseIndex(seq(position + 1))

                If nextBase >= 0 Then
                    dimer(outBase, nextBase) -= 1
                End If
            End If

            ' add the leading dimer (position+windowSize-1, position+windowSize)
            If inBase >= 0 Then
                monomer(inBase) += 1
                Dim prevBase As Integer = NucleicAcid.BaseIndex(seq(position + windowSize - 1))

                If prevBase >= 0 Then
                    dimer(prevBase, inBase) += 1
                End If
            End If

            position += 1
        End Sub

        ''' <summary>
        ''' The symmetrized 16-dim genome signature vector of the current window.
        ''' </summary>
        ''' <param name="windowSize"></param>
        ''' <returns></returns>
        Public Function SignatureVector(windowSize As Integer) As Double()
            Dim vec As Double() = New Double(15) {}
            Dim order As (X As DNA, Y As DNA)() = NucleicAcid.DimerOrder
            ' exact valid base/pair totals of the current window (the ambiguous 
            ' bases are excluded from the frequency denominators)
            Dim validBases As Double = monomer.Sum
            Dim validPairs As Double = 0

            For i As Integer = 0 To 3
                For j As Integer = 0 To 3
                    validPairs += dimer(i, j)
                Next
            Next

            If validBases < 2 OrElse validPairs < 1 Then
                Return vec
            End If

            For k As Integer = 0 To 15
                Dim i As Integer = NucleicAcid.BaseIndex(order(k).X)
                Dim j As Integer = NucleicAcid.BaseIndex(order(k).Y)

                If i < 0 OrElse j < 0 Then
                    Continue For
                End If

                Dim ci As Integer = NucleicAcid.ComplementIndex(i)
                Dim cj As Integer = NucleicAcid.ComplementIndex(j)
                Dim symPair As Double = dimer(i, j) + dimer(cj, ci)
                Dim symX As Double = monomer(i) + monomer(ci)
                Dim symY As Double = monomer(j) + monomer(cj)

                If symX > 0 AndAlso symY > 0 Then
                    ' same odds ratio convention as <see cref="NucleicAcid.RelativeAbundance"/>
                    Dim fxy As Double = symPair / validPairs
                    Dim fx As Double = symX / validBases
                    Dim fy As Double = symY / validBases

                    vec(k) = fxy / (fx * fy)
                End If
            Next

            Return vec
        End Function
    End Class
End Namespace

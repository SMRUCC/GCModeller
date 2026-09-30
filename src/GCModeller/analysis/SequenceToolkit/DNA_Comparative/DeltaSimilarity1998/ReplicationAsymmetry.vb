Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.SequenceModel.NucleotideModels

Namespace DeltaSimilarity1998

    ''' <summary>
    ''' One point of the strand composition asymmetry profile: 
    ''' ``(C - G) / (C + G)`` of one sliding window.
    ''' </summary>
    Public Class StrandSkew

        ''' <summary>
        ''' the 0-based start position of the window on the genome
        ''' </summary>
        <Column("Site")> Public Property Site As Integer

        ''' <summary>
        ''' the GC skew value ``(C - G) / (C + G)`` of the window
        ''' </summary>
        <Column("GcSkew")> Public Property GcSkew As Double

        Public Overrides Function ToString() As String
            Return $"[{Site}] (C-G)/(C+G) = {GcSkew:F5}"
        End Function
    End Class

    ''' <summary>
    ''' STRAND COMPOSITION ASYMMETRY - inferring the replication origin and direction 
    ''' (Karlin, Campbell &amp; Mrazek 1998, section 9).
    ''' 
    ''' ```
    '''    sliding window computation of (C - G) / (C + G), plotted along the genome
    ''' ```
    ''' 
    ''' In ``B. subtilis`` the curve flips its sign at the replication origin oriC: 
    ''' the leading strand is rich in G and poor in C (the leading strand favors 
    ''' purines, G &gt; C), and the lagging strand is the opposite. Linear genomes 
    ''' (e.g. ``B. burgdorferi``) split into a G&gt;C half and a C&gt;G half, while 
    ''' ``Synechocystis`` and many archaeal genomes show no such asymmetry (which 
    ''' indicates multiple replication origins).
    ''' 
    ''' Note: the dinucleotide genome signature (``rho*``) is always nearly invariant 
    ''' between the leading and the lagging strand - the signature is maintained by 
    ''' other whole-genome mechanisms.
    ''' </summary>
    <Package("Replication.Asymmetry",
             Description:="(C-G)/(C+G) strand composition asymmetry profile and the replication origin inference (Karlin 1998)")>
    Public Module ReplicationAsymmetry

        ''' <summary>
        ''' Compute the ``(C - G) / (C + G)`` sliding window profile.
        ''' </summary>
        ''' <param name="genome"></param>
        ''' <param name="windowSize">window size in bp (default 10kb)</param>
        ''' <param name="stepSize">sampling step in bp (default 1kb)</param>
        ''' <returns>profile points ordered by the site position</returns>
        <ExportAPI("genome.gc_skew")>
        <Extension>
        Public Function GcSkewProfile(genome As NucleicAcid,
                                      Optional windowSize As Integer = 10000,
                                      Optional stepSize As Integer = 1000) As StrandSkew()
            Dim seq As DNA() = genome.nt
            Dim n As Integer = seq.Length

            If windowSize > n Then
                windowSize = n
            End If

            If stepSize < 1 Then
                stepSize = 1
            End If

            Dim profile As New List(Of StrandSkew)
            Dim lastStart As Integer = n - windowSize
            ' incremental window counts of C and G (O(1) slide update)
            Dim cCount As Integer = 0
            Dim gCount As Integer = 0

            ' initial window [0, windowSize - 1]
            For i As Integer = 0 To windowSize - 1
                Select Case seq(i)
                    Case DNA.dCMP : cCount += 1
                    Case DNA.dGMP : gCount += 1
                End Select
            Next

            profile.Add(NewStrandSkew(0, cCount, gCount))

            For start As Integer = stepSize To lastStart Step stepSize
                ' fast-forward slide (bases leaving/entering between the samples)
                For i As Integer = start - stepSize To start - 1
                    Dim outBase As Integer = NucleicAcid.BaseIndex(seq(i))
                    Dim inBase As Integer = NucleicAcid.BaseIndex(seq(i + windowSize))

                    If outBase = 2 Then : cCount -= 1
                    ElseIf outBase = 1 Then : gCount -= 1
                    End If

                    If inBase = 2 Then : cCount += 1
                    ElseIf inBase = 1 Then : gCount += 1
                    End If
                Next

                profile.Add(NewStrandSkew(start, cCount, gCount))
            Next

            Return profile.ToArray
        End Function

        Private Function NewStrandSkew(site As Integer, cCount As Integer, gCount As Integer) As StrandSkew
            Dim denom As Double = cCount + gCount
            Dim skew As Double = If(denom > 0, (cCount - gCount) / denom, 0)

            Return New StrandSkew With {
                .Site = site,
                .GcSkew = skew
            }
        End Function

        ''' <summary>
        ''' Locate the candidate replication origin (oriC) positions: the sign-flip 
        ''' sites of the GC skew curve (transition from negative to positive skew 
        ''' marks the origin in the standard orientation, the transition from 
        ''' positive to negative marks the terminus).
        ''' </summary>
        ''' <param name="profile">result of <see cref="ReplicationAsymmetry.GcSkewProfile"/></param>
        ''' <returns>the window site positions where the skew changes sign</returns>
        <ExportAPI("replication_origin.predict")>
        <Extension>
        Public Function PredictReplicationOrigin(profile As StrandSkew()) As Integer()
            If profile.Length < 3 Then
                Return New Integer() {}
            End If

            ' smooth the profile with the cumulative sum to avoid the noise-driven 
            ' spurious sign flips (the cumulative GC skew is the standard tool for 
            ' the origin/terminus detection)
            Dim cumsum As Double = 0
            Dim cumulative As Double() = New Double(profile.Length - 1) {}

            For i As Integer = 0 To profile.Length - 1
                cumsum += profile(i).GcSkew
                cumulative(i) = cumsum
            Next

            Dim flips As New List(Of Integer)

            For i As Integer = 1 To cumulative.Length - 1
                Dim s0 As Double = cumulative(i - 1)
                Dim s1 As Double = cumulative(i)

                If s0 = 0 OrElse s1 = 0 Then
                    Continue For
                End If

                ' sign change of the slope of the cumulative curve = sign flip of 
                ' the local skew tendency
                If s0 < 0 AndAlso s1 >= 0 Then
                    flips.Add(profile(i).Site)
                ElseIf s0 > 0 AndAlso s1 <= 0 Then
                    flips.Add(profile(i).Site)
                End If
            Next

            Return flips.ToArray
        End Function
    End Module
End Namespace

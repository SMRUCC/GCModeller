Option Strict On

Imports System.Collections.Generic
Imports SMRUCC.genomics.SequenceModel.FASTA


    ''' <summary>
    ''' The synthetic MSA generator of the test suite.
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' Every sample is derived from one seeded reference sequence by independent
    ''' substitutions, gap and unknown runs, so the resulting pairwise identity is
    ''' concentrated around a known value. A fraction of the samples is emitted as an
    ''' exact copy of an earlier sample, which is what exercises the deduplication.
    ''' </para>
    ''' <para>
    ''' The sequences are produced lazily, one <see cref="FastaSeq"/> at a time, so a
    ''' dataset of hundreds of thousands of sequences can be streamed into the engine
    ''' without ever materializing the whole alignment as strings.
    ''' </para>
    ''' </remarks>
    Public NotInheritable Class SyntheticMsa

        ''' <summary> The default per site substitution rate. </summary>
        Public Const DefaultSubstitutionRate As Double = 0.05

        ''' <summary> The default per site gap rate. </summary>
        Public Const DefaultGapRate As Double = 0.01

        ''' <summary> The default per site unknown rate. </summary>
        Public Const DefaultUnknownRate As Double = 0.005

        Private Sub New()
        End Sub

        ''' <summary>
        ''' Generates the synthetic alignment lazily.
        ''' </summary>
        ''' <param name="count">The number of sequences.</param>
        ''' <param name="length">The alignment length.</param>
        ''' <param name="seed">The random seed, the same seed always produces the same dataset.</param>
        ''' <param name="substitutionRate"></param>
        ''' <param name="gapRate"></param>
        ''' <param name="unknownRate"></param>
        ''' <param name="duplicateFraction">
        ''' The fraction of the samples that is emitted as an exact copy of the first
        ''' sample, used to exercise the deduplication.
        ''' </param>
        ''' <returns></returns>
        Public Shared Iterator Function Generate(count As Integer,
                                                 alignmentLength As Integer,
                                                 seed As Integer,
                                                 Optional substitutionRate As Double = DefaultSubstitutionRate,
                                                 Optional gapRate As Double = DefaultGapRate,
                                                 Optional unknownRate As Double = DefaultUnknownRate,
                                                 Optional duplicateFraction As Double = 0.0,
                                                 Optional namePrefix As String = "sample") As IEnumerable(Of FastaSeq)
            If count <= 0 OrElse alignmentLength <= 0 Then
                Return
            End If

            Dim random As New Random(seed)
            Dim reference As Char() = RandomReference(random, alignmentLength)
            Dim first As Char() = Nothing

            For i As Integer = 0 To count - 1
                Dim row As Char()

                If duplicateFraction > 0 AndAlso first IsNot Nothing AndAlso random.NextDouble() < duplicateFraction Then
                    row = CType(first.Clone(), Char())
                Else
                    row = Mutate(reference, random, substitutionRate, gapRate, unknownRate)

                    If first Is Nothing Then
                        first = CType(row.Clone(), Char())
                    End If
                End If

                Yield New FastaSeq(New String(row), $"{namePrefix}_{i + 1:000000}")
            Next
        End Function

        ''' <summary>
        ''' Generates the synthetic alignment as a materialized <see cref="FastaFile"/>.
        ''' </summary>
        ''' <param name="count"></param>
        ''' <param name="length"></param>
        ''' <param name="seed"></param>
        ''' <param name="substitutionRate"></param>
        ''' <param name="gapRate"></param>
        ''' <param name="unknownRate"></param>
        ''' <param name="duplicateFraction"></param>
        ''' <returns></returns>
        Public Shared Function GenerateFile(count As Integer,
                                            alignmentLength As Integer,
                                            seed As Integer,
                                            Optional substitutionRate As Double = DefaultSubstitutionRate,
                                            Optional gapRate As Double = DefaultGapRate,
                                            Optional unknownRate As Double = DefaultUnknownRate,
                                            Optional duplicateFraction As Double = 0.0,
                                            Optional namePrefix As String = "sample") As FastaFile
            Return New FastaFile(Generate(count, alignmentLength, seed, substitutionRate, gapRate, unknownRate, duplicateFraction, namePrefix))
        End Function

        ''' <summary>
        ''' A random ACGT reference sequence.
        ''' </summary>
        Private Shared Function RandomReference(random As Random, alignmentLength As Integer) As Char()
            Dim row As Char() = New Char(alignmentLength - 1) {}
            Dim bases As Char() = {"A"c, "C"c, "G"c, "T"c}

            For i As Integer = 0 To alignmentLength - 1
                row(i) = bases(random.Next(4))
            Next

            Return row
        End Function

        ''' <summary>
        ''' Applies the mutation model to a copy of the reference sequence.
        ''' </summary>
        Private Shared Function Mutate(reference As Char(), random As Random, substitutionRate As Double, gapRate As Double, unknownRate As Double) As Char()
            Dim row As Char() = CType(reference.Clone(), Char())
            Dim bases As Char() = {"A"c, "C"c, "G"c, "T"c}

            For i As Integer = 0 To row.Length - 1
                Dim roll As Double = random.NextDouble()

                If roll < gapRate Then
                    row(i) = "-"c
                ElseIf roll < gapRate + unknownRate Then
                    row(i) = If(random.Next(2) = 0, "N"c, "n"c)
                ElseIf roll < gapRate + unknownRate + substitutionRate Then
                    Dim candidate As Char = bases(random.Next(4))

                    If candidate = row(i) Then
                        candidate = bases((Array.IndexOf(bases, candidate) + 1 + random.Next(3)) Mod 4)
                    End If

                    ' a fraction of the mutations is written in the lower case, the
                    ' codec has to be case insensitive
                    row(i) = If(random.Next(4) = 0, Char.ToLower(candidate), candidate)
                End If
            Next

            Return row
        End Function

    End Class


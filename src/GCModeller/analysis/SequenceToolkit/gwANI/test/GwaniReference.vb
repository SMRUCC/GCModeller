Option Strict On

Imports System.Collections.Generic
Imports System.IO
Imports SMRUCC.genomics.SequenceModel.FASTA


    ''' <summary>
    ''' The original scalar panito port, kept as the differential reference of the
    ''' vectorized engine.
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' The symbols follow the VB.NET naming conventions and the O(L^2)
    ''' <c>ChangeCharacter</c> rebuild of the reference row has been replaced by an
    ''' in place normalization of a character array, so the reference is fast enough
    ''' to be used as a test oracle. The mathematics is intentionally untouched:
    ''' </para>
    ''' <list type="bullet">
    ''' <item> <description> upper case the whole alignment and collapse N/n/-/? into 'N'; </description> </item>
    ''' <item> <description> a site is valid when both sides are known, it is a match when the two known bases are equal; </description> </item>
    ''' <item> <description> identity = 100 * matches / valids, or 0 when there is no valid site at all; </description> </item>
    ''' <item> <description> the diagonal is 100 and the lower triangle is -1. </description> </item>
    ''' </list>
    ''' </remarks>
    Public NotInheritable Class GwaniReference

        ''' <summary> The alignment length. </summary>
        Public ReadOnly Property AlignmentLength As Integer

        ''' <summary> The number of sequences. </summary>
        Public ReadOnly Property SampleCount As Integer

        ''' <summary> The sequence ids in the input order. </summary>
        Public ReadOnly Property SequenceNames As String()

        Private ReadOnly _rows As List(Of Char())

        Private Sub New(rows As List(Of Char()), names As String(), sites As Integer)
            _rows = rows
            SequenceNames = names
            ' the constructor parameter must not be named like the property, otherwise
            ' the parameter shadows the property and the assignment becomes a no-op
            AlignmentLength = sites
        End Sub

        ''' <summary>
        ''' Loads and normalizes a multi-FASTA alignment.
        ''' </summary>
        ''' <param name="msa"></param>
        ''' <returns></returns>
        Public Shared Function Load(msa As FastaFile) As GwaniReference
            If msa Is Nothing Then
                Throw New ArgumentNullException(NameOf(msa))
            End If

            Dim rows As New List(Of Char())()
            Dim names As New List(Of String)()
            Dim alignmentLength As Integer = -1

            For Each seq As FastaSeq In msa
                Dim data As String = If(seq.SequenceData, "")

                If alignmentLength < 0 Then
                    alignmentLength = data.Length
                ElseIf data.Length <> alignmentLength Then
                    Throw New InvalidDataException(
                        $"Alignment contains sequences of unequal length. Expected length is {alignmentLength} but got {data.Length} in sequence '{seq.Title}'.")
                End If

                Dim row As Char() = data.ToUpperInvariant().ToCharArray()

                For i As Integer = 0 To row.Length - 1
                    If IsUnknownBase(row(i)) Then
                        row(i) = "N"c
                    End If
                Next

                names.Add(seq.Title)
                rows.Add(row)
            Next

            Return New GwaniReference(rows, names.ToArray(), Math.Max(0, alignmentLength))
        End Function

        ''' <summary>
        ''' Computes the full input ordered matrix, including the lower triangle
        ''' placeholders, exactly like the original panito port did.
        ''' </summary>
        ''' <returns></returns>
        Public Function ComputeMatrix() As Double(,)
            Dim n As Integer = _rows.Count
            Dim matrix As Double(,) = New Double(n - 1, n - 1) {}

            For i As Integer = 0 To n - 1
                matrix(i, i) = 100.0

                For j As Integer = i + 1 To n - 1
                    matrix(i, j) = IdentityOf(i, j)
                Next

                For j As Integer = 0 To i - 1
                    matrix(i, j) = -1.0
                Next
            Next

            Return matrix
        End Function

        ''' <summary>
        ''' Computes the upper triangle of the row at <paramref name="referenceIndex"/>.
        ''' </summary>
        ''' <param name="referenceIndex"></param>
        ''' <returns></returns>
        Public Function ComputeRow(referenceIndex As Integer) As Double()
            Dim n As Integer = _rows.Count
            Dim row As Double() = New Double(n - 1) {}

            row(referenceIndex) = 100.0

            For j As Integer = 0 To referenceIndex - 1
                row(j) = -1.0
            Next

            For j As Integer = referenceIndex + 1 To n - 1
                row(j) = IdentityOf(referenceIndex, j)
            Next

            Return row
        End Function

        ''' <summary>
        ''' The identity of the pair <c>(a, b)</c>, both indices are input indices.
        ''' </summary>
        ''' <param name="a"></param>
        ''' <param name="b"></param>
        ''' <returns></returns>
        Public Function IdentityOf(a As Integer, b As Integer) As Double
            Dim rowA As Char() = _rows(a)
            Dim rowB As Char() = _rows(b)
            Dim matches As Integer = 0
            Dim valids As Integer = AlignmentLength

            For i As Integer = 0 To AlignmentLength - 1
                If rowA(i) = "N"c OrElse rowB(i) = "N"c Then
                    valids -= 1
                ElseIf rowA(i) = rowB(i) Then
                    matches += 1
                End If
            Next

            If valids > 0 Then
                Return 100.0 * matches / valids
            Else
                Return 0.0
            End If
        End Function

        ''' <summary>
        ''' The unknown base set of the original panito tool.
        ''' </summary>
        ''' <param name="symbol"></param>
        ''' <returns></returns>
        Private Shared Function IsUnknownBase(symbol As Char) As Boolean
            Return "Nn-?".IndexOf(symbol) >= 0
        End Function

    End Class


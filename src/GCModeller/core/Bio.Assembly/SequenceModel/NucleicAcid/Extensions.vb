#Region "Microsoft.VisualBasic::9828e761a016e85d25122e1e503dc773, ..\GCModeller\core\Bio.Assembly\SequenceModel\NucleicAcid\Extensions.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
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

#End Region

Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData

Namespace SequenceModel.NucleotideModels

    <PackageNamespace("NucleotideModel.Statics")>
    Public Module Extensions

        <ExportAPI("Distance.Hamming")>
        Public Function HammingDistance(seq1 As String, seq2 As String) As Integer
            Dim Length As Integer = seq1.Length
            Dim HammingDist As Integer = 0

            If seq1.Length <> seq2.Length Then
                Length = Math.Min(seq1.Length, seq2.Length)
                HammingDist = Math.Abs(seq1.Length - seq2.Length)
            End If

            For i As Integer = 0 To Length - 1
                If seq1(i) <> seq2(i) Then
                    HammingDist += 1
                End If
            Next

            Return HammingDist
        End Function

        ''' <summary>
        ''' Compute Levenshtein distance  Michael Gilleland, Merriam Park Software.(http://www.merriampark.com/ld.htm)
        ''' </summary>
        ''' <param name="s"></param>
        ''' <param name="t"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' 
        <ExportAPI("Distance.Levenshtein", Info:="Compute Levenshtein distance  Michael Gilleland, Merriam Park Software.(http://www.merriampark.com/ld.htm)")>
        Public Function LevenshteinDistance(s As String, t As String) As Integer
            Dim d As Integer()()  ' matrix
            Dim n As Integer = s.Length ' length of s
            Dim m As Integer = t.Length   ' length of t
            Dim s_i As Char ' ith character of s
            Dim t_j As Char  ' jth character of t
            Dim cost As Integer

            ' ========> Step 1

            If n = 0 Then
                Return m
            End If

            If m = 0 Then
                Return n
            End If

            d = MAT(Of Integer)(n + 1, m + 1)

            ' ========> Step 2

            For i As Integer = 0 To n
                d(i)(0) = i
            Next

            For j As Integer = 0 To m
                d(0)(j) = j
            Next

            ' ========> Step 3

            For i As Integer = 1 To n

                s_i = s(i - 1)

                ' ========> Step 4

                For j As Integer = 1 To m

                    t_j = t(j - 1)

                    ' ========> Step 5

                    If s_i = t_j Then
                        cost = 0
                    Else
                        cost = 1
                    End If

                    ' ========> Step 6

                    d(i)(j) = Minimum(d(i - 1)(j) + 1, d(i)(j - 1) + 1, d(i - 1)(j - 1) + cost)
                Next
            Next

            Return d(n)(m)
        End Function

        ''' <summary>
        ''' Chas Emerick.(http://www.merriampark.com/ldjava.htm)
        ''' </summary>
        ''' <param name="s"></param>
        ''' <param name="t"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' The difference between this impl. and the previous is that, rather
        ''' than creating and retaining a matrix of size s.length()+1 by t.length()+1,
        ''' we maintain two single-dimensional arrays of length s.length()+1.  The first, d,
        ''' is the 'current working' distance array that maintains the newest distance cost
        ''' counts as we iterate through the characters of String s.  Each time we increment
        ''' the index of String t we are comparing, d is copied to p, the second int[].  Doing so
        ''' allows us to retain the previous cost counts as required by the algorithm (taking
        ''' the minimum of the cost count to the left, up one, and diagonally up and to the left
        ''' of the current cost count being calculated).  (Note that the arrays aren't really
        ''' copied anymore, just switched...this is clearly much better than cloning an array
        ''' or doing a System.arraycopy() each time  through the outer loop.)
        ''' 
        ''' Effectively, the difference between the two implementations is this one does not
        ''' cause an out of memory condition when calculating the LD over two very large strings.
        ''' </remarks>
        ''' 
        <ExportAPI("Distance.Levenshtein2", Info:="The difference between this impl. and the previous is that, rather
than creating and retaining a matrix of size s.length()+1 by t.length()+1,
we maintain two single-dimensional arrays of length s.length()+1.  The first, d,
is the 'current working' distance array that maintains the newest distance cost
counts as we iterate through the characters of String s.  Each time we increment
the index of String t we are comparing, d is copied to p, the second int[].  Doing so
allows us to retain the previous cost counts as required by the algorithm (taking
the minimum of the cost count to the left, up one, and diagonally up and to the left
of the current cost count being calculated).  (Note that the arrays aren't really
copied anymore, just switched...this is clearly much better than cloning an array
or doing a System.arraycopy() each time  through the outer loop.)

Effectively, the difference between the two implementations is this one does not
cause an out of memory condition when calculating the LD over two very large strings.")>
        Public Function LevenshteinDistance2(s As String, t As String) As Integer

            If String.IsNullOrEmpty(s) OrElse String.IsNullOrEmpty(t) Then Throw New System.ArgumentException("Strings must not be null")

            Dim n As Integer = s.Length   ' length of s
            Dim m As Integer = t.Length   ' length of t

            If n = 0 Then
                Return m
            ElseIf m = 0 Then
                Return n
            End If

            Dim p As Integer() = New Integer(n) {}
            ''previous' cost array, horizontally
            Dim d As Integer() = New Integer(n) {}
            ' cost array, horizontally
            Dim _d As Integer()
            'placeholder to assist in swapping p and d
            ' indexes into strings s and t
            Dim i As Integer
            ' iterates through s
            Dim j As Integer
            ' iterates through t
            Dim t_j As Char
            ' jth character of t
            Dim cost As Integer
            ' cost
            For i = 0 To n
                p(i) = i
            Next

            For j = 1 To m
                t_j = t(j - 1)
                d(0) = j

                For i = 1 To n
                    cost = If(s(i - 1) = t_j, 0, 1)
                    ' minimum of cell to the left+1, to the top+1, diagonally left and up +cost
                    d(i) = Math.Min(Math.Min(d(i - 1) + 1, p(i) + 1), p(i - 1) + cost)
                Next

                ' copy current distance counts to 'previous row' distance counts
                _d = p
                p = d
                d = _d
            Next

            ' our last action in the above loop was to switch d and p, so p now
            ' actually has the most recent cost counts
            Return p(n)
        End Function

        <ExportAPI("Similarity")>
        Public Function Similarity(s1 As String, s2 As String) As Double
            Dim maxLength As Integer = Math.Max(s1.Length, s2.Length)
            Dim value As Double = 1.0 - CDbl(LevenshteinDistance(s1, s2)) / maxLength
            Return value
        End Function

        <ExportAPI("Is.Pattern.Matched?")>
        Public Function PatternMatched(pattern1 As String, pattern2 As String, confidence As Double) As Boolean
            Dim PatternSimilarity As Double = Similarity(pattern1, pattern2)
            Return PatternSimilarity >= confidence
        End Function

        Private Function Minimum(a As Integer, b As Integer, c As Integer) As Integer
            Dim mi As Integer = a

            If b < mi Then
                mi = b
            End If

            If c < mi Then
                mi = c
            End If

            Return mi
        End Function
    End Module
End Namespace

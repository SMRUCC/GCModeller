#Region "Microsoft.VisualBasic::8c8c4de9f46cb37a9a4c1c4d652c96c8, analysis\Motifs\CRISPR\CRT\SearchingModel\BoyerMooreAlgorithmSearcher.vb"

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

    '     Class BoyerMooreAlgorithmSearcher
    ' 
    '         Function: __search, BoyerMooreSearch, LinearSSearch
    ' 
    '         Sub: Compile
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text

Namespace SearchingModel

    ''' <summary>
    ''' Boyer Moore algorithm copyright by Michael Lecuyer 1998. Slight modification below.
    ''' </summary>
    ''' <remarks></remarks>
    Public Class BoyerMooreAlgorithmSearcher

        ''' <summary>
        ''' Maximum chars in character set.
        ''' </summary>
        ''' <remarks></remarks>
        Public Const MAXCHAR As Integer = 256

        ''' <summary>
        ''' Byte representation of pattern
        ''' </summary>
        ''' <remarks></remarks>
        Dim pat As Byte()
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <remarks></remarks>
        Dim patLen As Integer
        ''' <summary>
        ''' Bytes of a partial match found at the end of a text buffer.
        ''' The position at the end of the text buffer where a partial match was found.
        ''' (-1 the number of bytes that formed a partial match, -1 if no partial match)
        ''' </summary>
        ''' <remarks>
        ''' <P>
        ''' In many case where a full text search of a large amount of data
        ''' precludes access to the entire file or stream the search algorithm
        ''' will note where the final partial match occurs.
        ''' After an entire buffer has been searched for full matches calling
        ''' this method will reveal if a potential match appeared at the end.
        ''' This information can be used to patch together the partial match
        ''' with the next buffer of data to determine if a real match occurred.</P>
        ''' </remarks>
        Dim [partial] As Integer
        Dim skip As Integer() = New Integer(MAXCHAR - 1) {}
        ''' <summary>
        ''' Internal BM table
        ''' </summary>
        ''' <remarks></remarks>
        Dim d As Integer()

        ''' <summary>
        ''' Using this function to search the pattern occurring in the target text data. 
        ''' If the pattern is found in the <paramref name="text"></paramref> then the 
        ''' index of the pattern occurring in the text will be returned, if not then 
        ''' the value -1 will be return.
        ''' </summary>
        ''' <param name="text"></param>
        ''' <param name="pattern"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function BoyerMooreSearch(text As String, pattern As String) As Integer
            Dim byteText As Byte() = Encoding.ASCII.GetBytes(text)
            Compile(pattern)
            Return __search(byteText, 0, text.Length)
        End Function

        Public Function LinearSSearch(Text As String, Pattern As String) As Integer
            Dim TextLength As Integer = Text.Length
            Dim PatternLength As Integer = Pattern.Length

            For i As Integer = 0 To TextLength - PatternLength
                Dim subPattern As String = Text.Substring(i, PatternLength)
                If Pattern.Equals(subPattern) Then
                    Return i
                End If
            Next

            Return -1
        End Function

        Public Sub Compile(pattern As String)
            pat = Encoding.ASCII.GetBytes(pattern)
            patLen = pat.Length

            Dim j As Integer, k As Integer, m As Integer, t As Integer, t1 As Integer, q As Integer, q1 As Integer
            Dim f As Integer() = New Integer(patLen - 1) {}

            d = New Integer(patLen - 1) {}

            m = patLen

            For k = 0 To MAXCHAR - 1
                skip(k) = m
            Next

            For k = 1 To m
                d(k - 1) = (m << 1) - k
                skip(pat(k - 1)) = m - k
            Next

            t = m + 1
            For j = m To 1 Step -1
                f(j - 1) = t
                While t <= m AndAlso pat(j - 1) <> pat(t - 1)
                    d(t - 1) = If((d(t - 1) < m - j), d(t - 1), m - j)
                    t = f(t - 1)
                End While
                t -= 1
            Next
            q = t
            t = m + 1 - q
            q1 = 1
            t1 = 0

            For j = 1 To t
                f(j - 1) = t1
                While t1 >= 1 AndAlso pat(j - 1) <> pat(t1 - 1)
                    t1 = f(t1 - 1)
                End While
                t1 += 1
            Next

            While q < m
                For k = q1 To q
                    d(k - 1) = If((d(k - 1) < m + q - k), d(k - 1), m + q - k)
                Next
                q1 = q + 1
                q = q + t - f(t - 1)
                t = f(t - 1)
            End While
        End Sub

        ''' <summary>
        ''' Search for the compiled pattern in the given text.
        ''' A side effect of the search is the notion of a partial
        ''' match at the end of the searched buffer.
        ''' This partial match is helpful in searching text files when
        ''' the entire file doesn't fit into memory.
        ''' </summary>
        ''' <param name="text"> Buffer containing the text </param>
        ''' <param name="start"> Start position for search </param>
        ''' <param name="length"> Length of text in the buffer to be searched.
        ''' </param>
        ''' <returns> position in buffer where the pattern was found. </returns>
        Private Function __search(text As Byte(), start As Integer, length As Integer) As Integer
            Dim textLen As Integer = length + start
            [partial] = -1 ' assume no partial match

            If d.IsNullOrEmpty Then Return -1 ' no pattern compiled, nothing matches.

            Dim m As Integer = patLen

            If m = 0 Then Return 0

            Dim max As Integer   ' used in calculation of partial match. Max distand we jumped.
            Dim k As Integer = start + m - 1
            Dim j As Integer

            While k < textLen

                j = m - 1

                While j >= 0 AndAlso text(k) = pat(j)
                    k -= 1
                    j -= 1
                End While

                If j = -1 Then
                    Return k + 1
                End If

                Dim z As Integer = skip(text(k))

                max = If((z > d(j)), z, d(j))
                k += max
            End While

            If k >= textLen AndAlso j > 0 Then
                [partial] = k - max - 1  ' if we're near end of buffer --
            End If

            Return -1   ' No match
        End Function
    End Class
End Namespace

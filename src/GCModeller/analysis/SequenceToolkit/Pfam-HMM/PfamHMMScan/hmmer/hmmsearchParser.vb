#Region "Microsoft.VisualBasic::48cb953e17dfd10343f5928404cb2d85, analysis\SequenceToolkit\Pfam-HMM\PfamHMMScan\hmmer\hmmsearchParser.vb"

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

    ' Module hmmsearchParser
    ' 
    '     Function: (+2 Overloads) __alignmentParser, HitParser, LoadDoc, QueryParser
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text.Parser

Public Module hmmsearchParser

    Public Function LoadDoc(path As String) As hmmsearch
        Dim lines As IEnumerable(Of String) = path.IterateAllLines
        Dim head As String() = lines.Take(10).ToArray
        Dim sections As IEnumerable(Of String()) = lines.Skip(10).Split("//")
        Dim query As PfamQuery() = sections.Select(AddressOf QueryParser).ToArray
        Dim searchResult As New hmmsearch With {
            .HMM = Mid(head(5), 22).Trim,
            .source = Mid(head(6), 32).Trim,
            .version = head(1),
            .Queries = query
        }

        Return searchResult
    End Function

    Private Function QueryParser(buf As String()) As PfamQuery
        Dim query As String = Mid(buf(Scan0), 7).Trim
        Dim accession As String = Mid(buf(1), 11).Trim
        Dim describ As String
        Dim offset As Integer
        Dim len As Integer = CInt(Regex.Match(query, "M=\d+", RegexICSng).Value.Split("="c).Last)

        If InStr(buf(2), "Description:") = 1 Then
            describ = Mid(buf(2), 13).Trim
            offset = 6
        Else
            describ = ""
            offset = 5
        End If

        If buf.Lookup(hmmscan.NoHits) <> -1 Then
            Return New PfamQuery With {
                .Query = query,
                .MLen = len,
                .Accession = accession,
                .describ = describ
            }
        End If

        Dim fields As Integer() = buf(offset).CrossFields
        Dim hits As New List(Of Score)
        Dim s As New Value(Of String)

        offset += 1

        Do While Not (s = buf.Read(offset)).StringEmpty AndAlso
            InStr(s, hmmscan.inclusion) = 0
            hits += s.value.HitParser(fields)
        Loop

        Dim uhits As New List(Of Score)

        Do While Not (s = buf.Read(offset)).StringEmpty
            uhits += s.value.HitParser(fields)
        Loop

        offset = buf.Lookup("Domain annotation for each sequence")

        Dim details As AlignmentHit()

        If offset = -1 Then
            details = New AlignmentHit() {}
        Else
            details = __alignmentParser(buf.Skip(offset + 1))
        End If

        For Each x As AlignmentHit In details
            x.QueryTag = query.Split.First
        Next

        Return New PfamQuery With {
            .Query = query,
            .MLen = len,
            .hits = hits.ToArray,
            .uncertain = uhits.ToArray,
            .Accession = accession,
            .describ = describ,
            .alignments = details
        }
    End Function

    Private Function __alignmentParser(buf As IEnumerable(Of String)) As AlignmentHit()
        Dim blocks As IEnumerable(Of String()) =
                buf.FlagSplit(Function(s) s.IndexOf(">>") = 0 OrElse s.IndexOf("Internal") = 0)
        Return blocks.Select(Function(x) __alignmentParser(x)).ToArray
    End Function

    Private Function __alignmentParser(buf As String()) As AlignmentHit
        Dim title As String = Mid(buf(Scan0), 3).Trim
        Dim fields As Integer() = buf(2).CrossFields
        Dim s As New Value(Of String)
        Dim aligns As New List(Of hmmscan.Align)
        Dim p As Integer = 3

        Do While Not (s = buf.Read(p)).StringEmpty
            aligns += New hmmscan.Align(s.value.FieldParser(fields))
        Loop

        Return New AlignmentHit With {
            .hits = aligns,
            .locus = title
        }
    End Function

    <Extension>
    Private Function HitParser(line As String, fields As Integer()) As Score
        Dim buf As String() = line.FieldParser(fields)
        Dim s1 As New hmmscan.Score(buf(1), buf(3), buf(5))
        Dim s2 As New hmmscan.Score(buf(7), buf(9), buf(11))
        Dim model As String = Trim(buf(17))
        Dim describ As String = Trim(buf(19))
        Dim N As Integer = CInt(Val(Trim(buf(15))))
        Dim exp As Double = Val(Trim(buf(13)))

        Return New Score With {
            .Full = s1,
            .Best = s2,
            .exp = exp,
            .N = N,
            .locus = model,
            .describ = describ
        }
    End Function
End Module

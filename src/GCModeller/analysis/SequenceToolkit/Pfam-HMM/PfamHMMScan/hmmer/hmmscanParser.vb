#Region "Microsoft.VisualBasic::8fc128572bab994cad0bb17f655073d4, ..\GCModeller\analysis\SequenceToolkit\Pfam-HMM\PfamHMMScan\hmmer\hmmscanParser.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
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

Imports System.Runtime.CompilerServices
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.Text
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Language

Namespace hmmscan

    Public Module hmmscanParser

        Public Function LoadDoc(path As String) As hmmscan
            Dim buf As String() = IO.File.ReadAllLines(path)
            Dim i As Integer
            Dim head As String() = buf.ReadHead(i, AddressOf UntilBlank)
            Dim blocks As IEnumerable(Of String()) = buf.Skip(i).Split("//")

            Return New hmmscan With {
                .version = head(1),
                .Querys = QueryParser(blocks).ToArray,
                .query = Mid(head(5), 22).Trim,
                .HMM = Mid(head(6), 22).Trim
            }
        End Function

        Private Iterator Function QueryParser(source As IEnumerable(Of String())) As IEnumerable(Of Query)
            For Each block As String() In source
                Yield block.QueryParser
            Next
        End Function

        Public Const inclusion As String = "------ inclusion threshold ------"
        Public Const NoHits As String = "[No hits detected that satisfy reporting thresholds]"

        <Extension>
        Private Function QueryParser(buf As String()) As Query
            Dim query As String = buf(Scan0)
            Dim len As Integer = Regex.Match(query, "L=\d+", RegexICSng).Value.Split("="c).Last

            If buf.Lookup(NoHits) <> -1 Then
                Return New Query With {
                    .name = Mid(query, 7).Trim,
                    .length = len
                }
            End If

            Dim fields As Integer() = buf(4).CrossFields
            Dim hits As New List(Of Hit)
            Dim offset As Integer = 5
            Dim s As New Value(Of String)

            Do While Not (s = buf.Read(offset)).StringEmpty AndAlso
                InStr(s, inclusion) = 0
                hits += s.value.HitParser(fields)
            Loop

            Dim uhits As New List(Of Hit)

            Do While Not (s = buf.Read(offset)).StringEmpty
                uhits += s.value.HitParser(fields)
            Loop

            offset = buf.Lookup("Domain annotation for each model")

            Dim details As Alignment()

            If offset = -1 Then
                details = New Alignment() {}
            Else
                details = __alignmentParser(buf.Skip(offset + 1))
            End If

            Return New Query With {
                .name = Mid(query, 7).Trim,
                .length = len,
                .Hits = hits.ToArray,
                .uncertain = uhits.ToArray,
                .Alignments = details
            }
        End Function

        Private Function __alignmentParser(buf As IEnumerable(Of String)) As Alignment()
            Dim blocks As IEnumerable(Of String()) =
                buf.FlagSplit(Function(s) s.IndexOf(">>") = 0 OrElse s.IndexOf("Internal") = 0)
            Return blocks.ToArray(Function(x) __alignmentParser(x))
        End Function

        Private Function __alignmentParser(buf As String()) As Alignment
            Dim title As String = Mid(buf(Scan0), 3).Trim
            Dim p As Integer = InStr(title, " ")
            Dim name As String = Mid(title, 1, p - 1)
            Dim describ As String = Mid(title, p + 1).Trim
            Dim fields As Integer() = buf(2).CrossFields
            Dim s As New Value(Of String)
            Dim aligns As New List(Of Align)

            p = 3

            Do While Not (s = buf.Read(p)).StringEmpty
                aligns += New Align(FormattedParser.FieldParser(s, fields))
            Loop

            Return New Alignment With {
                .Aligns = aligns,
                .describ = describ,
                .model = name
            }
        End Function

        ''' <summary>
        ''' 因为Pfam编号的长度和域长度的指示标识符之间的状态不是一致的，所以不能用现有的域解析器
        ''' </summary>
        ''' <param name="s"></param>
        ''' <param name="fields"></param>
        ''' <returns></returns>
        <Extension>
        Private Function FieldParser(s As String, fields As Integer()) As String()
            Dim tokens As String() = (From l As String
                                      In s.Split
                                      Where Not String.IsNullOrEmpty(l)
                                      Select l).ToArray
            tokens = tokens.Take(9).AsList +
                s.Substring(fields.Take(fields.Length - 1).Sum).Trim
            Return tokens
        End Function

        <Extension>
        Private Function HitParser(line As String, fields As Integer()) As Hit
            Dim buf As String() = line.FieldParser(fields)
            Dim s1 As New Score(buf(0), buf(1), buf(2))
            Dim s2 As New Score(buf(3), buf(4), buf(5))
            Dim model As String = Trim(buf(8))
            Dim describ As String = Trim(buf(9))
            Dim N As Integer = CInt(Val(Trim(buf(7))))
            Dim exp As Double = Val(Trim(buf(6)))

            Return New Hit With {
                .Full = s1,
                .Best = s2,
                .exp = exp,
                .N = N,
                .Model = model,
                .Description = describ
            }
        End Function
    End Module
End Namespace

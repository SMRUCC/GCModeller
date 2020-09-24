#Region "Microsoft.VisualBasic::037e1b097202a6d43c906cfcbc1bcd4e, analysis\SequenceToolkit\Pfam-HMM\PfamHMMScan\Pfam.hmm\HMMParser\HMMParserAPI.vb"

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

    ' Module HMMParserAPI
    ' 
    '     Function: __probability, LoadDoc, NodeParser, STATSParser, StreamParser
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text.Xml.Models

Public Module HMMParserAPI

    ''' <summary>
    ''' 加载Pfam-A.hmm里面的隐马科夫模型数据
    ''' </summary>
    ''' <param name="path">Pfam-A.hmm</param>
    ''' <returns></returns>
    Public Iterator Function LoadDoc(path As String) As IEnumerable(Of HMMParser)
        Dim reader As IEnumerable(Of String) = path.IterateAllLines
        Dim buffer As New List(Of String)

        VBDebugger.Mute = True

        For Each line As String In reader
            buffer.Add(line)

            If line = "//" Then
                Yield StreamParser(buffer.PopAll)
            End If
        Next

        If buffer > 0 Then
            Yield StreamParser(buffer.PopAll)
        End If

        VBDebugger.Mute = False
    End Function

    Public Function StreamParser(stream As String()) As HMMParser
        Dim fields As New Dictionary(Of String, String)
        Dim i As Integer
        Dim pos As Integer
        Dim s As String
        Dim key As String

        For i = 1 To stream.Length - 1
            s = stream(i)
            pos = InStr(s, " ")
            key = s.Substring(0, pos - 1)

            If String.Equals(key, "STATS") Then
                Exit For
            Else
                s = s.Substring(pos).Trim
            End If

            fields += New NamedValue With {
                .name = key,
                .text = s
            }
        Next

        Dim stats As STATS = STATSParser(stream(i), stream(i + 1), stream(i + 2))
        i += 3
        i += 2

        Dim blocks As String()() = stream.Skip(i).Split(3)

        Return New HMMParser With {
            .STATS = stats,
            .HMM = New HMM With {
                .COMPO = NodeParser(blocks(Scan0)),
                .nodes = blocks.Skip(1).Select(Function(block) NodeParser(block)).ToArray
            },
            .ACC = fields.TryGetValue(NameOf(HMMParser.ACC)),
            .ALPH = fields.TryGetValue(NameOf(HMMParser.ALPH)),
            .BM = fields.TryGetValue(NameOf(HMMParser.BM)),
            .CKSUM = CLng(fields.TryGetValue(NameOf(HMMParser.CKSUM))),
            .COM = fields.TryGetValue(NameOf(HMMParser.COM)),
            .CONS = fields.TryGetValue(NameOf(HMMParser.CONS)),
            .CS = fields.TryGetValue(NameOf(HMMParser.CS)),
            .DATE = fields.TryGetValue(NameOf(HMMParser.DATE)),
            .DESC = fields.TryGetValue(NameOf(HMMParser.DESC)),
            .EFFN = Val(fields.TryGetValue(NameOf(HMMParser.EFFN))),
            .GA = fields.TryGetValue(NameOf(HMMParser.GA)).Split.TrimNull.Select(Function(sg) Val(sg)).ToArray,
            .LENG = CInt(fields.TryGetValue(NameOf(HMMParser.LENG))),
            .MAP = fields.TryGetValue(NameOf(HMMParser.MAP)),
            .MAXL = CInt(fields.TryGetValue(NameOf(HMMParser.MAXL))),
            .MM = fields.TryGetValue(NameOf(HMMParser.MM)),
            .NAME = fields.TryGetValue(NameOf(HMMParser.NAME)),
            .NC = fields.TryGetValue(NameOf(HMMParser.NC)).Split.TrimNull.Select(Function(sg) Val(sg)).ToArray,
            .NSEQ = CInt(fields.TryGetValue(NameOf(HMMParser.NSEQ))),
            .RF = fields.TryGetValue(NameOf(HMMParser.RF)),
            .SM = fields.TryGetValue(NameOf(HMMParser.SM)),
            .TC = fields.TryGetValue(NameOf(HMMParser.TC)).Split.TrimNull.Select(Function(sg) Val(sg)).ToArray
        }
    End Function

    ''' <summary>
    ''' All probability parameters are all stored As negative natural log probabilities With five digits Of precision To
    ''' the right Of the Decimal point, rounded. For example, a probability Of 0:25 Is stored as 􀀀log 0:25 = 1:38629.
    ''' The special Case Of a zero probability Is stored As '*’.
    ''' </summary>
    ''' <param name="x"></param>
    ''' <returns></returns>
    Private Function __probability(x As String) As Double
        If x = "*" Then  ' The special Case Of a zero probability Is stored As '*’.
            Return 0R
        Else
            Return Math.E ^ (-Val(x)) ' stored As negative natural log probabilities
        End If
    End Function

    ''' <summary>
    ''' 一个氨基酸残基
    ''' </summary>
    ''' <param name="block"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' 因为ln1=0，有些时候是0概率的，ln0会计算不出来，这个时候使用*代替
    ''' </remarks>
    Public Function NodeParser(block As String()) As Node
        Dim m As String() = block(0).Split.TrimNull
        Dim i As String() = block(1).Split.TrimNull
        Dim s As String() = block(2).Split.TrimNull

        If String.Equals(m(Scan0), "COMPO") Then
            ' 后面没有东西的
            Return New Node With {
                .Address = 0,
                .Match = m.Skip(1).Select(AddressOf __probability).ToArray,
                .Insert = i.Select(AddressOf __probability).ToArray,
                .StateTransitions = s.Select(AddressOf __probability).ToArray
            }
        Else
            Dim addr As Long = Scripting.CTypeDynamic(Of Long)(m(Scan0))
            Return New Node With {
                .Address = addr,
                .Match = m.Skip(1).Take(20).Select(AddressOf __probability).ToArray,
                .Insert = i.Select(AddressOf __probability).ToArray,
                .StateTransitions = s.Select(AddressOf __probability).ToArray
            }
        End If
    End Function

    Public Function STATSParser(msv As String, viterbi As String, forwards As String) As STATS
        msv = msv.Substring(16).Trim
        viterbi = viterbi.Substring(21).Trim
        forwards = forwards.Substring(21).Trim

        Return New STATS With {
            .MSV = msv.Split.Select(Function(s) Val(s)).ToArray,
            .VITERBI = viterbi.Split.Select(Function(s) Val(s)).ToArray,
            .FORWARD = forwards.Split.Select(Function(s) Val(s)).ToArray
        }
    End Function
End Module

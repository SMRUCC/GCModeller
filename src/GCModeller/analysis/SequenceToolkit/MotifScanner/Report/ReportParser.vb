#Region "Microsoft.VisualBasic::a097e9dd605bade36e68e2103430e738, GCModeller\analysis\SequenceToolkit\MotifScanner\Report\ReportParser.vb"

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


    ' Code Statistics:

    '   Total Lines: 90
    '    Code Lines: 77
    ' Comment Lines: 0
    '   Blank Lines: 13
    '     File Size: 3.39 KB


    ' Module ReportParser
    ' 
    '     Function: component, ParseReport, ParseSectionData, tfbs
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq

Public Module ReportParser

    Public Iterator Function ParseReport(text As String) As IEnumerable(Of GeneReport)
        For Each section As String() In text _
            .LineTokens _
            .Split(
                delimiter:=Function(l) l.FirstOrDefault = ">"c,
                deliPosition:=DelimiterLocation.NextFirst
            ) _
            .Where(Function(sec)
                       Return Not sec.IsNullOrEmpty
                   End Function)

            Yield ParseSectionData(section)
        Next
    End Function

    Private Function ParseSectionData(lines As String()) As GeneReport
        Dim id As String = lines(Scan0).Substring(1).Trim
        Dim len As Integer = lines(1).Split("-"c).Last.Trim.DoCall(AddressOf Integer.Parse)
        Dim threshold As Double = lines(2).Split("-"c).Last.Trim.DoCall(AddressOf Double.Parse)
        Dim n As Integer = lines(3).Split("-"c).Last.Trim.DoCall(AddressOf Integer.Parse)

        If n = 0 Then
            Return New GeneReport With {
                .locus_tag = id,
                .length = len,
                .threshold = threshold
            }
        End If

        Dim tokens As String() = lines(4).Split("-"c)
        Dim pos As Integer = tokens(Scan0).Split(":"c).Last.Trim.Match("\d+").DoCall(AddressOf Integer.Parse)
        Dim LDF As Double = tokens(1).Trim.DoCall(AddressOf Double.Parse)
        Dim i As i32 = 5
        Dim components As PromoterComponent() = lines.component(i).ToArray
        Dim tfbs As TFBindingSite() = lines.tfbs(i).ToArray

        Return New GeneReport With {
            .locus_tag = id,
            .length = len,
            .threshold = threshold,
            .numOfPromoters = n,
            .promoterPos = pos,
            .promoterPosLDF = LDF,
            .components = components,
            .tfBindingSites = tfbs
        }
    End Function

    <Extension>
    Private Iterator Function tfbs(lines As String(), i As i32) As IEnumerable(Of TFBindingSite)
        Dim tokens As String()

        For Each line As String In lines.Skip(i + 3)
            tokens = line.Trim.StringSplit("\s+")

            Yield New TFBindingSite With {
                .regulator = tokens(Scan0).Trim(" "c, ":"c),
                .oligonucleotides = tokens(1),
                .position = tokens(4).DoCall(AddressOf Integer.Parse),
                .score = tokens.Last.DoCall(AddressOf Double.Parse)
            }
        Next
    End Function

    <Extension>
    Private Iterator Function component(lines As String(), i As i32) As IEnumerable(Of PromoterComponent)
        Dim line As Value(Of String) = ""
        Dim tokens As String()
        Dim type As String

        Do While Not (line = lines(++i)).StringEmpty
            tokens = Strings.Split(line, "at pos.")
            type = tokens(Scan0).Trim
            tokens = tokens(1).StringSplit("\s+")

            Yield New PromoterComponent With {
                .type = type,
                .pos = tokens(1).DoCall(AddressOf Integer.Parse),
                .oligonucleotides = tokens(2),
                .score = tokens(4).DoCall(AddressOf Double.Parse)
            }
        Loop
    End Function
End Module

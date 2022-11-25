#Region "Microsoft.VisualBasic::375b22ce2d48d638584afdeb4e348313, GCModeller\analysis\SequenceToolkit\SmithWaterman\Extension\Output.vb"

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

    '   Total Lines: 108
    '    Code Lines: 69
    ' Comment Lines: 26
    '   Blank Lines: 13
    '     File Size: 4.21 KB


    ' Class Output
    ' 
    '     Properties: Best, Directions, DP, HSP, Query
    '                 Subject, Traceback
    ' 
    '     Function: ContainsPoint, CreateObject, ToString
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Linq
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.Algorithm.DynamicProgramming.Levenshtein
Imports Microsoft.VisualBasic.ComponentModel.Algorithm.DynamicProgramming.Levenshtein.LevenshteinDistance
Imports Microsoft.VisualBasic.DataMining.DynamicProgramming.SmithWaterman
Imports Microsoft.VisualBasic.Linq.Extensions
Imports Microsoft.VisualBasic.Text.Xml.Models

<XmlType("GSW", [Namespace]:=SMRUCC.genomics.LICENSE.GCModeller)>
Public Class Output

    ''' <summary>
    ''' best chain, 但是不明白这个有什么用途
    ''' </summary>
    ''' <returns></returns>
    Public Property Best As HSP

    ''' <summary>
    ''' 最佳的比对结果
    ''' </summary>
    ''' <returns></returns>
    Public Property HSP As HSP()
    ''' <summary>
    ''' Dynmaic programming matrix.(也可以看作为得分矩阵)
    ''' </summary>
    ''' <returns></returns>
    Public Property DP As ArrayRow()
    ''' <summary>
    ''' The directions pointing to the cells that
    ''' give the maximum score at the current cell.
    ''' The first index is the column index.
    ''' The second index is the row index.
    ''' </summary>
    ''' <returns></returns>
    Public Property Directions As ArrayRow()

    Public Property Query As String
    Public Property Subject As String
    Public Property Traceback As Coordinate()

    Public Function ContainsPoint(i As Integer, j As Integer) As Boolean
        Dim LQuery = (From x In Traceback Where x.X = i AndAlso x.Y = j Select 100).FirstOrDefault
        Return LQuery > 50
    End Function

    Public Overrides Function ToString() As String
        Dim edits As String = ""
        Dim pre = Traceback.First

        For Each cd As Coordinate In Traceback.Skip(1)
            If cd.X - pre.X = -1 AndAlso cd.Y - pre.Y = -1 Then
                edits &= "m" '  match 和 substitute应该如何进行判断？？？
            End If
            If cd.X - pre.X = 0 AndAlso cd.Y - pre.Y = -1 Then
                edits &= "i"
            End If
            If cd.X - pre.X = -1 AndAlso cd.Y - pre.Y = 0 Then
                edits &= "d"
            End If

            pre = cd
        Next

        Return edits
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="sw"></param>
    ''' <param name="threshold">0% - 100%</param>
    ''' <returns></returns>
    Public Shared Function CreateObject(Of T)(sw As GSW(Of T), threshold As Double, minW As Integer) As Output
        Dim hspList = sw.CreateHSP(cutoff:=threshold * sw.AlignmentScore).ToArray
        Dim direction = sw.prevCells.Select(Function(x) New ArrayRow(x)).ToArray
        Dim toChar As Func(Of T, Char) = AddressOf sw.symbol.ToChar
        Dim dp = sw.GetDPMAT.Select(Function(x) New ArrayRow(x)).ToArray
        Dim query = sw.query.Select(Function(x) toChar(x)).CharString
        Dim subject = sw.subject.Select(Function(x) toChar(x)).CharString
        Dim m2Len As Integer = Math.Min(query.Length, subject.Length)
        Dim best As HSP = SequenceTools.HSP.CreateFrom(hspList.GetBestAlignment, toChar)

        If m2Len < minW Then
            Call $"Min width {minW} is too large than query/subject, using min(query,subject):={m2Len} instead....".__DEBUG_ECHO
            minW = m2Len
        End If

        hspList = (From x In hspList Where x.LengthHit >= minW AndAlso x.LengthQuery >= minW Select x).ToArray

        Return New Output With {
            .Traceback = sw.GetTraceback,
            .Directions = direction,
            .DP = dp,
            .HSP = hspList _
                .Select(Function(h)
                            Dim hsp = SequenceTools.HSP.CreateFrom(h, AddressOf sw.symbol.ToChar)
                            hsp.QueryLength = query.Length
                            hsp.SubjectLength = subject.Length
                            Return hsp
                        End Function) _
                .ToArray,
            .Query = query,
            .Subject = subject,
            .Best = best
        }
    End Function
End Class

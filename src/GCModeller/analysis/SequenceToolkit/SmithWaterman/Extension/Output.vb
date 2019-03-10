﻿#Region "Microsoft.VisualBasic::5e40557f7da6924fa8b34239166fc092, analysis\SequenceToolkit\SmithWaterman\Extension\Output.vb"

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
Imports Microsoft.VisualBasic.DataMining.DynamicProgramming.SmithWaterman
Imports Microsoft.VisualBasic.Linq.Extensions
Imports Microsoft.VisualBasic.Net.Protocols
Imports Microsoft.VisualBasic.Text.Levenshtein.LevenshteinDistance
Imports Microsoft.VisualBasic.Text.Xml.Models

<XmlType("GSW", [Namespace]:="http://gcmodeller.org")>
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
    Public Property DP As Streams.Array.Double()
    ''' <summary>
    ''' The directions pointing to the cells that
    ''' give the maximum score at the current cell.
    ''' The first index is the column index.
    ''' The second index is the row index.
    ''' </summary>
    ''' <returns></returns>
    Public Property Directions As Streams.Array.Integer()

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
    ''' <param name="toChar"></param>
    ''' <param name="threshold">0% - 100%</param>
    ''' <returns></returns>
    Public Shared Function CreateObject(Of T)(sw As GSW(Of T), toChar As ToChar(Of T), threshold As Double, minW As Integer) As Output
        Dim best As HSP = Nothing
        Dim hsp = SequenceTools.HSP.CreateHSP(sw, toChar, best, cutoff:=threshold * sw.AlignmentScore)
        Dim direction = sw.prevCells.Select(Function(x) New Streams.Array.Integer(x)).ToArray
        Dim dp = sw.GetDPMAT.Select(Function(x) New Streams.Array.Double(x)).ToArray
        Dim query = sw.query.Select(Function(x) toChar(x)).CharString
        Dim subject = sw.subject.Select(Function(x) toChar(x)).CharString

        Dim m2Len As Integer = Math.Min(query.Length, subject.Length)
        If m2Len < minW Then
            Call $"Min width {minW} is too large than query/subject, using min(query,subject):={m2Len} instead....".__DEBUG_ECHO
            minW = m2Len
        End If
        hsp = (From x In hsp Where x.LengthHit >= minW AndAlso x.LengthQuery >= minW Select x).ToArray

        Return New Output With {
            .Traceback = sw.GetTraceback,
            .Directions = direction,
            .DP = dp,
            .HSP = hsp,
            .Query = query,
            .Subject = subject,
            .Best = best
        }
    End Function
End Class

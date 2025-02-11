﻿#Region "Microsoft.VisualBasic::045cc257430ceb757fd23a70ecedff77, analysis\SequenceToolkit\MotifFinder\Gibbs\Utils.vb"

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

    '   Total Lines: 41
    '    Code Lines: 35 (85.37%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 6 (14.63%)
    '     File Size: 2.02 KB


    ' Class Utils
    ' 
    '     Function: calcInformationContent, getSequenceFromPair, getSiteFromPair, indexOfBase
    ' 
    ' /********************************************************************************/

#End Region

Public Class Utils

    Public Shared ReadOnly ACGT As String() = New String() {"A", "C", "G", "T"}

    Shared ReadOnly LOG_2 As Double = Math.Log(2)

    Friend Shared Function getSequenceFromPair(sequences As IList(Of KeyValuePair(Of String, Integer))) As IList(Of String)
        Return sequences.[Select](Function(a) a.Key).ToList()
    End Function

    Friend Shared Function getSiteFromPair(sequences As IList(Of KeyValuePair(Of String, Integer))) As List(Of Integer)
        Return sequences.[Select](Function(a) a.Value).ToList()
    End Function

    Public Shared Function indexOfBase(base As Char) As Integer
        Select Case base
            Case "A"c
                Return 0
            Case "C"c
                Return 1
            Case "G"c
                Return 2
            Case "T"c
                Return 3
            Case Else
                Console.WriteLine("Unknown Base")
                Return -1
        End Select
    End Function

    Public Shared Function calcInformationContent(countSum As Double, counts As Integer()) As Double
        Dim probabilities As Double() = counts.[Select](Function(i) i / countSum).ToArray()
        Return Enumerable.Select(Of Double, Global.System.[Double])(probabilities, CType(Function(p)
                                                                                             If p = 0 Then
                                                                                                 Return CDbl(0)
                                                                                             Else
                                                                                                 Return p * (Math.Log(p / 0.25) / LOG_2)
                                                                                             End If
                                                                                         End Function, Func(Of Double, Double))).Sum()
    End Function
End Class


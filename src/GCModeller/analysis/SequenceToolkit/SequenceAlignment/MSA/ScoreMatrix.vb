#Region "Microsoft.VisualBasic::ff29406ac4c3712de319862fffd07503, analysis\SequenceToolkit\MSA\ScoreMatrix.vb"

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

    '   Total Lines: 44
    '    Code Lines: 36 (81.82%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 8 (18.18%)
    '     File Size: 1.80 KB


    ' Class ScoreMatrix
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: DefaultMatrix, getScore, LoadFile, MatrixParser
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Algorithm.DynamicProgramming
Imports Microsoft.VisualBasic.Language.Default

Namespace MSA

    Public Class ScoreMatrix : Implements IScore(Of Char)

        Dim matrix As Dictionary(Of Char, Dictionary(Of Char, Double))

        Private Sub New()
        End Sub

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function getScore(i As Char, j As Char) As Double Implements IScore(Of Char).GetSimilarityScore
            Return matrix(i)(j)
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Function LoadFile(filePath As String) As ScoreMatrix
            Return MatrixParser(filePath.ReadAllLines)
        End Function

        Public Shared Function MatrixParser(data As String()) As ScoreMatrix
            Dim chars = data.Select(Function(l) l.Split) _
                .GroupBy(Function(t) t(Scan0)) _
                .ToArray
            Dim tuples As Dictionary(Of Char, Dictionary(Of Char, Double)) = chars _
                .ToDictionary(Function(c) CChar(c.Key),
                              Function(scores)
                                  Return scores.ToDictionary(Function(line) CChar(line(1)),
                                                             Function(line)
                                                                 Return Val(line(2))
                                                             End Function)
                              End Function)

            Return New ScoreMatrix With {.matrix = tuples}
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Function DefaultMatrix() As [Default](Of ScoreMatrix)
            Return MatrixParser(My.Resources.MSAData.Matrix.LineTokens)
        End Function
    End Class
End Namespace
#Region "Microsoft.VisualBasic::72440eb8a3317a403a2d8047f6c92529, GCModeller\analysis\SequenceToolkit\MSA\ScoreMatrix.vb"

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

    '   Total Lines: 39
    '    Code Lines: 31
    ' Comment Lines: 0
    '   Blank Lines: 8
    '     File Size: 1.44 KB


    ' Class ScoreMatrix
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: DefaultMatrix, getScore, LoadFile, MatrixParser
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Language.Default

Public Class ScoreMatrix

    Dim matrix As Dictionary(Of Char, Dictionary(Of Char, Double))

    Private Sub New()
    End Sub

    Public Function getScore(i As Char, j As Char) As Double
        Return matrix(i)(j)
    End Function

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

        Return New ScoreMatrix With {
            .matrix = tuples
        }
    End Function

    Public Shared Function DefaultMatrix() As [Default](Of ScoreMatrix)
        Return MatrixParser(My.Resources.Matrix.LineTokens)
    End Function
End Class

#Region "Microsoft.VisualBasic::7ddc5d7e5ecabf3ca87f1cd4ec37eb7d, analysis\SequenceToolkit\MotifScanner\Scanner\ZERO.vb"

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

    '   Total Lines: 45
    '    Code Lines: 33 (73.33%)
    ' Comment Lines: 3 (6.67%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 9 (20.00%)
    '     File Size: 1.48 KB


    ' Class ZERO
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: NextSequence
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.Algorithm.base
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports randf = Microsoft.VisualBasic.Math.RandomExtensions

Public Class ZERO

    ReadOnly nucleotides As Char()
    ReadOnly cumulativeProbs As IReadOnlyCollection(Of Double)

    Sub New(background As Dictionary(Of Char, Double))
        Dim cumulativeProbs As New List(Of Double)()
        Dim nucleotides As Char() = background.Keys.ToArray
        ' 构建累积概率分布
        Dim cumulative As Double = 0
        For Each NT As Char In background.Keys
            cumulative += background(NT)
            cumulativeProbs.Add(cumulative)
        Next

        Me.nucleotides = nucleotides
        Me.cumulativeProbs = cumulativeProbs
    End Sub

    Public Function NextSequence(length As Integer) As String
        ' 生成随机序列
        Dim sequence As Char() = New Char(length - 1) {}

        For i As Integer = 1 To length
            Dim rndValue As Double = randf.NextDouble()

            ' 选择核苷酸
            For j As Integer = 0 To nucleotides.Length - 1
                If rndValue <= cumulativeProbs(j) Then
                    sequence(i - 1) = nucleotides(j)
                    Exit For
                End If
            Next
        Next

        Return New String(sequence)
    End Function

End Class

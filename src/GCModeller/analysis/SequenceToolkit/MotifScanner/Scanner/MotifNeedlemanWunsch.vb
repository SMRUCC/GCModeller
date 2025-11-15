#Region "Microsoft.VisualBasic::e588d996d78b09ded8f7483ebb6dab6a, analysis\SequenceToolkit\MotifFinder\Scanner\MotifNeedlemanWunsch.vb"

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

    '   Total Lines: 52
    '    Code Lines: 42 (80.77%)
    ' Comment Lines: 1 (1.92%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 9 (17.31%)
    '     File Size: 1.80 KB


    ' Class MotifNeedlemanWunsch
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: defaultScoreMatrix, equalsTo, symbolProvider
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.DataMining.DynamicProgramming
Imports Microsoft.VisualBasic.DataMining.DynamicProgramming.NeedlemanWunsch

Public Class MotifNeedlemanWunsch : Inherits NeedlemanWunsch(Of Residue)

    Sub New(query As Residue(), subject As Residue(), score As ResidueScore)
        Call MyBase.New(defaultScoreMatrix, symbolProvider(score))

        Me.Sequence1 = query
        Me.Sequence2 = subject
    End Sub

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Private Shared Function symbolProvider(score As ResidueScore) As GenericSymbol(Of Residue)
        Return New GenericSymbol(Of Residue)(
            equals:=Function(a, b) a.topChar = b.topChar,
            similarity:=Function(a, b) score.Cos(a, b),
            toChar:=Function(x) x.topChar,
            empty:=Function()
                       Return Residue.GetEmpty
                   End Function
        )
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Shared Function defaultScoreMatrix() As ScoreMatrix(Of Residue)
        Return New ScoreMatrix(Of Residue)(AddressOf equalsTo) With {.MatchScore = 10}
    End Function

    Private Shared Function equalsTo(a As Residue, b As Residue) As Boolean
        Dim maxA As Char = Residue.Max(a)
        Dim maxB As Char = Residue.Max(b)

        If a.isEmpty OrElse b.isEmpty Then
            Return False
        End If

        If maxA = maxB Then
            Return True
        Else
            ' A是motif模型，所以不一致的时候以A为准
            Dim freqB As Double = b(maxA)

            ' 最大的值是小于0.3，说明这个位点是随机分布的
            ' 即 0.25 0.25 0.25 0.25
            ' 是空白还是可以看作为可以匹配上任意符号？
            If freqB < 0.3 Then
                Return True
            Else
                ' 不是一个随机的分布，是一个比较明确的符号
                Return False
            End If
        End If
    End Function
End Class

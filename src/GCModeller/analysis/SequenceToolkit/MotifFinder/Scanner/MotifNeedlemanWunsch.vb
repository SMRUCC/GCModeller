﻿#Region "Microsoft.VisualBasic::6ac5466cec8da850e3ce15644babc860, analysis\SequenceToolkit\MotifScanner\MotifNeedlemanWunsch.vb"

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

    ' Class MotifNeedlemanWunsch
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: defaultScoreMatrix, symbolProvider
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.DataMining.DynamicProgramming
Imports Microsoft.VisualBasic.DataMining.DynamicProgramming.NeedlemanWunsch

Public Class MotifNeedlemanWunsch : Inherits NeedlemanWunsch(Of Residue)

    Sub New(query As Residue(), subject As Residue(), score As ResidueScore)
        Call MyBase.New(defaultScoreMatrix, symbolProvider(score))

        Me.Sequence1 = query
        Me.Sequence2 = subject
    End Sub

    Private Shared Function symbolProvider(score As ResidueScore) As GenericSymbol(Of Residue)
        Return New GenericSymbol(Of Residue)(
            equals:=Function(a, b) a.topChar = b.topChar,
            similarity:=Function(a, b) score.Cos(a, b),
            toChar:=Function(x) x.topChar,
            empty:=Function()
                       Return New Residue With {
                           .frequency = New Dictionary(Of Char, Double)
                       }
                   End Function
        )
    End Function

    Public Shared Function defaultScoreMatrix() As ScoreMatrix(Of Residue)
        Return New ScoreMatrix(Of Residue)(
            Function(a, b)
                Dim maxA = Residue.Max(a)
                Dim maxB = Residue.Max(b)

                If a.isEmpty OrElse b.isEmpty Then
                    Return False
                End If

                If maxA = maxB Then
                    Return True
                Else
                    ' A是motif模型，所以不一致的时候以A为准
                    Dim freqB = b(maxA)

                    If freqB < 0.3 Then
                        Return False
                    Else
                        Return True
                    End If
                End If
            End Function) With {.MatchScore = 10}
    End Function
End Class
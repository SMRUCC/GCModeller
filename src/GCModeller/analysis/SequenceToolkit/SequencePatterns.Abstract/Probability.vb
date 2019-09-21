#Region "Microsoft.VisualBasic::457ae8438a9ae3473560229722de686d, analysis\SequenceToolkit\SequencePatterns.Abstract\Probability.vb"

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

    ' Class Probability
    ' 
    '     Properties: pvalue, region, score
    ' 
    '     Function: ToString
    '     Structure Residue
    ' 
    '         Properties: frequency, index
    ' 
    '         Function: Max, ToString
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq

Public Class Probability

    Public Property region As Residue()
    Public Property pvalue As Double
    Public Property score As Double

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Overrides Function ToString() As String
        Return region _
            .Select(Function(r) r.ToString) _
            .JoinBy("") & $" @ {score}, pvalue={pvalue.ToString("G4")}"
    End Function

    Public Structure Residue

        Public Property frequency As Dictionary(Of Char, Double)
        Public Property index As Integer

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Overrides Function ToString() As String
            Return Max(Me)
        End Function

        Public Shared Function Max(r As Residue) As Char
            With r.frequency.ToArray
                If .Values.All(Function(p) p = 0R) Then
                    Return "-"c
                Else
                    Return .ByRef(Which.Max(.Values)) _
                           .Key
                End If
            End With
        End Function
    End Structure
End Class

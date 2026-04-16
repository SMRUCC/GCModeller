#Region "Microsoft.VisualBasic::851b8493294afb9fb8457439f0c23c08, analysis\SequenceToolkit\MotifScanner\Scanner\NullTest.vb"

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

    '   Total Lines: 38
    '    Code Lines: 29 (76.32%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 9 (23.68%)
    '     File Size: 1.20 KB


    ' Class NullTest
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: score, Score, ZeroSet
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Math.Statistics.Hypothesis

Public Class NullTest : Inherits NullHypothesis(Of String)

    ReadOnly length As Integer
    ReadOnly zero As ZERO
    ReadOnly motifSlice As Residue()

    Sub New(zero As ZERO, motifSlice As Residue(), length As Integer, Optional permutation As Integer = 1000)
        Call MyBase.New(permutation)

        Me.motifSlice = motifSlice
        Me.zero = zero
        Me.length = length
    End Sub

    Public Overrides Iterator Function ZeroSet() As IEnumerable(Of String)
        For i As Integer = 1 To Permutation
            Yield zero.NextSequence(length)
        Next
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Overrides Function Score(x As String) As Double
        Return score(x.ToCharArray, motifSlice)
    End Function

    Friend Overloads Shared Function score(seq As String, PWM As IReadOnlyCollection(Of Residue)) As Double
        Dim total As Double = 0

        For i As Integer = 0 To seq.Length - 1
            total += PWM(i)(seq(i))
        Next

        Return total
    End Function
End Class

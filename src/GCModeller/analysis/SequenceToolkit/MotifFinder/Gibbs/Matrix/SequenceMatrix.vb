﻿#Region "Microsoft.VisualBasic::e056498e22e886986a5d80257dd06c13, analysis\SequenceToolkit\MotifFinder\Gibbs\Matrix\SequenceMatrix.vb"

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

    '   Total Lines: 40
    '    Code Lines: 27 (67.50%)
    ' Comment Lines: 7 (17.50%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 6 (15.00%)
    '     File Size: 1.91 KB


    '     Class SequenceMatrix
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '         Function: probability
    ' 
    '         Sub: initSequenceMatrix
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace Matrix

    Public Class SequenceMatrix : Inherits WeightMatrix

        Private ReadOnly sequences As IList(Of String)
        Private ReadOnly sequenceCount As Integer
        Private ReadOnly sequenceLength As Integer

        Public Sub New(sequences As IList(Of String))
            Me.sequences = sequences
            sequenceCount = sequences.Count
            rowSum = sequenceCount
            sequenceLength = sequences(0).Length
            MyBase.initMatrix(sequenceLength)
            initSequenceMatrix()
        End Sub

        ''' <summary>
        ''' Counts the occurrences of each base along each position of each sequence
        ''' </summary>
        Private Sub initSequenceMatrix()
            Enumerable.Range(0, sequenceCount).ForEach(Sub(i, z)
                                                           Dim sequence = sequences(i)
                                                           Enumerable.Range(0, sequenceLength).ForEach(Sub(j, y)
                                                                                                           Dim b = Utils.indexOfBase(sequence(j))
                                                                                                           countsMatrix(j)(b) += 1
                                                                                                       End Sub)
                                                       End Sub)
        End Sub

        ''' <summary>
        ''' Returns the probability of seeing the base in the index </summary>
        ''' <param name="index">, index of base </param>
        ''' <param name="base">, base in the index </param>
        Public Overridable Function probability(index As Integer, base As Integer) As Double
            Return countsMatrix(index)(base) / rowSum
        End Function
    End Class

End Namespace

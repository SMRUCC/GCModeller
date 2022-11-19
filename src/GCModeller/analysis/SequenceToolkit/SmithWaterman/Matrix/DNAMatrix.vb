#Region "Microsoft.VisualBasic::44e3c30df96fc9847d64570719056a3a, GCModeller\analysis\SequenceToolkit\SmithWaterman\Matrix\DNAMatrix.vb"

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

    '   Total Lines: 46
    '    Code Lines: 30
    ' Comment Lines: 10
    '   Blank Lines: 6
    '     File Size: 1.25 KB


    ' Class DNAMatrix
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: getIndex
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.Collection

Public Class DNAMatrix : Inherits Blosum

    ' In general, matches are assigned positive scores, And mismatches are 
    ' assigned relatively lower scores. 
    ' Take DNA sequence As an example. If matches Get +1, mismatches Get -1, 
    ' Then the substitution matrix Is:

    '    A	 G	 C	 T  *
    ' A	 1	-1	-1	-1  0
    ' G	-1	 1	-1	-1  0
    ' C	-1	-1	 1	-1  0
    ' T	-1	-1	-1	 1  0
    ' *  0   0   0   0  0

    Sub New()
        Call MyBase.New({"A", "G", "C", "T", "*"})

        matrix = {
            {+1, -1, -1, -1, 0},
            {-1, +1, -1, -1, 0},
            {-1, -1, +1, -1, 0},
            {-1, -1, -1, +1, 0},
            {+0, +0, +0, +0, 0}
        }.ToVectorList _
         .ToArray
    End Sub

    Protected Overrides Function getIndex(a As Char) As Integer
        Select Case Char.ToUpper(a)
            Case "A"c
                Return 0
            Case "G"c
                Return 1
            Case "C"c
                Return 2
            Case "T"c
                Return 3
            Case "*"c, "-"c, "N"c
                Return 4
            Case Else
                Throw New InvalidCastException(a)
        End Select
    End Function
End Class

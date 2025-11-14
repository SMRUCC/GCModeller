#Region "Microsoft.VisualBasic::d56d192101878673bad7418c23c36350, analysis\SequenceToolkit\MotifFinder\Gibbs\Matrix\WeightMatrix.vb"

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

    '   Total Lines: 55
    '    Code Lines: 42 (76.36%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 13 (23.64%)
    '     File Size: 1.97 KB


    '     Class WeightMatrix
    ' 
    '         Constructor: (+2 Overloads) Sub New
    ' 
    '         Function: listsToArrays, ToString
    ' 
    '         Sub: initMatrix, SetCountsMatrixProp
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection

Namespace Matrix

    Public Class WeightMatrix

        Friend rowSum As Integer = 1000000
        Friend countsMatrix As Integer()()

        Friend Sub New(countsLists As List(Of List(Of Integer)))
            SetCountsMatrixProp(listsToArrays(countsLists))
        End Sub

        Friend Sub New()
        End Sub

        Private Function listsToArrays(countsLists As List(Of List(Of Integer))) As Integer()()
            Dim countMatrix = RectangularArray.Matrix(Of Integer)(countsLists.Count, countsLists(0).Count)
            Dim rows As List(Of Integer()) = countsLists.Select(Function(r) r.ToArray()).ToList()

            For i As Integer = 0 To countsLists.Count - 1
                countMatrix(i) = rows(i)
            Next

            Return countMatrix
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Friend Overridable Sub initMatrix(rows As Integer)
            countsMatrix = RectangularArray.Matrix(Of Integer)(rows, 4)
        End Sub

        Private Sub SetCountsMatrixProp(value As Integer()())
            Dim sumByRow As List(Of Integer) = Enumerable.Range(0, value.Length) _
                .Select(Function(i)
                            Return value(i).Sum()
                        End Function) _
                .ToList()
            Dim sum As Integer = sumByRow(0)

            countsMatrix = value
            rowSum = sum
        End Sub

        Public Overrides Function ToString() As String
            Return countsMatrix _
                .Select(Function(b)
                            Return String.Format(CStr("{0:D} {1:D} {2:D} {3:D}"), CObj(b(CInt(0))), CObj(b(CInt(1))), CObj(b(CInt(2))), CObj(b(CInt(3))))
                        End Function) _
                .JoinBy(vbLf)
        End Function
    End Class

End Namespace

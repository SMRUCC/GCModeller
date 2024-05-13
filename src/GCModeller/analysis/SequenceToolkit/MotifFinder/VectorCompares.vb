#Region "Microsoft.VisualBasic::c09ff0dac72147a120bd5ee1be3dd4d3, analysis\SequenceToolkit\MotifFinder\VectorCompares.vb"

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

    '   Total Lines: 24
    '    Code Lines: 18
    ' Comment Lines: 0
    '   Blank Lines: 6
    '     File Size: 681 B


    ' Class VectorCompares
    ' 
    '     Function: Compare, GetVector
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports SMRUCC.genomics.Model.MotifGraph

Public Class VectorCompares

    ReadOnly cache As New Dictionary(Of String, Vector)

    Public Function Compare(q$, s$) As Double
        Dim g1 = GetVector(q)
        Dim g2 = GetVector(s)
        Dim score As Double = SSM(g1, g2)

        Return score
    End Function

    Private Function GetVector(s As String) As Vector
        If Not cache.ContainsKey(s) Then
            cache.Add(s, Builder.SequenceGraph(s, SequenceModel.NT).GetVector(SequenceModel.NT))
        End If

        Return cache(s)
    End Function
End Class

#Region "Microsoft.VisualBasic::ce5487e04665ba38059f633d0f308fc8, GCModeller\analysis\HTS_matrix\Math.vb"

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

    '   Total Lines: 27
    '    Code Lines: 21
    ' Comment Lines: 3
    '   Blank Lines: 3
    '     File Size: 858 B


    ' Module Math
    ' 
    '     Function: AsNumeric, rowSds
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Math

''' <summary>
''' math helper for HTS matrix
''' </summary>
Public Module Math

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <Extension>
    Public Function rowSds(expr As Matrix) As Dictionary(Of String, Double)
        Return expr.expression _
            .ToDictionary(Function(g) g.geneID,
                          Function(g)
                              Return g.experiments.SD
                          End Function)
    End Function

    <DebuggerStepThrough>
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <Extension>
    Public Function AsNumeric(m As Matrix) As Double()()
        Return (From t As DataFrameRow
                In m.expression
                Select t.experiments).ToArray
    End Function
End Module

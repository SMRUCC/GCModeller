#Region "Microsoft.VisualBasic::26e74c985c57d445c8eca3e68912594d, engine\Cella\Environment.vb"

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

    '   Total Lines: 30
    '    Code Lines: 23 (76.67%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 7 (23.33%)
    '     File Size: 770 B


    ' Class Environment
    ' 
    '     Properties: Space
    ' 
    '     Function: GetAllCells
    ' 
    '     Sub: (+2 Overloads) Tick
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq

Public Class Environment

    Public Property Space As Spot()()()

    Dim itr As i32

    Public Sub Tick()
        Call Tick(++itr)
    End Sub

    Private Sub Tick(iteration As Integer)
        For Each row In Space
            For Each col In row
                For Each spot In col
                    Call spot.Tick(iteration)
                Next
            Next
        Next
    End Sub

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function GetAllCells() As IEnumerable(Of VirtualCella)
        Return Space.IteratesALL.IteratesALL.SelectMany(Function(s) s.cells)
    End Function

End Class


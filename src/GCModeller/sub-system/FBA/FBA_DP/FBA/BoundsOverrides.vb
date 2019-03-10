#Region "Microsoft.VisualBasic::460f1718bc293c511e5061624924ddd3, sub-system\FBA\FBA_DP\FBA\BoundsOverrides.vb"

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

    ' Class BoundsOverrides
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: __overrides, OverridesLower, OverridesUpper
    ' 
    ' Delegate Function
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Linq

''' <summary>
''' 复写模型之中的流的约束条件
''' </summary>
Public Class BoundsOverrides

    ReadOnly __UPPER_BOUNDS As IBoundsOverrides
    ReadOnly __LOWER_BOUNDS As IBoundsOverrides

    Sub New(up As IBoundsOverrides, lower As IBoundsOverrides)
        __UPPER_BOUNDS = up
        __LOWER_BOUNDS = lower
    End Sub

    Public Function OverridesUpper(fluxs As IEnumerable(Of String), bounds As Double()) As Double()
        Dim n As Double() = __overrides(fluxs, bounds, __UPPER_BOUNDS)
        Return n
    End Function

    Private Shared Function __overrides(fluxs As IEnumerable(Of String),
                                        bounds As Double(),
                                        ibo As IBoundsOverrides) As Double()
        Return (From rxn As SeqValue(Of String)
                In fluxs.SeqIterator
                Select ibo(rxn.value, bounds(rxn.i))).ToArray
    End Function

    Public Function OverridesLower(fluxs As IEnumerable(Of String), bounds As Double()) As Double()
        Dim n As Double() = __overrides(fluxs, bounds, __LOWER_BOUNDS)
        Return n
    End Function

End Class

Public Delegate Function IBoundsOverrides(rxn As String, curr As Double) As Double

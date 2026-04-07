#Region "Microsoft.VisualBasic::88879070d97a849cf3252b46a7c5f9a4, engine\Dynamics\Core\Mass\MassFactor.vb"

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

    '   Total Lines: 59
    '    Code Lines: 46 (77.97%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 13 (22.03%)
    '     File Size: 2.04 KB


    '     Class MassFactor
    ' 
    '         Properties: compartments, id, size
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '         Function: GetEnumerator, IEnumerable_GetEnumerator, ToString
    ' 
    '         Sub: (+2 Overloads) reset
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Dynamics.Core

Namespace Engine

    Public Class MassFactor : Implements IEnumerable(Of Factor)

        Public Property compartments As Dictionary(Of String, Factor)

        Public ReadOnly Property id As String

        Default Public ReadOnly Property getFactor(compart_id As String) As Factor
            Get
                Return compartments.TryGetValue(compart_id)
            End Get
        End Property

        Public ReadOnly Property size As Integer
            Get
                Return compartments.Where(Function(c) c.Value > 0).Count
            End Get
        End Property

        Sub New(id As String, list As IEnumerable(Of Factor))
            Me.id = id
            Me.compartments = list.ToDictionary(Function(c) c.cellular_compartment)
        End Sub

        Public Sub reset(value As Double)
            For Each factor As Factor In compartments.Values
                Call factor.reset(value)
            Next
        End Sub

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Sub reset(compart_id As String, value As Double)
            Call compartments(compart_id).reset(value)
        End Sub

        Public Overrides Function ToString() As String
            Dim locs As String() = compartments _
                .Where(Function(c) c.Value > 0) _
                .Select(Function(c) c.Key) _
                .ToArray

            Return $"{id}@{locs.JoinBy(", ")}"
        End Function

        Public Iterator Function GetEnumerator() As IEnumerator(Of Factor) Implements IEnumerable(Of Factor).GetEnumerator
            For Each factor As Factor In compartments.Values
                Yield factor
            Next
        End Function

        Private Function IEnumerable_GetEnumerator() As IEnumerator Implements IEnumerable.GetEnumerator
            Return GetEnumerator()
        End Function
    End Class
End Namespace

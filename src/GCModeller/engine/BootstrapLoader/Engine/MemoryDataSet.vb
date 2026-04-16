#Region "Microsoft.VisualBasic::562da6275d206b4be028fe50648d9801, engine\BootstrapLoader\Engine\MemoryDataSet.vb"

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

    '   Total Lines: 35
    '    Code Lines: 26 (74.29%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 9 (25.71%)
    '     File Size: 1.56 KB


    ' Class MemoryDataSet
    ' 
    '     Properties: mass
    ' 
    '     Function: getFluxDataSet, getMassDataSet
    ' 
    '     Sub: FluxSnapshot, ForwardRegulation, MassSnapshot, ReverseRegulation
    ' 
    ' /********************************************************************************/

#End Region

Imports SMRUCC.genomics.GCModeller.ModellingEngine.Dynamics.Engine

Public Class MemoryDataSet : Implements IOmicsDataAdapter

    Public ReadOnly Property mass As OmicsTuple(Of String()) Implements IOmicsDataAdapter.mass

    Dim massRows As New List(Of Dictionary(Of String, Double))
    Dim fluxRows As New List(Of Dictionary(Of String, Double))
    Dim forwardRows As New List(Of Dictionary(Of String, Double))
    Dim reverseRows As New List(Of Dictionary(Of String, Double))

    Public Sub MassSnapshot(iteration As Integer, data As Dictionary(Of String, Double)) Implements IOmicsDataAdapter.MassSnapshot
        massRows.Add(data)
    End Sub

    Public Sub FluxSnapshot(iteration As Integer, data As Dictionary(Of String, Double)) Implements IOmicsDataAdapter.FluxSnapshot
        fluxRows.Add(data)
    End Sub

    Public Function getMassDataSet() As ICollection(Of Dictionary(Of String, Double))
        Return massRows
    End Function

    Public Function getFluxDataSet() As ICollection(Of Dictionary(Of String, Double))
        Return fluxRows
    End Function

    Public Sub ForwardRegulation(iteration As Integer, data As Dictionary(Of String, Double)) Implements IOmicsDataAdapter.ForwardRegulation
        forwardRows.Add(New Dictionary(Of String, Double)(data))
    End Sub

    Public Sub ReverseRegulation(iteration As Integer, data As Dictionary(Of String, Double)) Implements IOmicsDataAdapter.ReverseRegulation
        reverseRows.Add(New Dictionary(Of String, Double)(data))
    End Sub
End Class


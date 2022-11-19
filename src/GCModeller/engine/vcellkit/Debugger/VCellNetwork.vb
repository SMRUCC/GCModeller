#Region "Microsoft.VisualBasic::75ab1e3ebb11d9be30a35a92f33f24a3, GCModeller\engine\vcellkit\Debugger\VCellNetwork.vb"

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

    '   Total Lines: 109
    '    Code Lines: 75
    ' Comment Lines: 28
    '   Blank Lines: 6
    '     File Size: 4.91 KB


    ' Module VCellNetwork
    ' 
    '     Function: CreateGraph
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream.Generic
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Dynamics.Core

Module VCellNetwork

    <Extension>
    Public Function CreateGraph(vcell As Vessel) As NetworkGraph
        Dim g As New NetworkGraph
        Dim processNode As Node

        For Each mass As Factor In vcell.MassEnvironment
            g.AddNode(New Node With {.label = mass.ID})
        Next

        For Each process As Channel In vcell.Channels
            processNode = g.AddNode(New Node With {.label = process.ID})

            For Each mass In process.GetReactants
                Call New Edge With {
                    .U = g.GetElementByID(mass.mass.ID),
                    .V = processNode,
                    .weight = mass.coefficient,
                    .ID = $"{process.ID}.reactant"，
                    .data = New EdgeData With {
                        .Properties = New Dictionary(Of String, String) From {
                            {NamesOf.REFLECTION_ID_MAPPING_INTERACTION_TYPE, "reaction"}
                        },
                        .label = process.ID
                    }
                }.DoCall(AddressOf g.AddEdge)
            Next
            For Each mass In process.GetProducts
                Call New Edge With {
                    .U = processNode,
                    .V = g.GetElementByID(mass.mass.ID),
                    .weight = mass.coefficient,
                    .ID = $"{process.ID}.product"，
                    .data = New EdgeData With {
                        .Properties = New Dictionary(Of String, String) From {
                            {NamesOf.REFLECTION_ID_MAPPING_INTERACTION_TYPE, "reaction"}
                        },
                        .label = process.ID
                    }
                }.DoCall(AddressOf g.AddEdge)
            Next
            'For Each factor In process.forward.activation
            '    Call New Edge With {
            '        .U = g.GetElementByID(factor.mass.ID),
            '        .V = processNode,
            '        .weight = factor.coefficient,
            '        .ID = $"{process.ID}.forward.activedBy.{factor.mass.ID}"，
            '        .data = New EdgeData With {
            '            .Properties = New Dictionary(Of String, String) From {
            '                {NamesOf.REFLECTION_ID_MAPPING_INTERACTION_TYPE, "forward.activation"}
            '            },
            '            .label = process.ID
            '        }
            '    }.DoCall(AddressOf g.AddEdge)
            'Next
            For Each factor In process.forward.inhibition
                Call New Edge With {
                    .U = g.GetElementByID(factor.mass.ID),
                    .V = processNode,
                    .weight = factor.coefficient,
                    .ID = $"{process.ID}.forward.inhibitedBy.{factor.mass.ID}"，
                    .data = New EdgeData With {
                        .Properties = New Dictionary(Of String, String) From {
                            {NamesOf.REFLECTION_ID_MAPPING_INTERACTION_TYPE, "forward.inhibition"}
                        },
                        .label = process.ID
                    }
                }.DoCall(AddressOf g.AddEdge)
            Next
            'For Each factor In process.reverse.activation
            '    Call New Edge With {
            '        .U = g.GetElementByID(factor.mass.ID),
            '        .V = processNode,
            '        .weight = factor.coefficient,
            '        .ID = $"{process.ID}.reverse.activedBy.{factor.mass.ID}"，
            '        .data = New EdgeData With {
            '            .Properties = New Dictionary(Of String, String) From {
            '                {NamesOf.REFLECTION_ID_MAPPING_INTERACTION_TYPE, "reverse.activation"}
            '            },
            '            .label = process.ID
            '        }
            '    }.DoCall(AddressOf g.AddEdge)
            'Next
            For Each factor In process.reverse.inhibition
                Call New Edge With {
                    .U = g.GetElementByID(factor.mass.ID),
                    .V = processNode,
                    .weight = factor.coefficient,
                    .ID = $"{process.ID}.reverse.inhibitedBy.{factor.mass.ID}"，
                    .data = New EdgeData With {
                        .Properties = New Dictionary(Of String, String) From {
                            {NamesOf.REFLECTION_ID_MAPPING_INTERACTION_TYPE, "reverse.inhibition"}
                        },
                        .label = process.ID
                    }
                }.DoCall(AddressOf g.AddEdge)
            Next
        Next

        Return g
    End Function
End Module

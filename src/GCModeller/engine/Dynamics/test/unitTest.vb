#Region "Microsoft.VisualBasic::3e05bc9342818c808bb7b08f327dd6dd, engine\Dynamics\test\unitTest.vb"

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

    ' Module unitTest
    ' 
    '     Sub: loopTest, Main, singleDirection
    ' 
    ' /********************************************************************************/

#End Region

Imports Dynamics.Debugger
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Dynamics.Core

Module unitTest
    Sub Main()
        ' Call singleDirection()
        Call loopTest()
    End Sub

    Sub singleDirection()

        ' a <=> b

        Dim a As New Factor With {.ID = "a", .Value = 1000}
        Dim b As New Factor With {.ID = "b", .Value = 1000}
        Dim reaction As New Channel({New Variable(a, 1)}, {New Variable(b, 2)}) With {
            .bounds = {10, 500},
            .ID = "a->b",
            .forward = 300,
            .reverse = New Controls With {.baseline = 0.05, .activation = {New Variable(b, 1)}}
        }

        Dim machine As New Vessel() With {.Channels = {reaction}, .MassEnvironment = {a, b}}

        machine.Initialize()

        Dim snapshots As New List(Of DataSet)
        Dim flux As New List(Of DataSet)


        For i As Integer = 0 To 100000
            flux += New DataSet With {
                .ID = i,
                .Properties = machine.ContainerIterator().ToDictionary.FlatTable
            }
            snapshots += New DataSet With {
                .ID = i,
                .Properties = machine.MassEnvironment.ToDictionary(Function(m) m.ID, Function(m) m.Value)
            }
        Next

        Call snapshots.SaveTo("./single/test_mass.csv")
        Call flux.SaveTo("./single/test_flux.csv")
        Call machine.ToGraph.DoCall(AddressOf Visualizer.CreateTabularFormat).Save("./single/test_network/")
    End Sub

    Sub loopTest()

        ' a <=> b <=> c <=> a

        Dim a As New Factor With {.ID = "a", .Value = 1000}
        Dim b As New Factor With {.ID = "b", .Value = 1000}
        Dim c As New Factor With {.ID = "c", .Value = 1000}
        Dim reaction As New Channel({New Variable(a, 1)}, {New Variable(b, 2)}) With {
            .bounds = {10, 500},
            .ID = "a->b",
            .forward = 300,
            .reverse = New Controls With {.baseline = 0.05, .activation = {New Variable(b, 1)}}
        }
        Dim reaction2 As New Channel({New Variable(b, 1)}, {New Variable(c, 2)}) With {
            .bounds = {10, 500},
            .ID = "b->c",
            .forward = 300,
            .reverse = New Controls With {.baseline = 0.05, .activation = {New Variable(a, 1)}}
        }
        Dim reaction3 As New Channel({New Variable(c, 4)}, {New Variable(a, 1)}) With {
            .bounds = {10, 500},
            .ID = "c->a",
            .forward = 300,
            .reverse = New Controls With {.baseline = 0.05, .activation = {New Variable(a, 0.01)}}
        }

        Dim machine As New Vessel() With {.Channels = {reaction, reaction2, reaction3}, .MassEnvironment = {a, b, c}}

        machine.Initialize()

        Dim snapshots As New List(Of DataSet)
        Dim flux As New List(Of DataSet)


        For i As Integer = 0 To 100000
            flux += New DataSet With {
                .ID = i,
                .Properties = machine.ContainerIterator().ToDictionary.FlatTable
            }
            snapshots += New DataSet With {
                .ID = i,
                .Properties = machine.MassEnvironment.ToDictionary(Function(m) m.ID, Function(m) m.Value)
            }
        Next

        Call snapshots.SaveTo("./loop/test_mass.csv")
        Call flux.SaveTo("./loop/test_flux.csv")
        Call machine.ToGraph.DoCall(AddressOf Visualizer.CreateTabularFormat).Save("./loop/test_network/")
    End Sub
End Module


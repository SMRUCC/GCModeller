#Region "Microsoft.VisualBasic::9108792265a89af206b19834c75e14bb, GCModeller\engine\Dynamics\test\unitTest.vb"

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

    '   Total Lines: 165
    '    Code Lines: 132
    ' Comment Lines: 4
    '   Blank Lines: 29
    '     File Size: 6.32 KB


    ' Module unitTest
    ' 
    '     Sub: kineticsTest, loopTest, Main, singleDirection
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Data.visualize.Network
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.GCModeller.ModellingEngine
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Dynamics
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Dynamics.Core
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Dynamics.Engine
Imports Microsoft.VisualBasic.Math.Scripting

Module unitTest
    Sub Main()
        Call kineticsTest()
        Call singleDirection()
        Call loopTest()
    End Sub

    Sub kineticsTest()
        ' a <=> b

        Dim exp = "(Vmax * S) / (Km + S)"
        Dim model As New Kinetics With {
            .formula = ScriptEngine.ParseExpression(exp),
            .parameters = {"Vmax", "S", "Km"},
            .paramVals = {10, "a", 2},
            .target = "a->b"
        }
        Dim machine As Vessel = New Vessel()

        Dim a As New Factor With {.ID = "a", .Value = 1000}
        Dim b As New Factor With {.ID = "b", .Value = 1000}
        Dim reaction As New Channel({New Variable(a, 1)}, {New Variable(b, 1)}) With {
            .bounds = {10, 10},
            .ID = "a->b",
            .forward = New KineticsControls(machine, model) With {.baseline = 0, .inhibition = {}},
            .reverse = New AdditiveControls With {.baseline = 0, .activation = {New Variable(b, 1)}, .inhibition = {New Variable(a, 2)}}
        }

        machine = machine.load({a, b}).load({reaction}).Initialize

        Dim snapshots As New List(Of DataSet)
        Dim flux As New List(Of DataSet)
        Dim dynamics = machine.ContainerIterator(100, 10000)
        Dim cache As New FluxAggregater(machine)

        For i As Integer = 0 To 10000
            Call dynamics.Tick()

            flux += New DataSet With {
                .ID = i,
                .Properties = cache.getFlux
            }
            snapshots += New DataSet With {
                .ID = i,
                .Properties = machine.MassEnvironment.ToDictionary(Function(m) m.ID, Function(m) m.Value)
            }
        Next

        Call snapshots.SaveTo("./kinetics/kinetics_test_mass.csv")
        Call flux.SaveTo("./kinetics/kinetics_test_flux.csv")
        Call machine.ToGraph.DoCall(AddressOf Visualizer.CreateTabularFormat).Save("./kinetics/test_network/")

        ' Pause()
    End Sub

    Sub singleDirection()

        ' a <=> b

        Dim a As New Factor With {.ID = "a", .Value = 1000}
        Dim b As New Factor With {.ID = "b", .Value = 1000}
        Dim reaction As New Channel({New Variable(a, 1)}, {New Variable(b, 1)}) With {
            .bounds = {10, 10},
            .ID = "a->b",
            .forward = CType(10, AdditiveControls),
            .reverse = New AdditiveControls With {.baseline = 0, .activation = {New Variable(b, 1)}, .inhibition = {New Variable(a, 2)}}
        }

        Dim machine As Vessel = New Vessel().load({a, b}).load({reaction}).Initialize

        Dim snapshots As New List(Of DataSet)
        Dim flux As New List(Of DataSet)
        Dim dynamics = machine.ContainerIterator(100, 10000)
        Dim cache As New FluxAggregater(machine)

        For i As Integer = 0 To 10000
            Call dynamics.Tick()

            flux += New DataSet With {
                .ID = i,
                .Properties = cache.getFlux
            }
            snapshots += New DataSet With {
                .ID = i,
                .Properties = machine.MassEnvironment.ToDictionary(Function(m) m.ID, Function(m) m.Value)
            }
        Next

        Call snapshots.SaveTo("./single/test_mass.csv")
        Call flux.SaveTo("./single/test_flux.csv")
        Call machine.ToGraph.DoCall(AddressOf Visualizer.CreateTabularFormat).Save("./single/test_network/")


        Pause()
    End Sub

    Sub loopTest()

        ' a <=> b <=> c <=> a

        Dim a As New Factor With {.ID = "a", .Value = 1000}
        Dim b As New Factor With {.ID = "b", .Value = 1000}
        Dim c As New Factor With {.ID = "c", .Value = 10000}
        Dim reaction As New Channel({New Variable(a, 3)}, {New Variable(b, 2)}) With {
            .bounds = {100, 5},
            .ID = "a->b",
            .forward = CType(300, AdditiveControls),
            .reverse = New AdditiveControls With {.baseline = 0.05, .activation = {New Variable(b, 1)}}
        }
        Dim reaction2 As New Channel({New Variable(b, 1)}, {New Variable(c, 2)}) With {
            .bounds = {10, 500},
            .ID = "b->c",
            .forward = CType(300, AdditiveControls),
            .reverse = New AdditiveControls With {.baseline = 0.05, .activation = {New Variable(a, 1)}}
        }
        Dim reaction3 As New Channel({New Variable(c, 4)}, {New Variable(a, 1)}) With {
            .bounds = {10, 5},
            .ID = "c->a",
            .forward = CType(300, AdditiveControls),
            .reverse = New AdditiveControls With {.baseline = 0.05, .activation = {New Variable(a, 0.01)}}
        }

        Dim reaction4 As New Channel({New Variable(b, 1), New Variable(a, 1)}, {New Variable(c, 2)}) With {
            .bounds = {100, 1},
            .ID = "ba->c",
            .forward = CType(300, AdditiveControls),
            .reverse = New AdditiveControls With {.baseline = 0.05, .activation = {New Variable(a, 1)}}
        }

        Dim machine As Vessel = New Vessel().load({reaction, reaction2, reaction3, reaction4}).load({a, b, c}).Initialize

        Dim snapshots As New List(Of DataSet)
        Dim flux As New List(Of DataSet)
        Dim dynamics = machine.ContainerIterator(100, 10000)
        Dim cache As New FluxAggregater(machine)

        For i As Integer = 0 To 2500
            dynamics.Tick()

            flux += New DataSet With {
                .ID = i,
                .Properties = cache.getFlux
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

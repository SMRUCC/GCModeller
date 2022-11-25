#Region "Microsoft.VisualBasic::b09cef191b927d3c5e12c4194e2e2735, GCModeller\engine\Dynamics\test\HugeNetworkTest.vb"

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

    '   Total Lines: 72
    '    Code Lines: 59
    ' Comment Lines: 0
    '   Blank Lines: 13
    '     File Size: 3.03 KB


    ' Module HugeNetworkTest
    ' 
    '     Sub: Main
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Data.visualize.Network
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject
Imports SMRUCC.genomics.ComponentModel.EquaionModel
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Dynamics
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Dynamics.Core
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Dynamics.Engine

Module HugeNetworkTest

    Sub Main()
        Dim reactions = (ls - l - r - "*.Xml" <= "D:\biodeep\biodeep_v2\data\KEGG\br08201").Select(AddressOf Reaction.LoadXml).ToArray
        Dim mass As New Dictionary(Of String, Factor)
        Dim channels As New List(Of Channel)

        For Each reaction As Reaction In reactions
            For Each name As String In reaction.GetSubstrateCompounds
                If Not mass.ContainsKey(name) Then
                    mass(name) = New Factor With {.ID = name, .Value = 3000}
                End If
            Next
        Next

        Dim left, right As Variable()
        Dim equation As DefaultTypes.Equation

        For Each reaction As Reaction In reactions.GroupBy(Function(r) r.ID).Select(Function(rg) rg.First)
            equation = reaction.ReactionModel
            left = equation.Reactants.Select(Function(c) New Variable(mass(c.ID), c.StoiChiometry)).ToArray
            right = equation.Products.Select(Function(c) New Variable(mass(c.ID), c.StoiChiometry)).ToArray

            channels += New Channel(left, right) With {
                .bounds = {300, 300},
                .forward = New AdditiveControls With {.baseline = 100, .inhibition = right.Select(Function(v) New Variable(v.mass, 0.025)).ToArray},
                .reverse = New AdditiveControls With {.baseline = 100, .inhibition = left.Select(Function(v) New Variable(v.mass, 0.025)).ToArray},
                .ID = reaction.ID
            }
        Next

        Dim cell As Vessel = New Vessel().load(channels).load(mass.Values)

        Call cell.Initialize()

        Dim snapshots As New List(Of DataSet)
        Dim flux As New List(Of DataSet)
        Dim dynamics = cell.ContainerIterator(5000, 10000)
        Dim cache As New FluxAggregater(cell)

        For i As Integer = 0 To 5000
            flux += New DataSet With {
                .ID = i,
                .Properties = cache.getFlux
            }
            snapshots += New DataSet With {
                .ID = i,
                .Properties = mass.ToDictionary(Function(m) m.Key, Function(m) m.Value.Value)
            }

            Call i.__DEBUG_ECHO
        Next

        Call snapshots.SaveTo("./test_mass.csv")
        Call flux.SaveTo("./test_flux.csv")
        Call cell.ToGraph.DoCall(AddressOf Visualizer.CreateTabularFormat).Save("./test_network/")

        Pause()
    End Sub
End Module

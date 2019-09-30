Imports Dynamics.Debugger
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject
Imports SMRUCC.genomics.ComponentModel.EquaionModel
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Dynamics.Core

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
                .forward = New Controls With {.baseline = 100, .inhibition = right.Select(Function(v) New Variable(v.Mass, 0.025)).ToArray},
                .reverse = New Controls With {.baseline = 100, .inhibition = left.Select(Function(v) New Variable(v.Mass, 0.025)).ToArray},
                .ID = reaction.ID
            }
        Next

        Dim cell As New Vessel With {.Channels = channels, .MassEnvironment = mass.Values.ToArray}

        Call cell.Initialize(100)

        Dim snapshots As New List(Of DataSet)
        Dim flux As New List(Of DataSet)

        For i As Integer = 0 To 5000
            flux += New DataSet With {
                .ID = i,
                .Properties = cell.ContainerIterator().ToDictionary.FlatTable
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

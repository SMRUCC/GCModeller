Imports System.Runtime.CompilerServices
Imports SMRUCC.genomics.GCModeller.ModellingEngine.BootstrapLoader.Definitions
Imports SMRUCC.genomics.GCModeller.ModellingEngine.BootstrapLoader.ModelLoader
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Dynamics.Core
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Model.Cellular

''' <summary>
''' Synthetic microbial community (SynComs)
''' </summary>
Public Module SyntheticMicrobialNetwork

    <Extension>
    Public Function CreateNetwork(models As IEnumerable(Of CellularModule),
                                  define As Definition,
                                  dynamics As FluxBaseline,
                                  referenceIds As Dictionary(Of String, String)) As (
        mass As MassTable,
        network As Channel(),
        fluxIndex As Dictionary(Of String, List(Of String))
    )

        Dim massTable As New MassTable(referenceIds)
        Dim fluxIndex As New Dictionary(Of String, List(Of String))
        Dim processList As New List(Of Channel)

        For Each modelData As CellularModule In models
            Dim loader As New Loader(define, dynamics, massTable:=massTable)

            Call massTable.SetDefaultCompartmentId(modelData.CellularEnvironmentName)

            With loader.CreateEnvironment(modelData)
                Call processList.AddRange(.processes)
            End With

            Dim modelFluxIndex = loader.GetFluxIndex

            For Each part_key As String In modelFluxIndex.Keys
                If Not fluxIndex.ContainsKey(part_key) Then
                    Call fluxIndex.Add(part_key, New List(Of String))
                End If

                Call fluxIndex(part_key).AddRange(modelFluxIndex(part_key))
            Next
        Next

        Return (massTable, processList.ToArray, fluxIndex)
    End Function
End Module

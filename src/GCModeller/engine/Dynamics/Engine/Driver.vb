Namespace Engine

    ''' <summary>
    ''' Data storage snapshot driver
    ''' </summary>
    ''' <param name="iteration">Iteration number</param>
    ''' <param name="data">
    ''' Read snapshot data from the simulator engine
    ''' </param>
    Public Delegate Sub SnapshotDriver(iteration%, data As Dictionary(Of String, Double))

    ''' <summary>
    ''' Data storage adapter driver
    ''' </summary>
    Public Interface IOmicsDataAdapter

        ''' <summary>
        ''' The metabolite mass id index list
        ''' </summary>
        ''' <returns></returns>
        ReadOnly Property mass As OmicsTuple(Of String())

        Sub MassSnapshot(iteration As Integer, data As Dictionary(Of String, Double))
        Sub FluxSnapshot(iteration As Integer, data As Dictionary(Of String, Double))
    End Interface
End Namespace
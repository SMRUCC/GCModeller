Namespace Engine

    Public Delegate Sub DataStorageDriver(iteration%, data As Dictionary(Of String, Double))

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
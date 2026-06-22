Public Class LatentDefinition

    Public Property varName As String

    ''' <summary>
    ''' manifestIndices
    ''' </summary>
    ''' <returns></returns>
    Public Property featureIDs As String()
    Public Property mode As MeasurementModels = MeasurementModels.A

    Sub New(name As String, manifest As IEnumerable(Of String), Optional mode As MeasurementModels = MeasurementModels.A)
        _varName = name
        _featureIDs = manifest.ToArray
        _mode = mode
    End Sub

End Class

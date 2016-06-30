Namespace Analysis.MotifScans

    ''' <summary>
    ''' 应用于调控位点的
    ''' </summary>
    Public Interface IMotifTrace
        Property MotifTrace As String
    End Interface

    ''' <summary>
    ''' 应用于调控作用的
    ''' </summary>
    Public Interface IFootprintTrace : Inherits IMotifTrace
        Property RegulatorTrace As String
    End Interface
End Namespace
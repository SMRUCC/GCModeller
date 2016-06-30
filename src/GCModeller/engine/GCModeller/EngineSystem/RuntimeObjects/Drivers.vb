Namespace EngineSystem.RuntimeObjects

    ''' <summary>
    ''' This system framework type object have a handle to running the network simulation.
    ''' (这个系统框架对象具备有一个可以驱动整个网络模型的方法句柄) 
    ''' </summary>
    ''' <remarks></remarks>
    Public Interface IDrivenable

#Region "ReadOnly Properties"
        ReadOnly Property EventId As String
#End Region

#Region "System Driver Handle"
        ''' <summary>
        ''' The System Driver Handle
        ''' </summary>
        ''' <param name="KernelCycle"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function __innerTicks(KernelCycle As Integer) As Integer
#End Region

    End Interface

    ''' <summary>
    ''' This type of object can running the cell system events as a network.
    ''' </summary>
    ''' <remarks></remarks>
    Public Interface ISystemDriver : Inherits IDrivenable

        ''' <summary>
        ''' The event object which can be running by this driver object.
        ''' </summary>
        ''' <param name="Event"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function RegisterEvent([Event] As IDrivenable)
    End Interface
End Namespace
Imports LANS.SystemsBiology.GCModeller.ModellingEngine.EngineSystem.Services.DataAcquisition.DataSerializer

Namespace EngineSystem.Services.MySQL

    ''' <summary>
    ''' 子系统模块之中的数据采集服务对象类型的抽象接口
    ''' </summary>
    ''' <remarks></remarks>
    Public Interface IDataAcquisitionService

        ''' <summary>
        ''' 本数据采集服务所映射到的数据库中的表或者CSV文件名
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property TableName As String
        ''' <summary>
        ''' 本数据采集服务对象实例连接至数据存储服务
        ''' </summary>
        ''' <param name="StorageService"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function Connect(StorageService As DataSerializer) As Integer
        ''' <summary>
        ''' 执行一次数据采集操作
        ''' </summary>
        ''' <param name="RTime"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function Tick(RTime As Integer) As Integer
        ''' <summary>
        ''' Disconnect the connection between the data acquisition service and data storage service and then write the cache data into the filesystem.
        ''' (数据采集服务关闭与数据存储服务之间的连接并将缓冲区中的数据写入磁盘文件之中)
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function Close() As Integer
        Sub GenerateDefinition()
        ''' <summary>
        ''' Initialize the data acquisition service module.(数据采集服务模块执行初始化操作)
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function Initialize() As Integer

        Sub SetUpCommitInterval(Interval As Integer)

        Function GetDataChunk() As DataFlowF()
        Function GetDefinitions() As HandleF()
    End Interface
End Namespace
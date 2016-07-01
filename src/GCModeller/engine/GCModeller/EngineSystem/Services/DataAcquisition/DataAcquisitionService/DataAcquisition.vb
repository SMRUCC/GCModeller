Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes
Imports System.Text
Imports SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.Services.DataAcquisition.DataSerializer
Imports SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.Engine
Imports SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.Services.MySQL
Imports SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.Services.DataAcquisition.Services
Imports SMRUCC.genomics.GCModeller.Framework
Imports SMRUCC.genomics.GCModeller.Framework.Kernel_Driver

Namespace EngineSystem.Services.DataAcquisition

    ''' <summary>
    ''' 子系统模块的数据输出至数据采集服务的数据转接器
    ''' </summary>
    ''' <remarks></remarks>
    Public MustInherit Class DataAdapter(Of TSystem As IDataSource)
        Inherits Kernel_Driver.DataAdapter(Of Double, EntityQuantitySample)
        Implements IDataAdapter

        Public MustOverride Function DataSource() As DataSource() Implements IDataAdapter.DataSource

        ''' <summary>
        '''  本数据采集对象所采集的目标模块对象
        ''' </summary>
        ''' <remarks></remarks>
        Protected System As TSystem

        Sub New(DataSourceSystem As TSystem)
            Me.System = DataSourceSystem
        End Sub

        Public MustOverride Function DefHandles() As HandleF() Implements IDataAdapter.DefHandles

        Public MustOverride ReadOnly Property TableName As String Implements IDataAdapter.TableName

        Public Overrides Function ToString() As String
            Return TableName
        End Function
    End Class
End Namespace
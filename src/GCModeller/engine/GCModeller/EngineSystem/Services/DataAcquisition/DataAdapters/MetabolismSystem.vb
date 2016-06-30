Imports System.Text
Imports LANS.SystemsBiology.GCModeller.ModellingEngine.EngineSystem.Services.DataAcquisition.DataSerializer
Imports LANS.SystemsBiology.GCModeller.ModellingEngine.EngineSystem.Services.DataAcquisition.Services

Namespace EngineSystem.Services.DataAcquisition.DataAdapters

    ''' <summary>
    ''' (数据采集服务获取代谢组中的每一种代谢物在每一次迭代计算之后的数量)
    ''' </summary>
    ''' <remarks></remarks>
    Public Class MetabolismSystem : Inherits EngineSystem.Services.DataAcquisition.DataAdapter(Of EngineSystem.ObjectModels.SubSystem.MetabolismCompartment)
        Implements IDataAdapter

        Public Overrides ReadOnly Property TableName As String
            Get
                Return "metabolism"
            End Get
        End Property

        Sub New(System As EngineSystem.ObjectModels.SubSystem.MetabolismCompartment)
            Call MyBase.New(System)
        End Sub

        Public Overrides Function DataSource() As DataSource()
            Return System.DataSource
        End Function

        Public Overrides Function DefHandles() As HandleF()
            Return System.get_DataSerializerHandles
        End Function
    End Class

    ''' <summary>
    ''' 统计酶动力学活性的数据接口
    ''' </summary>
    ''' <remarks></remarks>
    Public Class EnzymeActivity : Inherits EngineSystem.Services.DataAcquisition.DataAdapter(Of EngineSystem.ObjectModels.SubSystem.MetabolismCompartment)
        Implements IDataAdapter

        Sub New(System As EngineSystem.ObjectModels.SubSystem.MetabolismCompartment)
            Call MyBase.New(System)
        End Sub

        ''' <summary>
        ''' 主要记录的是酶浓度，PH值和温度所带来的效应
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function DataSource() As DataSource()
            Return System.EnzymeActivitiesDataSource
        End Function

        Public Overrides Function DefHandles() As HandleF()
            Return System.EnzymeActivitySerialHandles
        End Function

        Public Overrides ReadOnly Property TableName As String
            Get
                Return "EnzymeActivity"
            End Get
        End Property
    End Class

    Public Class ConstraintsMetabolite : Inherits EngineSystem.Services.DataAcquisition.DataAdapter(Of EngineSystem.ObjectModels.SubSystem.MetabolismCompartment)
        Implements IDataAdapter

        Sub New(System As EngineSystem.ObjectModels.SubSystem.MetabolismCompartment)
            Call MyBase.New(System)
        End Sub

        Public Overrides Function DataSource() As DataSource()
            Dim LQuery = (From EntityCompound In System.ConstraintMetabolite Let value = EntityCompound.DataSource Select value).ToArray
            Return LQuery
        End Function

        Public Overrides Function DefHandles() As HandleF()
            Return (From EntityCompound In System.ConstraintMetabolite Select EntityCompound.SerialsHandle).ToArray
        End Function

        Public Overrides ReadOnly Property TableName As String
            Get
                Return "ConstraintsMetabolites"
            End Get
        End Property
    End Class
End Namespace
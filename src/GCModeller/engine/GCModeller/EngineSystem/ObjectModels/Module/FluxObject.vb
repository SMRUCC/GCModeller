Imports System.Xml.Serialization
Imports LANS.SystemsBiology.GCModeller.Framework.Kernel_Driver
Imports LANS.SystemsBiology.GCModeller.ModellingEngine.EngineSystem.Services.DataAcquisition.DataSerializer
Imports LANS.SystemsBiology.GCModeller.ModellingEngine.EngineSystem.Services.DataAcquisition.Services

Namespace EngineSystem.ObjectModels.Module

    ''' <summary>
    ''' 细胞内部的流对象，本对象用于表示多个生物分子之间的相互作用关系
    ''' </summary>
    ''' <remarks></remarks>
    Public MustInherit Class FluxObject : Inherits ModellingEngine.EngineSystem.ObjectModels.ObjectModel
        Implements FluxObject.IFluxObjectHandle
        Implements IConsumptionStaticsInterface
        Implements IDataSourceEntity
        Implements IDynamicsExpression(Of Double)

        <DumpNode>
        <XmlAttribute>
        Public MustOverride ReadOnly Property FluxValue As Double Implements IFluxObjectHandle.FluxValue, GCModeller.Framework.Kernel_Driver.IDynamicsExpression(Of Double).Value

        Public Interface IFluxObjectHandle
            Inherits LANS.SystemsBiology.GCModeller.Framework.Kernel_Driver.IDynamicsExpression(Of Double)

            ReadOnly Property FluxValue As Double
            ReadOnly Property SerialsHandle() As HandleF

            ''' <summary>
            ''' 调用动力学模型之中的计算方法并返回计算值
            ''' </summary>
            ''' <returns></returns>
            ''' <remarks></remarks>
            Function Invoke() As Double
        End Interface

        Protected Friend Interface IConsumptionStaticsInterface
            ReadOnly Property get_ATP_EnergyConsumption As Double
        End Interface

        Public MustOverride Function Invoke() As Double Implements FluxObject.IFluxObjectHandle.Invoke, GCModeller.Framework.Kernel_Driver.IDynamicsExpression(Of Double).Evaluate

        ''' <summary>
        ''' 时间驱动程序所附加的额外的处理事件
        ''' </summary>
        ''' <param name="___attachedEvents">该事件的执行入口点</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Invoke(___attachedEvents As GCModeller.ModellingEngine.EngineSystem.ObjectModels.I_SystemEventDriver.___EVENT_HANDLE) As Double
            Dim i As Double = InternalEventInvoke()
            Call ___attachedEvents()
            Return i
        End Function

        ''' <summary>
        ''' 同名函数调用有BUG？？？？？所以必须要使用不相同名字的函数来调用同名函数？
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function InternalEventInvoke() As Double
            Return Me.Invoke()
        End Function

        ''' <summary>
        ''' 当前的这个反应流对象在一次内核循环之后所消耗掉的能量
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Protected Friend Overridable ReadOnly Property get_ATP_EnergyConsumption As Double Implements IConsumptionStaticsInterface.get_ATP_EnergyConsumption
            Get
                Return 0
            End Get
        End Property

        Public Overridable ReadOnly Property DataSource As DataSource Implements IDataSourceEntity.DataSource
            Get
                Return New DataSource(Handle, FluxValue)
            End Get
        End Property

        Public Overridable ReadOnly Property SerialsHandle As HandleF Implements IDataSourceEntity.SerialsHandle, IFluxObjectHandle.SerialsHandle
            Get
                Return New HandleF With {
                    .Identifier = Identifier,
                    .Handle = Handle
                }
            End Get
        End Property

        Public Overrides Property Identifier As String Implements GCModeller.Framework.Kernel_Driver.IDynamicsExpression(Of Double).Identifier

        Public Function CreateHandle() As LANS.SystemsBiology.GCModeller.Framework.Kernel_Driver.DataStorage.FileModel.ObjectHandle Implements LANS.SystemsBiology.GCModeller.Framework.Kernel_Driver.IDynamicsExpression(Of Double).get_ObjectHandle
            Return New LANS.SystemsBiology.GCModeller.Framework.Kernel_Driver.DataStorage.FileModel.ObjectHandle With {
                .Handle = Handle,
                .Identifier = Identifier
            }
        End Function
    End Class
End Namespace
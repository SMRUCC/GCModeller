#Region "Microsoft.VisualBasic::2ecc8df01c4272e5f1a9ff29a4054bd5, engine\GCModeller\EngineSystem\ObjectModels\SubSystem\FluxSystemFramework.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xie (genetics@smrucc.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program. If not, see <http://www.gnu.org/licenses/>.



    ' /********************************************************************************/

    ' Summaries:

    '     Class ReactorMachine
    ' 
    '         Properties: ATP_EnergyConsumption, Count, DataSource, Guid, Keys
    '                     NetworkComponents, RuntimeTicks, SystemActivityLoad, SystemLogging, Values
    ' 
    '         Function: __innerTicks, ContainsKey, get_DataSerializerHandles, get_RuntimeContainer, GetEnumerator
    '                   GetEnumerator1, TryGetValue
    ' 
    '         Sub: (+2 Overloads) Dispose, set_NetworkComponents
    ' 
    '     Interface ICellComponentContainer
    ' 
    '         Properties: SystemLogging
    ' 
    '         Function: get_runtimeContainer
    ' 
    '     Class SubSystemContainer
    ' 
    '         Properties: SystemLogging
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: get_runtimeContainer
    ' 
    '     Class CellComponentSystemFramework
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: Get_cellComponentContainer
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ApplicationServices.Debugging.Logging
Imports Microsoft.VisualBasic.Serialization
Imports SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.Engine
Imports SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.RuntimeObjects
Imports SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.Services.DataAcquisition.DataSerializer
Imports SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.Services.DataAcquisition.Services
Imports SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.Services.MySQL

Namespace EngineSystem.ObjectModels.SubSystem

    ''' <summary>
    ''' ReactorMachine is a subsystem for entities’ interactions, running the cell system network.(培养基，以及细胞子模块系统的基本类型)
    ''' </summary>
    ''' <typeparam name="TFluxObject"></typeparam>
    ''' <remarks></remarks>
    Public MustInherit Class ReactorMachine(Of TFluxObject As ModellingEngine.EngineSystem.ObjectModels.Module.FluxObject.IFluxObjectHandle)
        Inherits SMRUCC.genomics.GCModeller.Framework.Kernel_Driver.ReactorMachine(Of Double, TFluxObject)

        Implements IReadOnlyDictionary(Of String, TFluxObject) '<UniqueId, Item>
        Implements IDisposable
        Implements PlugIns.ISystemFrameworkEntry.ISystemFramework
        Implements EngineSystem.ObjectModels.Module.FluxObject.IConsumptionStaticsInterface
        Implements IDataSource
        Implements ModellingEngine.EngineSystem.ObjectModels.SubSystem.SystemObject.I_SystemModel
        Implements IRuntimeObject

        Protected _SystemLogging As LogFile
        Protected IRuntimeContainer As IContainerSystemRuntimeEnvironment

        ''' <summary>
        ''' 总流量的Log值
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <DumpNode> Public Overridable ReadOnly Property SystemActivityLoad As Double
            Get
                Dim Sum = (From item In Me._DynamicsExprs Select Global.System.Math.Abs(item.FluxValue)).Sum
                Return Global.System.Math.Log10(Sum)
            End Get
        End Property

        Protected Friend Overridable ReadOnly Property SystemLogging As LogFile Implements SystemObject.I_SystemModel.SystemLogging
            Get
                Return _SystemLogging
            End Get
        End Property

        ''' <summary>
        ''' 当前的子系统模块内的子模块组件对象的集合构成了当前的子系统模块
        ''' </summary>
        ''' <remarks></remarks>
        Public Overridable ReadOnly Property NetworkComponents As TFluxObject()
            Get
                Return _DynamicsExprs
            End Get
        End Property

        Public MustOverride Sub MemoryDump(Dir As String) Implements SystemObject.I_SystemModel.MemoryDump

        Public Shared Sub set_NetworkComponents(NetworkComponents As TFluxObject(), Reactor As ReactorMachine(Of TFluxObject))
            Reactor._DynamicsExprs = NetworkComponents
        End Sub

#Region "Implements PlugIns.ISystemFrameworkEntry.ISystemFramework"

        ''' <summary>
        ''' 子系统模块对数据采集服务所开放的数据接口
        ''' </summary>
        ''' <value></value>
        ''' <returns>(Id)Handle, value</returns>
        ''' <remarks></remarks>
        Public Overridable ReadOnly Property DataSource As DataSource() Implements PlugIns.ISystemFrameworkEntry.ISystemFramework.DataSource, IDataSource.DataSource
            Get
                Dim LQuery = (From FluxObject As ObjectModels.Module.FluxObject.IFluxObjectHandle
                              In _DynamicsExprs.AsParallel
                              Select New DataSource(FluxObject.Address, FluxObject.FluxValue)).ToArray
                Return LQuery
            End Get
        End Property

        Public MustOverride Overrides Function Initialize() As Integer Implements PlugIns.ISystemFrameworkEntry.ISystemFramework.Initialzie, SystemObject.I_SystemModel.Initialize

        ''' <summary>
        ''' 从本系统模块实例之中创建一个不带有数据存储服务连接参数的数据采集服务对象实例
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public MustOverride Function CreateServiceSerials() As IDataAcquisitionService() Implements PlugIns.ISystemFrameworkEntry.ISystemFramework.CreateServiceInstanceSerials
#End Region

        Protected Overrides Function __innerTicks(KernelCycle As Integer) As Integer
            Return 0
        End Function

#Region "Implements EngineSystem.ObjectModels.Module.FluxObject.IConsumptionStaticsInterface"

        Protected Friend ReadOnly Property ATP_EnergyConsumption As Double Implements [Module].FluxObject.IConsumptionStaticsInterface.get_ATP_EnergyConsumption
            Get
                Return 0
            End Get
        End Property
#End Region

#Region "IDisposable Support"
        Private disposedValue As Boolean ' 检测冗余的调用

        ' IDisposable
        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not Me.disposedValue Then
                If disposing Then
                    ' TODO:  释放托管状态(托管对象)。
                End If

                ' TODO:  释放非托管资源(非托管对象)并重写下面的 Finalize()。
                ' TODO:  将大型字段设置为 null。
            End If
            Me.disposedValue = True
        End Sub

        ' TODO:  仅当上面的 Dispose( disposing As Boolean)具有释放非托管资源的代码时重写 Finalize()。
        'Protected Overrides Sub Finalize()
        '    ' 不要更改此代码。    请将清理代码放入上面的 Dispose( disposing As Boolean)中。
        '    Dispose(False)
        '    MyBase.Finalize()
        'End Sub

        ' Visual Basic 添加此代码是为了正确实现可处置模式。
        Public Sub Dispose() Implements IDisposable.Dispose
            ' 不要更改此代码。    请将清理代码放入上面的 Dispose (disposing As Boolean)中。
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub
#End Region

#Region "Implements Global.System.Collections.Generic.IReadOnlyDictionary(Of String, TFluxObject)"

        Public Iterator Function GetEnumerator() As Global.System.Collections.Generic.IEnumerator(Of Global.System.Collections.Generic.KeyValuePair(Of String, TFluxObject)) Implements Global.System.Collections.Generic.IEnumerable(Of Global.System.Collections.Generic.KeyValuePair(Of String, TFluxObject)).GetEnumerator
            For i As Integer = 0 To _DynamicsExprs.Count - 1
                Dim Item = _DynamicsExprs(i)
                Yield New KeyValuePair(Of String, TFluxObject)(Item.Key, Item)
            Next
        End Function

        Public ReadOnly Property Count As Integer Implements Global.System.Collections.Generic.IReadOnlyCollection(Of Global.System.Collections.Generic.KeyValuePair(Of String, TFluxObject)).Count
            Get
                Return _DynamicsExprs.Count()
            End Get
        End Property

        Dim KeyCollection As String()

        Public Function ContainsKey(key As String) As Boolean Implements Global.System.Collections.Generic.IReadOnlyDictionary(Of String, TFluxObject).ContainsKey
            If KeyCollection.IsNullOrEmpty Then
                KeyCollection = (From item In _DynamicsExprs Select name = item.Key).ToArray
            End If

            Dim f As Boolean = Array.IndexOf(KeyCollection, key) > -1
            Return f
        End Function

        Default Public ReadOnly Property Item(key As String) As TFluxObject Implements Global.System.Collections.Generic.IReadOnlyDictionary(Of String, TFluxObject).Item
            Get
                If KeyCollection.IsNullOrEmpty Then
                    KeyCollection = (From FluxObject In _DynamicsExprs Select name = FluxObject.Key).ToArray
                End If

                Dim i = Array.IndexOf(KeyCollection, key)
                If Not i < 0 Then
                    Return _DynamicsExprs(i)
                End If
            End Get
        End Property

        Public ReadOnly Property Keys As IEnumerable(Of String) Implements IReadOnlyDictionary(Of String, TFluxObject).Keys
            Get
                If KeyCollection.IsNullOrEmpty Then
                    KeyCollection = (From FluxObject In _DynamicsExprs Select FluxObject.Key).ToArray
                End If

                Return KeyCollection
            End Get
        End Property

        Public Function TryGetValue(key As String, ByRef value As TFluxObject) As Boolean Implements IReadOnlyDictionary(Of String, TFluxObject).TryGetValue
            If KeyCollection.IsNullOrEmpty Then
                KeyCollection = (From FluxObject In _DynamicsExprs Select name = FluxObject.Key).ToArray
            End If

            Dim i = Array.IndexOf(KeyCollection, key)
            If Not i < 0 Then
                value = _DynamicsExprs(i)
                Return True
            Else
                Return False
            End If
        End Function

        Public ReadOnly Property Values As Global.System.Collections.Generic.IEnumerable(Of TFluxObject) Implements Global.System.Collections.Generic.IReadOnlyDictionary(Of String, TFluxObject).Values
            Get
                Return Me._DynamicsExprs
            End Get
        End Property

        Public Iterator Function GetEnumerator1() As Global.System.Collections.IEnumerator Implements Global.System.Collections.IEnumerable.GetEnumerator
            Yield GetEnumerator()
        End Function
#End Region

        Public Overridable Function get_DataSerializerHandles() As HandleF() Implements IDataSource.Get_DataSerializerHandles
            Dim LQuery = (From FluxObject In Me.NetworkComponents Select New HandleF With {.Identifier = FluxObject.Key, .Handle = FluxObject.Address}).ToArray
            Return LQuery
        End Function

        Public Overridable Function get_RuntimeContainer() As IContainerSystemRuntimeEnvironment Implements SystemObject.I_SystemModel.get_RuntimeContainer
            Return Me.IRuntimeContainer
        End Function

        Public ReadOnly Property Guid As Long Implements IRuntimeObject.Guid
            Get
                Return CType(Me.GetHashCode, Long)
            End Get
        End Property

        Public Overrides ReadOnly Property RuntimeTicks As Long
            Get
                Return Me.IRuntimeContainer.RuntimeTicks
            End Get
        End Property
    End Class

    Public Interface ICellComponentContainer
        ReadOnly Property SystemLogging As LogFile
        Function get_runtimeContainer() As IContainerSystemRuntimeEnvironment
    End Interface

    ''' <summary>
    ''' 代谢组和基因表达调控网络
    ''' </summary>
    ''' <typeparam name="TFluxObject"></typeparam>
    ''' <remarks></remarks>
    Public MustInherit Class SubSystemContainer(Of TFluxObject As ModellingEngine.EngineSystem.ObjectModels.Module.FluxObject.IFluxObjectHandle)
        Inherits ReactorMachine(Of TFluxObject)

        Implements Global.System.Collections.Generic.IReadOnlyDictionary(Of String, TFluxObject) '<UniqueId, Item>
        Implements Global.System.IDisposable
        Implements ModellingEngine.PlugIns.ISystemFrameworkEntry.ISystemFramework
        Implements EngineSystem.ObjectModels.Module.FluxObject.IConsumptionStaticsInterface
        Implements ICellComponentContainer

        Protected Friend _CellSystem As SubSystem.CellSystem

        Protected Friend Overrides ReadOnly Property SystemLogging As LogFile Implements ICellComponentContainer.SystemLogging
            Get
                Return MyBase.SystemLogging
            End Get
        End Property

        Sub New(CellSystem As SubSystem.CellSystem)
            _CellSystem = CellSystem
            _SystemLogging = CellSystem.SystemLogging
            IRuntimeContainer = CellSystem.get_runtimeContainer
        End Sub

        Public Overrides Function get_runtimeContainer() As IContainerSystemRuntimeEnvironment Implements ICellComponentContainer.get_runtimeContainer
            Return MyBase.get_RuntimeContainer
        End Function
    End Class

    ''' <summary>
    ''' 细胞内部模块子系统模块的基本框架
    ''' </summary>
    ''' <typeparam name="TFluxObject"></typeparam>
    ''' <remarks></remarks>
    Public MustInherit Class CellComponentSystemFramework(Of TFluxObject As ModellingEngine.EngineSystem.ObjectModels.Module.FluxObject.IFluxObjectHandle)
        Inherits ReactorMachine(Of TFluxObject)

        Implements Global.System.Collections.Generic.IReadOnlyDictionary(Of String, TFluxObject) '<UniqueId, Item>
        Implements Global.System.IDisposable
        Implements ModellingEngine.PlugIns.ISystemFrameworkEntry.ISystemFramework
        Implements EngineSystem.ObjectModels.Module.FluxObject.IConsumptionStaticsInterface

        ''' <summary>
        ''' 当前的系统模块对象所处的上一层系统模块
        ''' </summary>
        ''' <remarks></remarks>
        Protected _CellComponentContainer As ICellComponentContainer

        Sub New(CellComponentContainer As ICellComponentContainer)
            _SystemLogging = CellComponentContainer.SystemLogging
            _CellComponentContainer = CellComponentContainer
            IRuntimeContainer = CellComponentContainer.get_runtimeContainer
            Call _SystemLogging.WriteLine("     ----> CREATE_SYSTEM_OBJECT()::" & Me.GetType.FullName, "Load(Of TSystem As EngineSystem.ObjectModels.System.SystemFramework(Of FluxObject))", Type:=MSG_TYPES.INF)
        End Sub

        Protected Friend Function Get_cellComponentContainer() As ICellComponentContainer
            Return _CellComponentContainer
        End Function
    End Class
End Namespace

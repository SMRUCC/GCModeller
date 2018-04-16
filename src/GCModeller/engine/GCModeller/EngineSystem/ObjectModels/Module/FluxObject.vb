#Region "Microsoft.VisualBasic::7fbea02bb66a926f5c32d4f6c2cc7a95, engine\GCModeller\EngineSystem\ObjectModels\Module\FluxObject.vb"

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

    '     Class FluxObject
    ' 
    '         Properties: DataSource, get_ATP_EnergyConsumption, Identifier, SerialsHandle
    ' 
    '         Function: CreateHandle, InternalEventInvoke, Invoke
    '         Interface IFluxObjectHandle
    ' 
    '             Properties: FluxValue, SerialsHandle
    ' 
    '             Function: Invoke
    ' 
    '         Interface IConsumptionStaticsInterface
    ' 
    '             Properties: get_ATP_EnergyConsumption
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Serialization
Imports SMRUCC.genomics.GCModeller.Framework
Imports SMRUCC.genomics.GCModeller.Framework.Kernel_Driver
Imports SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.Services.DataAcquisition.DataSerializer
Imports SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.Services.DataAcquisition.Services

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
        Public MustOverride ReadOnly Property FluxValue As Double Implements IFluxObjectHandle.FluxValue, Kernel_Driver.IDynamicsExpression(Of Double).Value

        Public Interface IFluxObjectHandle
            Inherits SMRUCC.genomics.GCModeller.Framework.Kernel_Driver.IDynamicsExpression(Of Double)

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

        Public MustOverride Function Invoke() As Double Implements FluxObject.IFluxObjectHandle.Invoke, Kernel_Driver.IDynamicsExpression(Of Double).Evaluate

        ''' <summary>
        ''' 时间驱动程序所附加的额外的处理事件
        ''' </summary>
        ''' <param name="___attachedEvents">该事件的执行入口点</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Invoke(___attachedEvents As ModellingEngine.EngineSystem.ObjectModels.I_SystemEventDriver.___EVENT_HANDLE) As Double
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

        Public Overrides Property Identifier As String Implements Kernel_Driver.IDynamicsExpression(Of Double).Key

        Public Function CreateHandle() As SMRUCC.genomics.GCModeller.Framework.Kernel_Driver.DataStorage.FileModel.ObjectHandle Implements SMRUCC.genomics.GCModeller.Framework.Kernel_Driver.IDynamicsExpression(Of Double).get_ObjectHandle
            Return New SMRUCC.genomics.GCModeller.Framework.Kernel_Driver.DataStorage.FileModel.ObjectHandle With {
                .Handle = Handle,
                .ID = Identifier
            }
        End Function
    End Class
End Namespace

#Region "Microsoft.VisualBasic::b381509a61177f195f7f37c8a21cd8d9, GCModeller\engine\GCModeller.Framework.Kernel_Driver\Driver\Abstract\Abstract.vb"

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


    ' Code Statistics:

    '   Total Lines: 55
    '    Code Lines: 20
    ' Comment Lines: 24
    '   Blank Lines: 11
    '     File Size: 1.91 KB


    ' Interface IObjectStatus
    ' 
    '     Properties: locusId, Status
    ' 
    ' Class ObjectStatusReader
    ' 
    ' 
    ' 
    ' Interface IDynamicsExpression
    ' 
    '     Properties: Value
    ' 
    '     Function: Evaluate, get_ObjectHandle
    ' 
    ' Interface IDataSource
    ' 
    '     Properties: Address, Value
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic

Public Interface IObjectStatus : Inherits IAddressOf

    ReadOnly Property Status As Boolean
    Property locusId As String

End Interface

Public Class ObjectStatusReader : Inherits DataAdapter(Of Boolean, TransitionStateSample)

End Class

''' <summary>
''' The dynamics kinetics expression of calculates the system behavior.(这个动力学对象可以用于计算整个系统的行为的变化)
''' </summary>
''' <typeparam name="DataType">Just alow the basically numeric value type(Integer, Double, Long, Boolean).
''' (只能取值基本的数值类型(<see cref="Integer"></see>, <see cref="Double"></see>, <see cref="Long"></see>, <see cref="Boolean"></see>))</typeparam>
''' <remarks></remarks>
Public Interface IDynamicsExpression(Of DataType)
    Inherits IAddressOf, INamedValue

#Region "Interface"
    ''' <summary>
    ''' Get the value of the target expression object in current time.(获取本对象在当前时间点上的值)
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    ReadOnly Property Value As DataType

    Function Evaluate() As DataType
    Function get_ObjectHandle() As DataStorage.FileModel.ObjectHandle
#End Region

End Interface

Public Interface IDataSource(Of THandle, TValue)

    ''' <summary>
    ''' Handle pointer value of this node object in the data source.(本节点对象在数据源之中的指针信息)
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Property Address As THandle
    ''' <summary>
    ''' Data value to the data source of this node object.(本节点对象的数据源属性)
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    ReadOnly Property Value As TValue
End Interface

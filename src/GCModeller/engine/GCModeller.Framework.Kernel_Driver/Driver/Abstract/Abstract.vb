Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic

Public Interface IObjectStatus : Inherits IAddressHandle

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
    Inherits IAddressHandle, sIdEnumerable

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

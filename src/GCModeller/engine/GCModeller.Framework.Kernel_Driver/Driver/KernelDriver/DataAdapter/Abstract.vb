#Region "Microsoft.VisualBasic::6088e30ea13b6cbfeca76b73c20c54d4, GCModeller\engine\GCModeller.Framework.Kernel_Driver\Driver\KernelDriver\DataAdapter\Abstract.vb"

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

    '   Total Lines: 50
    '    Code Lines: 29
    ' Comment Lines: 9
    '   Blank Lines: 12
    '     File Size: 1.69 KB


    ' Interface IDriver_DataSource_Adapter
    ' 
    '     Function: get_DataSerials, get_ObjectHandlers
    ' 
    ' Class DataSourceHandler
    ' 
    '     Properties: Handle, TimeStamp, Value
    ' 
    '     Function: ToString
    ' 
    ' Interface IDataSourceHandle
    ' 
    '     Properties: Handle, TimeStamp
    ' 
    ' Class TransitionStateSample
    ' 
    '     Function: ToString
    ' 
    ' Class EntityQuantitySample
    ' 
    ' 
    ' 
    ' Class StateEnumerationsSample
    ' 
    ' 
    ' 
    ' Interface IDataStorage
    ' 
    '     Function: WriteData
    ' 
    ' /********************************************************************************/

#End Region

''' <summary>
''' <see cref="System.Double"></see>; <see cref="Integer"></see>; <see cref="System.Boolean"></see>
''' </summary>
''' <typeparam name="T"><see cref="System.Double"></see>; <see cref="Integer"></see>; <see cref="System.Boolean"></see></typeparam>
''' <remarks></remarks>
Public Interface IDriver_DataSource_Adapter(Of T)
    Function get_ObjectHandlers() As DataStorage.FileModel.ObjectHandle()
    Function get_DataSerials() As DataStorage.FileModel.DataSerials(Of T)()
End Interface

Public Class DataSourceHandler(Of TValue)
    Implements IDataSourceHandle

    Public Property Handle As Long Implements IDataSourceHandle.Handle
    Public Property Value As TValue
    Public Property TimeStamp As Integer Implements IDataSourceHandle.TimeStamp

    Public Overrides Function ToString() As String
        Return String.Format("[{0}] ${1} -> {2}", Handle, TimeStamp, Value.ToString)
    End Function
End Class

Public Interface IDataSourceHandle
    Property Handle As Long
    Property TimeStamp As Integer
End Interface

Public Class TransitionStateSample : Inherits DataSourceHandler(Of Boolean)

    Public Overrides Function ToString() As String
        Return String.Format("[{0}] ${1} -> Ts( '{2}' )", Handle, TimeStamp, Value)
    End Function
End Class

''' <summary>
''' <see cref="System.Double"></see>类型的值对象
''' </summary>
''' <remarks></remarks>
Public Class EntityQuantitySample : Inherits DataSourceHandler(Of Double)

End Class

Public Class StateEnumerationsSample : Inherits DataSourceHandler(Of Integer)

End Class

Public Interface IDataStorage : Inherits System.IDisposable

    Function WriteData(url As String) As Boolean
End Interface

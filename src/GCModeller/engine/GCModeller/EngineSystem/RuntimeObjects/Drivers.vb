#Region "Microsoft.VisualBasic::6510853bf402ad2b58d80a64eefd3b7e, engine\GCModeller\EngineSystem\RuntimeObjects\Drivers.vb"

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

    '     Interface IDrivenable
    ' 
    '         Properties: EventId
    ' 
    '         Function: __innerTicks
    ' 
    '     Interface ISystemDriver
    ' 
    '         Function: RegisterEvent
    ' 
    ' 
    ' /********************************************************************************/

#End Region

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

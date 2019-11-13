#Region "Microsoft.VisualBasic::6172b6fa6231edc37a6d9f189f01d581, engine\GCModeller\EngineSystem\ObjectModels\EventDriver.vb"

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

    '     Class I_SystemEventDriver
    ' 
    ' 
    '         Delegate Sub
    ' 
    '             Properties: SystemLogging
    ' 
    '             Constructor: (+1 Overloads) Sub New
    ' 
    '             Function: GetEnumerator, GetEnumerator1, Initialize, ToString
    ' 
    '             Sub: ___Internal_get_attachedEvent, __liquidBrothFilledWater, ConnectCultivationMediumSystem, EmptyEvent, InvokeEvents
    '                  JoinEvents, MemoryDump
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ApplicationServices.Debugging.Logging
Imports SMRUCC.genomics.GCModeller.Assembly

Namespace EngineSystem.ObjectModels

    ''' <summary>
    ''' Driver for all of the events in the target <see cref="I_SystemEventDriver._cellObject">virtual cell.</see>
    ''' (目标虚拟细胞对象之中的所有网络之中的边的驱动程序模块)
    ''' </summary>
    ''' <remarks></remarks>
    Public Class I_SystemEventDriver : Inherits EngineSystem.ObjectModels.SubSystem.SystemObjectModel
        Implements Generic.IEnumerable(Of EngineSystem.ObjectModels.Module.FluxObject)

        Public Delegate Sub ___EVENT_HANDLE()

#Region "Properties and fields"

        Dim _lstEventDelegate As List(Of EngineSystem.ObjectModels.Module.FluxObject) =
            New List(Of EngineSystem.ObjectModels.Module.FluxObject)
        Dim _cellObject As EngineSystem.ObjectModels.SubSystem.CellSystem
        Dim _innerCell_WATER As EngineSystem.ObjectModels.Entity.Compound
        ''' <summary>
        ''' 附加需要处理的额外事件
        ''' </summary>
        ''' <remarks></remarks>
        Dim _lstAttachedEvent As List(Of ___EVENT_HANDLE)

        ''' <summary>
        ''' <see cref="___Internal_get_attachedEvent"></see>函数的执行入口点
        ''' </summary>
        ''' <remarks></remarks>
        Dim _innerAttachedEventHandle As ___EVENT_HANDLE

        Protected Friend Overrides ReadOnly Property SystemLogging As LogFile
            Get
                Return _cellObject.SystemLogging
            End Get
        End Property
#End Region

        Sub New(CellSystem As EngineSystem.ObjectModels.SubSystem.CellSystem)
            Me._cellObject = CellSystem
            _lstAttachedEvent = New List(Of ___EVENT_HANDLE)
            _innerAttachedEventHandle = AddressOf Me.___Internal_get_attachedEvent
        End Sub

        ''' <summary>
        ''' Adding the flux object events into this <see cref="I_SystemEventDriver"></see>
        ''' </summary>
        ''' <param name="Events"></param>
        ''' <remarks></remarks>
        Public Sub JoinEvents(Events As Generic.IEnumerable(Of EngineSystem.ObjectModels.Module.FluxObject))
            Call _lstEventDelegate.AddRange(Events)
        End Sub

        Public Sub ConnectCultivationMediumSystem(obj As ObjectModels.SubSystem.CultivationMediums)
            If Not obj.IsLiquidsBrothMedium Then Return

            Dim WaterId As String =
                _cellObject.get_runtimeContainer.SystemVariable(GCMarkupLanguage.GCML_Documents.ComponentModels.SystemVariables.ID_WATER)
            Me._innerCell_WATER = _cellObject.Metabolism.GetMetabolite(WaterId)
            Call _lstAttachedEvent.Add(AddressOf __liquidBrothFilledWater)
        End Sub

        ''' <summary>
        ''' DO_NOTHING
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub EmptyEvent()
        End Sub

        Private Sub __liquidBrothFilledWater()
            _innerCell_WATER.Quantity = 1000
        End Sub

        Private Sub ___Internal_get_attachedEvent()
            For Each __Event As ___EVENT_HANDLE In _lstAttachedEvent
                Call __Event()
            Next
        End Sub

        ''' <summary>
        ''' Running the virtual cell from here!
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub InvokeEvents()
            Dim LQuery = (From cellEvent As [Module].FluxObject
                          In _lstEventDelegate.Shuffles
                          Select cellEvent.Invoke(___attachedEvents:=_innerAttachedEventHandle)).ToArray
        End Sub

#Region "Implements Generic.IEnumerable(Of EngineSystem.ObjectModels.Module.FluxObject)"

        Public Iterator Function GetEnumerator() As IEnumerator(Of EngineSystem.ObjectModels.Module.FluxObject) Implements IEnumerable(Of EngineSystem.ObjectModels.Module.FluxObject).GetEnumerator
            For Each item In _lstEventDelegate
                Yield item
            Next
        End Function

        Public Iterator Function GetEnumerator1() As IEnumerator Implements IEnumerable.GetEnumerator
            Yield GetEnumerator()
        End Function
#End Region

        Public Overrides Function ToString() As String
            Return $"{_lstEventDelegate.Count} events and {_lstAttachedEvent.Count} attached event(s) was registered in this event driver."
        End Function

        Public Overrides Function Initialize() As Integer
            _lstEventDelegate = (From item In _lstEventDelegate Select item Distinct).AsList
            Call SystemLogging.WriteLine(ToString)
            Return 0
        End Function

        Public Overrides Sub MemoryDump(Dir As String)

        End Sub
    End Class
End Namespace

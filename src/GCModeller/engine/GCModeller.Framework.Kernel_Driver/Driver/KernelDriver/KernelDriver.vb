#Region "Microsoft.VisualBasic::47ee5a74a491555ea2187cdb85769547, GCModeller\engine\GCModeller.Framework.Kernel_Driver\Driver\KernelDriver\KernelDriver.vb"

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

    '   Total Lines: 150
    '    Code Lines: 85
    ' Comment Lines: 40
    '   Blank Lines: 25
    '     File Size: 6.02 KB


    ' Interface IKernelDriver
    ' 
    '     Function: LoadEngineKernel
    ' 
    '     Sub: SetFilterHandles, SetKernelLoops
    ' 
    ' Class KernelDriver
    ' 
    '     Function: __loadEngineKernel, get_DataSerials, Get_ObjectHandlers, LoadEngineKernel, Run
    ' 
    '     Sub: __innerTicks, __invokeDataAcquisition, (+2 Overloads) Dispose, SetFilterHandles, SetKernelLoops
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ApplicationServices.Terminal.Utility
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.Serialization

''' <summary>
''' 内核驱动程序的接口
''' </summary>
Public Interface IKernelDriver : Inherits ItaskDriver

    Function LoadEngineKernel(kernel As IReactorMachine) As Integer

    Sub SetKernelLoops(n As Integer)
    Sub SetFilterHandles(hwnds As Long())
End Interface

''' <summary>
''' Driver of the GCModeller system kernel.(计算引擎核心的驱动程序)
''' </summary>
''' <remarks></remarks>
Public Class KernelDriver(Of T, Dynamics As GCModeller.Framework.Kernel_Driver.IDynamicsExpression(Of T),
                              Kernel As GCModeller.Framework.Kernel_Driver.ReactorMachine(Of T, Dynamics))

    Implements IKernelDriver
    Implements IDisposable
    Implements IDriver_DataSource_Adapter(Of T)

    Protected __engineKernel As Kernel
    Protected _innerKernelDataAdapter As DataAdapter(Of T, DataSourceHandler(Of T))

    ''' <summary>
    ''' The current ticks since from the start of running.
    ''' (从运行开始后到当前的时间中所流逝的内核循环次数)
    ''' </summary>
    ''' <remarks></remarks>
    Protected _runtimeTicks As Integer

    ''' <summary>
    ''' Load the simulation kernel into the calculation kernel driver and then initialize a data adapter for the kernel.
    ''' (加载计算内核，并且为该内核初始化一个数据采集适配器对象.)
    ''' </summary>
    ''' <param name="kernel"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function LoadEngineKernel(Kernel As Kernel) As Integer
        Return __loadEngineKernel(Kernel)
    End Function

    Protected Function __loadEngineKernel(kernel As IReactorMachine) As Integer Implements IKernelDriver.LoadEngineKernel
        __engineKernel = kernel
        _innerKernelDataAdapter = New DataAdapter(Of T, DataSourceHandler(Of T))
        Return __engineKernel.Initialize()
    End Function

    ''' <summary>
    ''' The engine kernel driver running the loadded kernel object.(内核驱动程序运行已经加载的内核程序)
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overridable Function Run() As Integer Implements IKernelDriver.Run
        Dim Sw As Stopwatch = Stopwatch.StartNew, p As Double = 0

        Using busy As New CBusyIndicator(start:=True)
            For Me._runtimeTicks = 0 To __engineKernel.IterationLoops
                Call Me.__innerTicks(p, Sw)
            Next
        End Using

        Return 0
    End Function

    Private Sub __innerTicks(ByRef p As Double, Sw As Stopwatch)
        Call __engineKernel.TICK()
        Call __invokeDataAcquisition()

        Dim n As Double = _runtimeTicks / __engineKernel.IterationLoops * 100

        If n - p > 1 Then
            p = n
            Call $"  ---> [{Sw.ElapsedMilliseconds}ms] {p}%".__DEBUG_ECHO
        End If
    End Sub

    Public Sub SetFilterHandles(hwnds As Long()) Implements IKernelDriver.SetFilterHandles
        Call _innerKernelDataAdapter.SetFiltedHandles(hwnds)
    End Sub

    ''' <summary>
    ''' 数据采集程序的驱动句柄
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub __invokeDataAcquisition()
        Dim chunkBuffer = (From data0Expr As Dynamics
                           In __engineKernel.get_Expressions.AsParallel
                           Let row As DataSourceHandler(Of T) = New DataSourceHandler(Of T) With {
                               .Handle = data0Expr.Address,
                               .Value = data0Expr.Value,
                               .TimeStamp = Me._runtimeTicks
                           }
                           Select row).ToArray
        Call _innerKernelDataAdapter.DataAcquiring(chunkBuffer)
    End Sub

    Public Function Get_ObjectHandlers() As DataStorage.FileModel.ObjectHandle() Implements IDriver_DataSource_Adapter(Of T).get_ObjectHandlers
        Return (From data0Expr As Dynamics In __engineKernel.get_Expressions.AsParallel
                Let __innerHandle = New DataStorage.FileModel.ObjectHandle With {
                    .Handle = data0Expr.Address,
                    .ID = data0Expr.Key
                }
                Select __innerHandle).ToArray
    End Function

    Public Function get_DataSerials() As DataStorage.FileModel.DataSerials(Of T)() Implements IDriver_DataSource_Adapter(Of T).get_DataSerials
        Return _innerKernelDataAdapter.FetchData(Get_ObjectHandlers)
    End Function

#Region "IDisposable Support"
    Private disposedValue As Boolean ' To detect redundant calls

    ' IDisposable
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not Me.disposedValue Then
            If disposing Then
                ' TODO: dispose managed state (managed objects).
            End If

            ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
            ' TODO: set large fields to null.
        End If
        Me.disposedValue = True
    End Sub

    ' TODO: override Finalize() only if Dispose( disposing As Boolean) above has code to free unmanaged resources.
    'Protected Overrides Sub Finalize()
    '    ' Do not change this code.  Put cleanup code in Dispose( disposing As Boolean) above.
    '    Dispose(False)
    '    MyBase.Finalize()
    'End Sub

    ' This code added by Visual Basic to correctly implement the disposable pattern.
    Public Sub Dispose() Implements IDisposable.Dispose
        ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub
#End Region

    Public Sub SetKernelLoops(n As Integer) Implements IKernelDriver.SetKernelLoops
        __engineKernel.IterationLoops = n
    End Sub
End Class

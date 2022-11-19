#Region "Microsoft.VisualBasic::51974bc59a5ab7b4c359f1f4f8ec1ed4, GCModeller\engine\GCModeller.Framework.Kernel_Driver\DriverAPI.vb"

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

    '   Total Lines: 84
    '    Code Lines: 57
    ' Comment Lines: 14
    '   Blank Lines: 13
    '     File Size: 3.95 KB


    ' Module DriverAPI
    ' 
    '     Function: CsvDataStorage, DetectedDriver, RunDriver, WriteData
    ' 
    '     Sub: set_DataAdapter_FilteringHandles, SetKernelLoops
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData

<Package("GCModeller.Engine_Driver",
                    Description:="This namespace define the generic method to running the GCModeller simulation engine. You can using the API functions in this module to run the GCModeller simulation.",
                    Publisher:="amethyst.asuka@gcmodeller.org")>
Module DriverAPI

    ''' <summary>
    ''' 根据所将要加载的<paramref name="kernel">内核对象</paramref>的泛型接口的参数来初始化内核的驱动程序
    ''' 仅接受<see cref="Boolean"></see>, <see cref="Double"></see>, <see cref="Integer"></see>这三种类型的参数
    ''' </summary>
    ''' <param name="kernel"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    ''' 
    <ExportAPI("Driver.LoadKernel", Info:="Please ensure that the intialization code for the kernel can be calling in driver this function.")>
    Public Function DetectedDriver(kernel As IReactorMachine) As GCModeller.Framework.Kernel_Driver.IKernelDriver
        Dim Type As Type = kernel.GetType.BaseType
        Dim initTypeParams As Type() = Type.GenericTypeArguments
        Dim SampleType = initTypeParams.First
        Dim DriverType = GetType(Framework.Kernel_Driver.KernelDriver(Of ,,)) '下面的步骤开始初始化内核驱动程序
        Dim DynamicsType As Type = initTypeParams.Last
        DriverType = DriverType.MakeGenericType(New System.Type() {SampleType, DynamicsType, kernel.GetType})
        Dim Driver As IKernelDriver = Activator.CreateInstance(DriverType)

        Call Driver.LoadEngineKernel(kernel)  '加载计算内核并且进行初始化

        Return Driver
    End Function

    ''' <summary>
    ''' 设置内核执行的循环次数
    ''' </summary>
    ''' <param name="driver"></param>
    ''' <param name="cycle"></param>
    ''' <remarks></remarks>
    <ExportAPI("Driver.Init.Set.KernelCycle")>
    Public Sub SetKernelLoops(driver As IKernelDriver, cycle As Integer)
        Call driver.SetKernelLoops(cycle)
    End Sub

    <ExportAPI("Driver.Init.Set.FilteringHandles")>
    Public Sub set_DataAdapter_FilteringHandles(driver As IKernelDriver, hwnds As IEnumerable(Of Long))
        Call driver.SetFilterHandles(hwnds)
    End Sub

    <ExportAPI("Driver.Run")>
    Public Function RunDriver(driver As IKernelDriver) As Integer
        Return driver.Run
    End Function

    <ExportAPI("Driver.OpenHandle.Csv")>
    Public Function CsvDataStorage(driver As IKernelDriver) As IDataStorage
        Dim type = driver.GetType.GenericTypeArguments.First
        Dim service As IDataStorage

        If type = GetType(Integer) Then
            service = New MsCsvChunkBuffer(Of Integer)().AttachKernel(driver)
        ElseIf type = GetType(Boolean) Then
            service = New MsCsvChunkBuffer(Of Boolean)().AttachKernel(driver)
        ElseIf type = GetType(Double) Then
            service = New MsCsvChunkBuffer(Of Double)().AttachKernel(driver)
        Else
            Throw New NotSupportedException($"Target data type ""{type.FullName}"" is not a supported type!")
        End If

        Return service
    End Function

    <ExportAPI("Close()", Info:="Close the data storage handle and flush the data into the target storage media which was specific by the url parameter.")>
    Public Function WriteData(service As IDataStorage, url As String) As Boolean
        Return service.WriteData(url)
    End Function

    Private ReadOnly __sampleDataTypes As Dictionary(Of Type, Func(Of IDataSourceHandle)) =
        New Dictionary(Of Type, Func(Of IDataSourceHandle)) From {
 _
        {GetType(Integer), Function() New Framework.Kernel_Driver.StateEnumerationsSample},
        {GetType(Double), Function() New Framework.Kernel_Driver.EntityQuantitySample},
        {GetType(Boolean), Function() New Framework.Kernel_Driver.TransitionStateSample}
    }

End Module

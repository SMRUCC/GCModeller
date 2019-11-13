#Region "Microsoft.VisualBasic::0c1a2d59fc65105373c5cd911d6f3266, engine\GCModeller\PlugIns\ModuleLoader.vb"

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

    '     Class ModuleLoader
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '         Function: LoadMainModule, LoadModel, ToString
    ' 
    '         Sub: DisableModule, (+2 Overloads) Dispose, LoadModules
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic

Namespace PlugIns

    ''' <summary>
    ''' External model loading operator.(外部计算模型加载对象，在加载外部计算模块的同时，还会向计算框架中的数据采集服务处理队列之中加载构造出来的数据采集服务)
    ''' </summary>
    ''' <remarks>先从外部加载对象模型，当加载计算模型之后再加载数据模型</remarks>
    Public Class ModuleLoader : Implements System.IDisposable

        Dim KernelModule As SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.ObjectModels.SubSystem.CellSystem
        Dim EngineSystem As SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.Engine.GCModeller

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="Kernel">目标计算引擎框架类型实例</param>
        ''' <remarks></remarks>
        Sub New(Kernel As SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.Engine.GCModeller)
            Me.EngineSystem = Kernel
            Me.KernelModule = Kernel.KernelModule
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="AssemblyPath"></param>
        ''' <returns>返回所加载的外部系统模块的模块类型，当加载不成功的时候返回-1</returns>
        ''' <remarks></remarks>
        Public Function LoadModel(AssemblyPath As String) As ISystemFrameworkEntry.Types
            If Not FileIO.FileSystem.FileExists(AssemblyPath) Then 'When the filesystem object can not find the assembly file, then this loading operation was abort.
                Return ISystemFrameworkEntry.Types.BROKEN_ASSEMBLY
            Else
                AssemblyPath = IO.Path.GetFullPath(AssemblyPath) 'Assembly.LoadFile required full path of a program assembly file.
            End If

            Dim Entry = ModuleLoader.LoadMainModule(AssemblyPath)
            Dim System As PlugIns.ISystemFrameworkEntry.ISystemFramework =
                DirectCast(Activator.CreateInstance(type:=Entry.ISystem), PlugIns.ISystemFrameworkEntry.ISystemFramework)  '生成计算框架的实例

            Call ModuleLoader.ExternalModuleLoadMethods(Entry.Type)(KernelModule, System)
            Call EngineSystem.DataAcquisitionService.Join(System.CreateServiceInstanceSerials)   '向处理队列之中加载数据采集服务模块

            Return Entry.Type
        End Function

        Public Overrides Function ToString() As String
            Return KernelModule.ToString
        End Function

        ''' <summary>
        ''' 下列类型的模块将不会被加载
        ''' </summary>
        ''' <remarks></remarks>
        Dim DisabledModules As ISystemFrameworkEntry.Types()

        Public Sub DisableModule(Modules As String())
            Dim LQuery = From ModuleName As String In Modules Select ModuleLoader.ModuleEnums(ModuleName.ToLower.Trim) '
            DisabledModules = LQuery.ToArray
        End Sub

        ''' <summary>
        ''' 外部系统模块的文件路径
        ''' </summary>
        ''' <param name="Systems"></param>
        ''' <remarks></remarks>
        Public Sub LoadModules(Systems As String())
            Dim ModuleNotLoad As List(Of ISystemFrameworkEntry.Types) = New List(Of ISystemFrameworkEntry.Types) From {
                ISystemFrameworkEntry.Types.Metabolism, ISystemFrameworkEntry.Types.ExpressionRegulationNetwork} '系统计算核心之中当前尚未加载进入计算框架的系统模块

            For Each SystemAssembly As String In Systems
                Dim System = LoadModel(AssemblyPath:=SystemAssembly)
                Call ModuleNotLoad.Remove(System)  '移除已经加载好的系统模块类型标识符
            Next

            For Each DisabledModule In DisabledModules
                Call ModuleNotLoad.Remove(DisabledModule)
            Next

            Call Load_InternalSystemModule(ModuleNotLoad)  '对于尚未加载的系统模块则，加载内部的模块
        End Sub

        Protected Friend Shared Function LoadMainModule(AssemblyPath As String) As ISystemFrameworkEntry
            Dim Assembly As System.Reflection.Assembly = System.Reflection.Assembly.LoadFrom(AssemblyPath)
            Dim EntryType As System.Type = GetType(ISystemFrameworkEntry)
            Dim AssemblyDefinedTypes = Assembly.DefinedTypes
            Dim FindModule = From [Class] As System.Reflection.TypeInfo In AssemblyDefinedTypes
                             Let attributes As Object() = [Class].GetCustomAttributes(EntryType, False)
                             Where attributes.Count = 1
                             Select DirectCast(attributes(0), ISystemFrameworkEntry).Initialize([Class]) '
            FindModule = FindModule.ToArray
            If FindModule.Count = 0 Then
                Return Nothing
            Else
                Return FindModule.First
            End If
        End Function

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

    End Class
End Namespace

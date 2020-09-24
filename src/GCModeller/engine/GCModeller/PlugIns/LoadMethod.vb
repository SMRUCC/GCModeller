#Region "Microsoft.VisualBasic::6f21de54664c17080e1384928851941e, engine\GCModeller\PlugIns\LoadMethod.vb"

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
    '         Sub: Load_InternalSystemModule, LoadExpressionNetwork, LoadInternalMetabolismModule, LoadInternalRegulationModule, LoadMetabolism
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.ObjectModels
Imports Microsoft.VisualBasic

Namespace PlugIns

    Partial Class ModuleLoader
        Public Shared ReadOnly ExternalModuleLoadMethods As Dictionary(Of ISystemFrameworkEntry.Types, System.Action(Of EngineSystem.ObjectModels.SubSystem.CellSystem, PlugIns.ISystemFrameworkEntry.ISystemFramework)) =
            New Dictionary(Of ISystemFrameworkEntry.Types, Action(Of EngineSystem.ObjectModels.SubSystem.CellSystem, PlugIns.ISystemFrameworkEntry.ISystemFramework)) From {
                {ISystemFrameworkEntry.Types.Metabolism, AddressOf ModuleLoader.LoadMetabolism},
                {ISystemFrameworkEntry.Types.ExpressionRegulationNetwork, AddressOf ModuleLoader.LoadExpressionNetwork}}

        Public Shared ReadOnly InternalModuleLoadmethods As Dictionary(Of ISystemFrameworkEntry.Types, System.Action(Of EngineSystem.ObjectModels.SubSystem.CellSystem, EngineSystem.Engine.GCModeller)) =
            New Dictionary(Of ISystemFrameworkEntry.Types, Action(Of EngineSystem.ObjectModels.SubSystem.CellSystem, EngineSystem.Engine.GCModeller)) From {
                {ISystemFrameworkEntry.Types.Metabolism, AddressOf LoadInternalMetabolismModule},
                {ISystemFrameworkEntry.Types.ExpressionRegulationNetwork, AddressOf LoadInternalRegulationModule}}

        Public Shared ReadOnly ModuleEnums As Dictionary(Of String, ISystemFrameworkEntry.Types) = New Dictionary(Of String, ISystemFrameworkEntry.Types) From {
            {"metabolism", ISystemFrameworkEntry.Types.Metabolism},
            {"expression", ISystemFrameworkEntry.Types.ExpressionRegulationNetwork}}

        Protected Friend Shared Sub LoadMetabolism(CellSystem As EngineSystem.ObjectModels.SubSystem.CellSystem, Metabolism As PlugIns.ISystemFrameworkEntry.ISystemFramework)
            CellSystem.Metabolism = Metabolism
        End Sub

        Protected Friend Shared Sub LoadExpressionNetwork(CellSystem As EngineSystem.ObjectModels.SubSystem.CellSystem, RegulationNetwork As PlugIns.ISystemFrameworkEntry.ISystemFramework)
            CellSystem.ExpressionRegulationNetwork = RegulationNetwork
        End Sub

        ''' <summary>
        ''' 计算核心加载系统内部的计算模块
        ''' </summary>
        ''' <param name="ModuleNotLoaded">尚未加载的系统模块的列表</param>
        ''' <remarks></remarks>
        Private Sub Load_InternalSystemModule(ModuleNotLoaded As List(Of ISystemFrameworkEntry.Types))
            For Each System As ISystemFrameworkEntry.Types In ModuleNotLoaded   '使用系统内部的模块来加载系统中尚未加载的模块
                Call InternalModuleLoadmethods(System)(KernelModule, Me.EngineSystem)
            Next
        End Sub

        ''' <summary>
        ''' 加载内部的代谢组计算模块，并初始化代谢组计算模块的数据采集服务
        ''' </summary>
        ''' <param name="KernelModule"></param>
        ''' <param name="EngineSystem"></param>
        ''' <remarks></remarks>
        Private Shared Sub LoadInternalMetabolismModule(KernelModule As EngineSystem.ObjectModels.SubSystem.CellSystem, EngineSystem As EngineSystem.Engine.GCModeller)
            Dim Metabolism = New EngineSystem.ObjectModels.SubSystem.MetabolismCompartment(CellSystem:=KernelModule)
            KernelModule.Metabolism = Metabolism
        End Sub

        Private Shared Sub LoadInternalRegulationModule(KernelModule As EngineSystem.ObjectModels.SubSystem.CellSystem, EngineSystem As EngineSystem.Engine.GCModeller)
            Dim ExpressionRegulationNetwork = New EngineSystem.ObjectModels.SubSystem.ExpressionSystem.ExpressionRegulationNetwork(CellSystem:=KernelModule)
            KernelModule.ExpressionRegulationNetwork = ExpressionRegulationNetwork
        End Sub
    End Class
End Namespace

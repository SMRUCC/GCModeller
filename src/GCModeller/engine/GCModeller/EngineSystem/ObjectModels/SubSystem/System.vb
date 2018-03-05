#Region "Microsoft.VisualBasic::b8bd6cff74f65b997bbc255b1ca77404, engine\GCModeller\EngineSystem\ObjectModels\SubSystem\System.vb"

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

    '     Class SystemObject
    ' 
    ' 
    '         Interface I_SystemModel
    ' 
    '             Properties: SystemLogging
    ' 
    '             Function: get_RuntimeContainer, Initialize
    ' 
    '             Sub: MemoryDump
    ' 
    ' 
    ' 
    '     Class SystemObjectModel
    ' 
    '         Properties: SystemLogging
    ' 
    '         Function: get_runtimeContainer
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ApplicationServices.Debugging.Logging
Imports SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.Engine
Imports SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.RuntimeObjects

Namespace EngineSystem.ObjectModels.SubSystem

    Public MustInherit Class SystemObject : Inherits RuntimeObject
        Protected Friend MustOverride ReadOnly Property SystemLogging As LogFile

        Public Interface I_SystemModel
            ReadOnly Property SystemLogging As LogFile

            Sub MemoryDump(Dir As String)
            Function get_RuntimeContainer() As IContainerSystemRuntimeEnvironment
            Function Initialize() As Integer
        End Interface
    End Class

    Public MustInherit Class SystemObjectModel : Inherits SystemObject
        Implements SystemObject.I_SystemModel

        ''' <summary>
        ''' 当前子系统模块的上一层系统模块
        ''' </summary>
        ''' <remarks></remarks>
        Protected I_RuntimeContainer As IContainerSystemRuntimeEnvironment

        Public MustOverride Function Initialize() As Integer Implements SystemObject.I_SystemModel.Initialize

        Protected Friend Overrides ReadOnly Property SystemLogging As LogFile Implements SystemObject.I_SystemModel.SystemLogging
            Get
                Return I_RuntimeContainer.SystemLogging
            End Get
        End Property

        MustOverride Sub MemoryDump(Dir As String) Implements SystemObject.I_SystemModel.MemoryDump

        Public Overridable Function get_runtimeContainer() As IContainerSystemRuntimeEnvironment Implements SystemObject.I_SystemModel.get_RuntimeContainer
            Return I_RuntimeContainer
        End Function
    End Class
End Namespace

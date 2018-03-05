#Region "Microsoft.VisualBasic::5adba1d4ec81a9f6698d69bc2fbae4ac, engine\GCModeller\EngineSystem\Engine\Abstract.vb"

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

    '     Interface IContainerSystemRuntimeEnvironment
    ' 
    '         Properties: ConfigurationData, RuntimeTicks, SystemLogging, SystemVariable
    ' 
    '         Function: GetArguments
    ' 
    '         Sub: MemoryDump
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ApplicationServices.Debugging.Logging
Imports SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.Engine.Configuration

Namespace EngineSystem.Engine

    Public Interface IContainerSystemRuntimeEnvironment
        ReadOnly Property SystemLogging As LogFile
        ReadOnly Property RuntimeTicks As Long
        ReadOnly Property SystemVariable(var As String) As String
        ReadOnly Property ConfigurationData As ConfigReader

        Sub MemoryDump(DumpFile As String)

        ''' <summary>
        ''' 从最开始输入程序的命令行之中获取目标开关的参数
        ''' </summary>
        ''' <param name="Name"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function GetArguments(Name As String) As String
    End Interface
End Namespace

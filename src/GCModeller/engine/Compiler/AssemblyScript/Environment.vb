#Region "Microsoft.VisualBasic::f5510a1b61a1a5f73797fc4b28e33fa0, engine\Compiler\AssemblyScript\Environment.vb"

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

    '     Class Environment
    ' 
    '         Properties: config, workingModel
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.CommandLine
Imports SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.v2

Namespace AssemblyScript

    ''' <summary>
    ''' virtual cell model assembly compiler session.
    ''' </summary>
    Public Class Environment

        ''' <summary>
        ''' ENV指令的环境变量配置设置结果
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property config As CommandLine
        Public ReadOnly Property workingModel As VirtualCell



    End Class
End Namespace

#Region "Microsoft.VisualBasic::492f9513b2bc143ff16df932f8f6a901, engine\Model\Cellular\Processes.vb"

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

    '   Total Lines: 20
    '    Code Lines: 7 (35.00%)
    ' Comment Lines: 12 (60.00%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 1 (5.00%)
    '     File Size: 551 B


    '     Enum Processes
    ' 
    '         MetabolicProcess, Transcription, Translation
    ' 
    '  
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace Cellular

    ''' <summary>
    ''' The enumeration of all biological process category.(枚举出所有生物学过程类型信息)
    ''' </summary>
    Public Enum Processes
        ''' <summary>
        ''' 转录过程
        ''' </summary>
        Transcription
        ''' <summary>
        ''' 翻译过程
        ''' </summary>
        Translation
        ''' <summary>
        ''' 代谢过程，主要是产物对上游的抑制类型
        ''' </summary>
        MetabolicProcess
    End Enum
End Namespace

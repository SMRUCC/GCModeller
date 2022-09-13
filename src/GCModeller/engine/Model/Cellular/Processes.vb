#Region "Microsoft.VisualBasic::4afeab892559a058bc05d7edc4f2a8f4, GCModeller\engine\Model\Cellular\Processes.vb"

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
    '    Code Lines: 7
    ' Comment Lines: 12
    '   Blank Lines: 1
    '     File Size: 468 B


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
    ''' 过程类型
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

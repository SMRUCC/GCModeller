#Region "Microsoft.VisualBasic::aadffd83a23647472c416adb7658aa50, sub-system\BNLearn\Intervention\InterventionMode.vb"

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

    '   Total Lines: 16
    '    Code Lines: 8 (50.00%)
    ' Comment Lines: 7 (43.75%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 1 (6.25%)
    '     File Size: 463 B


    '     Enum InterventionMode
    ' 
    '         Custom, Knockdown, Knockout, Overexpression
    ' 
    '  
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace Intervention

    ''' <summary>
    ''' 干预模式
    ''' </summary>
    Public Enum InterventionMode
        ''' <summary>基因敲除（设为0）</summary>
        Knockout
        ''' <summary>基因过表达（设为高值）</summary>
        Overexpression
        ''' <summary>基因下调（设为低值）</summary>
        Knockdown
        ''' <summary>自定义值干预</summary>
        Custom
    End Enum
End Namespace

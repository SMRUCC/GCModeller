#Region "Microsoft.VisualBasic::0999bdaa4b86c3dc1d113732db536e38, core\Bio.Assembly\ProteinModel\Chou-Fasman\SecondaryStructures.vb"

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

    '   Total Lines: 33
    '    Code Lines: 9 (27.27%)
    ' Comment Lines: 20 (60.61%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 4 (12.12%)
    '     File Size: 821 B


    '     Enum SecondaryStructures
    ' 
    ' 
    '  
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.ComponentModel

Namespace ProteinModel.ChouFasmanRules

    ''' <summary>
    ''' 蛋白质的二级结构分类
    ''' </summary>
    ''' <remarks></remarks>
    Public Enum SecondaryStructures

        ''' <summary>
        ''' alpha螺旋
        ''' </summary>
        ''' <remarks></remarks>
        <Description("@")> AlphaHelix
        ''' <summary>
        ''' Beta折叠
        ''' </summary>
        ''' <remarks></remarks>
        <Description("-")> BetaSheet
        ''' <summary>
        ''' Beta转角
        ''' </summary>
        ''' <remarks></remarks>
        <Description("^")> BetaTurn
        ''' <summary>
        ''' 无规则卷曲
        ''' </summary>
        ''' <remarks></remarks>
        <Description("&")> Coils

    End Enum
End Namespace

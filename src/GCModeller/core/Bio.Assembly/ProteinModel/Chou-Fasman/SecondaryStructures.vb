﻿#Region "Microsoft.VisualBasic::d6936bef732765b7a4bd813a1ddf4eea, core\Bio.Assembly\ProteinModel\Chou-Fasman\SecondaryStructures.vb"

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

'   Total Lines: 30
'    Code Lines: 8 (26.67%)
' Comment Lines: 20 (66.67%)
'    - Xml Docs: 100.00%
' 
'   Blank Lines: 2 (6.67%)
'     File Size: 710 B


'     Enum SecondaryStructures
' 
'         AlphaHelix, BetaSheet, BetaTurn, Coils
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

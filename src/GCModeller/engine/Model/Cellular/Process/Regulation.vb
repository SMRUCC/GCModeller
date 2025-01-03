﻿#Region "Microsoft.VisualBasic::476c08a1297fa0dc010e61f3c75fb724, engine\Model\Cellular\Process\Regulation.vb"

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

    '   Total Lines: 37
    '    Code Lines: 15 (40.54%)
    ' Comment Lines: 16 (43.24%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 6 (16.22%)
    '     File Size: 1.08 KB


    '     Structure Regulation
    ' 
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Language

Namespace Cellular.Process

    ''' <summary>
    ''' the regulation model for the biological process
    ''' </summary>
    Public Structure Regulation

        ''' <summary>
        ''' 这个调控过程的生物学名称
        ''' </summary>
        Public name As String
        ''' <summary>
        ''' Compound / RNA / Proteins
        ''' </summary>
        Public regulator As String
        ''' <summary>
        ''' The regulated target process name
        ''' </summary>
        Public process As String
        ''' <summary>
        ''' The type of the target process
        ''' </summary>
        Public type As Processes
        ''' <summary>
        ''' + positive: accelerate
        ''' + negative: inhibition
        ''' </summary>
        Public effects As Double

        Public Overrides Function ToString() As String
            Dim effectString$ = "accelerate" Or "inhibition".When(effects < 0)
            Dim descript$ = $"[{type.ToString}] {regulator} {effectString} of [{process}]"

            Return descript
        End Function

    End Structure
End Namespace

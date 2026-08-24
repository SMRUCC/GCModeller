#Region "Microsoft.VisualBasic::d713b3d54a0657e8bb7ee5cbad8e778d, analysis\Microarray\CausalModeling\MeasurementModels.vb"

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

    '   Total Lines: 29
    '    Code Lines: 5 (17.24%)
    ' Comment Lines: 23 (79.31%)
    '    - Xml Docs: 91.30%
    ' 
    '   Blank Lines: 1 (3.45%)
    '     File Size: 911 B


    ' Enum MeasurementModels
    ' 
    ' 
    '  
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.ComponentModel

''' <summary>
''' 
''' </summary>
''' <remarks>
''' flowchart LR
'''   subgraph A [反映型测量模型]
'''     LV[潜变量 ξ] --“导致”--> MV1[指标 X1]
'''     LV[潜变量 ξ] --“导致”--> MV2[指标 X2]
'''     LV[潜变量 ξ] --“导致”--> MV3[指标 X3]
'''   end
''' 
'''   subgraph B [形成型测量模型]
'''     MV4[指标 X1] --“构成”--> LV2[潜变量 ξ]
'''     MV5[指标 X2] --“构成”--> LV2[潜变量 ξ]
'''     MV6[指标 X3] --“构成”--> LV2[潜变量 ξ]
'''   end
''' </remarks>
Public Enum MeasurementModels
    ''' <summary>
    ''' *Reflective measurement model*（反映型测量模型）
    ''' </summary>
    <Description("A:Reflective")> A
    ''' <summary>
    ''' *Formative measurement model*（形成型测量模型）
    ''' </summary>
    <Description("B:Formative")> B
End Enum

#Region "Microsoft.VisualBasic::33db5738e7f1845b3cb9326c9a49940d, engine\Model\MetabolicThermodynamics\ThermoBoundResult.vb"

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

    '   Total Lines: 22
    '    Code Lines: 9 (40.91%)
    ' Comment Lines: 8 (36.36%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 5 (22.73%)
    '     File Size: 743 B


    '     Class ThermoBoundResult
    ' 
    '         Properties: DeltaG0, DeltaGActual, Direction, LowerBound, UpperBound
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace MetabolicThermodynamics

    ''' <summary>
    ''' 计算结果封装
    ''' </summary>
    Public Class ThermoBoundResult
        ''' <summary>反应标准吉布斯自由能变 ΔG'0 (kJ/mol)</summary>
        Public Property DeltaG0 As Double

        ''' <summary>实际吉布斯自由能变 ΔG' (kJ/mol)</summary>
        Public Property DeltaGActual As Double

        ''' <summary>判定出的反应方向性描述</summary>
        Public Property Direction As String

        ''' <summary>计算得出的 FBA 下界</summary>
        Public Property LowerBound As Double

        ''' <summary>计算得出的 FBA 上界</summary>
        Public Property UpperBound As Double
    End Class
End Namespace

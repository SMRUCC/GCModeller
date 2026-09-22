#Region "Microsoft.VisualBasic::5a4f7dad212c61919193b58762448cc1, engine\Cella\Networks\TranslationSystem.vb"

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

    '   Total Lines: 21
    '    Code Lines: 13 (61.90%)
    ' Comment Lines: 3 (14.29%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 5 (23.81%)
    '     File Size: 578 B


    ' Class TranslationSystem
    ' 
    '     Constructor: (+1 Overloads) Sub New
    ' 
    '     Function: GetStats
    ' 
    '     Sub: RunStep
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Math.Calculus.Dynamics

''' <summary>
''' 将基因表达出来的mRNA翻译为蛋白质，采用ODEs动力学系统来建模
''' </summary>
Public Class TranslationSystem : Inherits SubNetwork

    ReadOnly core As SolverIterator

    Sub New(cell As VirtualCella)
        Call MyBase.New(cell)
    End Sub

    Public Overrides Sub RunStep()
        Call core.Tick()
    End Sub

    Public Overrides Function GetStats() As Dictionary(Of String, Double)
        Throw New NotImplementedException()
    End Function
End Class


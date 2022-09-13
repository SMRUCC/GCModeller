#Region "Microsoft.VisualBasic::6e776ae82c22bda61ebaaf5597d9aea2, GCModeller\engine\GCModeller.Framework.Kernel_Driver\Driver\Engine.vb"

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

    '   Total Lines: 86
    '    Code Lines: 0
    ' Comment Lines: 70
    '   Blank Lines: 16
    '     File Size: 2.98 KB


    ' 
    ' /********************************************************************************/

#End Region

'#Region "Microsoft.VisualBasic::a09eb030b89bb37be40218754f55b942, GCModeller.Framework.Kernel_Driver\Driver\Engine.vb"

'    ' Author:
'    ' 
'    '       asuka (amethyst.asuka@gcmodeller.org)
'    '       xie (genetics@smrucc.org)
'    '       xieguigang (xie.guigang@live.com)
'    ' 
'    ' Copyright (c) 2018 GPL3 Licensed
'    ' 
'    ' 
'    ' GNU GENERAL PUBLIC LICENSE (GPL3)
'    ' 
'    ' 
'    ' This program is free software: you can redistribute it and/or modify
'    ' it under the terms of the GNU General Public License as published by
'    ' the Free Software Foundation, either version 3 of the License, or
'    ' (at your option) any later version.
'    ' 
'    ' This program is distributed in the hope that it will be useful,
'    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
'    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
'    ' GNU General Public License for more details.
'    ' 
'    ' You should have received a copy of the GNU General Public License
'    ' along with this program. If not, see <http://www.gnu.org/licenses/>.



'    ' /********************************************************************************/

'    ' Summaries:

'    ' Class IterationMathEngine
'    ' 
'    '     Properties: RuntimeTicks
'    ' 
'    '     Constructor: (+1 Overloads) Sub New
'    '     Function: Initialize, Run
'    ' 
'    ' /********************************************************************************/

'#End Region

'Imports SMRUCC.genomics.GCModeller.Framework.Kernel_Driver.LDM

'''' <summary>
'''' The simulation mechanism of this calculation math engine is that we calculates the finited iteration of the dynamics expression in this engine.
'''' (计算引擎的基本工作原理是对动力学方程组进行有限次的迭代计算)
'''' </summary>
'''' <remarks></remarks>
'Public MustInherit Class IterationMathEngine(Of T_Model As ModelBaseType) : Inherits ReactorMachine(Of Double, Expression)

'    ''' <summary>
'    ''' 驱动本计算引擎对象的数据采集服务对象的工作
'    ''' </summary>
'    ''' <remarks></remarks>
'    Protected Friend __runDataAdapter As System.Action
'    Protected _innerDataModel As T_Model
'    Protected _RTime As Integer

'    Sub New(Model As T_Model)
'        Me._innerDataModel = Model
'    End Sub

'    Public Overrides Function Initialize() As Integer
'        Return -1
'    End Function

'    Public Overridable Function Run() As Integer
'        For Me._RTime = 0 To IterationLoops
'            Call __innerTicks(_RTime)
'            Call __runDataAdapter()
'        Next

'        Return 0
'    End Function

'    Protected MustOverride Overrides Function __innerTicks(KernelCycle As Integer) As Integer

'    Public Overrides ReadOnly Property RuntimeTicks As Long
'        Get
'            Return _RTime
'        End Get
'    End Property
'End Class

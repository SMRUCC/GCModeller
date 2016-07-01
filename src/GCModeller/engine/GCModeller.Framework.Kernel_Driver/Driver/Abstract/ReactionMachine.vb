#Region "Microsoft.VisualBasic::de446b7eb8395e3a35f68909ff5090b0, ..\GCModeller\engine\GCModeller.Framework.Kernel_Driver\Driver\Abstract\ReactionMachine.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
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

#End Region


''' <summary>
''' The very base type of a network simulator. (一个IterationMathEngine类型的对象之中可能会存在多个反应机器类型的子模块)
''' </summary>
''' <typeparam name="DataType"></typeparam>
''' <typeparam name="TExpr"></typeparam>
''' <remarks></remarks>
Public MustInherit Class ReactorMachine(Of DataType, TExpr As IDynamicsExpression(Of DataType))
    Implements IReactorMachine

#Region "Public Property & Fields"

    ''' <summary>
    ''' The network entity that using for the system behaviour simulation.(所需要被进行模拟计算的网络对象实体)
    ''' </summary>
    ''' <remarks></remarks>
    <DumpNode> Protected _DynamicsExprs As TExpr()
    <DumpNode> Public Property IterationLoops As Integer Implements IReactorMachine.IterationCycle
    <DumpNode> MustOverride ReadOnly Property RuntimeTicks As Long
#End Region

#Region "Public Methods"

    Public MustOverride Function Initialize() As Integer Implements IReactorMachine.Initialize
    Protected MustOverride Function __innerTicks(KernelCycle As Integer) As Integer

    Public Overridable Function TICK() As Integer Implements IReactorMachine.TICK
        Return __innerTicks(RuntimeTicks)
    End Function

    Friend Function get_Expressions() As TExpr()
        Return _DynamicsExprs
    End Function
#End Region

End Class

''' <summary>
''' 生化反应器的接口
''' </summary>
Public Interface IReactorMachine

#Region "Public Property"

    ''' <summary>
    ''' The total kernel loop for this simulation experiment.(总的内核循环次数)
    ''' </summary>
    ''' <remarks></remarks>
    Property IterationCycle As Integer
#End Region

#Region "Public Methods"

    Function Initialize() As Integer
    Function TICK() As Integer
#End Region

End Interface


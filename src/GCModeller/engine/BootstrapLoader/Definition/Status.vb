#Region "Microsoft.VisualBasic::3efc92beba64f0a2777802275297aad4, GCModeller\engine\BootstrapLoader\Definition\Status.vb"

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

    '   Total Lines: 52
    '    Code Lines: 27
    ' Comment Lines: 17
    '   Blank Lines: 8
    '     File Size: 1.94 KB


    '     Class Status
    ' 
    '         Properties: definition, name
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: IsCurrentStatus
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Dynamics.Core
Imports Engine2 = SMRUCC.genomics.GCModeller.ModellingEngine.BootstrapLoader.Engine.Engine

Namespace Definitions

    ''' <summary>
    ''' 细胞状态定义
    ''' </summary>
    ''' <remarks>
    ''' 在这里应该是通过各种代谢物分子之间的浓度相对百分比来定义诸如死亡或者细胞分裂之类的状态？
    ''' </remarks>
    Public Class Status

        ''' <summary>
        ''' 状态名称
        ''' </summary>
        ''' <returns></returns>
        Public Property name As String
        ''' <summary>
        ''' 当虚拟细胞反应容器<see cref="Vessel"/>中的<see cref="Vessel.MassEnvironment"/>浓度
        ''' 百分比接近于这个向量的百分比的时候就认为<see cref="name"/>状态或者细胞活动事件发生了
        ''' </summary>
        ''' <returns></returns>
        Public Property definition As Dictionary(Of String, Double)

        ReadOnly masslist$()
        ReadOnly status As Vector

        Sub New(name$, status As Dictionary(Of String, Double))
            Me.masslist = status.Keys.ToArray
            Me.status = status.Takes(masslist).AsVector
            Me.name = name
        End Sub

        Public Function IsCurrentStatus(engine As Engine2, cutoff#) As Boolean
            Dim current As Vector = engine _
                .getMass(masslist) _
                .Select(Function(mass) mass.Value) _
                .AsVector
            Dim diff As Vector

            current = current / current.Max
            diff = current - status

            ' 通过当前的代谢物物质间的百分比是否和定义的状态相似
            ' 来判断特定的事件是否发生
            Return diff.Average <= cutoff
        End Function
    End Class
End Namespace

#Region "Microsoft.VisualBasic::c32fe00675b2aa33838b4bc36dcbd5fd, GCModeller\engine\Dynamics\Core\Flux\Channel.vb"

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

    '   Total Lines: 128
    '    Code Lines: 71
    ' Comment Lines: 37
    '   Blank Lines: 20
    '     File Size: 4.71 KB


    '     Class Channel
    ' 
    '         Properties: bounds, direct, forward, ID, isBroken
    '                     reverse
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: CoverLeft, CoverRight, (+2 Overloads) minimalUnit, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository

Namespace Core

    ''' <summary>
    ''' Biological flux object model, the reaction channel.
    ''' 
    ''' (反应过程通道)
    ''' </summary>
    Public Class Channel : Implements INamedValue

        Friend ReadOnly left As Variable()
        Friend ReadOnly right As Variable()

        Public Property forward As Controls
        Public Property reverse As Controls

        ''' <summary>
        ''' 因为细胞微环境内的容量很小，没有办法使整个反应过程以很大的速率来进行
        ''' 在这里设置反应的正反过程这两个方向上的上下限？
        ''' </summary>
        ''' <returns></returns>
        Public Property bounds As Boundary

        ''' <summary>
        ''' 在衡量了<see cref="forward"/>和<see cref="reverse"/>的效应大小之后，当前的反应的方向
        ''' </summary>
        ''' <returns></returns>
        Public Overloads ReadOnly Property direct As Directions
            Get
                If forward > reverse Then
                    Return Directions.forward
                ElseIf reverse > forward Then
                    Return Directions.reverse
                Else
                    Return Directions.stop
                End If
            End Get
        End Property

        Public ReadOnly Property isBroken As Boolean

        Public Property ID As String Implements IKeyedEntity(Of String).Key

        Sub New(left As IEnumerable(Of Variable), right As IEnumerable(Of Variable))
            Me.left = left.ToArray
            Me.right = right.ToArray

            isBroken = Me.left.IsNullOrEmpty OrElse Me.right.IsNullOrEmpty
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="shares">事件并行化模拟因子</param>
        ''' <param name="regulation"></param>
        ''' <returns></returns>
        Public Function CoverLeft(shares As Dictionary(Of String, Double), regulation#) As Double
            If isBroken Then
                Return 0
            Else
                Return minimalUnit(shares, left, regulation, bounds.forward)
            End If
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="shares">事件并行化模拟因子</param>
        ''' <param name="regulation"></param>
        ''' <returns></returns>
        Public Function CoverRight(shares As Dictionary(Of String, Double), regulation#) As Double
            If isBroken Then
                Return 0
            Else
                Return minimalUnit(shares, right, regulation, bounds.reverse)
            End If
        End Function

        ''' <summary>
        ''' 得到当前的物质内容所能够支撑的最小反应单位
        ''' </summary>
        ''' <param name="factors"></param>
        ''' <param name="regulation"></param>
        ''' <returns></returns>
        Private Shared Function minimalUnit(parallel As Dictionary(Of String, Double), factors As IEnumerable(Of Variable), regulation#, max#) As Double
            Return factors _
                .Select(Function(v)
                            Dim reactionUnit = minimalUnit(parallel, regulation, v)
                            Dim unit As Double = Math.Min(reactionUnit, max)

                            Return unit
                        End Function) _
                .Min
        End Function

        Private Shared Function minimalUnit(parallel As Dictionary(Of String, Double), regulation#, v As Variable) As Double
            Dim r = regulation * v.coefficient
            Dim shares# = parallel(v.mass.ID)
            Dim massUnit = v.mass.Value / shares
            Dim reactionUnit As Double

            If r > massUnit Then
                ' 消耗的已经超过了当前的容量
                ' 则最小的反应单位是当前的物质容量

                ' 如果某一个物质的容量是零，则表示没有反应物可以被利用了
                ' 则计算出来的最小反应单位是零
                ' 即此反应过程不可能会发生
                reactionUnit = massUnit / v.coefficient
            Else ' 能够正常的以当前的反应单位进行
                reactionUnit = regulation
            End If

            Return reactionUnit
        End Function

        Public Overrides Function ToString() As String
            If direct = Directions.stop Then
                Return "stopped..."
            Else
                Return Core.ToString(Me)
            End If
        End Function

    End Class
End Namespace

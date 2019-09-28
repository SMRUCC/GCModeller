#Region "Microsoft.VisualBasic::05200e88db0f01904c015fec0ecc1045, engine\Dynamics\Core\Channel.vb"

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

    '     Class Channel
    ' 
    '         Properties: bounds, Direction, Forward, ID, Reverse
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '         Function: CoverLeft, CoverRight, minimalUnit
    ' 
    '         Sub: Transition
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model

Namespace Core

    ''' <summary>
    ''' 反应过程通道
    ''' </summary>
    Public Class Channel : Implements INamedValue

        Friend left As Variable()
        Friend right As Variable()

        Public Property Forward As Controls
        Public Property Reverse As Controls

        ''' <summary>
        ''' 因为细胞微环境内的容量很小，没有办法使整个反应过程以很大的速率来进行
        ''' 在这里设置反应的正反过程这两个方向上的上下限？
        ''' </summary>
        ''' <returns></returns>
        Public Property bounds As DoubleRange

        ''' <summary>
        ''' 在衡量了<see cref="Forward"/>和<see cref="Reverse"/>的效应大小之后，当前的反应的方向
        ''' </summary>
        ''' <returns></returns>
        Public Overloads ReadOnly Property direct As Directions
            Get
                If Forward > Reverse Then
                    Return Directions.forward
                ElseIf Reverse > Forward Then
                    Return Directions.reverse
                Else
                    Return Directions.stop
                End If
            End Get
        End Property

        Public Property ID As String Implements IKeyedEntity(Of String).Key

        Sub New(left As IEnumerable(Of Variable), right As IEnumerable(Of Variable))
            Me.left = left.ToArray
            Me.right = right.ToArray
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="shares">事件并行化模拟因子</param>
        ''' <param name="regulation"></param>
        ''' <returns></returns>
        Public Function CoverLeft(shares As Dictionary(Of String, Double), regulation#, resolution#) As Double
            Return minimalUnit(shares, left, regulation, bounds.Max) / resolution#
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="shares">事件并行化模拟因子</param>
        ''' <param name="regulation"></param>
        ''' <returns></returns>
        Public Function CoverRight(shares As Dictionary(Of String, Double), regulation#, resolution#) As Double
            Return minimalUnit(shares, right, regulation, bounds.Min) / resolution#
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="reactUnit"></param>
        ''' <param name="dir"></param>
        ''' <remarks>
        ''' 模板物质的容量是不会发生变化的
        ''' </remarks>
        Public Sub Transition(reactUnit As Double, dir As Directions)
            reactUnit = reactUnit * dir

            ' 一般左边为模板
            For Each mass In left.Where(Function(m) Not m.IsTemplate)
                mass.Mass.Value -= reactUnit * mass.Coefficient
            Next
            For Each mass In right
                mass.Mass.Value += reactUnit * mass.Coefficient
            Next
        End Sub

        ''' <summary>
        ''' 得到当前的物质内容所能够支撑的最小反应单位
        ''' </summary>
        ''' <param name="factors"></param>
        ''' <param name="regulation"></param>
        ''' <returns></returns>
        Private Shared Function minimalUnit(parallel As Dictionary(Of String, Double), factors As IEnumerable(Of Variable), regulation#, max#) As Double
            Return factors _
                .Select(Function(v)
                            Dim r = regulation * v.Coefficient
                            Dim shares# = parallel(v.Mass.ID)
                            Dim massUnit = v.Mass.Value / shares
                            Dim reactionUnit As Double

                            If r > massUnit Then
                                ' 消耗的已经超过了当前的容量
                                ' 则最小的反应单位是当前的物质容量

                                ' 如果某一个物质的容量是零，则表示没有反应物可以被利用了
                                ' 则计算出来的最小反应单位是零
                                ' 即此反应过程不可能会发生
                                reactionUnit = massUnit / v.Coefficient
                            Else ' 能够正常的以当前的反应单位进行
                                reactionUnit = regulation
                            End If

                            Return Math.Min(reactionUnit, max)
                        End Function) _
                .Min
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

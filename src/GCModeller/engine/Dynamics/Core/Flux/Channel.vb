#Region "Microsoft.VisualBasic::da6d1d6e4776870a9401ee5d7f9657ab, engine\Dynamics\Core\Flux\Channel.vb"

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

'   Total Lines: 136
'    Code Lines: 74 (54.41%)
' Comment Lines: 42 (30.88%)
'    - Xml Docs: 80.95%
' 
'   Blank Lines: 20 (14.71%)
'     File Size: 5.29 KB


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

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository

Namespace Core

    ''' <summary>
    ''' Biological flux object model, the reaction channel.
    ''' 
    ''' (反应过程通道)
    ''' </summary>
    ''' <remarks>
    ''' make flux dynamics association between a set of metabolite mass <see cref="Variable"/>.
    ''' the flux dynamics could be affects via the environment <see cref="Controls"/>. dynamics
    ''' of the reaction flux was contraint via the <see cref="bounds"/> range.
    ''' </remarks>
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
                Return GetCurrentDirection()
            End Get
        End Property

        Public ReadOnly Property isBroken As Boolean
        Public ReadOnly Property Message As String

        Public Property ID As String Implements IKeyedEntity(Of String).Key
        ''' <summary>
        ''' the debug name
        ''' </summary>
        ''' <returns></returns>
        Public Property name As String

        Sub New(left As IEnumerable(Of Variable), right As IEnumerable(Of Variable))
            Me.left = left.ToArray
            Me.right = right.ToArray

            If Me.left.IsNullOrEmpty Then
                Message = "{0} - left side is empty!"
            ElseIf Me.right.IsNullOrEmpty Then
                Message = "{0} - right side is empty!"
            End If

            ' 20250830 check of the zero factor
            Dim zero = Me.left.Where(Function(f) f.coefficient = 0.0).ToArray

            If zero.Length > 0 Then
                Message = $"{0} - left side has zero coefficient factor: {zero.Select(Function(v) v.mass.ID).JoinBy(", ")}"
            End If

            zero = Me.right.Where(Function(f) f.coefficient = 0.0).ToArray

            If zero.Length > 0 Then
                Message = $"{0} - right side has zero coefficient factor: {zero.Select(Function(v) v.mass.ID).JoinBy(", ")}"
            End If

            isBroken = Not Message.StringEmpty()
        End Sub

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Sub New(left As Variable, right As Variable)
            Call Me.New({left}, {right})
        End Sub

        Public Function GetCurrentDirection() As Directions
            If forward > reverse Then
                Return Directions.forward
            ElseIf reverse > forward Then
                Return Directions.reverse
            Else
                Return Directions.stop
            End If
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="shares">事件并行化模拟因子</param>
        ''' <param name="regulation"></param>
        ''' <returns></returns>
        Public Function CoverLeft(shares As Dictionary(Of String, Double), regulation#) As Double
            If isBroken Then
                Return 0
            ElseIf left.Any(Function(v) v.mass.Value = 0.0) Then
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
            ElseIf right.Any(Function(v) v.mass.Value <= 0.0) Then
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
        Private Shared Function minimalUnit(parallel As Dictionary(Of String, Double),
                                            factors As IEnumerable(Of Variable),
                                            regulation#,
                                            max#) As Double
            Return factors _
                .Select(Function(v)
                            Dim reactionUnit = minimalUnit(parallel, regulation, v)
                            Dim unit As Double = Math.Min(reactionUnit, max)

                            Return unit
                        End Function) _
                .Min
        End Function

        Private Shared Function minimalUnit(parallel As Dictionary(Of String, Double), regulation#, v As Variable) As Double
            Dim r# = regulation * v.coefficient
            Dim shares# = parallel(v.mass.ID) + 1
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
                Return $"[{ID}] stopped... ({name})"
            Else
                Return Core.ToString(Me)
            End If
        End Function

    End Class
End Namespace

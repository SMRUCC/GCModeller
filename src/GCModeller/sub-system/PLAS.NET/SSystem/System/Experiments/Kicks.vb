#Region "Microsoft.VisualBasic::34636e58ee63007af1a4dfc6af66a278, PLAS.NET\SSystem\System\Experiments\Kicks.vb"

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

    '     Class Kicks
    ' 
    '         Constructor: (+2 Overloads) Sub New
    ' 
    '         Function: __getExpireds, __getPendings
    ' 
    '         Sub: loadKernel, Tick
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.Calculus.Dynamics
Imports SMRUCC.genomics.Analysis.SSystem.Kernel.ObjectModels

Namespace Kernel

    Public Class Kicks

        ''' <summary>
        ''' Standing by
        ''' </summary>
        ''' <remarks></remarks>
        ReadOnly __pendingKicks As List(Of Disturb)
        ''' <summary>
        ''' Active object.
        ''' </summary>
        ''' <remarks></remarks>
        ReadOnly __runningKicks As New List(Of Disturb)
        ' ReadOnly __kernel As Kernel

        Sub New(kernel As Kernel, model As Script.Model)
            ' __kernel = kernel
            __pendingKicks = model.Experiments _
                .SafeQuery _
                .ToList(Function(x)
                            Return New Disturb(x, kernel.GetValue(x.Id), Function() kernel.RuntimeTicks)
                        End Function)

            ' For i As Integer = 0 To __pendingKicks.Count - 1
            '    __pendingKicks(i).Set(kernel)
            ' Next
        End Sub

        Sub New(vars As Dictionary(Of Ivar), model As Script.Model, getRunTicks As Func(Of Long))
            __pendingKicks = model.Experiments.ToList(
                Function(x) New Disturb(
                    x,
                    vars(x.Id),
                    getRunTicks))
        End Sub

        Public Sub loadKernel(kernel As Kernel)
            kernel.kicks = Me
        End Sub

        Public Sub Tick()
            For Each x As Disturb In __getPendings()
                Call __pendingKicks.Remove(x)
                Call __runningKicks.Add(x)
            Next

            For i As Integer = 0 To __runningKicks.Count - 1
                __runningKicks(i).Tick()
            Next

            For Each x In __getExpireds()
                Call __runningKicks.Remove(x)
            Next
        End Sub

        ''' <summary>
        ''' 从当前正在运行的实验之中查找出过期的实验对象
        ''' </summary>
        ''' <returns></returns>
        Private Function __getExpireds() As Disturb()
            Return LinqAPI.Exec(Of Disturb) <=
 _
                From x As Disturb
                In __runningKicks
                Where x.LeftKicks = 0
                Select x ' Removed pending

        End Function

        ''' <summary>
        ''' 获取正在处于等待队列的实验
        ''' </summary>
        ''' <returns></returns>
        Private Function __getPendings() As Disturb()
            Return LinqAPI.Exec(Of Disturb) <=
 _
                From x As Disturb
                In __pendingKicks
                Where x.IsReady  ' Quene pending
                Select x

        End Function
    End Class
End Namespace

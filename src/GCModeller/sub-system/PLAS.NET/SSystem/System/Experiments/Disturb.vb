#Region "Microsoft.VisualBasic::642fd6f7a527e5b56d806022c84ea022, ..\GCModeller\sub-system\PLAS.NET\SSystem\System\Experiments\Disturb.vb"

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

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Mathematical.diffEq

Namespace Kernel.ObjectModels

    Public Class Disturb

        Dim Target As Ivar
        ' Dim Kernel As Kernel
        Dim model As Experiment
        Dim nextTime As Double
        Dim getRunTicks As Func(Of Long)

        ''' <summary>
        ''' 周期性的实验之中所剩余的刺激
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property LeftKicks As Integer
        Public ReadOnly Property IsReady As Boolean
            Get
                Return model.Start <= getRunTicks()
            End Get
        End Property

        ReadOnly __disturb As Func(Of Double, Double, Double)

        Sub New(model As Experiment, target As Ivar, getRunTicks As Func(Of Long))
            Me.model = model
            Me.Target = target
            Me.getRunTicks = getRunTicks

            LeftKicks = model.Kicks
            __disturb = Methods(model.DisturbType)
        End Sub

        Public Sub Tick()
            If getRunTicks() > nextTime Then
                Target.value = __disturb(Target.value, model.Value)
                nextTime = model.Interval + getRunTicks()
                _LeftKicks -= 1
            End If
        End Sub

        Public Overrides Function ToString() As String
            Return model.ToString
        End Function

        ' Public Sub [Set](Kernel As Kernel)
        ' Me.Kernel = Kernel
        ' Me.Target = Kernel.GetValue(model.Id)
        ' End Sub
    End Class
End Namespace

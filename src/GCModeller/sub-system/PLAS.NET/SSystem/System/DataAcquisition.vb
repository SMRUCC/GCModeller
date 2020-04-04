#Region "Microsoft.VisualBasic::eb63d5519ae24f2f4847e3580b0566c1, sub-system\PLAS.NET\SSystem\System\DataAcquisition.vb"

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

'     Class DataAcquisition
' 
'         Constructor: (+1 Overloads) Sub New
' 
'         Function: __tag
' 
'         Sub: Save, Tick
' 
' 
' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Language
Imports SMRUCC.genomics.Analysis.SSystem.Kernel.ObjectModels

Namespace Kernel

    ''' <summary>
    ''' Data service.(数据采集服务)
    ''' </summary>
    Public Class DataAcquisition

        Friend data As New List(Of DataSet)

        Dim kernel As Kernel
        ''' <summary>
        ''' 这个主要是调用外部接口的回调函数，这个回调函数会在一个内核循环之中采集完数据之后被触发调用
        ''' </summary>
        Dim __tickCallback As Action(Of DataSet)

        Sub New(k As Kernel, Optional tick As Action(Of DataSet) = Nothing)
            kernel = k
            __tickCallback = tick
        End Sub

        Public Sub Tick()
            Dim t As New DataSet With {
                .ID = kernel.RuntimeTicks * kernel.Precision,
                .Properties = kernel.Vars _
                    .ToDictionary(AddressOf __tag, Function(x) x.Value)
            }

            data += t

            ' 在一次内核循环之中才几万计算数据之后调用回调函数来触发外部事件
            If Not __tickCallback Is Nothing Then
                Call __tickCallback(t)
            End If
        End Sub

        Private Shared Function __tag(x As var) As String
            If Not String.IsNullOrEmpty(x.Title) Then
                Return $"{x.Id}({x.Title})"
            Else
                Return x.Id
            End If
        End Function

        Public Sub Save(Path As String)
            Call data.SaveTo(Path, False)
        End Sub
    End Class
End Namespace

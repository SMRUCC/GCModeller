#Region "Microsoft.VisualBasic::9a631503183ac7c0c8c461ccbc5ad859, ..\GCModeller\sub-system\PLAS.NET\SSystem\System\Experiments\Kicks.vb"

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
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.Language
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
        ReadOnly __runningKicks As List(Of Disturb)
        ReadOnly __kernel As Kernel

        Sub New(kernel As Kernel)
            __kernel = kernel
            __pendingKicks = kernel.get_Model.Experiments.ToList(Function(x) New Disturb(x))
            __runningKicks = New List(Of Disturb)

            For i As Integer = 0 To __pendingKicks.Count - 1
                __pendingKicks(i).Set(kernel)
            Next
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

        Private Function __getExpireds() As Disturb()
            Return LinqAPI.Exec(Of Disturb) <=
 _
                From x As Disturb
                In __runningKicks
                Where x.LeftKicks = 0
                Select x ' Removed pending

        End Function

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


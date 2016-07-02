#Region "Microsoft.VisualBasic::55e61bf9b86d0da16ab9a674ebfebc31, ..\GCModeller\sub-system\PLAS.NET\SSystem\System\DataAcquisition.vb"

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

Imports Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream
Imports SMRUCC.genomics.Analysis.SSystem.Kernel.ObjectModels

Namespace Kernel

    Public Class DataAcquisition
        Dim _dataPackage As New File
        Dim Kernel As Kernel

        Public Sub Tick()
            Dim Row As New RowObject

            Call Row.Add(Kernel.RuntimeTicks)
            For Each Var As var In Kernel.Vars
                Row.Add(Var.Value)
            Next

            _dataPackage.AppendLine(Row)
        End Sub

        Public Sub Save(Path As String)
            Call _dataPackage.Save(Path, False)
        End Sub

        Public Sub [Set](Kernel As Kernel)
            Dim Row As New RowObject

            Me.Kernel = Kernel
            Call Row.Add("Elapsed Time")
            For Each Var As var In Kernel.Vars
                If Not String.IsNullOrEmpty(Var.Title) Then
                    Call Row.Add(String.Format("""{0}""", Var.Title))
                Else
                    Call Row.Add(Var.UniqueId)
                End If
            Next

            Call _dataPackage.AppendLine(Row)
        End Sub

        Public Shared Function [Get](e As DataAcquisition) As File
            Return e._dataPackage
        End Function
    End Class
End Namespace


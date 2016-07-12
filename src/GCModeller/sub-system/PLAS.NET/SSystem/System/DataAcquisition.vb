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

Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.DocumentFormat.Csv
Imports Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream
Imports SMRUCC.genomics.Analysis.SSystem.Kernel.ObjectModels

Namespace Kernel

    ''' <summary>
    ''' Data service
    ''' </summary>
    Public Class DataAcquisition

        Friend data As New List(Of DataSet)

        Dim kernel As Kernel

        Sub New(k As Kernel)
            kernel = k
        End Sub

        Public Sub Tick()
            data += New DataSet With {
                .Identifier = kernel.RuntimeTicks * Kernel.precision,
                .Properties = kernel.Vars _
                    .ToDictionary(AddressOf __tag, Function(x) x.Value)
            }
        End Sub

        Private Shared Function __tag(x As var) As String
            If Not String.IsNullOrEmpty(x.Title) Then
                Return x.Title
            Else
                Return x.UniqueId
            End If
        End Function

        Public Sub Save(Path As String)
            Call data.SaveTo(Path, False)
        End Sub
    End Class
End Namespace


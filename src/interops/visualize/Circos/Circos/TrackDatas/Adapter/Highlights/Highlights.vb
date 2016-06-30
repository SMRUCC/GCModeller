#Region "Microsoft.VisualBasic::12ae399ce05490a009f9385d2b496ee9, ..\Circos\Circos\TrackDatas\Adapter\Highlights\Highlights.vb"

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

Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Drawing
Imports LANS.SystemsBiology.ComponentModel

Namespace TrackDatas.Highlights

    Public MustInherit Class Highlights : Inherits data(Of ValueTrackData)

        Sub New(source As IEnumerable(Of ValueTrackData))
            Call MyBase.New(source)
        End Sub

        Protected Sub New()
            Call MyBase.New(Nothing)
        End Sub

        Const COG_NULL_EXCEPTION As String = "This error usually caused by the null COG data in the gene annotations. " &
            "Please check of the COG data in your genome annotation data make sure not all of the gene have no COG value" &
            "(at least should parts of the genes in the genome have COG assigned value)."

        Protected Sub __throwSourceNullEx(Of T)(source As IEnumerable(Of T))
            If source.IsNullOrEmpty Then
                Dim exMsg As String =
                    $"{Me.GetType.FullName}, data Is null!" &
                    vbCrLf &
                    vbCrLf &
                    COG_NULL_EXCEPTION
                Throw New DataException(exMsg)
            End If
        End Sub
    End Class
End Namespace

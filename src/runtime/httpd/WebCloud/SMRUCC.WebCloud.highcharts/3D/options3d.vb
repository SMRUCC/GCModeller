#Region "Microsoft.VisualBasic::3e63da3f260fdc1c2b047c05026c42fd, ..\httpd\WebCloud\SMRUCC.WebCloud.highcharts\3D\options3d.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
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

Public Class options3d
    Public Property enabled As Boolean?
    Public Property alpha As Double?
    Public Property beta As Double?
    Public Property depth As Double?
    Public Property viewDistance As Double?
    Public Property fitToPlot As Boolean?
    Public Property frame As frame3DOptions

    Public Overrides Function ToString() As String
        If enabled Then
            Return NameOf(enabled)
        Else
            Return $"Not {NameOf(enabled)}"
        End If
    End Function
End Class

Public Class frame3DOptions
    Public Property bottom As frameOptions
    Public Property back As frameOptions
    Public Property side As frameOptions
End Class

Public Class frameOptions
    Public Property size As Double?
    Public Property color As String
End Class

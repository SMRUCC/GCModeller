#Region "Microsoft.VisualBasic::fa3a7693fcdd7a6ba99315b1c871fcc8, WebCloud\SMRUCC.WebCloud.GIS\MaxMind\geographical_information_view.vb"

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

    '     Class geographical_information_view
    ' 
    '         Properties: city_name, country_iso_code, country_name, geoname_id, latitude
    '                     longitude, subdivision_1_name, subdivision_2_name
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace MaxMind.Views

    Public Class geographical_information_view

        Public Property geoname_id As Long
        Public Property latitude As Double
        Public Property longitude As Double
        Public Property country_iso_code As String
        Public Property country_name As String
        Public Property city_name As String
        Public Property subdivision_1_name As String
        Public Property subdivision_2_name As String

    End Class
End Namespace

﻿#Region "Microsoft.VisualBasic::1daa6769f7cb55d5fab982ad65a39a7f, WebCloud\SMRUCC.WebCloud.GIS\MaxMind\QueryEngine\QueryOut.vb"

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

    '     Class FindResult
    ' 
    '         Properties: city_name, continent_code, continent_name, country_iso_code, country_name
    '                     geoname_id, metro_code, subdivision_1_iso_code, subdivision_1_name, subdivision_2_iso_code
    '                     subdivision_2_name, time_zone
    ' 
    '         Function: Null, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports SMRUCC.WebCloud.GIS.MaxMind.Views

Namespace MaxMind

    Public Class FindResult : Inherits GeographicLocation

        <XmlAttribute> Public Property continent_code As String
        <XmlAttribute> Public Property continent_name As String
        <XmlAttribute> Public Property country_iso_code As String
        <XmlAttribute> Public Property country_name As String
        <XmlAttribute> Public Property geoname_id As Long
        <XmlAttribute> Public Property subdivision_1_iso_code As String
        <XmlAttribute> Public Property subdivision_1_name As String
        <XmlAttribute> Public Property subdivision_2_iso_code As String
        <XmlAttribute> Public Property subdivision_2_name As String
        <XmlAttribute> Public Property city_name As String
        <XmlAttribute> Public Property metro_code As String
        <XmlAttribute> Public Property time_zone As String

        Public Overrides Function ToString() As String
            Return String.Format("{0}, {1}/{2}", city_name, subdivision_1_name, subdivision_2_name) & ";      " & String.Format("({0}){1}", country_iso_code, country_name) & "      [" & MyBase.ToString() & "]"
        End Function

        Public Shared Function Null() As FindResult
            Return New FindResult With {
                .city_name = "null",
                .country_name = "null",
                .subdivision_1_name = "null",
                .subdivision_2_name = "null"
            }
        End Function
    End Class
End Namespace

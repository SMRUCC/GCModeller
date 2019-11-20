#Region "Microsoft.VisualBasic::59a5e12b4bc01549dcca8d2581980d7d, data\ExternalDBSource\RegulonDB\PromoterFeature.vb"

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

    '     Class PromoterFeature
    ' 
    '         Properties: BOX_10_LEFT, BOX_10_RIGHT, BOX_10_SEQUENCE, BOX_35_LEFT, BOX_35_RIGHT
    '                     BOX_35_SEQUENCE, PROMOTER_FEATURE_ID, PROMOTER_ID, RELATIVE_BOX_10_LEFT, RELATIVE_BOX_10_RIGHT
    '                     RELATIVE_BOX_35_LEFT, RELATIVE_BOX_35_RIGHT, SCORE
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace RegulonDB

    Public Class PromoterFeature : Inherits LANS.SystemsBiology.ExternalDBSource.RegulonDB.ObjectItem
        <Xml.Serialization.XmlElement("PROMOTER_FEATURE_ID")> Public Property PROMOTER_FEATURE_ID As String
        <Xml.Serialization.XmlElement("PROMOTER_ID")> Public Property PROMOTER_ID As String
        <Xml.Serialization.XmlElement("BOX_10_LEFT")> Public Property BOX_10_LEFT As String
        <Xml.Serialization.XmlElement("BOX_10_RIGHT")> Public Property BOX_10_RIGHT As String
        <Xml.Serialization.XmlElement("BOX_35_LEFT")> Public Property BOX_35_LEFT As String
        <Xml.Serialization.XmlElement("BOX_35_RIGHT")> Public Property BOX_35_RIGHT As String
        <Xml.Serialization.XmlElement("BOX_10_SEQUENCE")> Public Property BOX_10_SEQUENCE As String
        <Xml.Serialization.XmlElement("BOX_35_SEQUENCE")> Public Property BOX_35_SEQUENCE As String
        <Xml.Serialization.XmlElement("SCORE")> Public Property SCORE As String
        <Xml.Serialization.XmlElement("RELATIVE_BOX_10_LEFT")> Public Property RELATIVE_BOX_10_LEFT As String
        <Xml.Serialization.XmlElement("RELATIVE_BOX_10_RIGHT")> Public Property RELATIVE_BOX_10_RIGHT As String
        <Xml.Serialization.XmlElement("RELATIVE_BOX_35_LEFT")> Public Property RELATIVE_BOX_35_LEFT As String
        <Xml.Serialization.XmlElement("RELATIVE_BOX_35_RIGHT")> Public Property RELATIVE_BOX_35_RIGHT As String
    End Class
End Namespace

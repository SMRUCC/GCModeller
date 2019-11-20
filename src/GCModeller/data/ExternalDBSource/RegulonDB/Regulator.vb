#Region "Microsoft.VisualBasic::2be8216f2ebf5db66ab186f4d8795d95, data\ExternalDBSource\RegulonDB\Regulator.vb"

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

    '     Class ObjectItem
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '     Class Regulator
    ' 
    '         Properties: KEY_ID_ORG, PRODUCT_ID, REGULATOR_ID, REGULATOR_INTERNAL_COMMNET, REGULATOR_NAME
    '                     REGULATOR_NOTE
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace RegulonDB

    <Serializable> Public MustInherit Class ObjectItem
        Protected Friend Sub New()
        End Sub
    End Class

    Public Class Regulator : Inherits LANS.SystemsBiology.ExternalDBSource.RegulonDB.ObjectItem
        <Xml.Serialization.XmlElement("REGULATOR_ID")> Public Property REGULATOR_ID As String
        <Xml.Serialization.XmlElement("PRODUCT_ID")> Public Property PRODUCT_ID As String
        <Xml.Serialization.XmlElement("REGULATOR_NAME")> Public Property REGULATOR_NAME As String
        <Xml.Serialization.XmlElement("REGULATOR_INTERNAL_COMMNET")> Public Property REGULATOR_INTERNAL_COMMNET As String
        <Xml.Serialization.XmlElement("REGULATOR_NOTE")> Public Property REGULATOR_NOTE As String
        <Xml.Serialization.XmlElement("KEY_ID_ORG")> Public Property KEY_ID_ORG As String
    End Class
End Namespace

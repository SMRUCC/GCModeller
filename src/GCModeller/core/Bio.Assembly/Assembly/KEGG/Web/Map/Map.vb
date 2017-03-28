#Region "Microsoft.VisualBasic::3e3b400449f08b97aa2769f02e1d4026, ..\core\Bio.Assembly\Assembly\KEGG\Web\Map\Map.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
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
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace Assembly.KEGG.WebServices

    Public Class Map

        <XmlElement> Public Property Areas As Area()

        Public Overrides Function ToString() As String
            Return Areas.GetJson
        End Function
    End Class

    Public Class Area

        <XmlAttribute> Public Property shape As String
        ''' <summary>
        ''' 位置坐标信息
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property coords As String
        <XmlAttribute> Public Property href As String
        <XmlAttribute> Public Property title As String

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Class
End Namespace

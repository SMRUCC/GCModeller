#Region "Microsoft.VisualBasic::32027627d069ded023ce3d6f1e8758ac, ..\GCModeller\data\ExternalDBSource\string-db\[xsd]net.sf.psidev.mi\XmlCommon.vb"

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
Imports Microsoft.VisualBasic.DocumentFormat.Csv.StorageProvider.Reflection

Namespace StringDB.MIF25.XmlCommon

    Public MustInherit Class DataItem
        <XmlAttribute("id")> Public Overridable Property Id As Integer

        Public Overrides Function ToString() As String
            Return Id
        End Function
    End Class

    Public Class Names
        <XmlElement("shortLabel")> Public Property ShortLabel As String
        <XmlElement("fullName")> Public Property FullName As String

        Public Overrides Function ToString() As String
            Return String.Format("({0}) {1}", ShortLabel, FullName)
        End Function
    End Class

    Public Class Xref
        <XmlElement("primaryRef")> Public Property PrimaryReference As Reference
        <XmlElement("secondaryRef")> Public Property SecondaryReference As Reference()

        Public Overrides Function ToString() As String
            Return PrimaryReference.ToString
        End Function
    End Class

    Public Class Reference
        <XmlAttribute("db")> Public Property Db As String
        <XmlAttribute("id")> Public Property Id As String
        <XmlAttribute("dbAc")> Public Property dbAc As String
        <XmlAttribute("refType")> Public Property refType As String
        <XmlAttribute("refTypeAc")> Public Property refTypeAc As String

        Public Overrides Function ToString() As String
            Return String.Format("{0}: {1}", Db, Id)
        End Function
    End Class
End Namespace

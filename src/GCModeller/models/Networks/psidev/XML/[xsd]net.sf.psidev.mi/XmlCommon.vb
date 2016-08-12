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

Namespace XML

    Public MustInherit Class DataItem

        <XmlAttribute> Public Overridable Property id As Integer

        Public Overrides Function ToString() As String
            Return id
        End Function
    End Class

    Public Class Names
        Public Property shortLabel As String
        Public Property fullName As String

        Public Overrides Function ToString() As String
            Return String.Format("({0}) {1}", shortLabel, fullName)
        End Function
    End Class

    Public Class Xref

        <XmlElement> Public Property primaryRef As Reference
        <XmlElement> Public Property secondaryRef As Reference()

        Public Overrides Function ToString() As String
            Return primaryRef.ToString
        End Function
    End Class

    Public Class Reference : Inherits DataItem

        <XmlAttribute> Public Property db As String
        <XmlAttribute> Public Property dbAc As String
        <XmlAttribute> Public Property refType As String
        <XmlAttribute> Public Property refTypeAc As String

        Public Overrides Function ToString() As String
            Return String.Format("{0}: {1}", db, id)
        End Function
    End Class
End Namespace

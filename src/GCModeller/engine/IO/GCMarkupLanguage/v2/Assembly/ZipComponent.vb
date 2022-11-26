#Region "Microsoft.VisualBasic::2d302c6a3d6564c7ff00f3ddf8fe613f, GCModeller\engine\IO\GCMarkupLanguage\v2\Assembly\ZipComponent.vb"

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


    ' Code Statistics:

    '   Total Lines: 58
    '    Code Lines: 46
    ' Comment Lines: 1
    '   Blank Lines: 11
    '     File Size: 1.75 KB


    '     Class ZipComponent
    ' 
    '         Properties: components, TypeComment
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: getCollection, getSize, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.Serialization

Imports System.Xml
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel
#If netcore5 = 1 Then
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
#Else
Imports System.Web.Script.Serialization
#End If

Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports SMRUCC.genomics.GCModeller.CompilerServices

Namespace v2

    Public Class ZipComponent(Of T) : Inherits ListOf(Of T)
        Implements XmlDataModel.IXmlType

        <DataMember>
        <IgnoreDataMember>
        <ScriptIgnore>
        <SoapIgnore>
        <XmlAnyElement>
        Public Property TypeComment As XmlComment Implements XmlDataModel.IXmlType.TypeComment
            Get
                Return XmlDataModel.CreateTypeReferenceComment(GetType(T))
            End Get
            Set(value As XmlComment)
                ' do nothing
            End Set
        End Property

        <XmlElement>
        Public Property components As T()

        <XmlNamespaceDeclarations()>
        Public xmlns As New XmlSerializerNamespaces

        Sub New()
            Call xmlns.Add("vcellkit", ModelBaseType.GCModellerVCellKit)
            Call xmlns.Add("biocad", VirtualCell.GCMarkupLanguage)
        End Sub

        Public Overrides Function ToString() As String
            Return $"{components.TryCount} components"
        End Function

        Protected Overrides Function getSize() As Integer
            Return components.TryCount
        End Function

        Protected Overrides Function getCollection() As IEnumerable(Of T)
            Return components.AsEnumerable
        End Function
    End Class
End Namespace

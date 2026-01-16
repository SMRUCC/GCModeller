#Region "Microsoft.VisualBasic::5959b7718ea7206da59d6e3a7a19e845, visualize\Cytoscape\Cytoscape\Graph\Xgmml\File\Attributes.vb"

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

    '   Total Lines: 126
    '    Code Lines: 95 (75.40%)
    ' Comment Lines: 13 (10.32%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 18 (14.29%)
    '     File Size: 4.68 KB


    '     Class Attribute
    ' 
    '         Properties: cyType, Directed, elementType, Hidden, name
    '                     Type, TypeMapping, Value
    ' 
    '         Function: StringValue, ToString
    ' 
    '     Class AttributeDictionary
    ' 
    '         Properties: attributes
    ' 
    '         Function: AddAttribute, SetAttribute, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Language

Namespace CytoscapeGraphView.XGMML.File

    ''' <summary>
    ''' 一个网络之中的对象所具备有的属性值
    ''' </summary>
    ''' <remarks></remarks>
    <XmlType("att")>
    Public Class Attribute : Implements INamedValue

        <XmlAttribute("name")> Public Property name As String Implements INamedValue.Key
        <XmlAttribute("value")> Public Property Value As String
        <XmlAttribute("type")> Public Property Type As String

        <XmlAttribute("hidden", [Namespace]:=XGMMLgraph.xmlnsCytoscape)> Public Property Hidden As String
        <XmlAttribute("directed", [Namespace]:=XGMMLgraph.xmlnsCytoscape)> Public Property Directed As String
        <XmlAttribute("type", [Namespace]:=XGMMLgraph.xmlnsCytoscape)> Public Property cyType As String
        <XmlAttribute("elementType", [Namespace]:=XGMMLgraph.xmlnsCytoscape)> Public Property elementType As String

        ''' <summary>
        ''' Maps the .NET basic data type to the cytoscape data type name.
        ''' </summary>
        ''' <returns></returns>
        Public Shared ReadOnly Property TypeMapping As IReadOnlyDictionary(Of Type, String) =
            New Dictionary(Of Type, String) From {
 _
            {GetType(String), ATTR_VALUE_TYPE_STRING},
            {GetType(Boolean), ATTR_VALUE_TYPE_BOOLEAN},
            {GetType(Integer), ATTR_VALUE_TYPE_INTEGER},
            {GetType(Double), ATTR_VALUE_TYPE_REAL}
        }

        Public Overrides Function ToString() As String
            Return $"({Type}) {Name} = {Value}"
        End Function

        Public Shared Function StringValue(name As String, value As String) As Attribute
            Return New Attribute With {
                .Name = name,
                .Value = value,
                .cyType = NameOf(System.String),
                .Type = ATTR_VALUE_TYPE_STRING
            }
        End Function

        Public Shared Narrowing Operator CType(attr As Attribute) As String
            If attr Is Nothing Then
                Return Nothing
            Else
                Return attr.Value
            End If
        End Operator
    End Class

    Public MustInherit Class AttributeDictionary

        Dim attrs As Dictionary(Of Attribute)

        <XmlElement("att")> Public Property attributes As Attribute()
            Get
                If attrs.IsNullOrEmpty Then
                    Return New Attribute() {}
                End If
                Return attrs.Values.ToArray
            End Get
            Set(value As Attribute())
                If value.IsNullOrEmpty Then
                    attrs = New Dictionary(Of Attribute)
                Else
                    attrs = value.ToDictionary
                End If
            End Set
        End Property

        ''' <summary>
        ''' 属性值不存在则返回空值
        ''' </summary>
        ''' <param name="Name"></param>
        ''' <returns></returns>
        Default Public ReadOnly Property Value(Name As String) As Attribute
            Get
                If attrs.ContainsKey(Name) Then
                    Return attrs(Name)
                Else
                    Return Nothing
                End If
            End Get
        End Property

        Public Function AddAttribute(Name As String, value As String, Type As String) As Boolean
            Dim attr As Attribute

            If attrs.ContainsKey(Name) Then
                attr = attrs(Name)
            Else
                attr = New Attribute With {.name = Name}
                Call attrs.Add(Name, attr)
            End If

            attr.Value = value
            attr.Type = Type

            Return True
        End Function

        Public Function SetAttribute(Name As String, Value As String) As Boolean
            If attrs.ContainsKey(Name) Then
                attrs(Name).Value = Value
            Else
                Dim attr As New Attribute With {
                    .Value = Value,
                    .name = Name,
                    .Type = ATTR_VALUE_TYPE_STRING
                }
                Call attrs.Add(Name, attr)
            End If

            Return True
        End Function

        Public Overrides Function ToString() As String
            Dim array As String() =
                LinqAPI.Exec(Of String) <= From attr As Attribute
                                           In attrs.Values
                                           Let strValue As String = attr.ToString
                                           Select strValue
            Return String.Join("; ", array)
        End Function
    End Class
End Namespace

#Region "Microsoft.VisualBasic::46eb5cee02005dedd6b5318273722cae, core\Bio.Assembly\ComponentModel\DBLinkBuilder\Xref.vb"

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

    '     Class XrefAttribute
    ' 
    '         Properties: Name
    ' 
    '         Function: CreateDictionary, GetProperties, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository
Imports Microsoft.VisualBasic.Scripting.Runtime

Namespace ComponentModel.DBLinkBuilder

    <AttributeUsage(AttributeTargets.Property)>
    Public Class XrefAttribute : Inherits Attribute
        Implements INamedValue

        Public Property Name As String Implements IKeyedEntity(Of String).Key

        Public Overrides Function ToString() As String
            Return "Xref Link"
        End Function

        Public Shared Function GetProperties(type As Type) As PropertyInfo()
            Return type _
                .Schema(PropertyAccess.ReadWrite, PublicProperty, True) _
                .Values _
                .Where(Function([property])
                           Return Not [property].GetCustomAttribute(Of XrefAttribute) Is Nothing
                       End Function) _
                .ToArray
        End Function

        Public Shared Function CreateDictionary(Of T)() As Func(Of T, Dictionary(Of String, String))
            Dim properties As PropertyInfo() = XrefAttribute.GetProperties(GetType(T))
            Dim reader As Dictionary(Of String, PropertyInfo) = properties _
                .ToDictionary(Function(name)
                                  Dim xref As XrefAttribute = name.GetCustomAttribute(Of XrefAttribute)

                                  If xref.Name.StringEmpty Then
                                      Return name.Name
                                  Else
                                      Return xref.Name
                                  End If
                              End Function)

            Return Function(x)
                       Return reader _
                           .ToDictionary(Function(key) key.Key,
                                         Function(read)
                                             Return CStrSafe(read.Value.GetValue(x))
                                         End Function)
                   End Function
        End Function
    End Class
End Namespace

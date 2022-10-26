#Region "Microsoft.VisualBasic::d8c88f2814f68448a420af814bbfb1f9, sciBASIC#\Microsoft.VisualBasic.Core\src\ComponentModel\DataSource\SchemaMaps\PropertyValue.vb"

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

'   Total Lines: 73
'    Code Lines: 40
' Comment Lines: 24
'   Blank Lines: 9
'     File Size: 2.60 KB


'     Interface IPropertyValue
' 
'         Properties: [Property]
' 
'     Class PropertyValue
' 
'         Properties: [Property], Key, Value
' 
'         Function: ImportsLines, ImportsTsv, ToString
' 
' 
' /********************************************************************************/

#End Region

Imports System.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Scripting.Abstract

Namespace ComponentModel.DataSourceModel.SchemaMaps

    Public Interface IPropertyValue : Inherits INamedValue, Value(Of String).IValueOf
        Property [Property] As String
    End Interface

    Public Class Bind

        Protected Friend ReadOnly handleSetValue As Action(Of Object, Object)
        Protected Friend ReadOnly handleGetValue As Func(Of Object, Object)

        ''' <summary>
        ''' The property/field object that bind with its custom attribute <see cref="field"/> of type 
        ''' </summary>
        Protected member As MemberInfo
        Protected caster As LoadObject

        Sub New(prop As PropertyInfo)
            ' Compile the property get/set as the delegate
            With prop
                handleSetValue = AddressOf prop.SetValue  ' .DeclaringType.PropertySet(.Name)
                handleGetValue = AddressOf prop.GetValue  ' .DeclaringType.PropertyGet(.Name)
            End With

            member = prop
        End Sub

        Sub New(field As FieldInfo)
            With field
                handleSetValue = AddressOf field.SetValue  ' .DeclaringType.FieldSet(.Name)
                handleGetValue = AddressOf field.GetValue  ' .DeclaringType.FieldGet(.Name)
            End With

            Me.member = field
        End Sub

        Sub New(method As MethodInfo)
            If method.IsNonParametric(optionalAsNone:=True) Then
                Throw New InvalidConstraintException("Only allows parameterless method or all of the parameter should be optional!")
            End If

            With method
                handleSetValue = Sub(a, b)
                                     Throw New ReadOnlyException
                                 End Sub
                handleGetValue = Function(obj) method.Invoke(obj, {})
            End With

            Me.member = method
        End Sub
    End Class
End Namespace

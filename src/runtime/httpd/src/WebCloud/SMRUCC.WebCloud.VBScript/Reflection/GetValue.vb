#Region "Microsoft.VisualBasic::cc9eb6779b5c2c20c08f3c110eeabaa9, G:/GCModeller/src/runtime/httpd/src/WebCloud/SMRUCC.WebCloud.VBScript//Reflection/GetValue.vb"

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

    '   Total Lines: 86
    '    Code Lines: 67
    ' Comment Lines: 1
    '   Blank Lines: 18
    '     File Size: 3.25 KB


    '     Class GetValue
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: GetValue, Read
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.MIME.application.json.Javascript

Namespace Reflection

    Friend Class GetValue

        ReadOnly clr As Type
        ReadOnly schema As New Dictionary(Of String, MemberInfo)

        Sub New(type As Type)
            clr = type

            For Each prop As PropertyInfo In clr.GetProperties(BindingFlags.Public Or BindingFlags.Instance)
                If prop.GetIndexParameters.Any Then
                    Continue For
                ElseIf Not prop.CanRead Then
                    Continue For
                End If

                schema(prop.Name.ToLower) = prop
            Next

            For Each field As FieldInfo In clr.GetFields(BindingFlags.Public Or BindingFlags.Instance)
                schema(field.Name.ToLower) = field
            Next

            For Each func As MethodInfo In clr.GetMethods(BindingFlags.Public Or BindingFlags.Instance)
                If func.ReturnType Is GetType(Void) Then
                    Continue For
                ElseIf func.GetParameters.Any Then
                    Continue For
                End If

                schema(func.Name.ToLower) = func
            Next
        End Sub

        Private Function GetValue(obj As Object, name As String) As Object
            Dim key As String = name.ToLower

            If Not schema.ContainsKey(key) Then
                Throw New MissingMemberException($"missing the property/field/function memeber which is named '{name}' in clr type {clr.FullName}.")
            Else
                Dim reader As MemberInfo = schema(key)

                If TypeOf reader Is PropertyInfo Then
                    Return DirectCast(reader, PropertyInfo).GetValue(obj)
                ElseIf TypeOf reader Is FieldInfo Then
                    Return DirectCast(reader, FieldInfo).GetValue(obj)
                ElseIf TypeOf reader Is MethodInfo Then
                    Return DirectCast(reader, MethodInfo).Invoke(obj, {})
                Else
                    Throw New InvalidProgramException($"unknown clr member type {reader.GetType.FullName} for get value by name!")
                End If
            End If
        End Function

        Public Shared Function Read(obj As Object, name As String) As Object
            If obj Is Nothing Then
                ' this will chaining nothing to next token
                Return Nothing
            End If

            Dim type As Type = obj.GetType

            If type Is GetType(JsonObject) Then
                Dim json As JsonObject = DirectCast(obj, JsonObject)
                Dim key As String = json.ObjectKeys.Where(Function(str) str.TextEquals(name)).FirstOrDefault

                If key Is Nothing Then
                    Return Nothing
                Else
                    Return json(key)
                End If
            End If

            Static cache As New Dictionary(Of Type, GetValue)

            Return cache _
                .ComputeIfAbsent(type, Function(t) New GetValue(t)) _
                .GetValue(obj, name)
        End Function
    End Class
End Namespace

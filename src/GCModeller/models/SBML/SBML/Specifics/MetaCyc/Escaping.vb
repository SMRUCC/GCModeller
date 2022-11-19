#Region "Microsoft.VisualBasic::1db7cd7417d950087b252b48935aa1ac, GCModeller\models\SBML\SBML\Specifics\MetaCyc\Escaping.vb"

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

    '   Total Lines: 88
    '    Code Lines: 63
    ' Comment Lines: 11
    '   Blank Lines: 14
    '     File Size: 3.78 KB


    '     Structure Escaping
    ' 
    '         Properties: Escape, Original
    ' 
    '         Function: DefaultEscapes, ToString
    ' 
    '         Sub: (+2 Overloads) Replace
    ' 
    '     Class EscapedAttribute
    ' 
    '         Properties: TypeInfo
    ' 
    '         Function: GetProperties
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Reflection
Imports System.Text
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic

Namespace Specifics.MetaCyc

    ''' <summary>
    ''' XML文件里面的字符串的转义，这个主要是针对MetaCyc数据库里面的sbml文件的操作的
    ''' </summary>
    Public Structure Escaping : Implements IKeyValuePairObject(Of String, String)

        ''' <summary>
        ''' 原来的特殊符号字符串
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property Original As String Implements IKeyValuePairObject(Of String, String).Key
        ''' <summary>
        ''' 保存在文件之中的转义字符串
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property Escape As String Implements IKeyValuePairObject(Of String, String).Value

        Public Overrides Function ToString() As String
            Return String.Format("{0} --> {1}", Escape, Original)
        End Function

        Public Shared Widening Operator CType(value As KeyValuePair(Of String, String)) As Escaping
            Return New Escaping With {
                .Escape = value.Key,
                .Original = value.Value
            }
        End Operator

        Public Shared Narrowing Operator CType(value As Escaping) As KeyValuePair(Of String, String)
            Return New KeyValuePair(Of String, String)(key:=value.Escape, value:=value.Original)
        End Operator

        Public Shared Sub Replace(Of T As Class)(source As IEnumerable(Of T), Replacement As Escaping())
            Dim type As Type = GetType(T)
            Dim lstProp As PropertyInfo() = EscapedAttribute.GetProperties(type)

            For i As Integer = 0 To source.Count - 1
                Dim Instance = source(i)
                For Each PropertyInfo As PropertyInfo In lstProp
                    Call Replace(Of T)(PropertyInfo, Instance, Replacement)
                Next
            Next
        End Sub

        Private Shared Sub Replace(Of T As Class)(PropertyInfo As Reflection.PropertyInfo, obj As T, ReplacementList As Escaping())
            Dim strTemp As StringBuilder = New StringBuilder(Scripting.ToString(PropertyInfo.GetValue(obj)))
            For Each item As Escaping In ReplacementList
                Call strTemp.Replace(item.Escape, item.Original)
            Next
            If strTemp.Chars(0) = "_"c Then
                Call strTemp.Remove(0, 1)
            End If

            Call PropertyInfo.SetValue(obj, strTemp.ToString)
        End Sub

        Public Shared Function DefaultEscapes() As Escaping()
            Return New Escaping() {
 _
                New Escaping With {.Escape = "__45__", .Original = "-"},
                New Escaping With {.Escape = "__46__", .Original = "."},
                New Escaping With {.Escape = "__43__", .Original = "+"},
                New Escaping With {.Escape = "__47__", .Original = "/"}
            }
        End Function
    End Structure

    <AttributeUsage(AttributeTargets.Property, AllowMultiple:=False, Inherited:=True)>
    Public Class EscapedAttribute : Inherits Attribute

        Public Shared ReadOnly Property TypeInfo As Type =
            GetType(EscapedAttribute)

        Public Shared Function GetProperties(type As Type) As PropertyInfo()
            Dim Properties = (From [property] As PropertyInfo In type.GetProperties(BindingFlags.Public Or BindingFlags.Instance)
                              Let attrs As Object() = [property].GetCustomAttributes(TypeInfo, inherit:=True)
                              Where Not attrs.IsNullOrEmpty
                              Select [property]).ToArray
            Return Properties
        End Function
    End Class
End Namespace

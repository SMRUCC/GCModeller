#Region "Microsoft.VisualBasic::5d33d83b8fbf7a2ed156287f226581d3, ..\LibMySQL\Module\Commonly.vb"

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

Imports System.Reflection
Imports Oracle.LinuxCompatibility.MySQL.Client.Reflection.DbAttributes
Imports System.Runtime.CompilerServices

Module Commonly
    Public Const SERVERSITE As String = ".+[:]\d+"

    ''' <summary>
    ''' Get the specific type of custom attribute from a property.
    ''' (从一个属性对象中获取特定的自定义属性对象)
    ''' </summary>
    ''' <typeparam name="T">The type of the custom attribute.(自定义属性的类型)</typeparam>
    ''' <param name="Property">Target property object.(目标属性对象)</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Extension> Public Function GetAttribute(Of T As Attribute)([Property] As PropertyInfo) As T
        Dim Attributes As Object() = [Property].GetCustomAttributes(GetType(T), True)

        If Not Attributes Is Nothing AndAlso Attributes.Length = 1 Then
            Dim CustomAttr As T = CType(Attributes(0), T)

            If Not CustomAttr Is Nothing Then
                Return CustomAttr
            End If
        End If
        Return Nothing
    End Function

    Public MySqlDbTypes As Dictionary(Of Type, Oracle.LinuxCompatibility.MySQL.Client.Reflection.DbAttributes.MySqlDbType) =
        New Dictionary(Of Type, MySqlDbType) From {
 _
            {GetType(String), MySqlDbType.Text},
            {GetType(Integer), MySqlDbType.MediumInt},
            {GetType(Long), MySqlDbType.BigInt},
            {GetType(Double), MySqlDbType.Double},
            {GetType(Decimal), MySqlDbType.Decimal},
            {GetType(Date), MySqlDbType.Date},
            {GetType(Byte), MySqlDbType.Byte},
            {GetType([Enum]), MySqlDbType.Enum}}

    ''' <summary>
    ''' Get the data type of a field in the data table.
    ''' (获取数据表之中的某一个域的数据类型)
    ''' </summary>
    ''' <param name="Type"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Extension> Public Function GetDbDataType(Type As Type) As Oracle.LinuxCompatibility.MySQL.Client.Reflection.DbAttributes.MySqlDbType
        If MySqlDbTypes.ContainsKey(Type) Then
            Return MySqlDbTypes(Type)
        Else
            Return MySqlDbType.Text
        End If
    End Function

    Public ReadOnly Numerics As List(Of Oracle.LinuxCompatibility.MySQL.Client.Reflection.DbAttributes.MySqlDbType) =
        New List(Of MySqlDbType) From {
 _
            MySqlDbType.BigInt, MySqlDbType.Bit, MySqlDbType.Byte, MySqlDbType.Decimal, MySqlDbType.Double,
            MySqlDbType.Enum, MySqlDbType.Float, MySqlDbType.Int16, MySqlDbType.Int24, MySqlDbType.Int32,
            MySqlDbType.Int64, MySqlDbType.MediumInt, MySqlDbType.TinyInt, MySqlDbType.UByte, MySqlDbType.UInt16,
            MySqlDbType.UInt24, MySqlDbType.UInt32, MySqlDbType.UInt64, MySqlDbType.Year}
End Module


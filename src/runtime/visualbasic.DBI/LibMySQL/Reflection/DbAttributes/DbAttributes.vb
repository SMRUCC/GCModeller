#Region "Microsoft.VisualBasic::b21654a42a0c6557bab5c4f27faf80c4, ..\LibMySQL\Reflection\DbAttributes\DbAttributes.vb"

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

Namespace Reflection.DbAttributes

    ''' <summary>
    ''' The field attribute in the database.
    ''' (数据库中的字段的属性)
    ''' </summary>
    ''' <remarks></remarks>
    Public MustInherit Class DbAttribute : Inherits Attribute
    End Class

    ''' <summary>
    ''' Custom attribute class to mapping the field in the data table.
    ''' (用于映射数据库中的表中的某一个字段的自定义属性类型)
    ''' </summary>
    ''' <remarks></remarks>
    <AttributeUsage(AttributeTargets.Property Or AttributeTargets.Field, AllowMultiple:=False, Inherited:=True)>
    Public Class DatabaseField : Inherits DbAttribute

        ''' <summary>
        ''' Get or set the name of the database field.
        ''' (获取或者设置数据库表中的字段的名称)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Name As String

        Sub New(Optional Name As String = "")
            Me.Name = Name
        End Sub

        Public Overrides Function ToString() As String
            Return Name
        End Function

        ''' <summary>
        ''' Get the field name property.
        ''' (获取字段名)
        ''' </summary>
        ''' <param name="DbField"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Narrowing Operator CType(DbField As DatabaseField) As String
            Return DbField.Name
        End Operator
    End Class

    <AttributeUsage(AttributeTargets.Class Or AttributeTargets.Struct, AllowMultiple:=False, Inherited:=True)>
    Public Class TableName : Inherits DbAttribute

        ''' <summary>
        ''' 数据库的表名
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public ReadOnly Property Name As String
        ''' <summary>
        ''' 这个数据表所处的数据库的名称，可选的属性
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property Database As String
        <XmlText> Public Property SchemaSQL As String

        ''' <summary>
        ''' 使用表名来初始化这个元数据属性
        ''' </summary>
        ''' <param name="Name"></param>
        Public Sub New(Name As String)
            Me.Name = Name
        End Sub

        Public Overrides Function ToString() As String
            If String.IsNullOrEmpty(Database) Then
                Return Name
            Else
                Return $"`{Database}`.`{Name}`"
            End If
        End Function

        ''' <summary>
        ''' Get the table name property.(获取表名称)
        ''' </summary>
        ''' <param name="obj"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Narrowing Operator CType(obj As TableName) As String
            Return obj.Name
        End Operator

        Public Shared Function GetTableName(Of T As Class)() As TableName
            Dim attrs As Object() = GetType(T).GetCustomAttributes(GetType(TableName), inherit:=True)
            If attrs.IsNullOrEmpty Then
                Return Nothing
            Else
                Return DirectCast(attrs(Scan0), TableName)
            End If
        End Function
    End Class

    ''' <summary>
    ''' Please notice that some data type in mysql database is not allow combine with some specific field 
    ''' attribute, and I can't find out this potential error in this code. So, when your schema definition can't 
    ''' create a table then you must check this combination is correct or not in the mysql.
    ''' (请注意：在MySql数据库中有一些数据类型是不能够和一些字段的属性组合使用的，我不能够在本代码中检查出此潜在
    ''' 的错误。故，当你定义的对象类型无法创建表的时候，请检查你的字段属性的组合是否有错误？)
    ''' </summary>
    ''' <remarks></remarks>
    <AttributeUsage(AttributeTargets.Property, AllowMultiple:=False, Inherited:=True)>
    Public Class DataType : Inherits DbAttribute

        Dim _Type As MySqlDbType
        Dim _argvs As String
        Dim _typeCastingHandle As Func(Of Object, Object)

        Public ReadOnly Property MySQLType As MySqlDbType
            Get
                Return _Type
            End Get
        End Property

        Public ReadOnly Property ParameterValue As String
            Get
                Return _argvs
            End Get
        End Property

        Sub New(DataType As MySqlDbType, Optional argvs As String = "")
            Me._Type = DataType
            Me._argvs = argvs
            Me._typeCastingHandle = _typeCasting(DataType)
        End Sub

        Public Overrides Function ToString() As String
            Return _Type.ToString & IIf(String.IsNullOrEmpty(_argvs), String.Empty, String.Format(" ({0})", _argvs))
        End Function

        Public Shared Narrowing Operator CType(dataType As DataType) As MySqlDbType
            Return dataType._Type
        End Operator

        Public Shared Widening Operator CType(Type As MySqlDbType) As DataType
            Return New DataType(Type)
        End Operator

        ''' <summary>
        ''' 可能由于操作系统的语言或者文化的差异，直接使用<see cref="DateTime"></see>的ToString方法所得到的字符串可能会在一些环境配置之下
        ''' 无法正确的插入MySQL数据库之中，所以需要使用本方法在将对象实例进行转换为SQL语句的时候进行转换为字符串
        ''' </summary>
        ''' <param name="value"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function ToMySqlDateTimeString(value As DateTime) As String
            Return String.Format("{0}-{1}-{2} {3}:{4}:{5}", value.Year, value.Month, value.Day, value.Hour, value.Minute, value.Second)
        End Function

        Public Delegate Function __typeCasting(value As Object) As Object

        Protected Shared ReadOnly _typeCasting As Dictionary(Of MySqlDbType, Func(Of Object, Object)) =
            New Dictionary(Of MySqlDbType, Func(Of Object, Object)) From {
 _
                {MySqlDbType.UInt32, AddressOf UInt32_2_UInteger},
                {MySqlDbType.Int32, Function(value As Object) If(IsDBNull(value), Nothing, value)},
                {MySqlDbType.Text, Function(value As Object) If(IsDBNull(value), "", CStr(value))},
                {MySqlDbType.String, Function(value As Object) If(IsDBNull(value), "", CStr(value))},
                {MySqlDbType.VarChar, Function(value As Object) If(IsDBNull(value), "", CStr(value))},
                {MySqlDbType.Byte, Function(value As Object) value},
                {MySqlDbType.LongBlob, Function(value As Object) If(IsDBNull(value), Nothing, CType(value, Byte()))},
                {MySqlDbType.Blob, Function(value As Object) If(IsDBNull(value), Nothing, CType(value, Byte()))},
                {MySqlDbType.MediumBlob, Function(value As Object) If(IsDBNull(value), Nothing, CType(value, Byte()))},
                {MySqlDbType.TinyBlob, Function(value As Object) If(IsDBNull(value), Nothing, CType(value, Byte()))},
                {MySqlDbType.Double, Function(value As Object) If(IsDBNull(value), Nothing, value)},
                {MySqlDbType.LongText, Function(value As Object) If(IsDBNull(value), "", value)},
                {MySqlDbType.Int64, Function(value As Object) If(IsDBNull(value), Nothing, CLng(value))},
                {MySqlDbType.Decimal, Function(value As Object) If(IsDBNull(value), Nothing, CType(value, Decimal))},
                {MySqlDbType.DateTime, Function(value As Object) If(IsDBNull(value), Nothing, CType(value, Date))}
        }

        Public Function TypeCasting(value As Object) As Object
            Return _typeCastingHandle(value)
        End Function

        Private Shared Function UInt32_2_UInteger(value As Object) As Object
            If IsDBNull(value) Then
                Return Nothing
            Else
                Return CType(Val(CStr(value)), UInteger)
            End If
        End Function
    End Class

    <AttributeUsage(AttributeTargets.Property, AllowMultiple:=False, Inherited:=True)>
    Public Class [Default] : Inherits DbAttribute

        Friend DefaultValue As String

        Public Shared Narrowing Operator CType(e As [Default]) As String
            Return e.DefaultValue
        End Operator
    End Class

    ''' <summary>
    ''' The value of this field is unique in a data table.
    ''' (本字段的值在一张表中唯一)
    ''' </summary>
    ''' <remarks></remarks>
    <AttributeUsage(AttributeTargets.Property, AllowMultiple:=False, Inherited:=True)>
    Public Class Unique : Inherits DbAttribute
    End Class

    ''' <summary>
    ''' This field is the primary key of the data table.
    ''' (本字段是本数据表的主键)
    ''' </summary>
    ''' <remarks></remarks>
    <AttributeUsage(AttributeTargets.Property, AllowMultiple:=False, Inherited:=True)>
    Public Class PrimaryKey : Inherits DbAttribute
    End Class

    ''' <summary>
    ''' The value of this field can not be null.
    ''' (本字段的值不能为空)
    ''' </summary>
    ''' <remarks></remarks>
    <AttributeUsage(AttributeTargets.Property, AllowMultiple:=False, Inherited:=True)>
    Public Class NotNULL : Inherits DbAttribute
    End Class

    <AttributeUsage(AttributeTargets.Property, AllowMultiple:=False, Inherited:=True)>
    Public Class Binary : Inherits DbAttribute
    End Class

    ''' <summary>
    ''' This filed value can not be a negative number, it just works on the number type.
    ''' (本字段的值不能够是一个负数值，本属性仅适用于数值类型)
    ''' </summary>
    ''' <remarks></remarks>
    <AttributeUsage(AttributeTargets.Property, AllowMultiple:=False, Inherited:=True)>
    Public Class Unsigned : Inherits DbAttribute
    End Class

    <AttributeUsage(AttributeTargets.Property, AllowMultiple:=False, Inherited:=True)>
    Public Class ZeroFill : Inherits DbAttribute
    End Class

    ''' <summary>
    ''' When we create new row in the table, this field's value will plus 1 automatically. 
    ''' (本属性指出本字段值将会自动加1当我们在表中新添加一条记录的时候)
    ''' </summary>
    ''' <remarks></remarks>
    <AttributeUsage(AttributeTargets.Property, AllowMultiple:=False, Inherited:=True)>
    Public Class AutoIncrement : Inherits DbAttribute
    End Class
End Namespace

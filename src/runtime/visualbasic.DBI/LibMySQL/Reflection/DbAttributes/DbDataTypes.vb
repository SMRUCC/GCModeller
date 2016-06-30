#Region "Microsoft.VisualBasic::6f59db4732d0d2f58040cd2f859105f6, ..\LibMySQL\Reflection\DbAttributes\DbDataTypes.vb"

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

Imports System.ComponentModel

Namespace Reflection.DbAttributes

    ''' <summary>
    ''' Enum all of the data type in the mysql database.
    ''' (枚举MYSQL数据库中所有的数据类型)
    ''' </summary>
    ''' <remarks></remarks>
    Public Enum MySqlDbType As Integer
        Binary = 0
        Bit
        Blob
        ''' <summary>
        ''' Long
        ''' </summary>
        ''' <remarks></remarks>
        BigInt
        [Byte]
        [Date]
        [DateTime]
        [Decimal]
        [Double]
        [Enum]
        Float
        Geometry
        Guid
        Int16
        Int24
        Int32
        Int64
        LongBlob
        LongText
        MediumBlob
        MediumText
        MediumInt
        NewDate
        NewDecimal
        [Set]
        [String]
        Text
        Time
        Timestamp
        TinyBlob
        TinyText
        TinyInt
        UByte
        UInt16
        UInt24
        UInt32
        UInt64
        VarBinary
        VarChar
        VarString
        Year
    End Enum
End Namespace



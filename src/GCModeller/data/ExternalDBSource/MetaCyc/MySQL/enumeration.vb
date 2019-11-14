#Region "Microsoft.VisualBasic::1950fbbb52bbe8f760b80db623df5073, data\ExternalDBSource\MetaCyc\MySQL\enumeration.vb"

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

    ' Class enumeration
    ' 
    '     Properties: ColumnName, Meaning, TableName, Value
    ' 
    '     Function: Clone, GetDeleteSQL, GetDumpInsertValue, (+2 Overloads) GetInsertSQL, (+2 Overloads) GetReplaceSQL
    '               GetUpdateSQL
    ' 
    ' 
    ' /********************************************************************************/

#End Region

REM  Oracle.LinuxCompatibility.MySQL.CodeSolution.VisualBasic.CodeGenerator
REM  MYSQL Schema Mapper
REM      for Microsoft VisualBasic.NET 2.1.0.2569

REM  Dump @2018/5/23 13:13:40


Imports System.Data.Linq.Mapping
Imports System.Xml.Serialization
Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes
Imports MySqlScript = Oracle.LinuxCompatibility.MySQL.Scripting.Extensions

Namespace MetaCyc.MySQL

''' <summary>
''' ```SQL
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `enumeration`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `enumeration` (
'''   `TableName` varchar(50) NOT NULL,
'''   `ColumnName` varchar(50) NOT NULL,
'''   `Value` varchar(50) NOT NULL,
'''   `Meaning` text
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("enumeration", Database:="warehouse", SchemaSQL:="
CREATE TABLE `enumeration` (
  `TableName` varchar(50) NOT NULL,
  `ColumnName` varchar(50) NOT NULL,
  `Value` varchar(50) NOT NULL,
  `Meaning` text
) ENGINE=InnoDB DEFAULT CHARSET=utf8;")>
Public Class enumeration: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("TableName"), NotNull, DataType(MySqlDbType.VarChar, "50"), Column(Name:="TableName")> Public Property TableName As String
    <DatabaseField("ColumnName"), NotNull, DataType(MySqlDbType.VarChar, "50"), Column(Name:="ColumnName")> Public Property ColumnName As String
    <DatabaseField("Value"), NotNull, DataType(MySqlDbType.VarChar, "50"), Column(Name:="Value")> Public Property Value As String
    <DatabaseField("Meaning"), DataType(MySqlDbType.Text), Column(Name:="Meaning")> Public Property Meaning As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `enumeration` (`TableName`, `ColumnName`, `Value`, `Meaning`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `enumeration` (`TableName`, `ColumnName`, `Value`, `Meaning`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `enumeration` (`TableName`, `ColumnName`, `Value`, `Meaning`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `enumeration` (`TableName`, `ColumnName`, `Value`, `Meaning`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `enumeration` WHERE ;</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `enumeration` SET `TableName`='{0}', `ColumnName`='{1}', `Value`='{2}', `Meaning`='{3}' WHERE ;</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `enumeration` WHERE ;
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Throw New NotImplementedException("Table key was Not found, unable To generate ___DELETE_SQL_Invoke automatically, please edit this Function manually!")
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `enumeration` (`TableName`, `ColumnName`, `Value`, `Meaning`) VALUES ('{0}', '{1}', '{2}', '{3}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, TableName, ColumnName, Value, Meaning)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `enumeration` (`TableName`, `ColumnName`, `Value`, `Meaning`) VALUES ('{0}', '{1}', '{2}', '{3}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, TableName, ColumnName, Value, Meaning)
        Else
        Return String.Format(INSERT_SQL, TableName, ColumnName, Value, Meaning)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{TableName}', '{ColumnName}', '{Value}', '{Meaning}')"
        Else
            Return $"('{TableName}', '{ColumnName}', '{Value}', '{Meaning}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `enumeration` (`TableName`, `ColumnName`, `Value`, `Meaning`) VALUES ('{0}', '{1}', '{2}', '{3}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, TableName, ColumnName, Value, Meaning)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `enumeration` (`TableName`, `ColumnName`, `Value`, `Meaning`) VALUES ('{0}', '{1}', '{2}', '{3}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, TableName, ColumnName, Value, Meaning)
        Else
        Return String.Format(REPLACE_SQL, TableName, ColumnName, Value, Meaning)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `enumeration` SET `TableName`='{0}', `ColumnName`='{1}', `Value`='{2}', `Meaning`='{3}' WHERE ;
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Throw New NotImplementedException("Table key was Not found, unable To generate ___UPDATE_SQL_Invoke automatically, please edit this Function manually!")
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As enumeration
                         Return DirectCast(MyClass.MemberwiseClone, enumeration)
                     End Function
End Class


End Namespace

#Region "Microsoft.VisualBasic::d9496760e4f3fd292b777044d178c5e7, data\ExternalDBSource\MetaCyc\MySQL\enumeration.vb"

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
    '     Function: GetDeleteSQL, GetDumpInsertValue, GetInsertSQL, GetReplaceSQL, GetUpdateSQL
    ' 
    ' 
    ' /********************************************************************************/

#End Region

REM  Oracle.LinuxCompatibility.MySQL.CodeSolution.VisualBasic.CodeGenerator
REM  MYSQL Schema Mapper
REM      for Microsoft VisualBasic.NET 1.0.0.0

REM  Dump @3/29/2017 8:48:56 PM


Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes

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
''' 
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("enumeration", Database:="warehouse", SchemaSQL:="
CREATE TABLE `enumeration` (
  `TableName` varchar(50) NOT NULL,
  `ColumnName` varchar(50) NOT NULL,
  `Value` varchar(50) NOT NULL,
  `Meaning` text
) ENGINE=InnoDB DEFAULT CHARSET=utf8;")>
Public Class enumeration: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("TableName"), NotNull, DataType(MySqlDbType.VarChar, "50")> Public Property TableName As String
    <DatabaseField("ColumnName"), NotNull, DataType(MySqlDbType.VarChar, "50")> Public Property ColumnName As String
    <DatabaseField("Value"), NotNull, DataType(MySqlDbType.VarChar, "50")> Public Property Value As String
    <DatabaseField("Meaning"), DataType(MySqlDbType.Text)> Public Property Meaning As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `enumeration` (`TableName`, `ColumnName`, `Value`, `Meaning`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `enumeration` (`TableName`, `ColumnName`, `Value`, `Meaning`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `enumeration` WHERE ;</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `enumeration` SET `TableName`='{0}', `ColumnName`='{1}', `Value`='{2}', `Meaning`='{3}' WHERE ;</SQL>
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
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue() As String
        Return $"('{TableName}', '{ColumnName}', '{Value}', '{Meaning}')"
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
''' UPDATE `enumeration` SET `TableName`='{0}', `ColumnName`='{1}', `Value`='{2}', `Meaning`='{3}' WHERE ;
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Throw New NotImplementedException("Table key was Not found, unable To generate ___UPDATE_SQL_Invoke automatically, please edit this Function manually!")
    End Function
#End Region
End Class


End Namespace

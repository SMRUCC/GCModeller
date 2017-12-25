#Region "Microsoft.VisualBasic::009cd033f982bdf6e88d4edbc7d91288, ..\GCModeller\data\Reactome\LocalMySQL\gk_current\parameters.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
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

REM  Oracle.LinuxCompatibility.MySQL.CodeSolution.VisualBasic.CodeGenerator
REM  MYSQL Schema Mapper
REM      for Microsoft VisualBasic.NET 1.0.0.0

REM  Dump @3/29/2017 9:40:27 PM


Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes

Namespace LocalMySQL.Tables.gk_current

''' <summary>
''' ```SQL
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `parameters`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `parameters` (
'''   `parameter` varchar(64) NOT NULL,
'''   `value` longblob,
'''   PRIMARY KEY (`parameter`)
''' ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("parameters", Database:="gk_current", SchemaSQL:="
CREATE TABLE `parameters` (
  `parameter` varchar(64) NOT NULL,
  `value` longblob,
  PRIMARY KEY (`parameter`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;")>
Public Class parameters: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("parameter"), PrimaryKey, NotNull, DataType(MySqlDbType.VarChar, "64")> Public Property parameter As String
    <DatabaseField("value"), DataType(MySqlDbType.Blob)> Public Property value As Byte()
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `parameters` (`parameter`, `value`) VALUES ('{0}', '{1}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `parameters` (`parameter`, `value`) VALUES ('{0}', '{1}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `parameters` WHERE `parameter` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `parameters` SET `parameter`='{0}', `value`='{1}' WHERE `parameter` = '{2}';</SQL>
#End Region
''' <summary>
''' ```SQL
''' DELETE FROM `parameters` WHERE `parameter` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, parameter)
    End Function
''' <summary>
''' ```SQL
''' INSERT INTO `parameters` (`parameter`, `value`) VALUES ('{0}', '{1}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, parameter, value)
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue() As String
        Return $"('{parameter}', '{value}')"
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `parameters` (`parameter`, `value`) VALUES ('{0}', '{1}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, parameter, value)
    End Function
''' <summary>
''' ```SQL
''' UPDATE `parameters` SET `parameter`='{0}', `value`='{1}' WHERE `parameter` = '{2}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, parameter, value, parameter)
    End Function
#End Region
End Class


End Namespace

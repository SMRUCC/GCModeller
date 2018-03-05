#Region "Microsoft.VisualBasic::5db3821e639f98a42a31aabb0ae4a324, data\Reactome\LocalMySQL\gk_stable_ids\name.vb"

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

    ' Class name
    ' 
    '     Function: GetDeleteSQL, GetDumpInsertValue, GetInsertSQL, GetReplaceSQL, GetUpdateSQL
    ' 
    ' 
    ' /********************************************************************************/

#End Region

REM  Oracle.LinuxCompatibility.MySQL.CodeSolution.VisualBasic.CodeGenerator
REM  MYSQL Schema Mapper
REM      for Microsoft VisualBasic.NET 1.0.0.0

REM  Dump @3/29/2017 9:40:31 PM


Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes

Namespace LocalMySQL.Tables.gk_stable_ids

''' <summary>
''' ```SQL
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `name`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `name` (
'''   `DB_ID` int(12) unsigned NOT NULL AUTO_INCREMENT,
'''   `ST_ID` int(12) unsigned NOT NULL,
'''   `name` text NOT NULL,
'''   PRIMARY KEY (`DB_ID`)
''' ) ENGINE=MyISAM AUTO_INCREMENT=1639545 DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("name", Database:="gk_stable_ids", SchemaSQL:="
CREATE TABLE `name` (
  `DB_ID` int(12) unsigned NOT NULL AUTO_INCREMENT,
  `ST_ID` int(12) unsigned NOT NULL,
  `name` text NOT NULL,
  PRIMARY KEY (`DB_ID`)
) ENGINE=MyISAM AUTO_INCREMENT=1639545 DEFAULT CHARSET=latin1;")>
Public Class name: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("DB_ID"), PrimaryKey, AutoIncrement, NotNull, DataType(MySqlDbType.Int64, "12")> Public Property DB_ID As Long
    <DatabaseField("ST_ID"), NotNull, DataType(MySqlDbType.Int64, "12")> Public Property ST_ID As Long
    <DatabaseField("name"), NotNull, DataType(MySqlDbType.Text)> Public Property name As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `name` (`ST_ID`, `name`) VALUES ('{0}', '{1}', '{2}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `name` (`ST_ID`, `name`) VALUES ('{0}', '{1}', '{2}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `name` WHERE `DB_ID` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `name` SET `DB_ID`='{0}', `ST_ID`='{1}', `name`='{2}' WHERE `DB_ID` = '{3}';</SQL>
#End Region
''' <summary>
''' ```SQL
''' DELETE FROM `name` WHERE `DB_ID` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, DB_ID)
    End Function
''' <summary>
''' ```SQL
''' INSERT INTO `name` (`ST_ID`, `name`) VALUES ('{0}', '{1}', '{2}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, ST_ID, name)
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue() As String
        Return $"('{ST_ID}', '{name}', '{2}')"
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `name` (`ST_ID`, `name`) VALUES ('{0}', '{1}', '{2}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, ST_ID, name)
    End Function
''' <summary>
''' ```SQL
''' UPDATE `name` SET `DB_ID`='{0}', `ST_ID`='{1}', `name`='{2}' WHERE `DB_ID` = '{3}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, DB_ID, ST_ID, name, DB_ID)
    End Function
#End Region
End Class


End Namespace

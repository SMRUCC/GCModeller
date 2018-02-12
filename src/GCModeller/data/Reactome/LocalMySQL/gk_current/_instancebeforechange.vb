#Region "Microsoft.VisualBasic::7bd9b7efb869312df63b879bca2982e7, data\Reactome\LocalMySQL\gk_current\_instancebeforechange.vb"

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

    ' Class _instancebeforechange
    ' 
    '     Function: GetDeleteSQL, GetDumpInsertValue, GetInsertSQL, GetReplaceSQL, GetUpdateSQL
    ' 
    ' 
    ' /********************************************************************************/

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
''' DROP TABLE IF EXISTS `_instancebeforechange`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `_instancebeforechange` (
'''   `DB_ID` int(10) unsigned NOT NULL,
'''   `changedInstanceDB_ID` int(10) DEFAULT NULL,
'''   `instanceEdit` int(10) unsigned DEFAULT NULL,
'''   `instanceEdit_class` varchar(64) DEFAULT NULL,
'''   PRIMARY KEY (`DB_ID`),
'''   KEY `changedInstanceDB_ID` (`changedInstanceDB_ID`),
'''   KEY `instanceEdit` (`instanceEdit`)
''' ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("_instancebeforechange", Database:="gk_current", SchemaSQL:="
CREATE TABLE `_instancebeforechange` (
  `DB_ID` int(10) unsigned NOT NULL,
  `changedInstanceDB_ID` int(10) DEFAULT NULL,
  `instanceEdit` int(10) unsigned DEFAULT NULL,
  `instanceEdit_class` varchar(64) DEFAULT NULL,
  PRIMARY KEY (`DB_ID`),
  KEY `changedInstanceDB_ID` (`changedInstanceDB_ID`),
  KEY `instanceEdit` (`instanceEdit`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;")>
Public Class _instancebeforechange: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("DB_ID"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "10")> Public Property DB_ID As Long
    <DatabaseField("changedInstanceDB_ID"), DataType(MySqlDbType.Int64, "10")> Public Property changedInstanceDB_ID As Long
    <DatabaseField("instanceEdit"), DataType(MySqlDbType.Int64, "10")> Public Property instanceEdit As Long
    <DatabaseField("instanceEdit_class"), DataType(MySqlDbType.VarChar, "64")> Public Property instanceEdit_class As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `_instancebeforechange` (`DB_ID`, `changedInstanceDB_ID`, `instanceEdit`, `instanceEdit_class`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `_instancebeforechange` (`DB_ID`, `changedInstanceDB_ID`, `instanceEdit`, `instanceEdit_class`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `_instancebeforechange` WHERE `DB_ID` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `_instancebeforechange` SET `DB_ID`='{0}', `changedInstanceDB_ID`='{1}', `instanceEdit`='{2}', `instanceEdit_class`='{3}' WHERE `DB_ID` = '{4}';</SQL>
#End Region
''' <summary>
''' ```SQL
''' DELETE FROM `_instancebeforechange` WHERE `DB_ID` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, DB_ID)
    End Function
''' <summary>
''' ```SQL
''' INSERT INTO `_instancebeforechange` (`DB_ID`, `changedInstanceDB_ID`, `instanceEdit`, `instanceEdit_class`) VALUES ('{0}', '{1}', '{2}', '{3}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, DB_ID, changedInstanceDB_ID, instanceEdit, instanceEdit_class)
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue() As String
        Return $"('{DB_ID}', '{changedInstanceDB_ID}', '{instanceEdit}', '{instanceEdit_class}')"
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `_instancebeforechange` (`DB_ID`, `changedInstanceDB_ID`, `instanceEdit`, `instanceEdit_class`) VALUES ('{0}', '{1}', '{2}', '{3}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, DB_ID, changedInstanceDB_ID, instanceEdit, instanceEdit_class)
    End Function
''' <summary>
''' ```SQL
''' UPDATE `_instancebeforechange` SET `DB_ID`='{0}', `changedInstanceDB_ID`='{1}', `instanceEdit`='{2}', `instanceEdit_class`='{3}' WHERE `DB_ID` = '{4}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, DB_ID, changedInstanceDB_ID, instanceEdit, instanceEdit_class, DB_ID)
    End Function
#End Region
End Class


End Namespace

#Region "Microsoft.VisualBasic::a065f88dcc62978c736cade53e9c3bab, data\RegulonDatabase\RegulonDB\MySQL\cond_effect_link.vb"

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

    ' Class cond_effect_link
    ' 
    '     Function: GetDeleteSQL, GetDumpInsertValue, GetInsertSQL, GetReplaceSQL, GetUpdateSQL
    ' 
    ' 
    ' /********************************************************************************/

#End Region

REM  Oracle.LinuxCompatibility.MySQL.CodeSolution.VisualBasic.CodeGenerator
REM  MYSQL Schema Mapper
REM      for Microsoft VisualBasic.NET 1.0.0.0

REM  Dump @3/29/2017 11:24:24 PM


Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes

Namespace RegulonDB.Tables

''' <summary>
''' ```SQL
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `cond_effect_link`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `cond_effect_link` (
'''   `cond_effect_link_id` char(12) NOT NULL,
'''   `condition_id` char(12) NOT NULL,
'''   `medium_id` char(12) NOT NULL,
'''   `effect` varchar(250) NOT NULL,
'''   `cond_effect_link_notes` varchar(2000) DEFAULT NULL
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("cond_effect_link", Database:="regulondb_93", SchemaSQL:="
CREATE TABLE `cond_effect_link` (
  `cond_effect_link_id` char(12) NOT NULL,
  `condition_id` char(12) NOT NULL,
  `medium_id` char(12) NOT NULL,
  `effect` varchar(250) NOT NULL,
  `cond_effect_link_notes` varchar(2000) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;")>
Public Class cond_effect_link: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("cond_effect_link_id"), NotNull, DataType(MySqlDbType.VarChar, "12")> Public Property cond_effect_link_id As String
    <DatabaseField("condition_id"), NotNull, DataType(MySqlDbType.VarChar, "12")> Public Property condition_id As String
    <DatabaseField("medium_id"), NotNull, DataType(MySqlDbType.VarChar, "12")> Public Property medium_id As String
    <DatabaseField("effect"), NotNull, DataType(MySqlDbType.VarChar, "250")> Public Property effect As String
    <DatabaseField("cond_effect_link_notes"), DataType(MySqlDbType.VarChar, "2000")> Public Property cond_effect_link_notes As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `cond_effect_link` (`cond_effect_link_id`, `condition_id`, `medium_id`, `effect`, `cond_effect_link_notes`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `cond_effect_link` (`cond_effect_link_id`, `condition_id`, `medium_id`, `effect`, `cond_effect_link_notes`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `cond_effect_link` WHERE ;</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `cond_effect_link` SET `cond_effect_link_id`='{0}', `condition_id`='{1}', `medium_id`='{2}', `effect`='{3}', `cond_effect_link_notes`='{4}' WHERE ;</SQL>
#End Region
''' <summary>
''' ```SQL
''' DELETE FROM `cond_effect_link` WHERE ;
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Throw New NotImplementedException("Table key was Not found, unable To generate ___DELETE_SQL_Invoke automatically, please edit this Function manually!")
    End Function
''' <summary>
''' ```SQL
''' INSERT INTO `cond_effect_link` (`cond_effect_link_id`, `condition_id`, `medium_id`, `effect`, `cond_effect_link_notes`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, cond_effect_link_id, condition_id, medium_id, effect, cond_effect_link_notes)
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue() As String
        Return $"('{cond_effect_link_id}', '{condition_id}', '{medium_id}', '{effect}', '{cond_effect_link_notes}')"
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `cond_effect_link` (`cond_effect_link_id`, `condition_id`, `medium_id`, `effect`, `cond_effect_link_notes`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, cond_effect_link_id, condition_id, medium_id, effect, cond_effect_link_notes)
    End Function
''' <summary>
''' ```SQL
''' UPDATE `cond_effect_link` SET `cond_effect_link_id`='{0}', `condition_id`='{1}', `medium_id`='{2}', `effect`='{3}', `cond_effect_link_notes`='{4}' WHERE ;
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Throw New NotImplementedException("Table key was Not found, unable To generate ___UPDATE_SQL_Invoke automatically, please edit this Function manually!")
    End Function
#End Region
End Class


End Namespace

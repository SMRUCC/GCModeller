#Region "Microsoft.VisualBasic::6afcc3708b64899d7600b3ebf8183401, data\RegulonDatabase\RegulonDB\MySQL\tu_terminator_link.vb"

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

    ' Class tu_terminator_link
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
''' DROP TABLE IF EXISTS `tu_terminator_link`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `tu_terminator_link` (
'''   `transcription_unit_id` char(12) NOT NULL,
'''   `terminator_id` char(12) NOT NULL
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("tu_terminator_link", Database:="regulondb_93", SchemaSQL:="
CREATE TABLE `tu_terminator_link` (
  `transcription_unit_id` char(12) NOT NULL,
  `terminator_id` char(12) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;")>
Public Class tu_terminator_link: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("transcription_unit_id"), NotNull, DataType(MySqlDbType.VarChar, "12")> Public Property transcription_unit_id As String
    <DatabaseField("terminator_id"), NotNull, DataType(MySqlDbType.VarChar, "12")> Public Property terminator_id As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `tu_terminator_link` (`transcription_unit_id`, `terminator_id`) VALUES ('{0}', '{1}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `tu_terminator_link` (`transcription_unit_id`, `terminator_id`) VALUES ('{0}', '{1}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `tu_terminator_link` WHERE ;</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `tu_terminator_link` SET `transcription_unit_id`='{0}', `terminator_id`='{1}' WHERE ;</SQL>
#End Region
''' <summary>
''' ```SQL
''' DELETE FROM `tu_terminator_link` WHERE ;
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Throw New NotImplementedException("Table key was Not found, unable To generate ___DELETE_SQL_Invoke automatically, please edit this Function manually!")
    End Function
''' <summary>
''' ```SQL
''' INSERT INTO `tu_terminator_link` (`transcription_unit_id`, `terminator_id`) VALUES ('{0}', '{1}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, transcription_unit_id, terminator_id)
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue() As String
        Return $"('{transcription_unit_id}', '{terminator_id}')"
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `tu_terminator_link` (`transcription_unit_id`, `terminator_id`) VALUES ('{0}', '{1}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, transcription_unit_id, terminator_id)
    End Function
''' <summary>
''' ```SQL
''' UPDATE `tu_terminator_link` SET `transcription_unit_id`='{0}', `terminator_id`='{1}' WHERE ;
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Throw New NotImplementedException("Table key was Not found, unable To generate ___UPDATE_SQL_Invoke automatically, please edit this Function manually!")
    End Function
#End Region
End Class


End Namespace

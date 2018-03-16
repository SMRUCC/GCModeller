#Region "Microsoft.VisualBasic::bce7dc262ebfda52e86ed80e49085313, data\RegulonDatabase\RegulonDB\MySQL\gu_component_link.vb"

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

    ' Class gu_component_link
    ' 
    '     Properties: component_function, component_id, gu_id, note
    ' 
    '     Function: GetDeleteSQL, GetDumpInsertValue, GetInsertSQL, GetReplaceSQL, GetUpdateSQL
    ' 
    ' 
    ' /********************************************************************************/

#End Region

REM  Oracle.LinuxCompatibility.MySQL.CodeSolution.VisualBasic.CodeGenerator
REM  MYSQL Schema Mapper
REM      for Microsoft VisualBasic.NET 2.1.0.2569

REM  Dump @3/16/2018 10:40:14 PM


Imports System.Data.Linq.Mapping
Imports System.Xml.Serialization
Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes
Imports MySqlScript = Oracle.LinuxCompatibility.MySQL.Scripting.Extensions

Namespace RegulonDB.Tables

''' <summary>
''' ```SQL
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `gu_component_link`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `gu_component_link` (
'''   `gu_id` char(12) NOT NULL,
'''   `component_id` char(12) NOT NULL,
'''   `component_function` varchar(50) NOT NULL,
'''   `note` longtext
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("gu_component_link", Database:="regulondb_93", SchemaSQL:="
CREATE TABLE `gu_component_link` (
  `gu_id` char(12) NOT NULL,
  `component_id` char(12) NOT NULL,
  `component_function` varchar(50) NOT NULL,
  `note` longtext
) ENGINE=InnoDB DEFAULT CHARSET=utf8;")>
Public Class gu_component_link: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("gu_id"), NotNull, DataType(MySqlDbType.VarChar, "12"), Column(Name:="gu_id")> Public Property gu_id As String
    <DatabaseField("component_id"), NotNull, DataType(MySqlDbType.VarChar, "12"), Column(Name:="component_id")> Public Property component_id As String
    <DatabaseField("component_function"), NotNull, DataType(MySqlDbType.VarChar, "50"), Column(Name:="component_function")> Public Property component_function As String
    <DatabaseField("note"), DataType(MySqlDbType.Text), Column(Name:="note")> Public Property note As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `gu_component_link` (`gu_id`, `component_id`, `component_function`, `note`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `gu_component_link` (`gu_id`, `component_id`, `component_function`, `note`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `gu_component_link` WHERE ;</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `gu_component_link` SET `gu_id`='{0}', `component_id`='{1}', `component_function`='{2}', `note`='{3}' WHERE ;</SQL>
#End Region
''' <summary>
''' ```SQL
''' DELETE FROM `gu_component_link` WHERE ;
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Throw New NotImplementedException("Table key was Not found, unable To generate ___DELETE_SQL_Invoke automatically, please edit this Function manually!")
    End Function
''' <summary>
''' ```SQL
''' INSERT INTO `gu_component_link` (`gu_id`, `component_id`, `component_function`, `note`) VALUES ('{0}', '{1}', '{2}', '{3}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, gu_id, component_id, component_function, note)
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue() As String
        Return $"('{gu_id}', '{component_id}', '{component_function}', '{note}')"
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `gu_component_link` (`gu_id`, `component_id`, `component_function`, `note`) VALUES ('{0}', '{1}', '{2}', '{3}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, gu_id, component_id, component_function, note)
    End Function
''' <summary>
''' ```SQL
''' UPDATE `gu_component_link` SET `gu_id`='{0}', `component_id`='{1}', `component_function`='{2}', `note`='{3}' WHERE ;
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Throw New NotImplementedException("Table key was Not found, unable To generate ___UPDATE_SQL_Invoke automatically, please edit this Function manually!")
    End Function
#End Region
Public Function Clone() As gu_component_link
                  Return DirectCast(MyClass.MemberwiseClone, gu_component_link)
              End Function
End Class


End Namespace


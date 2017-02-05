#Region "Microsoft.VisualBasic::05873b72f4807c901073f4ba24d62ff1, ..\GCModeller\data\GO_gene-ontology\GeneOntology\MySQL\go\instance_data.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
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

REM  Oracle.LinuxCompatibility.MySQL.CodeGenerator
REM  MYSQL Schema Mapper
REM      for Microsoft VisualBasic.NET 1.0.0.0

REM  Dump @9/5/2016 7:59:33 AM


Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes

Namespace MySQL.Tables

''' <summary>
''' ```SQL
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `instance_data`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `instance_data` (
'''   `release_name` varchar(255) DEFAULT NULL,
'''   `release_type` varchar(255) DEFAULT NULL,
'''   `release_notes` text,
'''   `ontology_data_version` varchar(255) DEFAULT NULL,
'''   UNIQUE KEY `release_name` (`release_name`)
''' ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("instance_data", Database:="go", SchemaSQL:="
CREATE TABLE `instance_data` (
  `release_name` varchar(255) DEFAULT NULL,
  `release_type` varchar(255) DEFAULT NULL,
  `release_notes` text,
  `ontology_data_version` varchar(255) DEFAULT NULL,
  UNIQUE KEY `release_name` (`release_name`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;")>
Public Class instance_data: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("release_name"), PrimaryKey, DataType(MySqlDbType.VarChar, "255")> Public Property release_name As String
    <DatabaseField("release_type"), DataType(MySqlDbType.VarChar, "255")> Public Property release_type As String
    <DatabaseField("release_notes"), DataType(MySqlDbType.Text)> Public Property release_notes As String
    <DatabaseField("ontology_data_version"), DataType(MySqlDbType.VarChar, "255")> Public Property ontology_data_version As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `instance_data` (`release_name`, `release_type`, `release_notes`, `ontology_data_version`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `instance_data` (`release_name`, `release_type`, `release_notes`, `ontology_data_version`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `instance_data` WHERE `release_name` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `instance_data` SET `release_name`='{0}', `release_type`='{1}', `release_notes`='{2}', `ontology_data_version`='{3}' WHERE `release_name` = '{4}';</SQL>
#End Region
''' <summary>
''' ```SQL
''' DELETE FROM `instance_data` WHERE `release_name` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, release_name)
    End Function
''' <summary>
''' ```SQL
''' INSERT INTO `instance_data` (`release_name`, `release_type`, `release_notes`, `ontology_data_version`) VALUES ('{0}', '{1}', '{2}', '{3}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, release_name, release_type, release_notes, ontology_data_version)
    End Function
''' <summary>
''' ```SQL
''' REPLACE INTO `instance_data` (`release_name`, `release_type`, `release_notes`, `ontology_data_version`) VALUES ('{0}', '{1}', '{2}', '{3}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, release_name, release_type, release_notes, ontology_data_version)
    End Function
''' <summary>
''' ```SQL
''' UPDATE `instance_data` SET `release_name`='{0}', `release_type`='{1}', `release_notes`='{2}', `ontology_data_version`='{3}' WHERE `release_name` = '{4}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, release_name, release_type, release_notes, ontology_data_version, release_name)
    End Function
#End Region
End Class


End Namespace

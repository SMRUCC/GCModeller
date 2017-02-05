#Region "Microsoft.VisualBasic::d31070947a4f97d7ca054f6f27c4515a, ..\GCModeller\data\GO_gene-ontology\GeneOntology\MySQL\go\graph_path2term.vb"

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
''' DROP TABLE IF EXISTS `graph_path2term`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `graph_path2term` (
'''   `graph_path_id` int(11) NOT NULL,
'''   `term_id` int(11) NOT NULL,
'''   `rank` int(11) NOT NULL,
'''   KEY `graph_path_id` (`graph_path_id`),
'''   KEY `term_id` (`term_id`)
''' ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("graph_path2term", Database:="go", SchemaSQL:="
CREATE TABLE `graph_path2term` (
  `graph_path_id` int(11) NOT NULL,
  `term_id` int(11) NOT NULL,
  `rank` int(11) NOT NULL,
  KEY `graph_path_id` (`graph_path_id`),
  KEY `term_id` (`term_id`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;")>
Public Class graph_path2term: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("graph_path_id"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "11")> Public Property graph_path_id As Long
    <DatabaseField("term_id"), NotNull, DataType(MySqlDbType.Int64, "11")> Public Property term_id As Long
    <DatabaseField("rank"), NotNull, DataType(MySqlDbType.Int64, "11")> Public Property rank As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `graph_path2term` (`graph_path_id`, `term_id`, `rank`) VALUES ('{0}', '{1}', '{2}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `graph_path2term` (`graph_path_id`, `term_id`, `rank`) VALUES ('{0}', '{1}', '{2}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `graph_path2term` WHERE `graph_path_id` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `graph_path2term` SET `graph_path_id`='{0}', `term_id`='{1}', `rank`='{2}' WHERE `graph_path_id` = '{3}';</SQL>
#End Region
''' <summary>
''' ```SQL
''' DELETE FROM `graph_path2term` WHERE `graph_path_id` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, graph_path_id)
    End Function
''' <summary>
''' ```SQL
''' INSERT INTO `graph_path2term` (`graph_path_id`, `term_id`, `rank`) VALUES ('{0}', '{1}', '{2}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, graph_path_id, term_id, rank)
    End Function
''' <summary>
''' ```SQL
''' REPLACE INTO `graph_path2term` (`graph_path_id`, `term_id`, `rank`) VALUES ('{0}', '{1}', '{2}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, graph_path_id, term_id, rank)
    End Function
''' <summary>
''' ```SQL
''' UPDATE `graph_path2term` SET `graph_path_id`='{0}', `term_id`='{1}', `rank`='{2}' WHERE `graph_path_id` = '{3}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, graph_path_id, term_id, rank, graph_path_id)
    End Function
#End Region
End Class


End Namespace

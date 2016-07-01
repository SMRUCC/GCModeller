#Region "Microsoft.VisualBasic::400cc12bd688066c4acbd31f397cf542, ..\GCModeller\data\GO_gene-ontology\MySQL\go\graph_path.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
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
REM      for Microsoft VisualBasic.NET 

REM  Dump @12/3/2015 8:49:20 PM


Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes

Namespace MySQL.Tables

''' <summary>
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `graph_path`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `graph_path` (
'''   `id` int(11) NOT NULL AUTO_INCREMENT,
'''   `term1_id` int(11) NOT NULL,
'''   `term2_id` int(11) NOT NULL,
'''   `relationship_type_id` int(11) DEFAULT NULL,
'''   `distance` int(11) DEFAULT NULL,
'''   `relation_distance` int(11) DEFAULT NULL,
'''   PRIMARY KEY (`id`),
'''   UNIQUE KEY `graph_path0` (`id`),
'''   KEY `relationship_type_id` (`relationship_type_id`),
'''   KEY `graph_path1` (`term1_id`),
'''   KEY `graph_path2` (`term2_id`),
'''   KEY `graph_path3` (`term1_id`,`term2_id`),
'''   KEY `graph_path4` (`term1_id`,`distance`),
'''   KEY `graph_path5` (`term1_id`,`term2_id`,`relationship_type_id`),
'''   KEY `graph_path6` (`term1_id`,`term2_id`,`relationship_type_id`,`distance`,`relation_distance`),
'''   KEY `graph_path7` (`term2_id`,`relationship_type_id`),
'''   KEY `graph_path8` (`term1_id`,`relationship_type_id`)
''' ) ENGINE=MyISAM AUTO_INCREMENT=1226557 DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("graph_path", Database:="go")>
Public Class graph_path: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("id"), PrimaryKey, AutoIncrement, NotNull, DataType(MySqlDbType.Int64, "11")> Public Property id As Long
    <DatabaseField("term1_id"), NotNull, DataType(MySqlDbType.Int64, "11")> Public Property term1_id As Long
    <DatabaseField("term2_id"), NotNull, DataType(MySqlDbType.Int64, "11")> Public Property term2_id As Long
    <DatabaseField("relationship_type_id"), DataType(MySqlDbType.Int64, "11")> Public Property relationship_type_id As Long
    <DatabaseField("distance"), DataType(MySqlDbType.Int64, "11")> Public Property distance As Long
    <DatabaseField("relation_distance"), DataType(MySqlDbType.Int64, "11")> Public Property relation_distance As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `graph_path` (`term1_id`, `term2_id`, `relationship_type_id`, `distance`, `relation_distance`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `graph_path` (`term1_id`, `term2_id`, `relationship_type_id`, `distance`, `relation_distance`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `graph_path` WHERE `id` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `graph_path` SET `id`='{0}', `term1_id`='{1}', `term2_id`='{2}', `relationship_type_id`='{3}', `distance`='{4}', `relation_distance`='{5}' WHERE `id` = '{6}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, id)
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, term1_id, term2_id, relationship_type_id, distance, relation_distance)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, term1_id, term2_id, relationship_type_id, distance, relation_distance)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, id, term1_id, term2_id, relationship_type_id, distance, relation_distance, id)
    End Function
#End Region
End Class


End Namespace

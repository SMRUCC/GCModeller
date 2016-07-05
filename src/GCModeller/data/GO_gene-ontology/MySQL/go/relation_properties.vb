#Region "Microsoft.VisualBasic::6ddb1a5ad6d5c440428652379e962686, ..\GCModeller\data\GO_gene-ontology\MySQL\go\relation_properties.vb"

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
''' DROP TABLE IF EXISTS `relation_properties`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `relation_properties` (
'''   `relationship_type_id` int(11) NOT NULL,
'''   `is_transitive` int(11) DEFAULT NULL,
'''   `is_symmetric` int(11) DEFAULT NULL,
'''   `is_anti_symmetric` int(11) DEFAULT NULL,
'''   `is_cyclic` int(11) DEFAULT NULL,
'''   `is_reflexive` int(11) DEFAULT NULL,
'''   `is_metadata_tag` int(11) DEFAULT NULL,
'''   UNIQUE KEY `relationship_type_id` (`relationship_type_id`),
'''   UNIQUE KEY `rp1` (`relationship_type_id`)
''' ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("relation_properties", Database:="go")>
Public Class relation_properties: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("relationship_type_id"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "11")> Public Property relationship_type_id As Long
    <DatabaseField("is_transitive"), DataType(MySqlDbType.Int64, "11")> Public Property is_transitive As Long
    <DatabaseField("is_symmetric"), DataType(MySqlDbType.Int64, "11")> Public Property is_symmetric As Long
    <DatabaseField("is_anti_symmetric"), DataType(MySqlDbType.Int64, "11")> Public Property is_anti_symmetric As Long
    <DatabaseField("is_cyclic"), DataType(MySqlDbType.Int64, "11")> Public Property is_cyclic As Long
    <DatabaseField("is_reflexive"), DataType(MySqlDbType.Int64, "11")> Public Property is_reflexive As Long
    <DatabaseField("is_metadata_tag"), DataType(MySqlDbType.Int64, "11")> Public Property is_metadata_tag As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `relation_properties` (`relationship_type_id`, `is_transitive`, `is_symmetric`, `is_anti_symmetric`, `is_cyclic`, `is_reflexive`, `is_metadata_tag`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `relation_properties` (`relationship_type_id`, `is_transitive`, `is_symmetric`, `is_anti_symmetric`, `is_cyclic`, `is_reflexive`, `is_metadata_tag`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `relation_properties` WHERE `relationship_type_id` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `relation_properties` SET `relationship_type_id`='{0}', `is_transitive`='{1}', `is_symmetric`='{2}', `is_anti_symmetric`='{3}', `is_cyclic`='{4}', `is_reflexive`='{5}', `is_metadata_tag`='{6}' WHERE `relationship_type_id` = '{7}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, relationship_type_id)
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, relationship_type_id, is_transitive, is_symmetric, is_anti_symmetric, is_cyclic, is_reflexive, is_metadata_tag)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, relationship_type_id, is_transitive, is_symmetric, is_anti_symmetric, is_cyclic, is_reflexive, is_metadata_tag)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, relationship_type_id, is_transitive, is_symmetric, is_anti_symmetric, is_cyclic, is_reflexive, is_metadata_tag, relationship_type_id)
    End Function
#End Region
End Class


End Namespace

#Region "Microsoft.VisualBasic::eb36099437af682bae9fc6f4ee7b1f18, ..\GCModeller\data\GO_gene-ontology\MySQL\Tables\intersection_of.vb"

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
''' DROP TABLE IF EXISTS `intersection_of`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `intersection_of` (
'''   `id` int(11) NOT NULL AUTO_INCREMENT,
'''   `relationship_type_id` int(11) NOT NULL,
'''   `term1_id` int(11) NOT NULL,
'''   `term2_id` int(11) NOT NULL,
'''   PRIMARY KEY (`id`),
'''   UNIQUE KEY `term1_id` (`term1_id`,`term2_id`,`relationship_type_id`),
'''   KEY `relationship_type_id` (`relationship_type_id`),
'''   KEY `term2_id` (`term2_id`)
''' ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("intersection_of", Database:="go")>
Public Class intersection_of: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("id"), PrimaryKey, AutoIncrement, NotNull, DataType(MySqlDbType.Int64, "11")> Public Property id As Long
    <DatabaseField("relationship_type_id"), NotNull, DataType(MySqlDbType.Int64, "11")> Public Property relationship_type_id As Long
    <DatabaseField("term1_id"), NotNull, DataType(MySqlDbType.Int64, "11")> Public Property term1_id As Long
    <DatabaseField("term2_id"), NotNull, DataType(MySqlDbType.Int64, "11")> Public Property term2_id As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `intersection_of` (`relationship_type_id`, `term1_id`, `term2_id`) VALUES ('{0}', '{1}', '{2}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `intersection_of` (`relationship_type_id`, `term1_id`, `term2_id`) VALUES ('{0}', '{1}', '{2}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `intersection_of` WHERE `id` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `intersection_of` SET `id`='{0}', `relationship_type_id`='{1}', `term1_id`='{2}', `term2_id`='{3}' WHERE `id` = '{4}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, id)
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, relationship_type_id, term1_id, term2_id)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, relationship_type_id, term1_id, term2_id)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, id, relationship_type_id, term1_id, term2_id, id)
    End Function
#End Region
End Class


End Namespace

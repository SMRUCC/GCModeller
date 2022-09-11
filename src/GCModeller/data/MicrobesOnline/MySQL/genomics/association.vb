#Region "Microsoft.VisualBasic::2850c1866245edc77568e6c6c8eca7b4, GCModeller\data\MicrobesOnline\MySQL\genomics\association.vb"

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


    ' Code Statistics:

    '   Total Lines: 76
    '    Code Lines: 39
    ' Comment Lines: 30
    '   Blank Lines: 7
    '     File Size: 3.87 KB


    ' Class association
    ' 
    '     Properties: assocdate, gene_product_id, id, is_not, role_group
    '                 source_db_id, term_id
    ' 
    '     Function: GetDeleteSQL, GetInsertSQL, GetReplaceSQL, GetUpdateSQL
    ' 
    ' 
    ' /********************************************************************************/

#End Region

REM  Oracle.LinuxCompatibility.MySQL.CodeGenerator
REM  MYSQL Schema Mapper
REM      for Microsoft VisualBasic.NET 

REM  Dump @12/3/2015 8:30:29 PM


Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes

Namespace MySQL.genomics

''' <summary>
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `association`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `association` (
'''   `id` int(11) NOT NULL AUTO_INCREMENT,
'''   `term_id` int(11) NOT NULL DEFAULT '0',
'''   `gene_product_id` int(11) NOT NULL DEFAULT '0',
'''   `is_not` int(11) DEFAULT NULL,
'''   `role_group` int(11) DEFAULT NULL,
'''   `assocdate` int(11) DEFAULT NULL,
'''   `source_db_id` int(11) DEFAULT NULL,
'''   PRIMARY KEY (`id`),
'''   UNIQUE KEY `a0` (`id`),
'''   KEY `source_db_id` (`source_db_id`),
'''   KEY `a1` (`term_id`),
'''   KEY `a2` (`gene_product_id`),
'''   KEY `a3` (`term_id`,`gene_product_id`),
'''   KEY `a4` (`id`,`term_id`,`gene_product_id`),
'''   KEY `a5` (`id`,`gene_product_id`)
''' ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("association")>
Public Class association: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("id"), PrimaryKey, AutoIncrement, NotNull, DataType(MySqlDbType.Int64, "11")> Public Property id As Long
    <DatabaseField("term_id"), NotNull, DataType(MySqlDbType.Int64, "11")> Public Property term_id As Long
    <DatabaseField("gene_product_id"), NotNull, DataType(MySqlDbType.Int64, "11")> Public Property gene_product_id As Long
    <DatabaseField("is_not"), DataType(MySqlDbType.Int64, "11")> Public Property is_not As Long
    <DatabaseField("role_group"), DataType(MySqlDbType.Int64, "11")> Public Property role_group As Long
    <DatabaseField("assocdate"), DataType(MySqlDbType.Int64, "11")> Public Property assocdate As Long
    <DatabaseField("source_db_id"), DataType(MySqlDbType.Int64, "11")> Public Property source_db_id As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `association` (`term_id`, `gene_product_id`, `is_not`, `role_group`, `assocdate`, `source_db_id`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `association` (`term_id`, `gene_product_id`, `is_not`, `role_group`, `assocdate`, `source_db_id`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `association` WHERE `id` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `association` SET `id`='{0}', `term_id`='{1}', `gene_product_id`='{2}', `is_not`='{3}', `role_group`='{4}', `assocdate`='{5}', `source_db_id`='{6}' WHERE `id` = '{7}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, id)
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, term_id, gene_product_id, is_not, role_group, assocdate, source_db_id)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, term_id, gene_product_id, is_not, role_group, assocdate, source_db_id)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, id, term_id, gene_product_id, is_not, role_group, assocdate, source_db_id, id)
    End Function
#End Region
End Class


End Namespace

#Region "Microsoft.VisualBasic::8e5e66f7aeb15bcf0f894c4fa7e0fb9a, ..\GCModeller\data\RegulonDatabase\Regtransbase\MySQL\genome2ncbitaxon.vb"

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
REM      for Microsoft VisualBasic.NET 

REM  Dump @12/3/2015 8:07:30 PM


Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes

Namespace Regtransbase.MySQL

''' <summary>
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `genome2ncbitaxon`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `genome2ncbitaxon` (
'''   `genome_guid` int(10) unsigned NOT NULL DEFAULT '0',
'''   `ncbi_tax_id` int(10) unsigned NOT NULL DEFAULT '0',
'''   PRIMARY KEY (`genome_guid`)
''' ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("genome2ncbitaxon", Database:="dbregulation_update")>
Public Class genome2ncbitaxon: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("genome_guid"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "10")> Public Property genome_guid As Long
    <DatabaseField("ncbi_tax_id"), NotNull, DataType(MySqlDbType.Int64, "10")> Public Property ncbi_tax_id As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `genome2ncbitaxon` (`genome_guid`, `ncbi_tax_id`) VALUES ('{0}', '{1}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `genome2ncbitaxon` (`genome_guid`, `ncbi_tax_id`) VALUES ('{0}', '{1}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `genome2ncbitaxon` WHERE `genome_guid` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `genome2ncbitaxon` SET `genome_guid`='{0}', `ncbi_tax_id`='{1}' WHERE `genome_guid` = '{2}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, genome_guid)
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, genome_guid, ncbi_tax_id)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, genome_guid, ncbi_tax_id)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, genome_guid, ncbi_tax_id, genome_guid)
    End Function
#End Region
End Class


End Namespace

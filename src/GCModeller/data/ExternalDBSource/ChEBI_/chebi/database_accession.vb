#Region "Microsoft.VisualBasic::c0443cf6ca9ab5142e9d5b681f868b1e, ..\GCModeller\data\ExternalDBSource\ChEBI_\chebi\database_accession.vb"

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
REM  Microsoft VisualBasic MYSQL




Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes

Namespace ChEBI.Tables

''' <summary>
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `database_accession`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `database_accession` (
'''   `id` int(11) NOT NULL,
'''   `compound_id` int(11) NOT NULL,
'''   `accession_number` varchar(255) NOT NULL,
'''   `type` text NOT NULL,
'''   `source` text NOT NULL,
'''   PRIMARY KEY (`id`),
'''   KEY `compound_id` (`compound_id`),
'''   CONSTRAINT `FK_DB_ACCESSION_TO_COMPOUND` FOREIGN KEY (`compound_id`) REFERENCES `compounds` (`id`)
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("database_accession")>
Public Class database_accession: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("id"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "11")> Public Property id As Long
    <DatabaseField("compound_id"), NotNull, DataType(MySqlDbType.Int64, "11")> Public Property compound_id As Long
    <DatabaseField("accession_number"), NotNull, DataType(MySqlDbType.VarChar, "255")> Public Property accession_number As String
    <DatabaseField("type"), NotNull, DataType(MySqlDbType.Text)> Public Property type As String
    <DatabaseField("source"), NotNull, DataType(MySqlDbType.Text)> Public Property source As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `database_accession` (`id`, `compound_id`, `accession_number`, `type`, `source`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `database_accession` WHERE `id`='{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `database_accession` SET `id`='{0}', `compound_id`='{1}', `accession_number`='{2}', `type`='{3}', `source`='{4}' WHERE `id`='{5}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, id)
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, id, compound_id, accession_number, type, source)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, id, compound_id, accession_number, type, source, id)
    End Function
#End Region
End Class


End Namespace

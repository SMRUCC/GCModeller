#Region "Microsoft.VisualBasic::e6a6d1758c00483213d013f5b9060412, ..\repository\DataMySql\Xfam\Rfam\Tables\rfamseq.vb"

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

REM  Oracle.LinuxCompatibility.MySQL.CodeSolution.VisualBasic.CodeGenerator
REM  MYSQL Schema Mapper
REM      for Microsoft VisualBasic.NET 1.0.0.0

REM  Dump @3/29/2017 11:55:32 PM


Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes

Namespace Xfam.Rfam.MySQL.Tables

''' <summary>
''' ```SQL
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `rfamseq`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `rfamseq` (
'''   `rfamseq_acc` varchar(20) NOT NULL DEFAULT '' COMMENT 'This should be ',
'''   `accession` varchar(15) NOT NULL,
'''   `version` int(6) NOT NULL,
'''   `ncbi_id` int(10) unsigned NOT NULL,
'''   `mol_type` enum('protein','genomic DNA','DNA','ss-DNA','RNA','genomic RNA','ds-RNA','ss-cRNA','ss-RNA','mRNA','tRNA','rRNA','snoRNA','snRNA','scRNA','pre-RNA','other RNA','other DNA','unassigned DNA','unassigned RNA','viral cRNA','cRNA','transcribed RNA') NOT NULL,
'''   `length` int(10) unsigned DEFAULT '0',
'''   `description` varchar(250) NOT NULL DEFAULT '',
'''   `previous_acc` mediumtext,
'''   `source` char(20) NOT NULL,
'''   PRIMARY KEY (`rfamseq_acc`),
'''   UNIQUE KEY `rfamseq_acc` (`rfamseq_acc`),
'''   KEY `version` (`version`),
'''   KEY `fk_rfamseq_taxonomy1_idx` (`ncbi_id`)
''' ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("rfamseq", Database:="rfam_12_2", SchemaSQL:="
CREATE TABLE `rfamseq` (
  `rfamseq_acc` varchar(20) NOT NULL DEFAULT '' COMMENT 'This should be ',
  `accession` varchar(15) NOT NULL,
  `version` int(6) NOT NULL,
  `ncbi_id` int(10) unsigned NOT NULL,
  `mol_type` enum('protein','genomic DNA','DNA','ss-DNA','RNA','genomic RNA','ds-RNA','ss-cRNA','ss-RNA','mRNA','tRNA','rRNA','snoRNA','snRNA','scRNA','pre-RNA','other RNA','other DNA','unassigned DNA','unassigned RNA','viral cRNA','cRNA','transcribed RNA') NOT NULL,
  `length` int(10) unsigned DEFAULT '0',
  `description` varchar(250) NOT NULL DEFAULT '',
  `previous_acc` mediumtext,
  `source` char(20) NOT NULL,
  PRIMARY KEY (`rfamseq_acc`),
  UNIQUE KEY `rfamseq_acc` (`rfamseq_acc`),
  KEY `version` (`version`),
  KEY `fk_rfamseq_taxonomy1_idx` (`ncbi_id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;")>
Public Class rfamseq: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
''' <summary>
''' This should be 
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
    <DatabaseField("rfamseq_acc"), PrimaryKey, NotNull, DataType(MySqlDbType.VarChar, "20")> Public Property rfamseq_acc As String
    <DatabaseField("accession"), NotNull, DataType(MySqlDbType.VarChar, "15")> Public Property accession As String
    <DatabaseField("version"), NotNull, DataType(MySqlDbType.Int64, "6")> Public Property version As Long
    <DatabaseField("ncbi_id"), NotNull, DataType(MySqlDbType.Int64, "10")> Public Property ncbi_id As Long
    <DatabaseField("mol_type"), NotNull, DataType(MySqlDbType.String)> Public Property mol_type As String
    <DatabaseField("length"), DataType(MySqlDbType.Int64, "10")> Public Property length As Long
    <DatabaseField("description"), NotNull, DataType(MySqlDbType.VarChar, "250")> Public Property description As String
    <DatabaseField("previous_acc"), DataType(MySqlDbType.Text)> Public Property previous_acc As String
    <DatabaseField("source"), NotNull, DataType(MySqlDbType.VarChar, "20")> Public Property source As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `rfamseq` (`rfamseq_acc`, `accession`, `version`, `ncbi_id`, `mol_type`, `length`, `description`, `previous_acc`, `source`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `rfamseq` (`rfamseq_acc`, `accession`, `version`, `ncbi_id`, `mol_type`, `length`, `description`, `previous_acc`, `source`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `rfamseq` WHERE `rfamseq_acc` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `rfamseq` SET `rfamseq_acc`='{0}', `accession`='{1}', `version`='{2}', `ncbi_id`='{3}', `mol_type`='{4}', `length`='{5}', `description`='{6}', `previous_acc`='{7}', `source`='{8}' WHERE `rfamseq_acc` = '{9}';</SQL>
#End Region
''' <summary>
''' ```SQL
''' DELETE FROM `rfamseq` WHERE `rfamseq_acc` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, rfamseq_acc)
    End Function
''' <summary>
''' ```SQL
''' INSERT INTO `rfamseq` (`rfamseq_acc`, `accession`, `version`, `ncbi_id`, `mol_type`, `length`, `description`, `previous_acc`, `source`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, rfamseq_acc, accession, version, ncbi_id, mol_type, length, description, previous_acc, source)
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue() As String
        Return $"('{rfamseq_acc}', '{accession}', '{version}', '{ncbi_id}', '{mol_type}', '{length}', '{description}', '{previous_acc}', '{source}')"
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `rfamseq` (`rfamseq_acc`, `accession`, `version`, `ncbi_id`, `mol_type`, `length`, `description`, `previous_acc`, `source`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, rfamseq_acc, accession, version, ncbi_id, mol_type, length, description, previous_acc, source)
    End Function
''' <summary>
''' ```SQL
''' UPDATE `rfamseq` SET `rfamseq_acc`='{0}', `accession`='{1}', `version`='{2}', `ncbi_id`='{3}', `mol_type`='{4}', `length`='{5}', `description`='{6}', `previous_acc`='{7}', `source`='{8}' WHERE `rfamseq_acc` = '{9}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, rfamseq_acc, accession, version, ncbi_id, mol_type, length, description, previous_acc, source, rfamseq_acc)
    End Function
#End Region
End Class


End Namespace


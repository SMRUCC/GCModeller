#Region "Microsoft.VisualBasic::ae7296d35bbf704e16aa00db5da5ef29, DataMySql\Xfam\Rfam\Tables\rfamseq.vb"

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

    ' Class rfamseq
    ' 
    '     Properties: accession, description, length, mol_type, ncbi_id
    '                 previous_acc, rfamseq_acc, source, version
    ' 
    '     Function: Clone, GetDeleteSQL, GetDumpInsertValue, (+2 Overloads) GetInsertSQL, (+2 Overloads) GetReplaceSQL
    '               GetUpdateSQL
    ' 
    ' 
    ' /********************************************************************************/

#End Region

REM  Oracle.LinuxCompatibility.MySQL.CodeSolution.VisualBasic.CodeGenerator
REM  MYSQL Schema Mapper
REM      for Microsoft VisualBasic.NET 2.1.0.2569

REM  Dump @2018/5/23 13:13:34


Imports System.Data.Linq.Mapping
Imports System.Xml.Serialization
Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes
Imports MySqlScript = Oracle.LinuxCompatibility.MySQL.Scripting.Extensions

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
) ENGINE=InnoDB DEFAULT CHARSET=latin1;")>
Public Class rfamseq: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
''' <summary>
''' This should be 
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
    <DatabaseField("rfamseq_acc"), PrimaryKey, NotNull, DataType(MySqlDbType.VarChar, "20"), Column(Name:="rfamseq_acc"), XmlAttribute> Public Property rfamseq_acc As String
    <DatabaseField("accession"), NotNull, DataType(MySqlDbType.VarChar, "15"), Column(Name:="accession")> Public Property accession As String
    <DatabaseField("version"), NotNull, DataType(MySqlDbType.Int64, "6"), Column(Name:="version")> Public Property version As Long
    <DatabaseField("ncbi_id"), NotNull, DataType(MySqlDbType.Int64, "10"), Column(Name:="ncbi_id")> Public Property ncbi_id As Long
    <DatabaseField("mol_type"), NotNull, DataType(MySqlDbType.String), Column(Name:="mol_type")> Public Property mol_type As String
    <DatabaseField("length"), DataType(MySqlDbType.Int64, "10"), Column(Name:="length")> Public Property length As Long
    <DatabaseField("description"), NotNull, DataType(MySqlDbType.VarChar, "250"), Column(Name:="description")> Public Property description As String
    <DatabaseField("previous_acc"), DataType(MySqlDbType.Text), Column(Name:="previous_acc")> Public Property previous_acc As String
    <DatabaseField("source"), NotNull, DataType(MySqlDbType.VarChar, "20"), Column(Name:="source")> Public Property source As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `rfamseq` (`rfamseq_acc`, `accession`, `version`, `ncbi_id`, `mol_type`, `length`, `description`, `previous_acc`, `source`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `rfamseq` (`rfamseq_acc`, `accession`, `version`, `ncbi_id`, `mol_type`, `length`, `description`, `previous_acc`, `source`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `rfamseq` (`rfamseq_acc`, `accession`, `version`, `ncbi_id`, `mol_type`, `length`, `description`, `previous_acc`, `source`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `rfamseq` (`rfamseq_acc`, `accession`, `version`, `ncbi_id`, `mol_type`, `length`, `description`, `previous_acc`, `source`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `rfamseq` WHERE `rfamseq_acc` = '{0}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `rfamseq` SET `rfamseq_acc`='{0}', `accession`='{1}', `version`='{2}', `ncbi_id`='{3}', `mol_type`='{4}', `length`='{5}', `description`='{6}', `previous_acc`='{7}', `source`='{8}' WHERE `rfamseq_acc` = '{9}';</SQL>

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
''' ```SQL
''' INSERT INTO `rfamseq` (`rfamseq_acc`, `accession`, `version`, `ncbi_id`, `mol_type`, `length`, `description`, `previous_acc`, `source`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, rfamseq_acc, accession, version, ncbi_id, mol_type, length, description, previous_acc, source)
        Else
        Return String.Format(INSERT_SQL, rfamseq_acc, accession, version, ncbi_id, mol_type, length, description, previous_acc, source)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{rfamseq_acc}', '{accession}', '{version}', '{ncbi_id}', '{mol_type}', '{length}', '{description}', '{previous_acc}', '{source}')"
        Else
            Return $"('{rfamseq_acc}', '{accession}', '{version}', '{ncbi_id}', '{mol_type}', '{length}', '{description}', '{previous_acc}', '{source}')"
        End If
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
''' REPLACE INTO `rfamseq` (`rfamseq_acc`, `accession`, `version`, `ncbi_id`, `mol_type`, `length`, `description`, `previous_acc`, `source`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, rfamseq_acc, accession, version, ncbi_id, mol_type, length, description, previous_acc, source)
        Else
        Return String.Format(REPLACE_SQL, rfamseq_acc, accession, version, ncbi_id, mol_type, length, description, previous_acc, source)
        End If
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

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As rfamseq
                         Return DirectCast(MyClass.MemberwiseClone, rfamseq)
                     End Function
End Class


End Namespace

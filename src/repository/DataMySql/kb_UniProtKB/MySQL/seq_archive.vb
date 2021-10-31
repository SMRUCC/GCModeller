#Region "Microsoft.VisualBasic::857f8c12d8083f542ffd438c991d1a35, DataMySql\kb_UniProtKB\MySQL\seq_archive.vb"

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

    ' Class seq_archive
    ' 
    '     Properties: entry_name, gn, hash_code, length, organism_id
    '                 organism_name, pe, prot_name, sequence, sv
    '                 uniprot_id
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

REM  Dump @2018/5/23 13:13:51


Imports System.Data.Linq.Mapping
Imports System.Xml.Serialization
Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes
Imports MySqlScript = Oracle.LinuxCompatibility.MySQL.Scripting.Extensions

Namespace kb_UniProtKB.mysql

''' <summary>
''' ```SQL
''' 蛋白质序列存储表
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `seq_archive`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `seq_archive` (
'''   `hash_code` int(10) unsigned NOT NULL,
'''   `uniprot_id` varchar(45) NOT NULL COMMENT 'UniqueIdentifier Is the primary accession number of the UniProtKB entry.(对hash_code起校验用)',
'''   `entry_name` varchar(45) DEFAULT NULL COMMENT 'EntryName Is the entry name of the UniProtKB entry.',
'''   `organism_id` int(10) unsigned NOT NULL COMMENT 'OrganismName Is the scientific name of the organism of the UniProtKB entry, this is the id reference to the organism_code table.',
'''   `organism_name` longtext NOT NULL COMMENT '对organism_id校验所使用的',
'''   `gn` varchar(45) DEFAULT NULL COMMENT 'GeneName Is the first gene name of the UniProtKB entry. If there Is no gene name, OrderedLocusName Or ORFname, the GN field Is Not listed.',
'''   `pe` varchar(45) DEFAULT NULL COMMENT 'ProteinExistence Is the numerical value describing the evidence for the existence of the protein.',
'''   `sv` varchar(45) DEFAULT NULL COMMENT 'SequenceVersion Is the version number of the sequence.',
'''   `prot_name` tinytext COMMENT 'ProteinName Is the recommended name of the UniProtKB entry as annotated in the RecName field. For UniProtKB/TrEMBL entries without a RecName field, the SubName field Is used. In case of multiple SubNames, the first one Is used. The ''precursor'' attribute is excluded, ''Fragment'' is included with the name if applicable.',
'''   `length` int(11) DEFAULT NULL COMMENT 'length of the protein sequence',
'''   `sequence` text COMMENT 'protein sequence',
'''   PRIMARY KEY (`hash_code`,`uniprot_id`),
'''   UNIQUE KEY `uniprot_id_UNIQUE` (`uniprot_id`),
'''   UNIQUE KEY `hash_code_UNIQUE` (`hash_code`)
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='蛋白质序列存储表';
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("seq_archive", Database:="kb_uniprotkb", SchemaSQL:="
CREATE TABLE `seq_archive` (
  `hash_code` int(10) unsigned NOT NULL,
  `uniprot_id` varchar(45) NOT NULL COMMENT 'UniqueIdentifier Is the primary accession number of the UniProtKB entry.(对hash_code起校验用)',
  `entry_name` varchar(45) DEFAULT NULL COMMENT 'EntryName Is the entry name of the UniProtKB entry.',
  `organism_id` int(10) unsigned NOT NULL COMMENT 'OrganismName Is the scientific name of the organism of the UniProtKB entry, this is the id reference to the organism_code table.',
  `organism_name` longtext NOT NULL COMMENT '对organism_id校验所使用的',
  `gn` varchar(45) DEFAULT NULL COMMENT 'GeneName Is the first gene name of the UniProtKB entry. If there Is no gene name, OrderedLocusName Or ORFname, the GN field Is Not listed.',
  `pe` varchar(45) DEFAULT NULL COMMENT 'ProteinExistence Is the numerical value describing the evidence for the existence of the protein.',
  `sv` varchar(45) DEFAULT NULL COMMENT 'SequenceVersion Is the version number of the sequence.',
  `prot_name` tinytext COMMENT 'ProteinName Is the recommended name of the UniProtKB entry as annotated in the RecName field. For UniProtKB/TrEMBL entries without a RecName field, the SubName field Is used. In case of multiple SubNames, the first one Is used. The ''precursor'' attribute is excluded, ''Fragment'' is included with the name if applicable.',
  `length` int(11) DEFAULT NULL COMMENT 'length of the protein sequence',
  `sequence` text COMMENT 'protein sequence',
  PRIMARY KEY (`hash_code`,`uniprot_id`),
  UNIQUE KEY `uniprot_id_UNIQUE` (`uniprot_id`),
  UNIQUE KEY `hash_code_UNIQUE` (`hash_code`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='蛋白质序列存储表';")>
Public Class seq_archive: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("hash_code"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "10"), Column(Name:="hash_code"), XmlAttribute> Public Property hash_code As Long
''' <summary>
''' UniqueIdentifier Is the primary accession number of the UniProtKB entry.(对hash_code起校验用)
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
    <DatabaseField("uniprot_id"), PrimaryKey, NotNull, DataType(MySqlDbType.VarChar, "45"), Column(Name:="uniprot_id"), XmlAttribute> Public Property uniprot_id As String
''' <summary>
''' EntryName Is the entry name of the UniProtKB entry.
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
    <DatabaseField("entry_name"), DataType(MySqlDbType.VarChar, "45"), Column(Name:="entry_name")> Public Property entry_name As String
''' <summary>
''' OrganismName Is the scientific name of the organism of the UniProtKB entry, this is the id reference to the organism_code table.
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
    <DatabaseField("organism_id"), NotNull, DataType(MySqlDbType.Int64, "10"), Column(Name:="organism_id")> Public Property organism_id As Long
''' <summary>
''' 对organism_id校验所使用的
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
    <DatabaseField("organism_name"), NotNull, DataType(MySqlDbType.Text), Column(Name:="organism_name")> Public Property organism_name As String
''' <summary>
''' GeneName Is the first gene name of the UniProtKB entry. If there Is no gene name, OrderedLocusName Or ORFname, the GN field Is Not listed.
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
    <DatabaseField("gn"), DataType(MySqlDbType.VarChar, "45"), Column(Name:="gn")> Public Property gn As String
''' <summary>
''' ProteinExistence Is the numerical value describing the evidence for the existence of the protein.
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
    <DatabaseField("pe"), DataType(MySqlDbType.VarChar, "45"), Column(Name:="pe")> Public Property pe As String
''' <summary>
''' SequenceVersion Is the version number of the sequence.
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
    <DatabaseField("sv"), DataType(MySqlDbType.VarChar, "45"), Column(Name:="sv")> Public Property sv As String
''' <summary>
''' ProteinName Is the recommended name of the UniProtKB entry as annotated in the RecName field. For UniProtKB/TrEMBL entries without a RecName field, the SubName field Is used. In case of multiple SubNames, the first one Is used. The ''precursor'' attribute is excluded, ''Fragment'' is included with the name if applicable.
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
    <DatabaseField("prot_name"), DataType(MySqlDbType.Text), Column(Name:="prot_name")> Public Property prot_name As String
''' <summary>
''' length of the protein sequence
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
    <DatabaseField("length"), DataType(MySqlDbType.Int64, "11"), Column(Name:="length")> Public Property length As Long
''' <summary>
''' protein sequence
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
    <DatabaseField("sequence"), DataType(MySqlDbType.Text), Column(Name:="sequence")> Public Property sequence As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `seq_archive` (`hash_code`, `uniprot_id`, `entry_name`, `organism_id`, `organism_name`, `gn`, `pe`, `sv`, `prot_name`, `length`, `sequence`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `seq_archive` (`hash_code`, `uniprot_id`, `entry_name`, `organism_id`, `organism_name`, `gn`, `pe`, `sv`, `prot_name`, `length`, `sequence`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `seq_archive` (`hash_code`, `uniprot_id`, `entry_name`, `organism_id`, `organism_name`, `gn`, `pe`, `sv`, `prot_name`, `length`, `sequence`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `seq_archive` (`hash_code`, `uniprot_id`, `entry_name`, `organism_id`, `organism_name`, `gn`, `pe`, `sv`, `prot_name`, `length`, `sequence`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `seq_archive` WHERE `hash_code`='{0}' and `uniprot_id`='{1}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `seq_archive` SET `hash_code`='{0}', `uniprot_id`='{1}', `entry_name`='{2}', `organism_id`='{3}', `organism_name`='{4}', `gn`='{5}', `pe`='{6}', `sv`='{7}', `prot_name`='{8}', `length`='{9}', `sequence`='{10}' WHERE `hash_code`='{11}' and `uniprot_id`='{12}';</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `seq_archive` WHERE `hash_code`='{0}' and `uniprot_id`='{1}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, hash_code, uniprot_id)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `seq_archive` (`hash_code`, `uniprot_id`, `entry_name`, `organism_id`, `organism_name`, `gn`, `pe`, `sv`, `prot_name`, `length`, `sequence`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, hash_code, uniprot_id, entry_name, organism_id, organism_name, gn, pe, sv, prot_name, length, sequence)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `seq_archive` (`hash_code`, `uniprot_id`, `entry_name`, `organism_id`, `organism_name`, `gn`, `pe`, `sv`, `prot_name`, `length`, `sequence`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, hash_code, uniprot_id, entry_name, organism_id, organism_name, gn, pe, sv, prot_name, length, sequence)
        Else
        Return String.Format(INSERT_SQL, hash_code, uniprot_id, entry_name, organism_id, organism_name, gn, pe, sv, prot_name, length, sequence)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{hash_code}', '{uniprot_id}', '{entry_name}', '{organism_id}', '{organism_name}', '{gn}', '{pe}', '{sv}', '{prot_name}', '{length}', '{sequence}')"
        Else
            Return $"('{hash_code}', '{uniprot_id}', '{entry_name}', '{organism_id}', '{organism_name}', '{gn}', '{pe}', '{sv}', '{prot_name}', '{length}', '{sequence}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `seq_archive` (`hash_code`, `uniprot_id`, `entry_name`, `organism_id`, `organism_name`, `gn`, `pe`, `sv`, `prot_name`, `length`, `sequence`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, hash_code, uniprot_id, entry_name, organism_id, organism_name, gn, pe, sv, prot_name, length, sequence)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `seq_archive` (`hash_code`, `uniprot_id`, `entry_name`, `organism_id`, `organism_name`, `gn`, `pe`, `sv`, `prot_name`, `length`, `sequence`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, hash_code, uniprot_id, entry_name, organism_id, organism_name, gn, pe, sv, prot_name, length, sequence)
        Else
        Return String.Format(REPLACE_SQL, hash_code, uniprot_id, entry_name, organism_id, organism_name, gn, pe, sv, prot_name, length, sequence)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `seq_archive` SET `hash_code`='{0}', `uniprot_id`='{1}', `entry_name`='{2}', `organism_id`='{3}', `organism_name`='{4}', `gn`='{5}', `pe`='{6}', `sv`='{7}', `prot_name`='{8}', `length`='{9}', `sequence`='{10}' WHERE `hash_code`='{11}' and `uniprot_id`='{12}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, hash_code, uniprot_id, entry_name, organism_id, organism_name, gn, pe, sv, prot_name, length, sequence, hash_code, uniprot_id)
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As seq_archive
                         Return DirectCast(MyClass.MemberwiseClone, seq_archive)
                     End Function
End Class


End Namespace

#Region "Microsoft.VisualBasic::215d72a9e3842ef105296da6b23edf17, ..\GCModeller\analysis\annoTools\DataMySql\UniprotSprot\uniprotKB\uniprotkb.vb"

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

REM  Dump @12/3/2015 8:59:58 PM


Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes

Namespace UniprotKB.MySQL.Tables

''' <summary>
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `uniprotkb`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `uniprotkb` (
'''   `uniprot_id` varchar(45) NOT NULL COMMENT 'UniqueIdentifier Is the primary accession number of the UniProtKB entry.',
'''   `entry_name` varchar(45) DEFAULT NULL COMMENT 'EntryName Is the entry name of the UniProtKB entry.',
'''   `orgnsm_sp_name` tinytext COMMENT 'OrganismName Is the scientific name of the organism of the UniProtKB entry.',
'''   `gn` varchar(45) DEFAULT NULL COMMENT 'GeneName Is the first gene name of the UniProtKB entry. If there Is no gene name, OrderedLocusName Or ORFname, the GN field Is Not listed.',
'''   `pe` varchar(45) DEFAULT NULL COMMENT 'ProteinExistence Is the numerical value describing the evidence for the existence of the protein.',
'''   `sv` varchar(45) DEFAULT NULL COMMENT 'SequenceVersion Is the version number of the sequence.',
'''   `prot_name` tinytext COMMENT 'ProteinName Is the recommended name of the UniProtKB entry as annotated in the RecName field. For UniProtKB/TrEMBL entries without a RecName field, the SubName field Is used. In case of multiple SubNames, the first one Is used. The ''precursor'' attribute is excluded, ''Fragment'' is included with the name if applicable.',
'''   `length` int(11) DEFAULT NULL COMMENT 'length of the protein sequence',
'''   `sequence` text COMMENT 'protein sequence',
'''   PRIMARY KEY (`uniprot_id`),
'''   UNIQUE KEY `uniprot_id_UNIQUE` (`uniprot_id`)
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' /*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;
''' 
''' /*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
''' /*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
''' /*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
''' /*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
''' /*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
''' /*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
''' /*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;
''' 
''' -- Dump completed on 2015-12-03 20:59:22
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("uniprotkb", Database:="uniprot")>
Public Class uniprotkb: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
''' <summary>
''' UniqueIdentifier Is the primary accession number of the UniProtKB entry.
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
    <DatabaseField("uniprot_id"), PrimaryKey, NotNull, DataType(MySqlDbType.VarChar, "45")> Public Property uniprot_id As String
''' <summary>
''' EntryName Is the entry name of the UniProtKB entry.
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
    <DatabaseField("entry_name"), DataType(MySqlDbType.VarChar, "45")> Public Property entry_name As String
''' <summary>
''' OrganismName Is the scientific name of the organism of the UniProtKB entry.
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
    <DatabaseField("orgnsm_sp_name"), DataType(MySqlDbType.Text)> Public Property orgnsm_sp_name As String
''' <summary>
''' GeneName Is the first gene name of the UniProtKB entry. If there Is no gene name, OrderedLocusName Or ORFname, the GN field Is Not listed.
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
    <DatabaseField("gn"), DataType(MySqlDbType.VarChar, "45")> Public Property gn As String
''' <summary>
''' ProteinExistence Is the numerical value describing the evidence for the existence of the protein.
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
    <DatabaseField("pe"), DataType(MySqlDbType.VarChar, "45")> Public Property pe As String
''' <summary>
''' SequenceVersion Is the version number of the sequence.
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
    <DatabaseField("sv"), DataType(MySqlDbType.VarChar, "45")> Public Property sv As String
''' <summary>
''' ProteinName Is the recommended name of the UniProtKB entry as annotated in the RecName field. For UniProtKB/TrEMBL entries without a RecName field, the SubName field Is used. In case of multiple SubNames, the first one Is used. The ''precursor'' attribute is excluded, ''Fragment'' is included with the name if applicable.
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
    <DatabaseField("prot_name"), DataType(MySqlDbType.Text)> Public Property prot_name As String
''' <summary>
''' length of the protein sequence
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
    <DatabaseField("length"), DataType(MySqlDbType.Int64, "11")> Public Property length As Long
''' <summary>
''' protein sequence
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
    <DatabaseField("sequence"), DataType(MySqlDbType.Text)> Public Property sequence As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `uniprotkb` (`uniprot_id`, `entry_name`, `orgnsm_sp_name`, `gn`, `pe`, `sv`, `prot_name`, `length`, `sequence`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `uniprotkb` (`uniprot_id`, `entry_name`, `orgnsm_sp_name`, `gn`, `pe`, `sv`, `prot_name`, `length`, `sequence`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `uniprotkb` WHERE `uniprot_id` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `uniprotkb` SET `uniprot_id`='{0}', `entry_name`='{1}', `orgnsm_sp_name`='{2}', `gn`='{3}', `pe`='{4}', `sv`='{5}', `prot_name`='{6}', `length`='{7}', `sequence`='{8}' WHERE `uniprot_id` = '{9}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, uniprot_id)
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, uniprot_id, entry_name, orgnsm_sp_name, gn, pe, sv, prot_name, length, sequence)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, uniprot_id, entry_name, orgnsm_sp_name, gn, pe, sv, prot_name, length, sequence)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, uniprot_id, entry_name, orgnsm_sp_name, gn, pe, sv, prot_name, length, sequence, uniprot_id)
    End Function
#End Region
End Class


End Namespace

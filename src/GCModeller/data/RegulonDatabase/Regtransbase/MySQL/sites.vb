#Region "Microsoft.VisualBasic::c5504a082561d6c778a66156b924ec36, GCModeller\data\RegulonDatabase\Regtransbase\MySQL\sites.vb"

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

    '   Total Lines: 238
    '    Code Lines: 128
    ' Comment Lines: 88
    '   Blank Lines: 22
    '     File Size: 19.32 KB


    ' Class sites
    ' 
    '     Properties: art_guid, descript, fl_dna_rna, fl_real_name, func_site_type_guid
    '                 genome_guid, last_update, name, pfo_side_guid, pfo_type_id
    '                 pkg_guid, pos_from, pos_from_guid, pos_to, pos_to_guid
    '                 pto_side_guid, pto_type_id, regulator_guid, sequence, signature
    '                 site_guid, site_len, struct_site_type_guid
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

REM  Dump @2018/5/23 13:13:38


Imports System.Data.Linq.Mapping
Imports System.Xml.Serialization
Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes
Imports MySqlScript = Oracle.LinuxCompatibility.MySQL.Scripting.Extensions

Namespace Regtransbase.MySQL

''' <summary>
''' ```SQL
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `sites`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `sites` (
'''   `site_guid` int(11) NOT NULL DEFAULT '0',
'''   `pkg_guid` int(11) NOT NULL DEFAULT '0',
'''   `art_guid` int(11) NOT NULL DEFAULT '0',
'''   `name` varchar(50) DEFAULT NULL,
'''   `fl_real_name` int(1) DEFAULT NULL,
'''   `genome_guid` int(11) DEFAULT NULL,
'''   `func_site_type_guid` int(11) DEFAULT NULL,
'''   `struct_site_type_guid` int(11) DEFAULT NULL,
'''   `regulator_guid` int(11) DEFAULT '0',
'''   `fl_dna_rna` int(1) DEFAULT NULL,
'''   `pos_from` int(11) DEFAULT NULL,
'''   `pos_from_guid` int(11) DEFAULT NULL,
'''   `pfo_type_id` int(11) DEFAULT NULL,
'''   `pfo_side_guid` int(11) DEFAULT NULL,
'''   `pos_to` int(11) DEFAULT NULL,
'''   `pos_to_guid` int(11) DEFAULT NULL,
'''   `pto_type_id` int(11) DEFAULT NULL,
'''   `pto_side_guid` int(11) DEFAULT NULL,
'''   `site_len` int(11) DEFAULT NULL,
'''   `sequence` text,
'''   `signature` varchar(255) DEFAULT NULL,
'''   `descript` blob,
'''   `last_update` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
'''   PRIMARY KEY (`site_guid`),
'''   KEY `FK_sites-pkg_guid` (`pkg_guid`),
'''   KEY `FK_sites-art_guid` (`art_guid`),
'''   KEY `FK_sites-genome_guid` (`genome_guid`),
'''   KEY `FK_sites-func_site_type_guid` (`func_site_type_guid`),
'''   KEY `FK_sites-struct_site_type_guid` (`struct_site_type_guid`),
'''   KEY `FK_sites-regulator_guid` (`regulator_guid`),
'''   CONSTRAINT `FK_sites-art_guid` FOREIGN KEY (`art_guid`) REFERENCES `articles` (`art_guid`),
'''   CONSTRAINT `FK_sites-func_site_type_guid` FOREIGN KEY (`func_site_type_guid`) REFERENCES `dict_func_site_types` (`func_site_type_guid`),
'''   CONSTRAINT `FK_sites-genome_guid` FOREIGN KEY (`genome_guid`) REFERENCES `dict_genomes` (`genome_guid`),
'''   CONSTRAINT `FK_sites-pkg_guid` FOREIGN KEY (`pkg_guid`) REFERENCES `packages` (`pkg_guid`),
'''   CONSTRAINT `FK_sites-struct_site_type_guid` FOREIGN KEY (`struct_site_type_guid`) REFERENCES `dict_struct_site_types` (`struct_site_type_guid`),
'''   CONSTRAINT `sites_ibfk_1` FOREIGN KEY (`regulator_guid`) REFERENCES `obj_name_genomes` (`obj_guid`)
''' ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("sites", Database:="dbregulation_update", SchemaSQL:="
CREATE TABLE `sites` (
  `site_guid` int(11) NOT NULL DEFAULT '0',
  `pkg_guid` int(11) NOT NULL DEFAULT '0',
  `art_guid` int(11) NOT NULL DEFAULT '0',
  `name` varchar(50) DEFAULT NULL,
  `fl_real_name` int(1) DEFAULT NULL,
  `genome_guid` int(11) DEFAULT NULL,
  `func_site_type_guid` int(11) DEFAULT NULL,
  `struct_site_type_guid` int(11) DEFAULT NULL,
  `regulator_guid` int(11) DEFAULT '0',
  `fl_dna_rna` int(1) DEFAULT NULL,
  `pos_from` int(11) DEFAULT NULL,
  `pos_from_guid` int(11) DEFAULT NULL,
  `pfo_type_id` int(11) DEFAULT NULL,
  `pfo_side_guid` int(11) DEFAULT NULL,
  `pos_to` int(11) DEFAULT NULL,
  `pos_to_guid` int(11) DEFAULT NULL,
  `pto_type_id` int(11) DEFAULT NULL,
  `pto_side_guid` int(11) DEFAULT NULL,
  `site_len` int(11) DEFAULT NULL,
  `sequence` text,
  `signature` varchar(255) DEFAULT NULL,
  `descript` blob,
  `last_update` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`site_guid`),
  KEY `FK_sites-pkg_guid` (`pkg_guid`),
  KEY `FK_sites-art_guid` (`art_guid`),
  KEY `FK_sites-genome_guid` (`genome_guid`),
  KEY `FK_sites-func_site_type_guid` (`func_site_type_guid`),
  KEY `FK_sites-struct_site_type_guid` (`struct_site_type_guid`),
  KEY `FK_sites-regulator_guid` (`regulator_guid`),
  CONSTRAINT `FK_sites-art_guid` FOREIGN KEY (`art_guid`) REFERENCES `articles` (`art_guid`),
  CONSTRAINT `FK_sites-func_site_type_guid` FOREIGN KEY (`func_site_type_guid`) REFERENCES `dict_func_site_types` (`func_site_type_guid`),
  CONSTRAINT `FK_sites-genome_guid` FOREIGN KEY (`genome_guid`) REFERENCES `dict_genomes` (`genome_guid`),
  CONSTRAINT `FK_sites-pkg_guid` FOREIGN KEY (`pkg_guid`) REFERENCES `packages` (`pkg_guid`),
  CONSTRAINT `FK_sites-struct_site_type_guid` FOREIGN KEY (`struct_site_type_guid`) REFERENCES `dict_struct_site_types` (`struct_site_type_guid`),
  CONSTRAINT `sites_ibfk_1` FOREIGN KEY (`regulator_guid`) REFERENCES `obj_name_genomes` (`obj_guid`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;")>
Public Class sites: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("site_guid"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "11"), Column(Name:="site_guid"), XmlAttribute> Public Property site_guid As Long
    <DatabaseField("pkg_guid"), NotNull, DataType(MySqlDbType.Int64, "11"), Column(Name:="pkg_guid")> Public Property pkg_guid As Long
    <DatabaseField("art_guid"), NotNull, DataType(MySqlDbType.Int64, "11"), Column(Name:="art_guid")> Public Property art_guid As Long
    <DatabaseField("name"), DataType(MySqlDbType.VarChar, "50"), Column(Name:="name")> Public Property name As String
    <DatabaseField("fl_real_name"), DataType(MySqlDbType.Int64, "1"), Column(Name:="fl_real_name")> Public Property fl_real_name As Long
    <DatabaseField("genome_guid"), DataType(MySqlDbType.Int64, "11"), Column(Name:="genome_guid")> Public Property genome_guid As Long
    <DatabaseField("func_site_type_guid"), DataType(MySqlDbType.Int64, "11"), Column(Name:="func_site_type_guid")> Public Property func_site_type_guid As Long
    <DatabaseField("struct_site_type_guid"), DataType(MySqlDbType.Int64, "11"), Column(Name:="struct_site_type_guid")> Public Property struct_site_type_guid As Long
    <DatabaseField("regulator_guid"), DataType(MySqlDbType.Int64, "11"), Column(Name:="regulator_guid")> Public Property regulator_guid As Long
    <DatabaseField("fl_dna_rna"), DataType(MySqlDbType.Int64, "1"), Column(Name:="fl_dna_rna")> Public Property fl_dna_rna As Long
    <DatabaseField("pos_from"), DataType(MySqlDbType.Int64, "11"), Column(Name:="pos_from")> Public Property pos_from As Long
    <DatabaseField("pos_from_guid"), DataType(MySqlDbType.Int64, "11"), Column(Name:="pos_from_guid")> Public Property pos_from_guid As Long
    <DatabaseField("pfo_type_id"), DataType(MySqlDbType.Int64, "11"), Column(Name:="pfo_type_id")> Public Property pfo_type_id As Long
    <DatabaseField("pfo_side_guid"), DataType(MySqlDbType.Int64, "11"), Column(Name:="pfo_side_guid")> Public Property pfo_side_guid As Long
    <DatabaseField("pos_to"), DataType(MySqlDbType.Int64, "11"), Column(Name:="pos_to")> Public Property pos_to As Long
    <DatabaseField("pos_to_guid"), DataType(MySqlDbType.Int64, "11"), Column(Name:="pos_to_guid")> Public Property pos_to_guid As Long
    <DatabaseField("pto_type_id"), DataType(MySqlDbType.Int64, "11"), Column(Name:="pto_type_id")> Public Property pto_type_id As Long
    <DatabaseField("pto_side_guid"), DataType(MySqlDbType.Int64, "11"), Column(Name:="pto_side_guid")> Public Property pto_side_guid As Long
    <DatabaseField("site_len"), DataType(MySqlDbType.Int64, "11"), Column(Name:="site_len")> Public Property site_len As Long
    <DatabaseField("sequence"), DataType(MySqlDbType.Text), Column(Name:="sequence")> Public Property sequence As String
    <DatabaseField("signature"), DataType(MySqlDbType.VarChar, "255"), Column(Name:="signature")> Public Property signature As String
    <DatabaseField("descript"), DataType(MySqlDbType.Blob), Column(Name:="descript")> Public Property descript As Byte()
    <DatabaseField("last_update"), NotNull, DataType(MySqlDbType.DateTime), Column(Name:="last_update")> Public Property last_update As Date
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `sites` (`site_guid`, `pkg_guid`, `art_guid`, `name`, `fl_real_name`, `genome_guid`, `func_site_type_guid`, `struct_site_type_guid`, `regulator_guid`, `fl_dna_rna`, `pos_from`, `pos_from_guid`, `pfo_type_id`, `pfo_side_guid`, `pos_to`, `pos_to_guid`, `pto_type_id`, `pto_side_guid`, `site_len`, `sequence`, `signature`, `descript`, `last_update`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}', '{16}', '{17}', '{18}', '{19}', '{20}', '{21}', '{22}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `sites` (`site_guid`, `pkg_guid`, `art_guid`, `name`, `fl_real_name`, `genome_guid`, `func_site_type_guid`, `struct_site_type_guid`, `regulator_guid`, `fl_dna_rna`, `pos_from`, `pos_from_guid`, `pfo_type_id`, `pfo_side_guid`, `pos_to`, `pos_to_guid`, `pto_type_id`, `pto_side_guid`, `site_len`, `sequence`, `signature`, `descript`, `last_update`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}', '{16}', '{17}', '{18}', '{19}', '{20}', '{21}', '{22}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `sites` (`site_guid`, `pkg_guid`, `art_guid`, `name`, `fl_real_name`, `genome_guid`, `func_site_type_guid`, `struct_site_type_guid`, `regulator_guid`, `fl_dna_rna`, `pos_from`, `pos_from_guid`, `pfo_type_id`, `pfo_side_guid`, `pos_to`, `pos_to_guid`, `pto_type_id`, `pto_side_guid`, `site_len`, `sequence`, `signature`, `descript`, `last_update`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}', '{16}', '{17}', '{18}', '{19}', '{20}', '{21}', '{22}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `sites` (`site_guid`, `pkg_guid`, `art_guid`, `name`, `fl_real_name`, `genome_guid`, `func_site_type_guid`, `struct_site_type_guid`, `regulator_guid`, `fl_dna_rna`, `pos_from`, `pos_from_guid`, `pfo_type_id`, `pfo_side_guid`, `pos_to`, `pos_to_guid`, `pto_type_id`, `pto_side_guid`, `site_len`, `sequence`, `signature`, `descript`, `last_update`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}', '{16}', '{17}', '{18}', '{19}', '{20}', '{21}', '{22}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `sites` WHERE `site_guid` = '{0}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `sites` SET `site_guid`='{0}', `pkg_guid`='{1}', `art_guid`='{2}', `name`='{3}', `fl_real_name`='{4}', `genome_guid`='{5}', `func_site_type_guid`='{6}', `struct_site_type_guid`='{7}', `regulator_guid`='{8}', `fl_dna_rna`='{9}', `pos_from`='{10}', `pos_from_guid`='{11}', `pfo_type_id`='{12}', `pfo_side_guid`='{13}', `pos_to`='{14}', `pos_to_guid`='{15}', `pto_type_id`='{16}', `pto_side_guid`='{17}', `site_len`='{18}', `sequence`='{19}', `signature`='{20}', `descript`='{21}', `last_update`='{22}' WHERE `site_guid` = '{23}';</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `sites` WHERE `site_guid` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, site_guid)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `sites` (`site_guid`, `pkg_guid`, `art_guid`, `name`, `fl_real_name`, `genome_guid`, `func_site_type_guid`, `struct_site_type_guid`, `regulator_guid`, `fl_dna_rna`, `pos_from`, `pos_from_guid`, `pfo_type_id`, `pfo_side_guid`, `pos_to`, `pos_to_guid`, `pto_type_id`, `pto_side_guid`, `site_len`, `sequence`, `signature`, `descript`, `last_update`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}', '{16}', '{17}', '{18}', '{19}', '{20}', '{21}', '{22}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, site_guid, pkg_guid, art_guid, name, fl_real_name, genome_guid, func_site_type_guid, struct_site_type_guid, regulator_guid, fl_dna_rna, pos_from, pos_from_guid, pfo_type_id, pfo_side_guid, pos_to, pos_to_guid, pto_type_id, pto_side_guid, site_len, sequence, signature, descript, MySqlScript.ToMySqlDateTimeString(last_update))
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `sites` (`site_guid`, `pkg_guid`, `art_guid`, `name`, `fl_real_name`, `genome_guid`, `func_site_type_guid`, `struct_site_type_guid`, `regulator_guid`, `fl_dna_rna`, `pos_from`, `pos_from_guid`, `pfo_type_id`, `pfo_side_guid`, `pos_to`, `pos_to_guid`, `pto_type_id`, `pto_side_guid`, `site_len`, `sequence`, `signature`, `descript`, `last_update`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}', '{16}', '{17}', '{18}', '{19}', '{20}', '{21}', '{22}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, site_guid, pkg_guid, art_guid, name, fl_real_name, genome_guid, func_site_type_guid, struct_site_type_guid, regulator_guid, fl_dna_rna, pos_from, pos_from_guid, pfo_type_id, pfo_side_guid, pos_to, pos_to_guid, pto_type_id, pto_side_guid, site_len, sequence, signature, descript, MySqlScript.ToMySqlDateTimeString(last_update))
        Else
        Return String.Format(INSERT_SQL, site_guid, pkg_guid, art_guid, name, fl_real_name, genome_guid, func_site_type_guid, struct_site_type_guid, regulator_guid, fl_dna_rna, pos_from, pos_from_guid, pfo_type_id, pfo_side_guid, pos_to, pos_to_guid, pto_type_id, pto_side_guid, site_len, sequence, signature, descript, MySqlScript.ToMySqlDateTimeString(last_update))
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{site_guid}', '{pkg_guid}', '{art_guid}', '{name}', '{fl_real_name}', '{genome_guid}', '{func_site_type_guid}', '{struct_site_type_guid}', '{regulator_guid}', '{fl_dna_rna}', '{pos_from}', '{pos_from_guid}', '{pfo_type_id}', '{pfo_side_guid}', '{pos_to}', '{pos_to_guid}', '{pto_type_id}', '{pto_side_guid}', '{site_len}', '{sequence}', '{signature}', '{descript}', '{last_update}')"
        Else
            Return $"('{site_guid}', '{pkg_guid}', '{art_guid}', '{name}', '{fl_real_name}', '{genome_guid}', '{func_site_type_guid}', '{struct_site_type_guid}', '{regulator_guid}', '{fl_dna_rna}', '{pos_from}', '{pos_from_guid}', '{pfo_type_id}', '{pfo_side_guid}', '{pos_to}', '{pos_to_guid}', '{pto_type_id}', '{pto_side_guid}', '{site_len}', '{sequence}', '{signature}', '{descript}', '{last_update}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `sites` (`site_guid`, `pkg_guid`, `art_guid`, `name`, `fl_real_name`, `genome_guid`, `func_site_type_guid`, `struct_site_type_guid`, `regulator_guid`, `fl_dna_rna`, `pos_from`, `pos_from_guid`, `pfo_type_id`, `pfo_side_guid`, `pos_to`, `pos_to_guid`, `pto_type_id`, `pto_side_guid`, `site_len`, `sequence`, `signature`, `descript`, `last_update`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}', '{16}', '{17}', '{18}', '{19}', '{20}', '{21}', '{22}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, site_guid, pkg_guid, art_guid, name, fl_real_name, genome_guid, func_site_type_guid, struct_site_type_guid, regulator_guid, fl_dna_rna, pos_from, pos_from_guid, pfo_type_id, pfo_side_guid, pos_to, pos_to_guid, pto_type_id, pto_side_guid, site_len, sequence, signature, descript, MySqlScript.ToMySqlDateTimeString(last_update))
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `sites` (`site_guid`, `pkg_guid`, `art_guid`, `name`, `fl_real_name`, `genome_guid`, `func_site_type_guid`, `struct_site_type_guid`, `regulator_guid`, `fl_dna_rna`, `pos_from`, `pos_from_guid`, `pfo_type_id`, `pfo_side_guid`, `pos_to`, `pos_to_guid`, `pto_type_id`, `pto_side_guid`, `site_len`, `sequence`, `signature`, `descript`, `last_update`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}', '{16}', '{17}', '{18}', '{19}', '{20}', '{21}', '{22}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, site_guid, pkg_guid, art_guid, name, fl_real_name, genome_guid, func_site_type_guid, struct_site_type_guid, regulator_guid, fl_dna_rna, pos_from, pos_from_guid, pfo_type_id, pfo_side_guid, pos_to, pos_to_guid, pto_type_id, pto_side_guid, site_len, sequence, signature, descript, MySqlScript.ToMySqlDateTimeString(last_update))
        Else
        Return String.Format(REPLACE_SQL, site_guid, pkg_guid, art_guid, name, fl_real_name, genome_guid, func_site_type_guid, struct_site_type_guid, regulator_guid, fl_dna_rna, pos_from, pos_from_guid, pfo_type_id, pfo_side_guid, pos_to, pos_to_guid, pto_type_id, pto_side_guid, site_len, sequence, signature, descript, MySqlScript.ToMySqlDateTimeString(last_update))
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `sites` SET `site_guid`='{0}', `pkg_guid`='{1}', `art_guid`='{2}', `name`='{3}', `fl_real_name`='{4}', `genome_guid`='{5}', `func_site_type_guid`='{6}', `struct_site_type_guid`='{7}', `regulator_guid`='{8}', `fl_dna_rna`='{9}', `pos_from`='{10}', `pos_from_guid`='{11}', `pfo_type_id`='{12}', `pfo_side_guid`='{13}', `pos_to`='{14}', `pos_to_guid`='{15}', `pto_type_id`='{16}', `pto_side_guid`='{17}', `site_len`='{18}', `sequence`='{19}', `signature`='{20}', `descript`='{21}', `last_update`='{22}' WHERE `site_guid` = '{23}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, site_guid, pkg_guid, art_guid, name, fl_real_name, genome_guid, func_site_type_guid, struct_site_type_guid, regulator_guid, fl_dna_rna, pos_from, pos_from_guid, pfo_type_id, pfo_side_guid, pos_to, pos_to_guid, pto_type_id, pto_side_guid, site_len, sequence, signature, descript, MySqlScript.ToMySqlDateTimeString(last_update), site_guid)
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As sites
                         Return DirectCast(MyClass.MemberwiseClone, sites)
                     End Function
End Class


End Namespace

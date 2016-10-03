#Region "Microsoft.VisualBasic::aec5f645d0fc3ceb3e621467fc1d0b14, ..\GCModeller\data\RegulonDatabase\Regtransbase\MySQL\regulators.vb"

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
''' DROP TABLE IF EXISTS `regulators`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `regulators` (
'''   `regulator_guid` int(11) NOT NULL DEFAULT '0',
'''   `pkg_guid` int(11) NOT NULL DEFAULT '0',
'''   `art_guid` int(11) NOT NULL DEFAULT '0',
'''   `name` varchar(50) DEFAULT NULL,
'''   `fl_real_name` int(1) DEFAULT NULL,
'''   `genome_guid` int(11) DEFAULT NULL,
'''   `fl_prot_rna` int(1) DEFAULT NULL,
'''   `regulator_type_guid` int(11) DEFAULT '0',
'''   `gene_guid` int(11) DEFAULT NULL,
'''   `ref_bank1` varchar(255) DEFAULT NULL,
'''   `ref_bank2` varchar(255) DEFAULT NULL,
'''   `ref_bank3` varchar(255) DEFAULT NULL,
'''   `ref_bank4` varchar(255) DEFAULT NULL,
'''   `consensus` varchar(50) DEFAULT NULL,
'''   `family` varchar(20) DEFAULT NULL,
'''   `descript` blob,
'''   `last_update` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
'''   PRIMARY KEY (`regulator_guid`),
'''   KEY `FK_regulators-pkg_guid` (`pkg_guid`),
'''   KEY `FK_regulators-art_guid` (`art_guid`),
'''   KEY `FK_regulators-genome_guid` (`genome_guid`),
'''   KEY `FK_regulators-regulator_type_guid` (`regulator_type_guid`),
'''   KEY `FK_regulators-gene_guid` (`gene_guid`),
'''   CONSTRAINT `FK_regulators-art_guid` FOREIGN KEY (`art_guid`) REFERENCES `articles` (`art_guid`),
'''   CONSTRAINT `FK_regulators-genome_guid` FOREIGN KEY (`genome_guid`) REFERENCES `dict_genomes` (`genome_guid`),
'''   CONSTRAINT `FK_regulators-pkg_guid` FOREIGN KEY (`pkg_guid`) REFERENCES `packages` (`pkg_guid`),
'''   CONSTRAINT `FK_regulators-regulator_type_guid` FOREIGN KEY (`regulator_type_guid`) REFERENCES `dict_regulator_types` (`regulator_type_guid`),
'''   CONSTRAINT `regulators_ibfk_1` FOREIGN KEY (`gene_guid`) REFERENCES `obj_name_genomes` (`obj_guid`)
''' ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("regulators", Database:="dbregulation_update")>
Public Class regulators: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("regulator_guid"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "11")> Public Property regulator_guid As Long
    <DatabaseField("pkg_guid"), NotNull, DataType(MySqlDbType.Int64, "11")> Public Property pkg_guid As Long
    <DatabaseField("art_guid"), NotNull, DataType(MySqlDbType.Int64, "11")> Public Property art_guid As Long
    <DatabaseField("name"), DataType(MySqlDbType.VarChar, "50")> Public Property name As String
    <DatabaseField("fl_real_name"), DataType(MySqlDbType.Int64, "1")> Public Property fl_real_name As Long
    <DatabaseField("genome_guid"), DataType(MySqlDbType.Int64, "11")> Public Property genome_guid As Long
    <DatabaseField("fl_prot_rna"), DataType(MySqlDbType.Int64, "1")> Public Property fl_prot_rna As Long
    <DatabaseField("regulator_type_guid"), DataType(MySqlDbType.Int64, "11")> Public Property regulator_type_guid As Long
    <DatabaseField("gene_guid"), DataType(MySqlDbType.Int64, "11")> Public Property gene_guid As Long
    <DatabaseField("ref_bank1"), DataType(MySqlDbType.VarChar, "255")> Public Property ref_bank1 As String
    <DatabaseField("ref_bank2"), DataType(MySqlDbType.VarChar, "255")> Public Property ref_bank2 As String
    <DatabaseField("ref_bank3"), DataType(MySqlDbType.VarChar, "255")> Public Property ref_bank3 As String
    <DatabaseField("ref_bank4"), DataType(MySqlDbType.VarChar, "255")> Public Property ref_bank4 As String
    <DatabaseField("consensus"), DataType(MySqlDbType.VarChar, "50")> Public Property consensus As String
    <DatabaseField("family"), DataType(MySqlDbType.VarChar, "20")> Public Property family As String
    <DatabaseField("descript"), DataType(MySqlDbType.Blob)> Public Property descript As Byte()
    <DatabaseField("last_update"), NotNull, DataType(MySqlDbType.DateTime)> Public Property last_update As Date
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `regulators` (`regulator_guid`, `pkg_guid`, `art_guid`, `name`, `fl_real_name`, `genome_guid`, `fl_prot_rna`, `regulator_type_guid`, `gene_guid`, `ref_bank1`, `ref_bank2`, `ref_bank3`, `ref_bank4`, `consensus`, `family`, `descript`, `last_update`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}', '{16}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `regulators` (`regulator_guid`, `pkg_guid`, `art_guid`, `name`, `fl_real_name`, `genome_guid`, `fl_prot_rna`, `regulator_type_guid`, `gene_guid`, `ref_bank1`, `ref_bank2`, `ref_bank3`, `ref_bank4`, `consensus`, `family`, `descript`, `last_update`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}', '{16}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `regulators` WHERE `regulator_guid` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `regulators` SET `regulator_guid`='{0}', `pkg_guid`='{1}', `art_guid`='{2}', `name`='{3}', `fl_real_name`='{4}', `genome_guid`='{5}', `fl_prot_rna`='{6}', `regulator_type_guid`='{7}', `gene_guid`='{8}', `ref_bank1`='{9}', `ref_bank2`='{10}', `ref_bank3`='{11}', `ref_bank4`='{12}', `consensus`='{13}', `family`='{14}', `descript`='{15}', `last_update`='{16}' WHERE `regulator_guid` = '{17}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, regulator_guid)
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, regulator_guid, pkg_guid, art_guid, name, fl_real_name, genome_guid, fl_prot_rna, regulator_type_guid, gene_guid, ref_bank1, ref_bank2, ref_bank3, ref_bank4, consensus, family, descript, DataType.ToMySqlDateTimeString(last_update))
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, regulator_guid, pkg_guid, art_guid, name, fl_real_name, genome_guid, fl_prot_rna, regulator_type_guid, gene_guid, ref_bank1, ref_bank2, ref_bank3, ref_bank4, consensus, family, descript, DataType.ToMySqlDateTimeString(last_update))
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, regulator_guid, pkg_guid, art_guid, name, fl_real_name, genome_guid, fl_prot_rna, regulator_type_guid, gene_guid, ref_bank1, ref_bank2, ref_bank3, ref_bank4, consensus, family, descript, DataType.ToMySqlDateTimeString(last_update), regulator_guid)
    End Function
#End Region
End Class


End Namespace

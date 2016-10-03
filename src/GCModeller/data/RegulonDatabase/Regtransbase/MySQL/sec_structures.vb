#Region "Microsoft.VisualBasic::48a19beb35ddd5aa048b88506dac1723, ..\GCModeller\data\RegulonDatabase\Regtransbase\MySQL\sec_structures.vb"

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
''' DROP TABLE IF EXISTS `sec_structures`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `sec_structures` (
'''   `sec_struct_guid` int(11) NOT NULL DEFAULT '0',
'''   `pkg_guid` int(11) NOT NULL DEFAULT '0',
'''   `art_guid` int(11) NOT NULL DEFAULT '0',
'''   `name` varchar(50) DEFAULT NULL,
'''   `fl_real_name` int(1) DEFAULT NULL,
'''   `genome_guid` int(11) DEFAULT NULL,
'''   `pos_from` int(11) DEFAULT NULL,
'''   `pos_from_guid` int(11) DEFAULT NULL,
'''   `pfo_type_id` int(11) DEFAULT NULL,
'''   `pfo_side_guid` int(11) DEFAULT NULL,
'''   `pos_to` int(11) DEFAULT NULL,
'''   `pos_to_guid` int(11) DEFAULT NULL,
'''   `pto_type_id` int(11) DEFAULT NULL,
'''   `pto_side_guid` int(11) DEFAULT NULL,
'''   `sequence` varchar(255) DEFAULT NULL,
'''   `descript` blob,
'''   PRIMARY KEY (`sec_struct_guid`),
'''   KEY `FK_sec_structures-pkg_guid` (`pkg_guid`),
'''   KEY `FK_sec_structures-art_guid` (`art_guid`),
'''   KEY `FK_sec_structures-genome_guid` (`genome_guid`),
'''   CONSTRAINT `FK_sec_structures-art_guid` FOREIGN KEY (`art_guid`) REFERENCES `articles` (`art_guid`),
'''   CONSTRAINT `FK_sec_structures-genome_guid` FOREIGN KEY (`genome_guid`) REFERENCES `dict_genomes` (`genome_guid`),
'''   CONSTRAINT `FK_sec_structures-pkg_guid` FOREIGN KEY (`pkg_guid`) REFERENCES `packages` (`pkg_guid`)
''' ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("sec_structures", Database:="dbregulation_update")>
Public Class sec_structures: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("sec_struct_guid"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "11")> Public Property sec_struct_guid As Long
    <DatabaseField("pkg_guid"), NotNull, DataType(MySqlDbType.Int64, "11")> Public Property pkg_guid As Long
    <DatabaseField("art_guid"), NotNull, DataType(MySqlDbType.Int64, "11")> Public Property art_guid As Long
    <DatabaseField("name"), DataType(MySqlDbType.VarChar, "50")> Public Property name As String
    <DatabaseField("fl_real_name"), DataType(MySqlDbType.Int64, "1")> Public Property fl_real_name As Long
    <DatabaseField("genome_guid"), DataType(MySqlDbType.Int64, "11")> Public Property genome_guid As Long
    <DatabaseField("pos_from"), DataType(MySqlDbType.Int64, "11")> Public Property pos_from As Long
    <DatabaseField("pos_from_guid"), DataType(MySqlDbType.Int64, "11")> Public Property pos_from_guid As Long
    <DatabaseField("pfo_type_id"), DataType(MySqlDbType.Int64, "11")> Public Property pfo_type_id As Long
    <DatabaseField("pfo_side_guid"), DataType(MySqlDbType.Int64, "11")> Public Property pfo_side_guid As Long
    <DatabaseField("pos_to"), DataType(MySqlDbType.Int64, "11")> Public Property pos_to As Long
    <DatabaseField("pos_to_guid"), DataType(MySqlDbType.Int64, "11")> Public Property pos_to_guid As Long
    <DatabaseField("pto_type_id"), DataType(MySqlDbType.Int64, "11")> Public Property pto_type_id As Long
    <DatabaseField("pto_side_guid"), DataType(MySqlDbType.Int64, "11")> Public Property pto_side_guid As Long
    <DatabaseField("sequence"), DataType(MySqlDbType.VarChar, "255")> Public Property sequence As String
    <DatabaseField("descript"), DataType(MySqlDbType.Blob)> Public Property descript As Byte()
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `sec_structures` (`sec_struct_guid`, `pkg_guid`, `art_guid`, `name`, `fl_real_name`, `genome_guid`, `pos_from`, `pos_from_guid`, `pfo_type_id`, `pfo_side_guid`, `pos_to`, `pos_to_guid`, `pto_type_id`, `pto_side_guid`, `sequence`, `descript`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `sec_structures` (`sec_struct_guid`, `pkg_guid`, `art_guid`, `name`, `fl_real_name`, `genome_guid`, `pos_from`, `pos_from_guid`, `pfo_type_id`, `pfo_side_guid`, `pos_to`, `pos_to_guid`, `pto_type_id`, `pto_side_guid`, `sequence`, `descript`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `sec_structures` WHERE `sec_struct_guid` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `sec_structures` SET `sec_struct_guid`='{0}', `pkg_guid`='{1}', `art_guid`='{2}', `name`='{3}', `fl_real_name`='{4}', `genome_guid`='{5}', `pos_from`='{6}', `pos_from_guid`='{7}', `pfo_type_id`='{8}', `pfo_side_guid`='{9}', `pos_to`='{10}', `pos_to_guid`='{11}', `pto_type_id`='{12}', `pto_side_guid`='{13}', `sequence`='{14}', `descript`='{15}' WHERE `sec_struct_guid` = '{16}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, sec_struct_guid)
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, sec_struct_guid, pkg_guid, art_guid, name, fl_real_name, genome_guid, pos_from, pos_from_guid, pfo_type_id, pfo_side_guid, pos_to, pos_to_guid, pto_type_id, pto_side_guid, sequence, descript)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, sec_struct_guid, pkg_guid, art_guid, name, fl_real_name, genome_guid, pos_from, pos_from_guid, pfo_type_id, pfo_side_guid, pos_to, pos_to_guid, pto_type_id, pto_side_guid, sequence, descript)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, sec_struct_guid, pkg_guid, art_guid, name, fl_real_name, genome_guid, pos_from, pos_from_guid, pfo_type_id, pfo_side_guid, pos_to, pos_to_guid, pto_type_id, pto_side_guid, sequence, descript, sec_struct_guid)
    End Function
#End Region
End Class


End Namespace

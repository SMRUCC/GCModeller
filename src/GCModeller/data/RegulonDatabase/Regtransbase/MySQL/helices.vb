#Region "Microsoft.VisualBasic::e4a9afb0b28ddbab72b09ec64f2c4aac, ..\GCModeller\data\RegulonDatabase\Regtransbase\MySQL\helices.vb"

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
''' DROP TABLE IF EXISTS `helices`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `helices` (
'''   `helix_guid` int(11) NOT NULL DEFAULT '0',
'''   `pkg_guid` int(11) NOT NULL DEFAULT '0',
'''   `art_guid` int(11) NOT NULL DEFAULT '0',
'''   `name` varchar(50) DEFAULT NULL,
'''   `fl_real_name` int(1) DEFAULT NULL,
'''   `genome_guid` int(11) DEFAULT NULL,
'''   `sec_struct_guid` int(11) DEFAULT NULL,
'''   `pos_from1` int(11) DEFAULT NULL,
'''   `pos_to1` int(11) DEFAULT NULL,
'''   `pos_from2` int(11) DEFAULT NULL,
'''   `pos_to2` int(11) DEFAULT NULL,
'''   `descript` blob,
'''   PRIMARY KEY (`helix_guid`),
'''   KEY `FK_helices-pkg_guid` (`pkg_guid`),
'''   KEY `FK_helices-art_guid` (`art_guid`),
'''   KEY `FK_helices-genome_guid` (`genome_guid`),
'''   KEY `FK_helices-sec_struct_guid` (`sec_struct_guid`),
'''   CONSTRAINT `FK_helices-art_guid` FOREIGN KEY (`art_guid`) REFERENCES `articles` (`art_guid`),
'''   CONSTRAINT `FK_helices-genome_guid` FOREIGN KEY (`genome_guid`) REFERENCES `dict_genomes` (`genome_guid`),
'''   CONSTRAINT `FK_helices-pkg_guid` FOREIGN KEY (`pkg_guid`) REFERENCES `packages` (`pkg_guid`),
'''   CONSTRAINT `FK_helices-sec_struct_guid` FOREIGN KEY (`sec_struct_guid`) REFERENCES `sec_structures` (`sec_struct_guid`),
'''   CONSTRAINT `helices_ibfk_1` FOREIGN KEY (`sec_struct_guid`) REFERENCES `obj_name_genomes` (`obj_guid`)
''' ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("helices", Database:="dbregulation_update")>
Public Class helices: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("helix_guid"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "11")> Public Property helix_guid As Long
    <DatabaseField("pkg_guid"), NotNull, DataType(MySqlDbType.Int64, "11")> Public Property pkg_guid As Long
    <DatabaseField("art_guid"), NotNull, DataType(MySqlDbType.Int64, "11")> Public Property art_guid As Long
    <DatabaseField("name"), DataType(MySqlDbType.VarChar, "50")> Public Property name As String
    <DatabaseField("fl_real_name"), DataType(MySqlDbType.Int64, "1")> Public Property fl_real_name As Long
    <DatabaseField("genome_guid"), DataType(MySqlDbType.Int64, "11")> Public Property genome_guid As Long
    <DatabaseField("sec_struct_guid"), DataType(MySqlDbType.Int64, "11")> Public Property sec_struct_guid As Long
    <DatabaseField("pos_from1"), DataType(MySqlDbType.Int64, "11")> Public Property pos_from1 As Long
    <DatabaseField("pos_to1"), DataType(MySqlDbType.Int64, "11")> Public Property pos_to1 As Long
    <DatabaseField("pos_from2"), DataType(MySqlDbType.Int64, "11")> Public Property pos_from2 As Long
    <DatabaseField("pos_to2"), DataType(MySqlDbType.Int64, "11")> Public Property pos_to2 As Long
    <DatabaseField("descript"), DataType(MySqlDbType.Blob)> Public Property descript As Byte()
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `helices` (`helix_guid`, `pkg_guid`, `art_guid`, `name`, `fl_real_name`, `genome_guid`, `sec_struct_guid`, `pos_from1`, `pos_to1`, `pos_from2`, `pos_to2`, `descript`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `helices` (`helix_guid`, `pkg_guid`, `art_guid`, `name`, `fl_real_name`, `genome_guid`, `sec_struct_guid`, `pos_from1`, `pos_to1`, `pos_from2`, `pos_to2`, `descript`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `helices` WHERE `helix_guid` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `helices` SET `helix_guid`='{0}', `pkg_guid`='{1}', `art_guid`='{2}', `name`='{3}', `fl_real_name`='{4}', `genome_guid`='{5}', `sec_struct_guid`='{6}', `pos_from1`='{7}', `pos_to1`='{8}', `pos_from2`='{9}', `pos_to2`='{10}', `descript`='{11}' WHERE `helix_guid` = '{12}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, helix_guid)
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, helix_guid, pkg_guid, art_guid, name, fl_real_name, genome_guid, sec_struct_guid, pos_from1, pos_to1, pos_from2, pos_to2, descript)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, helix_guid, pkg_guid, art_guid, name, fl_real_name, genome_guid, sec_struct_guid, pos_from1, pos_to1, pos_from2, pos_to2, descript)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, helix_guid, pkg_guid, art_guid, name, fl_real_name, genome_guid, sec_struct_guid, pos_from1, pos_to1, pos_from2, pos_to2, descript, helix_guid)
    End Function
#End Region
End Class


End Namespace

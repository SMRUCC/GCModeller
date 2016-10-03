#Region "Microsoft.VisualBasic::d31092d511172534e835b7b80ea2b04f, ..\GCModeller\data\RegulonDatabase\Regtransbase\MySQL\transcripts.vb"

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
''' DROP TABLE IF EXISTS `transcripts`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `transcripts` (
'''   `transcript_guid` int(11) NOT NULL DEFAULT '0',
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
'''   `tr_len` int(11) DEFAULT NULL,
'''   `descript` blob,
'''   `last_update` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
'''   PRIMARY KEY (`transcript_guid`),
'''   KEY `FK_transcripts-pkg_guid` (`pkg_guid`),
'''   KEY `FK_transcripts-art_guid` (`art_guid`),
'''   KEY `FK_transcripts-genome_guid` (`genome_guid`),
'''   CONSTRAINT `FK_transcripts-art_guid` FOREIGN KEY (`art_guid`) REFERENCES `articles` (`art_guid`),
'''   CONSTRAINT `FK_transcripts-genome_guid` FOREIGN KEY (`genome_guid`) REFERENCES `dict_genomes` (`genome_guid`),
'''   CONSTRAINT `FK_transcripts-pkg_guid` FOREIGN KEY (`pkg_guid`) REFERENCES `packages` (`pkg_guid`)
''' ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
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
''' -- Dump completed on 2015-10-09  1:50:00
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("transcripts", Database:="dbregulation_update")>
Public Class transcripts: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("transcript_guid"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "11")> Public Property transcript_guid As Long
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
    <DatabaseField("tr_len"), DataType(MySqlDbType.Int64, "11")> Public Property tr_len As Long
    <DatabaseField("descript"), DataType(MySqlDbType.Blob)> Public Property descript As Byte()
    <DatabaseField("last_update"), NotNull, DataType(MySqlDbType.DateTime)> Public Property last_update As Date
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `transcripts` (`transcript_guid`, `pkg_guid`, `art_guid`, `name`, `fl_real_name`, `genome_guid`, `pos_from`, `pos_from_guid`, `pfo_type_id`, `pfo_side_guid`, `pos_to`, `pos_to_guid`, `pto_type_id`, `pto_side_guid`, `tr_len`, `descript`, `last_update`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}', '{16}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `transcripts` (`transcript_guid`, `pkg_guid`, `art_guid`, `name`, `fl_real_name`, `genome_guid`, `pos_from`, `pos_from_guid`, `pfo_type_id`, `pfo_side_guid`, `pos_to`, `pos_to_guid`, `pto_type_id`, `pto_side_guid`, `tr_len`, `descript`, `last_update`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}', '{16}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `transcripts` WHERE `transcript_guid` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `transcripts` SET `transcript_guid`='{0}', `pkg_guid`='{1}', `art_guid`='{2}', `name`='{3}', `fl_real_name`='{4}', `genome_guid`='{5}', `pos_from`='{6}', `pos_from_guid`='{7}', `pfo_type_id`='{8}', `pfo_side_guid`='{9}', `pos_to`='{10}', `pos_to_guid`='{11}', `pto_type_id`='{12}', `pto_side_guid`='{13}', `tr_len`='{14}', `descript`='{15}', `last_update`='{16}' WHERE `transcript_guid` = '{17}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, transcript_guid)
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, transcript_guid, pkg_guid, art_guid, name, fl_real_name, genome_guid, pos_from, pos_from_guid, pfo_type_id, pfo_side_guid, pos_to, pos_to_guid, pto_type_id, pto_side_guid, tr_len, descript, DataType.ToMySqlDateTimeString(last_update))
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, transcript_guid, pkg_guid, art_guid, name, fl_real_name, genome_guid, pos_from, pos_from_guid, pfo_type_id, pfo_side_guid, pos_to, pos_to_guid, pto_type_id, pto_side_guid, tr_len, descript, DataType.ToMySqlDateTimeString(last_update))
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, transcript_guid, pkg_guid, art_guid, name, fl_real_name, genome_guid, pos_from, pos_from_guid, pfo_type_id, pfo_side_guid, pos_to, pos_to_guid, pto_type_id, pto_side_guid, tr_len, descript, DataType.ToMySqlDateTimeString(last_update), transcript_guid)
    End Function
#End Region
End Class


End Namespace

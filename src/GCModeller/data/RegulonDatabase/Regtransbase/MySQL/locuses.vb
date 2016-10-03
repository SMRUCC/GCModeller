#Region "Microsoft.VisualBasic::8877a1e991fcace6b36b623f72ff3dbd, ..\GCModeller\data\RegulonDatabase\Regtransbase\MySQL\locuses.vb"

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
''' DROP TABLE IF EXISTS `locuses`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `locuses` (
'''   `locus_guid` int(11) NOT NULL DEFAULT '0',
'''   `pkg_guid` int(11) NOT NULL DEFAULT '0',
'''   `art_guid` int(11) NOT NULL DEFAULT '0',
'''   `name` varchar(50) DEFAULT NULL,
'''   `fl_real_name` int(1) DEFAULT NULL,
'''   `genome_guid` int(11) DEFAULT NULL,
'''   `location` varchar(50) DEFAULT NULL,
'''   `descript` blob,
'''   `last_update` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
'''   PRIMARY KEY (`locus_guid`),
'''   KEY `FK_locuses-pkg_guid` (`pkg_guid`),
'''   KEY `FK_locuses-art_guid` (`art_guid`),
'''   KEY `FK_locuses-genome_guid` (`genome_guid`),
'''   CONSTRAINT `FK_locuses-art_guid` FOREIGN KEY (`art_guid`) REFERENCES `articles` (`art_guid`),
'''   CONSTRAINT `FK_locuses-genome_guid` FOREIGN KEY (`genome_guid`) REFERENCES `dict_genomes` (`genome_guid`),
'''   CONSTRAINT `FK_locuses-pkg_guid` FOREIGN KEY (`pkg_guid`) REFERENCES `packages` (`pkg_guid`)
''' ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("locuses", Database:="dbregulation_update")>
Public Class locuses: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("locus_guid"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "11")> Public Property locus_guid As Long
    <DatabaseField("pkg_guid"), NotNull, DataType(MySqlDbType.Int64, "11")> Public Property pkg_guid As Long
    <DatabaseField("art_guid"), NotNull, DataType(MySqlDbType.Int64, "11")> Public Property art_guid As Long
    <DatabaseField("name"), DataType(MySqlDbType.VarChar, "50")> Public Property name As String
    <DatabaseField("fl_real_name"), DataType(MySqlDbType.Int64, "1")> Public Property fl_real_name As Long
    <DatabaseField("genome_guid"), DataType(MySqlDbType.Int64, "11")> Public Property genome_guid As Long
    <DatabaseField("location"), DataType(MySqlDbType.VarChar, "50")> Public Property location As String
    <DatabaseField("descript"), DataType(MySqlDbType.Blob)> Public Property descript As Byte()
    <DatabaseField("last_update"), NotNull, DataType(MySqlDbType.DateTime)> Public Property last_update As Date
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `locuses` (`locus_guid`, `pkg_guid`, `art_guid`, `name`, `fl_real_name`, `genome_guid`, `location`, `descript`, `last_update`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `locuses` (`locus_guid`, `pkg_guid`, `art_guid`, `name`, `fl_real_name`, `genome_guid`, `location`, `descript`, `last_update`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `locuses` WHERE `locus_guid` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `locuses` SET `locus_guid`='{0}', `pkg_guid`='{1}', `art_guid`='{2}', `name`='{3}', `fl_real_name`='{4}', `genome_guid`='{5}', `location`='{6}', `descript`='{7}', `last_update`='{8}' WHERE `locus_guid` = '{9}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, locus_guid)
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, locus_guid, pkg_guid, art_guid, name, fl_real_name, genome_guid, location, descript, DataType.ToMySqlDateTimeString(last_update))
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, locus_guid, pkg_guid, art_guid, name, fl_real_name, genome_guid, location, descript, DataType.ToMySqlDateTimeString(last_update))
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, locus_guid, pkg_guid, art_guid, name, fl_real_name, genome_guid, location, descript, DataType.ToMySqlDateTimeString(last_update), locus_guid)
    End Function
#End Region
End Class


End Namespace

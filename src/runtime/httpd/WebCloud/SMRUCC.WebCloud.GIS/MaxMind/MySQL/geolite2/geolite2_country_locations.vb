#Region "Microsoft.VisualBasic::9f14df4464ebc103de1b9c28bf874780, ..\httpd\WebCloud\SMRUCC.WebCloud.GIS\MaxMind\MySQL\geolite2\geolite2_country_locations.vb"

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
REM      for Microsoft VisualBasic.NET 1.0.0.0

REM  Dump @9/4/2016 5:29:45 PM


Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes

Namespace MaxMind.geolite2

''' <summary>
''' ```SQL
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `geolite2_country_locations`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `geolite2_country_locations` (
'''   `geoname_id` int(11) NOT NULL,
'''   `locale_code` varchar(45) NOT NULL,
'''   `continent_code` varchar(45) DEFAULT NULL,
'''   `continent_name` varchar(45) DEFAULT NULL,
'''   `country_iso_code` varchar(45) DEFAULT NULL,
'''   `country_name` tinytext,
'''   PRIMARY KEY (`geoname_id`,`locale_code`),
'''   UNIQUE KEY `geoname_id_UNIQUE` (`geoname_id`)
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
''' -- Dump completed on 2016-09-05  1:26:27
''' 
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("geolite2_country_locations", Database:="maxmind_geolite2", SchemaSQL:="
CREATE TABLE `geolite2_country_locations` (
  `geoname_id` int(11) NOT NULL,
  `locale_code` varchar(45) NOT NULL,
  `continent_code` varchar(45) DEFAULT NULL,
  `continent_name` varchar(45) DEFAULT NULL,
  `country_iso_code` varchar(45) DEFAULT NULL,
  `country_name` tinytext,
  PRIMARY KEY (`geoname_id`,`locale_code`),
  UNIQUE KEY `geoname_id_UNIQUE` (`geoname_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;")>
Public Class geolite2_country_locations: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("geoname_id"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "11")> Public Property geoname_id As Long
    <DatabaseField("locale_code"), PrimaryKey, NotNull, DataType(MySqlDbType.VarChar, "45")> Public Property locale_code As String
    <DatabaseField("continent_code"), DataType(MySqlDbType.VarChar, "45")> Public Property continent_code As String
    <DatabaseField("continent_name"), DataType(MySqlDbType.VarChar, "45")> Public Property continent_name As String
    <DatabaseField("country_iso_code"), DataType(MySqlDbType.VarChar, "45")> Public Property country_iso_code As String
    <DatabaseField("country_name"), DataType(MySqlDbType.Text)> Public Property country_name As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `geolite2_country_locations` (`geoname_id`, `locale_code`, `continent_code`, `continent_name`, `country_iso_code`, `country_name`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `geolite2_country_locations` (`geoname_id`, `locale_code`, `continent_code`, `continent_name`, `country_iso_code`, `country_name`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `geolite2_country_locations` WHERE `geoname_id`='{0}' and `locale_code`='{1}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `geolite2_country_locations` SET `geoname_id`='{0}', `locale_code`='{1}', `continent_code`='{2}', `continent_name`='{3}', `country_iso_code`='{4}', `country_name`='{5}' WHERE `geoname_id`='{6}' and `locale_code`='{7}';</SQL>
#End Region
''' <summary>
''' ```SQL
''' DELETE FROM `geolite2_country_locations` WHERE `geoname_id`='{0}' and `locale_code`='{1}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, geoname_id, locale_code)
    End Function
''' <summary>
''' ```SQL
''' INSERT INTO `geolite2_country_locations` (`geoname_id`, `locale_code`, `continent_code`, `continent_name`, `country_iso_code`, `country_name`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, geoname_id, locale_code, continent_code, continent_name, country_iso_code, country_name)
    End Function
''' <summary>
''' ```SQL
''' REPLACE INTO `geolite2_country_locations` (`geoname_id`, `locale_code`, `continent_code`, `continent_name`, `country_iso_code`, `country_name`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, geoname_id, locale_code, continent_code, continent_name, country_iso_code, country_name)
    End Function
''' <summary>
''' ```SQL
''' UPDATE `geolite2_country_locations` SET `geoname_id`='{0}', `locale_code`='{1}', `continent_code`='{2}', `continent_name`='{3}', `country_iso_code`='{4}', `country_name`='{5}' WHERE `geoname_id`='{6}' and `locale_code`='{7}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, geoname_id, locale_code, continent_code, continent_name, country_iso_code, country_name, geoname_id, locale_code)
    End Function
#End Region
End Class


End Namespace

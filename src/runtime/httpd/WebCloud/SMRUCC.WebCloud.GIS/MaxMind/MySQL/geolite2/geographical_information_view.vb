#Region "Microsoft.VisualBasic::77bbbd5b52a5f00fae09c548dd69e829, ..\httpd\WebCloud\SMRUCC.WebCloud.GIS\MaxMind\MySQL\geolite2\geographical_information_view.vb"

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
''' DROP TABLE IF EXISTS `geographical_information_view`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `geographical_information_view` (
'''   `geoname_id` int(11) NOT NULL,
'''   `latitude` double NOT NULL,
'''   `longitude` double NOT NULL,
'''   `country_iso_code` varchar(16) DEFAULT NULL,
'''   `country_name` varchar(64) DEFAULT NULL,
'''   `city_name` varchar(128) DEFAULT NULL,
'''   `subdivision_1_name` varchar(128) DEFAULT NULL,
'''   `subdivision_2_name` varchar(128) DEFAULT NULL,
'''   PRIMARY KEY (`geoname_id`),
'''   UNIQUE KEY `geoname_id_UNIQUE` (`geoname_id`)
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("geographical_information_view", Database:="maxmind_geolite2", SchemaSQL:="
CREATE TABLE `geographical_information_view` (
  `geoname_id` int(11) NOT NULL,
  `latitude` double NOT NULL,
  `longitude` double NOT NULL,
  `country_iso_code` varchar(16) DEFAULT NULL,
  `country_name` varchar(64) DEFAULT NULL,
  `city_name` varchar(128) DEFAULT NULL,
  `subdivision_1_name` varchar(128) DEFAULT NULL,
  `subdivision_2_name` varchar(128) DEFAULT NULL,
  PRIMARY KEY (`geoname_id`),
  UNIQUE KEY `geoname_id_UNIQUE` (`geoname_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;")>
Public Class geographical_information_view: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("geoname_id"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "11")> Public Property geoname_id As Long
    <DatabaseField("latitude"), NotNull, DataType(MySqlDbType.Double)> Public Property latitude As Double
    <DatabaseField("longitude"), NotNull, DataType(MySqlDbType.Double)> Public Property longitude As Double
    <DatabaseField("country_iso_code"), DataType(MySqlDbType.VarChar, "16")> Public Property country_iso_code As String
    <DatabaseField("country_name"), DataType(MySqlDbType.VarChar, "64")> Public Property country_name As String
    <DatabaseField("city_name"), DataType(MySqlDbType.VarChar, "128")> Public Property city_name As String
    <DatabaseField("subdivision_1_name"), DataType(MySqlDbType.VarChar, "128")> Public Property subdivision_1_name As String
    <DatabaseField("subdivision_2_name"), DataType(MySqlDbType.VarChar, "128")> Public Property subdivision_2_name As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `geographical_information_view` (`geoname_id`, `latitude`, `longitude`, `country_iso_code`, `country_name`, `city_name`, `subdivision_1_name`, `subdivision_2_name`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `geographical_information_view` (`geoname_id`, `latitude`, `longitude`, `country_iso_code`, `country_name`, `city_name`, `subdivision_1_name`, `subdivision_2_name`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `geographical_information_view` WHERE `geoname_id` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `geographical_information_view` SET `geoname_id`='{0}', `latitude`='{1}', `longitude`='{2}', `country_iso_code`='{3}', `country_name`='{4}', `city_name`='{5}', `subdivision_1_name`='{6}', `subdivision_2_name`='{7}' WHERE `geoname_id` = '{8}';</SQL>
#End Region
''' <summary>
''' ```SQL
''' DELETE FROM `geographical_information_view` WHERE `geoname_id` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, geoname_id)
    End Function
''' <summary>
''' ```SQL
''' INSERT INTO `geographical_information_view` (`geoname_id`, `latitude`, `longitude`, `country_iso_code`, `country_name`, `city_name`, `subdivision_1_name`, `subdivision_2_name`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, geoname_id, latitude, longitude, country_iso_code, country_name, city_name, subdivision_1_name, subdivision_2_name)
    End Function
''' <summary>
''' ```SQL
''' REPLACE INTO `geographical_information_view` (`geoname_id`, `latitude`, `longitude`, `country_iso_code`, `country_name`, `city_name`, `subdivision_1_name`, `subdivision_2_name`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, geoname_id, latitude, longitude, country_iso_code, country_name, city_name, subdivision_1_name, subdivision_2_name)
    End Function
''' <summary>
''' ```SQL
''' UPDATE `geographical_information_view` SET `geoname_id`='{0}', `latitude`='{1}', `longitude`='{2}', `country_iso_code`='{3}', `country_name`='{4}', `city_name`='{5}', `subdivision_1_name`='{6}', `subdivision_2_name`='{7}' WHERE `geoname_id` = '{8}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, geoname_id, latitude, longitude, country_iso_code, country_name, city_name, subdivision_1_name, subdivision_2_name, geoname_id)
    End Function
#End Region
End Class


End Namespace

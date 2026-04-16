#Region "Microsoft.VisualBasic::7a7d9df62348bb0838ecbbc13818116e, WebCloud\SMRUCC.WebCloud.GIS\MaxMind\MySQL\geolite2\geolite2_city_locations.vb"

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

    ' Class geolite2_city_locations
    ' 
    '     Properties: city_name, continent_code, continent_name, country_iso_code, country_name
    '                 geoname_id, locale_code, metro_code, subdivision_1_iso_code, subdivision_1_name
    '                 subdivision_2_iso_code, subdivision_2_name, time_zone
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

REM  Dump @12/2/2018 7:45:34 PM


Imports System.Data.Linq.Mapping
Imports System.Xml.Serialization
Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes
Imports MySqlScript = Oracle.LinuxCompatibility.MySQL.Scripting.Extensions

Namespace MaxMind.geolite2

''' <summary>
''' ```SQL
''' 												\n
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `geolite2_city_locations`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `geolite2_city_locations` (
'''   `geoname_id` int(11) NOT NULL COMMENT 'A unique identifier for the a location as specified by GeoNames. This ID can be used as a key for the Location file.',
'''   `locale_code` varchar(16) NOT NULL COMMENT 'The locale that the names in this row are in. This will always correspond to the locale name of the file.',
'''   `continent_code` varchar(32) DEFAULT NULL COMMENT 'The continent code for this location. Possible codes are:\nAF - Africa\nAS - Asia\nEU - Europe \nNA - North America\nOC - Oceania\nSA - South America',
'''   `continent_name` varchar(512) DEFAULT NULL COMMENT 'The continent name for this location in the file’s locale.',
'''   `country_iso_code` varchar(512) DEFAULT NULL COMMENT 'A two-character ISO 3166-1 country code for the country associated with the location.',
'''   `country_name` varchar(512) DEFAULT NULL COMMENT 'The country name for this location in the file’s locale.',
'''   `subdivision_1_iso_code` varchar(512) DEFAULT NULL COMMENT 'A string of up to three characters containing the region-portion of the ISO 3166-2 code for the first level region associated with the IP address. Some countries have two levels of subdivisions, in which case this is the least specific. For example, in the United Kingdom this will be a country like ''England'', not a county like ''Devon''.',
'''   `subdivision_1_name` varchar(512) DEFAULT NULL COMMENT 'The subdivision name for this location in the file’s locale. As with the subdivision code, this is the least specific subdivision for the location.',
'''   `subdivision_2_iso_code` varchar(512) DEFAULT NULL COMMENT 'A string of up to three characters containing the region-portion of the ISO 3166-2 code for the second level region associated with the IP address. Some countries have two levels of subdivisions, in which case this is the most specific. For example, in the United Kingdom this will be a a county like ''Devon'', not a country like ''England''.',
'''   `subdivision_2_name` varchar(512) DEFAULT NULL COMMENT 'The subdivision name for this location in the file’s locale. As with the subdivision code, this is the most specific subdivision for the location.',
'''   `city_name` varchar(512) DEFAULT NULL COMMENT 'The city name for this location in the file’s locale.',
'''   `metro_code` int(11) DEFAULT NULL COMMENT 'The metro code associated with the IP address. These are only available for networks in the US. MaxMind provides the same metro codes as the Google AdWords API.',
'''   `time_zone` varchar(128) DEFAULT NULL COMMENT 'The time zone associated with location, as specified by the IANA Time Zone Database, e.g., ''America/New_York''.',
'''   PRIMARY KEY (`geoname_id`,`locale_code`)
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='												\n';
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("geolite2_city_locations", Database:="maxmind_geolite2", SchemaSQL:="
CREATE TABLE `geolite2_city_locations` (
  `geoname_id` int(11) NOT NULL COMMENT 'A unique identifier for the a location as specified by GeoNames. This ID can be used as a key for the Location file.',
  `locale_code` varchar(16) NOT NULL COMMENT 'The locale that the names in this row are in. This will always correspond to the locale name of the file.',
  `continent_code` varchar(32) DEFAULT NULL COMMENT 'The continent code for this location. Possible codes are:\nAF - Africa\nAS - Asia\nEU - Europe \nNA - North America\nOC - Oceania\nSA - South America',
  `continent_name` varchar(512) DEFAULT NULL COMMENT 'The continent name for this location in the file’s locale.',
  `country_iso_code` varchar(512) DEFAULT NULL COMMENT 'A two-character ISO 3166-1 country code for the country associated with the location.',
  `country_name` varchar(512) DEFAULT NULL COMMENT 'The country name for this location in the file’s locale.',
  `subdivision_1_iso_code` varchar(512) DEFAULT NULL COMMENT 'A string of up to three characters containing the region-portion of the ISO 3166-2 code for the first level region associated with the IP address. Some countries have two levels of subdivisions, in which case this is the least specific. For example, in the United Kingdom this will be a country like ''England'', not a county like ''Devon''.',
  `subdivision_1_name` varchar(512) DEFAULT NULL COMMENT 'The subdivision name for this location in the file’s locale. As with the subdivision code, this is the least specific subdivision for the location.',
  `subdivision_2_iso_code` varchar(512) DEFAULT NULL COMMENT 'A string of up to three characters containing the region-portion of the ISO 3166-2 code for the second level region associated with the IP address. Some countries have two levels of subdivisions, in which case this is the most specific. For example, in the United Kingdom this will be a a county like ''Devon'', not a country like ''England''.',
  `subdivision_2_name` varchar(512) DEFAULT NULL COMMENT 'The subdivision name for this location in the file’s locale. As with the subdivision code, this is the most specific subdivision for the location.',
  `city_name` varchar(512) DEFAULT NULL COMMENT 'The city name for this location in the file’s locale.',
  `metro_code` int(11) DEFAULT NULL COMMENT 'The metro code associated with the IP address. These are only available for networks in the US. MaxMind provides the same metro codes as the Google AdWords API.',
  `time_zone` varchar(128) DEFAULT NULL COMMENT 'The time zone associated with location, as specified by the IANA Time Zone Database, e.g., ''America/New_York''.',
  PRIMARY KEY (`geoname_id`,`locale_code`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='												\n';")>
Public Class geolite2_city_locations: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
''' <summary>
''' A unique identifier for the a location as specified by GeoNames. This ID can be used as a key for the Location file.
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
    <DatabaseField("geoname_id"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "11"), Column(Name:="geoname_id"), XmlAttribute> Public Property geoname_id As Long
''' <summary>
''' The locale that the names in this row are in. This will always correspond to the locale name of the file.
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
    <DatabaseField("locale_code"), PrimaryKey, NotNull, DataType(MySqlDbType.VarChar, "16"), Column(Name:="locale_code"), XmlAttribute> Public Property locale_code As String
''' <summary>
''' The continent code for this location. Possible codes are:\nAF - Africa\nAS - Asia\nEU - Europe \nNA - North America\nOC - Oceania\nSA - South America
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
    <DatabaseField("continent_code"), DataType(MySqlDbType.VarChar, "32"), Column(Name:="continent_code")> Public Property continent_code As String
''' <summary>
''' The continent name for this location in the file’s locale.
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
    <DatabaseField("continent_name"), DataType(MySqlDbType.VarChar, "512"), Column(Name:="continent_name")> Public Property continent_name As String
''' <summary>
''' A two-character ISO 3166-1 country code for the country associated with the location.
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
    <DatabaseField("country_iso_code"), DataType(MySqlDbType.VarChar, "512"), Column(Name:="country_iso_code")> Public Property country_iso_code As String
''' <summary>
''' The country name for this location in the file’s locale.
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
    <DatabaseField("country_name"), DataType(MySqlDbType.VarChar, "512"), Column(Name:="country_name")> Public Property country_name As String
''' <summary>
''' A string of up to three characters containing the region-portion of the ISO 3166-2 code for the first level region associated with the IP address. Some countries have two levels of subdivisions, in which case this is the least specific. For example, in the United Kingdom this will be a country like ''England'
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
    <DatabaseField("subdivision_1_iso_code"), DataType(MySqlDbType.VarChar, "512"), Column(Name:="subdivision_1_iso_code")> Public Property subdivision_1_iso_code As String
''' <summary>
''' The subdivision name for this location in the file’s locale. As with the subdivision code, this is the least specific subdivision for the location.
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
    <DatabaseField("subdivision_1_name"), DataType(MySqlDbType.VarChar, "512"), Column(Name:="subdivision_1_name")> Public Property subdivision_1_name As String
''' <summary>
''' A string of up to three characters containing the region-portion of the ISO 3166-2 code for the second level region associated with the IP address. Some countries have two levels of subdivisions, in which case this is the most specific. For example, in the United Kingdom this will be a a county like ''Devon'
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
    <DatabaseField("subdivision_2_iso_code"), DataType(MySqlDbType.VarChar, "512"), Column(Name:="subdivision_2_iso_code")> Public Property subdivision_2_iso_code As String
''' <summary>
''' The subdivision name for this location in the file’s locale. As with the subdivision code, this is the most specific subdivision for the location.
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
    <DatabaseField("subdivision_2_name"), DataType(MySqlDbType.VarChar, "512"), Column(Name:="subdivision_2_name")> Public Property subdivision_2_name As String
''' <summary>
''' The city name for this location in the file’s locale.
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
    <DatabaseField("city_name"), DataType(MySqlDbType.VarChar, "512"), Column(Name:="city_name")> Public Property city_name As String
''' <summary>
''' The metro code associated with the IP address. These are only available for networks in the US. MaxMind provides the same metro codes as the Google AdWords API.
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
    <DatabaseField("metro_code"), DataType(MySqlDbType.Int64, "11"), Column(Name:="metro_code")> Public Property metro_code As Long
''' <summary>
''' The time zone associated with location, as specified by the IANA Time Zone Database, e.g., ''America/New_York''.
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
    <DatabaseField("time_zone"), DataType(MySqlDbType.VarChar, "128"), Column(Name:="time_zone")> Public Property time_zone As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `geolite2_city_locations` (`geoname_id`, `locale_code`, `continent_code`, `continent_name`, `country_iso_code`, `country_name`, `subdivision_1_iso_code`, `subdivision_1_name`, `subdivision_2_iso_code`, `subdivision_2_name`, `city_name`, `metro_code`, `time_zone`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `geolite2_city_locations` (`geoname_id`, `locale_code`, `continent_code`, `continent_name`, `country_iso_code`, `country_name`, `subdivision_1_iso_code`, `subdivision_1_name`, `subdivision_2_iso_code`, `subdivision_2_name`, `city_name`, `metro_code`, `time_zone`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `geolite2_city_locations` (`geoname_id`, `locale_code`, `continent_code`, `continent_name`, `country_iso_code`, `country_name`, `subdivision_1_iso_code`, `subdivision_1_name`, `subdivision_2_iso_code`, `subdivision_2_name`, `city_name`, `metro_code`, `time_zone`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `geolite2_city_locations` (`geoname_id`, `locale_code`, `continent_code`, `continent_name`, `country_iso_code`, `country_name`, `subdivision_1_iso_code`, `subdivision_1_name`, `subdivision_2_iso_code`, `subdivision_2_name`, `city_name`, `metro_code`, `time_zone`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `geolite2_city_locations` WHERE `geoname_id`='{0}' and `locale_code`='{1}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `geolite2_city_locations` SET `geoname_id`='{0}', `locale_code`='{1}', `continent_code`='{2}', `continent_name`='{3}', `country_iso_code`='{4}', `country_name`='{5}', `subdivision_1_iso_code`='{6}', `subdivision_1_name`='{7}', `subdivision_2_iso_code`='{8}', `subdivision_2_name`='{9}', `city_name`='{10}', `metro_code`='{11}', `time_zone`='{12}' WHERE `geoname_id`='{13}' and `locale_code`='{14}';</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `geolite2_city_locations` WHERE `geoname_id`='{0}' and `locale_code`='{1}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, geoname_id, locale_code)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `geolite2_city_locations` (`geoname_id`, `locale_code`, `continent_code`, `continent_name`, `country_iso_code`, `country_name`, `subdivision_1_iso_code`, `subdivision_1_name`, `subdivision_2_iso_code`, `subdivision_2_name`, `city_name`, `metro_code`, `time_zone`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, geoname_id, locale_code, continent_code, continent_name, country_iso_code, country_name, subdivision_1_iso_code, subdivision_1_name, subdivision_2_iso_code, subdivision_2_name, city_name, metro_code, time_zone)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `geolite2_city_locations` (`geoname_id`, `locale_code`, `continent_code`, `continent_name`, `country_iso_code`, `country_name`, `subdivision_1_iso_code`, `subdivision_1_name`, `subdivision_2_iso_code`, `subdivision_2_name`, `city_name`, `metro_code`, `time_zone`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, geoname_id, locale_code, continent_code, continent_name, country_iso_code, country_name, subdivision_1_iso_code, subdivision_1_name, subdivision_2_iso_code, subdivision_2_name, city_name, metro_code, time_zone)
        Else
        Return String.Format(INSERT_SQL, geoname_id, locale_code, continent_code, continent_name, country_iso_code, country_name, subdivision_1_iso_code, subdivision_1_name, subdivision_2_iso_code, subdivision_2_name, city_name, metro_code, time_zone)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{geoname_id}', '{locale_code}', '{continent_code}', '{continent_name}', '{country_iso_code}', '{country_name}', '{subdivision_1_iso_code}', '{subdivision_1_name}', '{subdivision_2_iso_code}', '{subdivision_2_name}', '{city_name}', '{metro_code}', '{time_zone}')"
        Else
            Return $"('{geoname_id}', '{locale_code}', '{continent_code}', '{continent_name}', '{country_iso_code}', '{country_name}', '{subdivision_1_iso_code}', '{subdivision_1_name}', '{subdivision_2_iso_code}', '{subdivision_2_name}', '{city_name}', '{metro_code}', '{time_zone}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `geolite2_city_locations` (`geoname_id`, `locale_code`, `continent_code`, `continent_name`, `country_iso_code`, `country_name`, `subdivision_1_iso_code`, `subdivision_1_name`, `subdivision_2_iso_code`, `subdivision_2_name`, `city_name`, `metro_code`, `time_zone`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, geoname_id, locale_code, continent_code, continent_name, country_iso_code, country_name, subdivision_1_iso_code, subdivision_1_name, subdivision_2_iso_code, subdivision_2_name, city_name, metro_code, time_zone)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `geolite2_city_locations` (`geoname_id`, `locale_code`, `continent_code`, `continent_name`, `country_iso_code`, `country_name`, `subdivision_1_iso_code`, `subdivision_1_name`, `subdivision_2_iso_code`, `subdivision_2_name`, `city_name`, `metro_code`, `time_zone`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, geoname_id, locale_code, continent_code, continent_name, country_iso_code, country_name, subdivision_1_iso_code, subdivision_1_name, subdivision_2_iso_code, subdivision_2_name, city_name, metro_code, time_zone)
        Else
        Return String.Format(REPLACE_SQL, geoname_id, locale_code, continent_code, continent_name, country_iso_code, country_name, subdivision_1_iso_code, subdivision_1_name, subdivision_2_iso_code, subdivision_2_name, city_name, metro_code, time_zone)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `geolite2_city_locations` SET `geoname_id`='{0}', `locale_code`='{1}', `continent_code`='{2}', `continent_name`='{3}', `country_iso_code`='{4}', `country_name`='{5}', `subdivision_1_iso_code`='{6}', `subdivision_1_name`='{7}', `subdivision_2_iso_code`='{8}', `subdivision_2_name`='{9}', `city_name`='{10}', `metro_code`='{11}', `time_zone`='{12}' WHERE `geoname_id`='{13}' and `locale_code`='{14}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, geoname_id, locale_code, continent_code, continent_name, country_iso_code, country_name, subdivision_1_iso_code, subdivision_1_name, subdivision_2_iso_code, subdivision_2_name, city_name, metro_code, time_zone, geoname_id, locale_code)
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As geolite2_city_locations
                         Return DirectCast(MyClass.MemberwiseClone, geolite2_city_locations)
                     End Function
End Class


End Namespace

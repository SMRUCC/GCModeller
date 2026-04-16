#Region "Microsoft.VisualBasic::c14bb0e2035e8505f36698716e81dc3a, WebCloud\SMRUCC.WebCloud.GIS\MaxMind\MySQL\geolite2\geolite2_country_locations.vb"

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

    ' Class geolite2_country_locations
    ' 
    '     Properties: continent_code, continent_name, country_iso_code, country_name, geoname_id
    '                 locale_code
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
''' -- Dump completed on 2017-11-05  1:18:50
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
) ENGINE=InnoDB DEFAULT CHARSET=utf8;")>
Public Class geolite2_country_locations: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("geoname_id"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "11"), Column(Name:="geoname_id"), XmlAttribute> Public Property geoname_id As Long
    <DatabaseField("locale_code"), PrimaryKey, NotNull, DataType(MySqlDbType.VarChar, "45"), Column(Name:="locale_code"), XmlAttribute> Public Property locale_code As String
    <DatabaseField("continent_code"), DataType(MySqlDbType.VarChar, "45"), Column(Name:="continent_code")> Public Property continent_code As String
    <DatabaseField("continent_name"), DataType(MySqlDbType.VarChar, "45"), Column(Name:="continent_name")> Public Property continent_name As String
    <DatabaseField("country_iso_code"), DataType(MySqlDbType.VarChar, "45"), Column(Name:="country_iso_code")> Public Property country_iso_code As String
    <DatabaseField("country_name"), DataType(MySqlDbType.Text), Column(Name:="country_name")> Public Property country_name As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `geolite2_country_locations` (`geoname_id`, `locale_code`, `continent_code`, `continent_name`, `country_iso_code`, `country_name`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `geolite2_country_locations` (`geoname_id`, `locale_code`, `continent_code`, `continent_name`, `country_iso_code`, `country_name`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `geolite2_country_locations` (`geoname_id`, `locale_code`, `continent_code`, `continent_name`, `country_iso_code`, `country_name`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `geolite2_country_locations` (`geoname_id`, `locale_code`, `continent_code`, `continent_name`, `country_iso_code`, `country_name`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `geolite2_country_locations` WHERE `geoname_id`='{0}' and `locale_code`='{1}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `geolite2_country_locations` SET `geoname_id`='{0}', `locale_code`='{1}', `continent_code`='{2}', `continent_name`='{3}', `country_iso_code`='{4}', `country_name`='{5}' WHERE `geoname_id`='{6}' and `locale_code`='{7}';</SQL>

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
''' INSERT INTO `geolite2_country_locations` (`geoname_id`, `locale_code`, `continent_code`, `continent_name`, `country_iso_code`, `country_name`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, geoname_id, locale_code, continent_code, continent_name, country_iso_code, country_name)
        Else
        Return String.Format(INSERT_SQL, geoname_id, locale_code, continent_code, continent_name, country_iso_code, country_name)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{geoname_id}', '{locale_code}', '{continent_code}', '{continent_name}', '{country_iso_code}', '{country_name}')"
        Else
            Return $"('{geoname_id}', '{locale_code}', '{continent_code}', '{continent_name}', '{country_iso_code}', '{country_name}')"
        End If
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
''' REPLACE INTO `geolite2_country_locations` (`geoname_id`, `locale_code`, `continent_code`, `continent_name`, `country_iso_code`, `country_name`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, geoname_id, locale_code, continent_code, continent_name, country_iso_code, country_name)
        Else
        Return String.Format(REPLACE_SQL, geoname_id, locale_code, continent_code, continent_name, country_iso_code, country_name)
        End If
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

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As geolite2_country_locations
                         Return DirectCast(MyClass.MemberwiseClone, geolite2_country_locations)
                     End Function
End Class


End Namespace

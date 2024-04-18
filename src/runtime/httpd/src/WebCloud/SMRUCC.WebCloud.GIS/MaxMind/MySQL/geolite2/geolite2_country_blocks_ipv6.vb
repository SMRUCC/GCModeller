#Region "Microsoft.VisualBasic::818e4afacf03f5e4f7c5c58270901ba4, WebCloud\SMRUCC.WebCloud.GIS\MaxMind\MySQL\geolite2\geolite2_country_blocks_ipv6.vb"

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

    ' Class geolite2_country_blocks_ipv6
    ' 
    '     Properties: geoname_id, is_anonymous_proxy, is_satellite_provider, network, registered_country_geoname_id
    '                 represented_country_geoname_id
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
''' DROP TABLE IF EXISTS `geolite2_country_blocks_ipv6`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `geolite2_country_blocks_ipv6` (
'''   `network` varchar(128) NOT NULL,
'''   `geoname_id` varchar(45) DEFAULT NULL,
'''   `registered_country_geoname_id` varchar(45) DEFAULT NULL,
'''   `represented_country_geoname_id` varchar(45) DEFAULT NULL,
'''   `is_anonymous_proxy` varchar(45) DEFAULT NULL,
'''   `is_satellite_provider` varchar(45) DEFAULT NULL,
'''   PRIMARY KEY (`network`),
'''   UNIQUE KEY `network_UNIQUE` (`network`)
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("geolite2_country_blocks_ipv6", Database:="maxmind_geolite2", SchemaSQL:="
CREATE TABLE `geolite2_country_blocks_ipv6` (
  `network` varchar(128) NOT NULL,
  `geoname_id` varchar(45) DEFAULT NULL,
  `registered_country_geoname_id` varchar(45) DEFAULT NULL,
  `represented_country_geoname_id` varchar(45) DEFAULT NULL,
  `is_anonymous_proxy` varchar(45) DEFAULT NULL,
  `is_satellite_provider` varchar(45) DEFAULT NULL,
  PRIMARY KEY (`network`),
  UNIQUE KEY `network_UNIQUE` (`network`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;")>
Public Class geolite2_country_blocks_ipv6: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("network"), PrimaryKey, NotNull, DataType(MySqlDbType.VarChar, "128"), Column(Name:="network"), XmlAttribute> Public Property network As String
    <DatabaseField("geoname_id"), DataType(MySqlDbType.VarChar, "45"), Column(Name:="geoname_id")> Public Property geoname_id As String
    <DatabaseField("registered_country_geoname_id"), DataType(MySqlDbType.VarChar, "45"), Column(Name:="registered_country_geoname_id")> Public Property registered_country_geoname_id As String
    <DatabaseField("represented_country_geoname_id"), DataType(MySqlDbType.VarChar, "45"), Column(Name:="represented_country_geoname_id")> Public Property represented_country_geoname_id As String
    <DatabaseField("is_anonymous_proxy"), DataType(MySqlDbType.VarChar, "45"), Column(Name:="is_anonymous_proxy")> Public Property is_anonymous_proxy As String
    <DatabaseField("is_satellite_provider"), DataType(MySqlDbType.VarChar, "45"), Column(Name:="is_satellite_provider")> Public Property is_satellite_provider As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `geolite2_country_blocks_ipv6` (`network`, `geoname_id`, `registered_country_geoname_id`, `represented_country_geoname_id`, `is_anonymous_proxy`, `is_satellite_provider`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `geolite2_country_blocks_ipv6` (`network`, `geoname_id`, `registered_country_geoname_id`, `represented_country_geoname_id`, `is_anonymous_proxy`, `is_satellite_provider`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `geolite2_country_blocks_ipv6` (`network`, `geoname_id`, `registered_country_geoname_id`, `represented_country_geoname_id`, `is_anonymous_proxy`, `is_satellite_provider`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `geolite2_country_blocks_ipv6` (`network`, `geoname_id`, `registered_country_geoname_id`, `represented_country_geoname_id`, `is_anonymous_proxy`, `is_satellite_provider`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `geolite2_country_blocks_ipv6` WHERE `network` = '{0}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `geolite2_country_blocks_ipv6` SET `network`='{0}', `geoname_id`='{1}', `registered_country_geoname_id`='{2}', `represented_country_geoname_id`='{3}', `is_anonymous_proxy`='{4}', `is_satellite_provider`='{5}' WHERE `network` = '{6}';</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `geolite2_country_blocks_ipv6` WHERE `network` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, network)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `geolite2_country_blocks_ipv6` (`network`, `geoname_id`, `registered_country_geoname_id`, `represented_country_geoname_id`, `is_anonymous_proxy`, `is_satellite_provider`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, network, geoname_id, registered_country_geoname_id, represented_country_geoname_id, is_anonymous_proxy, is_satellite_provider)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `geolite2_country_blocks_ipv6` (`network`, `geoname_id`, `registered_country_geoname_id`, `represented_country_geoname_id`, `is_anonymous_proxy`, `is_satellite_provider`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, network, geoname_id, registered_country_geoname_id, represented_country_geoname_id, is_anonymous_proxy, is_satellite_provider)
        Else
        Return String.Format(INSERT_SQL, network, geoname_id, registered_country_geoname_id, represented_country_geoname_id, is_anonymous_proxy, is_satellite_provider)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{network}', '{geoname_id}', '{registered_country_geoname_id}', '{represented_country_geoname_id}', '{is_anonymous_proxy}', '{is_satellite_provider}')"
        Else
            Return $"('{network}', '{geoname_id}', '{registered_country_geoname_id}', '{represented_country_geoname_id}', '{is_anonymous_proxy}', '{is_satellite_provider}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `geolite2_country_blocks_ipv6` (`network`, `geoname_id`, `registered_country_geoname_id`, `represented_country_geoname_id`, `is_anonymous_proxy`, `is_satellite_provider`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, network, geoname_id, registered_country_geoname_id, represented_country_geoname_id, is_anonymous_proxy, is_satellite_provider)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `geolite2_country_blocks_ipv6` (`network`, `geoname_id`, `registered_country_geoname_id`, `represented_country_geoname_id`, `is_anonymous_proxy`, `is_satellite_provider`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, network, geoname_id, registered_country_geoname_id, represented_country_geoname_id, is_anonymous_proxy, is_satellite_provider)
        Else
        Return String.Format(REPLACE_SQL, network, geoname_id, registered_country_geoname_id, represented_country_geoname_id, is_anonymous_proxy, is_satellite_provider)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `geolite2_country_blocks_ipv6` SET `network`='{0}', `geoname_id`='{1}', `registered_country_geoname_id`='{2}', `represented_country_geoname_id`='{3}', `is_anonymous_proxy`='{4}', `is_satellite_provider`='{5}' WHERE `network` = '{6}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, network, geoname_id, registered_country_geoname_id, represented_country_geoname_id, is_anonymous_proxy, is_satellite_provider, network)
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As geolite2_country_blocks_ipv6
                         Return DirectCast(MyClass.MemberwiseClone, geolite2_country_blocks_ipv6)
                     End Function
End Class


End Namespace

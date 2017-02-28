#Region "Microsoft.VisualBasic::56dd3b62e6c45e50bb70491c8959f5f9, ..\httpd\WebCloud\smrucc_webcloud_datacenter\visitor_stat.vb"

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

REM  Dump @2016/11/8 17:27:22


Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes

Namespace mysql

''' <summary>
''' ```SQL
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `visitor_stat`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `visitor_stat` (
'''   `uid` int(11) NOT NULL AUTO_INCREMENT,
'''   `time` datetime NOT NULL,
'''   `ip` varchar(45) NOT NULL,
'''   `url` tinytext NOT NULL COMMENT 'Url that going to visit this web site',
'''   `success` int(11) NOT NULL,
'''   `method` varchar(45) DEFAULT NULL COMMENT 'GET/POST/PUT.....',
'''   `ua` varchar(1024) DEFAULT NULL COMMENT 'User agent',
'''   `ref` mediumtext COMMENT 'reference url, Referer',
'''   `data` mediumtext COMMENT 'additional data notes',
'''   `app` int(11) NOT NULL,
'''   PRIMARY KEY (`ip`,`time`),
'''   UNIQUE KEY `uid_UNIQUE` (`uid`)
''' ) ENGINE=InnoDB AUTO_INCREMENT=9717 DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' -- Dumping events for database 'smrucc_webcloud'
''' --
''' 
''' --
''' -- Dumping routines for database 'smrucc_webcloud'
''' --
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
''' -- Dump completed on 2016-11-08 17:27:13
''' 
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("visitor_stat", Database:="smrucc_webcloud", SchemaSQL:="
CREATE TABLE `visitor_stat` (
  `uid` int(11) NOT NULL AUTO_INCREMENT,
  `time` datetime NOT NULL,
  `ip` varchar(45) NOT NULL,
  `url` tinytext NOT NULL COMMENT 'Url that going to visit this web site',
  `success` int(11) NOT NULL,
  `method` varchar(45) DEFAULT NULL COMMENT 'GET/POST/PUT.....',
  `ua` varchar(1024) DEFAULT NULL COMMENT 'User agent',
  `ref` mediumtext COMMENT 'reference url, Referer',
  `data` mediumtext COMMENT 'additional data notes',
  `app` int(11) NOT NULL,
  PRIMARY KEY (`ip`,`time`),
  UNIQUE KEY `uid_UNIQUE` (`uid`)
) ENGINE=InnoDB AUTO_INCREMENT=9717 DEFAULT CHARSET=utf8;")>
Public Class visitor_stat: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("uid"), AutoIncrement, NotNull, DataType(MySqlDbType.Int64, "11")> Public Property uid As Long
    <DatabaseField("time"), PrimaryKey, NotNull, DataType(MySqlDbType.DateTime)> Public Property time As Date
    <DatabaseField("ip"), PrimaryKey, NotNull, DataType(MySqlDbType.VarChar, "45")> Public Property ip As String
''' <summary>
''' Url that going to visit this web site
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
    <DatabaseField("url"), NotNull, DataType(MySqlDbType.Text)> Public Property url As String
    <DatabaseField("success"), NotNull, DataType(MySqlDbType.Int64, "11")> Public Property success As Long
''' <summary>
''' GET/POST/PUT.....
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
    <DatabaseField("method"), DataType(MySqlDbType.VarChar, "45")> Public Property method As String
''' <summary>
''' User agent
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
    <DatabaseField("ua"), DataType(MySqlDbType.VarChar, "1024")> Public Property ua As String
''' <summary>
''' reference url, Referer
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
    <DatabaseField("ref"), DataType(MySqlDbType.Text)> Public Property ref As String
''' <summary>
''' additional data notes
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
    <DatabaseField("data"), DataType(MySqlDbType.Text)> Public Property data As String
    <DatabaseField("app"), NotNull, DataType(MySqlDbType.Int64, "11")> Public Property app As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `visitor_stat` (`time`, `ip`, `url`, `success`, `method`, `ua`, `ref`, `data`, `app`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `visitor_stat` (`time`, `ip`, `url`, `success`, `method`, `ua`, `ref`, `data`, `app`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `visitor_stat` WHERE `ip`='{0}' and `time`='{1}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `visitor_stat` SET `uid`='{0}', `time`='{1}', `ip`='{2}', `url`='{3}', `success`='{4}', `method`='{5}', `ua`='{6}', `ref`='{7}', `data`='{8}', `app`='{9}' WHERE `ip`='{10}' and `time`='{11}';</SQL>
#End Region
''' <summary>
''' ```SQL
''' DELETE FROM `visitor_stat` WHERE `ip`='{0}' and `time`='{1}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, ip, DataType.ToMySqlDateTimeString(time))
    End Function
''' <summary>
''' ```SQL
''' INSERT INTO `visitor_stat` (`time`, `ip`, `url`, `success`, `method`, `ua`, `ref`, `data`, `app`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, DataType.ToMySqlDateTimeString(time), ip, url, success, method, ua, ref, data, app)
    End Function
''' <summary>
''' ```SQL
''' REPLACE INTO `visitor_stat` (`time`, `ip`, `url`, `success`, `method`, `ua`, `ref`, `data`, `app`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, DataType.ToMySqlDateTimeString(time), ip, url, success, method, ua, ref, data, app)
    End Function
''' <summary>
''' ```SQL
''' UPDATE `visitor_stat` SET `uid`='{0}', `time`='{1}', `ip`='{2}', `url`='{3}', `success`='{4}', `method`='{5}', `ua`='{6}', `ref`='{7}', `data`='{8}', `app`='{9}' WHERE `ip`='{10}' and `time`='{11}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, uid, DataType.ToMySqlDateTimeString(time), ip, url, success, method, ua, ref, data, app, ip, DataType.ToMySqlDateTimeString(time))
    End Function
#End Region
End Class


End Namespace

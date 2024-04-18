#Region "Microsoft.VisualBasic::2e75b7c202dfc131b091869eabfa9e0c, WebCloud\SMRUCC.WebCloud.DataCenter\mysql\subscription.vb"

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

    ' Class subscription
    ' 
    '     Properties: active, add_time, app, email, hash
    '                 uid
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

REM  Dump @5/25/2019 3:17:58 PM


Imports System.Data.Linq.Mapping
Imports System.Xml.Serialization
Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes
Imports MySqlScript = Oracle.LinuxCompatibility.MySQL.Scripting.Extensions

Namespace mysql

''' <summary>
''' ```SQL
''' 向订阅了网站更新的用户发送产品的更新信息
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `subscription`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `subscription` (
'''   `uid` int(11) NOT NULL AUTO_INCREMENT,
'''   `email` varchar(128) NOT NULL,
'''   `hash` varchar(64) NOT NULL,
'''   `app` int(11) NOT NULL,
'''   `active` int(11) NOT NULL DEFAULT '0' COMMENT '1(active) OR 0(inactive)',
'''   `add_time` datetime NOT NULL,
'''   PRIMARY KEY (`email`),
'''   UNIQUE KEY `uid_UNIQUE` (`uid`),
'''   KEY `fk_subscription_app1_idx` (`app`),
'''   CONSTRAINT `fk_subscription_app1` FOREIGN KEY (`app`) REFERENCES `app` (`uid`) ON DELETE NO ACTION ON UPDATE NO ACTION
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='向订阅了网站更新的用户发送产品的更新信息';
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("subscription", Database:="smrucc-cloud", SchemaSQL:="
CREATE TABLE `subscription` (
  `uid` int(11) NOT NULL AUTO_INCREMENT,
  `email` varchar(128) NOT NULL,
  `hash` varchar(64) NOT NULL,
  `app` int(11) NOT NULL,
  `active` int(11) NOT NULL DEFAULT '0' COMMENT '1(active) OR 0(inactive)',
  `add_time` datetime NOT NULL,
  PRIMARY KEY (`email`),
  UNIQUE KEY `uid_UNIQUE` (`uid`),
  KEY `fk_subscription_app1_idx` (`app`),
  CONSTRAINT `fk_subscription_app1` FOREIGN KEY (`app`) REFERENCES `app` (`uid`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='向订阅了网站更新的用户发送产品的更新信息';")>
Public Class subscription: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("uid"), AutoIncrement, NotNull, DataType(MySqlDbType.Int64, "11"), Column(Name:="uid")> Public Property uid As Long
    <DatabaseField("email"), PrimaryKey, NotNull, DataType(MySqlDbType.VarChar, "128"), Column(Name:="email"), XmlAttribute> Public Property email As String
    <DatabaseField("hash"), NotNull, DataType(MySqlDbType.VarChar, "64"), Column(Name:="hash")> Public Property hash As String
    <DatabaseField("app"), NotNull, DataType(MySqlDbType.Int64, "11"), Column(Name:="app")> Public Property app As Long
''' <summary>
''' 1(active) OR 0(inactive)
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
    <DatabaseField("active"), NotNull, DataType(MySqlDbType.Int64, "11"), Column(Name:="active")> Public Property active As Long
    <DatabaseField("add_time"), NotNull, DataType(MySqlDbType.DateTime), Column(Name:="add_time")> Public Property add_time As Date
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `subscription` (`email`, `hash`, `app`, `active`, `add_time`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `subscription` (`uid`, `email`, `hash`, `app`, `active`, `add_time`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `subscription` (`email`, `hash`, `app`, `active`, `add_time`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `subscription` (`uid`, `email`, `hash`, `app`, `active`, `add_time`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `subscription` WHERE `email` = '{0}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `subscription` SET `uid`='{0}', `email`='{1}', `hash`='{2}', `app`='{3}', `active`='{4}', `add_time`='{5}' WHERE `email` = '{6}';</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `subscription` WHERE `email` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, email)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `subscription` (`uid`, `email`, `hash`, `app`, `active`, `add_time`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, email, hash, app, active, MySqlScript.ToMySqlDateTimeString(add_time))
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `subscription` (`uid`, `email`, `hash`, `app`, `active`, `add_time`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, uid, email, hash, app, active, MySqlScript.ToMySqlDateTimeString(add_time))
        Else
        Return String.Format(INSERT_SQL, email, hash, app, active, MySqlScript.ToMySqlDateTimeString(add_time))
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{uid}', '{email}', '{hash}', '{app}', '{active}', '{add_time}')"
        Else
            Return $"('{email}', '{hash}', '{app}', '{active}', '{add_time}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `subscription` (`uid`, `email`, `hash`, `app`, `active`, `add_time`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, email, hash, app, active, MySqlScript.ToMySqlDateTimeString(add_time))
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `subscription` (`uid`, `email`, `hash`, `app`, `active`, `add_time`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, uid, email, hash, app, active, MySqlScript.ToMySqlDateTimeString(add_time))
        Else
        Return String.Format(REPLACE_SQL, email, hash, app, active, MySqlScript.ToMySqlDateTimeString(add_time))
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `subscription` SET `uid`='{0}', `email`='{1}', `hash`='{2}', `app`='{3}', `active`='{4}', `add_time`='{5}' WHERE `email` = '{6}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, uid, email, hash, app, active, MySqlScript.ToMySqlDateTimeString(add_time), email)
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As subscription
                         Return DirectCast(MyClass.MemberwiseClone, subscription)
                     End Function
End Class


End Namespace

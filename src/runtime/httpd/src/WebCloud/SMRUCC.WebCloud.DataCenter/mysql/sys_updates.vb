#Region "Microsoft.VisualBasic::c6c01ad005939ad04e6ab4309f2ccce1, WebCloud\SMRUCC.WebCloud.DataCenter\mysql\sys_updates.vb"

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

    ' Class sys_updates
    ' 
    '     Properties: [date], app, details, title, uid
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
''' 网站更新记录
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `sys_updates`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `sys_updates` (
'''   `uid` int(11) NOT NULL AUTO_INCREMENT,
'''   `date` datetime NOT NULL,
'''   `title` varchar(45) NOT NULL,
'''   `details` mediumtext NOT NULL,
'''   `app` int(11) NOT NULL DEFAULT '-1' COMMENT '如果这个字段不为-1，则表示更新的内容为某一个app的内容更新',
'''   PRIMARY KEY (`uid`),
'''   UNIQUE KEY `uid_UNIQUE` (`uid`)
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='网站更新记录';
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("sys_updates", Database:="smrucc-cloud", SchemaSQL:="
CREATE TABLE `sys_updates` (
  `uid` int(11) NOT NULL AUTO_INCREMENT,
  `date` datetime NOT NULL,
  `title` varchar(45) NOT NULL,
  `details` mediumtext NOT NULL,
  `app` int(11) NOT NULL DEFAULT '-1' COMMENT '如果这个字段不为-1，则表示更新的内容为某一个app的内容更新',
  PRIMARY KEY (`uid`),
  UNIQUE KEY `uid_UNIQUE` (`uid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='网站更新记录';")>
Public Class sys_updates: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("uid"), PrimaryKey, AutoIncrement, NotNull, DataType(MySqlDbType.Int64, "11"), Column(Name:="uid"), XmlAttribute> Public Property uid As Long
    <DatabaseField("date"), NotNull, DataType(MySqlDbType.DateTime), Column(Name:="date")> Public Property [date] As Date
    <DatabaseField("title"), NotNull, DataType(MySqlDbType.VarChar, "45"), Column(Name:="title")> Public Property title As String
    <DatabaseField("details"), NotNull, DataType(MySqlDbType.Text), Column(Name:="details")> Public Property details As String
''' <summary>
''' 如果这个字段不为-1，则表示更新的内容为某一个app的内容更新
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
    <DatabaseField("app"), NotNull, DataType(MySqlDbType.Int64, "11"), Column(Name:="app")> Public Property app As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `sys_updates` (`date`, `title`, `details`, `app`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `sys_updates` (`uid`, `date`, `title`, `details`, `app`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `sys_updates` (`date`, `title`, `details`, `app`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `sys_updates` (`uid`, `date`, `title`, `details`, `app`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `sys_updates` WHERE `uid` = '{0}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `sys_updates` SET `uid`='{0}', `date`='{1}', `title`='{2}', `details`='{3}', `app`='{4}' WHERE `uid` = '{5}';</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `sys_updates` WHERE `uid` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, uid)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `sys_updates` (`uid`, `date`, `title`, `details`, `app`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, MySqlScript.ToMySqlDateTimeString([date]), title, details, app)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `sys_updates` (`uid`, `date`, `title`, `details`, `app`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, uid, MySqlScript.ToMySqlDateTimeString([date]), title, details, app)
        Else
        Return String.Format(INSERT_SQL, MySqlScript.ToMySqlDateTimeString([date]), title, details, app)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{uid}', '{[date]}', '{title}', '{details}', '{app}')"
        Else
            Return $"('{[date]}', '{title}', '{details}', '{app}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `sys_updates` (`uid`, `date`, `title`, `details`, `app`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, MySqlScript.ToMySqlDateTimeString([date]), title, details, app)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `sys_updates` (`uid`, `date`, `title`, `details`, `app`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, uid, MySqlScript.ToMySqlDateTimeString([date]), title, details, app)
        Else
        Return String.Format(REPLACE_SQL, MySqlScript.ToMySqlDateTimeString([date]), title, details, app)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `sys_updates` SET `uid`='{0}', `date`='{1}', `title`='{2}', `details`='{3}', `app`='{4}' WHERE `uid` = '{5}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, uid, MySqlScript.ToMySqlDateTimeString([date]), title, details, app, uid)
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As sys_updates
                         Return DirectCast(MyClass.MemberwiseClone, sys_updates)
                     End Function
End Class


End Namespace

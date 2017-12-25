#Region "Microsoft.VisualBasic::b00bbb0cc097090e051145815dd46ad3, ..\httpd\WebCloud\SMRUCC.WebCloud.DataCenter\mysql\app.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
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

REM  Oracle.LinuxCompatibility.MySQL.CodeSolution.VisualBasic.CodeGenerator
REM  MYSQL Schema Mapper
REM      for Microsoft VisualBasic.NET 2.1.0.2569

REM  Dump @2017/9/10 4:06:03


Imports System.Data.Linq.Mapping
Imports System.Xml.Serialization
Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes

Namespace mysql

''' <summary>
''' ```SQL
''' The analysis application that running the task
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `app`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `app` (
'''   `uid` int(11) NOT NULL,
'''   `name` varchar(128) NOT NULL,
'''   `description` longtext COMMENT '功能的详细描述',
'''   `catagory` varchar(45) DEFAULT NULL COMMENT '功能分类',
'''   PRIMARY KEY (`uid`),
'''   UNIQUE KEY `uid_UNIQUE` (`uid`)
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='The analysis application that running the task';
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("app", Database:="smrucc-cloud", SchemaSQL:="
CREATE TABLE `app` (
  `uid` int(11) NOT NULL,
  `name` varchar(128) NOT NULL,
  `description` longtext COMMENT '功能的详细描述',
  `catagory` varchar(45) DEFAULT NULL COMMENT '功能分类',
  PRIMARY KEY (`uid`),
  UNIQUE KEY `uid_UNIQUE` (`uid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='The analysis application that running the task';")>
Public Class app: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("uid"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "11"), Column(Name:="uid"), XmlAttribute> Public Property uid As Long
    <DatabaseField("name"), NotNull, DataType(MySqlDbType.VarChar, "128"), Column(Name:="name")> Public Property name As String
''' <summary>
''' 功能的详细描述
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
    <DatabaseField("description"), DataType(MySqlDbType.Text), Column(Name:="description")> Public Property description As String
''' <summary>
''' 功能分类
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
    <DatabaseField("catagory"), DataType(MySqlDbType.VarChar, "45"), Column(Name:="catagory")> Public Property catagory As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `app` (`uid`, `name`, `description`, `catagory`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `app` (`uid`, `name`, `description`, `catagory`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `app` WHERE `uid` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `app` SET `uid`='{0}', `name`='{1}', `description`='{2}', `catagory`='{3}' WHERE `uid` = '{4}';</SQL>
#End Region
''' <summary>
''' ```SQL
''' DELETE FROM `app` WHERE `uid` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, uid)
    End Function
''' <summary>
''' ```SQL
''' INSERT INTO `app` (`uid`, `name`, `description`, `catagory`) VALUES ('{0}', '{1}', '{2}', '{3}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, uid, name, description, catagory)
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue() As String
        Return $"('{uid}', '{name}', '{description}', '{catagory}')"
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `app` (`uid`, `name`, `description`, `catagory`) VALUES ('{0}', '{1}', '{2}', '{3}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, uid, name, description, catagory)
    End Function
''' <summary>
''' ```SQL
''' UPDATE `app` SET `uid`='{0}', `name`='{1}', `description`='{2}', `catagory`='{3}' WHERE `uid` = '{4}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, uid, name, description, catagory, uid)
    End Function
#End Region
Public Function Clone() As app
                  Return DirectCast(MyClass.MemberwiseClone, app)
              End Function
End Class


End Namespace

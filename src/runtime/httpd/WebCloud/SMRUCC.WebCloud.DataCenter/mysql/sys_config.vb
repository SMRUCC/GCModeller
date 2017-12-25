#Region "Microsoft.VisualBasic::22d08f150a536767f06bbb2ee8fde652, ..\httpd\WebCloud\SMRUCC.WebCloud.DataCenter\mysql\sys_config.vb"

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
''' 系统设置
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `sys_config`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `sys_config` (
'''   `variable` varchar(128) NOT NULL,
'''   `value` varchar(128) DEFAULT NULL,
'''   `set_time` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
'''   `set_by` varchar(128) DEFAULT NULL,
'''   PRIMARY KEY (`variable`)
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='系统设置';
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("sys_config", Database:="smrucc-cloud", SchemaSQL:="
CREATE TABLE `sys_config` (
  `variable` varchar(128) NOT NULL,
  `value` varchar(128) DEFAULT NULL,
  `set_time` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `set_by` varchar(128) DEFAULT NULL,
  PRIMARY KEY (`variable`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='系统设置';")>
Public Class sys_config: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("variable"), PrimaryKey, NotNull, DataType(MySqlDbType.VarChar, "128"), Column(Name:="variable"), XmlAttribute> Public Property variable As String
    <DatabaseField("value"), DataType(MySqlDbType.VarChar, "128"), Column(Name:="value")> Public Property value As String
    <DatabaseField("set_time"), NotNull, DataType(MySqlDbType.DateTime), Column(Name:="set_time")> Public Property set_time As Date
    <DatabaseField("set_by"), DataType(MySqlDbType.VarChar, "128"), Column(Name:="set_by")> Public Property set_by As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `sys_config` (`variable`, `value`, `set_time`, `set_by`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `sys_config` (`variable`, `value`, `set_time`, `set_by`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `sys_config` WHERE `variable` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `sys_config` SET `variable`='{0}', `value`='{1}', `set_time`='{2}', `set_by`='{3}' WHERE `variable` = '{4}';</SQL>
#End Region
''' <summary>
''' ```SQL
''' DELETE FROM `sys_config` WHERE `variable` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, variable)
    End Function
''' <summary>
''' ```SQL
''' INSERT INTO `sys_config` (`variable`, `value`, `set_time`, `set_by`) VALUES ('{0}', '{1}', '{2}', '{3}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, variable, value, DataType.ToMySqlDateTimeString(set_time), set_by)
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue() As String
        Return $"('{variable}', '{value}', '{set_time}', '{set_by}')"
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `sys_config` (`variable`, `value`, `set_time`, `set_by`) VALUES ('{0}', '{1}', '{2}', '{3}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, variable, value, DataType.ToMySqlDateTimeString(set_time), set_by)
    End Function
''' <summary>
''' ```SQL
''' UPDATE `sys_config` SET `variable`='{0}', `value`='{1}', `set_time`='{2}', `set_by`='{3}' WHERE `variable` = '{4}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, variable, value, DataType.ToMySqlDateTimeString(set_time), set_by, variable)
    End Function
#End Region
Public Function Clone() As sys_config
                  Return DirectCast(MyClass.MemberwiseClone, sys_config)
              End Function
End Class


End Namespace

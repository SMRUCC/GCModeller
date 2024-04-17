#Region "Microsoft.VisualBasic::0b3447d4ff2499dd46ec7fe8bc2a1ea5, WebCloud\SMRUCC.WebCloud.DataCenter\mysql\task_errors.vb"

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

    ' Class task_errors
    ' 
    '     Properties: app, exception, solved, stack_trace, task
    '                 type, uid
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
''' Task executing errors log
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `task_errors`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `task_errors` (
'''   `uid` int(11) NOT NULL,
'''   `app` int(11) NOT NULL COMMENT 'The task app name',
'''   `task` int(11) NOT NULL COMMENT 'The task uid',
'''   `exception` longtext NOT NULL COMMENT 'The exception message',
'''   `type` varchar(45) NOT NULL COMMENT 'GetType.ToString',
'''   `stack-trace` varchar(45) NOT NULL,
'''   `solved` int(11) NOT NULL DEFAULT '0' COMMENT '这个bug是否已经解决了？ 默认是0未解决，1为已经解决了',
'''   PRIMARY KEY (`uid`,`app`),
'''   KEY `fk_task_errors_app1_idx` (`app`),
'''   CONSTRAINT `error_task` FOREIGN KEY (`app`) REFERENCES `task_pool` (`uid`) ON DELETE NO ACTION ON UPDATE NO ACTION,
'''   CONSTRAINT `fk_task_errors_app1` FOREIGN KEY (`app`) REFERENCES `app` (`uid`) ON DELETE NO ACTION ON UPDATE NO ACTION
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='Task executing errors log';
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("task_errors", Database:="smrucc-cloud", SchemaSQL:="
CREATE TABLE `task_errors` (
  `uid` int(11) NOT NULL,
  `app` int(11) NOT NULL COMMENT 'The task app name',
  `task` int(11) NOT NULL COMMENT 'The task uid',
  `exception` longtext NOT NULL COMMENT 'The exception message',
  `type` varchar(45) NOT NULL COMMENT 'GetType.ToString',
  `stack-trace` varchar(45) NOT NULL,
  `solved` int(11) NOT NULL DEFAULT '0' COMMENT '这个bug是否已经解决了？ 默认是0未解决，1为已经解决了',
  PRIMARY KEY (`uid`,`app`),
  KEY `fk_task_errors_app1_idx` (`app`),
  CONSTRAINT `error_task` FOREIGN KEY (`app`) REFERENCES `task_pool` (`uid`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `fk_task_errors_app1` FOREIGN KEY (`app`) REFERENCES `app` (`uid`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='Task executing errors log';")>
Public Class task_errors: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("uid"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "11"), Column(Name:="uid"), XmlAttribute> Public Property uid As Long
''' <summary>
''' The task app name
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
    <DatabaseField("app"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "11"), Column(Name:="app"), XmlAttribute> Public Property app As Long
''' <summary>
''' The task uid
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
    <DatabaseField("task"), NotNull, DataType(MySqlDbType.Int64, "11"), Column(Name:="task")> Public Property task As Long
''' <summary>
''' The exception message
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
    <DatabaseField("exception"), NotNull, DataType(MySqlDbType.Text), Column(Name:="exception")> Public Property exception As String
''' <summary>
''' GetType.ToString
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
    <DatabaseField("type"), NotNull, DataType(MySqlDbType.VarChar, "45"), Column(Name:="type")> Public Property type As String
    <DatabaseField("stack-trace"), NotNull, DataType(MySqlDbType.VarChar, "45"), Column(Name:="stack-trace")> Public Property stack_trace As String
''' <summary>
''' 这个bug是否已经解决了？ 默认是0未解决，1为已经解决了
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
    <DatabaseField("solved"), NotNull, DataType(MySqlDbType.Int64, "11"), Column(Name:="solved")> Public Property solved As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `task_errors` (`uid`, `app`, `task`, `exception`, `type`, `stack-trace`, `solved`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `task_errors` (`uid`, `app`, `task`, `exception`, `type`, `stack-trace`, `solved`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `task_errors` (`uid`, `app`, `task`, `exception`, `type`, `stack-trace`, `solved`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `task_errors` (`uid`, `app`, `task`, `exception`, `type`, `stack-trace`, `solved`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `task_errors` WHERE `uid`='{0}' and `app`='{1}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `task_errors` SET `uid`='{0}', `app`='{1}', `task`='{2}', `exception`='{3}', `type`='{4}', `stack-trace`='{5}', `solved`='{6}' WHERE `uid`='{7}' and `app`='{8}';</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `task_errors` WHERE `uid`='{0}' and `app`='{1}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, uid, app)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `task_errors` (`uid`, `app`, `task`, `exception`, `type`, `stack-trace`, `solved`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, uid, app, task, exception, type, stack_trace, solved)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `task_errors` (`uid`, `app`, `task`, `exception`, `type`, `stack-trace`, `solved`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, uid, app, task, exception, type, stack_trace, solved)
        Else
        Return String.Format(INSERT_SQL, uid, app, task, exception, type, stack_trace, solved)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{uid}', '{app}', '{task}', '{exception}', '{type}', '{stack_trace}', '{solved}')"
        Else
            Return $"('{uid}', '{app}', '{task}', '{exception}', '{type}', '{stack_trace}', '{solved}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `task_errors` (`uid`, `app`, `task`, `exception`, `type`, `stack-trace`, `solved`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, uid, app, task, exception, type, stack_trace, solved)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `task_errors` (`uid`, `app`, `task`, `exception`, `type`, `stack-trace`, `solved`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, uid, app, task, exception, type, stack_trace, solved)
        Else
        Return String.Format(REPLACE_SQL, uid, app, task, exception, type, stack_trace, solved)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `task_errors` SET `uid`='{0}', `app`='{1}', `task`='{2}', `exception`='{3}', `type`='{4}', `stack-trace`='{5}', `solved`='{6}' WHERE `uid`='{7}' and `app`='{8}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, uid, app, task, exception, type, stack_trace, solved, uid, app)
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As task_errors
                         Return DirectCast(MyClass.MemberwiseClone, task_errors)
                     End Function
End Class


End Namespace

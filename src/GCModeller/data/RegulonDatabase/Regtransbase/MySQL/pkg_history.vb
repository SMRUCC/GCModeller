#Region "Microsoft.VisualBasic::d14d0bd1414d831e2c0da19ba4df018d, GCModeller\data\RegulonDatabase\Regtransbase\MySQL\pkg_history.vb"

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


    ' Code Statistics:

    '   Total Lines: 182
    '    Code Lines: 95
    ' Comment Lines: 65
    '   Blank Lines: 22
    '     File Size: 11.77 KB


    ' Class pkg_history
    ' 
    '     Properties: event_date, event_operation, pkg_guid, user_by_email, user_by_id
    '                 user_by_name, user_by_phone, user_by_role, user_to_email, user_to_id
    '                 user_to_name, user_to_phone, user_to_role
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

REM  Dump @2018/5/23 13:13:38


Imports System.Data.Linq.Mapping
Imports System.Xml.Serialization
Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes
Imports MySqlScript = Oracle.LinuxCompatibility.MySQL.Scripting.Extensions

Namespace Regtransbase.MySQL

''' <summary>
''' ```SQL
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `pkg_history`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `pkg_history` (
'''   `pkg_guid` int(11) NOT NULL DEFAULT '0',
'''   `event_date` varchar(100) NOT NULL DEFAULT '',
'''   `event_operation` varchar(100) NOT NULL DEFAULT '',
'''   `user_by_id` int(11) DEFAULT '0',
'''   `user_by_name` varchar(100) DEFAULT '',
'''   `user_by_role` varchar(100) DEFAULT '',
'''   `user_by_email` varchar(100) DEFAULT '',
'''   `user_by_phone` varchar(100) DEFAULT '',
'''   `user_to_id` int(11) DEFAULT '0',
'''   `user_to_name` varchar(100) DEFAULT '',
'''   `user_to_role` varchar(100) DEFAULT '',
'''   `user_to_email` varchar(100) DEFAULT '',
'''   `user_to_phone` varchar(100) DEFAULT NULL
''' ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("pkg_history", Database:="dbregulation_update", SchemaSQL:="
CREATE TABLE `pkg_history` (
  `pkg_guid` int(11) NOT NULL DEFAULT '0',
  `event_date` varchar(100) NOT NULL DEFAULT '',
  `event_operation` varchar(100) NOT NULL DEFAULT '',
  `user_by_id` int(11) DEFAULT '0',
  `user_by_name` varchar(100) DEFAULT '',
  `user_by_role` varchar(100) DEFAULT '',
  `user_by_email` varchar(100) DEFAULT '',
  `user_by_phone` varchar(100) DEFAULT '',
  `user_to_id` int(11) DEFAULT '0',
  `user_to_name` varchar(100) DEFAULT '',
  `user_to_role` varchar(100) DEFAULT '',
  `user_to_email` varchar(100) DEFAULT '',
  `user_to_phone` varchar(100) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;")>
Public Class pkg_history: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("pkg_guid"), NotNull, DataType(MySqlDbType.Int64, "11"), Column(Name:="pkg_guid")> Public Property pkg_guid As Long
    <DatabaseField("event_date"), NotNull, DataType(MySqlDbType.VarChar, "100"), Column(Name:="event_date")> Public Property event_date As String
    <DatabaseField("event_operation"), NotNull, DataType(MySqlDbType.VarChar, "100"), Column(Name:="event_operation")> Public Property event_operation As String
    <DatabaseField("user_by_id"), DataType(MySqlDbType.Int64, "11"), Column(Name:="user_by_id")> Public Property user_by_id As Long
    <DatabaseField("user_by_name"), DataType(MySqlDbType.VarChar, "100"), Column(Name:="user_by_name")> Public Property user_by_name As String
    <DatabaseField("user_by_role"), DataType(MySqlDbType.VarChar, "100"), Column(Name:="user_by_role")> Public Property user_by_role As String
    <DatabaseField("user_by_email"), DataType(MySqlDbType.VarChar, "100"), Column(Name:="user_by_email")> Public Property user_by_email As String
    <DatabaseField("user_by_phone"), DataType(MySqlDbType.VarChar, "100"), Column(Name:="user_by_phone")> Public Property user_by_phone As String
    <DatabaseField("user_to_id"), DataType(MySqlDbType.Int64, "11"), Column(Name:="user_to_id")> Public Property user_to_id As Long
    <DatabaseField("user_to_name"), DataType(MySqlDbType.VarChar, "100"), Column(Name:="user_to_name")> Public Property user_to_name As String
    <DatabaseField("user_to_role"), DataType(MySqlDbType.VarChar, "100"), Column(Name:="user_to_role")> Public Property user_to_role As String
    <DatabaseField("user_to_email"), DataType(MySqlDbType.VarChar, "100"), Column(Name:="user_to_email")> Public Property user_to_email As String
    <DatabaseField("user_to_phone"), DataType(MySqlDbType.VarChar, "100"), Column(Name:="user_to_phone")> Public Property user_to_phone As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `pkg_history` (`pkg_guid`, `event_date`, `event_operation`, `user_by_id`, `user_by_name`, `user_by_role`, `user_by_email`, `user_by_phone`, `user_to_id`, `user_to_name`, `user_to_role`, `user_to_email`, `user_to_phone`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `pkg_history` (`pkg_guid`, `event_date`, `event_operation`, `user_by_id`, `user_by_name`, `user_by_role`, `user_by_email`, `user_by_phone`, `user_to_id`, `user_to_name`, `user_to_role`, `user_to_email`, `user_to_phone`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `pkg_history` (`pkg_guid`, `event_date`, `event_operation`, `user_by_id`, `user_by_name`, `user_by_role`, `user_by_email`, `user_by_phone`, `user_to_id`, `user_to_name`, `user_to_role`, `user_to_email`, `user_to_phone`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `pkg_history` (`pkg_guid`, `event_date`, `event_operation`, `user_by_id`, `user_by_name`, `user_by_role`, `user_by_email`, `user_by_phone`, `user_to_id`, `user_to_name`, `user_to_role`, `user_to_email`, `user_to_phone`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `pkg_history` WHERE ;</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `pkg_history` SET `pkg_guid`='{0}', `event_date`='{1}', `event_operation`='{2}', `user_by_id`='{3}', `user_by_name`='{4}', `user_by_role`='{5}', `user_by_email`='{6}', `user_by_phone`='{7}', `user_to_id`='{8}', `user_to_name`='{9}', `user_to_role`='{10}', `user_to_email`='{11}', `user_to_phone`='{12}' WHERE ;</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `pkg_history` WHERE ;
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Throw New NotImplementedException("Table key was Not found, unable To generate ___DELETE_SQL_Invoke automatically, please edit this Function manually!")
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `pkg_history` (`pkg_guid`, `event_date`, `event_operation`, `user_by_id`, `user_by_name`, `user_by_role`, `user_by_email`, `user_by_phone`, `user_to_id`, `user_to_name`, `user_to_role`, `user_to_email`, `user_to_phone`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, pkg_guid, event_date, event_operation, user_by_id, user_by_name, user_by_role, user_by_email, user_by_phone, user_to_id, user_to_name, user_to_role, user_to_email, user_to_phone)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `pkg_history` (`pkg_guid`, `event_date`, `event_operation`, `user_by_id`, `user_by_name`, `user_by_role`, `user_by_email`, `user_by_phone`, `user_to_id`, `user_to_name`, `user_to_role`, `user_to_email`, `user_to_phone`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, pkg_guid, event_date, event_operation, user_by_id, user_by_name, user_by_role, user_by_email, user_by_phone, user_to_id, user_to_name, user_to_role, user_to_email, user_to_phone)
        Else
        Return String.Format(INSERT_SQL, pkg_guid, event_date, event_operation, user_by_id, user_by_name, user_by_role, user_by_email, user_by_phone, user_to_id, user_to_name, user_to_role, user_to_email, user_to_phone)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{pkg_guid}', '{event_date}', '{event_operation}', '{user_by_id}', '{user_by_name}', '{user_by_role}', '{user_by_email}', '{user_by_phone}', '{user_to_id}', '{user_to_name}', '{user_to_role}', '{user_to_email}', '{user_to_phone}')"
        Else
            Return $"('{pkg_guid}', '{event_date}', '{event_operation}', '{user_by_id}', '{user_by_name}', '{user_by_role}', '{user_by_email}', '{user_by_phone}', '{user_to_id}', '{user_to_name}', '{user_to_role}', '{user_to_email}', '{user_to_phone}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `pkg_history` (`pkg_guid`, `event_date`, `event_operation`, `user_by_id`, `user_by_name`, `user_by_role`, `user_by_email`, `user_by_phone`, `user_to_id`, `user_to_name`, `user_to_role`, `user_to_email`, `user_to_phone`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, pkg_guid, event_date, event_operation, user_by_id, user_by_name, user_by_role, user_by_email, user_by_phone, user_to_id, user_to_name, user_to_role, user_to_email, user_to_phone)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `pkg_history` (`pkg_guid`, `event_date`, `event_operation`, `user_by_id`, `user_by_name`, `user_by_role`, `user_by_email`, `user_by_phone`, `user_to_id`, `user_to_name`, `user_to_role`, `user_to_email`, `user_to_phone`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, pkg_guid, event_date, event_operation, user_by_id, user_by_name, user_by_role, user_by_email, user_by_phone, user_to_id, user_to_name, user_to_role, user_to_email, user_to_phone)
        Else
        Return String.Format(REPLACE_SQL, pkg_guid, event_date, event_operation, user_by_id, user_by_name, user_by_role, user_by_email, user_by_phone, user_to_id, user_to_name, user_to_role, user_to_email, user_to_phone)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `pkg_history` SET `pkg_guid`='{0}', `event_date`='{1}', `event_operation`='{2}', `user_by_id`='{3}', `user_by_name`='{4}', `user_by_role`='{5}', `user_by_email`='{6}', `user_by_phone`='{7}', `user_to_id`='{8}', `user_to_name`='{9}', `user_to_role`='{10}', `user_to_email`='{11}', `user_to_phone`='{12}' WHERE ;
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Throw New NotImplementedException("Table key was Not found, unable To generate ___UPDATE_SQL_Invoke automatically, please edit this Function manually!")
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As pkg_history
                         Return DirectCast(MyClass.MemberwiseClone, pkg_history)
                     End Function
End Class


End Namespace

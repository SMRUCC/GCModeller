#Region "Microsoft.VisualBasic::8c345c3fad3c7695f0daebd52481e46b, GCModeller\data\RegulonDatabase\RegulonDB\MySQL\regulon_d_tmp.vb"

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

    '   Total Lines: 158
    '    Code Lines: 79
    ' Comment Lines: 57
    '   Blank Lines: 22
    '     File Size: 7.15 KB


    ' Class regulon_d_tmp
    ' 
    '     Properties: re_id, regulon_id, regulon_key_id_org, regulon_name, regulon_tf_group
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

REM  Dump @2018/5/23 13:13:36


Imports System.Data.Linq.Mapping
Imports System.Xml.Serialization
Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes
Imports MySqlScript = Oracle.LinuxCompatibility.MySQL.Scripting.Extensions

Namespace RegulonDB.Tables

''' <summary>
''' ```SQL
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `regulon_d_tmp`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `regulon_d_tmp` (
'''   `re_id` decimal(10,0) NOT NULL,
'''   `regulon_id` char(12) NOT NULL,
'''   `regulon_name` varchar(500) DEFAULT NULL,
'''   `regulon_key_id_org` char(5) NOT NULL,
'''   `regulon_tf_group` decimal(10,0) NOT NULL
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("regulon_d_tmp", Database:="regulondb_93", SchemaSQL:="
CREATE TABLE `regulon_d_tmp` (
  `re_id` decimal(10,0) NOT NULL,
  `regulon_id` char(12) NOT NULL,
  `regulon_name` varchar(500) DEFAULT NULL,
  `regulon_key_id_org` char(5) NOT NULL,
  `regulon_tf_group` decimal(10,0) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;")>
Public Class regulon_d_tmp: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("re_id"), NotNull, DataType(MySqlDbType.Decimal), Column(Name:="re_id")> Public Property re_id As Decimal
    <DatabaseField("regulon_id"), NotNull, DataType(MySqlDbType.VarChar, "12"), Column(Name:="regulon_id")> Public Property regulon_id As String
    <DatabaseField("regulon_name"), DataType(MySqlDbType.VarChar, "500"), Column(Name:="regulon_name")> Public Property regulon_name As String
    <DatabaseField("regulon_key_id_org"), NotNull, DataType(MySqlDbType.VarChar, "5"), Column(Name:="regulon_key_id_org")> Public Property regulon_key_id_org As String
    <DatabaseField("regulon_tf_group"), NotNull, DataType(MySqlDbType.Decimal), Column(Name:="regulon_tf_group")> Public Property regulon_tf_group As Decimal
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `regulon_d_tmp` (`re_id`, `regulon_id`, `regulon_name`, `regulon_key_id_org`, `regulon_tf_group`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `regulon_d_tmp` (`re_id`, `regulon_id`, `regulon_name`, `regulon_key_id_org`, `regulon_tf_group`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `regulon_d_tmp` (`re_id`, `regulon_id`, `regulon_name`, `regulon_key_id_org`, `regulon_tf_group`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `regulon_d_tmp` (`re_id`, `regulon_id`, `regulon_name`, `regulon_key_id_org`, `regulon_tf_group`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `regulon_d_tmp` WHERE ;</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `regulon_d_tmp` SET `re_id`='{0}', `regulon_id`='{1}', `regulon_name`='{2}', `regulon_key_id_org`='{3}', `regulon_tf_group`='{4}' WHERE ;</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `regulon_d_tmp` WHERE ;
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Throw New NotImplementedException("Table key was Not found, unable To generate ___DELETE_SQL_Invoke automatically, please edit this Function manually!")
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `regulon_d_tmp` (`re_id`, `regulon_id`, `regulon_name`, `regulon_key_id_org`, `regulon_tf_group`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, re_id, regulon_id, regulon_name, regulon_key_id_org, regulon_tf_group)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `regulon_d_tmp` (`re_id`, `regulon_id`, `regulon_name`, `regulon_key_id_org`, `regulon_tf_group`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, re_id, regulon_id, regulon_name, regulon_key_id_org, regulon_tf_group)
        Else
        Return String.Format(INSERT_SQL, re_id, regulon_id, regulon_name, regulon_key_id_org, regulon_tf_group)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{re_id}', '{regulon_id}', '{regulon_name}', '{regulon_key_id_org}', '{regulon_tf_group}')"
        Else
            Return $"('{re_id}', '{regulon_id}', '{regulon_name}', '{regulon_key_id_org}', '{regulon_tf_group}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `regulon_d_tmp` (`re_id`, `regulon_id`, `regulon_name`, `regulon_key_id_org`, `regulon_tf_group`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, re_id, regulon_id, regulon_name, regulon_key_id_org, regulon_tf_group)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `regulon_d_tmp` (`re_id`, `regulon_id`, `regulon_name`, `regulon_key_id_org`, `regulon_tf_group`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, re_id, regulon_id, regulon_name, regulon_key_id_org, regulon_tf_group)
        Else
        Return String.Format(REPLACE_SQL, re_id, regulon_id, regulon_name, regulon_key_id_org, regulon_tf_group)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `regulon_d_tmp` SET `re_id`='{0}', `regulon_id`='{1}', `regulon_name`='{2}', `regulon_key_id_org`='{3}', `regulon_tf_group`='{4}' WHERE ;
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Throw New NotImplementedException("Table key was Not found, unable To generate ___UPDATE_SQL_Invoke automatically, please edit this Function manually!")
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As regulon_d_tmp
                         Return DirectCast(MyClass.MemberwiseClone, regulon_d_tmp)
                     End Function
End Class


End Namespace

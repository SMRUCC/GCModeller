#Region "Microsoft.VisualBasic::558d993d8b0792c603f066713464b310, GCModeller\data\RegulonDatabase\RegulonDB\MySQL\method.vb"

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

    '   Total Lines: 164
    '    Code Lines: 83
    ' Comment Lines: 59
    '   Blank Lines: 22
    '     File Size: 8.42 KB


    ' Class method
    ' 
    '     Properties: key_id_org, method_description, method_id, method_internal_comment, method_name
    '                 method_note, parameter_used
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
''' DROP TABLE IF EXISTS `method`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `method` (
'''   `method_id` char(12) NOT NULL,
'''   `method_name` varchar(200) NOT NULL,
'''   `method_description` varchar(2000) DEFAULT NULL,
'''   `parameter_used` varchar(2000) DEFAULT NULL,
'''   `method_note` varchar(2000) DEFAULT NULL,
'''   `method_internal_comment` longtext,
'''   `key_id_org` varchar(5) NOT NULL
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("method", Database:="regulondb_93", SchemaSQL:="
CREATE TABLE `method` (
  `method_id` char(12) NOT NULL,
  `method_name` varchar(200) NOT NULL,
  `method_description` varchar(2000) DEFAULT NULL,
  `parameter_used` varchar(2000) DEFAULT NULL,
  `method_note` varchar(2000) DEFAULT NULL,
  `method_internal_comment` longtext,
  `key_id_org` varchar(5) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;")>
Public Class method: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("method_id"), NotNull, DataType(MySqlDbType.VarChar, "12"), Column(Name:="method_id")> Public Property method_id As String
    <DatabaseField("method_name"), NotNull, DataType(MySqlDbType.VarChar, "200"), Column(Name:="method_name")> Public Property method_name As String
    <DatabaseField("method_description"), DataType(MySqlDbType.VarChar, "2000"), Column(Name:="method_description")> Public Property method_description As String
    <DatabaseField("parameter_used"), DataType(MySqlDbType.VarChar, "2000"), Column(Name:="parameter_used")> Public Property parameter_used As String
    <DatabaseField("method_note"), DataType(MySqlDbType.VarChar, "2000"), Column(Name:="method_note")> Public Property method_note As String
    <DatabaseField("method_internal_comment"), DataType(MySqlDbType.Text), Column(Name:="method_internal_comment")> Public Property method_internal_comment As String
    <DatabaseField("key_id_org"), NotNull, DataType(MySqlDbType.VarChar, "5"), Column(Name:="key_id_org")> Public Property key_id_org As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `method` (`method_id`, `method_name`, `method_description`, `parameter_used`, `method_note`, `method_internal_comment`, `key_id_org`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `method` (`method_id`, `method_name`, `method_description`, `parameter_used`, `method_note`, `method_internal_comment`, `key_id_org`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `method` (`method_id`, `method_name`, `method_description`, `parameter_used`, `method_note`, `method_internal_comment`, `key_id_org`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `method` (`method_id`, `method_name`, `method_description`, `parameter_used`, `method_note`, `method_internal_comment`, `key_id_org`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `method` WHERE ;</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `method` SET `method_id`='{0}', `method_name`='{1}', `method_description`='{2}', `parameter_used`='{3}', `method_note`='{4}', `method_internal_comment`='{5}', `key_id_org`='{6}' WHERE ;</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `method` WHERE ;
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Throw New NotImplementedException("Table key was Not found, unable To generate ___DELETE_SQL_Invoke automatically, please edit this Function manually!")
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `method` (`method_id`, `method_name`, `method_description`, `parameter_used`, `method_note`, `method_internal_comment`, `key_id_org`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, method_id, method_name, method_description, parameter_used, method_note, method_internal_comment, key_id_org)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `method` (`method_id`, `method_name`, `method_description`, `parameter_used`, `method_note`, `method_internal_comment`, `key_id_org`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, method_id, method_name, method_description, parameter_used, method_note, method_internal_comment, key_id_org)
        Else
        Return String.Format(INSERT_SQL, method_id, method_name, method_description, parameter_used, method_note, method_internal_comment, key_id_org)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{method_id}', '{method_name}', '{method_description}', '{parameter_used}', '{method_note}', '{method_internal_comment}', '{key_id_org}')"
        Else
            Return $"('{method_id}', '{method_name}', '{method_description}', '{parameter_used}', '{method_note}', '{method_internal_comment}', '{key_id_org}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `method` (`method_id`, `method_name`, `method_description`, `parameter_used`, `method_note`, `method_internal_comment`, `key_id_org`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, method_id, method_name, method_description, parameter_used, method_note, method_internal_comment, key_id_org)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `method` (`method_id`, `method_name`, `method_description`, `parameter_used`, `method_note`, `method_internal_comment`, `key_id_org`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, method_id, method_name, method_description, parameter_used, method_note, method_internal_comment, key_id_org)
        Else
        Return String.Format(REPLACE_SQL, method_id, method_name, method_description, parameter_used, method_note, method_internal_comment, key_id_org)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `method` SET `method_id`='{0}', `method_name`='{1}', `method_description`='{2}', `parameter_used`='{3}', `method_note`='{4}', `method_internal_comment`='{5}', `key_id_org`='{6}' WHERE ;
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Throw New NotImplementedException("Table key was Not found, unable To generate ___UPDATE_SQL_Invoke automatically, please edit this Function manually!")
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As method
                         Return DirectCast(MyClass.MemberwiseClone, method)
                     End Function
End Class


End Namespace

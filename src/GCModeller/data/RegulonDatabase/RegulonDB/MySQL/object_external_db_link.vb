#Region "Microsoft.VisualBasic::4e2db9de04c7270205d505d3dbe633c4, GCModeller\data\RegulonDatabase\RegulonDB\MySQL\object_external_db_link.vb"

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

    '   Total Lines: 152
    '    Code Lines: 75
    ' Comment Lines: 55
    '   Blank Lines: 22
    '     File Size: 6.22 KB


    ' Class object_external_db_link
    ' 
    '     Properties: ext_reference_id, external_db_id, object_id
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
''' DROP TABLE IF EXISTS `object_external_db_link`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `object_external_db_link` (
'''   `object_id` char(12) NOT NULL,
'''   `external_db_id` char(12) NOT NULL,
'''   `ext_reference_id` varchar(255) NOT NULL
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("object_external_db_link", Database:="regulondb_93", SchemaSQL:="
CREATE TABLE `object_external_db_link` (
  `object_id` char(12) NOT NULL,
  `external_db_id` char(12) NOT NULL,
  `ext_reference_id` varchar(255) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;")>
Public Class object_external_db_link: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("object_id"), NotNull, DataType(MySqlDbType.VarChar, "12"), Column(Name:="object_id")> Public Property object_id As String
    <DatabaseField("external_db_id"), NotNull, DataType(MySqlDbType.VarChar, "12"), Column(Name:="external_db_id")> Public Property external_db_id As String
    <DatabaseField("ext_reference_id"), NotNull, DataType(MySqlDbType.VarChar, "255"), Column(Name:="ext_reference_id")> Public Property ext_reference_id As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `object_external_db_link` (`object_id`, `external_db_id`, `ext_reference_id`) VALUES ('{0}', '{1}', '{2}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `object_external_db_link` (`object_id`, `external_db_id`, `ext_reference_id`) VALUES ('{0}', '{1}', '{2}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `object_external_db_link` (`object_id`, `external_db_id`, `ext_reference_id`) VALUES ('{0}', '{1}', '{2}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `object_external_db_link` (`object_id`, `external_db_id`, `ext_reference_id`) VALUES ('{0}', '{1}', '{2}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `object_external_db_link` WHERE ;</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `object_external_db_link` SET `object_id`='{0}', `external_db_id`='{1}', `ext_reference_id`='{2}' WHERE ;</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `object_external_db_link` WHERE ;
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Throw New NotImplementedException("Table key was Not found, unable To generate ___DELETE_SQL_Invoke automatically, please edit this Function manually!")
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `object_external_db_link` (`object_id`, `external_db_id`, `ext_reference_id`) VALUES ('{0}', '{1}', '{2}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, object_id, external_db_id, ext_reference_id)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `object_external_db_link` (`object_id`, `external_db_id`, `ext_reference_id`) VALUES ('{0}', '{1}', '{2}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, object_id, external_db_id, ext_reference_id)
        Else
        Return String.Format(INSERT_SQL, object_id, external_db_id, ext_reference_id)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{object_id}', '{external_db_id}', '{ext_reference_id}')"
        Else
            Return $"('{object_id}', '{external_db_id}', '{ext_reference_id}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `object_external_db_link` (`object_id`, `external_db_id`, `ext_reference_id`) VALUES ('{0}', '{1}', '{2}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, object_id, external_db_id, ext_reference_id)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `object_external_db_link` (`object_id`, `external_db_id`, `ext_reference_id`) VALUES ('{0}', '{1}', '{2}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, object_id, external_db_id, ext_reference_id)
        Else
        Return String.Format(REPLACE_SQL, object_id, external_db_id, ext_reference_id)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `object_external_db_link` SET `object_id`='{0}', `external_db_id`='{1}', `ext_reference_id`='{2}' WHERE ;
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Throw New NotImplementedException("Table key was Not found, unable To generate ___UPDATE_SQL_Invoke automatically, please edit this Function manually!")
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As object_external_db_link
                         Return DirectCast(MyClass.MemberwiseClone, object_external_db_link)
                     End Function
End Class


End Namespace

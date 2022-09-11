#Region "Microsoft.VisualBasic::5fa9de908e713d62b6daf0f3c3f1dbac, GCModeller\data\RegulonDatabase\RegulonDB\MySQL\intermediate.vb"

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
    '     File Size: 6.25 KB


    ' Class intermediate
    ' 
    '     Properties: intermediate_id, intermediate_name, intermediate_note
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
''' DROP TABLE IF EXISTS `intermediate`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `intermediate` (
'''   `intermediate_id` char(12) NOT NULL,
'''   `intermediate_name` varchar(255) NOT NULL,
'''   `intermediate_note` varchar(2000) DEFAULT NULL
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("intermediate", Database:="regulondb_93", SchemaSQL:="
CREATE TABLE `intermediate` (
  `intermediate_id` char(12) NOT NULL,
  `intermediate_name` varchar(255) NOT NULL,
  `intermediate_note` varchar(2000) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;")>
Public Class intermediate: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("intermediate_id"), NotNull, DataType(MySqlDbType.VarChar, "12"), Column(Name:="intermediate_id")> Public Property intermediate_id As String
    <DatabaseField("intermediate_name"), NotNull, DataType(MySqlDbType.VarChar, "255"), Column(Name:="intermediate_name")> Public Property intermediate_name As String
    <DatabaseField("intermediate_note"), DataType(MySqlDbType.VarChar, "2000"), Column(Name:="intermediate_note")> Public Property intermediate_note As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `intermediate` (`intermediate_id`, `intermediate_name`, `intermediate_note`) VALUES ('{0}', '{1}', '{2}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `intermediate` (`intermediate_id`, `intermediate_name`, `intermediate_note`) VALUES ('{0}', '{1}', '{2}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `intermediate` (`intermediate_id`, `intermediate_name`, `intermediate_note`) VALUES ('{0}', '{1}', '{2}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `intermediate` (`intermediate_id`, `intermediate_name`, `intermediate_note`) VALUES ('{0}', '{1}', '{2}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `intermediate` WHERE ;</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `intermediate` SET `intermediate_id`='{0}', `intermediate_name`='{1}', `intermediate_note`='{2}' WHERE ;</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `intermediate` WHERE ;
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Throw New NotImplementedException("Table key was Not found, unable To generate ___DELETE_SQL_Invoke automatically, please edit this Function manually!")
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `intermediate` (`intermediate_id`, `intermediate_name`, `intermediate_note`) VALUES ('{0}', '{1}', '{2}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, intermediate_id, intermediate_name, intermediate_note)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `intermediate` (`intermediate_id`, `intermediate_name`, `intermediate_note`) VALUES ('{0}', '{1}', '{2}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, intermediate_id, intermediate_name, intermediate_note)
        Else
        Return String.Format(INSERT_SQL, intermediate_id, intermediate_name, intermediate_note)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{intermediate_id}', '{intermediate_name}', '{intermediate_note}')"
        Else
            Return $"('{intermediate_id}', '{intermediate_name}', '{intermediate_note}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `intermediate` (`intermediate_id`, `intermediate_name`, `intermediate_note`) VALUES ('{0}', '{1}', '{2}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, intermediate_id, intermediate_name, intermediate_note)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `intermediate` (`intermediate_id`, `intermediate_name`, `intermediate_note`) VALUES ('{0}', '{1}', '{2}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, intermediate_id, intermediate_name, intermediate_note)
        Else
        Return String.Format(REPLACE_SQL, intermediate_id, intermediate_name, intermediate_note)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `intermediate` SET `intermediate_id`='{0}', `intermediate_name`='{1}', `intermediate_note`='{2}' WHERE ;
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Throw New NotImplementedException("Table key was Not found, unable To generate ___UPDATE_SQL_Invoke automatically, please edit this Function manually!")
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As intermediate
                         Return DirectCast(MyClass.MemberwiseClone, intermediate)
                     End Function
End Class


End Namespace

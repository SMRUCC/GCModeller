#Region "Microsoft.VisualBasic::31aa5720d482c696ff705707f1ff432d, GCModeller\data\RegulonDatabase\RegulonDB\MySQL\reaction_component_link.vb"

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

    '   Total Lines: 155
    '    Code Lines: 77
    ' Comment Lines: 56
    '   Blank Lines: 22
    '     File Size: 6.73 KB


    ' Class reaction_component_link
    ' 
    '     Properties: component_id, reaction_component_id, reaction_id, role
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
''' DROP TABLE IF EXISTS `reaction_component_link`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `reaction_component_link` (
'''   `reaction_component_id` char(12) NOT NULL,
'''   `reaction_id` char(12) NOT NULL,
'''   `component_id` char(12) NOT NULL,
'''   `role` varchar(100) NOT NULL
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("reaction_component_link", Database:="regulondb_93", SchemaSQL:="
CREATE TABLE `reaction_component_link` (
  `reaction_component_id` char(12) NOT NULL,
  `reaction_id` char(12) NOT NULL,
  `component_id` char(12) NOT NULL,
  `role` varchar(100) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;")>
Public Class reaction_component_link: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("reaction_component_id"), NotNull, DataType(MySqlDbType.VarChar, "12"), Column(Name:="reaction_component_id")> Public Property reaction_component_id As String
    <DatabaseField("reaction_id"), NotNull, DataType(MySqlDbType.VarChar, "12"), Column(Name:="reaction_id")> Public Property reaction_id As String
    <DatabaseField("component_id"), NotNull, DataType(MySqlDbType.VarChar, "12"), Column(Name:="component_id")> Public Property component_id As String
    <DatabaseField("role"), NotNull, DataType(MySqlDbType.VarChar, "100"), Column(Name:="role")> Public Property role As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `reaction_component_link` (`reaction_component_id`, `reaction_id`, `component_id`, `role`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `reaction_component_link` (`reaction_component_id`, `reaction_id`, `component_id`, `role`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `reaction_component_link` (`reaction_component_id`, `reaction_id`, `component_id`, `role`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `reaction_component_link` (`reaction_component_id`, `reaction_id`, `component_id`, `role`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `reaction_component_link` WHERE ;</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `reaction_component_link` SET `reaction_component_id`='{0}', `reaction_id`='{1}', `component_id`='{2}', `role`='{3}' WHERE ;</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `reaction_component_link` WHERE ;
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Throw New NotImplementedException("Table key was Not found, unable To generate ___DELETE_SQL_Invoke automatically, please edit this Function manually!")
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `reaction_component_link` (`reaction_component_id`, `reaction_id`, `component_id`, `role`) VALUES ('{0}', '{1}', '{2}', '{3}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, reaction_component_id, reaction_id, component_id, role)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `reaction_component_link` (`reaction_component_id`, `reaction_id`, `component_id`, `role`) VALUES ('{0}', '{1}', '{2}', '{3}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, reaction_component_id, reaction_id, component_id, role)
        Else
        Return String.Format(INSERT_SQL, reaction_component_id, reaction_id, component_id, role)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{reaction_component_id}', '{reaction_id}', '{component_id}', '{role}')"
        Else
            Return $"('{reaction_component_id}', '{reaction_id}', '{component_id}', '{role}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `reaction_component_link` (`reaction_component_id`, `reaction_id`, `component_id`, `role`) VALUES ('{0}', '{1}', '{2}', '{3}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, reaction_component_id, reaction_id, component_id, role)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `reaction_component_link` (`reaction_component_id`, `reaction_id`, `component_id`, `role`) VALUES ('{0}', '{1}', '{2}', '{3}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, reaction_component_id, reaction_id, component_id, role)
        Else
        Return String.Format(REPLACE_SQL, reaction_component_id, reaction_id, component_id, role)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `reaction_component_link` SET `reaction_component_id`='{0}', `reaction_id`='{1}', `component_id`='{2}', `role`='{3}' WHERE ;
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Throw New NotImplementedException("Table key was Not found, unable To generate ___UPDATE_SQL_Invoke automatically, please edit this Function manually!")
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As reaction_component_link
                         Return DirectCast(MyClass.MemberwiseClone, reaction_component_link)
                     End Function
End Class


End Namespace

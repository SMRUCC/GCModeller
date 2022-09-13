#Region "Microsoft.VisualBasic::59806d17f8a8048b96e3f13aeb3d3dd9, GCModeller\data\RegulonDatabase\RegulonDB\MySQL\gu_reaction_link.vb"

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
    '     File Size: 6.53 KB


    ' Class gu_reaction_link
    ' 
    '     Properties: gu_id, reaction_id, reaction_number, reaction_order
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
''' DROP TABLE IF EXISTS `gu_reaction_link`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `gu_reaction_link` (
'''   `gu_id` char(12) NOT NULL,
'''   `reaction_id` char(12) NOT NULL,
'''   `reaction_number` varchar(50) NOT NULL,
'''   `reaction_order` decimal(10,0) DEFAULT NULL
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("gu_reaction_link", Database:="regulondb_93", SchemaSQL:="
CREATE TABLE `gu_reaction_link` (
  `gu_id` char(12) NOT NULL,
  `reaction_id` char(12) NOT NULL,
  `reaction_number` varchar(50) NOT NULL,
  `reaction_order` decimal(10,0) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;")>
Public Class gu_reaction_link: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("gu_id"), NotNull, DataType(MySqlDbType.VarChar, "12"), Column(Name:="gu_id")> Public Property gu_id As String
    <DatabaseField("reaction_id"), NotNull, DataType(MySqlDbType.VarChar, "12"), Column(Name:="reaction_id")> Public Property reaction_id As String
    <DatabaseField("reaction_number"), NotNull, DataType(MySqlDbType.VarChar, "50"), Column(Name:="reaction_number")> Public Property reaction_number As String
    <DatabaseField("reaction_order"), DataType(MySqlDbType.Decimal), Column(Name:="reaction_order")> Public Property reaction_order As Decimal
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `gu_reaction_link` (`gu_id`, `reaction_id`, `reaction_number`, `reaction_order`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `gu_reaction_link` (`gu_id`, `reaction_id`, `reaction_number`, `reaction_order`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `gu_reaction_link` (`gu_id`, `reaction_id`, `reaction_number`, `reaction_order`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `gu_reaction_link` (`gu_id`, `reaction_id`, `reaction_number`, `reaction_order`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `gu_reaction_link` WHERE ;</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `gu_reaction_link` SET `gu_id`='{0}', `reaction_id`='{1}', `reaction_number`='{2}', `reaction_order`='{3}' WHERE ;</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `gu_reaction_link` WHERE ;
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Throw New NotImplementedException("Table key was Not found, unable To generate ___DELETE_SQL_Invoke automatically, please edit this Function manually!")
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `gu_reaction_link` (`gu_id`, `reaction_id`, `reaction_number`, `reaction_order`) VALUES ('{0}', '{1}', '{2}', '{3}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, gu_id, reaction_id, reaction_number, reaction_order)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `gu_reaction_link` (`gu_id`, `reaction_id`, `reaction_number`, `reaction_order`) VALUES ('{0}', '{1}', '{2}', '{3}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, gu_id, reaction_id, reaction_number, reaction_order)
        Else
        Return String.Format(INSERT_SQL, gu_id, reaction_id, reaction_number, reaction_order)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{gu_id}', '{reaction_id}', '{reaction_number}', '{reaction_order}')"
        Else
            Return $"('{gu_id}', '{reaction_id}', '{reaction_number}', '{reaction_order}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `gu_reaction_link` (`gu_id`, `reaction_id`, `reaction_number`, `reaction_order`) VALUES ('{0}', '{1}', '{2}', '{3}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, gu_id, reaction_id, reaction_number, reaction_order)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `gu_reaction_link` (`gu_id`, `reaction_id`, `reaction_number`, `reaction_order`) VALUES ('{0}', '{1}', '{2}', '{3}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, gu_id, reaction_id, reaction_number, reaction_order)
        Else
        Return String.Format(REPLACE_SQL, gu_id, reaction_id, reaction_number, reaction_order)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `gu_reaction_link` SET `gu_id`='{0}', `reaction_id`='{1}', `reaction_number`='{2}', `reaction_order`='{3}' WHERE ;
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Throw New NotImplementedException("Table key was Not found, unable To generate ___UPDATE_SQL_Invoke automatically, please edit this Function manually!")
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As gu_reaction_link
                         Return DirectCast(MyClass.MemberwiseClone, gu_reaction_link)
                     End Function
End Class


End Namespace

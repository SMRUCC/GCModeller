#Region "Microsoft.VisualBasic::91232b6ff76db953e92351a79f70e8fb, GCModeller\data\RegulonDatabase\RegulonDB\MySQL\cond_effect_link.vb"

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
    '     File Size: 7.37 KB


    ' Class cond_effect_link
    ' 
    '     Properties: cond_effect_link_id, cond_effect_link_notes, condition_id, effect, medium_id
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
''' DROP TABLE IF EXISTS `cond_effect_link`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `cond_effect_link` (
'''   `cond_effect_link_id` char(12) NOT NULL,
'''   `condition_id` char(12) NOT NULL,
'''   `medium_id` char(12) NOT NULL,
'''   `effect` varchar(250) NOT NULL,
'''   `cond_effect_link_notes` varchar(2000) DEFAULT NULL
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("cond_effect_link", Database:="regulondb_93", SchemaSQL:="
CREATE TABLE `cond_effect_link` (
  `cond_effect_link_id` char(12) NOT NULL,
  `condition_id` char(12) NOT NULL,
  `medium_id` char(12) NOT NULL,
  `effect` varchar(250) NOT NULL,
  `cond_effect_link_notes` varchar(2000) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;")>
Public Class cond_effect_link: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("cond_effect_link_id"), NotNull, DataType(MySqlDbType.VarChar, "12"), Column(Name:="cond_effect_link_id")> Public Property cond_effect_link_id As String
    <DatabaseField("condition_id"), NotNull, DataType(MySqlDbType.VarChar, "12"), Column(Name:="condition_id")> Public Property condition_id As String
    <DatabaseField("medium_id"), NotNull, DataType(MySqlDbType.VarChar, "12"), Column(Name:="medium_id")> Public Property medium_id As String
    <DatabaseField("effect"), NotNull, DataType(MySqlDbType.VarChar, "250"), Column(Name:="effect")> Public Property effect As String
    <DatabaseField("cond_effect_link_notes"), DataType(MySqlDbType.VarChar, "2000"), Column(Name:="cond_effect_link_notes")> Public Property cond_effect_link_notes As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `cond_effect_link` (`cond_effect_link_id`, `condition_id`, `medium_id`, `effect`, `cond_effect_link_notes`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `cond_effect_link` (`cond_effect_link_id`, `condition_id`, `medium_id`, `effect`, `cond_effect_link_notes`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `cond_effect_link` (`cond_effect_link_id`, `condition_id`, `medium_id`, `effect`, `cond_effect_link_notes`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `cond_effect_link` (`cond_effect_link_id`, `condition_id`, `medium_id`, `effect`, `cond_effect_link_notes`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `cond_effect_link` WHERE ;</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `cond_effect_link` SET `cond_effect_link_id`='{0}', `condition_id`='{1}', `medium_id`='{2}', `effect`='{3}', `cond_effect_link_notes`='{4}' WHERE ;</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `cond_effect_link` WHERE ;
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Throw New NotImplementedException("Table key was Not found, unable To generate ___DELETE_SQL_Invoke automatically, please edit this Function manually!")
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `cond_effect_link` (`cond_effect_link_id`, `condition_id`, `medium_id`, `effect`, `cond_effect_link_notes`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, cond_effect_link_id, condition_id, medium_id, effect, cond_effect_link_notes)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `cond_effect_link` (`cond_effect_link_id`, `condition_id`, `medium_id`, `effect`, `cond_effect_link_notes`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, cond_effect_link_id, condition_id, medium_id, effect, cond_effect_link_notes)
        Else
        Return String.Format(INSERT_SQL, cond_effect_link_id, condition_id, medium_id, effect, cond_effect_link_notes)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{cond_effect_link_id}', '{condition_id}', '{medium_id}', '{effect}', '{cond_effect_link_notes}')"
        Else
            Return $"('{cond_effect_link_id}', '{condition_id}', '{medium_id}', '{effect}', '{cond_effect_link_notes}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `cond_effect_link` (`cond_effect_link_id`, `condition_id`, `medium_id`, `effect`, `cond_effect_link_notes`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, cond_effect_link_id, condition_id, medium_id, effect, cond_effect_link_notes)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `cond_effect_link` (`cond_effect_link_id`, `condition_id`, `medium_id`, `effect`, `cond_effect_link_notes`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, cond_effect_link_id, condition_id, medium_id, effect, cond_effect_link_notes)
        Else
        Return String.Format(REPLACE_SQL, cond_effect_link_id, condition_id, medium_id, effect, cond_effect_link_notes)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `cond_effect_link` SET `cond_effect_link_id`='{0}', `condition_id`='{1}', `medium_id`='{2}', `effect`='{3}', `cond_effect_link_notes`='{4}' WHERE ;
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Throw New NotImplementedException("Table key was Not found, unable To generate ___UPDATE_SQL_Invoke automatically, please edit this Function manually!")
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As cond_effect_link
                         Return DirectCast(MyClass.MemberwiseClone, cond_effect_link)
                     End Function
End Class


End Namespace

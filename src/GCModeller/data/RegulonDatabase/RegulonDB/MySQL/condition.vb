#Region "Microsoft.VisualBasic::28e06710b83b72610233af7688b7efac, GCModeller\data\RegulonDatabase\RegulonDB\MySQL\condition.vb"

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
    '     File Size: 8.60 KB


    ' Class condition
    ' 
    '     Properties: condition_global, condition_id, condition_notes, control_condition, control_details
    '                 exp_condition, exp_details
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
''' DROP TABLE IF EXISTS `condition`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `condition` (
'''   `condition_id` char(12) NOT NULL,
'''   `control_condition` varchar(2000) NOT NULL,
'''   `control_details` varchar(2000) DEFAULT NULL,
'''   `exp_condition` varchar(2000) NOT NULL,
'''   `exp_details` varchar(2000) DEFAULT NULL,
'''   `condition_global` varchar(2000) DEFAULT NULL,
'''   `condition_notes` varchar(2000) DEFAULT NULL
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("condition", Database:="regulondb_93", SchemaSQL:="
CREATE TABLE `condition` (
  `condition_id` char(12) NOT NULL,
  `control_condition` varchar(2000) NOT NULL,
  `control_details` varchar(2000) DEFAULT NULL,
  `exp_condition` varchar(2000) NOT NULL,
  `exp_details` varchar(2000) DEFAULT NULL,
  `condition_global` varchar(2000) DEFAULT NULL,
  `condition_notes` varchar(2000) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;")>
Public Class condition: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("condition_id"), NotNull, DataType(MySqlDbType.VarChar, "12"), Column(Name:="condition_id")> Public Property condition_id As String
    <DatabaseField("control_condition"), NotNull, DataType(MySqlDbType.VarChar, "2000"), Column(Name:="control_condition")> Public Property control_condition As String
    <DatabaseField("control_details"), DataType(MySqlDbType.VarChar, "2000"), Column(Name:="control_details")> Public Property control_details As String
    <DatabaseField("exp_condition"), NotNull, DataType(MySqlDbType.VarChar, "2000"), Column(Name:="exp_condition")> Public Property exp_condition As String
    <DatabaseField("exp_details"), DataType(MySqlDbType.VarChar, "2000"), Column(Name:="exp_details")> Public Property exp_details As String
    <DatabaseField("condition_global"), DataType(MySqlDbType.VarChar, "2000"), Column(Name:="condition_global")> Public Property condition_global As String
    <DatabaseField("condition_notes"), DataType(MySqlDbType.VarChar, "2000"), Column(Name:="condition_notes")> Public Property condition_notes As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `condition` (`condition_id`, `control_condition`, `control_details`, `exp_condition`, `exp_details`, `condition_global`, `condition_notes`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `condition` (`condition_id`, `control_condition`, `control_details`, `exp_condition`, `exp_details`, `condition_global`, `condition_notes`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `condition` (`condition_id`, `control_condition`, `control_details`, `exp_condition`, `exp_details`, `condition_global`, `condition_notes`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `condition` (`condition_id`, `control_condition`, `control_details`, `exp_condition`, `exp_details`, `condition_global`, `condition_notes`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `condition` WHERE ;</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `condition` SET `condition_id`='{0}', `control_condition`='{1}', `control_details`='{2}', `exp_condition`='{3}', `exp_details`='{4}', `condition_global`='{5}', `condition_notes`='{6}' WHERE ;</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `condition` WHERE ;
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Throw New NotImplementedException("Table key was Not found, unable To generate ___DELETE_SQL_Invoke automatically, please edit this Function manually!")
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `condition` (`condition_id`, `control_condition`, `control_details`, `exp_condition`, `exp_details`, `condition_global`, `condition_notes`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, condition_id, control_condition, control_details, exp_condition, exp_details, condition_global, condition_notes)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `condition` (`condition_id`, `control_condition`, `control_details`, `exp_condition`, `exp_details`, `condition_global`, `condition_notes`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, condition_id, control_condition, control_details, exp_condition, exp_details, condition_global, condition_notes)
        Else
        Return String.Format(INSERT_SQL, condition_id, control_condition, control_details, exp_condition, exp_details, condition_global, condition_notes)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{condition_id}', '{control_condition}', '{control_details}', '{exp_condition}', '{exp_details}', '{condition_global}', '{condition_notes}')"
        Else
            Return $"('{condition_id}', '{control_condition}', '{control_details}', '{exp_condition}', '{exp_details}', '{condition_global}', '{condition_notes}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `condition` (`condition_id`, `control_condition`, `control_details`, `exp_condition`, `exp_details`, `condition_global`, `condition_notes`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, condition_id, control_condition, control_details, exp_condition, exp_details, condition_global, condition_notes)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `condition` (`condition_id`, `control_condition`, `control_details`, `exp_condition`, `exp_details`, `condition_global`, `condition_notes`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, condition_id, control_condition, control_details, exp_condition, exp_details, condition_global, condition_notes)
        Else
        Return String.Format(REPLACE_SQL, condition_id, control_condition, control_details, exp_condition, exp_details, condition_global, condition_notes)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `condition` SET `condition_id`='{0}', `control_condition`='{1}', `control_details`='{2}', `exp_condition`='{3}', `exp_details`='{4}', `condition_global`='{5}', `condition_notes`='{6}' WHERE ;
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Throw New NotImplementedException("Table key was Not found, unable To generate ___UPDATE_SQL_Invoke automatically, please edit this Function manually!")
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As condition
                         Return DirectCast(MyClass.MemberwiseClone, condition)
                     End Function
End Class


End Namespace

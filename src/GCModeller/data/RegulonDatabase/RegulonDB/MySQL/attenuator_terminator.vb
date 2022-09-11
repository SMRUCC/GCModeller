#Region "Microsoft.VisualBasic::e94690f21daf75f7d1f37cc1447db686, GCModeller\data\RegulonDatabase\RegulonDB\MySQL\attenuator_terminator.vb"

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
    '     File Size: 9.72 KB


    ' Class attenuator_terminator
    ' 
    '     Properties: a_terminator_attenuator_id, a_terminator_energy, a_terminator_id, a_terminator_posleft, a_terminator_posright
    '                 a_terminator_sequence, a_terminator_type
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
''' DROP TABLE IF EXISTS `attenuator_terminator`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `attenuator_terminator` (
'''   `a_terminator_id` varchar(12) NOT NULL,
'''   `a_terminator_type` varchar(25) DEFAULT NULL,
'''   `a_terminator_posleft` decimal(10,0) DEFAULT NULL,
'''   `a_terminator_posright` decimal(10,0) DEFAULT NULL,
'''   `a_terminator_energy` decimal(7,2) DEFAULT NULL,
'''   `a_terminator_sequence` varchar(200) DEFAULT NULL,
'''   `a_terminator_attenuator_id` varchar(12) DEFAULT NULL
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("attenuator_terminator", Database:="regulondb_93", SchemaSQL:="
CREATE TABLE `attenuator_terminator` (
  `a_terminator_id` varchar(12) NOT NULL,
  `a_terminator_type` varchar(25) DEFAULT NULL,
  `a_terminator_posleft` decimal(10,0) DEFAULT NULL,
  `a_terminator_posright` decimal(10,0) DEFAULT NULL,
  `a_terminator_energy` decimal(7,2) DEFAULT NULL,
  `a_terminator_sequence` varchar(200) DEFAULT NULL,
  `a_terminator_attenuator_id` varchar(12) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;")>
Public Class attenuator_terminator: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("a_terminator_id"), NotNull, DataType(MySqlDbType.VarChar, "12"), Column(Name:="a_terminator_id")> Public Property a_terminator_id As String
    <DatabaseField("a_terminator_type"), DataType(MySqlDbType.VarChar, "25"), Column(Name:="a_terminator_type")> Public Property a_terminator_type As String
    <DatabaseField("a_terminator_posleft"), DataType(MySqlDbType.Decimal), Column(Name:="a_terminator_posleft")> Public Property a_terminator_posleft As Decimal
    <DatabaseField("a_terminator_posright"), DataType(MySqlDbType.Decimal), Column(Name:="a_terminator_posright")> Public Property a_terminator_posright As Decimal
    <DatabaseField("a_terminator_energy"), DataType(MySqlDbType.Decimal), Column(Name:="a_terminator_energy")> Public Property a_terminator_energy As Decimal
    <DatabaseField("a_terminator_sequence"), DataType(MySqlDbType.VarChar, "200"), Column(Name:="a_terminator_sequence")> Public Property a_terminator_sequence As String
    <DatabaseField("a_terminator_attenuator_id"), DataType(MySqlDbType.VarChar, "12"), Column(Name:="a_terminator_attenuator_id")> Public Property a_terminator_attenuator_id As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `attenuator_terminator` (`a_terminator_id`, `a_terminator_type`, `a_terminator_posleft`, `a_terminator_posright`, `a_terminator_energy`, `a_terminator_sequence`, `a_terminator_attenuator_id`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `attenuator_terminator` (`a_terminator_id`, `a_terminator_type`, `a_terminator_posleft`, `a_terminator_posright`, `a_terminator_energy`, `a_terminator_sequence`, `a_terminator_attenuator_id`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `attenuator_terminator` (`a_terminator_id`, `a_terminator_type`, `a_terminator_posleft`, `a_terminator_posright`, `a_terminator_energy`, `a_terminator_sequence`, `a_terminator_attenuator_id`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `attenuator_terminator` (`a_terminator_id`, `a_terminator_type`, `a_terminator_posleft`, `a_terminator_posright`, `a_terminator_energy`, `a_terminator_sequence`, `a_terminator_attenuator_id`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `attenuator_terminator` WHERE ;</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `attenuator_terminator` SET `a_terminator_id`='{0}', `a_terminator_type`='{1}', `a_terminator_posleft`='{2}', `a_terminator_posright`='{3}', `a_terminator_energy`='{4}', `a_terminator_sequence`='{5}', `a_terminator_attenuator_id`='{6}' WHERE ;</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `attenuator_terminator` WHERE ;
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Throw New NotImplementedException("Table key was Not found, unable To generate ___DELETE_SQL_Invoke automatically, please edit this Function manually!")
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `attenuator_terminator` (`a_terminator_id`, `a_terminator_type`, `a_terminator_posleft`, `a_terminator_posright`, `a_terminator_energy`, `a_terminator_sequence`, `a_terminator_attenuator_id`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, a_terminator_id, a_terminator_type, a_terminator_posleft, a_terminator_posright, a_terminator_energy, a_terminator_sequence, a_terminator_attenuator_id)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `attenuator_terminator` (`a_terminator_id`, `a_terminator_type`, `a_terminator_posleft`, `a_terminator_posright`, `a_terminator_energy`, `a_terminator_sequence`, `a_terminator_attenuator_id`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, a_terminator_id, a_terminator_type, a_terminator_posleft, a_terminator_posright, a_terminator_energy, a_terminator_sequence, a_terminator_attenuator_id)
        Else
        Return String.Format(INSERT_SQL, a_terminator_id, a_terminator_type, a_terminator_posleft, a_terminator_posright, a_terminator_energy, a_terminator_sequence, a_terminator_attenuator_id)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{a_terminator_id}', '{a_terminator_type}', '{a_terminator_posleft}', '{a_terminator_posright}', '{a_terminator_energy}', '{a_terminator_sequence}', '{a_terminator_attenuator_id}')"
        Else
            Return $"('{a_terminator_id}', '{a_terminator_type}', '{a_terminator_posleft}', '{a_terminator_posright}', '{a_terminator_energy}', '{a_terminator_sequence}', '{a_terminator_attenuator_id}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `attenuator_terminator` (`a_terminator_id`, `a_terminator_type`, `a_terminator_posleft`, `a_terminator_posright`, `a_terminator_energy`, `a_terminator_sequence`, `a_terminator_attenuator_id`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, a_terminator_id, a_terminator_type, a_terminator_posleft, a_terminator_posright, a_terminator_energy, a_terminator_sequence, a_terminator_attenuator_id)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `attenuator_terminator` (`a_terminator_id`, `a_terminator_type`, `a_terminator_posleft`, `a_terminator_posright`, `a_terminator_energy`, `a_terminator_sequence`, `a_terminator_attenuator_id`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, a_terminator_id, a_terminator_type, a_terminator_posleft, a_terminator_posright, a_terminator_energy, a_terminator_sequence, a_terminator_attenuator_id)
        Else
        Return String.Format(REPLACE_SQL, a_terminator_id, a_terminator_type, a_terminator_posleft, a_terminator_posright, a_terminator_energy, a_terminator_sequence, a_terminator_attenuator_id)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `attenuator_terminator` SET `a_terminator_id`='{0}', `a_terminator_type`='{1}', `a_terminator_posleft`='{2}', `a_terminator_posright`='{3}', `a_terminator_energy`='{4}', `a_terminator_sequence`='{5}', `a_terminator_attenuator_id`='{6}' WHERE ;
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Throw New NotImplementedException("Table key was Not found, unable To generate ___UPDATE_SQL_Invoke automatically, please edit this Function manually!")
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As attenuator_terminator
                         Return DirectCast(MyClass.MemberwiseClone, attenuator_terminator)
                     End Function
End Class


End Namespace

#Region "Microsoft.VisualBasic::811d96d338740d4586e8b7a17e36e9e4, GCModeller\data\RegulonDatabase\RegulonDB\MySQL\regulator.vb"

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

    '   Total Lines: 161
    '    Code Lines: 81
    ' Comment Lines: 58
    '   Blank Lines: 22
    '     File Size: 7.92 KB


    ' Class regulator
    ' 
    '     Properties: key_id_org, product_id, regulator_id, regulator_internal_commnet, regulator_name
    '                 regulator_note
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
''' DROP TABLE IF EXISTS `regulator`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `regulator` (
'''   `regulator_id` varchar(12) NOT NULL,
'''   `product_id` char(12) DEFAULT NULL,
'''   `regulator_name` varchar(250) DEFAULT NULL,
'''   `regulator_internal_commnet` varchar(2000) DEFAULT NULL,
'''   `regulator_note` longtext,
'''   `key_id_org` varchar(5) NOT NULL
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("regulator", Database:="regulondb_93", SchemaSQL:="
CREATE TABLE `regulator` (
  `regulator_id` varchar(12) NOT NULL,
  `product_id` char(12) DEFAULT NULL,
  `regulator_name` varchar(250) DEFAULT NULL,
  `regulator_internal_commnet` varchar(2000) DEFAULT NULL,
  `regulator_note` longtext,
  `key_id_org` varchar(5) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;")>
Public Class regulator: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("regulator_id"), NotNull, DataType(MySqlDbType.VarChar, "12"), Column(Name:="regulator_id")> Public Property regulator_id As String
    <DatabaseField("product_id"), DataType(MySqlDbType.VarChar, "12"), Column(Name:="product_id")> Public Property product_id As String
    <DatabaseField("regulator_name"), DataType(MySqlDbType.VarChar, "250"), Column(Name:="regulator_name")> Public Property regulator_name As String
    <DatabaseField("regulator_internal_commnet"), DataType(MySqlDbType.VarChar, "2000"), Column(Name:="regulator_internal_commnet")> Public Property regulator_internal_commnet As String
    <DatabaseField("regulator_note"), DataType(MySqlDbType.Text), Column(Name:="regulator_note")> Public Property regulator_note As String
    <DatabaseField("key_id_org"), NotNull, DataType(MySqlDbType.VarChar, "5"), Column(Name:="key_id_org")> Public Property key_id_org As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `regulator` (`regulator_id`, `product_id`, `regulator_name`, `regulator_internal_commnet`, `regulator_note`, `key_id_org`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `regulator` (`regulator_id`, `product_id`, `regulator_name`, `regulator_internal_commnet`, `regulator_note`, `key_id_org`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `regulator` (`regulator_id`, `product_id`, `regulator_name`, `regulator_internal_commnet`, `regulator_note`, `key_id_org`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `regulator` (`regulator_id`, `product_id`, `regulator_name`, `regulator_internal_commnet`, `regulator_note`, `key_id_org`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `regulator` WHERE ;</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `regulator` SET `regulator_id`='{0}', `product_id`='{1}', `regulator_name`='{2}', `regulator_internal_commnet`='{3}', `regulator_note`='{4}', `key_id_org`='{5}' WHERE ;</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `regulator` WHERE ;
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Throw New NotImplementedException("Table key was Not found, unable To generate ___DELETE_SQL_Invoke automatically, please edit this Function manually!")
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `regulator` (`regulator_id`, `product_id`, `regulator_name`, `regulator_internal_commnet`, `regulator_note`, `key_id_org`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, regulator_id, product_id, regulator_name, regulator_internal_commnet, regulator_note, key_id_org)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `regulator` (`regulator_id`, `product_id`, `regulator_name`, `regulator_internal_commnet`, `regulator_note`, `key_id_org`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, regulator_id, product_id, regulator_name, regulator_internal_commnet, regulator_note, key_id_org)
        Else
        Return String.Format(INSERT_SQL, regulator_id, product_id, regulator_name, regulator_internal_commnet, regulator_note, key_id_org)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{regulator_id}', '{product_id}', '{regulator_name}', '{regulator_internal_commnet}', '{regulator_note}', '{key_id_org}')"
        Else
            Return $"('{regulator_id}', '{product_id}', '{regulator_name}', '{regulator_internal_commnet}', '{regulator_note}', '{key_id_org}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `regulator` (`regulator_id`, `product_id`, `regulator_name`, `regulator_internal_commnet`, `regulator_note`, `key_id_org`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, regulator_id, product_id, regulator_name, regulator_internal_commnet, regulator_note, key_id_org)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `regulator` (`regulator_id`, `product_id`, `regulator_name`, `regulator_internal_commnet`, `regulator_note`, `key_id_org`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, regulator_id, product_id, regulator_name, regulator_internal_commnet, regulator_note, key_id_org)
        Else
        Return String.Format(REPLACE_SQL, regulator_id, product_id, regulator_name, regulator_internal_commnet, regulator_note, key_id_org)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `regulator` SET `regulator_id`='{0}', `product_id`='{1}', `regulator_name`='{2}', `regulator_internal_commnet`='{3}', `regulator_note`='{4}', `key_id_org`='{5}' WHERE ;
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Throw New NotImplementedException("Table key was Not found, unable To generate ___UPDATE_SQL_Invoke automatically, please edit this Function manually!")
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As regulator
                         Return DirectCast(MyClass.MemberwiseClone, regulator)
                     End Function
End Class


End Namespace

#Region "Microsoft.VisualBasic::a2cd732de86f8d8b67fca5e50d0a3a2a, DataMySql\Interpro\Tables\uniprot_taxonomy.vb"

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

    ' Class uniprot_taxonomy
    ' 
    '     Properties: left_number, protein_ac, right_number, tax_id
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

REM  Dump @2018/5/23 13:13:37


Imports System.Data.Linq.Mapping
Imports System.Xml.Serialization
Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes
Imports MySqlScript = Oracle.LinuxCompatibility.MySQL.Scripting.Extensions

Namespace Interpro.Tables

''' <summary>
''' ```SQL
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `uniprot_taxonomy`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `uniprot_taxonomy` (
'''   `protein_ac` varchar(10) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
'''   `tax_id` bigint(15) NOT NULL,
'''   `left_number` bigint(15) NOT NULL,
'''   `right_number` bigint(15) NOT NULL
''' ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("uniprot_taxonomy", Database:="interpro", SchemaSQL:="
CREATE TABLE `uniprot_taxonomy` (
  `protein_ac` varchar(10) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `tax_id` bigint(15) NOT NULL,
  `left_number` bigint(15) NOT NULL,
  `right_number` bigint(15) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;")>
Public Class uniprot_taxonomy: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("protein_ac"), NotNull, DataType(MySqlDbType.VarChar, "10"), Column(Name:="protein_ac")> Public Property protein_ac As String
    <DatabaseField("tax_id"), NotNull, DataType(MySqlDbType.Int64, "15"), Column(Name:="tax_id")> Public Property tax_id As Long
    <DatabaseField("left_number"), NotNull, DataType(MySqlDbType.Int64, "15"), Column(Name:="left_number")> Public Property left_number As Long
    <DatabaseField("right_number"), NotNull, DataType(MySqlDbType.Int64, "15"), Column(Name:="right_number")> Public Property right_number As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `uniprot_taxonomy` (`protein_ac`, `tax_id`, `left_number`, `right_number`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `uniprot_taxonomy` (`protein_ac`, `tax_id`, `left_number`, `right_number`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `uniprot_taxonomy` (`protein_ac`, `tax_id`, `left_number`, `right_number`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `uniprot_taxonomy` (`protein_ac`, `tax_id`, `left_number`, `right_number`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `uniprot_taxonomy` WHERE ;</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `uniprot_taxonomy` SET `protein_ac`='{0}', `tax_id`='{1}', `left_number`='{2}', `right_number`='{3}' WHERE ;</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `uniprot_taxonomy` WHERE ;
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Throw New NotImplementedException("Table key was Not found, unable To generate ___DELETE_SQL_Invoke automatically, please edit this Function manually!")
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `uniprot_taxonomy` (`protein_ac`, `tax_id`, `left_number`, `right_number`) VALUES ('{0}', '{1}', '{2}', '{3}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, protein_ac, tax_id, left_number, right_number)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `uniprot_taxonomy` (`protein_ac`, `tax_id`, `left_number`, `right_number`) VALUES ('{0}', '{1}', '{2}', '{3}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, protein_ac, tax_id, left_number, right_number)
        Else
        Return String.Format(INSERT_SQL, protein_ac, tax_id, left_number, right_number)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{protein_ac}', '{tax_id}', '{left_number}', '{right_number}')"
        Else
            Return $"('{protein_ac}', '{tax_id}', '{left_number}', '{right_number}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `uniprot_taxonomy` (`protein_ac`, `tax_id`, `left_number`, `right_number`) VALUES ('{0}', '{1}', '{2}', '{3}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, protein_ac, tax_id, left_number, right_number)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `uniprot_taxonomy` (`protein_ac`, `tax_id`, `left_number`, `right_number`) VALUES ('{0}', '{1}', '{2}', '{3}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, protein_ac, tax_id, left_number, right_number)
        Else
        Return String.Format(REPLACE_SQL, protein_ac, tax_id, left_number, right_number)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `uniprot_taxonomy` SET `protein_ac`='{0}', `tax_id`='{1}', `left_number`='{2}', `right_number`='{3}' WHERE ;
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Throw New NotImplementedException("Table key was Not found, unable To generate ___UPDATE_SQL_Invoke automatically, please edit this Function manually!")
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As uniprot_taxonomy
                         Return DirectCast(MyClass.MemberwiseClone, uniprot_taxonomy)
                     End Function
End Class


End Namespace

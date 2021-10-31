#Region "Microsoft.VisualBasic::c5274889dd09ef58131a1d0121fac482, DataMySql\Interpro\Tables\taxonomy2protein.vb"

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

    ' Class taxonomy2protein
    ' 
    '     Properties: hierarchy, protein_ac, tax_id, tax_name_concat
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
''' DROP TABLE IF EXISTS `taxonomy2protein`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `taxonomy2protein` (
'''   `protein_ac` varchar(6) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
'''   `tax_id` bigint(15) NOT NULL,
'''   `hierarchy` varchar(200) CHARACTER SET latin1 COLLATE latin1_bin DEFAULT NULL,
'''   `tax_name_concat` varchar(80) CHARACTER SET latin1 COLLATE latin1_bin DEFAULT NULL,
'''   PRIMARY KEY (`protein_ac`,`tax_id`),
'''   CONSTRAINT `fk_taxonomy2protein$p` FOREIGN KEY (`protein_ac`) REFERENCES `protein` (`protein_ac`) ON DELETE CASCADE ON UPDATE NO ACTION
''' ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("taxonomy2protein", Database:="interpro", SchemaSQL:="
CREATE TABLE `taxonomy2protein` (
  `protein_ac` varchar(6) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `tax_id` bigint(15) NOT NULL,
  `hierarchy` varchar(200) CHARACTER SET latin1 COLLATE latin1_bin DEFAULT NULL,
  `tax_name_concat` varchar(80) CHARACTER SET latin1 COLLATE latin1_bin DEFAULT NULL,
  PRIMARY KEY (`protein_ac`,`tax_id`),
  CONSTRAINT `fk_taxonomy2protein$p` FOREIGN KEY (`protein_ac`) REFERENCES `protein` (`protein_ac`) ON DELETE CASCADE ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=latin1;")>
Public Class taxonomy2protein: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("protein_ac"), PrimaryKey, NotNull, DataType(MySqlDbType.VarChar, "6"), Column(Name:="protein_ac"), XmlAttribute> Public Property protein_ac As String
    <DatabaseField("tax_id"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "15"), Column(Name:="tax_id"), XmlAttribute> Public Property tax_id As Long
    <DatabaseField("hierarchy"), DataType(MySqlDbType.VarChar, "200"), Column(Name:="hierarchy")> Public Property hierarchy As String
    <DatabaseField("tax_name_concat"), DataType(MySqlDbType.VarChar, "80"), Column(Name:="tax_name_concat")> Public Property tax_name_concat As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `taxonomy2protein` (`protein_ac`, `tax_id`, `hierarchy`, `tax_name_concat`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `taxonomy2protein` (`protein_ac`, `tax_id`, `hierarchy`, `tax_name_concat`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `taxonomy2protein` (`protein_ac`, `tax_id`, `hierarchy`, `tax_name_concat`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `taxonomy2protein` (`protein_ac`, `tax_id`, `hierarchy`, `tax_name_concat`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `taxonomy2protein` WHERE `protein_ac`='{0}' and `tax_id`='{1}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `taxonomy2protein` SET `protein_ac`='{0}', `tax_id`='{1}', `hierarchy`='{2}', `tax_name_concat`='{3}' WHERE `protein_ac`='{4}' and `tax_id`='{5}';</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `taxonomy2protein` WHERE `protein_ac`='{0}' and `tax_id`='{1}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, protein_ac, tax_id)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `taxonomy2protein` (`protein_ac`, `tax_id`, `hierarchy`, `tax_name_concat`) VALUES ('{0}', '{1}', '{2}', '{3}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, protein_ac, tax_id, hierarchy, tax_name_concat)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `taxonomy2protein` (`protein_ac`, `tax_id`, `hierarchy`, `tax_name_concat`) VALUES ('{0}', '{1}', '{2}', '{3}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, protein_ac, tax_id, hierarchy, tax_name_concat)
        Else
        Return String.Format(INSERT_SQL, protein_ac, tax_id, hierarchy, tax_name_concat)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{protein_ac}', '{tax_id}', '{hierarchy}', '{tax_name_concat}')"
        Else
            Return $"('{protein_ac}', '{tax_id}', '{hierarchy}', '{tax_name_concat}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `taxonomy2protein` (`protein_ac`, `tax_id`, `hierarchy`, `tax_name_concat`) VALUES ('{0}', '{1}', '{2}', '{3}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, protein_ac, tax_id, hierarchy, tax_name_concat)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `taxonomy2protein` (`protein_ac`, `tax_id`, `hierarchy`, `tax_name_concat`) VALUES ('{0}', '{1}', '{2}', '{3}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, protein_ac, tax_id, hierarchy, tax_name_concat)
        Else
        Return String.Format(REPLACE_SQL, protein_ac, tax_id, hierarchy, tax_name_concat)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `taxonomy2protein` SET `protein_ac`='{0}', `tax_id`='{1}', `hierarchy`='{2}', `tax_name_concat`='{3}' WHERE `protein_ac`='{4}' and `tax_id`='{5}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, protein_ac, tax_id, hierarchy, tax_name_concat, protein_ac, tax_id)
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As taxonomy2protein
                         Return DirectCast(MyClass.MemberwiseClone, taxonomy2protein)
                     End Function
End Class


End Namespace

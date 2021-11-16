#Region "Microsoft.VisualBasic::3a6fafff8aa28d93cf6097603fd7ea3c, DataMySql\Interpro\Tables\intact_data.vb"

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

    ' Class intact_data
    ' 
    '     Properties: entry_ac, intact_id, interacts_with, protein_ac, pubmed_id
    '                 type, undetermined, uniprot_id
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
''' DROP TABLE IF EXISTS `intact_data`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `intact_data` (
'''   `uniprot_id` varchar(20) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
'''   `protein_ac` varchar(30) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
'''   `undetermined` char(1) CHARACTER SET latin1 COLLATE latin1_bin DEFAULT NULL,
'''   `intact_id` varchar(30) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
'''   `interacts_with` varchar(30) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
'''   `type` varchar(20) CHARACTER SET latin1 COLLATE latin1_bin DEFAULT NULL,
'''   `entry_ac` varchar(30) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
'''   `pubmed_id` varchar(30) CHARACTER SET latin1 COLLATE latin1_bin DEFAULT NULL,
'''   PRIMARY KEY (`entry_ac`,`intact_id`,`interacts_with`,`protein_ac`),
'''   CONSTRAINT `fk_intact_data$entry_ac` FOREIGN KEY (`entry_ac`) REFERENCES `entry` (`entry_ac`) ON DELETE CASCADE ON UPDATE NO ACTION
''' ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("intact_data", Database:="interpro", SchemaSQL:="
CREATE TABLE `intact_data` (
  `uniprot_id` varchar(20) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `protein_ac` varchar(30) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `undetermined` char(1) CHARACTER SET latin1 COLLATE latin1_bin DEFAULT NULL,
  `intact_id` varchar(30) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `interacts_with` varchar(30) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `type` varchar(20) CHARACTER SET latin1 COLLATE latin1_bin DEFAULT NULL,
  `entry_ac` varchar(30) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `pubmed_id` varchar(30) CHARACTER SET latin1 COLLATE latin1_bin DEFAULT NULL,
  PRIMARY KEY (`entry_ac`,`intact_id`,`interacts_with`,`protein_ac`),
  CONSTRAINT `fk_intact_data$entry_ac` FOREIGN KEY (`entry_ac`) REFERENCES `entry` (`entry_ac`) ON DELETE CASCADE ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=latin1;")>
Public Class intact_data: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("uniprot_id"), NotNull, DataType(MySqlDbType.VarChar, "20"), Column(Name:="uniprot_id")> Public Property uniprot_id As String
    <DatabaseField("protein_ac"), PrimaryKey, NotNull, DataType(MySqlDbType.VarChar, "30"), Column(Name:="protein_ac"), XmlAttribute> Public Property protein_ac As String
    <DatabaseField("undetermined"), DataType(MySqlDbType.VarChar, "1"), Column(Name:="undetermined")> Public Property undetermined As String
    <DatabaseField("intact_id"), PrimaryKey, NotNull, DataType(MySqlDbType.VarChar, "30"), Column(Name:="intact_id"), XmlAttribute> Public Property intact_id As String
    <DatabaseField("interacts_with"), PrimaryKey, NotNull, DataType(MySqlDbType.VarChar, "30"), Column(Name:="interacts_with"), XmlAttribute> Public Property interacts_with As String
    <DatabaseField("type"), DataType(MySqlDbType.VarChar, "20"), Column(Name:="type")> Public Property type As String
    <DatabaseField("entry_ac"), PrimaryKey, NotNull, DataType(MySqlDbType.VarChar, "30"), Column(Name:="entry_ac"), XmlAttribute> Public Property entry_ac As String
    <DatabaseField("pubmed_id"), DataType(MySqlDbType.VarChar, "30"), Column(Name:="pubmed_id")> Public Property pubmed_id As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `intact_data` (`uniprot_id`, `protein_ac`, `undetermined`, `intact_id`, `interacts_with`, `type`, `entry_ac`, `pubmed_id`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `intact_data` (`uniprot_id`, `protein_ac`, `undetermined`, `intact_id`, `interacts_with`, `type`, `entry_ac`, `pubmed_id`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `intact_data` (`uniprot_id`, `protein_ac`, `undetermined`, `intact_id`, `interacts_with`, `type`, `entry_ac`, `pubmed_id`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `intact_data` (`uniprot_id`, `protein_ac`, `undetermined`, `intact_id`, `interacts_with`, `type`, `entry_ac`, `pubmed_id`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `intact_data` WHERE `entry_ac`='{0}' and `intact_id`='{1}' and `interacts_with`='{2}' and `protein_ac`='{3}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `intact_data` SET `uniprot_id`='{0}', `protein_ac`='{1}', `undetermined`='{2}', `intact_id`='{3}', `interacts_with`='{4}', `type`='{5}', `entry_ac`='{6}', `pubmed_id`='{7}' WHERE `entry_ac`='{8}' and `intact_id`='{9}' and `interacts_with`='{10}' and `protein_ac`='{11}';</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `intact_data` WHERE `entry_ac`='{0}' and `intact_id`='{1}' and `interacts_with`='{2}' and `protein_ac`='{3}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, entry_ac, intact_id, interacts_with, protein_ac)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `intact_data` (`uniprot_id`, `protein_ac`, `undetermined`, `intact_id`, `interacts_with`, `type`, `entry_ac`, `pubmed_id`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, uniprot_id, protein_ac, undetermined, intact_id, interacts_with, type, entry_ac, pubmed_id)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `intact_data` (`uniprot_id`, `protein_ac`, `undetermined`, `intact_id`, `interacts_with`, `type`, `entry_ac`, `pubmed_id`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, uniprot_id, protein_ac, undetermined, intact_id, interacts_with, type, entry_ac, pubmed_id)
        Else
        Return String.Format(INSERT_SQL, uniprot_id, protein_ac, undetermined, intact_id, interacts_with, type, entry_ac, pubmed_id)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{uniprot_id}', '{protein_ac}', '{undetermined}', '{intact_id}', '{interacts_with}', '{type}', '{entry_ac}', '{pubmed_id}')"
        Else
            Return $"('{uniprot_id}', '{protein_ac}', '{undetermined}', '{intact_id}', '{interacts_with}', '{type}', '{entry_ac}', '{pubmed_id}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `intact_data` (`uniprot_id`, `protein_ac`, `undetermined`, `intact_id`, `interacts_with`, `type`, `entry_ac`, `pubmed_id`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, uniprot_id, protein_ac, undetermined, intact_id, interacts_with, type, entry_ac, pubmed_id)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `intact_data` (`uniprot_id`, `protein_ac`, `undetermined`, `intact_id`, `interacts_with`, `type`, `entry_ac`, `pubmed_id`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, uniprot_id, protein_ac, undetermined, intact_id, interacts_with, type, entry_ac, pubmed_id)
        Else
        Return String.Format(REPLACE_SQL, uniprot_id, protein_ac, undetermined, intact_id, interacts_with, type, entry_ac, pubmed_id)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `intact_data` SET `uniprot_id`='{0}', `protein_ac`='{1}', `undetermined`='{2}', `intact_id`='{3}', `interacts_with`='{4}', `type`='{5}', `entry_ac`='{6}', `pubmed_id`='{7}' WHERE `entry_ac`='{8}' and `intact_id`='{9}' and `interacts_with`='{10}' and `protein_ac`='{11}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, uniprot_id, protein_ac, undetermined, intact_id, interacts_with, type, entry_ac, pubmed_id, entry_ac, intact_id, interacts_with, protein_ac)
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As intact_data
                         Return DirectCast(MyClass.MemberwiseClone, intact_data)
                     End Function
End Class


End Namespace

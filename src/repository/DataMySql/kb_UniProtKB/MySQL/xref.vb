#Region "Microsoft.VisualBasic::c531c5bb34b1e6b5af3f196b04ff7efd, DataMySql\kb_UniProtKB\MySQL\xref.vb"

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

    ' Class xref
    ' 
    '     Properties: external_id, hash_code, molecule_type, nucleotide_ID, protein_ID
    '                 uniprot_id, xref
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

REM  Dump @2018/5/23 13:13:51


Imports System.Data.Linq.Mapping
Imports System.Xml.Serialization
Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes
Imports MySqlScript = Oracle.LinuxCompatibility.MySQL.Scripting.Extensions

Namespace kb_UniProtKB.mysql

''' <summary>
''' ```SQL
''' 某一个uniprot蛋白质记录对外部的链接信息
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `xref`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `xref` (
'''   `hash_code` int(10) unsigned NOT NULL,
'''   `uniprot_id` varchar(45) DEFAULT NULL,
'''   `xref` varchar(45) NOT NULL,
'''   `external_id` varchar(45) NOT NULL,
'''   `molecule_type` varchar(45) DEFAULT NULL,
'''   `protein_ID` varchar(45) DEFAULT NULL,
'''   `nucleotide_ID` varchar(45) DEFAULT NULL,
'''   PRIMARY KEY (`hash_code`)
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='某一个uniprot蛋白质记录对外部的链接信息';
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' -- Dumping events for database 'kb_uniprotkb'
''' --
''' 
''' --
''' -- Dumping routines for database 'kb_uniprotkb'
''' --
''' /*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;
''' 
''' /*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
''' /*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
''' /*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
''' /*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
''' /*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
''' /*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
''' /*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;
''' 
''' -- Dump completed on 2017-09-03 12:29:28
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("xref", Database:="kb_uniprotkb", SchemaSQL:="
CREATE TABLE `xref` (
  `hash_code` int(10) unsigned NOT NULL,
  `uniprot_id` varchar(45) DEFAULT NULL,
  `xref` varchar(45) NOT NULL,
  `external_id` varchar(45) NOT NULL,
  `molecule_type` varchar(45) DEFAULT NULL,
  `protein_ID` varchar(45) DEFAULT NULL,
  `nucleotide_ID` varchar(45) DEFAULT NULL,
  PRIMARY KEY (`hash_code`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='某一个uniprot蛋白质记录对外部的链接信息';")>
Public Class xref: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("hash_code"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "10"), Column(Name:="hash_code"), XmlAttribute> Public Property hash_code As Long
    <DatabaseField("uniprot_id"), DataType(MySqlDbType.VarChar, "45"), Column(Name:="uniprot_id")> Public Property uniprot_id As String
    <DatabaseField("xref"), NotNull, DataType(MySqlDbType.VarChar, "45"), Column(Name:="xref")> Public Property xref As String
    <DatabaseField("external_id"), NotNull, DataType(MySqlDbType.VarChar, "45"), Column(Name:="external_id")> Public Property external_id As String
    <DatabaseField("molecule_type"), DataType(MySqlDbType.VarChar, "45"), Column(Name:="molecule_type")> Public Property molecule_type As String
    <DatabaseField("protein_ID"), DataType(MySqlDbType.VarChar, "45"), Column(Name:="protein_ID")> Public Property protein_ID As String
    <DatabaseField("nucleotide_ID"), DataType(MySqlDbType.VarChar, "45"), Column(Name:="nucleotide_ID")> Public Property nucleotide_ID As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `xref` (`hash_code`, `uniprot_id`, `xref`, `external_id`, `molecule_type`, `protein_ID`, `nucleotide_ID`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `xref` (`hash_code`, `uniprot_id`, `xref`, `external_id`, `molecule_type`, `protein_ID`, `nucleotide_ID`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `xref` (`hash_code`, `uniprot_id`, `xref`, `external_id`, `molecule_type`, `protein_ID`, `nucleotide_ID`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `xref` (`hash_code`, `uniprot_id`, `xref`, `external_id`, `molecule_type`, `protein_ID`, `nucleotide_ID`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `xref` WHERE `hash_code` = '{0}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `xref` SET `hash_code`='{0}', `uniprot_id`='{1}', `xref`='{2}', `external_id`='{3}', `molecule_type`='{4}', `protein_ID`='{5}', `nucleotide_ID`='{6}' WHERE `hash_code` = '{7}';</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `xref` WHERE `hash_code` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, hash_code)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `xref` (`hash_code`, `uniprot_id`, `xref`, `external_id`, `molecule_type`, `protein_ID`, `nucleotide_ID`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, hash_code, uniprot_id, xref, external_id, molecule_type, protein_ID, nucleotide_ID)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `xref` (`hash_code`, `uniprot_id`, `xref`, `external_id`, `molecule_type`, `protein_ID`, `nucleotide_ID`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, hash_code, uniprot_id, xref, external_id, molecule_type, protein_ID, nucleotide_ID)
        Else
        Return String.Format(INSERT_SQL, hash_code, uniprot_id, xref, external_id, molecule_type, protein_ID, nucleotide_ID)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{hash_code}', '{uniprot_id}', '{xref}', '{external_id}', '{molecule_type}', '{protein_ID}', '{nucleotide_ID}')"
        Else
            Return $"('{hash_code}', '{uniprot_id}', '{xref}', '{external_id}', '{molecule_type}', '{protein_ID}', '{nucleotide_ID}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `xref` (`hash_code`, `uniprot_id`, `xref`, `external_id`, `molecule_type`, `protein_ID`, `nucleotide_ID`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, hash_code, uniprot_id, xref, external_id, molecule_type, protein_ID, nucleotide_ID)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `xref` (`hash_code`, `uniprot_id`, `xref`, `external_id`, `molecule_type`, `protein_ID`, `nucleotide_ID`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, hash_code, uniprot_id, xref, external_id, molecule_type, protein_ID, nucleotide_ID)
        Else
        Return String.Format(REPLACE_SQL, hash_code, uniprot_id, xref, external_id, molecule_type, protein_ID, nucleotide_ID)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `xref` SET `hash_code`='{0}', `uniprot_id`='{1}', `xref`='{2}', `external_id`='{3}', `molecule_type`='{4}', `protein_ID`='{5}', `nucleotide_ID`='{6}' WHERE `hash_code` = '{7}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, hash_code, uniprot_id, xref, external_id, molecule_type, protein_ID, nucleotide_ID, hash_code)
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As xref
                         Return DirectCast(MyClass.MemberwiseClone, xref)
                     End Function
End Class


End Namespace

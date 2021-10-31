#Region "Microsoft.VisualBasic::9b3e4947b3f4a9d2254436fd3d727e7c, DataMySql\Interpro\Tables\journal_syn.vb"

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

    ' Class journal_syn
    ' 
    '     Properties: code, issn, text, uppercase
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
''' DROP TABLE IF EXISTS `journal_syn`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `journal_syn` (
'''   `issn` varchar(10) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
'''   `code` char(4) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
'''   `text` varchar(200) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
'''   `uppercase` varchar(200) CHARACTER SET latin1 COLLATE latin1_bin DEFAULT NULL,
'''   PRIMARY KEY (`issn`,`text`,`code`),
'''   KEY `fk_journal_syn$code` (`code`),
'''   CONSTRAINT `fk_journal_syn$code` FOREIGN KEY (`code`) REFERENCES `cv_synonym` (`code`) ON DELETE NO ACTION ON UPDATE NO ACTION,
'''   CONSTRAINT `fk_journal_syn$issn` FOREIGN KEY (`issn`) REFERENCES `journal` (`issn`) ON DELETE CASCADE ON UPDATE NO ACTION
''' ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("journal_syn", Database:="interpro", SchemaSQL:="
CREATE TABLE `journal_syn` (
  `issn` varchar(10) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `code` char(4) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `text` varchar(200) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `uppercase` varchar(200) CHARACTER SET latin1 COLLATE latin1_bin DEFAULT NULL,
  PRIMARY KEY (`issn`,`text`,`code`),
  KEY `fk_journal_syn$code` (`code`),
  CONSTRAINT `fk_journal_syn$code` FOREIGN KEY (`code`) REFERENCES `cv_synonym` (`code`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `fk_journal_syn$issn` FOREIGN KEY (`issn`) REFERENCES `journal` (`issn`) ON DELETE CASCADE ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=latin1;")>
Public Class journal_syn: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("issn"), PrimaryKey, NotNull, DataType(MySqlDbType.VarChar, "10"), Column(Name:="issn"), XmlAttribute> Public Property issn As String
    <DatabaseField("code"), PrimaryKey, NotNull, DataType(MySqlDbType.VarChar, "4"), Column(Name:="code"), XmlAttribute> Public Property code As String
    <DatabaseField("text"), PrimaryKey, NotNull, DataType(MySqlDbType.VarChar, "200"), Column(Name:="text"), XmlAttribute> Public Property text As String
    <DatabaseField("uppercase"), DataType(MySqlDbType.VarChar, "200"), Column(Name:="uppercase")> Public Property uppercase As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `journal_syn` (`issn`, `code`, `text`, `uppercase`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `journal_syn` (`issn`, `code`, `text`, `uppercase`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `journal_syn` (`issn`, `code`, `text`, `uppercase`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `journal_syn` (`issn`, `code`, `text`, `uppercase`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `journal_syn` WHERE `issn`='{0}' and `text`='{1}' and `code`='{2}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `journal_syn` SET `issn`='{0}', `code`='{1}', `text`='{2}', `uppercase`='{3}' WHERE `issn`='{4}' and `text`='{5}' and `code`='{6}';</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `journal_syn` WHERE `issn`='{0}' and `text`='{1}' and `code`='{2}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, issn, text, code)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `journal_syn` (`issn`, `code`, `text`, `uppercase`) VALUES ('{0}', '{1}', '{2}', '{3}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, issn, code, text, uppercase)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `journal_syn` (`issn`, `code`, `text`, `uppercase`) VALUES ('{0}', '{1}', '{2}', '{3}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, issn, code, text, uppercase)
        Else
        Return String.Format(INSERT_SQL, issn, code, text, uppercase)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{issn}', '{code}', '{text}', '{uppercase}')"
        Else
            Return $"('{issn}', '{code}', '{text}', '{uppercase}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `journal_syn` (`issn`, `code`, `text`, `uppercase`) VALUES ('{0}', '{1}', '{2}', '{3}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, issn, code, text, uppercase)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `journal_syn` (`issn`, `code`, `text`, `uppercase`) VALUES ('{0}', '{1}', '{2}', '{3}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, issn, code, text, uppercase)
        Else
        Return String.Format(REPLACE_SQL, issn, code, text, uppercase)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `journal_syn` SET `issn`='{0}', `code`='{1}', `text`='{2}', `uppercase`='{3}' WHERE `issn`='{4}' and `text`='{5}' and `code`='{6}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, issn, code, text, uppercase, issn, text, code)
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As journal_syn
                         Return DirectCast(MyClass.MemberwiseClone, journal_syn)
                     End Function
End Class


End Namespace

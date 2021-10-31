#Region "Microsoft.VisualBasic::bc347adaf96b19afc078d5ced03d00ba, DataMySql\Interpro\Tables\varsplic_match.vb"

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

    ' Class varsplic_match
    ' 
    '     Properties: dbcode, evidence, match_date, method_ac, pos_from
    '                 pos_to, protein_ac, score, seq_date, status
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
''' DROP TABLE IF EXISTS `varsplic_match`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `varsplic_match` (
'''   `protein_ac` varchar(12) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
'''   `method_ac` varchar(25) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
'''   `pos_from` int(5) DEFAULT NULL,
'''   `pos_to` int(5) DEFAULT NULL,
'''   `status` char(1) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
'''   `dbcode` char(1) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
'''   `evidence` char(3) CHARACTER SET latin1 COLLATE latin1_bin DEFAULT NULL,
'''   `seq_date` datetime NOT NULL,
'''   `match_date` datetime NOT NULL,
'''   `score` double DEFAULT NULL,
'''   KEY `fk_varsplic_match$dbcode` (`dbcode`),
'''   KEY `fk_varsplic_match$evidence` (`evidence`),
'''   KEY `fk_varsplic_match$method` (`method_ac`),
'''   CONSTRAINT `fk_varsplic_match$dbcode` FOREIGN KEY (`dbcode`) REFERENCES `cv_database` (`dbcode`) ON DELETE NO ACTION ON UPDATE NO ACTION,
'''   CONSTRAINT `fk_varsplic_match$evidence` FOREIGN KEY (`evidence`) REFERENCES `cv_evidence` (`code`) ON DELETE NO ACTION ON UPDATE NO ACTION,
'''   CONSTRAINT `fk_varsplic_match$method` FOREIGN KEY (`method_ac`) REFERENCES `method` (`method_ac`) ON DELETE CASCADE ON UPDATE NO ACTION
''' ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("varsplic_match", Database:="interpro", SchemaSQL:="
CREATE TABLE `varsplic_match` (
  `protein_ac` varchar(12) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `method_ac` varchar(25) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `pos_from` int(5) DEFAULT NULL,
  `pos_to` int(5) DEFAULT NULL,
  `status` char(1) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `dbcode` char(1) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `evidence` char(3) CHARACTER SET latin1 COLLATE latin1_bin DEFAULT NULL,
  `seq_date` datetime NOT NULL,
  `match_date` datetime NOT NULL,
  `score` double DEFAULT NULL,
  KEY `fk_varsplic_match$dbcode` (`dbcode`),
  KEY `fk_varsplic_match$evidence` (`evidence`),
  KEY `fk_varsplic_match$method` (`method_ac`),
  CONSTRAINT `fk_varsplic_match$dbcode` FOREIGN KEY (`dbcode`) REFERENCES `cv_database` (`dbcode`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `fk_varsplic_match$evidence` FOREIGN KEY (`evidence`) REFERENCES `cv_evidence` (`code`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `fk_varsplic_match$method` FOREIGN KEY (`method_ac`) REFERENCES `method` (`method_ac`) ON DELETE CASCADE ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=latin1;")>
Public Class varsplic_match: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("protein_ac"), NotNull, DataType(MySqlDbType.VarChar, "12"), Column(Name:="protein_ac")> Public Property protein_ac As String
    <DatabaseField("method_ac"), NotNull, DataType(MySqlDbType.VarChar, "25"), Column(Name:="method_ac")> Public Property method_ac As String
    <DatabaseField("pos_from"), DataType(MySqlDbType.Int64, "5"), Column(Name:="pos_from")> Public Property pos_from As Long
    <DatabaseField("pos_to"), DataType(MySqlDbType.Int64, "5"), Column(Name:="pos_to")> Public Property pos_to As Long
    <DatabaseField("status"), NotNull, DataType(MySqlDbType.VarChar, "1"), Column(Name:="status")> Public Property status As String
    <DatabaseField("dbcode"), PrimaryKey, NotNull, DataType(MySqlDbType.VarChar, "1"), Column(Name:="dbcode"), XmlAttribute> Public Property dbcode As String
    <DatabaseField("evidence"), DataType(MySqlDbType.VarChar, "3"), Column(Name:="evidence")> Public Property evidence As String
    <DatabaseField("seq_date"), NotNull, DataType(MySqlDbType.DateTime), Column(Name:="seq_date")> Public Property seq_date As Date
    <DatabaseField("match_date"), NotNull, DataType(MySqlDbType.DateTime), Column(Name:="match_date")> Public Property match_date As Date
    <DatabaseField("score"), DataType(MySqlDbType.Double), Column(Name:="score")> Public Property score As Double
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `varsplic_match` (`protein_ac`, `method_ac`, `pos_from`, `pos_to`, `status`, `dbcode`, `evidence`, `seq_date`, `match_date`, `score`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `varsplic_match` (`protein_ac`, `method_ac`, `pos_from`, `pos_to`, `status`, `dbcode`, `evidence`, `seq_date`, `match_date`, `score`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `varsplic_match` (`protein_ac`, `method_ac`, `pos_from`, `pos_to`, `status`, `dbcode`, `evidence`, `seq_date`, `match_date`, `score`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `varsplic_match` (`protein_ac`, `method_ac`, `pos_from`, `pos_to`, `status`, `dbcode`, `evidence`, `seq_date`, `match_date`, `score`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `varsplic_match` WHERE `dbcode` = '{0}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `varsplic_match` SET `protein_ac`='{0}', `method_ac`='{1}', `pos_from`='{2}', `pos_to`='{3}', `status`='{4}', `dbcode`='{5}', `evidence`='{6}', `seq_date`='{7}', `match_date`='{8}', `score`='{9}' WHERE `dbcode` = '{10}';</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `varsplic_match` WHERE `dbcode` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, dbcode)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `varsplic_match` (`protein_ac`, `method_ac`, `pos_from`, `pos_to`, `status`, `dbcode`, `evidence`, `seq_date`, `match_date`, `score`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, protein_ac, method_ac, pos_from, pos_to, status, dbcode, evidence, MySqlScript.ToMySqlDateTimeString(seq_date), MySqlScript.ToMySqlDateTimeString(match_date), score)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `varsplic_match` (`protein_ac`, `method_ac`, `pos_from`, `pos_to`, `status`, `dbcode`, `evidence`, `seq_date`, `match_date`, `score`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, protein_ac, method_ac, pos_from, pos_to, status, dbcode, evidence, MySqlScript.ToMySqlDateTimeString(seq_date), MySqlScript.ToMySqlDateTimeString(match_date), score)
        Else
        Return String.Format(INSERT_SQL, protein_ac, method_ac, pos_from, pos_to, status, dbcode, evidence, MySqlScript.ToMySqlDateTimeString(seq_date), MySqlScript.ToMySqlDateTimeString(match_date), score)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{protein_ac}', '{method_ac}', '{pos_from}', '{pos_to}', '{status}', '{dbcode}', '{evidence}', '{seq_date}', '{match_date}', '{score}')"
        Else
            Return $"('{protein_ac}', '{method_ac}', '{pos_from}', '{pos_to}', '{status}', '{dbcode}', '{evidence}', '{seq_date}', '{match_date}', '{score}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `varsplic_match` (`protein_ac`, `method_ac`, `pos_from`, `pos_to`, `status`, `dbcode`, `evidence`, `seq_date`, `match_date`, `score`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, protein_ac, method_ac, pos_from, pos_to, status, dbcode, evidence, MySqlScript.ToMySqlDateTimeString(seq_date), MySqlScript.ToMySqlDateTimeString(match_date), score)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `varsplic_match` (`protein_ac`, `method_ac`, `pos_from`, `pos_to`, `status`, `dbcode`, `evidence`, `seq_date`, `match_date`, `score`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, protein_ac, method_ac, pos_from, pos_to, status, dbcode, evidence, MySqlScript.ToMySqlDateTimeString(seq_date), MySqlScript.ToMySqlDateTimeString(match_date), score)
        Else
        Return String.Format(REPLACE_SQL, protein_ac, method_ac, pos_from, pos_to, status, dbcode, evidence, MySqlScript.ToMySqlDateTimeString(seq_date), MySqlScript.ToMySqlDateTimeString(match_date), score)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `varsplic_match` SET `protein_ac`='{0}', `method_ac`='{1}', `pos_from`='{2}', `pos_to`='{3}', `status`='{4}', `dbcode`='{5}', `evidence`='{6}', `seq_date`='{7}', `match_date`='{8}', `score`='{9}' WHERE `dbcode` = '{10}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, protein_ac, method_ac, pos_from, pos_to, status, dbcode, evidence, MySqlScript.ToMySqlDateTimeString(seq_date), MySqlScript.ToMySqlDateTimeString(match_date), score, dbcode)
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As varsplic_match
                         Return DirectCast(MyClass.MemberwiseClone, varsplic_match)
                     End Function
End Class


End Namespace

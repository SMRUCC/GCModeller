#Region "Microsoft.VisualBasic::790249f4441f4d719b860605eebe549e, DataMySql\Interpro\Tables\mv_method_match.vb"

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

    ' Class mv_method_match
    ' 
    '     Properties: match_count, method_ac, protein_count
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
''' DROP TABLE IF EXISTS `mv_method_match`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `mv_method_match` (
'''   `method_ac` varchar(25) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
'''   `protein_count` int(7) NOT NULL,
'''   `match_count` int(7) NOT NULL,
'''   PRIMARY KEY (`method_ac`),
'''   CONSTRAINT `fk_mv_method_match$method` FOREIGN KEY (`method_ac`) REFERENCES `method` (`method_ac`) ON DELETE CASCADE ON UPDATE NO ACTION
''' ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("mv_method_match", Database:="interpro", SchemaSQL:="
CREATE TABLE `mv_method_match` (
  `method_ac` varchar(25) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `protein_count` int(7) NOT NULL,
  `match_count` int(7) NOT NULL,
  PRIMARY KEY (`method_ac`),
  CONSTRAINT `fk_mv_method_match$method` FOREIGN KEY (`method_ac`) REFERENCES `method` (`method_ac`) ON DELETE CASCADE ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=latin1;")>
Public Class mv_method_match: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("method_ac"), PrimaryKey, NotNull, DataType(MySqlDbType.VarChar, "25"), Column(Name:="method_ac"), XmlAttribute> Public Property method_ac As String
    <DatabaseField("protein_count"), NotNull, DataType(MySqlDbType.Int64, "7"), Column(Name:="protein_count")> Public Property protein_count As Long
    <DatabaseField("match_count"), NotNull, DataType(MySqlDbType.Int64, "7"), Column(Name:="match_count")> Public Property match_count As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `mv_method_match` (`method_ac`, `protein_count`, `match_count`) VALUES ('{0}', '{1}', '{2}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `mv_method_match` (`method_ac`, `protein_count`, `match_count`) VALUES ('{0}', '{1}', '{2}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `mv_method_match` (`method_ac`, `protein_count`, `match_count`) VALUES ('{0}', '{1}', '{2}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `mv_method_match` (`method_ac`, `protein_count`, `match_count`) VALUES ('{0}', '{1}', '{2}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `mv_method_match` WHERE `method_ac` = '{0}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `mv_method_match` SET `method_ac`='{0}', `protein_count`='{1}', `match_count`='{2}' WHERE `method_ac` = '{3}';</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `mv_method_match` WHERE `method_ac` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, method_ac)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `mv_method_match` (`method_ac`, `protein_count`, `match_count`) VALUES ('{0}', '{1}', '{2}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, method_ac, protein_count, match_count)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `mv_method_match` (`method_ac`, `protein_count`, `match_count`) VALUES ('{0}', '{1}', '{2}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, method_ac, protein_count, match_count)
        Else
        Return String.Format(INSERT_SQL, method_ac, protein_count, match_count)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{method_ac}', '{protein_count}', '{match_count}')"
        Else
            Return $"('{method_ac}', '{protein_count}', '{match_count}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `mv_method_match` (`method_ac`, `protein_count`, `match_count`) VALUES ('{0}', '{1}', '{2}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, method_ac, protein_count, match_count)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `mv_method_match` (`method_ac`, `protein_count`, `match_count`) VALUES ('{0}', '{1}', '{2}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, method_ac, protein_count, match_count)
        Else
        Return String.Format(REPLACE_SQL, method_ac, protein_count, match_count)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `mv_method_match` SET `method_ac`='{0}', `protein_count`='{1}', `match_count`='{2}' WHERE `method_ac` = '{3}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, method_ac, protein_count, match_count, method_ac)
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As mv_method_match
                         Return DirectCast(MyClass.MemberwiseClone, mv_method_match)
                     End Function
End Class


End Namespace

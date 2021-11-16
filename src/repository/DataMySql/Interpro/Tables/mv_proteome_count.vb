#Region "Microsoft.VisualBasic::cd341d4f0041ff1cf7140960e15a154a, DataMySql\Interpro\Tables\mv_proteome_count.vb"

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

    ' Class mv_proteome_count
    ' 
    '     Properties: entry_ac, method_count, name, oscode, protein_count
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
''' DROP TABLE IF EXISTS `mv_proteome_count`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `mv_proteome_count` (
'''   `entry_ac` varchar(9) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
'''   `name` varchar(100) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
'''   `oscode` varchar(5) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
'''   `protein_count` int(7) NOT NULL,
'''   `method_count` int(7) NOT NULL,
'''   PRIMARY KEY (`entry_ac`,`oscode`),
'''   KEY `fk_mv_proteome_count$oscode` (`oscode`),
'''   CONSTRAINT `fk_mv_proteome_count$entry` FOREIGN KEY (`entry_ac`) REFERENCES `entry` (`entry_ac`) ON DELETE CASCADE ON UPDATE NO ACTION,
'''   CONSTRAINT `fk_mv_proteome_count$oscode` FOREIGN KEY (`oscode`) REFERENCES `organism` (`oscode`) ON DELETE CASCADE ON UPDATE NO ACTION
''' ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("mv_proteome_count", Database:="interpro", SchemaSQL:="
CREATE TABLE `mv_proteome_count` (
  `entry_ac` varchar(9) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `name` varchar(100) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `oscode` varchar(5) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `protein_count` int(7) NOT NULL,
  `method_count` int(7) NOT NULL,
  PRIMARY KEY (`entry_ac`,`oscode`),
  KEY `fk_mv_proteome_count$oscode` (`oscode`),
  CONSTRAINT `fk_mv_proteome_count$entry` FOREIGN KEY (`entry_ac`) REFERENCES `entry` (`entry_ac`) ON DELETE CASCADE ON UPDATE NO ACTION,
  CONSTRAINT `fk_mv_proteome_count$oscode` FOREIGN KEY (`oscode`) REFERENCES `organism` (`oscode`) ON DELETE CASCADE ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=latin1;")>
Public Class mv_proteome_count: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("entry_ac"), PrimaryKey, NotNull, DataType(MySqlDbType.VarChar, "9"), Column(Name:="entry_ac"), XmlAttribute> Public Property entry_ac As String
    <DatabaseField("name"), NotNull, DataType(MySqlDbType.VarChar, "100"), Column(Name:="name")> Public Property name As String
    <DatabaseField("oscode"), PrimaryKey, NotNull, DataType(MySqlDbType.VarChar, "5"), Column(Name:="oscode"), XmlAttribute> Public Property oscode As String
    <DatabaseField("protein_count"), NotNull, DataType(MySqlDbType.Int64, "7"), Column(Name:="protein_count")> Public Property protein_count As Long
    <DatabaseField("method_count"), NotNull, DataType(MySqlDbType.Int64, "7"), Column(Name:="method_count")> Public Property method_count As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `mv_proteome_count` (`entry_ac`, `name`, `oscode`, `protein_count`, `method_count`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `mv_proteome_count` (`entry_ac`, `name`, `oscode`, `protein_count`, `method_count`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `mv_proteome_count` (`entry_ac`, `name`, `oscode`, `protein_count`, `method_count`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `mv_proteome_count` (`entry_ac`, `name`, `oscode`, `protein_count`, `method_count`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `mv_proteome_count` WHERE `entry_ac`='{0}' and `oscode`='{1}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `mv_proteome_count` SET `entry_ac`='{0}', `name`='{1}', `oscode`='{2}', `protein_count`='{3}', `method_count`='{4}' WHERE `entry_ac`='{5}' and `oscode`='{6}';</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `mv_proteome_count` WHERE `entry_ac`='{0}' and `oscode`='{1}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, entry_ac, oscode)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `mv_proteome_count` (`entry_ac`, `name`, `oscode`, `protein_count`, `method_count`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, entry_ac, name, oscode, protein_count, method_count)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `mv_proteome_count` (`entry_ac`, `name`, `oscode`, `protein_count`, `method_count`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, entry_ac, name, oscode, protein_count, method_count)
        Else
        Return String.Format(INSERT_SQL, entry_ac, name, oscode, protein_count, method_count)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{entry_ac}', '{name}', '{oscode}', '{protein_count}', '{method_count}')"
        Else
            Return $"('{entry_ac}', '{name}', '{oscode}', '{protein_count}', '{method_count}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `mv_proteome_count` (`entry_ac`, `name`, `oscode`, `protein_count`, `method_count`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, entry_ac, name, oscode, protein_count, method_count)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `mv_proteome_count` (`entry_ac`, `name`, `oscode`, `protein_count`, `method_count`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, entry_ac, name, oscode, protein_count, method_count)
        Else
        Return String.Format(REPLACE_SQL, entry_ac, name, oscode, protein_count, method_count)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `mv_proteome_count` SET `entry_ac`='{0}', `name`='{1}', `oscode`='{2}', `protein_count`='{3}', `method_count`='{4}' WHERE `entry_ac`='{5}' and `oscode`='{6}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, entry_ac, name, oscode, protein_count, method_count, entry_ac, oscode)
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As mv_proteome_count
                         Return DirectCast(MyClass.MemberwiseClone, mv_proteome_count)
                     End Function
End Class


End Namespace

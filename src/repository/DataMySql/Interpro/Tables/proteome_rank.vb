#Region "Microsoft.VisualBasic::bed2a27b1bd9580be6ad6ca67e476ea9, DataMySql\Interpro\Tables\proteome_rank.vb"

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

    ' Class proteome_rank
    ' 
    '     Properties: entry_ac, oscode, rank
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
''' DROP TABLE IF EXISTS `proteome_rank`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `proteome_rank` (
'''   `entry_ac` varchar(9) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
'''   `oscode` varchar(5) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
'''   `rank` int(7) NOT NULL,
'''   PRIMARY KEY (`entry_ac`,`oscode`),
'''   KEY `fk_proteome_rank$oscode` (`oscode`),
'''   CONSTRAINT `fk_proteome_rank$entry` FOREIGN KEY (`entry_ac`) REFERENCES `entry` (`entry_ac`) ON DELETE CASCADE ON UPDATE NO ACTION,
'''   CONSTRAINT `fk_proteome_rank$oscode` FOREIGN KEY (`oscode`) REFERENCES `organism` (`oscode`) ON DELETE CASCADE ON UPDATE NO ACTION
''' ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("proteome_rank", Database:="interpro", SchemaSQL:="
CREATE TABLE `proteome_rank` (
  `entry_ac` varchar(9) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `oscode` varchar(5) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `rank` int(7) NOT NULL,
  PRIMARY KEY (`entry_ac`,`oscode`),
  KEY `fk_proteome_rank$oscode` (`oscode`),
  CONSTRAINT `fk_proteome_rank$entry` FOREIGN KEY (`entry_ac`) REFERENCES `entry` (`entry_ac`) ON DELETE CASCADE ON UPDATE NO ACTION,
  CONSTRAINT `fk_proteome_rank$oscode` FOREIGN KEY (`oscode`) REFERENCES `organism` (`oscode`) ON DELETE CASCADE ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=latin1;")>
Public Class proteome_rank: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("entry_ac"), PrimaryKey, NotNull, DataType(MySqlDbType.VarChar, "9"), Column(Name:="entry_ac"), XmlAttribute> Public Property entry_ac As String
    <DatabaseField("oscode"), PrimaryKey, NotNull, DataType(MySqlDbType.VarChar, "5"), Column(Name:="oscode"), XmlAttribute> Public Property oscode As String
    <DatabaseField("rank"), NotNull, DataType(MySqlDbType.Int64, "7"), Column(Name:="rank")> Public Property rank As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `proteome_rank` (`entry_ac`, `oscode`, `rank`) VALUES ('{0}', '{1}', '{2}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `proteome_rank` (`entry_ac`, `oscode`, `rank`) VALUES ('{0}', '{1}', '{2}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `proteome_rank` (`entry_ac`, `oscode`, `rank`) VALUES ('{0}', '{1}', '{2}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `proteome_rank` (`entry_ac`, `oscode`, `rank`) VALUES ('{0}', '{1}', '{2}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `proteome_rank` WHERE `entry_ac`='{0}' and `oscode`='{1}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `proteome_rank` SET `entry_ac`='{0}', `oscode`='{1}', `rank`='{2}' WHERE `entry_ac`='{3}' and `oscode`='{4}';</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `proteome_rank` WHERE `entry_ac`='{0}' and `oscode`='{1}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, entry_ac, oscode)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `proteome_rank` (`entry_ac`, `oscode`, `rank`) VALUES ('{0}', '{1}', '{2}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, entry_ac, oscode, rank)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `proteome_rank` (`entry_ac`, `oscode`, `rank`) VALUES ('{0}', '{1}', '{2}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, entry_ac, oscode, rank)
        Else
        Return String.Format(INSERT_SQL, entry_ac, oscode, rank)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{entry_ac}', '{oscode}', '{rank}')"
        Else
            Return $"('{entry_ac}', '{oscode}', '{rank}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `proteome_rank` (`entry_ac`, `oscode`, `rank`) VALUES ('{0}', '{1}', '{2}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, entry_ac, oscode, rank)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `proteome_rank` (`entry_ac`, `oscode`, `rank`) VALUES ('{0}', '{1}', '{2}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, entry_ac, oscode, rank)
        Else
        Return String.Format(REPLACE_SQL, entry_ac, oscode, rank)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `proteome_rank` SET `entry_ac`='{0}', `oscode`='{1}', `rank`='{2}' WHERE `entry_ac`='{3}' and `oscode`='{4}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, entry_ac, oscode, rank, entry_ac, oscode)
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As proteome_rank
                         Return DirectCast(MyClass.MemberwiseClone, proteome_rank)
                     End Function
End Class


End Namespace

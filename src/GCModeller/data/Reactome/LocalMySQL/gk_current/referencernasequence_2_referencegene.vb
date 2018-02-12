#Region "Microsoft.VisualBasic::94b13a0cbb206fbec6a56c3260af627a, data\Reactome\LocalMySQL\gk_current\referencernasequence_2_referencegene.vb"

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

    ' Class referencernasequence_2_referencegene
    ' 
    '     Function: GetDeleteSQL, GetDumpInsertValue, GetInsertSQL, GetReplaceSQL, GetUpdateSQL
    ' 
    ' 
    ' /********************************************************************************/

#End Region

REM  Oracle.LinuxCompatibility.MySQL.CodeSolution.VisualBasic.CodeGenerator
REM  MYSQL Schema Mapper
REM      for Microsoft VisualBasic.NET 1.0.0.0

REM  Dump @3/29/2017 9:40:28 PM


Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes

Namespace LocalMySQL.Tables.gk_current

''' <summary>
''' ```SQL
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `referencernasequence_2_referencegene`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `referencernasequence_2_referencegene` (
'''   `DB_ID` int(10) unsigned DEFAULT NULL,
'''   `referenceGene_rank` int(10) unsigned DEFAULT NULL,
'''   `referenceGene` int(10) unsigned DEFAULT NULL,
'''   `referenceGene_class` varchar(64) DEFAULT NULL,
'''   KEY `DB_ID` (`DB_ID`),
'''   KEY `referenceGene` (`referenceGene`)
''' ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("referencernasequence_2_referencegene", Database:="gk_current", SchemaSQL:="
CREATE TABLE `referencernasequence_2_referencegene` (
  `DB_ID` int(10) unsigned DEFAULT NULL,
  `referenceGene_rank` int(10) unsigned DEFAULT NULL,
  `referenceGene` int(10) unsigned DEFAULT NULL,
  `referenceGene_class` varchar(64) DEFAULT NULL,
  KEY `DB_ID` (`DB_ID`),
  KEY `referenceGene` (`referenceGene`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;")>
Public Class referencernasequence_2_referencegene: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("DB_ID"), PrimaryKey, DataType(MySqlDbType.Int64, "10")> Public Property DB_ID As Long
    <DatabaseField("referenceGene_rank"), DataType(MySqlDbType.Int64, "10")> Public Property referenceGene_rank As Long
    <DatabaseField("referenceGene"), DataType(MySqlDbType.Int64, "10")> Public Property referenceGene As Long
    <DatabaseField("referenceGene_class"), DataType(MySqlDbType.VarChar, "64")> Public Property referenceGene_class As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `referencernasequence_2_referencegene` (`DB_ID`, `referenceGene_rank`, `referenceGene`, `referenceGene_class`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `referencernasequence_2_referencegene` (`DB_ID`, `referenceGene_rank`, `referenceGene`, `referenceGene_class`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `referencernasequence_2_referencegene` WHERE `DB_ID` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `referencernasequence_2_referencegene` SET `DB_ID`='{0}', `referenceGene_rank`='{1}', `referenceGene`='{2}', `referenceGene_class`='{3}' WHERE `DB_ID` = '{4}';</SQL>
#End Region
''' <summary>
''' ```SQL
''' DELETE FROM `referencernasequence_2_referencegene` WHERE `DB_ID` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, DB_ID)
    End Function
''' <summary>
''' ```SQL
''' INSERT INTO `referencernasequence_2_referencegene` (`DB_ID`, `referenceGene_rank`, `referenceGene`, `referenceGene_class`) VALUES ('{0}', '{1}', '{2}', '{3}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, DB_ID, referenceGene_rank, referenceGene, referenceGene_class)
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue() As String
        Return $"('{DB_ID}', '{referenceGene_rank}', '{referenceGene}', '{referenceGene_class}')"
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `referencernasequence_2_referencegene` (`DB_ID`, `referenceGene_rank`, `referenceGene`, `referenceGene_class`) VALUES ('{0}', '{1}', '{2}', '{3}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, DB_ID, referenceGene_rank, referenceGene, referenceGene_class)
    End Function
''' <summary>
''' ```SQL
''' UPDATE `referencernasequence_2_referencegene` SET `DB_ID`='{0}', `referenceGene_rank`='{1}', `referenceGene`='{2}', `referenceGene_class`='{3}' WHERE `DB_ID` = '{4}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, DB_ID, referenceGene_rank, referenceGene, referenceGene_class, DB_ID)
    End Function
#End Region
End Class


End Namespace

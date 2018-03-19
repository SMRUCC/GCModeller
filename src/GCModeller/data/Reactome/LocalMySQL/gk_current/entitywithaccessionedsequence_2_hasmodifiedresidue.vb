#Region "Microsoft.VisualBasic::55bd4322aa55369bb1fabd0e8c8dc32a, data\Reactome\LocalMySQL\gk_current\entitywithaccessionedsequence_2_hasmodifiedresidue.vb"

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

    ' Class entitywithaccessionedsequence_2_hasmodifiedresidue
    ' 
    '     Properties: DB_ID, hasModifiedResidue, hasModifiedResidue_class, hasModifiedResidue_rank
    ' 
    '     Function: GetDeleteSQL, GetDumpInsertValue, GetInsertSQL, GetReplaceSQL, GetUpdateSQL
    ' 
    ' 
    ' /********************************************************************************/

#End Region

REM  Oracle.LinuxCompatibility.MySQL.CodeSolution.VisualBasic.CodeGenerator
REM  MYSQL Schema Mapper
REM      for Microsoft VisualBasic.NET 2.1.0.2569

REM  Dump @3/16/2018 10:40:21 PM


Imports System.Data.Linq.Mapping
Imports System.Xml.Serialization
Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes
Imports MySqlScript = Oracle.LinuxCompatibility.MySQL.Scripting.Extensions

Namespace LocalMySQL.Tables.gk_current

''' <summary>
''' ```SQL
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `entitywithaccessionedsequence_2_hasmodifiedresidue`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `entitywithaccessionedsequence_2_hasmodifiedresidue` (
'''   `DB_ID` int(10) unsigned DEFAULT NULL,
'''   `hasModifiedResidue_rank` int(10) unsigned DEFAULT NULL,
'''   `hasModifiedResidue` int(10) unsigned DEFAULT NULL,
'''   `hasModifiedResidue_class` varchar(64) DEFAULT NULL,
'''   KEY `DB_ID` (`DB_ID`),
'''   KEY `hasModifiedResidue` (`hasModifiedResidue`)
''' ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("entitywithaccessionedsequence_2_hasmodifiedresidue", Database:="gk_current", SchemaSQL:="
CREATE TABLE `entitywithaccessionedsequence_2_hasmodifiedresidue` (
  `DB_ID` int(10) unsigned DEFAULT NULL,
  `hasModifiedResidue_rank` int(10) unsigned DEFAULT NULL,
  `hasModifiedResidue` int(10) unsigned DEFAULT NULL,
  `hasModifiedResidue_class` varchar(64) DEFAULT NULL,
  KEY `DB_ID` (`DB_ID`),
  KEY `hasModifiedResidue` (`hasModifiedResidue`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;")>
Public Class entitywithaccessionedsequence_2_hasmodifiedresidue: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("DB_ID"), PrimaryKey, DataType(MySqlDbType.Int64, "10"), Column(Name:="DB_ID"), XmlAttribute> Public Property DB_ID As Long
    <DatabaseField("hasModifiedResidue_rank"), DataType(MySqlDbType.Int64, "10"), Column(Name:="hasModifiedResidue_rank")> Public Property hasModifiedResidue_rank As Long
    <DatabaseField("hasModifiedResidue"), DataType(MySqlDbType.Int64, "10"), Column(Name:="hasModifiedResidue")> Public Property hasModifiedResidue As Long
    <DatabaseField("hasModifiedResidue_class"), DataType(MySqlDbType.VarChar, "64"), Column(Name:="hasModifiedResidue_class")> Public Property hasModifiedResidue_class As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `entitywithaccessionedsequence_2_hasmodifiedresidue` (`DB_ID`, `hasModifiedResidue_rank`, `hasModifiedResidue`, `hasModifiedResidue_class`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `entitywithaccessionedsequence_2_hasmodifiedresidue` (`DB_ID`, `hasModifiedResidue_rank`, `hasModifiedResidue`, `hasModifiedResidue_class`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `entitywithaccessionedsequence_2_hasmodifiedresidue` WHERE `DB_ID` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `entitywithaccessionedsequence_2_hasmodifiedresidue` SET `DB_ID`='{0}', `hasModifiedResidue_rank`='{1}', `hasModifiedResidue`='{2}', `hasModifiedResidue_class`='{3}' WHERE `DB_ID` = '{4}';</SQL>
#End Region
''' <summary>
''' ```SQL
''' DELETE FROM `entitywithaccessionedsequence_2_hasmodifiedresidue` WHERE `DB_ID` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, DB_ID)
    End Function
''' <summary>
''' ```SQL
''' INSERT INTO `entitywithaccessionedsequence_2_hasmodifiedresidue` (`DB_ID`, `hasModifiedResidue_rank`, `hasModifiedResidue`, `hasModifiedResidue_class`) VALUES ('{0}', '{1}', '{2}', '{3}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, DB_ID, hasModifiedResidue_rank, hasModifiedResidue, hasModifiedResidue_class)
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue() As String
        Return $"('{DB_ID}', '{hasModifiedResidue_rank}', '{hasModifiedResidue}', '{hasModifiedResidue_class}')"
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `entitywithaccessionedsequence_2_hasmodifiedresidue` (`DB_ID`, `hasModifiedResidue_rank`, `hasModifiedResidue`, `hasModifiedResidue_class`) VALUES ('{0}', '{1}', '{2}', '{3}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, DB_ID, hasModifiedResidue_rank, hasModifiedResidue, hasModifiedResidue_class)
    End Function
''' <summary>
''' ```SQL
''' UPDATE `entitywithaccessionedsequence_2_hasmodifiedresidue` SET `DB_ID`='{0}', `hasModifiedResidue_rank`='{1}', `hasModifiedResidue`='{2}', `hasModifiedResidue_class`='{3}' WHERE `DB_ID` = '{4}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, DB_ID, hasModifiedResidue_rank, hasModifiedResidue, hasModifiedResidue_class, DB_ID)
    End Function
#End Region
Public Function Clone() As entitywithaccessionedsequence_2_hasmodifiedresidue
                  Return DirectCast(MyClass.MemberwiseClone, entitywithaccessionedsequence_2_hasmodifiedresidue)
              End Function
End Class


End Namespace


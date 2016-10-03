#Region "Microsoft.VisualBasic::9e72e6864c96a4ed0666bc23d2aaddea, ..\GCModeller\data\Reactome\LocalMySQL\gk_current\event_2_relatedspecies.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
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

#End Region

REM  Oracle.LinuxCompatibility.MySQL.CodeGenerator
REM  MYSQL Schema Mapper
REM      for Microsoft VisualBasic.NET 

REM  Dump @12/3/2015 8:15:49 PM


Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes

Namespace LocalMySQL.Tables.gk_current

''' <summary>
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `event_2_relatedspecies`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `event_2_relatedspecies` (
'''   `DB_ID` int(10) unsigned DEFAULT NULL,
'''   `relatedSpecies_rank` int(10) unsigned DEFAULT NULL,
'''   `relatedSpecies` int(10) unsigned DEFAULT NULL,
'''   `relatedSpecies_class` varchar(64) DEFAULT NULL,
'''   KEY `DB_ID` (`DB_ID`),
'''   KEY `relatedSpecies` (`relatedSpecies`)
''' ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("event_2_relatedspecies", Database:="gk_current")>
Public Class event_2_relatedspecies: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("DB_ID"), PrimaryKey, DataType(MySqlDbType.Int64, "10")> Public Property DB_ID As Long
    <DatabaseField("relatedSpecies_rank"), DataType(MySqlDbType.Int64, "10")> Public Property relatedSpecies_rank As Long
    <DatabaseField("relatedSpecies"), DataType(MySqlDbType.Int64, "10")> Public Property relatedSpecies As Long
    <DatabaseField("relatedSpecies_class"), DataType(MySqlDbType.VarChar, "64")> Public Property relatedSpecies_class As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `event_2_relatedspecies` (`DB_ID`, `relatedSpecies_rank`, `relatedSpecies`, `relatedSpecies_class`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `event_2_relatedspecies` (`DB_ID`, `relatedSpecies_rank`, `relatedSpecies`, `relatedSpecies_class`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `event_2_relatedspecies` WHERE `DB_ID` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `event_2_relatedspecies` SET `DB_ID`='{0}', `relatedSpecies_rank`='{1}', `relatedSpecies`='{2}', `relatedSpecies_class`='{3}' WHERE `DB_ID` = '{4}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, DB_ID)
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, DB_ID, relatedSpecies_rank, relatedSpecies, relatedSpecies_class)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, DB_ID, relatedSpecies_rank, relatedSpecies, relatedSpecies_class)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, DB_ID, relatedSpecies_rank, relatedSpecies, relatedSpecies_class, DB_ID)
    End Function
#End Region
End Class


End Namespace

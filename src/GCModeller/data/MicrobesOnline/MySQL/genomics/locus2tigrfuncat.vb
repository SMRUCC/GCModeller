#Region "Microsoft.VisualBasic::29d5000f91cf15487751123a3e8d4ebe, GCModeller\data\MicrobesOnline\MySQL\genomics\locus2tigrfuncat.vb"

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


    ' Code Statistics:

    '   Total Lines: 65
    '    Code Lines: 36
    ' Comment Lines: 22
    '   Blank Lines: 7
    '     File Size: 3.00 KB


    ' Class locus2tigrfuncat
    ' 
    '     Properties: evidence, locusId, mainRole, subRole
    ' 
    '     Function: GetDeleteSQL, GetInsertSQL, GetReplaceSQL, GetUpdateSQL
    ' 
    ' 
    ' /********************************************************************************/

#End Region

REM  Oracle.LinuxCompatibility.MySQL.CodeGenerator
REM  MYSQL Schema Mapper
REM      for Microsoft VisualBasic.NET 

REM  Dump @12/3/2015 8:30:29 PM


Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes

Namespace MySQL.genomics

''' <summary>
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `locus2tigrfuncat`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `locus2tigrfuncat` (
'''   `locusId` int(10) unsigned NOT NULL DEFAULT '0',
'''   `mainRole` varchar(255) DEFAULT NULL,
'''   `subRole` varchar(255) DEFAULT NULL,
'''   `evidence` varchar(64) DEFAULT NULL,
'''   PRIMARY KEY (`locusId`),
'''   KEY `mainRole` (`mainRole`),
'''   KEY `subRole` (`subRole`)
''' ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("locus2tigrfuncat")>
Public Class locus2tigrfuncat: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("locusId"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "10")> Public Property locusId As Long
    <DatabaseField("mainRole"), DataType(MySqlDbType.VarChar, "255")> Public Property mainRole As String
    <DatabaseField("subRole"), DataType(MySqlDbType.VarChar, "255")> Public Property subRole As String
    <DatabaseField("evidence"), DataType(MySqlDbType.VarChar, "64")> Public Property evidence As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `locus2tigrfuncat` (`locusId`, `mainRole`, `subRole`, `evidence`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `locus2tigrfuncat` (`locusId`, `mainRole`, `subRole`, `evidence`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `locus2tigrfuncat` WHERE `locusId` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `locus2tigrfuncat` SET `locusId`='{0}', `mainRole`='{1}', `subRole`='{2}', `evidence`='{3}' WHERE `locusId` = '{4}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, locusId)
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, locusId, mainRole, subRole, evidence)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, locusId, mainRole, subRole, evidence)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, locusId, mainRole, subRole, evidence, locusId)
    End Function
#End Region
End Class


End Namespace

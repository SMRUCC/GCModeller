#Region "Microsoft.VisualBasic::7fcce865d3f3e98f04fd0154893f2fca, GCModeller\data\MicrobesOnline\MySQL\genomics\locus.vb"

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

    '   Total Lines: 79
    '    Code Lines: 42
    ' Comment Lines: 30
    '   Blank Lines: 7
    '     File Size: 4.49 KB


    ' Class locus
    ' 
    '     Properties: cdsCoords, created, evidence, exonCoords, locusId
    '                 posId, priority, scaffoldId, type, version
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
''' DROP TABLE IF EXISTS `locus`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `locus` (
'''   `locusId` int(10) unsigned NOT NULL AUTO_INCREMENT,
'''   `version` int(2) unsigned NOT NULL DEFAULT '1',
'''   `priority` tinyint(3) unsigned DEFAULT NULL,
'''   `created` date DEFAULT NULL,
'''   `posId` int(10) unsigned DEFAULT NULL,
'''   `evidence` varchar(50) DEFAULT NULL,
'''   `scaffoldId` int(10) unsigned DEFAULT NULL,
'''   `type` smallint(5) unsigned DEFAULT NULL,
'''   `exonCoords` text,
'''   `cdsCoords` text,
'''   PRIMARY KEY (`locusId`,`version`),
'''   KEY `Indx_ORF_PosId` (`posId`),
'''   KEY `Indx_ORF_OrfId` (`locusId`),
'''   KEY `scaffoldId` (`scaffoldId`),
'''   KEY `priority` (`priority`)
''' ) ENGINE=MyISAM AUTO_INCREMENT=11826436 DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("locus")>
Public Class locus: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("locusId"), PrimaryKey, AutoIncrement, NotNull, DataType(MySqlDbType.Int64, "10")> Public Property locusId As Long
    <DatabaseField("version"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "2")> Public Property version As Long
    <DatabaseField("priority"), DataType(MySqlDbType.Int64, "3")> Public Property priority As Long
    <DatabaseField("created"), DataType(MySqlDbType.DateTime)> Public Property created As Date
    <DatabaseField("posId"), DataType(MySqlDbType.Int64, "10")> Public Property posId As Long
    <DatabaseField("evidence"), DataType(MySqlDbType.VarChar, "50")> Public Property evidence As String
    <DatabaseField("scaffoldId"), DataType(MySqlDbType.Int64, "10")> Public Property scaffoldId As Long
    <DatabaseField("type"), DataType(MySqlDbType.Int64, "5")> Public Property type As Long
    <DatabaseField("exonCoords"), DataType(MySqlDbType.Text)> Public Property exonCoords As String
    <DatabaseField("cdsCoords"), DataType(MySqlDbType.Text)> Public Property cdsCoords As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `locus` (`version`, `priority`, `created`, `posId`, `evidence`, `scaffoldId`, `type`, `exonCoords`, `cdsCoords`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `locus` (`version`, `priority`, `created`, `posId`, `evidence`, `scaffoldId`, `type`, `exonCoords`, `cdsCoords`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `locus` WHERE `locusId`='{0}' and `version`='{1}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `locus` SET `locusId`='{0}', `version`='{1}', `priority`='{2}', `created`='{3}', `posId`='{4}', `evidence`='{5}', `scaffoldId`='{6}', `type`='{7}', `exonCoords`='{8}', `cdsCoords`='{9}' WHERE `locusId`='{10}' and `version`='{11}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, locusId, version)
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, version, priority, DataType.ToMySqlDateTimeString(created), posId, evidence, scaffoldId, type, exonCoords, cdsCoords)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, version, priority, DataType.ToMySqlDateTimeString(created), posId, evidence, scaffoldId, type, exonCoords, cdsCoords)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, locusId, version, priority, DataType.ToMySqlDateTimeString(created), posId, evidence, scaffoldId, type, exonCoords, cdsCoords, locusId, version)
    End Function
#End Region
End Class


End Namespace

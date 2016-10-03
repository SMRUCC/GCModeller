#Region "Microsoft.VisualBasic::8fa0202b64a6d99fb5fd77a276b95058, ..\GCModeller\data\MicrobesOnline\MySQL\genomics\interpro.vb"

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

REM  Dump @12/3/2015 8:30:29 PM


Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes

Namespace MySQL.genomics

''' <summary>
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `interpro`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `interpro` (
'''   `locusId` int(10) unsigned NOT NULL DEFAULT '0',
'''   `version` int(2) unsigned NOT NULL DEFAULT '1',
'''   `checksum` varchar(16) DEFAULT NULL,
'''   `length` int(5) unsigned DEFAULT NULL,
'''   `domainDb` varchar(30) DEFAULT NULL,
'''   `domainId` varchar(30) NOT NULL DEFAULT '',
'''   `domainDesc` varchar(255) DEFAULT NULL,
'''   `domainStart` int(5) NOT NULL DEFAULT '0',
'''   `domainEnd` int(5) DEFAULT NULL,
'''   `evalue` float DEFAULT NULL,
'''   `status` varchar(10) DEFAULT NULL,
'''   `date` varchar(50) DEFAULT NULL,
'''   `iprId` varchar(9) DEFAULT NULL,
'''   `iprName` varchar(255) DEFAULT NULL,
'''   `geneOntology` longtext,
'''   PRIMARY KEY (`locusId`,`version`,`domainId`,`domainStart`),
'''   KEY `locusId` (`locusId`,`version`),
'''   KEY `iprId` (`iprId`),
'''   KEY `checksum` (`checksum`),
'''   KEY `domainId` (`domainId`)
''' ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("interpro")>
Public Class interpro: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("locusId"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "10")> Public Property locusId As Long
    <DatabaseField("version"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "2")> Public Property version As Long
    <DatabaseField("checksum"), DataType(MySqlDbType.VarChar, "16")> Public Property checksum As String
    <DatabaseField("length"), DataType(MySqlDbType.Int64, "5")> Public Property length As Long
    <DatabaseField("domainDb"), DataType(MySqlDbType.VarChar, "30")> Public Property domainDb As String
    <DatabaseField("domainId"), PrimaryKey, NotNull, DataType(MySqlDbType.VarChar, "30")> Public Property domainId As String
    <DatabaseField("domainDesc"), DataType(MySqlDbType.VarChar, "255")> Public Property domainDesc As String
    <DatabaseField("domainStart"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "5")> Public Property domainStart As Long
    <DatabaseField("domainEnd"), DataType(MySqlDbType.Int64, "5")> Public Property domainEnd As Long
    <DatabaseField("evalue"), DataType(MySqlDbType.Double)> Public Property evalue As Double
    <DatabaseField("status"), DataType(MySqlDbType.VarChar, "10")> Public Property status As String
    <DatabaseField("date"), DataType(MySqlDbType.VarChar, "50")> Public Property [date] As String
    <DatabaseField("iprId"), DataType(MySqlDbType.VarChar, "9")> Public Property iprId As String
    <DatabaseField("iprName"), DataType(MySqlDbType.VarChar, "255")> Public Property iprName As String
    <DatabaseField("geneOntology"), DataType(MySqlDbType.Text)> Public Property geneOntology As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `interpro` (`locusId`, `version`, `checksum`, `length`, `domainDb`, `domainId`, `domainDesc`, `domainStart`, `domainEnd`, `evalue`, `status`, `date`, `iprId`, `iprName`, `geneOntology`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `interpro` (`locusId`, `version`, `checksum`, `length`, `domainDb`, `domainId`, `domainDesc`, `domainStart`, `domainEnd`, `evalue`, `status`, `date`, `iprId`, `iprName`, `geneOntology`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `interpro` WHERE `locusId`='{0}' and `version`='{1}' and `domainId`='{2}' and `domainStart`='{3}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `interpro` SET `locusId`='{0}', `version`='{1}', `checksum`='{2}', `length`='{3}', `domainDb`='{4}', `domainId`='{5}', `domainDesc`='{6}', `domainStart`='{7}', `domainEnd`='{8}', `evalue`='{9}', `status`='{10}', `date`='{11}', `iprId`='{12}', `iprName`='{13}', `geneOntology`='{14}' WHERE `locusId`='{15}' and `version`='{16}' and `domainId`='{17}' and `domainStart`='{18}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, locusId, version, domainId, domainStart)
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, locusId, version, checksum, length, domainDb, domainId, domainDesc, domainStart, domainEnd, evalue, status, [date], iprId, iprName, geneOntology)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, locusId, version, checksum, length, domainDb, domainId, domainDesc, domainStart, domainEnd, evalue, status, [date], iprId, iprName, geneOntology)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, locusId, version, checksum, length, domainDb, domainId, domainDesc, domainStart, domainEnd, evalue, status, [date], iprId, iprName, geneOntology, locusId, version, domainId, domainStart)
    End Function
#End Region
End Class


End Namespace

#Region "Microsoft.VisualBasic::8fcfe511bc57d204e6161b7e3894645f, data\MicrobesOnline\MySQL\genomics\taxonomy.vb"

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

    ' Class taxonomy
    ' 
    '     Properties: created, name, ncbiProjectId, placement, PMID
    '                 Publication, shortName, taxDispGroupId, taxonomyId, Uniprot
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
''' DROP TABLE IF EXISTS `taxonomy`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `taxonomy` (
'''   `taxonomyId` int(10) NOT NULL DEFAULT '0',
'''   `name` varchar(255) DEFAULT NULL,
'''   `placement` int(10) DEFAULT NULL,
'''   `shortName` varchar(100) DEFAULT NULL,
'''   `taxDispGroupId` int(10) DEFAULT NULL,
'''   `created` date DEFAULT NULL,
'''   `PMID` varchar(50) DEFAULT NULL,
'''   `Publication` varchar(255) DEFAULT NULL,
'''   `Uniprot` varchar(10) DEFAULT NULL,
'''   `ncbiProjectId` int(10) DEFAULT NULL,
'''   PRIMARY KEY (`taxonomyId`),
'''   KEY `taxDispGroupId` (`taxDispGroupId`)
''' ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("taxonomy")>
Public Class taxonomy: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("taxonomyId"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "10")> Public Property taxonomyId As Long
    <DatabaseField("name"), DataType(MySqlDbType.VarChar, "255")> Public Property name As String
    <DatabaseField("placement"), DataType(MySqlDbType.Int64, "10")> Public Property placement As Long
    <DatabaseField("shortName"), DataType(MySqlDbType.VarChar, "100")> Public Property shortName As String
    <DatabaseField("taxDispGroupId"), DataType(MySqlDbType.Int64, "10")> Public Property taxDispGroupId As Long
    <DatabaseField("created"), DataType(MySqlDbType.DateTime)> Public Property created As Date
    <DatabaseField("PMID"), DataType(MySqlDbType.VarChar, "50")> Public Property PMID As String
    <DatabaseField("Publication"), DataType(MySqlDbType.VarChar, "255")> Public Property Publication As String
    <DatabaseField("Uniprot"), DataType(MySqlDbType.VarChar, "10")> Public Property Uniprot As String
    <DatabaseField("ncbiProjectId"), DataType(MySqlDbType.Int64, "10")> Public Property ncbiProjectId As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `taxonomy` (`taxonomyId`, `name`, `placement`, `shortName`, `taxDispGroupId`, `created`, `PMID`, `Publication`, `Uniprot`, `ncbiProjectId`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `taxonomy` (`taxonomyId`, `name`, `placement`, `shortName`, `taxDispGroupId`, `created`, `PMID`, `Publication`, `Uniprot`, `ncbiProjectId`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `taxonomy` WHERE `taxonomyId` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `taxonomy` SET `taxonomyId`='{0}', `name`='{1}', `placement`='{2}', `shortName`='{3}', `taxDispGroupId`='{4}', `created`='{5}', `PMID`='{6}', `Publication`='{7}', `Uniprot`='{8}', `ncbiProjectId`='{9}' WHERE `taxonomyId` = '{10}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, taxonomyId)
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, taxonomyId, name, placement, shortName, taxDispGroupId, DataType.ToMySqlDateTimeString(created), PMID, Publication, Uniprot, ncbiProjectId)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, taxonomyId, name, placement, shortName, taxDispGroupId, DataType.ToMySqlDateTimeString(created), PMID, Publication, Uniprot, ncbiProjectId)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, taxonomyId, name, placement, shortName, taxDispGroupId, DataType.ToMySqlDateTimeString(created), PMID, Publication, Uniprot, ncbiProjectId, taxonomyId)
    End Function
#End Region
End Class


End Namespace

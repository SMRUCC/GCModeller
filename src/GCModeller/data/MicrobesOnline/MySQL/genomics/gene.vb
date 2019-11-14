#Region "Microsoft.VisualBasic::90b4ce7054daac45335f27fbb614cec8, data\MicrobesOnline\MySQL\genomics\gene.vb"

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

    ' Class gene
    ' 
    '     Properties: evidence, geneId, name, paralog, product
    '                 productInfo, productType, taxId
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
''' DROP TABLE IF EXISTS `gene`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `gene` (
'''   `taxId` int(20) unsigned NOT NULL,
'''   `geneId` varchar(100) NOT NULL,
'''   `name` varchar(20) DEFAULT NULL,
'''   `evidence` varchar(100) DEFAULT NULL,
'''   `paralog` int(10) unsigned DEFAULT NULL,
'''   `product` varchar(100) DEFAULT NULL,
'''   `productInfo` varchar(255) DEFAULT NULL,
'''   `productType` varchar(255) DEFAULT NULL,
'''   PRIMARY KEY (`geneId`),
'''   KEY `taxId` (`taxId`),
'''   KEY `paralog` (`paralog`),
'''   KEY `product` (`product`)
''' ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("gene")>
Public Class gene: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("taxId"), NotNull, DataType(MySqlDbType.Int64, "20")> Public Property taxId As Long
    <DatabaseField("geneId"), PrimaryKey, NotNull, DataType(MySqlDbType.VarChar, "100")> Public Property geneId As String
    <DatabaseField("name"), DataType(MySqlDbType.VarChar, "20")> Public Property name As String
    <DatabaseField("evidence"), DataType(MySqlDbType.VarChar, "100")> Public Property evidence As String
    <DatabaseField("paralog"), DataType(MySqlDbType.Int64, "10")> Public Property paralog As Long
    <DatabaseField("product"), DataType(MySqlDbType.VarChar, "100")> Public Property product As String
    <DatabaseField("productInfo"), DataType(MySqlDbType.VarChar, "255")> Public Property productInfo As String
    <DatabaseField("productType"), DataType(MySqlDbType.VarChar, "255")> Public Property productType As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `gene` (`taxId`, `geneId`, `name`, `evidence`, `paralog`, `product`, `productInfo`, `productType`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `gene` (`taxId`, `geneId`, `name`, `evidence`, `paralog`, `product`, `productInfo`, `productType`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `gene` WHERE `geneId` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `gene` SET `taxId`='{0}', `geneId`='{1}', `name`='{2}', `evidence`='{3}', `paralog`='{4}', `product`='{5}', `productInfo`='{6}', `productType`='{7}' WHERE `geneId` = '{8}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, geneId)
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, taxId, geneId, name, evidence, paralog, product, productInfo, productType)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, taxId, geneId, name, evidence, paralog, product, productInfo, productType)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, taxId, geneId, name, evidence, paralog, product, productInfo, productType, geneId)
    End Function
#End Region
End Class


End Namespace

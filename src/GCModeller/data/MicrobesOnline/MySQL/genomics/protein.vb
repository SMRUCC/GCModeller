#Region "Microsoft.VisualBasic::2ae3a305d24404af74cc2bc4f1a17fff, GCModeller\data\MicrobesOnline\MySQL\genomics\protein.vb"

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

    '   Total Lines: 87
    '    Code Lines: 47
    ' Comment Lines: 33
    '   Blank Lines: 7
    '     File Size: 5.45 KB


    ' Class protein
    ' 
    '     Properties: dnaFootprintSize, geneId, location, modifiedForm, mw
    '                 mwExp, mwSeq, name, neidhardtSpotNum, PI
    '                 proteinId, symmetry, taxId, type, unmodifiedForm
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
''' DROP TABLE IF EXISTS `protein`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `protein` (
'''   `taxId` int(20) unsigned NOT NULL,
'''   `proteinId` varchar(100) NOT NULL,
'''   `name` varchar(20) DEFAULT NULL,
'''   `dnaFootprintSize` int(5) DEFAULT NULL,
'''   `geneId` varchar(100) DEFAULT NULL,
'''   `location` varchar(100) DEFAULT NULL,
'''   `modifiedForm` varchar(255) DEFAULT NULL,
'''   `mw` float DEFAULT NULL,
'''   `mwSeq` float DEFAULT NULL,
'''   `mwExp` float DEFAULT NULL,
'''   `neidhardtSpotNum` varchar(50) DEFAULT NULL,
'''   `PI` float DEFAULT NULL,
'''   `symmetry` varchar(50) DEFAULT NULL,
'''   `type` varchar(255) DEFAULT NULL,
'''   `unmodifiedForm` varchar(255) DEFAULT NULL,
'''   PRIMARY KEY (`proteinId`),
'''   KEY `taxId` (`taxId`),
'''   KEY `geneId` (`geneId`)
''' ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("protein")>
Public Class protein: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("taxId"), NotNull, DataType(MySqlDbType.Int64, "20")> Public Property taxId As Long
    <DatabaseField("proteinId"), PrimaryKey, NotNull, DataType(MySqlDbType.VarChar, "100")> Public Property proteinId As String
    <DatabaseField("name"), DataType(MySqlDbType.VarChar, "20")> Public Property name As String
    <DatabaseField("dnaFootprintSize"), DataType(MySqlDbType.Int64, "5")> Public Property dnaFootprintSize As Long
    <DatabaseField("geneId"), DataType(MySqlDbType.VarChar, "100")> Public Property geneId As String
    <DatabaseField("location"), DataType(MySqlDbType.VarChar, "100")> Public Property location As String
    <DatabaseField("modifiedForm"), DataType(MySqlDbType.VarChar, "255")> Public Property modifiedForm As String
    <DatabaseField("mw"), DataType(MySqlDbType.Double)> Public Property mw As Double
    <DatabaseField("mwSeq"), DataType(MySqlDbType.Double)> Public Property mwSeq As Double
    <DatabaseField("mwExp"), DataType(MySqlDbType.Double)> Public Property mwExp As Double
    <DatabaseField("neidhardtSpotNum"), DataType(MySqlDbType.VarChar, "50")> Public Property neidhardtSpotNum As String
    <DatabaseField("PI"), DataType(MySqlDbType.Double)> Public Property PI As Double
    <DatabaseField("symmetry"), DataType(MySqlDbType.VarChar, "50")> Public Property symmetry As String
    <DatabaseField("type"), DataType(MySqlDbType.VarChar, "255")> Public Property type As String
    <DatabaseField("unmodifiedForm"), DataType(MySqlDbType.VarChar, "255")> Public Property unmodifiedForm As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `protein` (`taxId`, `proteinId`, `name`, `dnaFootprintSize`, `geneId`, `location`, `modifiedForm`, `mw`, `mwSeq`, `mwExp`, `neidhardtSpotNum`, `PI`, `symmetry`, `type`, `unmodifiedForm`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `protein` (`taxId`, `proteinId`, `name`, `dnaFootprintSize`, `geneId`, `location`, `modifiedForm`, `mw`, `mwSeq`, `mwExp`, `neidhardtSpotNum`, `PI`, `symmetry`, `type`, `unmodifiedForm`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `protein` WHERE `proteinId` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `protein` SET `taxId`='{0}', `proteinId`='{1}', `name`='{2}', `dnaFootprintSize`='{3}', `geneId`='{4}', `location`='{5}', `modifiedForm`='{6}', `mw`='{7}', `mwSeq`='{8}', `mwExp`='{9}', `neidhardtSpotNum`='{10}', `PI`='{11}', `symmetry`='{12}', `type`='{13}', `unmodifiedForm`='{14}' WHERE `proteinId` = '{15}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, proteinId)
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, taxId, proteinId, name, dnaFootprintSize, geneId, location, modifiedForm, mw, mwSeq, mwExp, neidhardtSpotNum, PI, symmetry, type, unmodifiedForm)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, taxId, proteinId, name, dnaFootprintSize, geneId, location, modifiedForm, mw, mwSeq, mwExp, neidhardtSpotNum, PI, symmetry, type, unmodifiedForm)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, taxId, proteinId, name, dnaFootprintSize, geneId, location, modifiedForm, mw, mwSeq, mwExp, neidhardtSpotNum, PI, symmetry, type, unmodifiedForm, proteinId)
    End Function
#End Region
End Class


End Namespace

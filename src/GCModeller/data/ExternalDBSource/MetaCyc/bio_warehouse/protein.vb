#Region "Microsoft.VisualBasic::0eb694bdcb97b30999729d49da98fc17, data\ExternalDBSource\MetaCyc\bio_warehouse\protein.vb"

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

    ' Class protein
    ' 
    '     Properties: AASequence, Charge, DataSetWID, Fragment, Length
    '                 LengthApproximate, MolecularWeightCalc, MolecularWeightExp, Name, PICalc
    '                 PIExp, WID
    ' 
    '     Function: GetDeleteSQL, GetInsertSQL, GetReplaceSQL, GetUpdateSQL
    ' 
    ' 
    ' /********************************************************************************/

#End Region

REM  Oracle.LinuxCompatibility.MySQL.CodeGenerator
REM  MYSQL Schema Mapper
REM      for Microsoft VisualBasic.NET 

REM  Dump @12/3/2015 8:02:47 PM


Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes

Namespace MetaCyc.MySQL

''' <summary>
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `protein`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `protein` (
'''   `WID` bigint(20) NOT NULL,
'''   `Name` text,
'''   `AASequence` longtext,
'''   `Length` int(11) DEFAULT NULL,
'''   `LengthApproximate` varchar(10) DEFAULT NULL,
'''   `Charge` smallint(6) DEFAULT NULL,
'''   `Fragment` char(1) DEFAULT NULL,
'''   `MolecularWeightCalc` float DEFAULT NULL,
'''   `MolecularWeightExp` float DEFAULT NULL,
'''   `PICalc` varchar(50) DEFAULT NULL,
'''   `PIExp` varchar(50) DEFAULT NULL,
'''   `DataSetWID` bigint(20) NOT NULL,
'''   PRIMARY KEY (`WID`),
'''   KEY `PROTEIN_DWID` (`DataSetWID`),
'''   CONSTRAINT `FK_Protein` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("protein", Database:="warehouse")>
Public Class protein: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("WID"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "20")> Public Property WID As Long
    <DatabaseField("Name"), DataType(MySqlDbType.Text)> Public Property Name As String
    <DatabaseField("AASequence"), DataType(MySqlDbType.Text)> Public Property AASequence As String
    <DatabaseField("Length"), DataType(MySqlDbType.Int64, "11")> Public Property Length As Long
    <DatabaseField("LengthApproximate"), DataType(MySqlDbType.VarChar, "10")> Public Property LengthApproximate As String
    <DatabaseField("Charge"), DataType(MySqlDbType.Int64, "6")> Public Property Charge As Long
    <DatabaseField("Fragment"), DataType(MySqlDbType.VarChar, "1")> Public Property Fragment As String
    <DatabaseField("MolecularWeightCalc"), DataType(MySqlDbType.Double)> Public Property MolecularWeightCalc As Double
    <DatabaseField("MolecularWeightExp"), DataType(MySqlDbType.Double)> Public Property MolecularWeightExp As Double
    <DatabaseField("PICalc"), DataType(MySqlDbType.VarChar, "50")> Public Property PICalc As String
    <DatabaseField("PIExp"), DataType(MySqlDbType.VarChar, "50")> Public Property PIExp As String
    <DatabaseField("DataSetWID"), NotNull, DataType(MySqlDbType.Int64, "20")> Public Property DataSetWID As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `protein` (`WID`, `Name`, `AASequence`, `Length`, `LengthApproximate`, `Charge`, `Fragment`, `MolecularWeightCalc`, `MolecularWeightExp`, `PICalc`, `PIExp`, `DataSetWID`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `protein` (`WID`, `Name`, `AASequence`, `Length`, `LengthApproximate`, `Charge`, `Fragment`, `MolecularWeightCalc`, `MolecularWeightExp`, `PICalc`, `PIExp`, `DataSetWID`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `protein` WHERE `WID` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `protein` SET `WID`='{0}', `Name`='{1}', `AASequence`='{2}', `Length`='{3}', `LengthApproximate`='{4}', `Charge`='{5}', `Fragment`='{6}', `MolecularWeightCalc`='{7}', `MolecularWeightExp`='{8}', `PICalc`='{9}', `PIExp`='{10}', `DataSetWID`='{11}' WHERE `WID` = '{12}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, WID)
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, WID, Name, AASequence, Length, LengthApproximate, Charge, Fragment, MolecularWeightCalc, MolecularWeightExp, PICalc, PIExp, DataSetWID)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, WID, Name, AASequence, Length, LengthApproximate, Charge, Fragment, MolecularWeightCalc, MolecularWeightExp, PICalc, PIExp, DataSetWID)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, WID, Name, AASequence, Length, LengthApproximate, Charge, Fragment, MolecularWeightCalc, MolecularWeightExp, PICalc, PIExp, DataSetWID, WID)
    End Function
#End Region
End Class


End Namespace

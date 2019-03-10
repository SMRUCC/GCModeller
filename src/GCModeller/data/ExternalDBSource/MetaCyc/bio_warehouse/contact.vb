#Region "Microsoft.VisualBasic::b37f2e67f9190c64d308136590b891a1, data\ExternalDBSource\MetaCyc\bio_warehouse\contact.vb"

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

    ' Class contact
    ' 
    '     Properties: Address, Affiliation, DataSetWID, Email, Fax
    '                 FirstName, Identifier, LastName, MAGEClass, MidInitials
    '                 Name, Parent, Phone, TollFreePhone, URI
    '                 WID
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
''' DROP TABLE IF EXISTS `contact`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `contact` (
'''   `WID` bigint(20) NOT NULL,
'''   `DataSetWID` bigint(20) NOT NULL,
'''   `MAGEClass` varchar(100) NOT NULL,
'''   `Identifier` varchar(255) DEFAULT NULL,
'''   `Name` varchar(255) DEFAULT NULL,
'''   `URI` varchar(255) DEFAULT NULL,
'''   `Address` varchar(255) DEFAULT NULL,
'''   `Phone` varchar(255) DEFAULT NULL,
'''   `TollFreePhone` varchar(255) DEFAULT NULL,
'''   `Email` varchar(255) DEFAULT NULL,
'''   `Fax` varchar(255) DEFAULT NULL,
'''   `Parent` bigint(20) DEFAULT NULL,
'''   `LastName` varchar(255) DEFAULT NULL,
'''   `FirstName` varchar(255) DEFAULT NULL,
'''   `MidInitials` varchar(255) DEFAULT NULL,
'''   `Affiliation` bigint(20) DEFAULT NULL,
'''   PRIMARY KEY (`WID`),
'''   KEY `FK_Contact1` (`DataSetWID`),
'''   KEY `FK_Contact3` (`Parent`),
'''   KEY `FK_Contact4` (`Affiliation`),
'''   CONSTRAINT `FK_Contact1` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_Contact3` FOREIGN KEY (`Parent`) REFERENCES `contact` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_Contact4` FOREIGN KEY (`Affiliation`) REFERENCES `contact` (`WID`) ON DELETE CASCADE
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("contact", Database:="warehouse")>
Public Class contact: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("WID"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "20")> Public Property WID As Long
    <DatabaseField("DataSetWID"), NotNull, DataType(MySqlDbType.Int64, "20")> Public Property DataSetWID As Long
    <DatabaseField("MAGEClass"), NotNull, DataType(MySqlDbType.VarChar, "100")> Public Property MAGEClass As String
    <DatabaseField("Identifier"), DataType(MySqlDbType.VarChar, "255")> Public Property Identifier As String
    <DatabaseField("Name"), DataType(MySqlDbType.VarChar, "255")> Public Property Name As String
    <DatabaseField("URI"), DataType(MySqlDbType.VarChar, "255")> Public Property URI As String
    <DatabaseField("Address"), DataType(MySqlDbType.VarChar, "255")> Public Property Address As String
    <DatabaseField("Phone"), DataType(MySqlDbType.VarChar, "255")> Public Property Phone As String
    <DatabaseField("TollFreePhone"), DataType(MySqlDbType.VarChar, "255")> Public Property TollFreePhone As String
    <DatabaseField("Email"), DataType(MySqlDbType.VarChar, "255")> Public Property Email As String
    <DatabaseField("Fax"), DataType(MySqlDbType.VarChar, "255")> Public Property Fax As String
    <DatabaseField("Parent"), DataType(MySqlDbType.Int64, "20")> Public Property Parent As Long
    <DatabaseField("LastName"), DataType(MySqlDbType.VarChar, "255")> Public Property LastName As String
    <DatabaseField("FirstName"), DataType(MySqlDbType.VarChar, "255")> Public Property FirstName As String
    <DatabaseField("MidInitials"), DataType(MySqlDbType.VarChar, "255")> Public Property MidInitials As String
    <DatabaseField("Affiliation"), DataType(MySqlDbType.Int64, "20")> Public Property Affiliation As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `contact` (`WID`, `DataSetWID`, `MAGEClass`, `Identifier`, `Name`, `URI`, `Address`, `Phone`, `TollFreePhone`, `Email`, `Fax`, `Parent`, `LastName`, `FirstName`, `MidInitials`, `Affiliation`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `contact` (`WID`, `DataSetWID`, `MAGEClass`, `Identifier`, `Name`, `URI`, `Address`, `Phone`, `TollFreePhone`, `Email`, `Fax`, `Parent`, `LastName`, `FirstName`, `MidInitials`, `Affiliation`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `contact` WHERE `WID` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `contact` SET `WID`='{0}', `DataSetWID`='{1}', `MAGEClass`='{2}', `Identifier`='{3}', `Name`='{4}', `URI`='{5}', `Address`='{6}', `Phone`='{7}', `TollFreePhone`='{8}', `Email`='{9}', `Fax`='{10}', `Parent`='{11}', `LastName`='{12}', `FirstName`='{13}', `MidInitials`='{14}', `Affiliation`='{15}' WHERE `WID` = '{16}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, WID)
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, WID, DataSetWID, MAGEClass, Identifier, Name, URI, Address, Phone, TollFreePhone, Email, Fax, Parent, LastName, FirstName, MidInitials, Affiliation)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, WID, DataSetWID, MAGEClass, Identifier, Name, URI, Address, Phone, TollFreePhone, Email, Fax, Parent, LastName, FirstName, MidInitials, Affiliation)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, WID, DataSetWID, MAGEClass, Identifier, Name, URI, Address, Phone, TollFreePhone, Email, Fax, Parent, LastName, FirstName, MidInitials, Affiliation, WID)
    End Function
#End Region
End Class


End Namespace

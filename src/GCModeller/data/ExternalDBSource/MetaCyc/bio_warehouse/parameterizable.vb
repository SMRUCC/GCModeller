#Region "Microsoft.VisualBasic::2d92cde3d4f7460ffe50ee58f06e89be, ExternalDBSource\MetaCyc\bio_warehouse\parameterizable.vb"

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

    ' Class parameterizable
    ' 
    '     Properties: DataSetWID, Hardware, Hardware_Type, Identifier, MAGEClass
    '                 Make, Model, Name, Protocol_Type, Software_Type
    '                 Text, Title, URI, WID
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
''' DROP TABLE IF EXISTS `parameterizable`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!40101 SET character_set_client = utf8 */;
''' CREATE TABLE `parameterizable` (
'''   `WID` bigint(20) NOT NULL,
'''   `DataSetWID` bigint(20) NOT NULL,
'''   `MAGEClass` varchar(100) NOT NULL,
'''   `Identifier` varchar(255) DEFAULT NULL,
'''   `Name` varchar(255) DEFAULT NULL,
'''   `URI` varchar(255) DEFAULT NULL,
'''   `Model` varchar(255) DEFAULT NULL,
'''   `Make` varchar(255) DEFAULT NULL,
'''   `Hardware_Type` bigint(20) DEFAULT NULL,
'''   `Text` varchar(1000) DEFAULT NULL,
'''   `Title` varchar(255) DEFAULT NULL,
'''   `Protocol_Type` bigint(20) DEFAULT NULL,
'''   `Software_Type` bigint(20) DEFAULT NULL,
'''   `Hardware` bigint(20) DEFAULT NULL,
'''   PRIMARY KEY (`WID`),
'''   KEY `FK_Parameterizable1` (`DataSetWID`),
'''   KEY `FK_Parameterizable3` (`Hardware_Type`),
'''   KEY `FK_Parameterizable4` (`Protocol_Type`),
'''   KEY `FK_Parameterizable5` (`Software_Type`),
'''   KEY `FK_Parameterizable6` (`Hardware`),
'''   CONSTRAINT `FK_Parameterizable1` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_Parameterizable3` FOREIGN KEY (`Hardware_Type`) REFERENCES `term` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_Parameterizable4` FOREIGN KEY (`Protocol_Type`) REFERENCES `term` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_Parameterizable5` FOREIGN KEY (`Software_Type`) REFERENCES `term` (`WID`) ON DELETE CASCADE,
'''   CONSTRAINT `FK_Parameterizable6` FOREIGN KEY (`Hardware`) REFERENCES `parameterizable` (`WID`) ON DELETE CASCADE
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' 
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("parameterizable", Database:="warehouse")>
Public Class parameterizable: Inherits Oracle.LinuxCompatibility.MySQL.SQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("WID"), PrimaryKey, NotNull, DataType(MySqlDbType.Int64, "20")> Public Property WID As Long
    <DatabaseField("DataSetWID"), NotNull, DataType(MySqlDbType.Int64, "20")> Public Property DataSetWID As Long
    <DatabaseField("MAGEClass"), NotNull, DataType(MySqlDbType.VarChar, "100")> Public Property MAGEClass As String
    <DatabaseField("Identifier"), DataType(MySqlDbType.VarChar, "255")> Public Property Identifier As String
    <DatabaseField("Name"), DataType(MySqlDbType.VarChar, "255")> Public Property Name As String
    <DatabaseField("URI"), DataType(MySqlDbType.VarChar, "255")> Public Property URI As String
    <DatabaseField("Model"), DataType(MySqlDbType.VarChar, "255")> Public Property Model As String
    <DatabaseField("Make"), DataType(MySqlDbType.VarChar, "255")> Public Property Make As String
    <DatabaseField("Hardware_Type"), DataType(MySqlDbType.Int64, "20")> Public Property Hardware_Type As Long
    <DatabaseField("Text"), DataType(MySqlDbType.VarChar, "1000")> Public Property Text As String
    <DatabaseField("Title"), DataType(MySqlDbType.VarChar, "255")> Public Property Title As String
    <DatabaseField("Protocol_Type"), DataType(MySqlDbType.Int64, "20")> Public Property Protocol_Type As Long
    <DatabaseField("Software_Type"), DataType(MySqlDbType.Int64, "20")> Public Property Software_Type As Long
    <DatabaseField("Hardware"), DataType(MySqlDbType.Int64, "20")> Public Property Hardware As Long
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Private Shared ReadOnly INSERT_SQL As String = <SQL>INSERT INTO `parameterizable` (`WID`, `DataSetWID`, `MAGEClass`, `Identifier`, `Name`, `URI`, `Model`, `Make`, `Hardware_Type`, `Text`, `Title`, `Protocol_Type`, `Software_Type`, `Hardware`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}');</SQL>
    Private Shared ReadOnly REPLACE_SQL As String = <SQL>REPLACE INTO `parameterizable` (`WID`, `DataSetWID`, `MAGEClass`, `Identifier`, `Name`, `URI`, `Model`, `Make`, `Hardware_Type`, `Text`, `Title`, `Protocol_Type`, `Software_Type`, `Hardware`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}');</SQL>
    Private Shared ReadOnly DELETE_SQL As String = <SQL>DELETE FROM `parameterizable` WHERE `WID` = '{0}';</SQL>
    Private Shared ReadOnly UPDATE_SQL As String = <SQL>UPDATE `parameterizable` SET `WID`='{0}', `DataSetWID`='{1}', `MAGEClass`='{2}', `Identifier`='{3}', `Name`='{4}', `URI`='{5}', `Model`='{6}', `Make`='{7}', `Hardware_Type`='{8}', `Text`='{9}', `Title`='{10}', `Protocol_Type`='{11}', `Software_Type`='{12}', `Hardware`='{13}' WHERE `WID` = '{14}';</SQL>
#End Region
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, WID)
    End Function
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, WID, DataSetWID, MAGEClass, Identifier, Name, URI, Model, Make, Hardware_Type, Text, Title, Protocol_Type, Software_Type, Hardware)
    End Function
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, WID, DataSetWID, MAGEClass, Identifier, Name, URI, Model, Make, Hardware_Type, Text, Title, Protocol_Type, Software_Type, Hardware)
    End Function
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, WID, DataSetWID, MAGEClass, Identifier, Name, URI, Model, Make, Hardware_Type, Text, Title, Protocol_Type, Software_Type, Hardware, WID)
    End Function
#End Region
End Class


End Namespace

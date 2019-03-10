#Region "Microsoft.VisualBasic::6bc6ee758fa2cebe228fa2cb43cf8b47, data\ExternalDBSource\MetaCyc\BioWarehouse.vb"

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

    '     Class BioWarehouse
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace MetaCyc

    ''' <summary>
    ''' BioWarehouse – Database Integration for Bioinformatics
    ''' 
    ''' The BioWarehouse is a toolkit for constructing a warehouse of bioinformatics databases. 
    ''' It consists of a relational schema definition for bioinformatics datatypes, loaders for 
    ''' each component database, and Perl/SQL code to query the warehouse for testing and demonstrations. 
    ''' Both Oracle and MySQL are supported.
    ''' The BioWarehouse contains a Set Of loader programs. Each loader parses one Or more input database 
    ''' file(s), translates the data into the warehouse schema, And inserts the data into the warehouse database. 
    ''' The BioWarehouse also contains sample SQL And perl code To query the database.
    ''' 
    ''' http://biowarehouse.ai.sri.com/
    ''' </summary>
    Public Class BioWarehouse

    End Class
End Namespace

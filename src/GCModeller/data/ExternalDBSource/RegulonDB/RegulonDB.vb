#Region "Microsoft.VisualBasic::a0a20012cd6e450fb32547b14fb523a8, ExternalDBSource\RegulonDB\RegulonDB.vb"

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

    '     Class RegulonDB
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: ExportSites
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace RegulonDB

    Public Class RegulonDB : Inherits LANS.SystemsBiology.ExternalDBSource.MySQLDbReflector

        Sub New(MySQL As Oracle.LinuxCompatibility.MySQL.Client.ConnectionUri)
            Call MyBase.New(MySQL)
        End Sub

        Public Function ExportSites() As LANS.SystemsBiology.Assembly.SequenceModel.FASTA.FastaFile
            Dim Table = DbReflector.Query(Of ExternalDBSource.RegulonDB.Site)("select * from site")
            Dim File = LANS.SystemsBiology.Assembly.SequenceModel.FASTA.FastaExportMethods.Export(Table.ToArray)
            Return File
        End Function
    End Class
End Namespace

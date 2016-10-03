#Region "Microsoft.VisualBasic::892f59a5615d1044468f0435498fd1ac, ..\GCModeller\data\ExternalDBSource\MySQLDbReflector.vb"

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

Public MustInherit Class MySQLDbReflector

    Protected Friend DbReflector As Oracle.LinuxCompatibility.MySQL.Reflection.DbReflector
    '  Protected Friend Export As SMRUCC.genomics.SequenceModel.FASTA.FsaExport = New SequenceModel.FASTA.FsaExport

    Protected Friend Sub New(MySQL As Oracle.LinuxCompatibility.MySQL.ConnectionUri)
        DbReflector = New Oracle.LinuxCompatibility.MySQL.Reflection.DbReflector(MySQL.GetConnectionString)
    End Sub

    Public Overrides Function ToString() As String
        Return String.Format("{0}::{1}", Me.GetType.Name, DbReflector.ToString)
    End Function
End Class

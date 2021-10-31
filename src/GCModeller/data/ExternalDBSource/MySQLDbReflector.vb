#Region "Microsoft.VisualBasic::956d8d74ec712a92f9aefeedb2109529, data\ExternalDBSource\MySQLDbReflector.vb"

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

    ' 
    ' /********************************************************************************/

#End Region

'#Region "Microsoft.VisualBasic::17ab7d3f923f441ab57f871e89b0c6c9, data\ExternalDBSource\MySQLDbReflector.vb"

'    ' Author:
'    ' 
'    '       asuka (amethyst.asuka@gcmodeller.org)
'    '       xie (genetics@smrucc.org)
'    '       xieguigang (xie.guigang@live.com)
'    ' 
'    ' Copyright (c) 2018 GPL3 Licensed
'    ' 
'    ' 
'    ' GNU GENERAL PUBLIC LICENSE (GPL3)
'    ' 
'    ' 
'    ' This program is free software: you can redistribute it and/or modify
'    ' it under the terms of the GNU General Public License as published by
'    ' the Free Software Foundation, either version 3 of the License, or
'    ' (at your option) any later version.
'    ' 
'    ' This program is distributed in the hope that it will be useful,
'    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
'    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
'    ' GNU General Public License for more details.
'    ' 
'    ' You should have received a copy of the GNU General Public License
'    ' along with this program. If not, see <http://www.gnu.org/licenses/>.



'    ' /********************************************************************************/

'    ' Summaries:

'    ' Class MySQLDbReflector
'    ' 
'    '     Constructor: (+1 Overloads) Sub New
'    '     Function: ToString
'    ' 
'    ' /********************************************************************************/

'#End Region

'Imports Oracle.LinuxCompatibility.MySQL.Reflection
'Imports Oracle.LinuxCompatibility.MySQL.Uri

'Public MustInherit Class MySQLDbReflector

'    Protected Friend DbReflector As DbReflector
'    '  Protected Friend Export As SMRUCC.genomics.SequenceModel.FASTA.FsaExport = New SequenceModel.FASTA.FsaExport

'    Protected Friend Sub New(MySQL As ConnectionUri)
'        DbReflector = New DbReflector(MySQL.GetConnectionString)
'    End Sub

'    Public Overrides Function ToString() As String
'        Return String.Format("{0}::{1}", Me.GetType.Name, DbReflector.ToString)
'    End Function
'End Class

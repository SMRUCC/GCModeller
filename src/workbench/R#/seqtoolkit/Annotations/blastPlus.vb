﻿#Region "Microsoft.VisualBasic::d61c2b1621510aa35d164a5f87acac4b, R#\seqtoolkit\Annotations\blastPlus.vb"

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

    ' Module blastPlusInterop
    ' 
    '     Function: blastn, blastp, blastx, makeblastdb
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData

<Package("blast+")>
Module blastPlusInterop

    <ExportAPI("makeblastdb")>
    Public Function makeblastdb([in] As String, dbtype As String, Optional env As Environment = Nothing)

    End Function

    <ExportAPI("blastp")>
    Public Function blastp()

    End Function

    <ExportAPI("blastn")>
    Public Function blastn()

    End Function

    <ExportAPI("blastx")>
    Public Function blastx()

    End Function

End Module

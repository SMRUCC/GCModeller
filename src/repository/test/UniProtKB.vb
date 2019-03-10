﻿#Region "Microsoft.VisualBasic::d43ef543f0099648c2ddba3954b4f43a, test\UniProtKB.vb"

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

    ' Module UniProtKB
    ' 
    '     Sub: Main
    ' 
    ' /********************************************************************************/

#End Region

Imports SMRUCC.genomics.Assembly.Uniprot.XML
Imports SMRUCC.genomics.Data.Repository.kb_UniProtKB

Module UniProtKB

    Sub Main()
        Dim path$ = "G:\GCModeller-repo\uniprot-all.xml\uniprot-id-Q9Y478+OR+id-P54619+OR+id-Q5VST6+OR+id-Q7Z5R6+OR+id-Q9NRW3+--.xml"
        Call UniProtXML _
            .EnumerateEntries(path) _
            .DumpMySQL(path.TrimSuffix & ".sql")
    End Sub
End Module

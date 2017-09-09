#Region "Microsoft.VisualBasic::402c2393e3a67842971806df66c69d6f, ..\CLI_tools\mpl\Program.vb"

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

Imports SMRUCC.genomics.Analysis.ProteinTools.Family

Module Program

    Public Function Main() As Integer
#If DEBUG Then
        Call KEGG.ParsingFamilyDef("DNA-binding transcriptional repressor FabR (A)").__DEBUG_ECHO
#End If
        Return GetType(CLI).RunCLI(App.CommandLine)
    End Function
End Module

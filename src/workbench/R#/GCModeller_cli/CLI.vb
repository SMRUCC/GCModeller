#Region "Microsoft.VisualBasic::a0313c644d59a7d5bb0ff580b6c21f31, GCModeller_cli\CLI.vb"

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

    ' Module CLI
    ' 
    '     Function: eggHTS, keggTools, profiler
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData

''' <summary>
''' GCModeller internal cli pipeline api for R# scripting
''' </summary>
<Package("GCModeller")>
Public Module CLI

    <ExportAPI("profiler")>
    Public Function profiler() As GCModellerApps.Profiler
        Return GCModellerApps.Profiler.FromEnvironment(App.HOME)
    End Function

    <ExportAPI("eggHTS")>
    Public Function eggHTS() As GCModellerApps.eggHTS
        Return GCModellerApps.eggHTS.FromEnvironment(App.HOME)
    End Function

    <ExportAPI("KEGG_tools")>
    Public Function keggTools() As GCModellerApps.KEGG_tools
        Return GCModellerApps.KEGG_tools.FromEnvironment(App.HOME)
    End Function
End Module

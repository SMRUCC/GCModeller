#Region "Microsoft.VisualBasic::546c11ac28f922bff81fe97256b5a309, ..\CLI_tools\KEGG\CLI\Pathways.vb"

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

Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Language
Imports SMRUCC.genomics.Assembly.KEGG.Archives.Xml
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject

Partial Module CLI

    <ExportAPI("/Compile.Model", Info:="KEGG pathway model compiler",
               Usage:="/Compile.Model /pathway <pathwayDIR> /mods <modulesDIR> /sp <sp_code> [/out <out.Xml>]")>
    Public Function Compile(args As CommandLine) As Integer
        Dim pwyDIR As String = args("/pathway")
        Dim modDIR As String = args("/mods")
        Dim sp As String = args("/sp")
        Dim reactions As String = GCModeller.FileSystem.KEGG.GetReactions
        Dim out As String = args.GetValue("/out", pwyDIR & "." & sp & "_KEGG.xml")
        Dim model As XmlModel = CompilerAPI.Compile(
            KEGGPathways:=pwyDIR,
            KEGGModules:=modDIR,
            KEGGReactions:=reactions,
            speciesCode:=sp)
        Return model.SaveAsXml(out).CLICode
    End Function

    <ExportAPI("/Pathway.geneIDs",
               Usage:="/Pathway.geneIDs /in <pathway.XML> [/out <out.list.txt>]")>
    Public Function PathwayGeneList(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim out As String = args.GetValue("/out", [in].TrimSuffix & ".geneIDs.txt")
        Dim pathway As Pathway = [in].LoadXml(Of Pathway)
        Return pathway.GetPathwayGenes.SaveTo(out).CLICode
    End Function

    <ExportAPI("/Compound.Map.Render")>
    <Usage("/Compound.Map.Render /list <csv/txt> [/repo <pathwayMap.repository> /scale <default=1> /color <default=red> /out <out.DIR>]")>
    Public Function CompoundMapRender(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim repo$ = (args <= "/repo") Or (GCModeller.FileSystem.FileSystem.RepositoryRoot & "/KEGG/pathwayMap/").AsDefault
        Dim scale# = args.GetValue("/scale", 1.0#)
        Dim color$ = (args <= "/color") Or "red".AsDefault
        Dim out$ = (args <= "/out") Or [in].TrimSuffix.AsDefault


    End Function
End Module

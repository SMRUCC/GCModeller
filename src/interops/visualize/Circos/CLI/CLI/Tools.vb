#Region "Microsoft.VisualBasic::2743abe3763df62a79ae1c8f2d7bef9a, ..\interops\visualize\Circos\CLI\CLI\Tools.vb"

' Author:
' 
'       asuka (amethyst.asuka@gcmodeller.org)
'       xieguigang (xie.guigang@live.com)
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

Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.DocumentFormat.Csv
Imports Microsoft.VisualBasic.Linq.Extensions
Imports SMRUCC.genomics.Assembly.DOOR
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.RpsBLAST
Imports SMRUCC.genomics.Visualize.Circos

Partial Module CLI

    <ExportAPI("/DOOR.COGs", Usage:="/DOOR.COGs /DOOR <genome.opr> [/out <out.COGs.Csv>]")>
    Public Function DOOR_COGs(args As CommandLine) As Integer
        Dim inFile As String = args("/DOOR")
        Dim out As String = args.GetValue("/out", inFile.TrimSuffix & ".COGs.Csv")
        Dim DOOR As DOOR = DOOR_API.Load(inFile)
        Dim COGs As MyvaCOG() = DOOR.Genes.ToArray(
            Function(x) New MyvaCOG With {
                .COG = x.COG_number,
                .Description = x.Product,
                .Length = x.Length,
                .Category = Regex.Split(x.COG_number, "\d+").Last,
                .MyvaCOG = x.COG_number,
                .QueryName = x.Synonym}).OrderBy(Function(x) x.QueryName).ToArray
        Return COGs.SaveTo(out).CLICode
    End Function

    <ExportAPI("/Regulons.Dumps", Usage:="/Regulons.Dumps /in <genomes.bbh.DIR> /ptt <genome.ptt> [/out <out.Csv>]")>
    Public Function DumpNames(args As CommandLine) As Integer
        Dim inDIR As String = args("/in")
        Dim ptt As String = args("/ptt")
        Dim out As String = args.GetValue("/out", inDIR & ".Names.Csv")
        Dim gbPTT = SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.PTT.Load(ptt)
        Dim names = NameExtensions.DumpNames(inDIR, gbPTT)
        Return names.SaveTo(out).CLICode
    End Function
End Module

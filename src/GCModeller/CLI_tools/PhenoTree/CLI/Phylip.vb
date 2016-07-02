#Region "Microsoft.VisualBasic::de88d57b394f41ffc91992b0ac48f93d, ..\GCModeller\CLI_tools\PhenoTree\CLI\Phylip.vb"

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

Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports SMRUCC.genomics.Interops.NCBI.Extensions.Analysis
Imports SMRUCC.genomics.Interops.Visualize.Phylip.MatrixFile
Imports SMRUCC.genomics.Interops.Visualize.Phylip.ShellScriptAPI

Partial Module CLI

    ''' <summary>
    ''' 为phylip构建进化树创建遗传矩阵
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/venn.Matrix",
               Usage:="/venn.Matrix /besthits <besthits.xml.DIR> [/query <sp.name> /limits -1 /out <out.txt>]")>
    Public Function VennMatrix(args As CommandLine) As Integer
        Dim inDIR As String = args - "/besthits"
        Dim out As String = args.GetValue("/out", inDIR & $".{NameOf(VennMatrix)}.txt")
        Dim source As BestHit() = LoadHitsVennData(inDIR)
        Dim limits As Integer = args.GetInt32("/limits")
        Dim query As String = args - "/query"
        Dim gendist As Gendist = source.ExportGendistMatrixFromBesthitMeta(query, Limits:=limits)
        Call gendist.MATRaw.Save(out.TrimFileExt & ".csv", Encodings.ASCII)
        Call gendist.GenerateDocument.SaveTo(out, Encodings.ASCII.GetEncodings)
        Return 0
    End Function
End Module


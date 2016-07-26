#Region "Microsoft.VisualBasic::275952104490e5e6074a551111e12586, ..\GCModeller\CLI_tools\MEME\Cli\WGCNA.vb"

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

Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports SMRUCC.genomics.Analysis.RNA_Seq
Imports SMRUCC.genomics.Analysis.RNA_Seq.RTools
Imports SMRUCC.genomics.Assembly.NCBI
Imports SMRUCC.genomics.Interops.NBCR.MEME_Suite.Workflows.PromoterParser

Partial Module CLI

    <ExportAPI("--CExpr.WGCNA", Usage:="--CExpr.WGCNA /mods <CytoscapeNodes.txt> /genome <genome.DIR|*.PTT;*.fna> /out <DIR.out>")>
    Public Function WGCNAModsCExpr(args As CommandLine) As Integer
        Dim mods = WGCNA.ModsView(WGCNA.LoadModules(args("/mods")))
        Dim gb As New GenBank.TabularFormat.PTTDbLoader(args("/genome"))
        Dim geneParser As New GenePromoterParser(gb)
        Dim ExportDir As String = args("/out")

        For Each Length As Integer In GenePromoterParser.PrefixLength

            For Each profile In mods
                Dim path As String = $"{ExportDir}/{Length}/{profile.Key}.fasta"
                Dim fasta = geneParser.GetSequenceById(lstId:=profile.Value.Join(profile.Key).Distinct.ToArray, Length:=Length)
                Call fasta.Save(path)
            Next
        Next

        Return 0
    End Function
End Module

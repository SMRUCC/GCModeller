#Region "Microsoft.VisualBasic::6321eea4a1c38885b5ed0ab26c488430, data\BASys\Export.vb"

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

    ' Module Export
    ' 
    '     Function: ExportCOG, ExportFaa, ExportFfn, ExportGFF, ExportPTT
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Language
Imports SMRUCC.genomics.Annotation.Assembly.NCBI.GenBank.TabularFormat.GFF
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.ComponentModels
Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.Interops.NCBI.Extensions.Pipeline.COG
Imports SMRUCC.genomics.SequenceModel.FASTA

Public Module Export

    <Extension>
    Public Function ExportPTT(proj As Project, Optional title As String = "") As PTT
        Dim genes As GeneBrief() = LinqAPI.Exec(Of GeneBrief) <=
 _
            From x As TableBrief
            In proj.Briefs
            Let loci As NucleotideLocation =
                New NucleotideLocation(
                x.Start,
                x.End,
                x.Strand.GetStrands)
            Select New GeneBrief With {
                .COG = x.COG,
                .Length = loci.Length,
                .Gene = x.Gene,
                .Location = loci,
                .PID = x.Accession.Match("\d+"),
                .Product = x.Function,
                .Synonym = x.Accession,
                .Code = "-"
            }

        Return New PTT(
            genes,
            If(String.IsNullOrEmpty(title),
            proj.Summary.chrId,
            title),
            CInt(proj.Summary.Length))
    End Function

    <Extension>
    Public Function ExportGFF(proj As Project) As GFFTable

    End Function

    ''' <summary>
    ''' Export annotated protein fasta sequence.
    ''' </summary>
    ''' <param name="proj"></param>
    ''' <returns></returns>
    <Extension>
    Public Function ExportFaa(proj As Project) As FastaFile
        Dim aa As IEnumerable(Of FastaSeq) =
            From x As Ecard
            In proj.Ecards
            Let fa As FastaSeq = x.GetProt
            Where Not fa Is Nothing
            Select fa

        Return New FastaFile(aa)
    End Function

    <Extension>
    Public Function ExportFfn(proj As Project) As FastaFile
        Dim nt As IEnumerable(Of FastaSeq) =
            From x As Ecard
            In proj.Ecards
            Let fa As FastaSeq = x.GetNt
            Where Not fa Is Nothing
            Select fa

        Return New FastaFile(nt)
    End Function

    <Extension>
    Public Function ExportCOG(proj As Project) As MyvaCOG()
        Return LinqAPI.Exec(Of MyvaCOG) <=
 _
            From x As TableBrief
            In proj.Briefs
            Select New MyvaCOG With {
                .COG = x.COG,
                .QueryName = x.Accession,
                .Description = x.Function,
                .MyvaCOG = x.Gene,
                .QueryLength = Math.Abs(x.End - x.Start),
                .Length = Math.Abs(x.End - x.Start)
            }
    End Function
End Module

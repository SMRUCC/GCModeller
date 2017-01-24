#Region "Microsoft.VisualBasic::c3f5c7901f1ae74d6907ab1b7dc828c7, ..\GCModeller\data\BASys\Export.vb"

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

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Language
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.ComponentModels
Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.RpsBLAST
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
    Public Function ExportGFF(proj As Project) As GFF

    End Function

    ''' <summary>
    ''' Export annotated protein fasta sequence.
    ''' </summary>
    ''' <param name="proj"></param>
    ''' <returns></returns>
    <Extension>
    Public Function ExportFaa(proj As Project) As FastaFile
        Dim aa As IEnumerable(Of FastaToken) =
            From x As Ecard
            In proj.Ecards
            Let fa As FastaToken = x.GetProt
            Where Not fa Is Nothing
            Select fa

        Return New FastaFile(aa)
    End Function

    <Extension>
    Public Function ExportFfn(proj As Project) As FastaFile
        Dim nt As IEnumerable(Of FastaToken) =
            From x As Ecard
            In proj.Ecards
            Let fa As FastaToken = x.GetNt
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

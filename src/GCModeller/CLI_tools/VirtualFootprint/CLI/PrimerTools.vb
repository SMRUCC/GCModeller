#Region "Microsoft.VisualBasic::8f6ac8b181e1442931052ed0feaa8380, CLI_tools\VirtualFootprint\CLI\PrimerTools.vb"

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
    '     Function: BuildEnzymeDb, ExportPrimer
    ' 
    '     Sub: __exportPrimer
    ' 
    ' Class SSRPrimers
    ' 
    '     Properties: ForwardPrimer, name, ReversePrimer
    ' 
    '     Function: ToString
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Serialization
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.Analysis.PrimerDesigner.Restriction_enzyme
Imports SMRUCC.genomics.SequenceModel.FASTA

Partial Module CLI

    <ExportAPI("/restrict_enzyme.builds",
               Info:="",
               Usage:="/restrict_enzyme.builds [/source <html_DIR> /out <out.json>]")>
    <ArgumentAttribute("/source", True,
                   Description:="Default using the data source from Wikipedia.")>
    <ArgumentAttribute("/out", True,
                   Description:="Enzyme database was writing to the GCModeller repository by default.")>
    <Group(CLIGrouping.PrimerTools)>
    Public Function BuildEnzymeDb(args As CommandLine) As Integer
        Dim source As String = args("/source")
        Dim out As String = args.GetValue("/out", GCModeller.FileSystem.RepositoryRoot & "/restrict_enzyme.Xml")

        If Not source.DirectoryExists Then
            Return -1
        Else
            Dim result As Enzyme() = source.LoadDIR
            Return result.GetXml.SaveTo(out).CLICode
        End If
    End Function

    <ExportAPI("/Export.Primer",
               Info:="[SSR name], [Forward primer], [Reverse primer]",
               Usage:="/Export.Primer /in <primer.csv/DIR> [/out <out.DIR> /batch]")>
    <Group(CLIGrouping.PrimerTools)>
    Public Function ExportPrimer(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim batch As Boolean = args.GetBoolean("/batch")
        Dim out As String =
            args.GetValue("/out", If(batch, [in].TrimDIR, [in].TrimSuffix) & ".Primers/")

        If batch Then
            For Each file As String In ls - l - r - wildcards("*.Csv") <= [in]
                Call __exportPrimer(file, out)
            Next
        Else
            Call __exportPrimer([in], out)
        End If

        Return 0
    End Function

    Private Sub __exportPrimer([in] As String, out As String)
        Dim raw As IEnumerable(Of SSRPrimers) = [in].LoadCsv(Of SSRPrimers)
        Dim forwards As FastaSeq() =
            LinqAPI.Exec(Of FastaSeq) <= From x As SSRPrimers
                                           In raw
                                           Select New FastaSeq({x.name}, x.ForwardPrimer)
        Dim reversed As FastaSeq() =
            LinqAPI.Exec(Of FastaSeq) <= From x As SSRPrimers
                                           In raw
                                           Select New FastaSeq({x.name}, x.ReversePrimer)

        Call New FastaFile(forwards).Save(out & "/F-" & [in].BaseName & ".fasta")
        Call New FastaFile(reversed).Save(out & "/R-" & [in].BaseName & ".fasta")

        Dim interval As String = New String("N"c, 20)
        Dim primers As FastaSeq() =
            LinqAPI.Exec(Of FastaSeq) <= From x As SSRPrimers
                                           In raw
                                           Let seq As String = x.ForwardPrimer & interval & x.ReversePrimer
                                           Select New FastaSeq({x.name}, seq)

        Call New FastaFile(primers).Save(out & $"/Primers-{[in].BaseName}.fasta")
    End Sub
End Module

Public Class SSRPrimers
    <Column("SSR name")> Public Property name As String
    <Column("Forward primer")> Public Property ForwardPrimer As String
    <Column("Reverse primer")> Public Property ReversePrimer As String

    Public Overrides Function ToString() As String
        Return Me.GetJson
    End Function
End Class

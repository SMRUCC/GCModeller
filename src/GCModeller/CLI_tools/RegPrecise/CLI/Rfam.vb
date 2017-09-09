#Region "Microsoft.VisualBasic::9cb6c144c8c1d5018955078c67f9269e, ..\CLI_tools\RegPrecise\CLI\Rfam.vb"

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

Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.Assembly
Imports SMRUCC.genomics.Data.Regprecise
Imports SMRUCC.genomics.Data.Xfam.Rfam.Infernal
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH

Partial Module CLI

    <ExportAPI("/Rfam.Regulates",
           Usage:="/Rfam.Regulates /in <RegPrecise.regulons.csv> /rfam <rfam_search.csv> [/out <out.csv>]")>
    Public Function RfamRegulates(args As CommandLine) As Integer
        Dim [in] As String = args <= "/in"
        Dim rfam As String = args <= "/rfam"
        Dim regulons As IEnumerable(Of RegPreciseOperon) =
            [in].LoadCsv(Of RegPreciseOperon)
        Dim rfamSites As IEnumerable(Of HitDataRow) =
            (From x As HitDataRow
             In rfam.LoadCsv(Of HitDataRow)
             Where x.data("rank").Trim.Last <> "?"c
             Select x)
        Dim oprRG = (From x As RegPreciseOperon
                     In regulons
                     Where Not x.Regulators.IsNullOrEmpty
                     Select From rg As String
                            In x.Regulators
                            Select rg,
                                opr = x).IteratesALL
        Dim oprHash As Dictionary(Of String, RegPreciseOperon()) =
            (From x
             In oprRG
             Select x
             Group x By x.rg Into Group) _
                  .ToDictionary(Function(x) x.rg,
                                Function(x) x.Group.ToArray(Function(o) o.opr))
        Dim out As String = args.GetValue("/out", rfam.TrimSuffix & $".{[in].BaseName}.Csv")
        Dim result As New List(Of RfamRegulon)

        For Each x As HitDataRow In rfamSites
            Dim acc As String = x.data("Accession")

            If oprHash.ContainsKey(acc) Then
                For Each opr As RegPreciseOperon In oprHash(acc)
                    result += New RfamRegulon(x, opr)
                Next
            End If
        Next

        Return result.SaveTo(out).CLICode
    End Function

    <ExportAPI("/siRNA.Maps",
               Usage:="/siRNA.Maps /in <siRNA.csv> /hits <blastn.csv> [/out <out.csv>]")>
    Public Function siRNAMaps(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim hits As String = args("/hits")
        Dim out As String = args.GetValue("/out", [in].TrimSuffix & $".{hits.BaseName}.csv")
        Dim inSites As IEnumerable(Of HitDataRow) =
            [in].LoadCsv(Of HitDataRow)
        Dim blastnHits As IEnumerable(Of BestHit) = hits.LoadCsv(Of BestHit)
        Dim indexed = (From x As BestHit In blastnHits
                       Let query As Bac_sRNA.org.Sequence =
                           New Bac_sRNA.org.Sequence(x.QueryName.Split("|"c))
                       Let loci As String = query.MappingLocation.ToString
                       Select loci,
                           query,
                           x.HitName
                       Group By loci Into Group) _
                               .ToDictionary(Function(x) x.loci,
                                             Function(x) x.Group.ToArray)
        Dim result As New List(Of HitDataRow)

        For Each x As HitDataRow In inSites
            If indexed.ContainsKey(x.MappingLocation.ToString) Then
                For Each hit In indexed(x.MappingLocation.ToString)
                    Dim clone As HitDataRow = x.Copy
                    clone.data.Add("siRNA", hit.HitName)
                    result += clone
                Next

                Call Console.Write(".")
            End If
        Next

        Return result.SaveTo(out).CLICode
    End Function
End Module

Public Class RfamRegulon : Inherits HitDataRow

    Public Property Effector As String
    Public Property Pathway As String
    Public Property BiologicalProcess As String
    Public Property source As String
    Public Property Operon As String()
    <Column("Operon.Strand")> Public Property OPR_Strand As String
    Public Property bbh As String()

    Sub New(rfam As HitDataRow, operon As RegPreciseOperon)
        Me.data = rfam.data
        Me.direction = rfam.direction
        Me.distance = rfam.distance
        Me.end = rfam.end
        Me.LociDescrib = rfam.LociDescrib
        Me.ORF = rfam.ORF
        Me.start = rfam.start
        Me.strand = rfam.strand

        Me.Effector = operon.Effector
        Me.Pathway = operon.Pathway
        Me.BiologicalProcess = operon.BiologicalProcess
        Me.source = operon.source
        Me.Operon = operon.Operon
        Me.OPR_Strand = operon.Strand
        Me.bbh = operon.bbh
    End Sub

    Public Overrides Function ToString() As String
        Return Me.GetJson
    End Function
End Class

#Region "Microsoft.VisualBasic::75e1d4835d465e77716182c11c238eeb, ..\interops\localblast\Tools\CLI\VennDiagram\VennDiagram.vb"

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
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Language.UnixBash
Imports SMRUCC.genomics.Analysis.localblast.VennDiagram.BlastAPI
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject
Imports SMRUCC.genomics.Interops.NCBI.Extensions.Analysis

Partial Module CLI

    <ExportAPI("/SSBH2BH_LDM",
               Usage:="/SSBH2BH_LDM /in <ssbh.csv> [/xml /coverage 0.8 /identities 0.3 /out <out.xml>]")>
    Public Function KEGGSSOrtholog2Bh(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim out As String = args.GetValue("/out", [in].TrimSuffix & ".BestHit.Xml")
        Dim isXml As Boolean = args.GetBoolean("/xml")
        Dim Xml As HitCollection
        Dim coverage As Double = args.GetValue("/coverage", 0.8)
        Dim identities As Double = args.GetValue("/identities", 0.3)

        If isXml Then
            Dim ssbh As SSDB.OrthologREST = [in].LoadXml(Of SSDB.OrthologREST)
            Xml = KEGG_API.Export(ssbh)
        Else
            Dim ssbh As IEnumerable(Of SSDB.Ortholog) = [in].LoadCsv(Of SSDB.Ortholog)
            Xml = KEGG_API.Export(ssbh, [in].BaseName)
        End If

        Return Xml.SaveAsXml(out).CLICode
    End Function

    <ExportAPI("/SSDB.Export", Usage:="/SSDB.Export /in <inDIR> [/coverage 0.8 /identities 0.3 /out <out.Xml>]")>
    Public Function KEGGSSDBExport(args As CommandLine) As Integer
        Dim [in] As String = args - "/in"
        Dim out As String = args.GetValue("/out", [in].ParentPath & "/" & [in].BaseName & ".SSDB_BBH.Xml")
        Dim Xmls As IEnumerable(Of String) = ls - l - r - wildcards("*.xml") <= [in]
        Dim coverage As Double = args.GetValue("/coverage", 0.8)
        Dim identities As Double = args.GetValue("/identities", 0.3)
        Dim SSDB As BestHit = KEGG_API.EXPORT(Xmls.Select(AddressOf LoadXml(Of SSDB.OrthologREST)), coverage, identities)
        Return SSDB.SaveAsXml(out).CLICode
    End Function

    <ExportAPI("/BestHits.Filtering", Usage:="/BestHits.Filtering /in <besthit.xml> /sp <table.txt> [/out <out.Xml>]")>
    Public Function BestHitFiltering(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim sp As String = args("/sp")
        Dim out As String = args.GetValue("/out", [in].TrimSuffix & "." & sp.BaseName & ".Xml")
        Dim bbh As BestHit = [in].LoadXml(Of BestHit)
        Dim lstSP As String() = (From s As String In sp.ReadAllLines Select s.Split(CChar(vbTab)).First).ToArray
        For Each x In bbh.hits
            If x.Hits.IsNullOrEmpty Then
                Continue For
            End If
            x.Hits = (From hit As Hit In x.Hits.AsParallel Where Array.IndexOf(lstSP, hit.tag) > -1 Select hit).ToArray
        Next
        Return bbh.SaveAsXml(out).CLICode
    End Function

    <ExportAPI("/Venn.Single", Usage:="/Venn.Single /in <besthits.Xml> [/out <out.csv>]")>
    Public Function VennSingle(args As CommandLine) As Integer
        Dim [in] As String = args - "/in"
        Dim out As String = args.GetValue("/out", [in].TrimSuffix & ".venn.Csv")
        Dim besthit As BestHit = [in].LoadXml(Of BestHit)
        Dim df As DocumentStream.File = VennDataModel.DeltaMove({besthit})
        Return df.Save(out, Encodings.ASCII).CLICode
    End Function
End Module

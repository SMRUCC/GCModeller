#Region "Microsoft.VisualBasic::94487cf35eb8cb259b916aea1ebe70fc, visualize\Cytoscape\CLI_tool\CLI\Metagenomics.vb"

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
    '     Function: __loadDict, BBHTrimIdentities, GenerateBlastNetwork, MatrixToNetwork, MetaBuildBLAST
    '               SimpleBBH, SSU_MetagenomeNetwork
    ' 
    ' /********************************************************************************/

#End Region

Imports System.ComponentModel
Imports System.Drawing
Imports System.Text
Imports Microsoft.VisualBasic.ApplicationServices
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Data.visualize.KMeans
Imports Microsoft.VisualBasic.Data.visualize.Network
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream
Imports Microsoft.VisualBasic.DataMining.KMeans
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.Default
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.Assembly.NCBI.Taxonomy
Imports SMRUCC.genomics.Interops.NCBI.Extensions
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.NtMapping
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.BLASTOutput
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.BLASTOutput.BlastPlus
Imports SMRUCC.genomics.Model.Network.BLAST.API
Imports SMRUCC.genomics.Model.Network.BLAST.BBHAPI
Imports SMRUCC.genomics.Model.Network.BLAST.LDM
Imports SMRUCC.genomics.Model.Network.BLAST.Metagenome

Partial Module CLI

    <ExportAPI("/bbh.Trim.Indeitites",
               Usage:="/bbh.Trim.Indeitites /in <bbh.csv> [/identities <0.3> /out <out.csv>]")>
    <Group(CLIGrouping.Metagenomics)>
    Public Function BBHTrimIdentities(args As CommandLine) As Integer
        Dim inFile As String = args("/in")
        Dim identities As Double = args.GetValue("/identities", 0.3)
        Dim out As String = args.GetValue("/out", inFile.TrimSuffix & $".identities.{identities}.csv")

        Using IO As New IO.Linq.WriteStream(Of BBH)(out)
            Dim reader As New IO.Linq.DataStream(inFile)
            Call reader.ForEachBlock(Of BBH)(Sub(data0)
                                                 data0 = (From x In data0.AsParallel Where x.identities >= identities Select x).ToArray
                                                 Call IO.Flush(data0)
                                             End Sub)
            Return 0
        End Using
    End Function

    <ExportAPI("/BBH.Simple", Usage:="/BBH.Simple /in <sbh.csv> [/evalue <evalue: 1e-5> /out <out.bbh.csv>]")>
    <Group(CLIGrouping.Metagenomics)>
    Public Function SimpleBBH(args As CommandLine) As Integer
        Dim inFile As String = args("/in")
        Dim out As String = args.GetValue("/out ", inFile.TrimSuffix & ".bbh.simple.Csv")
        Dim evalue As Double = args.GetValue("/evalue", 0.00001)
        Dim lstSBH As New List(Of LocalBLAST.Application.BBH.BestHit)

        Using read As New IO.Linq.DataStream(inFile)
            Call read.ForEachBlock(Of LocalBLAST.Application.BBH.BestHit)(
                invoke:=Sub(block As LocalBLAST.Application.BBH.BestHit()) Call lstSBH.AddRange((From x In block.AsParallel Where x.evalue <= evalue Select x).ToArray),
                blockSize:=51200 * 2)
        End Using

        Dim simpleBBHArray = BBHHits(lstSBH)
        Using IO As New IO.Linq.WriteStream(Of BBH)(out)
            Dim buffer = simpleBBHArray.Split(102400)

            For Each block In buffer
                Call IO.Flush(block)
            Next

            Return 0
        End Using
    End Function

    <ExportAPI("/BLAST.Network", Usage:="/BLAST.Network /in <inFile> [/out <outDIR> /type <default:blast_out; values: blast_out, sbh, bbh> /dict <dict.xml>]")>
    <Group(CLIGrouping.Metagenomics)>
    Public Function GenerateBlastNetwork(args As CommandLine) As Integer
        Dim inFile As String = args("/in")
        Dim out As String = args.GetValue("/out", inFile.TrimSuffix)
        Dim type As String = args.GetValue("/type", "blast_out").ToLower
        Dim method As BuildFromSource

        If BuildMethods.ContainsKey(type) Then
            method = BuildMethods(type)
        Else
            method = AddressOf BuildFromBlastOUT
        End If

        Dim dict As String = args("/dict")
        Dim locusDict As Dictionary(Of String, String) = __loadDict(dict)
        Dim network = method(source:=inFile, locusDict:=locusDict)
        Return network.Save(out, Encodings.UTF8).CLICode
    End Function

    Private Function __loadDict(xml As String) As Dictionary(Of String, String)
        If Not xml.FileExists Then Return New Dictionary(Of String, String)

        Dim locusList As LocusDict() = xml.LoadXml(Of LocusDict())

        If locusList Is Nothing Then
            Return New Dictionary(Of String, String)
        End If

        Return LocusDict.CreateDictionary(locusList)
    End Function

    <ExportAPI("/BLAST.Network.MetaBuild", Usage:="/BLAST.Network.MetaBuild /in <inDIR> [/out <outDIR> /dict <dict.xml>]")>
    <Group(CLIGrouping.Metagenomics)>
    Public Function MetaBuildBLAST(args As CommandLine) As Integer
        Dim inDIR As String = args("/in")
        Dim out As String = args.GetValue("/out", inDIR & ".MetaBuild")
        Dim dict As String = args("/dict")
        Dim locusDict As Dictionary(Of String, String) = __loadDict(dict)
        Dim network = MetaBuildFromBBH(inDIR, locusDict)
        Return network.Save(out, Encodings.UTF8).CLICode
    End Function

    <ExportAPI("/Matrix.NET",
               Info:="Converts a generic distance matrix or kmeans clustering result to network model.",
               Usage:="/Matrix.NET /in <kmeans-out.csv> [/out <net.DIR> /generic /colors <clusters> /cutoff 0 /cutoff.paired]")>
    <ArgumentAttribute("/in", AcceptTypes:={GetType(EntityClusterModel), GetType(DataSet)})>
    <ArgumentAttribute("/generic", True, CLITypes.Boolean,
              AcceptTypes:={GetType(Boolean)},
              Description:="If this argument parameter was presents, then the ""/in"" input data is a generic matrix(DataSet) type, otherwise is a kmeans output result csv file.")>
    Public Function MatrixToNetwork(args As CommandLine) As Integer
        Dim inFile As String = args("/in")
        Dim out As String = args.GetValue("/out", inFile.TrimSuffix & ".network/")
        Dim colors$ = args.GetValue("/colors", "clusters")
        Dim data As EntityClusterModel()

        If args.GetBoolean("/generic") Then
            data = DataSet.LoadDataSet(inFile).ToKMeansModels
        Else
            data = inFile.LoadCsv(Of EntityClusterModel)
        End If

        Dim cutoff As Double = args.GetDouble("/cutoff")
        Dim clusters$() = data.Select(Function(d) d.Cluster).Distinct.ToArray
        Dim clusterColors As Dictionary(Of String, Color) = Designer _
            .GetColors(colors, clusters.Length) _
            .SeqIterator _
            .ToDictionary(Function(cluster) clusters(cluster),
                          Function(color) +color)
        Dim cut As Func(Of Double, Boolean)

        If args.GetBoolean("/cutoff.paired") Then
            cut = Function(score) Math.Abs(score) > cutoff
        Else
            cut = Function(d) d > cutoff
        End If

        Dim net As NetworkTables = data.ToNetwork(clusterColors, cut:=cut)
        Return net.Save(out, Encoding.ASCII).CLICode
    End Function

    <ExportAPI("/BLAST.Metagenome.SSU.Network")>
    <Description("> Viral assemblage composition in Yellowstone acidic hot springs assessed by network analysis, DOI: 10.1038/ismej.2015.28")>
    <Usage("/BLAST.Metagenome.SSU.Network /net <blastn.self.txt/blastn.mapping.csv> /tax <ssu-nt.blastnMaps.csv> /taxonomy <ncbi_taxonomy:names,nodes> [/x2taxid <x2taxid.dmp/DIR> /tax-build-in /skip-exists /gi2taxid /parallel /theme-color <default='Paired:c12'> /identities <default:0.3> /coverage <default:0.3> /out <out-net.DIR>]")>
    <Group(CLIGrouping.Metagenomics)>
    <ArgumentAttribute("/net", Description:="The blastn mapping that you can creates from the self pairwise blastn alignment of your SSU sequence. Using for create the network graph based on the similarity result between the aligned sequnece.")>
    <ArgumentAttribute("/tax", Description:="The blastn mapping that you can creates from the blastn alignment of your SSU sequence against the NCBI nt database.")>
    <ArgumentAttribute("/x2taxid", Description:="NCBI taxonomy database that you can download from the NCBI ftp server.")>
    Public Function SSU_MetagenomeNetwork(args As CommandLine) As Integer
        Dim net$ = args("/net")
        Dim tax$ = args("/tax")
        Dim taxonomy$ = args("/taxonomy")
        Dim identities As Double = args.GetValue("/identities", 0.3)
        Dim coverage As Double = args.GetValue("/coverage", 0.3)
        Dim EXPORT$ = args.GetValue("/out", net.TrimSuffix & "-" & tax.BaseName & "-metagenome-network/")
        Dim out As New Value(Of String)
        Dim taxdata As BlastnMapping() = tax.LoadCsv(Of BlastnMapping)
        Dim gi2taxid As Boolean = args.GetBoolean("/gi2taxid")
        Dim x2taxid As String = args("/x2taxid")
        Dim skipExists As Boolean = args.GetBoolean("/skip-exists")
        Dim xid$() = taxdata _
            .Select(Function(x) x.Reference) _
            .Select(AddressOf TaxidMaps.GetParser(gi2taxid).Invoke) _
            .Distinct _
            .ToArray
        Dim theme$ = args.GetValue("/theme-color", "Paired:c12")
        Dim parallel As Boolean = args.GetBoolean("/parallel")
        Dim taxBuildin As Boolean = args.GetBoolean("/tax-build-in")

        Call xid.FlushAllLines(out = EXPORT & "/reference_xid.txt")

        ' subset database
        Dim CLI$

        If gi2taxid Then
            CLI = $"/gi.Match /in {(+out).CLIPath} /gi2taxid {x2taxid.CLIPath} /out {(out = EXPORT & "/gi2taxid.dmp").CLIPath}"
        Else
            CLI = $"/accid2taxid.Match /in {(+out).CLIPath} /acc2taxid {x2taxid.CLIPath} /gb_priority /out {(out = EXPORT & "/acc2taxid.dmp").CLIPath}"
        End If

        If (+out).FileExists(ZERO_Nonexists:=True) Then
            If skipExists Then
                ' 已经存在数据了，则直接忽略掉
            Else
                Call New IORedirectFile(Apps.NCBI_tools.Path, CLI).Run()
            End If
        Else
            If taxBuildin Then
                ' 重新直接生成
                If taxdata(Scan0).Extensions.ContainsKey("taxid") Then
                    Call taxdata.Dump_x2taxid(gi2taxid).FlushAllLines(out)
                Else
                    Call New IORedirectFile(Apps.NCBI_tools.Path, CLI).Run()
                End If
            Else
                Call New IORedirectFile(Apps.NCBI_tools.Path, CLI).Run()
            End If
        End If

        ' step1
        Dim notFound As New List(Of String)
        Dim ssuTax As IEnumerable(Of BlastnMapping) = taxdata.TaxonomyMaps(
            x2taxid:=+out,
            is_gi2taxid:=gi2taxid,
            notFound:=notFound,
            taxonomy:=New NcbiTaxonomyTree(taxonomy))

        ' step2
        Dim matrix As IEnumerable(Of DataSet)

        If net.ReadFirstLine.MatchPattern("BLASTN \d+\.\d+\.", RegexICSng) Then  ' blastn raw data
            Dim netdata As v228 = BlastPlus.Parser.ParsingSizeAuto(net)
            matrix = netdata.BuildMatrix(identities, coverage)
        Else
            Dim maps As IEnumerable(Of BlastnMapping) =
                net.AsLinq(Of BlastnMapping)(parallel:=True)
            matrix = maps.BuildMatrix(identities, coverage)
        End If

        ' step3
        Dim network As NetworkTables = Protocol.BuildNetwork(matrix, ssuTax, theme, parallel)

        ' 第一步的iterator直到第三布的时候才会被执行，所以这个列表要放在最后面保存，否则会没有数据
        Call notFound.FlushAllLines(EXPORT & "/taxonomy_notfound.txt")

        Return network.Save(EXPORT, Encoding.ASCII).CLICode
    End Function
End Module

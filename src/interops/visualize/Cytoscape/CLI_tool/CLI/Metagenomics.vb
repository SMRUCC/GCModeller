#Region "Microsoft.VisualBasic::7af52f23cc41cfdb83dd6c87f3b94ef0, ..\interops\visualize\Cytoscape\CLI_tool\CLI\Metagenomics.vb"

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
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream
Imports Microsoft.VisualBasic.Linq.Extensions
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.Interops.NCBI.Extensions
Imports SMRUCC.genomics.Model.Network.BLAST.API
Imports SMRUCC.genomics.Model.Network.BLAST.BBHAPI
Imports SMRUCC.genomics.Model.Network.BLAST.LDM

Partial Module CLI

    <ExportAPI("/bbh.Trim.Indeitites",
               Usage:="/bbh.Trim.Indeitites /in <bbh.csv> [/identities <0.3> /out <out.csv>]")>
    <Group(CLIGrouping.Metagenomics)>
    Public Function BBHTrimIdentities(args As CommandLine) As Integer
        Dim inFile As String = args("/in")
        Dim identities As Double = args.GetValue("/identities", 0.3)
        Dim out As String = args.GetValue("/out", inFile.TrimSuffix & $".identities.{identities}.csv")

        Using IO As New DocumentStream.Linq.WriteStream(Of BBH)(out)
            Dim reader As New DocumentStream.Linq.DataStream(inFile)
            Call reader.ForEachBlock(Of BBH)(Sub(data0)
                                                 data0 = (From x In data0.AsParallel Where x.Identities >= identities Select x).ToArray
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

        Using read As New DocumentStream.Linq.DataStream(inFile)
            Call read.ForEachBlock(Of LocalBLAST.Application.BBH.BestHit)(
                invoke:=Sub(block As LocalBLAST.Application.BBH.BestHit()) Call lstSBH.AddRange((From x In block.AsParallel Where x.evalue <= evalue Select x).ToArray),
                blockSize:=51200 * 2)
        End Using

        Dim simpleBBHArray = BBHHits(lstSBH)
        Using IO As New DocumentStream.Linq.WriteStream(Of BBH)(out)
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

    <ExportAPI("/MAT2NET", Usage:="/MAT2NET /in <mat.csv> [/out <net.csv> /cutoff 0]")>
    <Group(CLIGrouping.Metagenomics)>
    Public Function MatrixToNetwork(args As CommandLine) As Integer
        Dim inFile As String = args("/in")
        Dim out As String = args.GetValue("/out", inFile.TrimSuffix & ".network.Csv")
        Dim Csv = DocumentStream.File.Load(Path:=inFile)
        Dim ids As String() = Csv.First.Skip(1).ToArray
        Dim net As New List(Of NetworkEdge)
        Dim cutoff As Double = args.GetDouble("/cutoff")

        For Each row As DocumentStream.RowObject In Csv.Skip(1)
            Dim from As String = row.First
            Dim values As Double() = row.Skip(1).ToArray(Function(x) Val(x))

            For i As Integer = 0 To ids.Length - 1
                Dim n As Double = values(i)

                If n <> 0R AndAlso Not Double.IsNaN(n) AndAlso n <= cutoff Then
                    Dim edge As New NetworkEdge With {
                        .FromNode = from,
                        .Confidence = n,
                        .ToNode = ids(i)
                    }
                    Call net.Add(edge)
                End If
            Next
        Next

        Return net.SaveTo(out).CLICode
    End Function
End Module

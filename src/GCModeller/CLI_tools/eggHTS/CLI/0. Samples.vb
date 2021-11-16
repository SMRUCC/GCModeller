#Region "Microsoft.VisualBasic::11fb03982a0bcbd0326da5bc6cf12c0d, CLI_tools\eggHTS\CLI\0. Samples.vb"

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
    '     Function: (+2 Overloads) AddReMapping, DataAddORF, DataAddUniprotIDs, NormalizeSpecies_samples, pimwScatterPlot
    '               StripShotgunData
    ' 
    ' /********************************************************************************/

#End Region

Imports System.ComponentModel
Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Data.ChartPlots
Imports Microsoft.VisualBasic.Data.ChartPlots.csv
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Imaging.Driver
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.MIME.Html.CSS
Imports SMRUCC.genomics.Analysis.HTS.Proteomics
Imports SMRUCC.genomics.Assembly
Imports SMRUCC.genomics.Assembly.Uniprot.Web
Imports SMRUCC.genomics.Assembly.Uniprot.XML
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH.Abstract

Partial Module CLI

    <ExportAPI("/Shotgun.Data.Strip", Usage:="/Shotgun.Data.Strip /in <data.csv> [/out <output.csv>]")>
    <Group(CLIGroups.Samples_CLI)>
    Public Function StripShotgunData(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim out As String = args.GetValue("/out", [in].TrimSuffix & ".Data.csv")
        Return iTraq_csvReader.StripCsv([in]).Save(out,)
    End Function

    ''' <summary>
    ''' 假若蛋白质组的原始检测结果之中含有多个物种的蛋白，则可以使用这个方法利用bbh将其他的物种的蛋白映射回某一个指定的物种的蛋白
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/Sample.Species.Normalization",
               Usage:="/Sample.Species.Normalization /bbh <bbh.csv> /uniprot <uniprot.XML/DIR> /idMapping <refSeq2uniprotKB_mappings.tsv> /sample <sample.csv> [/Description <Description.FileName> /ID <columnName> /out <out.csv>]")>
    <ArgumentAttribute("/bbh", False, CLITypes.File,
              Description:="The queryName should be the entry accession ID in the uniprot and the subject name is the refSeq proteinID in the NCBI database.")>
    <Group(CLIGroups.Samples_CLI)>
    Public Function NormalizeSpecies_samples(args As CommandLine) As Integer
        Dim bbh As String = args <= "/bbh"
        Dim uniprot As String = args <= "/uniprot"
        Dim mappings As String = args <= "/idMapping"
        Dim sample As String = args <= "/sample"
        Dim out As String = args.GetValue("/out", sample.TrimSuffix & "-sample.species.normalization.csv")
        Dim sampleData As IEnumerable(Of EntityObject) = EntityObject.LoadDataSet(sample, args("/ID").DefaultValue)
        Dim mappingsID As Dictionary(Of String, String()) = Retrieve_IDmapping.MappingReader(mappings)
        Dim output As New List(Of EntityObject)
        Dim bbhData As Dictionary(Of String, BBHIndex) = bbh _
            .LoadCsv(Of BBHIndex) _
            .Where(Function(bh) bh.isMatched) _
            .ToDictionary(Function(bh)
                              Return bh.QueryName.Split("|"c).First
                          End Function)
        Dim uniprotTable As Dictionary(Of Uniprot.XML.entry) = UniProtXML.LoadDictionary(uniprot)
        Dim describKey As String = args("/Description")
        Dim ORF$

        If Not describKey.StringEmpty Then
            Call $"Substitute description in field: '{describKey}'.".Warning
        End If

        For Each protein As EntityObject In sampleData
            Dim uniprotHit_raw As String = protein.ID

            ' 如果uniprot能够在bbh数据之中查找到，则说明为其他物种的数据，需要进行映射
            If bbhData.ContainsKey(protein.ID) Then
                Dim bbhHit As String = bbhData(protein.ID).HitName

                ' 然后在id_mapping表之中进行查找
                If Not bbhHit.StringEmpty AndAlso mappingsID.ContainsKey(bbhHit) Then
                    ' 存在则更新数据
                    Dim uniprotData As Uniprot.XML.entry = uniprotTable(mappingsID(bbhHit).First)

                    protein.ID = DirectCast(uniprotData, INamedValue).Key
                    ORF = uniprotData.ORF
                    If ORF.StringEmpty Then
                        ORF = protein.ID
                    End If
                    protein.Properties.Add("ORF", ORF)
                    If Not describKey.StringEmpty Then
                        protein(describKey) = uniprotData.proteinFullName
                    End If
                Else
                    ORF = bbhHit
                    protein.Properties.Add("ORF", ORF)
                End If

                ' 可能有些编号在uniprot之中还不存在，则记录下来这个id
                protein.Properties.Add("bbh", bbhHit)
            Else
                ' 直接查找
                Dim uniprotData As Uniprot.XML.entry = uniprotTable(protein.ID)
                ORF = uniprotData.ORF
                If ORF.StringEmpty Then
                    ORF = DirectCast(uniprotData, INamedValue).Key
                End If
                protein.Properties.Add("ORF", ORF)
            End If

            protein.Properties.Add(NameOf(uniprotHit_raw), uniprotHit_raw)
            output += protein
        Next

        Return output.SaveTo(out).CLICode
    End Function

    <ExportAPI("/Data.Add.Mappings",
               Usage:="/Data.Add.Mappings /in <data.csv> /bbh <bbh.csv> /ID.mappings <uniprot.ID.mappings.tsv> /uniprot <uniprot.XML> [/ID <fieldName> /out <out.csv>]")>
    <Group(CLIGroups.Samples_CLI)>
    Public Function AddReMapping(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim bbh As String = args("/bbh")
        Dim IDMappings As String = args("/ID.mappings")
        Dim uniprot As String = args("/uniprot")
        Dim out As String = args.GetValue("/out", [in].TrimSuffix & ".ID.mappings.csv")
        Dim ID As String = args("/ID")
        Dim data = EntityObject.LoadDataSet([in], uidMap:=ID)
        Dim bbhData As IEnumerable(Of BBHIndex) = bbh.LoadCsv(Of BBHIndex)
        Dim IDMappingsTable = Retrieve_IDmapping.MappingReader(IDMappings)
        Dim result As EntityObject() = data.AddReMapping(bbhData, IDMappingsTable, uniprot)

        Return result.SaveTo(out).CLICode
    End Function

    ''' <summary>
    ''' 有时候并不需要重新计算数据，则使用这个函数在原来的数据基础上面添加重新映射的结果即可
    ''' </summary>
    ''' <param name="proteins"></param>
    ''' <returns></returns>
    <Extension>
    Public Function AddReMapping(proteins As IEnumerable(Of EntityObject), bbh As IEnumerable(Of BBHIndex), mappingsID As Dictionary(Of String, String()), uniprot As String) As EntityObject()
        Dim output As New List(Of EntityObject)
        Dim bbhData As Dictionary(Of String, BBHIndex) = bbh _
            .Where(Function(bh)
                       Return bh.isMatched
                   End Function) _
            .ToDictionary(Function(bh) bh.QueryName.Split("|"c).First)
        Dim uniprotTable As Dictionary(Of Uniprot.XML.entry) = UniProtXML.LoadDictionary(uniprot)
        Dim ORF$

        ' 由于只是为了添加重新mapping的信息，所以在这里不需要进行ID替换
        For Each protein As EntityObject In proteins

            ' 如果uniprot能够在bbh数据之中查找到，则说明为其他物种的数据，需要进行映射
            If bbhData.ContainsKey(protein.ID) Then
                Dim bbhHit As String = bbhData(protein.ID).HitName

                ' 然后在id_mapping表之中进行查找
                If Not bbhHit.StringEmpty AndAlso mappingsID.ContainsKey(bbhHit) Then
                    ' 存在则更新数据
                    Dim uniprotData As Uniprot.XML.entry = uniprotTable(mappingsID(bbhHit).First)
                    ORF = uniprotData.ORF
                    If ORF.StringEmpty Then
                        ORF = DirectCast(uniprotData, INamedValue).Key
                    End If
                    protein.Properties.Add("ORF", ORF)
                Else
                    ORF = bbhHit
                    protein.Properties.Add("ORF", ORF)
                End If
            Else
                ' 直接查找
                Dim uniprotData As Uniprot.XML.entry = uniprotTable(protein.ID)
                ORF = uniprotData.ORF
                If ORF.StringEmpty Then
                    ORF = DirectCast(uniprotData, INamedValue).Key
                End If
                protein.Properties.Add("ORF", ORF)
            End If

            output += protein
        Next

        Return output
    End Function

    ''' <summary>
    ''' 可能uniprot的标识符的识别度会比较低，在这里添加上比较熟悉的ORF编号
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/Data.Add.ORF", Usage:="/Data.Add.ORF /in <data.csv> /uniprot <uniprot.XML> [/ID <fieldName> /out <out.csv>]")>
    <Group(CLIGroups.Samples_CLI)>
    Public Function DataAddORF(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim uniprot As String = args("/uniprot")
        Dim out As String = args.GetValue("/out", [in].TrimSuffix & ".ORF_ID.csv")
        Dim ID As String = args("/ID")
        Dim data = EntityObject.LoadDataSet([in], uidMap:=ID).ToArray
        Dim uniprotTable = UniProtXML.LoadDictionary(uniprot)

        For Each protein As EntityObject In data
            uniprot = protein.ID

            If uniprotTable.ContainsKey(uniprot) Then
                Dim gene As String = uniprotTable(uniprot).ORF

                If Not gene.StringEmpty Then
                    Call protein.Properties.Add("ORF", gene)
                Else
                    Call protein.Properties.Add("ORF", "*")
                End If
            End If
        Next

        Return data.SaveTo(out).CLICode
    End Function

    <ExportAPI("/Data.Add.uniprotIDs",
               Usage:="/Data.Add.uniprotIDs /in <annotations.csv> /data <data.csv> [/out <out.csv>]")>
    <Group(CLIGroups.Samples_CLI)>
    Public Function DataAddUniprotIDs(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim data As String = args("/data")
        Dim out As String = args.GetValue("/out", data.TrimSuffix & ".uniprotIDs.csv")
        Dim annotations = EntityObject.LoadDataSet([in]).ToDictionary
        Dim dataTable = EntityObject.LoadDataSet(data)

        For Each protein As EntityObject In dataTable
            Call protein.Properties.Add("uniprot", annotations(protein.ID)("uniprot"))
        Next

        Return dataTable.SaveTo(out).CLICode
    End Function

    <ExportAPI("/plot.pimw")>
    <Description("'calc. pI' - 'MW [kDa]' scatter plot of the protomics raw sample data.")>
    <Usage("/plot.pimw /in <samples.csv> [/field.pi <default=""calc. pI""> /field.mw <default=""MW [kDa]""> /legend.fontsize <20> /legend.size (100,30) /quantile.removes <default=1> /out <pimw.png> /size <1600,1200> /color <black> /ticks.Y <-1> /pt.size <8>]")>
    <ArgumentAttribute("/field.pi", True, CLITypes.String,
              AcceptTypes:={GetType(String)},
              Description:="The field name that records the pI value of the protein samples, default is using iTraq result out format: ``calc. pI``")>
    <ArgumentAttribute("/field.mw", True, CLITypes.String,
              AcceptTypes:={GetType(String)},
              Description:="The field name that records the MW value of the protein samples, default is using iTraq result out format: ``MW [kDa]``")>
    <ArgumentAttribute("/pt.size", True, CLITypes.Double,
              AcceptTypes:={GetType(Double)},
              Description:="The radius size of the point in this scatter plot.")>
    <ArgumentAttribute("/size", True, CLITypes.String,
              AcceptTypes:={GetType(Size), GetType(SizeF)},
              Description:="The plot image its canvas size of this scatter plot.")>
    <ArgumentAttribute("/quantile.removes", True, CLITypes.Double,
              AcceptTypes:={GetType(Double)},
              Description:="All of the Y sample value greater than this quantile value will be removed. By default is quantile 100%, means no cuts.")>
    <Group(CLIGroups.Samples_CLI)>
    Public Function pimwScatterPlot(args As CommandLine) As Integer
        Dim [in] As String = args <= "/in"
        Dim pi$ = args.GetValue("/field.pi", "calc. pI")
        Dim mw$ = args.GetValue("/field.mw", "MW [kDa]")
        Dim size$ = args("/size") Or "1600,1200"
        Dim out As String = args.GetValue("/out", [in].TrimSuffix & ".pI_MW.png")
        Dim color As String = args("/color") Or "black"
        Dim ptSize! = args.GetValue("/pt.Size", 8.0!)
        Dim legendFontSize! = args("/legend.fontsize") Or 20.0#
        Dim legendSize$ = args("/legend.size") Or "100,30"
        Dim quantileRemoves# = args.GetValue("/quantile.removes", 1.0#)
        Dim yTicks# = args.GetValue("/ticks.Y", -1.0R)
        Dim res As GraphicsData = {
            ScatterSerials(File.Load([in]), pi, mw, color, ptSize) _
            .RemovesYOutlier(q:=quantileRemoves)
        }.Plot(size:=size,
               padding:="padding:100px 100px 150px 200px;",
               drawLine:=False,
               XaxisAbsoluteScalling:=True,
               absoluteScaling:=False,
               legendFontCSS:=CSSFont.GetFontStyle("Microsoft YaHei", FontStyle.Regular, legendFontSize),
               legendSize:=legendSize,
               Xlabel:="Calc.pI",
               Ylabel:="MW [kDa]",
               ticksY:=yTicks,
               legendRegionBorder:=New Stroke With {
                   .fill = "Black",
                   .dash = DashStyle.Solid,
                   .width = 2
               },
               htmlLabel:=False,
               densityColor:=color.TextEquals("density"),
               tickFontStyle:=CSSFont.Win7LargeBold,
               labelFontStyle:=CSSFont.Win7Large)

        Return res.Save(out).CLICode
    End Function
End Module

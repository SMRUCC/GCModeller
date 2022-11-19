#Region "Microsoft.VisualBasic::b54322293aa551ca196e201b4c93c821, GCModeller\visualize\ChromosomeMap\Data\Extensions.vb"

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


    ' Code Statistics:

    '   Total Lines: 122
    '    Code Lines: 85
    ' Comment Lines: 24
    '   Blank Lines: 13
    '     File Size: 6.28 KB


    ' Module Extensions
    ' 
    '     Function: __addMotifSites, (+2 Overloads) AddMotifSites, AddTSSs, LoadTSSs, TryExportMyva
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat
Imports SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis.GenomeMotifFootPrints
Imports SMRUCC.genomics.Interops.NBCR.MEME_Suite.DocumentFormat
Imports SMRUCC.genomics.Interops.NCBI.Extensions.Pipeline.COG
Imports SMRUCC.genomics.Model.Network.VirtualFootprint.DocumentFormat
Imports SMRUCC.genomics.Visualize.ChromosomeMap.DrawingModels

Public Module Extensions

    <ExportAPI("MyvaCOG.Export.From.PTT",
           Info:="If the ptt data file containing the COG information of the genes, then you can using this function to export the COG information from the ptt file using for the downstream visualization.")>
    Public Function TryExportMyva(data As PTTDbLoader) As MyvaCOG()
        ' Return data.ExportCOGProfiles(Of RpsBLAST.MyvaCOG)()
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="model"></param>
    ''' <param name="data">可以使用<see cref="GroupMotifs"></see>方法来合并一些重复的motif数据</param>
    ''' <param name="onlyRegulations">如果为真，则仅会将有调控因子的位点进行转换，如果为假，则所有的位点都会被绘制出来</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    ''' 
    <ExportAPI("Add.Motif_Sites",
               Info:="Trim.Regulation: if TRUE, then only the motif site which has the regulator will be drawing on the map, else all of the motif site will be drawing on the maps.")>
    Public Function AddMotifSites(model As ChromesomeDrawingModel,
                                  data As IEnumerable(Of PredictedRegulationFootprint),
                                  <Parameter("Trim.Regulation", "If the parameter is TRUE, then only the site data with regulation data that will be drawn.")>
                                  Optional onlyRegulations As Boolean = True) As ChromesomeDrawingModel

        If onlyRegulations Then
            data = (From footprint As PredictedRegulationFootprint
                    In data.AsParallel
                    Where Not String.IsNullOrEmpty(footprint.Regulator)'s.IsNullOrEmpty
                    Select footprint).ToArray
        End If

        model = __addMotifSites(model, data:=(From footprint As PredictedRegulationFootprint
                                              In data
                                              Select New KeyValuePair(Of String(), PredictedRegulationFootprint)({footprint.Regulator}, footprint)).ToArray)
        Return model
    End Function

    ''' <summary>
    ''' 将TSS位点以Motif位点的形式添加到绘图模型之上
    ''' </summary>
    ''' <param name="Model"></param>
    ''' <param name="Sites"></param>
    ''' <returns></returns>
    <ExportAPI("Add.TSSs")>
    Public Function AddTSSs(Model As ChromesomeDrawingModel, <Parameter("TSSs.Sites")> Sites As IEnumerable(Of Transcript)) As ChromesomeDrawingModel
        With Model
            Dim TSSs = LinqAPI.Exec(Of DrawingModels.TSSs) <=
                From t As Transcript
                In Sites
                Select New DrawingModels.TSSs With {
                    .Left = t.TSSs,
                    .Comments = t.Position,
                    .Right = t.TSSs + (CInt(t.MappingLocation.Strand) * 10),
                    .Strand = t.MappingLocation.Strand,
                    .SiteName = t.TSS_ID,
                    .Synonym = t.Synonym
                }

            .TSSs = TSSs
        End With

        Return Model
    End Function

    <ExportAPI("Read.Csv.TSSs")>
    Public Function LoadTSSs(path As String) As Transcript()
        Return path.LoadCsv(Of Transcript)(False).ToArray
    End Function

    Private Function __addMotifSites(Of T As VirtualFootprints)(
                                    model As ChromesomeDrawingModel,
                                    data As IEnumerable(Of KeyValuePair(Of String(), T))) As ChromesomeDrawingModel

        Dim ColorProfiles As New ColorProfiles((From item In data Select VirtualFootprintAPI.FamilyFromId(item.Value) Distinct).ToArray, DefaultColor:=Color.Black)
        Dim LQuery = (From siteEntry As KeyValuePair(Of String(), T) In data.AsParallel
                      Let footprint = siteEntry.Value
                      Let sf As String = VirtualFootprintAPI.FamilyFromId(footprint)
                      Select New MotifSite With {
                          .MotifName = sf,
                          .Regulators = If(siteEntry.Key.IsNullOrEmpty, New String() {}, siteEntry.Key),
                          .Color = ColorProfiles(sf),
                          .Left = footprint.Starts, .Right = footprint.Ends,
                          .Strand = footprint.Strand.First,
                          .Comments = If(siteEntry.Key.IsNullOrEmpty, "", String.Join(", ", siteEntry.Key)),
                          .SiteName = footprint.MotifId}).ToArray
        Call model.MotifSiteColors.SetValue(ColorProfiles.ColorProfiles)
        model.MotifSites = LQuery
        Return model
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="model"></param>
    ''' <param name="data">可以使用<see cref="GroupMotifs"></see>方法来合并一些重复的motif数据</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    ''' 
    <ExportAPI("add.motif_sites",
               Info:="only_regulations: if TRUE, then only the motif site which has the regulator will be drawing on the map, else all of the motif site will be drawing on the maps.")>
    Public Function AddMotifSites(model As ChromesomeDrawingModel,
                                  <Parameter("Data.VirtualFootprint")>
                                  data As IEnumerable(Of VirtualFootprints)) As ChromesomeDrawingModel

        Dim DataSource = (From footprint As VirtualFootprints
                          In data.AsParallel
                          Select New KeyValuePair(Of String(), VirtualFootprints)(Nothing, footprint)).ToArray
        Return __addMotifSites(Of VirtualFootprints)(model, DataSource)
    End Function
End Module

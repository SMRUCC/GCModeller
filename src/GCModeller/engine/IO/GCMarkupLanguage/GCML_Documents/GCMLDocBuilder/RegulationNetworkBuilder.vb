#Region "Microsoft.VisualBasic::32312d5e98cb6b03f843663429d10828, GCModeller\engine\IO\GCMarkupLanguage\GCML_Documents\GCMLDocBuilder\RegulationNetworkBuilder.vb"

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

    '   Total Lines: 95
    '    Code Lines: 71
    ' Comment Lines: 10
    '   Blank Lines: 14
    '     File Size: 6.28 KB


    '     Class RegulationNetworkBuilder
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '         Function: GetTransUnit, Invoke
    ' 
    '         Sub: BuildRegulationNetwork
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ApplicationServices.Terminal.STDIO
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.GCML_Documents.XmlElements.Bacterial_GENOME

Namespace Builder

    Public Class RegulationNetworkBuilder : Inherits Builder.IBuilder

        Sub New(MetaCyc As SMRUCC.genomics.Assembly.MetaCyc.File.FileSystem.DatabaseLoadder, Model As BacterialModel)
            MyBase.New(MetaCyc, Model)
        End Sub

        Public Overrides Function Invoke() As BacterialModel
            Call BuildRegulationNetwork(MetaCyc, Model)

            Return Me.Model
        End Function

        ''' <summary>
        ''' 创建基因表达调控网络，在构造出了基因对象和转录单元对象之后进行调用
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub BuildRegulationNetwork(MetaCyc As SMRUCC.genomics.Assembly.MetaCyc.File.FileSystem.DatabaseLoadder, Model As BacterialModel)
            Dim Regulations As SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles.Regulations = MetaCyc.GetRegulations
            Dim AllGeneList As String() = MetaCyc.GetGenes.Index
            Dim TUList As List(Of GCML_Documents.XmlElements.Bacterial_GENOME.TranscriptUnit) = Model.BacteriaGenome.TransUnits.AsList

            For Each regulation In Regulations
                Select Case GCML_Documents.XmlElements.SignalTransductions.Regulator.GetRegulationsType(regulation.Types)
                    Case GCML_Documents.XmlElements.SignalTransductions.Regulator.RegulationTypes.TranscriptionRegulation Or GCML_Documents.XmlElements.SignalTransductions.Regulator.RegulationTypes.TranslationRegulation '在转录单元对象中查找
                        Dim Regulator As GCML_Documents.XmlElements.SignalTransductions.Regulator = New GCML_Documents.XmlElements.SignalTransductions.Regulator With
                                                     {
                                                         .Identifier = regulation.Identifier,
                                                         .Activation = String.Equals(regulation.Mode, "+"),
                                                         .CommonName = regulation.CommonName}
                        Dim Target = GetTransUnit(regulation)

                        If Target Is Nothing Then Continue For

                        For Each TU In Target '一个基因对象，可能会包含在多个转录单元之中
                            TU._add_Regulator(regulation.RegulatedEntity, Regulator)
                        Next
                    Case GCML_Documents.XmlElements.SignalTransductions.Regulator.RegulationTypes.EnzymeActivityRegulation '在代谢网络中查找
                    Case GCML_Documents.XmlElements.SignalTransductions.Regulator.RegulationTypes.Regulation '未知，仅做下记录
                        Call printf("[WARN] Unknown regulation: '%s'", regulation.Identifier)
                End Select
            Next
        End Sub

        ''' <summary>
        ''' 根据Regulation中的对象查找出相应的转录单元对象
        ''' </summary>
        ''' <param name="Regulation"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function GetTransUnit(Regulation As SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles.Slots.Regulation) As GCML_Documents.XmlElements.Bacterial_GENOME.TranscriptUnit()
            Dim RegulatedObject As SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles.Slots.Object = Regulation.GetRegulatedObject(MetaCyc)  '获取目标被调控对象
            Dim [Handles] = RegulatedObject.GetHandles(Model)

            If [Handles].IsNullOrEmpty Then
                If RegulatedObject.Table = SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles.Slots.Object.Tables.genes Then '目标对象为一个基因，并且没有与之相对应的转录单元对象，则创建一个新的转录单元对象
                    Printf("[INFO] TU_NOT_FOUND: create a new transcript unit for gene object: %s", RegulatedObject.Identifier)

                    Dim Gene = RegulatedObject.Select(Of SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles.Slots.Gene, GeneObject)(Model.BacteriaGenome.Genes)
                    Dim TU As GCML_Documents.XmlElements.Bacterial_GENOME.TranscriptUnit =
                        New GCML_Documents.XmlElements.Bacterial_GENOME.TranscriptUnit With
                        {
                            .GeneCluster = New KeyValuePair() {
                                New KeyValuePair With {
                                    .Value = Gene.AccessionId}
                        }, .Identifier = Gene.Identifier & "_TU"} '转录单元中进含有单个基因对象
                    Call Model.BacteriaGenome.TransUnits.Add(TU)

                    Return New GCML_Documents.XmlElements.Bacterial_GENOME.TranscriptUnit() {TU}
                End If
            Else
                If RegulatedObject.Table = SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles.Slots.Object.Tables.transunits Then
                    Return Take(Of SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles.Slots.TransUnit, GCMarkupLanguage.GCML_Documents.XmlElements.Bacterial_GENOME.TranscriptUnit)(Model.BacteriaGenome.TransUnits, [Handles])
                ElseIf RegulatedObject.Table = SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles.Slots.Object.Tables.genes Then
                    Console.WriteLine("[NOT_IMPLEMENTS] takes.genes")
                ElseIf RegulatedObject.Table = SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles.Slots.Object.Tables.promoters Then
                    Console.WriteLine("[NOT_IMPLEMENTS] takes.promoters")
                ElseIf RegulatedObject.Table = SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles.Slots.Object.Tables.terminators Then
                    Console.WriteLine("[NOT_IMPLEMENTS] takes.terminators")
                ElseIf RegulatedObject.Table = SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles.Slots.Object.Tables.trna Then
                    Console.WriteLine("[NOT_IMPLEMENTS] takes.trna")
                Else
                    Return Nothing
                End If
            End If

            Return Nothing
        End Function
    End Class
End Namespace

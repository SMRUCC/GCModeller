﻿#Region "Microsoft.VisualBasic::f445ccf751501555710cb66ffb114139, data\RegulonDatabase\Regprecise\WebServices\WebParser\TranscriptionFactors.vb"

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

    '     Class TranscriptionFactors
    ' 
    '         Properties: genomes, update
    ' 
    '         Function: BuildRegulatesHash, Export_TFBSInfo, Get_Regulators, GetBacteriaGenomeProfile, GetRegulatorId
    '                   GetRegulators, InsertRegulog, ListAllRegulators, Load, Save
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Data.Regprecise.WebServices
Imports SMRUCC.genomics.SequenceModel

Namespace Regprecise

    ''' <summary>
    ''' [Regprecise database] [Collections of regulogs classified by transcription factors]
    ''' Each transcription factor collection organizes all reconstructed regulogs for a given set of orthologous
    ''' regulators across different taxonomic groups of microorganisms. These collections represent regulons for
    ''' a selected subset of transcription factors. The collections include both widespread transcription factors
    ''' such as NrdR, LexA, and Zur, that are present in more than 25 diverse taxonomic groups of Bacteria, and
    ''' narrowly distributed transcription factors such as Irr and PurR. The TF regulon collections are valuable
    ''' for comparative and evolutionary analysis of TF binding site motifs and regulon content for orthologous
    ''' transcription factors.
    ''' </summary>
    ''' <remarks></remarks>
    '''
    <XmlRoot("TranscriptionFactors", Namespace:="http://regprecise.lbl.gov/RegPrecise/")>
    Public Class TranscriptionFactors : Inherits ITextFile

        <XmlElement>
        Public Property genomes As BacteriaRegulome()
        <XmlAttribute("update")>
        Public Property update As String

        Public Overrides Function Save(Optional FilePath As String = "", Optional Encoding As Encoding = Nothing) As Boolean
            Return Me.GetXml.SaveTo(getPath(FilePath), Encoding)
        End Function

        Public Function InsertRegulog(Family As String,
                                      Bacteria As String,
                                      RegulatorSites As FASTA.FastaFile,
                                      RegulatorId As String) As Boolean
            Dim BacteriaGenome As BacteriaRegulome = GetBacteriaGenomeProfile(Bacteria)

            If BacteriaGenome Is Nothing Then '不存在这条记录，则插入新的记录
                Dim Regulog = New Regulon With {
                    .regulators = {CreateRegulator(Family, Bacteria, RegulatorSites, RegulatorId)}
                }
                BacteriaGenome = New BacteriaRegulome With {
                    .genome = New JSON.genome With {
                        .name = Bacteria
                    }
                }
                BacteriaGenome.regulons = Regulog
                genomes.Add(BacteriaGenome)

                Return True
            End If

            Dim Regulator = (From tf As Regulator In BacteriaGenome.regulons.regulators
                             Where String.Equals(tf.locus_tag.name, RegulatorId, StringComparison.OrdinalIgnoreCase)
                             Select tf).FirstOrDefault
            If Regulator Is Nothing Then
                Regulator = CreateRegulator(Family, Bacteria, RegulatorSites, RegulatorId)
                BacteriaGenome.regulons.regulators = Regulator.Join(BacteriaGenome.regulons.regulators).ToArray
            Else
                Dim RegulatorySites = (From FastaObject As FastaReaders.Site
                                       In FastaReaders.Site.CreateObject(RegulatorSites)
                                       Select FastaObject.ToFastaObject).ToArray
                Regulator.regulatorySites = {RegulatorySites, Regulator.regulatorySites}.ToVector
            End If

            Return True
        End Function

        Public Function GetBacteriaGenomeProfile(SpeciesName As String, Optional FuzzyMatch As Boolean = False) As BacteriaRegulome
            Dim LQuery = (From Bacteria As BacteriaRegulome
                          In Me.genomes.AsParallel
                          Where String.Equals(SpeciesName, Bacteria.genome.name, StringComparison.OrdinalIgnoreCase)
                          Select Bacteria).FirstOrDefault

            If Not LQuery Is Nothing Then Return LQuery

            If FuzzyMatch Then
                LQuery = (From Bacteria As BacteriaRegulome
                          In genomes.AsParallel
                          Where SpeciesName.FuzzyMatching(Bacteria.genome.name)
                          Select Bacteria).FirstOrDefault
                Return LQuery
            Else
                Return Nothing
            End If
        End Function

        Public Function GetRegulatorId(locus_tag As String, MotifPosition As Integer) As String
            For Each Genome As BacteriaRegulome In genomes
                Dim LQuery = (From Regulator As Regulator In Genome.regulons.regulators
                              Where Not Regulator.GetMotifSite(locus_tag, MotifPosition) Is Nothing
                              Select Regulator).FirstOrDefault
                If Not LQuery Is Nothing Then
                    Return LQuery.locus_tag.name
                End If
            Next

            Return ""
        End Function

        ''' <summary>
        ''' 相较于<see cref="GetRegulatorId"/>方法，其只是获取得到的第一个调控因子的编号，推荐使用这个方法来获取完整的调控因子的信息
        ''' </summary>
        ''' <param name="trace">locus_tag:position</param>
        ''' <returns></returns>
        Public Function GetRegulators(trace As String) As Regulator()
            For Each genome As BacteriaRegulome In genomes
                Dim LQuery As Regulator() = (From Regulator As Regulator In genome.regulons.regulators
                                             Where Not Regulator.GetMotifSite(trace) Is Nothing
                                             Select Regulator).ToArray
                If Not LQuery.IsNullOrEmpty Then
                    Return LQuery
                End If
            Next

            Return Nothing
        End Function

        Public Shared Function Load(File As String) As TranscriptionFactors
            Dim XmlFile = File.LoadXml(Of TranscriptionFactors)()
            XmlFile.FilePath = File
            Return XmlFile
        End Function

        Public Overloads Shared Widening Operator CType(FilePath As String) As TranscriptionFactors
            Return TranscriptionFactors.Load(FilePath)
        End Operator

        Public Function Export_TFBSInfo() As FASTA.FastaFile
            Dim TFBS_sites = (From Regulator As Regulator In Me.ListAllRegulators()
                              Where Regulator.type = Types.TF
                              Select (From site In Regulator.regulatorySites
                                      Select RegulatorId = Regulator.locus_tag.name,
                                          Regulator.family,
                                          Species = Strings.Split(Regulator.regulog.name, " - ").Last,
                                          Tfbs_siteInfo = site).ToArray).ToArray.ToVector
            Dim LQuery = (From Tfbs As Integer
                          In TFBS_sites.Sequence.AsParallel
                          Let SiteInfo = TFBS_sites(Tfbs)
                          Select GenerateFastaData(SiteInfo.Tfbs_siteInfo,
                                                   SiteInfo.Family,
                                                   SiteInfo.Species,
                                                   lcl:=Tfbs,
                                                   Regulator:=SiteInfo.RegulatorId)).ToArray
            Return New FASTA.FastaFile(LQuery)
        End Function

        Public Function ListAllRegulators() As Regulator()
            Dim LQuery = (From BacteriaGenome As BacteriaRegulome
                          In Me.genomes.AsParallel
                          Select BacteriaGenome.regulons.regulators).ToArray
            Return LQuery.ToVector
        End Function

        ''' <summary>
        ''' 选择所有的调控因子请使用<see cref="Get_Regulators"></see>
        ''' </summary>
        ''' <param name="Type"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Get_Regulators(Type As Types) As Regulator()
            Dim LQuery = (From genome As BacteriaRegulome
                          In Me.genomes.AsParallel
                          Select genome.regulons.regulators).ToArray
            Return (From reg As Regulator
                    In LQuery.IteratesALL.AsParallel
                    Where reg.type = Type
                    Select reg).ToArray
        End Function

        ''' <summary>
        ''' 生成映射{site, TF()}
        ''' </summary>
        ''' <returns></returns>
        Public Function BuildRegulatesHash() As Dictionary(Of String, String())
            Dim LQuery = (From g As BacteriaRegulome
                          In Me.genomes
                          Where Not g.regulons Is Nothing
                          Select g.regulons.regulators.Select(Function(x) (From site As Regtransbase.WebServices.MotifFasta
                                                                            In x.regulatorySites
                                                                           Select uid = $"{site.locus_tag}:{site.position}",
                                                                                x.LocusId)).IteratesALL).IteratesALL
            Dim Groups = (From x In LQuery
                          Select x
                          Group x By x.uid Into Group) _
                               .ToDictionary(Function(x) x.uid,
                                             Function(x) x.Group.Select(Function(s) s.LocusId).ToArray)
            Return Groups
        End Function
    End Class
End Namespace

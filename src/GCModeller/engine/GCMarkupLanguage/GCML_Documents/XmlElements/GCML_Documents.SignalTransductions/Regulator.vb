#Region "Microsoft.VisualBasic::1eaac1ed5b02a6214761b7c8199f16ac, ..\GCModeller\engine\GCMarkupLanguage\GCML_Documents\XmlElements\GCML_Documents.SignalTransductions\Regulator.vb"

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

Imports System.Text
Imports System.Xml.Serialization
Imports SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles
Imports SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.GCML_Documents.ComponentModels
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic

Namespace GCML_Documents.XmlElements.SignalTransductions

    ''' <summary>
    ''' 对基因表达调控起作用的对象分子，在这里是一个调控因子对一个调控对象
    ''' </summary>
    ''' <remarks></remarks>
    Public Class Regulator : Inherits T_MetaCycEntity(Of Slots.Regulation)

#Region "Public Properties"

        <XmlIgnore> Friend Shadows BaseType As Slots.Regulation.IRegulator
        <XmlElement("CommonName", Namespace:="http://code.google.com/p/genome-in-code/GCMarkupLanguage/GCML_Documents.XmlElements.SignalTransductions/")>
        Public Property CommonName As String

        ''' <summary>
        ''' 该调控因子的<see cref="ProteinAssembly.Identifier">蛋白质复合物的组成</see>
        ''' </summary>
        ''' <remarks></remarks>
        <XmlElement("ProteinAssembly_ID", Namespace:="http://code.google.com/p/genome-in-code/GCMarkupLanguage/GCML_Documents.XmlElements.SignalTransductions/ProteinAssembly/")>
        Public Property ProteinAssembly As String
        <XmlAttribute("T_ActivationEffects?")> Public Property Activation As Boolean
        <XmlAttribute> Public Property K_Dynamics As Double
        <XmlAttribute("PhysiologicallyRelevant?")> Public Property PhysiologicallyRelevant As Boolean
        <XmlElement("Regulator_T_TYPES", Namespace:="http://code.google.com/p/genome-in-code/GCMarkupLanguage/GCML_Documents.XmlElements.SignalTransductions/Event_Regulator/")>
        Public Property Types As String()
        ''' <summary>
        ''' 调控的目标对象
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Regulates As String

        ''' <summary>
        ''' 本调控因子对目标调控对象的权重(可以看作为皮尔森相关系数)，对于转录调控因子而言，Key属性指的是目标motif所处的TranscriptUnit的编号，
        ''' 与<see cref="Regulates"></see>所不同的是，<see cref="Regulates"></see>是所调控的motif的编号
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Weight As KeyValuePairObject(Of String, Double)
        Public Property Effector As String

#End Region

        Public Function Clone() As Regulator
            Return New Regulator With
                   {
                       .Activation = Activation, .BaseType = BaseType,
                       .K_Dynamics = K_Dynamics, .CommonName = CommonName,
                       .PhysiologicallyRelevant = PhysiologicallyRelevant,
                       .ProteinAssembly = ProteinAssembly,
                       .Types = Types.Clone,
                       .Identifier = Identifier,
                       .Weight = Weight,
                       .Regulates = Regulates,
                       .Effector = Effector}
        End Function

        Public Enum RegulationTypes
            Regulation
            TranscriptionRegulation
            TranslationRegulation
            EnzymeActivityRegulation
        End Enum

        Public Overrides Function ToString() As String
            Return Identifier
        End Function

        Public Shared Function GetRegulationsType(RegulationTypesHandle As Generic.IEnumerable(Of String)) As RegulationTypes
            Dim type As String = RegulationTypesHandle.First

            If String.Equals(type, "Regulation", StringComparison.OrdinalIgnoreCase) Then
                Return RegulationTypes.Regulation
            End If

            If Array.IndexOf(Regulator.TranscriptionalRegulationTypes, type) > -1 Then
                Return RegulationTypes.TranscriptionRegulation
            ElseIf Array.IndexOf(Regulator.TranslationRegulationTypes, type) > -1 Then
                Return RegulationTypes.TranslationRegulation
            ElseIf Array.IndexOf(Regulator.EnzymeActivityRegulationTypes, type) > -1 Then
                Return RegulationTypes.EnzymeActivityRegulation
            Else
                Return RegulationTypes.Regulation
            End If
        End Function

        '    ''' <summary>
        '    ''' 该调控作用是否为正调控作用？
        '    ''' </summary>
        '    ''' <remarks></remarks>
        '    <XmlAttribute> Public Active As Boolean

        '    ''' <summary>
        '    ''' 所调控的目标对象实体的句柄值（当调控对象为一个转录单元的时候，目标对象可能为一个基因的集合）
        '    ''' 【Regulation -> 调控对象为一个蛋白质】
        '    ''' 【Regulation-of-Enzyme-Activity -> 调控对象为一个酶促反应】
        '    ''' 【Regulation-of-Reactions -> 调控的对象为一个反应过程】
        '    ''' 【Allosteric-Regulation-of-RNAP -> 调控对象为一个启动子】
        '    ''' 【Protein-Mediated-Attenuation -> 调控对象为一个转录单元或者终止子】
        '    ''' 【Rho-Blocking-Antitermination -> 调控对象为一个终止子】
        '    ''' 【Ribosome-Mediated-Attenuation -> 调控对象为一个终止子】
        '    ''' 【Small-Molecule-Mediated-Attenuation -> 调控对象为一个转录单元或者终止子】
        '    ''' 【Transcriptional-Attenuation -> 调控对象为一个终止子】
        '    ''' 【Transcription-Factor-Binding -> 调控对象为一个启动子】
        '    ''' 【Compound-Mediated-Translation-Regulation -> 调控对象为一个转录单元】
        '    ''' 【Protein-Mediated-Translation-Regulation -> 调控对象为一个转录单元】
        '    ''' 【Regulation-of-Translation -> 调控对象为一个转录单元】
        '    ''' 【RNA-Mediated-Translation-Regulation -> 调控对象为一个基因或者转录单元】
        '    ''' </summary>
        '    ''' <remarks>
        '    ''' 基因，启动子，终止子 -> 转录单元
        '    ''' 
        '    ''' </remarks>
        '    <XmlAttribute> Public Property RegulatedEntity As String()
        '    ''' <summary>
        '    ''' 调控对象指针所指向的目标对象的类型
        '    ''' </summary>
        '    ''' <value></value>
        '    ''' <returns></returns>
        '    ''' <remarks></remarks>
        '    <XmlAttribute> Public Property [Class] As SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles.Slots.Object.Tables

        '    Public Overrides Function ToString() As String
        '        Dim sBuilder As StringBuilder = New StringBuilder
        '        For Each Hwnd In RegulatedEntity
        '            Call sBuilder.AppendFormat("{0}, ", Hwnd)
        '        Next
        '        Call sBuilder.Remove(sBuilder.Length - 2, 2)

        '        Return String.Format("{0} --> {1}; {2}", Type, sBuilder.ToString, If(Active, "Active Effect", "InActive Effect"))
        '    End Function

        Friend Shared ReadOnly TranscriptionalRegulationTypes As String() = {
            "Allosteric-Regulation-of-RNAP", "Protein-Mediated-Attenuation", "Rho-Blocking-Antitermination",
            "Ribosome-Mediated-Attenuation", "Small-Molecule-Mediated-Attenuation",
            "Transcriptional-Attenuation", "Transcription-Factor-Binding"}
        Friend Shared ReadOnly TranslationRegulationTypes As String() = {
            "Compound-Mediated-Translation-Regulation", "Protein-Mediated-Translation-Regulation",
            "Regulation-of-Translation", "RNA-Mediated-Translation-Regulation"}
        ''' <summary>
        ''' 【Regulation: protein activity regulation】
        ''' 【Regulation-of-Enzyme-Activity: enzymatic reaction regulation】
        ''' 【Regulation-of-Reactions: reaction regulation】
        ''' </summary>
        ''' <remarks></remarks>
        Friend Shared ReadOnly EnzymeActivityRegulationTypes As String() = {
            "Regulation", "Regulation-of-Enzyme-Activity", "Regulation-of-Reactions"}
        'End Class

        Public Shared Function CastTo(e As Slots.Regulation.IRegulator, Model As BacterialModel) As Regulator
            Dim Regulator As Regulator = New Regulator With {.BaseType = e}
            Regulator.Identifier = e.locusId
            Regulator.CommonName = e.CommonName
            Return Regulator
        End Function
    End Class
End Namespace

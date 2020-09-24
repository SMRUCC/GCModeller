#Region "Microsoft.VisualBasic::82931fd5f3b0281f70e6eddc791acb55, sub-system\FBA\FBA_DP\Simpheny\Tables.vb"

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

    '     Class DocumentElement
    ' 
    '         Properties: abbreviation, officialName
    ' 
    '         Function: ToString
    ' 
    '     Class GPR
    ' 
    '         Properties: equation, geneAssociation, proteinAssociation, proteinClass, proteinGeneAssociation
    '                     subSystem
    ' 
    '     Class Cmpd
    ' 
    '         Properties: casNumber, charge, CompoundNames, formula, formulaNeutral
    '                     KeggID, reviewStatus
    ' 
    '     Class HeadProperty
    ' 
    '         Properties: DatabaseName, ModelVersion, ProgramName, ProjectName, SimulationName
    '                     SimulationNumber, SimulationType, StudyName, StudyOwner
    ' 
    '         Function: LoadData, ToString
    ' 
    '     Class RXN
    ' 
    '         Properties: Abbreviation, Enzyme, LOWER_BOUND, Objective, OfficialName
    '                     ReactionId, Reversible, UPPER_BOUND
    ' 
    '         Function: LoadData
    ' 
    '     Class Met
    ' 
    '         Properties: Compartment, Metabolite, MetaboliteNumber, Molecule, ShadowPrice
    ' 
    '         Function: LoadData
    ' 
    '     Class Sto
    ' 
    '         Properties: DataVector
    ' 
    '         Function: LoadData
    ' 
    '     Class ProjectElement
    ' 
    '         Function: ToString
    ' 
    '     Class ProjectFile
    ' 
    '         Properties: Elements, Properties
    ' 
    '         Function: LoadDocument, LoadFile
    ' 
    '     Class Project
    ' 
    '         Properties: MAT, Metabolites, Reactions
    ' 
    '         Function: LoadProject
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Reflection
Imports System.Text
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.Settings
Imports Microsoft.VisualBasic.Data.csv.StorageProvider.Reflection

Namespace Simpheny

    Public MustInherit Class DocumentElement
        Public Property abbreviation As String
        Public Property officialName As String

        Public Overrides Function ToString() As String
            Return String.Format("({0})   {1}", abbreviation, officialName)
        End Function
    End Class

    Public Class GPR : Inherits DocumentElement

        Public Property equation As String
        Public Property subSystem As String
        Public Property proteinClass As String
        Public Property proteinGeneAssociation As String
        Public Property geneAssociation As String
        Public Property proteinAssociation As String

    End Class

    Public Class Cmpd : Inherits DocumentElement

        Public Property formula As String
        Public Property reviewStatus As String
        Public Property charge As String
        Public Property casNumber As String
        Public Property formulaNeutral As String
        Public Property CompoundNames As String
        Public Property KeggID As String

    End Class

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks>
    ''' '********************************************************************
    ''' '*
    ''' '* Database Name     : UCSD1.GENOMATICA.COM
    ''' '* Program Name      : Yeast
    ''' '* Project Name      : S. cerevisiae
    ''' '* Study Owner       : YEAST
    ''' '* Study Name        : Final iMM904 simulations
    ''' '* Model Version     : Saccharomyces cerevisiae S288C - 4.0 - iMM904 - YEAST - YEAST - (02/19/2006 10:42 AM).
    ''' '* Simulation Type   : Single Optimization
    ''' '* Simulation Number : 5
    ''' '* Simulation Name   : iMM904 GlcMM o2 s10 notrace
    ''' '*
    ''' '********************************************************************
    ''' </remarks>
    Public Class HeadProperty

        <SimpleConfig("* Database Name     : ")> Public Property DatabaseName As String
        <SimpleConfig("* Program Name      : ")> Public Property ProgramName As String
        <SimpleConfig("* Project Name      : ")> Public Property ProjectName As String
        <SimpleConfig("* Study Owner       : ")> Public Property StudyOwner As String
        <SimpleConfig("* Study Name        : ")> Public Property StudyName As String
        <SimpleConfig("* Model Version     : ")> Public Property ModelVersion As String
        <SimpleConfig("* Simulation Type   : ")> Public Property SimulationType As String
        <SimpleConfig("* Simulation Number : ")> Public Property SimulationNumber As String
        <SimpleConfig("* Simulation Name   : ")> Public Property SimulationName As String

        Public Overrides Function ToString() As String
            Return String.Format("({0})  {1}", StudyOwner, StudyName)
        End Function

        Public Shared Function LoadData(strDatas As Generic.IEnumerable(Of String)) As HeadProperty
            Dim Entries = (From p As PropertyInfo
                           In GetType(HeadProperty).GetProperties(BindingFlags.Instance Or BindingFlags.Public)
                           Let attrs As Object() = p.GetCustomAttributes(attributeType:=SimpleConfig.TypeInfo, inherit:=True)
                           Where Not attrs.IsNullOrEmpty
                           Select [property] = p, attribute = DirectCast(attrs.First, SimpleConfig)).ToArray
            Dim Entity As HeadProperty = New HeadProperty
            Dim l As Integer = Len(Entries.First.attribute.Name)

            For Each [Property] In Entries
                Dim LQuery = (From strLine As String In strDatas.AsParallel
                              Where InStr(strLine, [Property].attribute.ToString, CompareMethod.Text) = 1
                              Select Mid(strLine, l).Trim).ToArray
                If Not LQuery.IsNullOrEmpty Then
                    Call [Property].property.SetValue(Entity, value:=LQuery.First)
                End If
            Next

            Return Entity
        End Function
    End Class

    ''' <summary>
    ''' 代谢过程的模型
    ''' </summary>
    Public Class RXN : Inherits ProjectElement
        Implements INamedValue

        <Column("REACTION-ID")> Public Property ReactionId As String Implements INamedValue.Key
        <Column("ABBREVIATION")> Public Property Abbreviation As String
        <Column("OFFICIAL NAME")> Public Property OfficialName As String
        <Column("REVERSIBLE")> Public Property Reversible As Boolean
        <Column("LOWER_BOUND")> Public Property LOWER_BOUND As Double
        <Column("UPPER_BOUND")> Public Property UPPER_BOUND As Double
        <Column("OBJECTIVE")> Public Property Objective As Double
        Public Property Enzyme As String()

        Protected Friend Shared Function LoadData(strData As String) As ProjectElement()
            Throw New NotImplementedException
        End Function
    End Class

    Public Class Met : Inherits ProjectElement

        <Column("METABOLITE NUMBER")> Public Property MetaboliteNumber As String
        <Column("METABOLITE")> Public Property Metabolite As String
        <Column("MOLECULE")> Public Property Molecule As String
        <Column("COMPARTMENT")> Public Property Compartment As String
        <Column("SHADOW PRICE")> Public Property ShadowPrice As Double

        Protected Friend Shared Function LoadData(strData As String) As ProjectElement()
            Throw New NotImplementedException
        End Function
    End Class

    Public Class Sto : Inherits ProjectElement
        Public Property DataVector As Double()

        Protected Friend Shared Function LoadData(strData As String) As ProjectElement()
            Throw New NotImplementedException
        End Function
    End Class

    Public MustInherit Class ProjectElement
        Public Overrides Function ToString() As String
            Return "FBA::Simpheny::ProjectElement"
        End Function
    End Class

    Public Class ProjectFile(Of ElementType As ProjectElement)

        Public Property Properties As HeadProperty
        Public Property Elements As ElementType()

        Public Shared Function LoadFile(path As String) As ProjectFile(Of ElementType)
            Dim Document As ProjectFile(Of ElementType) = New ProjectFile(Of ElementType)
            Dim Elements = LoadDocument(FileIO.FileSystem.ReadAllText(path))
            Document.Properties = Elements.Key

            If GetType(ElementType) = GetType(Met) Then
                Document.Elements = Met.LoadData(Elements.Value)
            ElseIf GetType(ElementType) = GetType(Sto) Then
                Document.Elements = Sto.LoadData(Elements.Value)
            ElseIf GetType(ElementType) = GetType(RXN) Then
                Document.Elements = RXN.LoadData(Elements.Value)
            End If

            Return Document
        End Function

        Private Shared Function LoadDocument(strData As String) As KeyValuePair(Of HeadProperty, String)
            Dim Head As String = Regex.Match(strData, "\*{3,}.+?\*{3,}", RegexOptions.Singleline).Value
            Return New KeyValuePair(Of HeadProperty, String)(HeadProperty.LoadData(Strings.Split(Head, vbCrLf)), Mid(strData, Len(Head) + 1))
        End Function
    End Class

    Public Class Project

        Public Property Reactions As ProjectFile(Of RXN)
        Public Property Metabolites As ProjectFile(Of Met)
        Public Property MAT As ProjectFile(Of Sto)

        Public Shared Function LoadProject(dir As String) As Project
            Dim Project As Project = New Project
            Dim File As String

            File = FileIO.FileSystem.GetFiles(dir, FileIO.SearchOption.SearchTopLevelOnly, "*.met").FirstOrDefault
            If Not String.IsNullOrEmpty(File) Then Project.Metabolites = ProjectFile(Of Met).LoadFile(File)

            File = FileIO.FileSystem.GetFiles(dir, FileIO.SearchOption.SearchTopLevelOnly, "*.rxn").FirstOrDefault
            If Not String.IsNullOrEmpty(File) Then Project.Reactions = ProjectFile(Of RXN).LoadFile(File)

            File = FileIO.FileSystem.GetFiles(dir, FileIO.SearchOption.SearchTopLevelOnly, "*.sto").FirstOrDefault
            If Not String.IsNullOrEmpty(File) Then Project.MAT = ProjectFile(Of Sto).LoadFile(File)

            Return Project
        End Function
    End Class
End Namespace

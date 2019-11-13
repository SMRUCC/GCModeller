#Region "Microsoft.VisualBasic::d3b202e921a77138f11082d40dc36153, engine\IO\GCTabular\CsvTabularData\FileStream.vb"

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

    '     Class Pathway
    ' 
    '         Properties: Comment, Identifier, MetabolismFlux
    ' 
    '         Function: ToString
    ' 
    '     Class ExpressionKinetics
    ' 
    '         Properties: Km, pH, ProteinComplex, Temperature
    ' 
    '     Class ProteinAssembly
    ' 
    '         Properties: Comments, Lambda, p_Dynamics_K, ProteinComplexes, ProteinComponents
    '                     Upper_Bound
    ' 
    '         Function: ToString
    ' 
    '     Class Transcript
    ' 
    '         Properties: Comments, Lamda, PolypeptideCompositionVector, Product, Template
    '                     TranscriptCompositionVectors, TranscriptType, UniqueId
    ' 
    '         Function: ToString
    ' 
    '     Class Protein
    ' 
    '         Properties: ECNumber, Identifier, Lambda, PolypeptideCompositionVector, ProteinType
    ' 
    '         Function: ToString
    ' 
    '     Class MetabolismFlux
    ' 
    '         Properties: CommonName, EnzymeClass, Enzymes, Equation, Identifier
    '                     KEGGReaction, Left, LOWER_Bound, MetaCycId, p_Dynamics_K_1
    '                     p_Dynamics_K_2, Reversible, Right, UPPER_Bound
    ' 
    '         Function: CreateGCMLModel, CreateMetaCycReactionSchema, (+3 Overloads) CreateObject, get_Coefficient, get_Metabolites
    '                   ToString
    ' 
    '         Sub: ReCreateEquation
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Data.csv.StorageProvider.Reflection
Imports SMRUCC.genomics.Assembly
Imports SMRUCC.genomics.ComponentModel
Imports SMRUCC.genomics.ComponentModel.EquaionModel
Imports SMRUCC.genomics.ComponentModel.EquaionModel.DefaultTypes
Imports SMRUCC.genomics.GCModeller.Assembly
Imports SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.GCML_Documents.XmlElements
Imports SMRUCC.genomics.Model
Imports SMRUCC.genomics.Model.SBML.Level2.Elements

Namespace FileStream

    Public Class Pathway : Implements INamedValue

        <Column("UniqueId")> Public Property Identifier As String Implements INamedValue.Key
        <CollectionAttribute("MetabolismFlux")> Public Property MetabolismFlux As String()
        <Column("Comments")> Public Property Comment As String

        Public Overrides Function ToString() As String
            Return Identifier
        End Function
    End Class

    Public Class ExpressionKinetics
        Public Property ProteinComplex As String
        Public Property Km As Double
        Public Property pH As Double
        Public Property Temperature As Double
    End Class

    Public Class ProteinAssembly : Implements INamedValue

        <Collection("ProteinComponents", "; ")> Public Property ProteinComponents As String()

        ''' <summary>
        ''' Unique-ID
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property ProteinComplexes As String Implements INamedValue.Key
        Public Property Upper_Bound As Double

        ''' <summary>
        ''' 解离的速率
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Lambda As Double
        Public Property p_Dynamics_K As Double
        Public Property Comments As String

        Public Overrides Function ToString() As String
            Return ProteinComplexes
        End Function
    End Class

    ''' <summary>
    ''' Product of <see cref="TranscriptUnit"></see> transcription event
    ''' </summary>
    ''' <remarks></remarks>
    Public Class Transcript : Implements INamedValue

        ''' <summary>
        ''' <seealso cref="Metabolite.Identifier"></seealso> for itself in the table of <see cref="Metabolite"></see>
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property UniqueId As String
        Public Property Lamda As Double

        Public Property Template As String Implements INamedValue.Key

        ''' <summary>
        ''' <seealso cref="Metabolite.Identifier"></seealso> for its protein product.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Product As String
        Public Property Comments As String

        ' ''' <summary>
        ' ''' 0表示被缺失突变，1表示正常表达状态，大于1表示过量表达
        ' ''' </summary>
        ' ''' <value></value>
        ' ''' <returns></returns>
        ' ''' <remarks></remarks>
        'Public Property CoefficientFactor As Double
        Public Property TranscriptType As Bacterial_GENOME.Transcript.TranscriptTypes

        <CollectionAttribute("Transcript.Compositions", "; ")> Public Property TranscriptCompositionVectors As Integer()
        <CollectionAttribute("Polypeptide.Composition", "; ")> Public Property PolypeptideCompositionVector As Integer()

        Public Overrides Function ToString() As String
            Return UniqueId
        End Function
    End Class

    Public Class Protein : Implements INamedValue
        Public Property Identifier As String Implements INamedValue.Key
        Public Property ECNumber As String
        <Column("Lambda")> Public Property Lambda As Double
        <CollectionAttribute("Polypeptide.Composition", "; ")> Public Property PolypeptideCompositionVector As Integer()

        Public Property ProteinType As GCModeller.Assembly.GCMarkupLanguage.GCML_Documents.XmlElements.Metabolism.Polypeptide.ProteinTypes

        Public Overrides Function ToString() As String
            Return Identifier
        End Function
    End Class

    Public Class MetabolismFlux : Implements INamedValue
        Public Property Identifier As String Implements INamedValue.Key
        Public Property Equation As String
            Get
                Return _Equation
            End Get
            Set(value As String)
                _Equation = value
                Dim Model As DefaultTypes.Equation = EquationBuilder.CreateObject(value)
                _Internal_compilerLeft = (From item In Model.Reactants Select New KeyValuePair(Of Double, String)(item.StoiChiometry, item.ID)).ToArray
                _Internal_compilerRight = (From item In Model.Products Select New KeyValuePair(Of Double, String)(item.StoiChiometry, item.ID)).ToArray
            End Set
        End Property

        Dim _Equation As String

        <Column("p_LOWER_Bound")> Public Property LOWER_Bound As Double
        <Column("p_UPPER_Bound")> Public Property UPPER_Bound As Double

        Public Property CommonName As String

        <Column("p_Dynamics_K_1")> Public Property p_Dynamics_K_1 As Double
        <Column("p_Dynamics_K_2")> Public Property p_Dynamics_K_2 As Double

        ''' <summary>
        ''' 编译器所使用的属性 底物端{计量数，UniqueId}
        ''' </summary>
        ''' <remarks></remarks>
        Protected Friend _Internal_compilerLeft As KeyValuePair(Of Double, String)()
        ''' <summary>
        ''' 产物端
        ''' </summary>
        ''' <remarks></remarks>
        Protected Friend _Internal_compilerRight As KeyValuePair(Of Double, String)()

        Public ReadOnly Property Left As KeyValuePair(Of Double, String)()
            Get
                Return _Internal_compilerLeft
            End Get
        End Property

        Public ReadOnly Property Right As KeyValuePair(Of Double, String)()
            Get
                Return _Internal_compilerRight
            End Get
        End Property

        ''' <summary>
        ''' If the target metabolite is the reactant of this reaction object, then return -1, return 1 for the target 
        ''' metabolite is the product of this reaction event, and return 0 for the metabolite is not exists in this 
        ''' reaction.
        ''' (底物端为-1，产物端为1，不存在为0)
        ''' </summary>
        ''' <param name="UniqueId"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function get_Coefficient(UniqueId As String) As Integer
            Dim LQuery = (From item In _Internal_compilerLeft Where String.Equals(item.Value.Split.First, UniqueId) Select item.Key).ToArray

            If LQuery.IsNullOrEmpty Then
                LQuery = (From item In _Internal_compilerRight Where String.Equals(item.Value.Split.First, UniqueId) Select item.Key).ToArray
                If LQuery.IsNullOrEmpty Then
                    Return 0
                Else
                    Return LQuery.First
                End If
            Else
                Return -LQuery.First
            End If
        End Function

        ''' <summary>
        ''' 催化本反应过程的基因或者调控因子(列)，请注意，由于在前半部分为代谢流对象，故而Key的值不是从零开始的
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <CollectionAttribute("ModifierEnzymes")> Public Property Enzymes As String()
        'Dim _DBLinks As SMRUCC.genomics.Assembly.MetaCyc.Schema.DBLinkManager
        'Public Property DBLinks As String()
        '    Get
        '        If _DBLinks Is Nothing Then
        '            Return New String() {}
        '        Else
        '            Return _DBLinks.DBLinks
        '        End If
        '    End Get
        '    Set(value As String())
        '        _DBLinks = New MetaCyc.Schema.DBLinkManager(value)
        '    End Set
        'End Property

        'Public Function GetDBLinks() As SMRUCC.genomics.Assembly.MetaCyc.Schema.DBLinkManager
        '    Return _DBLinks
        'End Function
        Public Property EnzymeClass As String
        Public Property KEGGReaction As String
        Public Property MetaCycId As String

        Public ReadOnly Property Reversible As Boolean
            Get
                Return InStr(Equation, " <=> ") > 0
            End Get
        End Property

        Friend Sub ReCreateEquation()
            _Equation = EquationBuilder.ToString(Function() _Internal_compilerLeft, Function() _Internal_compilerRight, Reversible).ToUpper
        End Sub

        Public Function get_Metabolites() As String()
            Dim Metabolites As List(Of String) = New List(Of String)

            If Not _Internal_compilerLeft.IsNullOrEmpty AndAlso Not _Internal_compilerRight.IsNullOrEmpty Then
                Call Metabolites.AddRange((From item In _Internal_compilerLeft Select item.Value).ToArray)
                Call Metabolites.AddRange((From item In _Internal_compilerRight Select item.Value).ToArray)
                Return Metabolites.ToArray
            End If

            Dim FluxObject As DataModel.FluxObject =
                EquationBuilder.CreateObject(Of MetaCyc.Schema.Metabolism.Compound, DataModel.FluxObject)(Equation)

            Call Metabolites.AddRange((From item In FluxObject.LeftSides Select item.Identifier).AsList)
            Call Metabolites.AddRange((From item In FluxObject.RightSide Select item.Identifier).AsList)

            Return Metabolites.ToArray
        End Function

        Public Function CreateObject() As DataModel.FluxObject
            Dim FluxObject As DataModel.FluxObject = EquaionModel.CreateObject(Of MetaCyc.Schema.Metabolism.Compound, DataModel.FluxObject)(Equation)

            FluxObject.Identifier = Identifier
            FluxObject.Upper_Bound = UPPER_Bound
            FluxObject.Lower_Bound = LOWER_Bound
            FluxObject.K1 = p_Dynamics_K_1
            FluxObject.K2 = p_Dynamics_K_2

            If Enzymes.IsNullOrEmpty Then
                FluxObject.AssociatedRegulationGenes = New DataModel.AssociatedGene() {}
            Else
                FluxObject.AssociatedRegulationGenes = (From strId As String In Enzymes Select New DataModel.AssociatedGene With {.Identifier = strId}).ToArray
            End If

            Return FluxObject
        End Function

        Public Function CreateGCMLModel() As Metabolism.Reaction
            Dim Model = EquationBuilder.CreateObject(Equation)
            Dim Reactants = (From item As CompoundSpecieReference
                             In Model.Reactants
                             Select New GCMarkupLanguage.GCML_Documents.ComponentModels.CompoundSpeciesReference With {.Identifier = item.ID, .StoiChiometry = item.StoiChiometry}).ToArray
            Dim Products = (From item As CompoundSpecieReference
                            In Model.Products
                            Select New GCMarkupLanguage.GCML_Documents.ComponentModels.CompoundSpeciesReference With {.Identifier = item.ID, .StoiChiometry = item.StoiChiometry}).ToArray

            Return New Metabolism.Reaction With {
                .Identifier = Identifier,
                .UPPER_BOUND = UPPER_Bound,
                .LOWER_BOUND = LOWER_Bound,
                .p_Dynamics_K_1 = p_Dynamics_K_1,
                .p_Dynamics_K_2 = p_Dynamics_K_2,
                .Reversible = Reversible,
                .Enzymes = Enzymes,
                .Reactants = Reactants,
                .Products = Products
            }
        End Function

        Public Function CreateMetaCycReactionSchema() As MetaCyc.Schema.Metabolism.Reaction
            Dim FluxObject As MetaCyc.Schema.Metabolism.Reaction =
                EquationBuilder.CreateObject(Of MetaCyc.Schema.Metabolism.Compound, MetaCyc.Schema.Metabolism.Reaction)(Equation)

            FluxObject.Identifier = Identifier
            'FluxObject.DBLinks = If(DBLinks.IsNullOrEmpty, New String() {}, DBLinks)

            Return FluxObject
        End Function

        Public Shared Function CreateObject(SBML As SBML.Level2.XmlFile, MetabolismEnzymeLink As List(Of Mapping.EnzymeGeneMap), MetaCycReactions As MetaCyc.File.DataFiles.Reactions) As List(Of MetabolismFlux)

            Dim FluxObject As List(Of MetabolismFlux) = (From MetabolismFlux As SBML.Level2.Elements.Reaction
                                                         In SBML.Model.listOfReactions.AsParallel
                                                         Select CreateObject(MetabolismFlux, MetaCycReactions, MetabolismEnzymeLink)).AsList
            Return FluxObject
        End Function

        Private Shared Function CreateObject(MetabolismFlux As Reaction,
                                             MetaCycReactions As MetaCyc.File.DataFiles.Reactions,
                                             MetabolismEnzymeLink As List(Of Mapping.EnzymeGeneMap)) As MetabolismFlux

            Dim Flux = New GCTabular.FileStream.MetabolismFlux
            Flux.Equation = EquationBuilder.ToString(Function() (From item As speciesReference In MetabolismFlux.Reactants Select New KeyValuePair(Of Double, String)(item.stoichiometry, item.species)).ToArray,
            Function() (From item As speciesReference In MetabolismFlux.Products Select New KeyValuePair(Of Double, String)(item.stoichiometry, item.species)).ToArray, MetabolismFlux.reversible).ToUpper
            Flux.LOWER_Bound = MetabolismFlux.LowerBound
            Flux.UPPER_Bound = MetabolismFlux.UpperBound
            Flux.Identifier = MetabolismFlux.id
            Flux.CommonName = MetabolismFlux.name
            Flux.p_Dynamics_K_1 = 1
            Flux.p_Dynamics_K_2 = 1

            Dim MetaCycReaction = MetaCycReactions.Item(MetabolismFlux.id)
            If Not MetaCycReaction Is Nothing Then
                '     Flux.DBLinks = MetaCycReaction.DBLinks
            Else
                '     Flux.DBLinks = New String() {}
            End If

            Dim Enzymes = (From item As Mapping.EnzymeGeneMap
                           In MetabolismEnzymeLink.AsParallel
                           Where String.Equals(Flux.Identifier, item.EnzymeRxn)
                           Select item).ToArray

            If Not Enzymes.IsNullOrEmpty Then
                Dim item = Enzymes.First
                Flux.Enzymes = item.GeneId
                If Not String.IsNullOrEmpty(item.CommonName) Then Flux.CommonName = item.CommonName
            End If

            Return Flux
        End Function

        Public Overrides Function ToString() As String
            Return String.Format("{0}  [ {1} ]", Identifier, Equation)
        End Function
    End Class
End Namespace

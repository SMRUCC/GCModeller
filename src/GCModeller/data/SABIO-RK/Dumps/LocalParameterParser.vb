#Region "Microsoft.VisualBasic::c067336702c70c7d00ebaa327951a8e7, GCModeller\data\SABIO-RK\Dumps\LocalParameterParser.vb"

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

    '   Total Lines: 114
    '    Code Lines: 60
    ' Comment Lines: 42
    '   Blank Lines: 12
    '     File Size: 6.05 KB


    '     Module LocalParameterParser
    ' 
    '         Function: GetEnzymeId, (+2 Overloads) TryParseEnzymeCatalyst, (+2 Overloads) TryParseModifierKinetic
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports SMRUCC.genomics.Assembly.MetaCyc
Imports SMRUCC.genomics.Data.SABIORK.TabularDump

Namespace SBML

    Public Module LocalParameterParser

        ''' <summary>
        ''' Km_[<see cref="SBMLParser.CompoundSpecie.Id"></see>]
        ''' </summary>
        ''' <remarks></remarks>
        Const KM_ID As String = "^Km_(.+?_)?SPC_.+?"

        Public Function TryParseEnzymeCatalyst(DataModel As SabiorkSBML) As EnzymeCatalystKineticLaw()
            Dim LQuery = (From item In DataModel.LocalParameters Where Regex.Match(item.name, KM_ID).Success Select item).ToArray
            Dim Enzyme As SBMLParser.CompoundSpecie

            If LQuery.IsNullOrEmpty Then
                Return New EnzymeCatalystKineticLaw() {}
            Else
                Enzyme = GetEnzymeId(DataModel)
            End If
            If Enzyme Is Nothing Then
                Return New EnzymeCatalystKineticLaw() {}
            End If

            Dim EnzCatalystLQuery = (From KM In LQuery Select TryParseEnzymeCatalyst(KM, Enzyme, DataModel)).ToArray
            Return EnzCatalystLQuery
        End Function

        Private Function TryParseEnzymeCatalyst(Km As [Property], Enzyme As SBMLParser.CompoundSpecie, DataModel As SabiorkSBML) As EnzymeCatalystKineticLaw
            'Dim Kcat = (From item In DataModel.LocalParameters Where String.Equals(item.name, "kcat", StringComparison.OrdinalIgnoreCase) Select item).ToArray
            'Dim KcatValue As Double = If(Kcat.IsNullOrEmpty, 0, Val(Kcat.First.value))
            'Dim SubstrateId As String = Regex.Match(Km.name, "SPC.+").Value
            'Dim CatalystSubstrate = DataModel.CompoundSpecies.GetItem(SubstrateId)
            'Dim KineticLaw As New EnzymeCatalystKineticLaw With {
            '    .Metabolite = CatalystSubstrate.Id,
            '    .Enzyme = Enzyme.Id,
            '    .Km = Val(Km.value),
            '    .Kcat = KcatValue,
            '    .Buffer = DataModel.Buffer,
            '    .KineticRecord = DataModel.kineticLawID,
            '    .PH = DataModel.startValuepH,
            '    .Temperature = DataModel.startValueTemperature,
            '    .Ec = GetIdentifier(DataModel.Identifiers, "ec-code"),
            '    .PubMed = GetIdentifier(DataModel.Identifiers, "pubmed"),
            '    .Uniprot = GetIdentifier(Enzyme.Identifiers, "uniprot"),
            '    .KEGGReactionId = GetIdentifier(DataModel.Identifiers, "kegg.reaction"),
            '    .KEGGCompoundId = GetIdentifier(CatalystSubstrate.Identifiers, "kegg.compound")
            '}
            'Return KineticLaw
        End Function

        Const ENZYME_ID As String = "^ENZ_.+?"

        Private Function GetEnzymeId(DataModel As SabiorkSBML) As SBMLParser.CompoundSpecie
            Dim LQuery = (From item In DataModel.CompoundSpecies Where Regex.Match(item.Id, ENZYME_ID).Success Select item).ToArray
            If LQuery.IsNullOrEmpty Then
                Return Nothing
            Else
                If LQuery.Count = 1 Then
                    Return LQuery.First
                Else
                    LQuery = (From item In LQuery Where String.Equals(item.modifierType, "Modifier-Catalyst") Select item).ToArray
                    If LQuery.IsNullOrEmpty Then
                        Return Nothing
                    Else
                        Return LQuery.First
                    End If
                End If
            End If
        End Function

        Public Function TryParseModifierKinetic(DataModel As SabiorkSBML) As ModifierKinetics()
            Dim Modifiers = (From item In DataModel.CompoundSpecies
                             Where Not String.IsNullOrEmpty(item.modifierType) AndAlso
                                 InStr("Modifier-Inhibitor|Modifier-Activator|Modifier-Cofactor", item.modifierType) > 0
                             Select item).ToArray
            If Modifiers.IsNullOrEmpty Then
                Return New ModifierKinetics() {}
            End If
            Dim Enzyme = GetEnzymeId(DataModel)
            Dim LQuery = (From modifier In Modifiers
                          Let modifierKinetic = TryParseModifierKinetic(modifier, Enzyme, DataModel)
                          Where Not modifierKinetic Is Nothing
                          Select modifierKinetic).ToArray
            Return LQuery
        End Function

        Private Function TryParseModifierKinetic(Modifier As SBMLParser.CompoundSpecie, Enzyme As SBMLParser.CompoundSpecie, DataModel As SabiorkSBML) As ModifierKinetics
            'Dim ModifierType As ModifierKinetics.ModifierTypes = ModifierKinetics.TryGetType(Modifier.modifierType)
            'Dim Id As String = If(ModifierType = ModifierKinetics.ModifierTypes.Inhibitor, "Ki_" & Modifier.Id, "Ka_" & Modifier.Id)
            'Dim Parameters = (From item In DataModel.LocalParameters Where String.Equals(Id, item.name) Select item).ToArray
            'If Parameters.IsNullOrEmpty Then
            '    Return Nothing
            'End If

            'Dim ObjectId As String = If(Enzyme Is Nothing,
            '                            Schema.PropertyAttributes.ToString(DataModel.kineticLawID, New KeyValuePair(Of String, String)() {New KeyValuePair(Of String, String)("TYPE", "REACTION-ACTIVITY")}),
            '                            Schema.PropertyAttributes.ToString(GetIdentifier(Enzyme.Identifiers, "uniprot"), New KeyValuePair(Of String, String)() {New KeyValuePair(Of String, String)("TYPE", "ENZYME-ACTIVITY")}))
            'Dim KineticsData As New ModifierKinetics With {
            '    .K = Val(Parameters.First.value),
            '    .Modifier = Modifier.Id,
            '    .ModifierType = ModifierType,
            '    .ObjectId = ObjectId,
            '    .KineticsRecordId = DataModel.kineticLawID,
            '    .KEGGCompoundId = GetIdentifier(Modifier.Identifiers, "kegg.compound")
            '}
            'Return KineticsData
        End Function
    End Module
End Namespace

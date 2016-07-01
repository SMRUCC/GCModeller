Imports System.Text.RegularExpressions
Imports SMRUCC.genomics.Assembly.MetaCyc
Imports SMRUCC.genomics.DatabaseServices.SabiorkKineticLaws
Imports SMRUCC.genomics.DatabaseServices.SabiorkKineticLaws.TabularDump
Imports Microsoft.VisualBasic.ComponentModel

Namespace SabiorkKineticLaws

    Public Module LocalParameterParser

        ''' <summary>
        ''' Km_[<see cref="SabiorkKineticLaws.SBMLParser.CompoundSpecie.Id"></see>]
        ''' </summary>
        ''' <remarks></remarks>
        Const KM_ID As String = "^Km_(.+?_)?SPC_.+?"

        Public Function TryParseEnzymeCatalyst(DataModel As SABIORK) As EnzymeCatalystKineticLaw()
            Dim LQuery = (From item In DataModel.LocalParameters Where Regex.Match(item.Key, KM_ID).Success Select item).ToArray
            Dim Enzyme As SabiorkKineticLaws.SBMLParser.CompoundSpecie

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

        Private Function TryParseEnzymeCatalyst(Km As TripleKeyValuesPair, Enzyme As SBMLParser.CompoundSpecie, DataModel As SABIORK) As EnzymeCatalystKineticLaw
            Dim Kcat = (From item In DataModel.LocalParameters Where String.Equals(item.Key, "kcat", StringComparison.OrdinalIgnoreCase) Select item).ToArray
            Dim KcatValue As Double = If(Kcat.IsNullOrEmpty, 0, Val(Kcat.First.Value2))
            Dim SubstrateId As String = Regex.Match(Km.Key, "SPC.+").Value
            Dim CatalystSubstrate = DataModel.CompoundSpecies.GetItem(SubstrateId)
            Dim KineticLaw As EnzymeCatalystKineticLaw = New EnzymeCatalystKineticLaw With {
                .Metabolite = CatalystSubstrate.Id,
                .Enzyme = Enzyme.Id,
                .Km = Val(Km.Value2),
                .Kcat = KcatValue,
                .Buffer = DataModel.Buffer,
                .KineticRecord = DataModel.kineticLawID,
                .PH = DataModel.startValuepH,
                .Temperature = DataModel.startValueTemperature,
                .Ec = GetIdentifier(DataModel.Identifiers, "ec-code"),
                .PubMed = GetIdentifier(DataModel.Identifiers, "pubmed"),
                .Uniprot = GetIdentifier(Enzyme.Identifiers, "uniprot"),
                .KEGGReactionId = GetIdentifier(DataModel.Identifiers, "kegg.reaction"),
                .KEGGCompoundId = GetIdentifier(CatalystSubstrate.Identifiers, "kegg.compound")}
            Return KineticLaw
        End Function

        Const ENZYME_ID As String = "^ENZ_.+?"

        Private Function GetEnzymeId(DataModel As SABIORK) As SBMLParser.CompoundSpecie
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

        Public Function TryParseModifierKinetic(DataModel As SABIORK) As ModifierKinetics()
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

        Private Function TryParseModifierKinetic(Modifier As SBMLParser.CompoundSpecie, Enzyme As SBMLParser.CompoundSpecie, DataModel As SABIORK) As ModifierKinetics
            Dim ModifierType As ModifierKinetics.ModifierTypes = ModifierKinetics.TryGetType(Modifier.modifierType)
            Dim Id As String = If(ModifierType = ModifierKinetics.ModifierTypes.Inhibitor, "Ki_" & Modifier.Id, "Ka_" & Modifier.Id)
            Dim Parameters = (From item In DataModel.LocalParameters Where String.Equals(Id, item.Key) Select item).ToArray
            If Parameters.IsNullOrEmpty Then
                Return Nothing
            End If

            Dim ObjectId As String = If(Enzyme Is Nothing,
                                        Schema.PropertyAttributes.ToString(DataModel.kineticLawID, New KeyValuePair(Of String, String)() {New KeyValuePair(Of String, String)("TYPE", "REACTION-ACTIVITY")}),
                                        Schema.PropertyAttributes.ToString(GetIdentifier(Enzyme.Identifiers, "uniprot"), New KeyValuePair(Of String, String)() {New KeyValuePair(Of String, String)("TYPE", "ENZYME-ACTIVITY")}))
            Dim KineticsData As New ModifierKinetics With {
                .K = Val(Parameters.First.Value2),
                .Modifier = Modifier.Id,
                .ModifierType = ModifierType,
                .ObjectId = ObjectId,
                .KineticsRecordId = DataModel.kineticLawID,
                .KEGGCompoundId = GetIdentifier(Modifier.Identifiers, "kegg.compound")
            }
            Return KineticsData
        End Function
    End Module
End Namespace
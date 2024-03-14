﻿
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Model.SBML
Imports SMRUCC.genomics.Model.SBML.Level3
Imports SMRUCC.genomics.Model.SBML.SBGN
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Internal.[Object]
Imports SMRUCC.Rsharp.Runtime.Interop

''' <summary>
''' Systems Biology Markup Language (SBML)
''' 
''' a free and open data format for computational systems biology 
''' that’s used by thousands of people worldwide.
''' </summary>
<Package("SBML")>
Module SBMLTools

    <ExportAPI("read.sbgn")>
    Public Function readSBGN(file As String) As sbgnFile
        Return sbgnFile.ReadXml(file)
    End Function

    ''' <summary>
    ''' Read a sbml model file from a given file path
    ''' </summary>
    ''' <param name="file"></param>
    ''' <returns></returns>
    <ExportAPI("read.sbml")>
    <RApiReturn(GetType(XmlFile(Of Reaction)))>
    Public Function readSBML(file As String) As Object
        Return Level3.LoadSBML(file)
    End Function

    <ExportAPI("extract.pathway_model")>
    Public Function extract_pathwayModel(sbml As Object, Optional env As Environment = Nothing) As Object
        If sbml Is Nothing Then
            Return Nothing
        End If

        If TypeOf sbml Is Level3.XmlFile(Of Level3.Reaction) Then
            Dim xml As Level3.XmlFile(Of Level3.Reaction) = sbml
            Dim model = xml.model

            Return New list With {
                .slots = New Dictionary(Of String, Object) From {
                    {"id", Long.Parse(model.id.Match("\d+"))},
                    {"name", model.name},
                    {"notes", model.notes.GetText}
                }
            }
        Else
            Return Message.InCompatibleType(GetType(Level3.XmlFile(Of Level3.Reaction)), sbml.GetType, env)
        End If
    End Function

    <ExportAPI("extract.compartments")>
    Public Function extract_compartments(sbml As Object,
                                         Optional json As Boolean = False,
                                         Optional env As Environment = Nothing) As Object
        If sbml Is Nothing Then
            Return Nothing
        End If

        If TypeOf sbml Is Level3.XmlFile(Of Level3.Reaction) Then
            Dim xml As Level3.XmlFile(Of Level3.Reaction) = sbml
            Dim list = xml.model.listOfCompartments.SafeQuery.ToArray

            If json Then
                Dim array As New list With {.slots = New Dictionary(Of String, Object)}

                For Each loc As Level3.compartment In list
                    array.slots(loc.id) = New list With {
                        .slots = New Dictionary(Of String, Object) From {
                            {"id", Long.Parse(loc.id.Match("\d+"))},
                            {"name", loc.name},
                            {"is", If(loc.annotation Is Nothing, New String() {}, loc.annotation.GetIdMappings.Distinct.ToArray)},
                            {"type", loc.sboTerm}
                        }
                    }
                Next

                Return array
            Else
                Dim df As New dataframe With {
                    .columns = New Dictionary(Of String, Array),
                    .rownames = list _
                        .Select(Function(c) c.id) _
                        .ToArray
                }

                Call df.add("name", list.Select(Function(c) c.name))
                Call df.add("is", list.Select(Function(c) If(c.annotation Is Nothing, "", c.annotation.GetIdMappings.Distinct.JoinBy("; "))))
                Call df.add("type", list.Select(Function(c) c.sboTerm))

                Return df
            End If
        Else
            Return Message.InCompatibleType(GetType(Level3.XmlFile(Of Level3.Reaction)), sbml.GetType, env)
        End If
    End Function

    <ExportAPI("extract_reactions")>
    Public Function extract_reactions(sbml As Object,
                                      Optional json As Boolean = False,
                                      Optional env As Environment = Nothing) As Object
        If sbml Is Nothing Then
            Return Nothing
        End If

        If TypeOf sbml Is Level3.XmlFile(Of Level3.Reaction) Then
            Dim xml As Level3.XmlFile(Of Level3.Reaction) = sbml
            Dim list = xml.model.listOfReactions.SafeQuery.ToArray

            If json Then
                Dim array As New list With {.slots = New Dictionary(Of String, Object)}

                For Each rxn As Level3.Reaction In list
                    array.slots(rxn.id) = New list With {
                        .slots = New Dictionary(Of String, Object) From {
                            {"id", Long.Parse(rxn.id.Match("\d+"))},
                            {"name", rxn.name},
                            {"is", If(rxn.annotation Is Nothing, New String() {}, rxn.annotation.GetIdMappings.Distinct.ToArray)},
                            {"compartment", rxn.compartment},
                            {"reversible", rxn.reversible},
                            {"fast", rxn.fast},
                            {"reactants", rxn.listOfReactants.ToDictionary(Function(a) a.species, Function(a) a.stoichiometry)},
                            {"products", rxn.listOfProducts.ToDictionary(Function(a) a.species, Function(a) a.stoichiometry)},
                            {"modifiers", rxn.listOfModifiers.Select(Function(a) a.species).ToArray},
                            {"notes", rxn.notes.GetText}
                        }
                    }
                Next

                Return array
            Else
                Dim df As New dataframe With {
                    .columns = New Dictionary(Of String, Array),
                    .rownames = list _
                        .Select(Function(c) c.id) _
                        .ToArray
                }

                Call df.add("name", list.Select(Function(c) c.name))
                Call df.add("is", list.Select(Function(c) If(c.annotation Is Nothing, "", c.annotation.GetIdMappings.Distinct.JoinBy("; "))))
                Call df.add("compartment", list.Select(Function(c) c.compartment))
                Call df.add("reversible", list.Select(Function(c) c.reversible))
                Call df.add("fast", list.Select(Function(c) c.fast))
                Call df.add("reactants", list.Select(Function(c) c.listOfReactants.Select(Function(a) $"{a.stoichiometry} {a.species}").JoinBy("; ")))
                Call df.add("products", list.Select(Function(c) c.listOfProducts.Select(Function(a) $"{a.stoichiometry} {a.species}").JoinBy("; ")))
                Call df.add("modifiers", list.Select(Function(c) c.listOfModifiers.Select(Function(a) a.species).JoinBy("; ")))
                Call df.add("notes", list.Select(Function(c) c.notes.GetText))

                Return df
            End If
        Else
            Return Message.InCompatibleType(GetType(Level3.XmlFile(Of Level3.Reaction)), sbml.GetType, env)
        End If
    End Function

    <ExportAPI("extract_compounds")>
    Public Function extract_compounds(sbml As Object,
                                      Optional json As Boolean = False,
                                      Optional env As Environment = Nothing) As Object
        If sbml Is Nothing Then
            Return Nothing
        End If

        If TypeOf sbml Is Level3.XmlFile(Of Level3.Reaction) Then
            Dim xml As Level3.XmlFile(Of Level3.Reaction) = sbml
            Dim list = xml.model.listOfSpecies.ToArray

            If json Then
                Dim array As New list With {.slots = New Dictionary(Of String, Object)}

                For Each cpd As Level3.species In list
                    array.slots(cpd.id) = New list With {
                        .slots = New Dictionary(Of String, Object) From {
                            {"id", Long.Parse(cpd.id.Match("\d+"))},
                            {"name", cpd.name},
                            {"is", If(cpd.annotation Is Nothing, New String() {}, cpd.annotation.GetIdMappings.Distinct.ToArray)},
                            {"type", cpd.sboTerm},
                            {"components", If(cpd.annotation Is Nothing, New String() {}, cpd.annotation.GetIdComponents.Distinct.ToArray)},
                            {"homolog", If(cpd.annotation Is Nothing, New String() {}, cpd.annotation.GetIdHomolog.Distinct.ToArray)},
                            {"notes", cpd.notes.GetText}
                        }
                    }
                Next

                Return array
            Else
                Dim df As New dataframe With {
                    .columns = New Dictionary(Of String, Array),
                    .rownames = list _
                        .Select(Function(c) c.id) _
                        .ToArray
                }

                Call df.add("name", list.Select(Function(c) c.name))
                Call df.add("is", list.Select(Function(c) If(c.annotation Is Nothing, "", c.annotation.GetIdMappings.Distinct.JoinBy("; "))))
                Call df.add("type", list.Select(Function(c) c.sboTerm))
                Call df.add("components", list.Select(Function(c) If(c.annotation Is Nothing, "", c.annotation.GetIdComponents.Distinct.JoinBy("; "))))
                Call df.add("homolog", list.Select(Function(c) If(c.annotation Is Nothing, "", c.annotation.GetIdHomolog.Distinct.JoinBy("; "))))
                Call df.add("notes", list.Select(Function(c) c.notes.GetText.TrimNewLine))

                Return df
            End If
        Else
            Return Message.InCompatibleType(GetType(Level3.XmlFile(Of Level3.Reaction)), sbml.GetType, env)
        End If
    End Function
End Module

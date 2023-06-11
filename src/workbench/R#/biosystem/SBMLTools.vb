
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Model.SBML
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Internal.[Object]

<Package("SBML")>
Module SBMLTools

    ''' <summary>
    ''' Read a sbml model file from a given file path
    ''' </summary>
    ''' <param name="file"></param>
    ''' <returns></returns>
    <ExportAPI("read.sbml")>
    Public Function readSBML(file As String) As Object
        Return Level3.LoadSBML(file)
    End Function

    <ExportAPI("extract.compartments")>
    Public Function extract_compartments(sbml As Object, Optional env As Environment = Nothing) As Object
        If sbml Is Nothing Then
            Return Nothing
        End If

        If TypeOf sbml Is Level3.XmlFile(Of Level3.Reaction) Then
            Dim xml As Level3.XmlFile(Of Level3.Reaction) = sbml
            Dim list = xml.model.listOfCompartments.SafeQuery.ToArray
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
        Else
            Return Message.InCompatibleType(GetType(Level3.XmlFile(Of Level3.Reaction)), sbml.GetType, env)
        End If
    End Function

    <ExportAPI("extract_reactions")>
    Public Function extract_reactions(sbml As Object, Optional env As Environment = Nothing) As Object
        If sbml Is Nothing Then
            Return Nothing
        End If

        If TypeOf sbml Is Level3.XmlFile(Of Level3.Reaction) Then
            Dim xml As Level3.XmlFile(Of Level3.Reaction) = sbml
            Dim list = xml.model.listOfReactions.SafeQuery.ToArray
            Dim df As New dataframe With {
                .columns = New Dictionary(Of String, Array),
                .rownames = list _
                    .Select(Function(c) c.id) _
                    .ToArray
            }

            Call df.add("name", list.Select(Function(c) c.name))
            Call df.add("is", list.Select(Function(c) If(c.annotation Is Nothing, "", c.annotation.GetIdMappings.Distinct.JoinBy("; "))))
            Call df.add("reversible", list.Select(Function(c) c.reversible))
            Call df.add("fast", list.Select(Function(c) c.fast))
            Call df.add("reactants", list.Select(Function(c) c.listOfReactants.Select(Function(a) $"{a.stoichiometry} {a.species}").JoinBy("; ")))
            Call df.add("products", list.Select(Function(c) c.listOfProducts.Select(Function(a) $"{a.stoichiometry} {a.species}").JoinBy("; ")))
            Call df.add("modifiers", list.Select(Function(c) c.listOfModifiers.Select(Function(a) a.species).JoinBy("; ")))
            Call df.add("notes", list.Select(Function(c) c.notes.GetText))

            Return df
        Else
            Return Message.InCompatibleType(GetType(Level3.XmlFile(Of Level3.Reaction)), sbml.GetType, env)
        End If
    End Function

    <ExportAPI("extract_compounds")>
    Public Function extract_compounds(sbml As Object, Optional env As Environment = Nothing) As Object
        If sbml Is Nothing Then
            Return Nothing
        End If

        If TypeOf sbml Is Level3.XmlFile(Of Level3.Reaction) Then
            Dim xml As Level3.XmlFile(Of Level3.Reaction) = sbml
            Dim list = xml.model.listOfSpecies.ToArray
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
        Else
            Return Message.InCompatibleType(GetType(Level3.XmlFile(Of Level3.Reaction)), sbml.GetType, env)
        End If
    End Function
End Module

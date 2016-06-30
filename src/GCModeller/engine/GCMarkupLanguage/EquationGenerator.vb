Imports System.Text
Imports System.Xml.Serialization
Imports LANS.SystemsBiology.Assembly
Imports Microsoft.VisualBasic

Public Class EquationGenerator

    Public Class ChemicalCompound
        Public Class Element
            <XmlAttribute> Public Property Atom As String
            <XmlAttribute> Public Property Count As Integer
        End Class

        <XmlElement> Public Property Elements As Element()

        Public Function Exists(e As String) As Integer
            Dim LQuery = From ele In Elements Where String.Equals(ele.Atom, e) Select ele.Count '
            Return LQuery.Sum
        End Function

        Public Shared Function TryParse(Chemical As String) As String
            Throw New NotImplementedException
        End Function
    End Class

    Public Class Equation
        <XmlArray> Public Property Reactants As SpeciesReference()
        <XmlArray> Public Property Products As SpeciesReference()

        <XmlAttribute> Public Property Reversible As Boolean

        Public Shared Function GetAtoms(Metabolites As Generic.IEnumerable(Of SpeciesReference)) As KeyValuePair(Of String, Integer)()
            Dim List As List(Of KeyValuePair(Of String, Integer)) = New List(Of KeyValuePair(Of String, Integer))
            Dim Atoms As List(Of String) = New List(Of String)
            For Each m In Metabolites
                For Each e In m.Elements
                    Atoms.Add(e.Atom)
                Next
            Next
            Call Atoms.Distinct()
            For Each Atom In Atoms
                Dim n As Integer
                For Each e In Metabolites
                    Dim LQuery = From ele In e.Elements Where String.Equals(ele.Atom, Atom) Select ele.Count '
                    n += LQuery.Sum
                Next
                Call List.Add(New KeyValuePair(Of String, Integer)(Atom, n))
            Next

            Return List.ToArray
        End Function

        Public Class SpeciesReference : Inherits EquationGenerator.ChemicalCompound
            <XmlAttribute> Public Property Stoichiometry As Integer
            <XmlIgnore> Friend Property Stoichiometry2 As List(Of KeyValuePair(Of Integer, Integer)) = New List(Of KeyValuePair(Of Integer, Integer))
            <XmlAttribute> Public Property UniqueID As String

            Public Overrides Function ToString() As String
                Return UniqueID
            End Function
        End Class

        Private Overloads Function ToString(m As SpeciesReference()) As String
            Dim sBuilder As StringBuilder = New StringBuilder(1024)
            For Each Metabolite In m
                If Metabolite.Stoichiometry = 1 Then
                    sBuilder.Append(Metabolite.UniqueID & " + ")
                Else
                    sBuilder.AppendFormat("{0} {1} + ", Metabolite.Stoichiometry, Metabolite.UniqueID)
                End If
            Next
            Call sBuilder.Remove(sBuilder.Length - 3, 3)

            Return sBuilder.ToString
        End Function

        Public Overrides Function ToString() As String
            Dim sBuilder As StringBuilder = New StringBuilder(1024)
            sBuilder.Append(ToString(Reactants))
            If Reversible Then
                sBuilder.Append(" <--> ")
            Else
                sBuilder.Append(" --> ")
            End If
            sBuilder.Append(ToString(Products))

            Return sBuilder.ToString
        End Function
    End Class

    Public Function Generate(Reaction As MetaCyc.File.DataFiles.Slots.Reaction, Compounds As Generic.IEnumerable(Of MetaCyc.File.DataFiles.Slots.Compound)) As Equation
        Throw New NotImplementedException
    End Function

    Public Shared Function Balance(Equation As Equation) As Equation
        Dim List = EquationGenerator.Equation.GetAtoms(Equation.Reactants)     '先计算出底物端的原子总数
        For Each m In Equation.Products
            For Each Atom In List
                Dim n = m.Exists(Atom.Key)
                If n > 0 Then
                    m.Stoichiometry2.Add(New KeyValuePair(Of Integer, Integer)(n, Atom.Value))
                End If
            Next
        Next



        Return Equation
    End Function
End Class

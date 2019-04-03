Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository
Imports Microsoft.VisualBasic.ComponentModel.TagData
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Model

Public Class MassTable : Implements IRepository(Of String, Factor)

    Dim massTable As New Dictionary(Of String, Factor)

    Public Sub Delete(key As String) Implements IRepositoryWrite(Of String, Factor).Delete
        If massTable.ContainsKey(key) Then
            Call massTable.Remove(key)
        End If
    End Sub

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Sub AddOrUpdate(entity As Factor, key As String) Implements IRepositoryWrite(Of String, Factor).AddOrUpdate
        massTable(key) = entity
    End Sub

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function variables(compounds As IEnumerable(Of String), factor As Double) As IEnumerable(Of Variable)
        Return compounds.Select(Function(cpd) Me.variable(cpd, factor))
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function variables(compounds As IEnumerable(Of FactorString(Of Double))) As IEnumerable(Of Variable)
        Return compounds.Select(Function(cpd) Me.variable(cpd.text, cpd.factor))
    End Function

    Public Iterator Function variables(complex As Protein) As IEnumerable(Of Variable)
        For Each compound In complex.compounds
            Yield Me.variable(compound)
        Next
        For Each peptide In complex.polypeptides
            Yield Me.variable(peptide)
        Next
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function variable(mass As String, Optional coefficient As Double = 1) As Variable
        Return New Variable(massTable(mass), coefficient, False)
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function template(mass As String) As Variable
        Return New Variable(massTable(mass), 1, True)
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function Exists(key As String) As Boolean Implements IRepositoryRead(Of String, Factor).Exists
        Return massTable.ContainsKey(key)
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function GetByKey(key As String) As Factor Implements IRepositoryRead(Of String, Factor).GetByKey
        Return massTable.TryGetValue(key)
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function GetWhere(clause As Func(Of Factor, Boolean)) As IReadOnlyDictionary(Of String, Factor) Implements IRepositoryRead(Of String, Factor).GetWhere
        Return massTable.Values.Where(clause).ToDictionary
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function GetAll() As IReadOnlyDictionary(Of String, Factor) Implements IRepositoryRead(Of String, Factor).GetAll
        Return massTable
    End Function

    Public Function AddNew(entity As Factor) As String Implements IRepositoryWrite(Of String, Factor).AddNew
        massTable(entity.ID) = entity
        Return entity.ID
    End Function
End Class
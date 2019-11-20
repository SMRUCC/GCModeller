#Region "Microsoft.VisualBasic::85d22da18367cb8d53a55a5a4398d965, engine\Dynamics\Engine\MassTable.vb"

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

    ' Class MassTable
    ' 
    '     Function: AddNew, Exists, GetAll, (+2 Overloads) GetByKey, GetEnumerator
    '               GetWhere, IEnumerable_GetEnumerator, template, variable, (+3 Overloads) variables
    ' 
    '     Sub: AddOrUpdate, Delete
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository
Imports Microsoft.VisualBasic.ComponentModel.TagData
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Dynamics.Core
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Model

Namespace Engine

    Public Class MassTable : Implements IRepository(Of String, Factor)
        Implements IEnumerable(Of Factor)

        Dim massTable As New Dictionary(Of String, Factor)

        Public ReadOnly Property GetMassValues As Dictionary(Of String, Double)
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return Me.ToDictionary(Function(m) m.ID, Function(m) m.Value)
            End Get
        End Property

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
        Public Function GetByKey(keys As IEnumerable(Of String)) As Factor()
            Return massTable.Takes(keys)
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

        Public Iterator Function GetEnumerator() As IEnumerator(Of Factor) Implements IEnumerable(Of Factor).GetEnumerator
            For Each mass As Factor In massTable.Values
                Yield mass
            Next
        End Function

        Private Iterator Function IEnumerable_GetEnumerator() As IEnumerator Implements IEnumerable.GetEnumerator
            Yield GetEnumerator()
        End Function
    End Class
End Namespace
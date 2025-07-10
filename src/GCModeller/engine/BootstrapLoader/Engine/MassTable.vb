#Region "Microsoft.VisualBasic::c23a11d1074b4ad93d7efc691256b7a8, engine\BootstrapLoader\Engine\MassTable.vb"

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

'   Total Lines: 304
'    Code Lines: 199 (65.46%)
' Comment Lines: 56 (18.42%)
'    - Xml Docs: 91.07%
' 
'   Blank Lines: 49 (16.12%)
'     File Size: 12.38 KB


'     Class MassTable
' 
'         Properties: compartment_ids, GetMassValues, metabolites, micsRNA, mRNA
'                     polypeptide, rRNA, tRNA
' 
'         Constructor: (+2 Overloads) Sub New
' 
'         Function: addCompartment, (+2 Overloads) AddNew, Exists, ExistsAllCompartment, (+2 Overloads) GetByKey
'                   GetEnumerator, GetRole, GetWhere, IEnumerable_GetEnumerator, template
'                   variable, (+4 Overloads) variables
' 
'         Sub: AddOrUpdate, Delete
'         Class CompartTable
' 
'             Properties: Keys, Values
' 
'             Constructor: (+2 Overloads) Sub New
' 
'             Function: getFactor, variable
' 
'             Sub: delete
' 
' 
' 
' 
' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.ComponentModel.EquaionModel.DefaultTypes
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Dynamics.Core

Namespace Engine

    ''' <summary>
    ''' the cellular mass environment 
    ''' </summary>
    Public Class MassTable : Implements IEnumerable(Of Factor)

        Dim massTable As New CompartTable

        Private Class CompartTable

            ReadOnly compartments As New Dictionary(Of String, Dictionary(Of String, Factor))

            Public ReadOnly Property Values As IEnumerable(Of Factor)
                Get
                    Return compartments.Values.Select(Function(d) d.Values).IteratesALL
                End Get
            End Property

            ''' <summary>
            ''' get a collection of the cellular compartment id
            ''' </summary>
            ''' <returns></returns>
            Public ReadOnly Property Keys As IEnumerable(Of String)
                Get
                    Return compartments.Keys
                End Get
            End Property

            Default Public ReadOnly Property compartment(compart_id As String) As Dictionary(Of String, Factor)
                Get
                    If Not compartments.ContainsKey(compart_id) Then
                        Call compartments.Add(compart_id, New Dictionary(Of String, Factor))
                    End If

                    Return compartments(compart_id)
                End Get
            End Property

            Sub New(cache As Dictionary(Of String, Factor), compart_id As String)
                compartments = New Dictionary(Of String, Dictionary(Of String, Factor)) From {{compart_id, cache}}
            End Sub

            Sub New()
            End Sub

            Public Sub delete(mass_id As String)
                For Each compart As Dictionary(Of String, Factor) In compartments.Values
                    Call compart.Remove(mass_id)
                Next
            End Sub

            Public Function getFactor(compart_id As String, mass_id As String) As Factor
                Dim massTable = Me(compart_id)

                If Not massTable.ContainsKey(mass_id) Then
                    Throw New InvalidDataException($"missing molecule '{mass_id}' factor data inside compartment '{compart_id}'!")
                Else
                    Return massTable(mass_id)
                End If
            End Function

            ''' <summary>
            ''' Create a mass factor link to the current mass environment
            ''' </summary>
            ''' <param name="mass"></param>
            ''' <param name="coefficient"></param>
            ''' <returns></returns>
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Public Function variable(mass As String, compart As String, Optional coefficient As Double = 1) As Variable
                If Not compartments.ContainsKey(compart) Then
                    Throw New InvalidDataException($"missing compartment '{compart}' for molecule: '{mass}'!")
                End If

                Dim massTable = compartments(compart)

                Return New Variable(massTable(mass), coefficient, False)
            End Function

        End Class

        ''' <summary>
        ''' make a snapshot of current mass environment
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property GetMassValues As Dictionary(Of String, Double)
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return Me.ToDictionary(Function(m) m.ID, Function(m) m.Value)
            End Get
        End Property

        Public ReadOnly Property mRNA As Factor()
            Get
                Return GetRole(MassRoles.mRNA).ToArray
            End Get
        End Property

        Public ReadOnly Property tRNA As Factor()
            Get
                Return GetRole(MassRoles.tRNA).ToArray
            End Get
        End Property

        Public ReadOnly Property rRNA As Factor()
            Get
                Return GetRole(MassRoles.rRNA).ToArray
            End Get
        End Property

        Public ReadOnly Property micsRNA As Factor()
            Get
                Return GetRole(MassRoles.RNA).ToArray
            End Get
        End Property

        Public ReadOnly Property metabolites As Factor()
            Get
                Return GetRole(MassRoles.compound).ToArray
            End Get
        End Property

        Public ReadOnly Property polypeptide As Factor()
            Get
                Return GetRole(MassRoles.polypeptide).ToArray
            End Get
        End Property

        Default Public ReadOnly Iterator Property GetVariables(list As IEnumerable(Of CompoundSpecieReference)) As IEnumerable(Of Variable)
            Get
                For Each ref As CompoundSpecieReference In list
                    Yield variable(ref.ID, ref.StoiChiometry)
                Next
            End Get
        End Property

        Public ReadOnly Property compartment_ids As IEnumerable(Of String)
            Get
                Return massTable.Keys
            End Get
        End Property

        Sub New()
        End Sub

        Sub New(cache As Dictionary(Of String, Factor), compart As String)
            massTable = New CompartTable(cache, compart)
        End Sub

        Public Function addCompartment(id As String) As Boolean
            Return massTable(id).IsNullOrEmpty
        End Function

        ''' <summary>
        ''' delete metabolites from all compartment
        ''' </summary>
        ''' <param name="key"></param>
        Public Sub Delete(key As String)
            Call massTable.delete(key)
        End Sub

        ''' <summary>
        ''' filter all molecules with a specific <see cref="MassRoles"/> filter condition.
        ''' </summary>
        ''' <param name="role"></param>
        ''' <returns></returns>
        Public Iterator Function GetRole(role As MassRoles) As IEnumerable(Of Factor)
            For Each mass As Factor In massTable.Values
                If mass.role = role Then
                    Yield mass
                End If
            Next
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Sub AddOrUpdate(entity As Factor, key As String, compart As String)
            Dim massTable = Me.massTable(compart)
            massTable(key) = entity
        End Sub

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function variables(compounds As IEnumerable(Of String), factor As Double, compart_id As String) As IEnumerable(Of Variable)
            Return compounds.Select(Function(cpd) Me.variable(cpd, compart_id, factor))
        End Function

        ''' <summary>
        ''' refresh and make copy of the mass factor data to the simulator core links
        ''' </summary>
        ''' <param name="compounds"></param>
        ''' <param name="factor"></param>
        ''' <param name="compart_id"></param>
        ''' <returns></returns>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function variables(compounds As IEnumerable(Of Variable), factor As Double, compart_id As String) As IEnumerable(Of Variable)
            Return compounds.Select(Function(cpd) Me.variable(cpd.mass.ID, compart_id, factor))
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function variables(compounds As IEnumerable(Of CompoundSpecieReference), factor As Double) As IEnumerable(Of Variable)
            Return compounds.Select(Function(cpd) variable(cpd.ID, cpd.Compartment, cpd.Stoichiometry))
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function variables(compounds As IEnumerable(Of CompoundSpecieReference), templates As Index(Of String)) As IEnumerable(Of Variable)
            Return compounds _
                .Select(Function(cpd)
                            If cpd.ID Like templates Then
                                Return Me.template(cpd.ID, cpd.Compartment)
                            Else
                                Return Me.variable(cpd.ID, cpd.Compartment, cpd.Stoichiometry)
                            End If
                        End Function)
        End Function

        ''' <summary>
        ''' Create a mass factor link to the current mass environment
        ''' </summary>
        ''' <param name="mass"></param>
        ''' <param name="coefficient"></param>
        ''' <returns></returns>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function variable(mass As String, compart As String, Optional coefficient As Double = 1) As Variable
            Return New Variable(massTable.getFactor(compart, mass), coefficient, False)
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function template(mass As String, compart_id As String) As Variable
            Return New Variable(massTable.getFactor(compart_id, mass), 1, True)
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function Exists(mass_id As String, compart_id As String) As Boolean
            Return massTable(compart_id).ContainsKey(mass_id)
        End Function

        Public Function ExistsAllCompartment(mass_id As String) As Boolean
            Return massTable.Keys.All(Function(ref) massTable(ref).ContainsKey(mass_id))
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function GetByKey(mass_id As String, compart_id As String) As Factor
            Return massTable(compart_id).TryGetValue(mass_id)
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="keys">a collection of the molecule mass id</param>
        ''' <param name="compart_id"></param>
        ''' <returns></returns>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function GetByKey(keys As IEnumerable(Of String), compart_id As String) As Factor()
            Return massTable(compart_id).Takes(keys).ToArray
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function GetWhere(clause As Func(Of Factor, Boolean)) As IReadOnlyDictionary(Of String, Factor)
            Return massTable.Values _
                .Where(clause) _
                .ToDictionary
        End Function

        Private Function AddNew(entity As Factor, compart_id As String) As String
            ' 20200313 在这里不可以使用可能产生对象替换的代码调用方式
            ' 否则可能会让之前的反应对象失去正确的对象引用关系
            ' 所以下面的字典索引引用被替换为字典直接添加方法了
            ' massTable(entity.ID) = entity
            massTable(compart_id).Add(entity.ID, entity)
            Return entity.ID
        End Function

        ''' <summary>
        ''' 这个函数会先判断目标对象是否存在，只会添加不存在的对象
        ''' </summary>
        ''' <param name="entity"></param>
        ''' <returns>
        ''' this function returns the entity id back
        ''' </returns>
        Public Function AddNew(entity As String, role As MassRoles) As String
            For Each compart_id As String In massTable.Keys
                If Not massTable(compart_id).ContainsKey(entity) Then
                    Call AddNew(New Factor(entity, role, compart_id), compart_id)
                Else
                    Call $"try to add duplicated {entity}({role}) into mass environment, the {role} entity is already existsed.".Warning
                End If
            Next

            Return entity
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

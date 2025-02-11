#Region "Microsoft.VisualBasic::0db14e5a85fef8a3a746204327f5d7c2, engine\BootstrapLoader\Engine\MassTable.vb"

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

    '   Total Lines: 198
    '    Code Lines: 143 (72.22%)
    ' Comment Lines: 24 (12.12%)
    '    - Xml Docs: 83.33%
    ' 
    '   Blank Lines: 31 (15.66%)
    '     File Size: 8.06 KB


    '     Class MassTable
    ' 
    '         Properties: GetMassValues, metabolites, micsRNA, mRNA, polypeptide
    '                     rRNA, tRNA
    ' 
    '         Constructor: (+2 Overloads) Sub New
    ' 
    '         Function: (+2 Overloads) AddNew, Exists, GetAll, (+2 Overloads) GetByKey, GetEnumerator
    '                   GetRole, GetWhere, IEnumerable_GetEnumerator, template, variable
    '                   (+3 Overloads) variables
    ' 
    '         Sub: AddOrUpdate, Delete
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository
Imports Microsoft.VisualBasic.ComponentModel.TagData
Imports SMRUCC.genomics.ComponentModel.EquaionModel.DefaultTypes
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Dynamics.Core

Namespace Engine

    ''' <summary>
    ''' the cellular mass environment 
    ''' </summary>
    Public Class MassTable : Implements IRepository(Of String, Factor)
        Implements IEnumerable(Of Factor)

        Dim massTable As New Dictionary(Of String, Factor)

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

        Sub New()
        End Sub

        Sub New(cache As Dictionary(Of String, Factor))
            massTable = cache
        End Sub

        Public Sub Delete(key As String) Implements IRepositoryWrite(Of String, Factor).Delete
            If massTable.ContainsKey(key) Then
                Call massTable.Remove(key)
            End If
        End Sub

        Public Iterator Function GetRole(role As MassRoles) As IEnumerable(Of Factor)
            For Each mass As Factor In massTable.Values
                If mass.role = role Then
                    Yield mass
                End If
            Next
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Sub AddOrUpdate(entity As Factor, key As String) Implements IRepositoryWrite(Of String, Factor).AddOrUpdate
            massTable(key) = entity
        End Sub

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function variables(compounds As IEnumerable(Of String), factor As Double) As IEnumerable(Of Variable)
            Return compounds.Select(Function(cpd) Me.variable(cpd, factor))
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function variables(compounds As IEnumerable(Of Variable), factor As Double) As IEnumerable(Of Variable)
            Return compounds.Select(Function(cpd) Me.variable(cpd.mass.ID, factor))
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function variables(compounds As IEnumerable(Of FactorString(Of Double)), templates As Index(Of String)) As IEnumerable(Of Variable)
            Return compounds _
                .Select(Function(cpd)
                            If cpd.result Like templates Then
                                Return Me.template(cpd.result)
                            Else
                                Return Me.variable(cpd.result, cpd.factor)
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
            Return massTable.Takes(keys).ToArray
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function GetWhere(clause As Func(Of Factor, Boolean)) As IReadOnlyDictionary(Of String, Factor) Implements IRepositoryRead(Of String, Factor).GetWhere
            Return massTable.Values _
                .Where(clause) _
                .ToDictionary
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function GetAll() As IReadOnlyDictionary(Of String, Factor) Implements IRepositoryRead(Of String, Factor).GetAll
            Return massTable
        End Function

        Private Function AddNew(entity As Factor) As String Implements IRepositoryWrite(Of String, Factor).AddNew
            ' 20200313 在这里不可以使用可能产生对象替换的代码调用方式
            ' 否则可能会让之前的反应对象失去正确的对象引用关系
            '所以下面的字典索引引用被替换为字典直接添加方法了
            ' massTable(entity.ID) = entity
            massTable.Add(entity.ID, entity)
            Return entity.ID
        End Function

        ''' <summary>
        ''' 这个函数会先判断目标对象是否存在，只会添加不存在的对象
        ''' </summary>
        ''' <param name="entity"></param>
        ''' <returns>
        ''' this function returns the entity id back
        ''' </returns>
        Public Function AddNew(entity As String， role As MassRoles) As String
            If Not massTable.ContainsKey(entity) Then
                Return AddNew(New Factor(entity, role))
            End If

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

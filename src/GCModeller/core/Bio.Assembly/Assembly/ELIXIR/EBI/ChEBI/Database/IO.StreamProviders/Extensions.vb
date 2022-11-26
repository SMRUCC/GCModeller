#Region "Microsoft.VisualBasic::e5382c9703f0c64acba82b311b1f761a, GCModeller\core\Bio.Assembly\Assembly\ELIXIR\EBI\ChEBI\Database\IO.StreamProviders\Extensions.vb"

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

    '   Total Lines: 84
    '    Code Lines: 59
    ' Comment Lines: 15
    '   Blank Lines: 10
    '     File Size: 3.47 KB


    '     Module TsvExtensions
    ' 
    '         Function: CreateProperty, Types
    ' 
    '     Class ChemicalProperty
    ' 
    '         Properties: CHARGE, ChEBI_ID, FORMULA, ID, MASS
    '                     MONOISOTOPIC_MASS
    ' 
    '         Function: ToString
    ' 
    '         Sub: Assign
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.ComponentModel
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository
Imports SMRUCC.genomics.Assembly.ELIXIR.EBI.ChEBI.Database.IO.StreamProviders.Tsv.Tables

Namespace Assembly.ELIXIR.EBI.ChEBI.Database.IO.StreamProviders.Tsv

    ''' <summary>
    ''' Extension for ChEBI tsv tables entity.
    ''' </summary>
    Public Module TsvExtensions

        ''' <summary>
        ''' Listing all types in a chebi entity object from the property <see cref="Tables.Entity.TYPE"/>.
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="source"></param>
        ''' <returns></returns>
        ''' 
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function Types(Of T As Tables.Entity)(source As IEnumerable(Of T)) As String()
            Return source.Select(Function(o) o.TYPE).Distinct.ToArray
        End Function

        ''' <summary>
        ''' Group the chemical data rows into group by compound id and create property value by <see cref="ChemicalData.TYPE"/> value.
        ''' </summary>
        ''' <param name="data"></param>
        ''' <returns></returns>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function CreateProperty(data As IEnumerable(Of ChemicalData)) As ChemicalProperty()
            Return ChemicalData _
                .ChemicalModel(data) _
                .Select(Function(group)
                            Return New ChemicalProperty With {
                                .ChEBI_ID = group.Key,
                                .FORMULA = group.Value.TryGetValue(NameOf(.FORMULA)),
                                .CHARGE = group.Value.TryGetValue(NameOf(.CHARGE)),
                                .MASS = group.Value.TryGetValue(NameOf(.MASS)),
                                .MONOISOTOPIC_MASS = group.Value.TryGetValue("MONOISOTOPIC MASS")
                            }
                        End Function) _
                .ToArray
        End Function
    End Module

    Public Class ChemicalProperty : Implements INamedValue, IAddressOf

        Public Property ChEBI_ID As String Implements IKeyedEntity(Of String).Key

        Private Property ID As Integer Implements IAddress(Of Integer).Address
            Get
                Return Val(ChEBI_ID)
            End Get
            Set(value As Integer)
                ChEBI_ID = value
            End Set
        End Property

#Region "Property Group"
        Public Property FORMULA As ChemicalData()
        Public Property MASS As ChemicalData()
        Public Property CHARGE As ChemicalData()
        <Description("MONOISOTOPIC MASS")>
        Public Property MONOISOTOPIC_MASS As ChemicalData()
#End Region

        Public Overrides Function ToString() As String
            If FORMULA.IsNullOrEmpty Then
                Return ChEBI_ID
            Else
                Return $"{ChEBI_ID} ({FORMULA.Select(Function(f) f.CHEMICAL_DATA).Distinct.JoinBy(" / ")})"
            End If
        End Function

        Public Sub Assign(address As Integer) Implements IAddress(Of Integer).Assign
            Throw New NotImplementedException()
        End Sub
    End Class
End Namespace

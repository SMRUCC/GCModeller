#Region "Microsoft.VisualBasic::2c8f0dcb73d97a9bc63825ab1bbd60e6, GCModeller\core\Bio.Assembly\ComponentModel\Equations\DefaultTypes.vb"

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

    '   Total Lines: 167
    '    Code Lines: 127
    ' Comment Lines: 13
    '   Blank Lines: 27
    '     File Size: 6.67 KB


    '     Class CompoundSpecieReference
    ' 
    '         Properties: Compartment, ID, StoiChiometry
    ' 
    '         Constructor: (+2 Overloads) Sub New
    '         Function: AsFactor, Equals, ToString
    ' 
    '     Class Equation
    ' 
    '         Constructor: (+3 Overloads) Sub New
    ' 
    '         Function: __equals, GetBuffer, GetCoEfficient, ParseBuffer, ReadMetabolite
    '                   TryParse
    ' 
    '         Sub: SaveMetabolite
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Runtime.CompilerServices
Imports System.Text
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.TagData
Imports Microsoft.VisualBasic.Linq

Namespace ComponentModel.EquaionModel.DefaultTypes

    Public Class CompoundSpecieReference : Implements ICompoundSpecies

        ''' <summary>
        ''' 化学计量数
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property StoiChiometry As Double Implements ICompoundSpecies.StoiChiometry
        <XmlText> Public Property ID As String Implements ICompoundSpecies.Key
        <XmlAttribute> Public Property Compartment As String

        Sub New()
        End Sub

        Sub New(ref As ICompoundSpecies)
            StoiChiometry = ref.StoiChiometry
            ID = ref.Key
        End Sub

        Public Overloads Function Equals(b As ICompoundSpecies, strict As Boolean) As Boolean
            Return Equivalence.Equals(Me, b, strict)
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function AsFactor() As FactorString(Of Double)
            Return New FactorString(Of Double) With {
                .factor = StoiChiometry,
                .text = ID
            }
        End Function

        Public Overrides Function ToString() As String
            If StoiChiometry > 1 Then
                Return String.Format("{0} {1}", StoiChiometry, ID)
            Else
                Return ID
            End If
        End Function
    End Class

    ''' <summary>
    ''' 默认类型的反应表达式的数据结构，可以使用<see cref="EquationBuilder.CreateObject(String)"/>来进行构建
    ''' </summary>
    ''' <remarks></remarks>
    Public Class Equation : Inherits Equation(Of CompoundSpecieReference)
        Implements IEquation(Of CompoundSpecieReference)

        Sub New()
        End Sub

        Sub New(left As IEnumerable(Of ICompoundSpecies), right As IEnumerable(Of ICompoundSpecies), canReverse As Boolean)
            Reactants = left.Select(Function(x) New CompoundSpecieReference(x)).ToArray
            Products = right.Select(Function(x) New CompoundSpecieReference(x)).ToArray
            reversible = canReverse
        End Sub

        Sub New(left As IEnumerable(Of ICompoundSpecies), right As IEnumerable(Of ICompoundSpecies),
                idMaps As Dictionary(Of String, String),
                canReverse As Boolean)

            Reactants = left _
                .Select(Function(x)
                            Return New CompoundSpecieReference With {
                                .ID = idMaps(x.Key),
                                .StoiChiometry = x.StoiChiometry
                            }
                        End Function) _
                .ToArray
            Products = right _
                .Select(Function(x)
                            Return New CompoundSpecieReference With {
                                .ID = idMaps(x.Key),
                                .StoiChiometry = x.StoiChiometry
                            }
                        End Function) _
                .ToArray
            reversible = canReverse
        End Sub

        Public Overloads Function GetCoEfficient(id As String, Optional directional As Boolean = False) As Double
            Dim factor As Double = MyBase.GetCoEfficient(id)

            If reversible AndAlso directional Then
                Return factor / 2
            Else
                Return factor
            End If
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Function TryParse(expression As String) As Equation
            Return EquationBuilder.CreateObject(expression)
        End Function

        Protected Overrides Function __equals(a As CompoundSpecieReference, b As CompoundSpecieReference, strict As Boolean) As Object
            Return a.Equals(b, strict)
        End Function

        ''' <summary>
        ''' save the given equation model as byte stream
        ''' </summary>
        ''' <param name="eq"></param>
        ''' <returns></returns>
        Public Shared Function GetBuffer(eq As Equation) As Byte()
            Using ms As New MemoryStream, file As New BinaryWriter(ms)
                Call file.Write(Encoding.ASCII.GetBytes(eq.Id))
                Call file.Write(CByte(0))
                Call file.Write(CByte(If(eq.reversible, 1, 0)))
                Call SaveMetabolite(file, eq.Reactants)
                Call SaveMetabolite(file, eq.Products)
                Call file.Flush()

                Return ms.ToArray
            End Using
        End Function

        Private Shared Sub SaveMetabolite(ms As BinaryWriter, list As CompoundSpecieReference())
            Call ms.Write(list.Length)

            For Each factor As CompoundSpecieReference In list
                Call ms.Write(Encoding.ASCII.GetBytes(factor.ID))
                Call ms.Write(CByte(0))
                Call ms.Write(Encoding.ASCII.GetBytes(If(factor.Compartment, "")))
                Call ms.Write(CByte(0))
                Call ms.Write(factor.StoiChiometry)
                Call ms.Write(CByte(0))
            Next
        End Sub

        Private Shared Iterator Function ReadMetabolite(file As BinaryReader) As IEnumerable(Of CompoundSpecieReference)
            Dim size As Integer = file.ReadInt32

            For i As Integer = 0 To size - 1
                Dim id As String = file.ReadStringZero(Encoding.ASCII)
                Dim compartment As String = file.ReadStringZero(Encoding.ASCII)
                Dim factor As Double = file.ReadDouble

                Call file.ReadByte()

                Yield New CompoundSpecieReference With {
                    .Compartment = compartment,
                    .ID = id,
                    .StoiChiometry = factor
                }
            Next
        End Function

        Public Shared Function ParseBuffer(buffer As Stream) As Equation
            Using file As New BinaryReader(buffer)
                Dim id As String = file.ReadStringZero(Encoding.ASCII)
                Dim is_reverse As Boolean = file.ReadByte > 0
                Dim left As CompoundSpecieReference() = ReadMetabolite(file).ToArray
                Dim right As CompoundSpecieReference() = ReadMetabolite(file).ToArray

                Return New Equation(left, right, is_reverse) With {.Id = id}
            End Using
        End Function
    End Class
End Namespace

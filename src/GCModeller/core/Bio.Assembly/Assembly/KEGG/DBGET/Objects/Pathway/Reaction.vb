#Region "Microsoft.VisualBasic::7967d3d25141d1a36315c2d1135927dc, ..\GCModeller\core\Bio.Assembly\Assembly\KEGG\DBGET\Objects\Pathway\Reaction.vb"

' Author:
' 
'       asuka (amethyst.asuka@gcmodeller.org)
'       xieguigang (xie.guigang@live.com)
'       xie (genetics@smrucc.org)
' 
' Copyright (c) 2016 GPL3 Licensed
' 
' 
' GNU GENERAL PUBLIC LICENSE (GPL3)
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

#End Region

Imports System.Text.RegularExpressions
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.ComponentModel.EquaionModel

Namespace Assembly.KEGG.DBGET.bGetObject

    ''' <summary>
    ''' KEGG reaction annotation data.
    ''' </summary>
    ''' <remarks></remarks>
    <XmlRoot("bGetObject.Reaction", [Namespace]:="http://GCModeller.org/core/assembly/KEGG/dbget/reaction?rn:r_ID")>
    Public Class Reaction : Implements INamedValue

        <XmlAttribute>
        Public Property Entry As String Implements INamedValue.Key
        Public Property CommonNames As String()
        Public Property Definition As String
        Public Property Equation As String

        ''' <summary>
        ''' 标号： <see cref="Expasy.Database.Enzyme.Identification"></see>
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property ECNum As String()
        Public Property Comments As String
        Public Property Pathway As KeyValuePair()
        Public Property [Module] As KeyValuePair()
        Public Property Orthology As TripleKeyValuesPair()

        ''' <summary>
        ''' The reaction class
        ''' </summary>
        ''' <returns></returns>
        Public Property [Class] As KeyValuePair()

        Public ReadOnly Property ReactionModel As DefaultTypes.Equation
            Get
                Try
                    Return EquationBuilder.CreateObject(Me.Equation)
                Catch ex As Exception
                    ex = New Exception(Me.GetJson, ex)
                    Throw ex
                End Try
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return String.Format("[{0}] {1}:  {2}", ECNum, Entry, Definition)
        End Function

        Public ReadOnly Property Reversible As Boolean
            Get
                Return InStr(Equation, " <=> ") > 0
            End Get
        End Property

        ''' <summary>
        ''' 得到本反应过程对象中的所有的代谢底物的KEGG编号，以便于查询和下载
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetSubstrateCompounds() As String()
            Dim FluxModel = EquationBuilder.CreateObject(Of
                DefaultTypes.CompoundSpecieReference,
                DefaultTypes.Equation)(Regex.Replace(Equation, "(\s*\(.+?\))|(n )", ""))
            Dim Compounds$() = LinqAPI.Exec(Of String) <=
 _
                From csr As DefaultTypes.CompoundSpecieReference
                In {
                    FluxModel.Reactants,
                    FluxModel.Products
                }.IteratesALL
                Select csr.ID
                Distinct

            Return Compounds
        End Function

        Public Function IsConnectWith([next] As Reaction) As Boolean
            Dim a = GetSubstrateCompounds(),
                b = [next].GetSubstrateCompounds

            For Each s In a
                If Array.IndexOf(b, s) > -1 Then
                    Return True
                End If
            Next

            Return False
        End Function
    End Class
End Namespace

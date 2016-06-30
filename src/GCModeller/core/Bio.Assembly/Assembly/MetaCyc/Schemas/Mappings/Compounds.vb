#Region "Microsoft.VisualBasic::b67a62e705a2d26eefa8150164e8c47f, ..\Bio.Assembly\Assembly\MetaCyc\Schemas\Mappings\Compounds.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
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
Imports System.Text
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports SMRUCC.genomics.Assembly.MetaCyc.File.FileSystem
Imports SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles

Namespace Assembly.MetaCyc.Schema

    Public Interface ICompoundObject : Inherits sIdEnumerable
        Property CommonNames As String()
        Property PUBCHEM As String
        Property CHEBI As String()
        Property locusId As String
    End Interface

    ''' <summary>
    ''' Regprecise Effector与MetaCyc Compounds Mapping
    ''' </summary>
    ''' <remarks></remarks>
    Public Class EffectorMap : Implements sIdEnumerable, ICompoundObject

        Public Property Effector As String Implements sIdEnumerable.Identifier
        ''' <summary>
        ''' <see cref="ICompoundObject.CommonNames"></see>
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property EffectorAlias As String() Implements ICompoundObject.CommonNames
        Public Property MetaCycId As String
        Public Property CommonName As String
        Public Property Synonym As String

        Sub New()
        End Sub

        Public Overrides Function ToString() As String
            Return String.Format("{0} --> {1}", Effector, MetaCycId)
        End Function

        Public Property CHEBI As String() Implements ICompoundObject.CHEBI
        Public Property KEGGCompound As String Implements ICompoundObject.locusId
        Public Property PUBCHEM As String Implements ICompoundObject.PUBCHEM

        Public Shared Function GenerateMap(CompoundSpecies As ICompoundObject()) As EffectorMap()
            Dim Effectors = (From cps As ICompoundObject In CompoundSpecies Select cps).ToArray
            Return Effectors
        End Function

        Private Shared Function __newMap(cps As ICompoundObject) As EffectorMap
            Return New EffectorMap With {
                .Effector = cps.Identifier,
                .EffectorAlias = cps.CommonNames,
                ._CHEBI = cps.CHEBI,
                .KEGGCompound = cps.Identifier,
                ._PUBCHEM = cps.PUBCHEM
            }
        End Function
    End Class
End Namespace

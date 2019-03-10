#Region "Microsoft.VisualBasic::f3530db05837e8c743f5cc1875c8b4c3, Bio.Assembly\Assembly\MetaCyc\Schemas\Mappings\Compounds.vb"

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

    '     Class EffectorMap
    ' 
    '         Properties: CHEBI, CommonName, Effector, EffectorAlias, KEGGCompound
    '                     MetaCycId, PUBCHEM, Synonym
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: __newMap, GenerateMap, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text.RegularExpressions
Imports System.Text
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports SMRUCC.genomics.Assembly.MetaCyc.File.FileSystem
Imports SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles
Imports SMRUCC.genomics.ComponentModel.EquaionModel

Namespace Assembly.MetaCyc.Schema

    ''' <summary>
    ''' Regprecise Effector与MetaCyc Compounds Mapping
    ''' </summary>
    ''' <remarks></remarks>
    Public Class EffectorMap : Implements INamedValue, ICompoundObject

        Public Property Effector As String Implements INamedValue.Key
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
        Public Property KEGGCompound As String Implements ICompoundObject.KEGG_cpd
        Public Property PUBCHEM As String Implements ICompoundObject.PUBCHEM

        Public Shared Function GenerateMap(CompoundSpecies As ICompoundObject()) As EffectorMap()
            Dim Effectors = (From cps As ICompoundObject In CompoundSpecies Select cps).ToArray
            Return Effectors
        End Function

        Private Shared Function __newMap(cps As ICompoundObject) As EffectorMap
            Return New EffectorMap With {
                .Effector = cps.Key,
                .EffectorAlias = cps.CommonNames,
                ._CHEBI = cps.CHEBI,
                .KEGGCompound = cps.Key,
                ._PUBCHEM = cps.PUBCHEM
            }
        End Function
    End Class
End Namespace

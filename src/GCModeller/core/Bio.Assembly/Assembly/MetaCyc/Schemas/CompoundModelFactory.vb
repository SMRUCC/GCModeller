#Region "Microsoft.VisualBasic::97ad620a7a5a1de80a8f4cc11d2bd371, Bio.Assembly\Assembly\MetaCyc\Schemas\CompoundModelFactory.vb"

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

    '     Module CompoundModelFactory
    ' 
    ' 
    '         Class GeneralCompoundModel
    ' 
    '             Properties: CHEBI, CommonNames, ID, KEGGCompound, PUBCHEM
    ' 
    '             Function: GenerateModels
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace Assembly.MetaCyc.Schema

    Public Module CompoundModelFactory

        Public Class GeneralCompoundModel

            Public Property ID As String
            Public Property CommonNames As String()

            Public Property CHEBI As String()
            Public Property KEGGCompound As String
            Public Property PUBCHEM As String

            Public Function GenerateModels(data As IEnumerable(Of MetaCyc.File.DataFiles.Slots.Compound)) As GeneralCompoundModel()
                Dim LQuery = (From item In data
                              Select New GeneralCompoundModel With {
                                  .ID = item.Identifier,
                                  ._PUBCHEM = item.PUBCHEM,
                                  ._CHEBI = item.CHEBI,
                                  .CommonNames = item.Names,
                                  .KEGGCompound = item.KEGGCompound}).ToArray
                Return LQuery
            End Function
        End Class


    End Module
End Namespace

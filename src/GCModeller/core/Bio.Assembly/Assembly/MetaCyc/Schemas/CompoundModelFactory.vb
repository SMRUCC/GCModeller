#Region "Microsoft.VisualBasic::863307bcea6aa4fb9c9b3b5c167d7a53, ..\GCModeller\core\Bio.Assembly\Assembly\MetaCyc\Schemas\CompoundModelFactory.vb"

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

Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports SMRUCC.genomics.Assembly.KEGG.DBGET
Imports SMRUCC.genomics.ComponentModel.EquaionModel

Namespace Assembly.MetaCyc.Schema

    Public Module CompoundModelFactory

        Public Class GeneralCompoundModel : Implements ICompoundObject

            Public Property ID As String Implements INamedValue.Key
            Public Property CommonNames As String() Implements ICompoundObject.CommonNames

            Public Property CHEBI As String() Implements ICompoundObject.CHEBI
            Public Property KEGGCompound As String Implements ICompoundObject.KEGG_cpd
            Public Property PUBCHEM As String Implements ICompoundObject.PUBCHEM

            Public Function GenerateModels(data As IEnumerable(Of bGetObject.Compound)) As GeneralCompoundModel()
                Dim LQuery = (From item In data
                              Select New GeneralCompoundModel With {
                                  .ID = item.Entry,
                                  ._CHEBI = item.CHEBI,
                                  .CommonNames = item.CommonNames,
                                  .KEGGCompound = item.Entry,
                                  ._PUBCHEM = item.PUBCHEM}).ToArray
                Return LQuery
            End Function

            Public Function GenerateModels(data As Generic.IEnumerable(Of MetaCyc.File.DataFiles.Slots.Compound)) As GeneralCompoundModel()
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

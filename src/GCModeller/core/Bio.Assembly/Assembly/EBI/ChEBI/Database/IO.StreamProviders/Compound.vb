#Region "Microsoft.VisualBasic::e53e9e4e14788f11d0d8a947d51627d2, core\Bio.Assembly\Assembly\EBI\ChEBI\Database\IO.StreamProviders\Compound.vb"

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

    '     Class Compound
    ' 
    '         Properties: CHEBI_ACCESSION, CREATED_BY, DEFINITION, ID, MODIFIED_ON
    '                     NAME, PARENT_ID, SOURCE, STAR, STATUS
    ' 
    '         Function: Load, LoadTable, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Serialization.JSON
Imports Microsoft.VisualBasic.Text

Namespace Assembly.EBI.ChEBI.Database.IO.StreamProviders.Tsv

    Public Class Compound : Implements INamedValue

        Public Property ID As String
        Public Property STATUS As String
        ''' <summary>
        ''' ``CHEBI:\d+``
        ''' </summary>
        ''' <returns></returns>
        Public Property CHEBI_ACCESSION As String Implements INamedValue.Key
        Public Property SOURCE As String
        Public Property PARENT_ID As String
        Public Property NAME As String
        Public Property DEFINITION As String
        Public Property MODIFIED_ON As String
        Public Property CREATED_BY As String
        Public Property STAR As String

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function

        Public Shared Function LoadTable(path$) As Dictionary(Of String, Compound)
            Dim data As Compound() = Load(path)
            Dim nameds As NamedValue(Of Compound)() = data _
                .Select(Function(c) New NamedValue(Of Compound) With {
                    .Name = c.CHEBI_ACCESSION.GetTagValue(":", trim:=True).Value,
                    .Value = c
                }).ToArray
            Return nameds.ToDictionary(Function(k) k.Name, Function(v) v.Value)
        End Function

        Public Shared Function Load(path$) As Compound()
            Dim index As Index(Of String) = path.TsvHeaders
            Dim out As New List(Of Compound)

            For Each line As String In path.IterateAllLines.Skip(1)
                Dim t$() = line.Split(ASCII.TAB)
                out += New Compound With {
                    .CHEBI_ACCESSION = t(index(NameOf(.CHEBI_ACCESSION))),
                    .CREATED_BY = t(index(NameOf(.CREATED_BY))),
                    .DEFINITION = t(index(NameOf(.DEFINITION))),
                    .ID = t(index(NameOf(.ID))),
                    .MODIFIED_ON = t(index(NameOf(.MODIFIED_ON))),
                    .NAME = t(index(NameOf(.NAME))),
                    .PARENT_ID = t(index(NameOf(.PARENT_ID))),
                    .SOURCE = t(index(NameOf(.SOURCE))),
                    .STAR = t(index(NameOf(.STAR))),
                    .STATUS = t(index(NameOf(.STATUS)))
                }
            Next

            Return out
        End Function
    End Class
End Namespace

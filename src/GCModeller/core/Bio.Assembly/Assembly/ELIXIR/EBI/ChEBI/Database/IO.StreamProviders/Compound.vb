#Region "Microsoft.VisualBasic::11d076386e900dff60c67add9473bf31, GCModeller\core\Bio.Assembly\Assembly\ELIXIR\EBI\ChEBI\Database\IO.StreamProviders\Compound.vb"

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

    '   Total Lines: 68
    '    Code Lines: 55
    ' Comment Lines: 4
    '   Blank Lines: 9
    '     File Size: 2.89 KB


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
Imports Microsoft.VisualBasic.Serialization.JSON
Imports Microsoft.VisualBasic.Text

Namespace Assembly.ELIXIR.EBI.ChEBI.Database.IO.StreamProviders.Tsv

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
                .Select(Function(c)
                            Return New NamedValue(Of Compound) With {
                                .Name = c.CHEBI_ACCESSION.GetTagValue(":", trim:=True).Value,
                                .Value = c
                            }
                        End Function) _
                .ToArray

            Return nameds.ToDictionary(Function(k) k.Name, Function(v) v.Value)
        End Function

        Public Shared Iterator Function Load(path$) As IEnumerable(Of Compound)
            Dim index As Index(Of String) = path.TsvHeaders
            Dim compound As Compound
            Dim tParts$()

            For Each line As String In path.IterateAllLines.Skip(1)
                tParts = line.Split(ASCII.TAB)
                compound = New Compound With {
                    .CHEBI_ACCESSION = tParts(index(NameOf(.CHEBI_ACCESSION))),
                    .CREATED_BY = tParts(index(NameOf(.CREATED_BY))),
                    .DEFINITION = tParts(index(NameOf(.DEFINITION))),
                    .ID = tParts(index(NameOf(.ID))),
                    .MODIFIED_ON = tParts(index(NameOf(.MODIFIED_ON))),
                    .NAME = tParts(index(NameOf(.NAME))),
                    .PARENT_ID = tParts(index(NameOf(.PARENT_ID))),
                    .SOURCE = tParts(index(NameOf(.SOURCE))),
                    .STAR = tParts(index(NameOf(.STAR))),
                    .STATUS = tParts(index(NameOf(.STATUS)))
                }

                Yield compound
            Next
        End Function
    End Class
End Namespace

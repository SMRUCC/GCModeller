#Region "Microsoft.VisualBasic::9e0c2de86d896866c0afae6b89102558, core\Bio.Assembly\Assembly\KEGG\DBGET\BriteHEntry\BriteHText\htextJSON.vb"

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

    '     Class htextJSON
    ' 
    '         Properties: children, name
    ' 
    '         Function: DeflateTerms, parseJSON, ToString
    ' 
    '     Class htextJSONnode
    ' 
    '         Properties: children, commonName, entryId, name
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Serialization.JSON

Namespace Assembly.KEGG.DBGET.BriteHEntry

    Public Class htextJSON

        Public Property name As String
        Public Property children As htextJSONnode()

        Public Overrides Function ToString() As String
            Return name
        End Function

        Public Shared Function parseJSON(json As String) As htextJSON
            Return json.SolveStream.LoadJSON(Of htextJSON)
        End Function

        Public Iterator Function DeflateTerms() As IEnumerable(Of BriteTerm)

        End Function

    End Class

    Public Class htextJSONnode

        Public Property name As String
        Public Property children As htextJSONnode()

        Public ReadOnly Property entryId As String
            Get
                If children.IsNullOrEmpty Then
                    Return name.Split.First
                Else
                    Return Nothing
                End If
            End Get
        End Property

        Public ReadOnly Property commonName As String
            Get
                If children.IsNullOrEmpty Then
                    Return name.GetTagValue(" ", trim:=True).Value
                Else
                    Return name
                End If
            End Get
        End Property

    End Class
End Namespace

#Region "Microsoft.VisualBasic::b966b1836f041a0383cb1523d0e10b92, analysis\ProteinTools\Interactions.BioGRID\ID.vb"

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

    ' Class ID
    ' 
    '     Properties: sId, StringTypes, Type
    ' 
    '     Constructor: (+3 Overloads) Sub New
    '     Enum Types
    ' 
    '         BioGrid, NCBI
    ' 
    ' 
    ' 
    '  
    ' 
    '     Function: FieldParser, ToString
    ' 
    ' /********************************************************************************/

#End Region

Imports System.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Linq

''' <summary>
''' <see cref="ID.Type"/>:<see cref="ID.sId"/>
''' </summary>
Public Class ID

    Public Const Type_NCBI_Locus As String = "entrez gene/locuslink"
    Public Const Type_biogridID As String = "biogrid"

    Public Property Type As Types
    Public Property sId As String

    Sub New()
    End Sub

    Sub New(type As String, id As String)
        Me.Type = StringTypes.TryGetValue(type, Types.Unknown)
        Me.sId = id
    End Sub

    ''' <summary>
    ''' <see cref="ID.Type"/>:<see cref="ID.sId"/>
    ''' </summary>
    ''' <param name="raw"></param>
    Sub New(raw As String)
        Dim value As NamedValue(Of String) = raw.GetTagValue(":")

        Me.sId = value.Value
        Me.Type = StringTypes.TryGetValue(value.Name, Types.Unknown)
    End Sub

    Public Shared ReadOnly Property StringTypes As Dictionary(Of String, ID.Types) =
        Enums(Of Types)().ToDictionary(Function(x) x.Description)

    ''' <summary>
    ''' 编号的数据库来源
    ''' </summary>
    Public Enum Types
        Unknown = -1
        ''' <summary>
        ''' <see cref="ID.Type_NCBI_Locus"/>
        ''' </summary>
        <Description(ID.Type_NCBI_Locus)>
        NCBI
        ''' <summary>
        ''' <see cref="ID.Type_biogridID"/>
        ''' </summary>
        <Description(ID.Type_biogridID)>
        BioGrid
    End Enum

    Public Overrides Function ToString() As String
        Return $"{Type.Description}:{sId}"
    End Function

    Public Shared Function FieldParser(raw As String) As ID()
        Dim tokens As String() = raw.Split("|"c)
        Return tokens.Select(Function(s) New ID(s)).ToArray
    End Function
End Class

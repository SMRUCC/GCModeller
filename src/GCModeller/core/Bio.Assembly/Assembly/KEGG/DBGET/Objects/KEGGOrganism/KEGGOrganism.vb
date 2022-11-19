#Region "Microsoft.VisualBasic::0086d55b16a76d6e4c1aa3cfd5658c6c, GCModeller\core\Bio.Assembly\Assembly\KEGG\DBGET\Objects\KEGGOrganism\KEGGOrganism.vb"

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

    '   Total Lines: 62
    '    Code Lines: 45
    ' Comment Lines: 8
    '   Blank Lines: 9
    '     File Size: 1.98 KB


    '     Class KEGGOrganism
    ' 
    '         Properties: Eukaryotes, Prokaryote
    ' 
    '         Function: ToArray
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.Collection

Namespace Assembly.KEGG.DBGET.bGetObject.Organism

    ''' <summary>
    ''' KEGG Organisms: Complete Genomes (http://www.genome.jp/kegg/catalog/org_list.html)
    ''' </summary>
    ''' <remarks></remarks>
    Public Class KEGGOrganism

        ''' <summary>
        ''' returns all of the organism data in an array
        ''' </summary>
        ''' <returns></returns>
        Public Function ToArray() As Organism()
            Return __eukaryotes.GetValueList + __prokaryote.Values
        End Function

        Public Property Eukaryotes As Organism()
            Get
                Return __eukaryotes.Values.ToArray
            End Get
            Set(value As Organism())
                If value.IsNullOrEmpty Then
                    __eukaryotes = New Dictionary(Of Organism)
                Else
                    __eukaryotes = value.ToDictionary
                End If
            End Set
        End Property

        Public Property Prokaryote As Prokaryote()
            Get
                Return __prokaryote.Values.ToArray
            End Get
            Set(value As Prokaryote())
                If value.IsNullOrEmpty Then
                    __prokaryote = New Dictionary(Of Prokaryote)
                Else
                    __prokaryote = value.ToDictionary
                End If
            End Set
        End Property

        Dim __eukaryotes As Dictionary(Of Organism)
        Dim __prokaryote As Dictionary(Of Prokaryote)

        Default Public ReadOnly Property GetOrganismData(spCode As String) As Organism
            Get
                If __eukaryotes.ContainsKey(spCode) Then
                    Return __eukaryotes(spCode)
                End If

                If __prokaryote.ContainsKey(spCode) Then
                    Return __prokaryote(spCode)
                End If

                Return Nothing
            End Get
        End Property
    End Class
End Namespace

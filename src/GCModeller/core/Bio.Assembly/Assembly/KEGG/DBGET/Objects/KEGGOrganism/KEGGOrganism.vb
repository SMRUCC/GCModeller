Imports System.Text.RegularExpressions
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Text.Similarity

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
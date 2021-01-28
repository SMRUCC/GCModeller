#Region "Microsoft.VisualBasic::6685dbfb688ecfc3f54c04bc45a7e5a8, LocalBLAST\Web\KeggSSDB.vb"

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

    '     Module KeggSSDB
    ' 
    '         Function: (+2 Overloads) __export, (+2 Overloads) Export, EXPORT
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Language
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject
Imports SMRUCC.genomics.Interops.NCBI.Extensions.Tasks.Models

Namespace Web

    ''' <summary>
    ''' KEGG SSDB API
    ''' </summary>
    Public Module KeggSSDB

        Public Function EXPORT(source As IEnumerable(Of SSDB.OrthologREST),
                               Optional coverage As Double = 0.8,
                               Optional identities As Double = 0.3) As SpeciesBesthit

            Dim result As New SpeciesBesthit With {
                .sp = source.First.KEGG_ID.Split(":"c).First,
                .hits = LinqAPI.Exec(Of HitCollection) <= From query As SSDB.OrthologREST
                                                          In source.AsParallel
                                                          Select KeggSSDB.Export(query, coverage, identities)
            }
            Return result
        End Function

        <Extension> Public Function Export(source As SSDB.OrthologREST,
                                           Optional coverage As Double = 0.8,
                                           Optional identities As Double = 0.3) As HitCollection

            If source.Orthologs.IsNullOrEmpty Then
                Return New HitCollection With {
                    .Description = source.Definition,
                    .QueryName = source.KEGG_ID.Split(":"c).Last
                }
            Else
                Call Console.Write(".")
            End If
            Dim hits As New HitCollection With {
                .Description = source.Definition,
                .QueryName = source.KEGG_ID.Split(":"c).Last,
                .Hits = LinqAPI.Exec(Of Hit) <= From x As SSDB.SShit
                                                In source.Orthologs
                                                Where x.Coverage >= coverage AndAlso
                                                    Val(x.Identity) >= identities
                                                Select x.__export
            }
            Return hits
        End Function

        Public Function Export(source As IEnumerable(Of SSDB.Ortholog),
                               tag As String,
                               Optional coverage As Double = 0.8,
                               Optional identities As Double = 0.3) As HitCollection

            Dim hits As New HitCollection With {
                .QueryName = tag,
                .hits = source _
                    .Select(AddressOf KeggSSDB.__export) _
                    .ToArray
            }
            Return hits
        End Function

        Private Function __export(kegg As SSDB.Ortholog) As Hit
            Return New Hit With {
                .HitName = kegg.hit_name,
                .Identities = kegg.identity,
                .Positive = kegg.identity,
                .tag = kegg.hit_name.Split(":"c).First
            }
        End Function

        <Extension>
        Private Function __export(kegg As SSDB.SShit) As Hit
            Return New Hit With {
                .HitName = kegg.Entry.locusID,
                .tag = kegg.Entry.speciesID,
                .Identities = Val(kegg.Identity),
                .Positive = Val(kegg.Identity)
            }
        End Function
    End Module
End Namespace

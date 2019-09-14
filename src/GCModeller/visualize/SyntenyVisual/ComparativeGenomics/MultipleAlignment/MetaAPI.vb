#Region "Microsoft.VisualBasic::ccf81f7fe906409b02bf2db248403853, visualize\SyntenyVisual\ComparativeGenomics\MultipleAlignment\MetaAPI.vb"

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

    '     Module MetaAPI
    ' 
    '         Function: CompilePTT, (+2 Overloads) FromMetaData
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.UnixBash
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject.Organism
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat
Imports SMRUCC.genomics.Interops.NCBI.Extensions.Tasks.Models

Namespace ComparativeAlignment

    Public Module MetaAPI

        <Extension>
        Public Function CompilePTT(source As SpeciesBesthit, DIR As String) As Dictionary(Of String, PTT)
            Dim lst As IEnumerable(Of String) = ls - l - r - wildcards("*.PTT") <= DIR
            Dim allSp As String() = LinqAPI.Exec(Of String) <= From prot As HitCollection
                                                               In source.hits
                                                               Select From hit As Hit
                                                                      In prot.hits
                                                                      Select hit.tag
            Dim PTTHash As Dictionary(Of String, String) = lst.ToDictionary(Function(x) x.BaseName.ToLower.Trim)
            Dim names As Dictionary(Of String, String) =
                LinqAPI.BuildHash(Of String, String, NamedValue(Of String))(Function(x) x.Name, Function(x) x.Value) <=
                    From name As String
                    In allSp.Distinct
                    Let sp = EntryAPI.GetValue(name)
                    Where Not sp Is Nothing
                    Select New NamedValue(Of String)(name, sp.Species.ToLower.Trim)

            Dim files = (From x In names
                         Where PTTHash.ContainsKey(x.Value)
                         Select x.Key,
                             path = PTTHash(x.Value)).ToDictionary(Function(x) x.Key,
                                                                   Function(x) PTT.Load(x.path))
            Return files
        End Function

        <Extension>
        Public Function FromMetaData(source As SpeciesBesthit, PTT_DIR As String) As DrawingModel
            Dim res As Dictionary(Of String, PTT) = source.CompilePTT(PTT_DIR)
            Dim result As DrawingModel = source.FromMetaData(res)
            Return result
        End Function

        <Extension>
        Public Function FromMetaData(source As SpeciesBesthit, PTT As Dictionary(Of String, PTT)) As DrawingModel

        End Function
    End Module
End Namespace

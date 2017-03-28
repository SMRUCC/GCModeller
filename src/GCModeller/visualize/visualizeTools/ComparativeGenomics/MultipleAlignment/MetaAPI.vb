#Region "Microsoft.VisualBasic::98147eaa5211f226518d97b050d2102a, ..\visualize\visualizeTools\ComparativeGenomics\MultipleAlignment\MetaAPI.vb"

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

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.UnixBash
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject.Organism
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat
Imports SMRUCC.genomics.Interops.NCBI.Extensions.Analysis

Namespace ComparativeAlignment

    Public Module MetaAPI

        <Extension>
        Public Function CompilePTT(source As BestHit, DIR As String) As Dictionary(Of String, PTT)
            Dim lst As IEnumerable(Of String) = ls - l - r - wildcards("*.PTT") <= DIR
            Dim allSp As String() = LinqAPI.Exec(Of String) <= From prot As HitCollection
                                                               In source.hits
                                                               Select From hit As Hit
                                                                      In prot.Hits
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
        Public Function FromMetaData(source As BestHit, PTT_DIR As String) As DrawingModel
            Dim res As Dictionary(Of String, PTT) = source.CompilePTT(PTT_DIR)
            Dim result As DrawingModel = source.FromMetaData(res)
            Return result
        End Function

        <Extension>
        Public Function FromMetaData(source As BestHit, PTT As Dictionary(Of String, PTT)) As DrawingModel

        End Function
    End Module
End Namespace

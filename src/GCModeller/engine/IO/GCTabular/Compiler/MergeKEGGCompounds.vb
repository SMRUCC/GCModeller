#Region "Microsoft.VisualBasic::d31ffc897cb2189243309a9398afa3d6, engine\IO\GCTabular\Compiler\MergeKEGGCompounds.vb"

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

    '     Class MergeKEGGCompounds
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '         Function: Internal_getCompound
    ' 
    '         Sub: InvokeMergeCompoundSpecies
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv.Extensions

Namespace Compiler.Components

    Public Class MergeKEGGCompounds

        Dim _ModelLoader As FileStream.IO.XmlresxLoader, KEGGCompounds As SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject.Compound()
        Dim _EntryViews As EntryViews

        Sub New(Loader As FileStream.IO.XmlresxLoader, KEGGCompoundsCsv As String)
            Me._ModelLoader = Loader
            Me.KEGGCompounds = KEGGCompoundsCsv.LoadCsv(Of SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject.Compound)(explicit:=False).ToArray
            Call Console.WriteLine("There is {0} metabolites define in the model", _ModelLoader.MetabolitesModel.Count)
            Me._EntryViews = New EntryViews(Loader.MetabolitesModel.Values.AsList)
        End Sub

        Public Sub InvokeMergeCompoundSpecies()
            Dim UpdateCounts As Integer, Insert As Integer

            For Each item In KEGGCompounds
                If item.GetDBLinkManager Is Nothing OrElse item.GetDBLinkManager.IsEmpty Then
                    Continue For
                End If
                Dim cpd As FileStream.Metabolite = Internal_getCompound(item)
                If cpd Is Nothing Then '没有则进行添加
                    Call _EntryViews.AddEntry(item)
                    Insert += 1
                Else
                    '存在，则尝试更新DBLink属性
                    Call _EntryViews.Update(cpd, item)
                    UpdateCounts += 1
                End If
            Next

            Call Console.WriteLine("Job done, {0} was updated dblinks and {1} was insert into the compiled model!", UpdateCounts, Insert)
        End Sub

        ''' <summary>
        ''' 会自动根据目标对象<paramref name="Compound"></paramref>的DBLink属性来从集合之中获取数据
        ''' </summary>
        ''' <param name="Compound"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function Internal_getCompound(Compound As SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject.Compound) As FileStream.Metabolite
            'Dim cpd = _EntryViews.GetByCheBIEntry(Compound.CHEBI)
            'If Not cpd Is Nothing Then
            '    Return cpd
            'End If
            'cpd = _EntryViews.GetByKeggEntry(Compound.Entry)
            'If Not cpd Is Nothing Then
            '    Return cpd
            'End If
            'cpd = _EntryViews.GetByPubChemEntry(Compound.PUBCHEM)
            'Return cpd
        End Function
    End Class
End Namespace

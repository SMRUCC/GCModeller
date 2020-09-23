#Region "Microsoft.VisualBasic::43c405a4a9da09edb8674fcd09363e4f, engine\IO\GCTabular\Compiler\MergeSabiork.vb"

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

    '     Class MergeSabiork
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '         Function: GetCompound, GetItems, LoadSabiorkEnzyme, LoadUniprotBesthits, Matches
    '                   WriteResult
    ' 
    '         Sub: InvokeMergeCompoundSpecies
    '         Class MatchedSabiorkEnzyme
    ' 
    '             Properties: MatchedId
    ' 
    '             Function: CreateObject, ToString
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv.Extensions
Imports SMRUCC.genomics.Data.SabiorkKineticLaws.TabularDump
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH

Namespace Compiler.Components

    <[Namespace]("Sabio-RK")>
    Public Class MergeSabiork

        Public Class MatchedSabiorkEnzyme : Inherits EnzymeCatalystKineticLaw

            Public Property MatchedId As String

            Public Overrides Function ToString() As String
                Return String.Format("{0} <==> {1}", MyBase.Uniprot, MatchedId)
            End Function

            Public Shared Function CreateObject(UniprotBesthit As BestHit, SabiorkEnzyme As EnzymeCatalystKineticLaw) As MatchedSabiorkEnzyme
                Dim EnzymeData As MatchedSabiorkEnzyme = SabiorkEnzyme.Copy(Of MatchedSabiorkEnzyme)()
                EnzymeData.MatchedId = UniprotBesthit.QueryName
                Return EnzymeData
            End Function
        End Class

        <ExportAPI("read.besthitcsv")>
        Public Shared Function LoadUniprotBesthits(path As String) As BBH.BestHit()
            Return path.LoadCsv(Of BBH.BestHit)(False).ToArray
        End Function

        <ExportAPI("read.enzymes")>
        Public Shared Function LoadSabiorkEnzyme(path As String) As EnzymeCatalystKineticLaw()
            Return path.LoadCsv(Of EnzymeCatalystKineticLaw)(False).ToArray
        End Function

        ''' <summary>
        ''' Match enzyme between the target genome and uniprot
        ''' </summary>
        ''' <param name="UniprotBesthits"></param>
        ''' <param name="SabiorkEnzyme"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <ExportAPI("-match_enzyme")>
        Public Shared Function Matches(UniprotBesthits As BestHit(), SabiorkEnzyme As EnzymeCatalystKineticLaw()) As MatchedSabiorkEnzyme()

            Dim LQuery = (From Enzyme In SabiorkEnzyme.AsParallel
                          Select New KeyValuePair(Of EnzymeCatalystKineticLaw, BestHit())(Enzyme, GetItems(UniprotBesthits, Enzyme.Uniprot))).ToArray
            Dim List As List(Of MatchedSabiorkEnzyme) = New List(Of MatchedSabiorkEnzyme)
            For Each Line In LQuery
                Dim ChunkBuffer As MatchedSabiorkEnzyme() = (From item In Line.Value Select MatchedSabiorkEnzyme.CreateObject(item, Line.Key)).ToArray
                Call List.AddRange(ChunkBuffer)
            Next
            Return List.ToArray
        End Function

        <ExportAPI("write.matched_enzyme")>
        Public Shared Function WriteResult(data As MatchedSabiorkEnzyme(), savefile As String) As Boolean
            Try
                Call data.SaveTo(savefile, False)
            Catch ex As Exception
                Call Console.WriteLine(ex.ToString)
                Return False
            End Try
            Return True
        End Function

        Private Shared Function GetItems(UniprotBesthits As BBH.BestHit(), UniprotId As String) As BBH.BestHit()
            Dim LQuery = (From item In UniprotBesthits Where String.Equals(item.HitName, UniprotId, StringComparison.OrdinalIgnoreCase) Select item).ToArray
            Return LQuery
        End Function

        Dim _ModelLoader As FileStream.IO.XmlresxLoader
        Dim _SabiorkCompoundsCSV As String
        Dim _EntryViews As EntryViews

        Sub New(ModelLoader As FileStream.IO.XmlresxLoader, SabiorkCompoundsCsv As String)
            Me._ModelLoader = ModelLoader
            Me._SabiorkCompoundsCSV = SabiorkCompoundsCsv
        End Sub

        Public Sub InvokeMergeCompoundSpecies()
            Dim UpdateCounts As Integer = 0
            Dim InsertCounts As Integer = 0
            Dim scpd = LoadCsv(Of CompoundSpecie)(Path:=Me._SabiorkCompoundsCSV, explicit:=False)

            Call Console.WriteLine(Me._ModelLoader.ToString)

            Me._EntryViews = New EntryViews(_ModelLoader.MetabolitesModel.Values.AsList)

            For Each item In scpd
                If item.GetDBLinkManager Is Nothing OrElse item.GetDBLinkManager.IsEmpty Then
                    Continue For
                End If
                Dim cpd As FileStream.Metabolite = GetCompound(item)
                If cpd Is Nothing Then '没有则进行添加
                    Call Me._EntryViews.AddEntry(item)
                    InsertCounts = InsertCounts + 1
                Else
                    Call Me._EntryViews.Update(cpd, item)   '存在，则尝试更新DBLink属性
                    UpdateCounts = UpdateCounts + 1
                End If
            Next

            Call Console.WriteLine("Job done, {0} was updated dblinks and {1} was insert into the compiled model!", UpdateCounts, InsertCounts)
        End Sub

        Private Function GetCompound(SabiorkCompound As CompoundSpecie) As FileStream.Metabolite
            'Dim cpd = _EntryViews.GetByCheBIEntry(SabiorkCompound.ICompoundObjectCHEBI_values)
            'If Not cpd Is Nothing Then
            '    Return cpd
            'End If
            'cpd = _EntryViews.GetByKeggEntry(SabiorkCompound.KEGG_Compound)
            'If Not cpd Is Nothing Then
            '    Return cpd
            'End If
            'cpd = _EntryViews.GetByPubChemEntry(SabiorkCompound.PUBCHEM)
            'Return cpd
        End Function
    End Class
End Namespace

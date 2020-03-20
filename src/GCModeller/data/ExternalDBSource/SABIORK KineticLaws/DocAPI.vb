#Region "Microsoft.VisualBasic::2fe9f0a8b01fbfcc4f093ba9da072f31, data\ExternalDBSource\SABIORK KineticLaws\DocAPI.vb"

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

'     Module DocAPI
' 
'         Function: Download, GetIdentifier, GetIdentifiers, LoadDocument, QueryUsing_KEGGId
' 
'         Sub: (+4 Overloads) ExportDatabase
' 
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv.Extensions
Imports Microsoft.VisualBasic.IEnumerations
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Text.Parser.HtmlParser
Imports SMRUCC.genomics.Data.SabiorkKineticLaws.TabularDump

Namespace SabiorkKineticLaws

    <Package("Sabio-rk.DbAPI")>
    Public Module DocAPI

        <ExportAPI("GET.Identifier")>
        Public Function GetIdentifier(strData As String(), Keyword As String) As String
            Dim LQuery As String = LinqAPI.DefaultFirst(Of String) <=
                From strItem As String
                In strData
                Where InStr(strItem, Keyword)
                Select strItem.Split(CChar("/")).Last

            Return LQuery
        End Function

        <ExportAPI("GET.Identifiers")>
        Public Function GetIdentifiers(strData As String(), Keyword As String) As String()
            Dim LQuery As String() = LinqAPI.Exec(Of String) <=
                From strItem As String
                In strData
                Where InStr(strItem, Keyword)
                Select strItem.Split(CChar("/")).Last
            Return LQuery
        End Function

        <ExportAPI("Db.Export")>
        Public Sub ExportDatabase(DataDir As String, ExportDir As String)
            Dim KineticLawModels As KineticLawModel() = Nothing
            Dim CompoundSpecies As CompoundSpecie() = Nothing
            Dim EnzymeModifiers As EnzymeModifier() = Nothing
            Dim ModifierKinetics As ModifierKinetics() = Nothing
            Dim EnzymeCatalystKineticLaws As EnzymeCatalystKineticLaw() = Nothing

            Call ExportDatabase(DataDir, KineticLawModels, CompoundSpecies, EnzymeModifiers, ModifierKinetics, EnzymeCatalystKineticLaws)

            Call KineticLawModels.SaveTo(String.Format("{0}/KineticLawModels.csv", ExportDir), False)
            Call CompoundSpecies.SaveTo(String.Format("{0}/CompoundSpecies.csv", ExportDir), False)
            Call EnzymeModifiers.SaveTo(String.Format("{0}/EnzymeModifiers.csv", ExportDir), False)
            Call ModifierKinetics.SaveTo(String.Format("{0}/ModifierKinetics.csv", ExportDir), False)
            Call EnzymeCatalystKineticLaws.SaveTo(String.Format("{0}/EnzymeCatalystKineticLaws.csv", ExportDir), False)
        End Sub

        <ExportAPI("Db.Export")>
        Public Sub ExportDatabase(data As SABIORK(), ExportDir As String)
            Dim KineticLawModels As KineticLawModel() = Nothing
            Dim CompoundSpecies As CompoundSpecie() = Nothing
            Dim EnzymeModifiers As EnzymeModifier() = Nothing
            Dim ModifierKinetics As ModifierKinetics() = Nothing
            Dim EnzymeCatalystKineticLaws As EnzymeCatalystKineticLaw() = Nothing

            Call ExportDatabase(data, KineticLawModels, CompoundSpecies, EnzymeModifiers, ModifierKinetics, EnzymeCatalystKineticLaws)

            Call KineticLawModels.SaveTo(String.Format("{0}/KineticLawModels.csv", ExportDir), False)
            Call CompoundSpecies.SaveTo(String.Format("{0}/CompoundSpecies.csv", ExportDir), False)
            Call EnzymeModifiers.SaveTo(String.Format("{0}/EnzymeModifiers.csv", ExportDir), False)
            Call ModifierKinetics.SaveTo(String.Format("{0}/ModifierKinetics.csv", ExportDir), False)
            Call EnzymeCatalystKineticLaws.SaveTo(String.Format("{0}/EnzymeCatalystKineticLaws.csv", ExportDir), False)
        End Sub

        <ExportAPI("Db.Export")>
        Public Sub ExportDatabase(data As SABIORK(),
 _
                      ByRef KineticLawModels As KineticLawModel(),
                      ByRef CompoundSpecies As CompoundSpecie(),
                      ByRef EnzymeModifiers As EnzymeModifier(),
                      ByRef ModifierKinetics As ModifierKinetics(),
                      ByRef EnzymeCatalystKineticLaws As EnzymeCatalystKineticLaw())

            Dim CompoundSpeciesDict As SortedDictionary(Of String, CompoundSpecie) = New SortedDictionary(Of String, CompoundSpecie)
            Dim Enzymes As SortedDictionary(Of String, EnzymeModifier) = New SortedDictionary(Of String, EnzymeModifier)
            Dim KineticLaws As List(Of KineticLawModel) = New List(Of KineticLawModel)
            Dim ModifierKineticsList = New List(Of ModifierKinetics)
            Dim EnzymeCatalystKineticLawsList = New List(Of EnzymeCatalystKineticLaw)

            For Each ItemObject In data
                Dim Compounds = CompoundSpecie.CreateObjects(ItemObject)
                For Each cps In Compounds
                    If Not CompoundSpeciesDict.ContainsKey(cps.CommonNames.First) Then
                        Call CompoundSpeciesDict.Add(cps.CommonNames.First, cps)
                    End If
                Next
                For Each Enzyme In EnzymeModifier.CreateObjects(ItemObject)
                    If Not Enzymes.ContainsKey(Enzyme.CommonName) Then
                        Call Enzymes.Add(Enzyme.CommonName, Enzyme)
                    End If
                Next

                Call ModifierKineticsList.AddRange(SabiorkKineticLaws.LocalParameterParser.TryParseModifierKinetic(ItemObject))
                Call EnzymeCatalystKineticLawsList.AddRange(SabiorkKineticLaws.LocalParameterParser.TryParseEnzymeCatalyst(ItemObject))
                Call KineticLaws.Add(KineticLawModel.CreateObject(ItemObject))
            Next

            CompoundSpecies = CompoundSpeciesDict.Values.ToArray
            KineticLawModels = (From ItemObject In KineticLaws Select ItemObject Order By ItemObject.Ec Ascending).ToArray
            EnzymeModifiers = Enzymes.Values.ToArray
            ModifierKinetics = ModifierKineticsList.TrimNull
            EnzymeCatalystKineticLaws = EnzymeCatalystKineticLawsList.TrimNull
        End Sub

        <ExportAPI("Db.Export")>
        Public Sub ExportDatabase(DataDir As String,
 _
                      ByRef KineticLawModels As KineticLawModel(),
                      ByRef CompoundSpecies As CompoundSpecie(),
                      ByRef EnzymeModifiers As EnzymeModifier(),
                      ByRef ModifierKinetics As ModifierKinetics(),
                      ByRef EnzymeCatalystKineticLaws As EnzymeCatalystKineticLaw())

            Dim LQuery = (From strPath As String
                          In FileIO.FileSystem.GetFiles(DataDir, FileIO.SearchOption.SearchTopLevelOnly, "*.sbml").AsParallel
                          Where FileIO.FileSystem.GetFileInfo(strPath).Length > 0
                          Select SabiorkKineticLaws.SBMLParser.kineticLawModel.LoadDocument(strPath)).ToArray 'Read sbml file document from the filesystem

            Call ExportDatabase(LQuery, KineticLawModels, CompoundSpecies, EnzymeModifiers, ModifierKinetics, EnzymeCatalystKineticLaws)
        End Sub

        Public Const KEGG_QUERY_ENTRY As String = "http://sabiork.h-its.org/sabioRestWebServices/reactions/reactionIDs?q=KeggReactionID:"

        <ExportAPI("Load.Doc")>
        Public Function LoadDocument(FilePath As String) As SABIORK
            Return SabiorkKineticLaws.SBMLParser.kineticLawModel.LoadDocument(FilePath)
        End Function

        <ExportAPI("Query.KEGG")>
        <Extension>
        Public Iterator Function QueryUsing_KEGGId(IdList As String(), ExportDir As String) As IEnumerable(Of String)
            For Each Id As String In IdList
                Dim url As String = (KEGG_QUERY_ENTRY & Id)
                Dim PageContent = url.GET
                Dim Entries As String() = (From m As Match In Regex.Matches(PageContent, "<SabioReactionID>\d+</SabioReactionID>") Select m.Value.GetValue).ToArray

                For Each Entry In Entries
                    Dim File = String.Format("{0}/{1}-{2}.sbml", ExportDir, Id, Entry)

                    url = SABIORK.URL_SABIORK_KINETIC_LAWS_QUERY & Entry
                    Call url.GET.SaveTo(File)
                Next
            Next
        End Function

        <ExportAPI("SABIORK.Downloads")>
        Public Function Download(Dir As String) As Integer
            Dim c As Integer = 0

            For i As Integer = FileIO.FileSystem.GetFiles(Dir).Count + 1 To Integer.MaxValue
                Dim id As String = "kinlawids_" & i
                Dim url As String = SABIORK.URL_SABIORK_KINETIC_LAWS_QUERY & i
                Dim File = String.Format("{0}/{1}.sbml", Dir, id)

                Call url.GET.SaveTo(File)

                If FileIO.FileSystem.GetFileInfo(File).Length < 100 Then
                    c += 1
                    If c > 1500 Then
                        Exit For
                    End If
                Else
                    c = 0
                End If

                Call Threading.Thread.Sleep(10)
            Next

            Return 0
        End Function
    End Module
End Namespace

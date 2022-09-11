#Region "Microsoft.VisualBasic::337242bbd630aba96a92ea84b5b61609, GCModeller\data\SABIO-RK\Dumps\DocAPI.vb"

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

    '   Total Lines: 128
    '    Code Lines: 109
    ' Comment Lines: 0
    '   Blank Lines: 19
    '     File Size: 6.94 KB


    '     Module DocAPI
    ' 
    '         Function: GetIdentifier, GetIdentifiers
    ' 
    '         Sub: (+4 Overloads) ExportDatabase
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv.Extensions
Imports Microsoft.VisualBasic.IEnumerations
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Data.SABIORK.TabularDump

Namespace SBML

    <Package("Sabio-rk.DbAPI")>
    Public Module DocAPI

        <ExportAPI("GET.Identifier")>
        <Extension>
        Public Function GetIdentifier(strData As String(), Keyword As String) As String
            Dim LQuery As String = LinqAPI.DefaultFirst(Of String) <=
                From strItem As String
                In strData
                Where InStr(strItem, Keyword)
                Select strItem.Split(CChar("/")).Last

            Return LQuery
        End Function

        <ExportAPI("GET.Identifiers")>
        <Extension>
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
        Public Sub ExportDatabase(data As SabiorkSBML(), ExportDir As String)
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
        Public Sub ExportDatabase(data As SabiorkSBML(),
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

                Call ModifierKineticsList.AddRange(LocalParameterParser.TryParseModifierKinetic(ItemObject))
                Call EnzymeCatalystKineticLawsList.AddRange(LocalParameterParser.TryParseEnzymeCatalyst(ItemObject))
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
                          Select SBMLParser.kineticLawModel.LoadDocument(strPath)).ToArray 'Read sbml file document from the filesystem

            Call ExportDatabase(LQuery, KineticLawModels, CompoundSpecies, EnzymeModifiers, ModifierKinetics, EnzymeCatalystKineticLaws)
        End Sub
    End Module
End Namespace

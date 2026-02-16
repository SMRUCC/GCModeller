#Region "Microsoft.VisualBasic::1b4af0aef0b0a6f6a3b7d32b5ce310ed, R#\biosystem\Sabiork.vb"

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

    '   Total Lines: 127
    '    Code Lines: 90 (70.87%)
    ' Comment Lines: 23 (18.11%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 14 (11.02%)
    '     File Size: 4.94 KB


    ' Module sabiork_repository
    ' 
    '     Function: createNewRepository, documentReader, enzyme_info, getKineticis, getMetabolites
    '               openRepository, parseSbml, query, unset
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.ComponentModel.DBLinkBuilder
Imports SMRUCC.genomics.Data
Imports SMRUCC.genomics.Data.SABIORK
Imports SMRUCC.genomics.Data.SABIORK.docuRESTfulWeb
Imports SMRUCC.genomics.Data.SABIORK.SBML
Imports SMRUCC.genomics.Data.SABIORK.TabularDump
Imports SMRUCC.genomics.Model.SBML.Level3
Imports SMRUCC.Rsharp
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop

''' <summary>
''' sabio-rk data repository
''' </summary>
<Package("sabiork")>
Public Module sabiork_repository

    <ExportAPI("new")>
    Public Function createNewRepository(file As String) As SabiorkRepository
        Return New SabiorkRepository(file.Open(FileMode.OpenOrCreate, doClear:=True))
    End Function

    <ExportAPI("open")>
    Public Function openRepository(file As String) As SabiorkRepository
        Return New SabiorkRepository(file.Open(FileMode.OpenOrCreate, doClear:=False))
    End Function

    <ExportAPI("query")>
    <RApiReturn(GetType(SbmlDocument))>
    Public Function query(ec_number As String, cache As SabiorkRepository) As Object
        Return cache.GetByECNumber(ec_number)
    End Function

    <ExportAPI("get_kineticis")>
    <RApiReturn(GetType(EnzymeCatalystKineticLaw))>
    Public Function getKineticis(cache As SabiorkRepository, ec_number As String) As Object
        Return cache.GetKineticisLaw(ec_number).ToArray
    End Function

    ''' <summary>
    ''' parse the sbml xml document text as kineticis data
    ''' </summary>
    ''' <param name="data">
    ''' the xml document text or the file path to the sbml xml document file.
    ''' </param>
    ''' <returns>
    ''' a SBML document data that contains list of reactions and list of the lambda formula expression 
    ''' </returns>
    <ExportAPI("parseSbml")>
    Public Function parseSbml(data As String) As SbmlDocument
        Dim xml As String = data.LineIterators.JoinBy(vbLf)
        Dim model As SbmlDocument = ModelQuery.parseSBML(xml, schema:=GetType(SbmlDocument))
        Return model
    End Function

    <ExportAPI("unset_sbml")>
    Public Function unset(x As SbmlDocument) As EnzymeCatalystKineticLaw()
        If x.empty Then
            Return {}
        Else
            Return ModelHelper.CreateKineticsData(x) _
                .Select(Function(a)
                            Return a.Item2
                        End Function) _
                .ToArray
        End If
    End Function

    ''' <summary>
    ''' Create a helper reader for load element model from the sbml document
    ''' </summary>
    ''' <param name="sbml"></param>
    ''' <returns></returns>
    <ExportAPI("sbmlReader")>
    Public Function documentReader(sbml As SbmlDocument) As SBMLInternalIndexer
        Return New SBMLInternalIndexer(sbml)
    End Function

    <ExportAPI("metabolite_species")>
    Public Function getMetabolites(sbml As SBMLInternalIndexer, reaction As SBMLReaction) As Object
        Dim list As list = list.empty
        Dim entity As species
        Dim db_xrefs As DBLink()

        For Each sp As SpeciesReference In reaction.listOfProducts.JoinIterates(reaction.listOfReactants)
            entity = sbml.getSpecies(sp.species)
            db_xrefs = entity.db_xrefs.SafeQuery.ToArray
            list.add(sp.species, New list With {
                .slots = New Dictionary(Of String, Object) From {
                    {"name", entity.name},
                    {"xrefs", New dataframe With {
                        .columns = New Dictionary(Of String, Array) From {
                            {"source", db_xrefs.Select(Function(d) d.DBName).ToArray},
                            {"xref", db_xrefs.Select(Function(d) d.entry).ToArray}
                        }
                    }}
                }
            })
        Next

        Return list
    End Function

    ''' <summary>
    ''' get enzyme info of a given reaction model
    ''' </summary>
    ''' <param name="sbml"></param>
    ''' <param name="reaction"></param>
    ''' <returns></returns>
    <ExportAPI("enzyme_info")>
    <RApiReturn("ec_number", "uniprot")>
    Public Function enzyme_info(sbml As SBMLInternalIndexer, reaction As SBMLReaction) As Object
        Dim ec_number As String = reaction.ec_number
        Dim enz = reaction.listOfModifiers.SafeQuery.Select(Function(r) sbml.getSpecies(r.species)).ToArray
        Dim db_xrefs = enz.Select(Function(e) e.db_xrefs).IteratesALL.ToArray
        Dim info As New list(slot("ec_number") = ec_number)

        For Each external In db_xrefs.GroupBy(Function(d) d.DBName)
            Call info.add(external.Key, external.Select(Function(d) d.entry))
        Next

        Return info
    End Function
End Module

﻿#Region "Microsoft.VisualBasic::38a574dc531569061df9f37d01c744de, G:/GCModeller/src/repository/Bio.Repository//HDS/SabiorkRepository.vb"

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

    '   Total Lines: 135
    '    Code Lines: 90
    ' Comment Lines: 25
    '   Blank Lines: 20
    '     File Size: 5.26 KB


    ' Class SabiorkRepository
    ' 
    '     Constructor: (+1 Overloads) Sub New
    ' 
    '     Function: GetByECNumber, getEcNumberDirectoryPath, GetKineticisLaw
    ' 
    '     Sub: (+2 Overloads) Dispose, saveKineticsModel
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports Microsoft.VisualBasic.DataStorage.HDSPack
Imports Microsoft.VisualBasic.DataStorage.HDSPack.FileSystem
Imports Microsoft.VisualBasic.MIME.application.xml.MathML
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.ComponentModel.Annotation
Imports SMRUCC.genomics.Data.SABIORK
Imports SMRUCC.genomics.Data.SABIORK.SBML
Imports SMRUCC.genomics.Data.SABIORK.TabularDump

Public Class SabiorkRepository : Implements IDisposable

    ReadOnly cache As StreamPack
    ReadOnly webRequest As ModelQuery
    ReadOnly enzyme_class As Dictionary(Of String, String)

    Private disposedValue As Boolean

    Sub New(file As Stream)
        Me.cache = New StreamPack(file, meta_size:=32 * 1024 * 1024, [readonly]:=False)
        Me.webRequest = New ModelQuery(cache)
        Me.enzyme_class = Enums(Of EnzymeClasses)() _
            .ToDictionary(Function(c) CInt(c).ToString,
                          Function(c)
                              Return c & "." & c.Description
                          End Function)
    End Sub

    Public Iterator Function GetKineticisLaw(ec_number As String) As IEnumerable(Of EnzymeCatalystKineticLaw)
        Dim pathDir As String = getEcNumberDirectoryPath(ec_number) & "/"
        Dim folder As StreamGroup = cache.GetObject(pathDir)

        If folder Is Nothing Then
            Return
        Else
            For Each file As StreamObject In folder.files
                If TypeOf file Is StreamBlock Then
                    Yield cache.ReadText(file.ToString).LoadJSON(Of EnzymeCatalystKineticLaw)
                End If
            Next
        End If
    End Function

    ''' <summary>
    ''' web query of the sabio-rk document content data
    ''' </summary>
    ''' <param name="ec_number"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' url = `https://sabiork.h-its.org/sabioRestWebServices/searchKineticLaws/sbml?q=ecnumber:${num}`;
    ''' </remarks>
    Public Function GetByECNumber(ec_number As String) As SbmlDocument
        Dim q As New Dictionary(Of QueryFields, String) From {
            {QueryFields.ECNumber, ec_number}
        }
        Dim str As String = webRequest.QueryCacheText(q)
        Dim result As SbmlDocument = ModelQuery.parseSBML(str)

        ' 20221112 andalso write kinetics model data 
        ' to the repository package
        If Not result Is Nothing Then
            Call saveKineticsModel(ec_number, model:=result)
        End If

        Return result
    End Function

    Private Function getEcNumberDirectoryPath(ec_number As String) As String
        Dim numbers As String() = ec_number.Split("."c)
        Dim pathDir As String = $"/{enzyme_class(numbers(Scan0).ToString)}/{numbers.Skip(1).JoinBy("/")}"

        Return pathDir
    End Function

    ' V = Vmax[S] / ( KM + [S])
    ' V = Kcat[E]t[S] / ( KM + [S])
    ' [S] substracte concentration
    ' [E]t enzyme concentration
    ' Vmax = kcat[E]

    Private Sub saveKineticsModel(ec_number As String, model As SbmlDocument)
        Dim mathList = model.mathML.ToDictionary(Function(a) a.Name, Function(a) a.Value)
        Dim path As String
        Dim math As LambdaExpression
        Dim mathId As String
        Dim json As String
        Dim kineticisModel As EnzymeCatalystKineticLaw
        Dim indexer As New SBMLInternalIndexer(model)
        Dim pathDir As String = getEcNumberDirectoryPath(ec_number)

        For Each rxn As SBMLReaction In model.sbml.model.listOfReactions
            path = $"{pathDir}/{rxn.id}.json"
            mathId = "KL_" & rxn.kineticLawID
            math = mathList.TryGetValue(mathId)

            If math Is Nothing OrElse math.lambda Is Nothing Then
                Continue For
            ElseIf math.parameters.Length <> rxn.kineticLaw.math.apply.ci.Length Then
                Continue For
            Else
                kineticisModel = EnzymeCatalystKineticLaw.Create(rxn, math, doc:=indexer)
                json = kineticisModel.GetJson
            End If

            Call cache.Delete(path)
            Call cache.WriteText(json, path)
        Next
    End Sub

    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                ' TODO: dispose managed state (managed objects)
                Call cache.Dispose()
            End If

            ' TODO: free unmanaged resources (unmanaged objects) and override finalizer
            ' TODO: set large fields to null
            disposedValue = True
        End If
    End Sub

    ' ' TODO: override finalizer only if 'Dispose(disposing As Boolean)' has code to free unmanaged resources
    ' Protected Overrides Sub Finalize()
    '     ' Do not change this code. Put cleanup code in 'Dispose(disposing As Boolean)' method
    '     Dispose(disposing:=False)
    '     MyBase.Finalize()
    ' End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        ' Do not change this code. Put cleanup code in 'Dispose(disposing As Boolean)' method
        Dispose(disposing:=True)
        GC.SuppressFinalize(Me)
    End Sub
End Class


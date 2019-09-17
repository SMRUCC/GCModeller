#Region "Microsoft.VisualBasic::17b99860a908689cc6974f6256967d9e, engine\IO\GCTabular\Compiler\Downloader\MetaboliteInformationDownloader.vb"

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

    '     Class MetaboliteInformationDownloader
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: Match
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports SMRUCC.genomics.Assembly.ELIXIR.EBI.ChEBI.Database.IO.StreamProviders.Tsv
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject
Imports SMRUCC.genomics.ComponentModel

Namespace Compiler.Components

    Public Class MetaboliteInformationDownloader

        Dim ChEBILoader As TSVTables

        Sub New(ChEBIDir As String)
            ChEBILoader = New TSVTables(ChEBIDir)
        End Sub

        Public Function Match(Model As FileStream.IO.XmlresxLoader) As Integer
            Dim Metabolites = Model.MetabolitesModel
            Dim ChEBINames = ChEBILoader.GetNames
            Dim ChEBIAccessions = ChEBILoader.GetDatabaseAccessions
            Dim Counts As Integer

            Dim DonwloadList As List(Of DownloadedData) = New List(Of DownloadedData)

            Using log As New StreamWriter(File.Open("./model_optimization_logs.log", FileMode.OpenOrCreate))
                For Each metabolite In Metabolites.Values
                    If Not String.IsNullOrEmpty(metabolite.KEGGCompound) Then
                        Continue For
                    End If

                    If metabolite.MolWeight > 0 Then
                        Continue For
                    End If

                    If metabolite.CommonNames.IsNullOrEmpty Then
                        Continue For
                    End If

                    For Each CommonName As String In metabolite.CommonNames
                        Dim Result = (From item In ChEBINames.AsParallel Where FuzzyMatching(CommonName, item.NAME) Select item.COMPOUND_ID).ToArray
                        Dim KEGGCompound = (From item In ChEBIAccessions
                                            Where Array.IndexOf(Result, item.COMPOUND_ID) > -1 AndAlso
                                                String.Equals(item.TYPE, "KEGG COMPOUND accession")
                                            Select item).FirstOrDefault

                        If KEGGCompound Is Nothing Then
                            Continue For
                        End If

                        Dim Compound = MetaboliteWebApi.DownloadCompound(KEGGCompound.ACCESSION_NUMBER)

                        metabolite.KEGGCompound = Compound.Entry
                        metabolite.MolWeight = Compound.MolWeight
                        If metabolite.MolWeight = 0.0R AndAlso Not String.IsNullOrEmpty(Compound.Formula) Then
                            metabolite.MolWeight = PeriodicTable.MolecularWeightCalculate(Compound.Formula)
                        End If

                        Call DonwloadList.Add(New DownloadedData With {.CommonName = CommonName, .KEGGCompounds = Compound, .MetaCycId = metabolite.Identifier})

                        Dim msg As String = String.Format("[{0}] {1}  <->  {2}:  {3}", metabolite.Identifier, CommonName, Result.FirstOrDefault, Compound.CommonNames.FirstOrDefault)
                        Call log.WriteLine(msg)

                        Counts += 1

                        Exit For
                    Next
                Next
            End Using

            Model.MetabolitesModel = Metabolites
            Call DonwloadList.GetXml.SaveTo("./downloadlist.xml")
            Call New MolecularWeightCalculator().CalculateK(Model)

            Return Counts
        End Function
    End Class
End Namespace

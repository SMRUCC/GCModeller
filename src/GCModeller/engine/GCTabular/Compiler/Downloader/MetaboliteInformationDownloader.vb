#Region "Microsoft.VisualBasic::a4c76a0c9a324aa7e3383f9a2d05fbe8, ..\GCModeller\engine\GCTabular\Compiler\Downloader\MetaboliteInformationDownloader.vb"

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

Imports System.IO
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Text
Imports Microsoft.VisualBasic.Text.Similarity
Imports SMRUCC.genomics.Assembly.EBI.ChEBI.Database.IO.StreamProviders.Tsv

Namespace Compiler.Components

    Public Class MetaboliteInformationDownloader

        Dim ChEBILoader As TSV

        Sub New(ChEBIDir As String)
            ChEBILoader = New TSV(ChEBIDir)
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

                    If Metabolite.MolWeight > 0 Then
                        Continue For
                    End If

                    If Metabolite.CommonNames.IsNullOrEmpty Then
                        Continue For
                    End If

                    For Each CommonName As String In Metabolite.CommonNames
                        Dim Result = (From item In ChEBINames.AsParallel Where FuzzyMatching(CommonName, item.NAME) Select item.COMPOUND_ID).ToArray
                        Dim KEGGCompound = (From item In ChEBIAccessions
                                            Where Array.IndexOf(Result, item.COMPOUND_ID) > -1 AndAlso
                                                String.Equals(item.TYPE, "KEGG COMPOUND accession")
                                            Select item).FirstOrDefault

                        If KEGGCompound Is Nothing Then
                            Continue For
                        End If

                        Dim Compound = SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject.Compound.Download(KEGGCompound.ACCESSION_NUMBER)

                        metabolite.KEGGCompound = Compound.Entry
                        metabolite.MolWeight = Compound.MolWeight
                        If metabolite.MolWeight = 0.0R AndAlso Not String.IsNullOrEmpty(Compound.Formula) Then
                            metabolite.MolWeight = SMRUCC.genomics.ComponentModel.PeriodicTable.MolecularWeightCalculate(Compound.Formula)
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

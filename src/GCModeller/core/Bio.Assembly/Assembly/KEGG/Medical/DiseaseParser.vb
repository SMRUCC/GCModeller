#Region "Microsoft.VisualBasic::d61605d708499cb04f4fc5bc34493334, Bio.Assembly\Assembly\KEGG\Medical\DiseaseParser.vb"

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

    '     Module DiseaseParser
    ' 
    '         Function: CreateDiseaseModel, ParseStream
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject
Imports SMRUCC.genomics.ComponentModel.DBLinkBuilder

Namespace Assembly.KEGG.Medical

    Public Module DiseaseParser

        Public Iterator Function ParseStream(path$) As IEnumerable(Of Disease)
            Dim lines$() = path.ReadAllLines
            Dim ref As Reference() = Nothing

            For Each pack As String() In lines.Split("///")
                Yield pack.ParseStream(ref).CreateDiseaseModel(ref)
            Next
        End Function

        <Extension>
        Public Function CreateDiseaseModel(getValue As Func(Of String, String()), ref As Reference()) As Disease
            Return New Disease With {
                .References = ref,
                .Entry = getValue("ENTRY").FirstOrDefault.Split.First,
                .Names = getValue("NAME") _
                    .Where(Function(s) Not s.StringEmpty) _
                    .ToArray,
                .Comments = getValue("COMMENT").JoinBy(" "),
                .Carcinogen = getValue("CARCINOGEN"),
                .Category = getValue("CATEGORY").FirstOrDefault,
                .DbLinks = getValue("DBLINKS") _
                    .Select(AddressOf DBLink.FromTagValue) _
                    .ToArray,
                .Description = getValue("DESCRIPTION").FirstOrDefault,
                .Drugs = getValue("DRUG"),
                .Env_factors = getValue("ENV_FACTOR"),
                .Genes = getValue("GENE"),
                .Markers = getValue("MARKER"),
                .Pathogens = getValue("PATHOGEN")
            }
        End Function
    End Module
End Namespace

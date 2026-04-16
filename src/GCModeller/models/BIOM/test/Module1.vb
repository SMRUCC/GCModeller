#Region "Microsoft.VisualBasic::ba475acd58aa0913127fc53b006783e9, models\BIOM\test\Module1.vb"

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

    ' Module Module1
    ' 
    '     Sub: BoxPlot, exports, jsonDumpTest, loadertest, Main
    '          testCDFBIOM
    ' 
    ' Class DDDD
    ' 
    '     Properties: d
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.MIME.application.json
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.foundation.BIOM.v10

Module Module1

    Const JsonLongTime$ = "\d+-\d+-\d+T\d+:\d+:\d+\.\d+"

    Sub Main()

        ' Call exports()

        ' Call loadertest()


        Call testCDFBIOM()

        '2016-10-31T17:30:49.768484

        Dim ddd = Regex.Match("2016-10-31T17:30:49.768484", JsonLongTime).Value
        Dim json = Date.Parse(ddd).GetJson(indent:=False, simpleDict:=True)

        Dim biom = SMRUCC.genomics.foundation.BIOM.v10.BIOMDataSet(Of Double).LoadFile("C:\Users\xieguigang\Desktop\predictions_ko.L3.biom.json")

        Call biom.GetJson(indent:=True, simpleDict:=True).SaveTo("C:\Users\xieguigang\Desktop\predictions_ko.L3.biom.formatted.json")

        Pause()
    End Sub

    Sub loadertest()


        ' Dim json As JsonElement = Microsoft.VisualBasic.MIME.application.json.ParseJsonFile("D:\GCModeller\src\GCModeller\models\BIOM\data\Minimal_dense_OTU_table.json")

        'Dim modify = json.BuildJsonString

        Dim aaaaa = SMRUCC.genomics.foundation.BIOM.ReadAuto("D:\GCModeller\src\GCModeller\models\BIOM\data\Minimal_dense_OTU_table.json")
        Dim ccccc = SMRUCC.genomics.foundation.BIOM.ReadAuto("D:\GCModeller\src\GCModeller\models\BIOM\data\Minimal_sparse_OTU_table.json")
        Dim ddddd = SMRUCC.genomics.foundation.BIOM.ReadAuto("D:\GCModeller\src\GCModeller\models\BIOM\data\Rich_dense_OTU_table.json")
        Dim eeeee = SMRUCC.genomics.foundation.BIOM.ReadAuto("D:\GCModeller\src\GCModeller\models\BIOM\data\Rich_sparse_OTU_table.json")


        Dim needsConversion = eeeee.RequiredConvertToDenseMatrix
        Dim matrix = eeeee.data.ToDenseMatrix(eeeee.shape)

        needsConversion = ddddd.RequiredConvertToDenseMatrix
        matrix = ddddd.data.ToSparseMatrix(Function(x) x = 0.0)


        Dim [default] = SMRUCC.genomics.foundation.BIOM.ReadAuto("D:\GCModeller\src\GCModeller\models\BIOM\data\Rich_sparse_OTU_table.json")
        Dim dense = SMRUCC.genomics.foundation.BIOM.ReadAuto("D:\GCModeller\src\GCModeller\models\BIOM\data\Rich_sparse_OTU_table.json", denseMatrix:=True)

        Pause()
    End Sub

    Sub jsonDumpTest(file As String)
        Dim biom = SMRUCC.genomics.foundation.BIOM.v21.ReadFile(file)

        Call biom.GetJson(indent:=True, simpleDict:=True).SaveTo(file.ChangeSuffix("json"))
    End Sub

    Sub testCDFBIOM()

        Call jsonDumpTest("D:\GCModeller\src\GCModeller\models\BIOM\data\EP418446_K40_BS1D.otu_table.biom")
        Call jsonDumpTest("D:\GCModeller\src\GCModeller\models\BIOM\data\EP034068_K60_BS1D.otu_table.biom")

        Pause()
    End Sub

    Sub exports()
        Dim matrix As New Dictionary(Of String, [Property](Of Double))

        For Each biom As String In ls - l - r - "*.biom" <= "W:\HMP_biom\downloads.hmpdacc.org\ihmp\ptb\genome\microbiome\16s\analysis\hmqcp"
            Dim json As BIOMDataSet(Of Double) = SMRUCC.genomics.foundation.BIOM.v21.ReadFile(biom)

            For Each otu In json.PopulateRows
                If Not matrix.ContainsKey(otu.Name) Then
                    matrix.Add(otu.Name, New [Property](Of Double))
                End If

                matrix(otu.Name) += otu.Value
            Next
        Next

        Pause()
    End Sub

    Sub BoxPlot()

        '

    End Sub

End Module

Public Class DDDD


    Public Property d As Date

End Class

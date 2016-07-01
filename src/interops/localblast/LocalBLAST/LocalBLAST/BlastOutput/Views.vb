Imports System.Xml.Serialization
Imports SMRUCC.genomics.NCBI.Extensions.LocalBLAST.Application.BBH
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.DocumentFormat.Csv
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Serialization
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace LocalBLAST.BLASTOutput.Views

    ''' <summary>
    ''' 方便程序调试的一个对象数据结构
    ''' </summary>
    ''' <remarks></remarks>
    Public Class Overview : Inherits ClassObject

        <XmlElement> Public Property Queries As Query()

        Public Function ExportParalogs() As BestHit()
            Dim bh = Me.ExportAllBestHist() '符合最佳条件，但是不是自身的记录都是旁系同源
            bh = (From besthit As BestHit
                  In bh.AsParallel
                  Where Not String.Equals(besthit.QueryName, besthit.HitName, StringComparison.OrdinalIgnoreCase)
                  Select besthit).ToArray
            Return bh
        End Function

        Public Function GetExcelData() As BestHit()
            Dim LQuery As BestHit() = (From query As Query In Queries Select query.Hits).MatrixToVector
            Return LQuery
        End Function

        Public Shared Function LoadExcel(path As String) As Overview
            Dim Excel = path.LoadCsv(Of BestHit)(False)
            Dim LQuery = (From besthit As Application.BBH.BestHit
                          In Excel
                          Select besthit
                          Group By besthit.QueryName Into Group).ToArray
            Dim lstQuery As Query() = (From queryEntry In LQuery
                                       Let queryData As Query = New Query With {
                                           .Id = queryEntry.QueryName,
                                           .Hits = queryEntry.Group.ToArray
                                       }
                                       Select queryData).ToArray
            Dim Overview As Overview = New Overview With {
                .Queries = lstQuery
            }
            Return Overview
        End Function

        Public Function ExportAllBestHist(Optional identities As Double = 0.15) As BestHit()
            Dim LQuery = (From besthit As BestHit
                          In GetExcelData.AsParallel
                          Where besthit.IsMatchedBesthit(identities)
                          Select besthit).ToArray
            Return LQuery
        End Function

        Public Function ExportBestHit(Optional identities As Double = 0.15) As BestHit()
            Dim LQuery = (From queryEntry As Query
                          In Queries.AsParallel
                          Let besthit As BestHit = (From hit As BestHit
                                                    In queryEntry.Hits
                                                    Where hit.IsMatchedBesthit(identities)
                                                    Select hit).FirstOrDefault
                          Select If(besthit Is Nothing,
                              New BestHit With {
                                    .QueryName = queryEntry.Id,
                                    .HitName = IBlastOutput.HITS_NOT_FOUND},
                              besthit)).ToArray
            Return LQuery
        End Function
    End Class

    Public Structure Query : Implements sIdEnumerable
        <XmlAttribute> Public Property Id As String Implements sIdEnumerable.Identifier
        <XmlElement> Public Property Hits As BestHit()

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Structure
End Namespace
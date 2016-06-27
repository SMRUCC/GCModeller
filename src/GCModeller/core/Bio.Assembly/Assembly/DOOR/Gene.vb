Imports LANS.SystemsBiology.ComponentModel
Imports LANS.SystemsBiology.ComponentModel.Loci
Imports LANS.SystemsBiology.ComponentModel.Loci.Abstract
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic

Namespace Assembly.DOOR

    ''' <summary>
    ''' Door操纵子之中的一个基因对象的数据
    ''' </summary>
    ''' <remarks></remarks>
    Public Class GeneBrief : Implements sIdEnumerable
        Implements IGeneBrief

        Public Property OperonID As String Implements sIdEnumerable.Identifier
        Public Property GI As String
        Public Property Synonym As String
        Public Property Length As Integer Implements ICOGDigest.Length
        Public Property COG_number As String Implements ICOGDigest.COG
        Public Property Product As String Implements ICOGDigest.Product
        Public Property Location As NucleotideLocation Implements IContig.Location

        Public Overrides Function ToString() As String
            Return String.Format("[{0}]{1}: {2}", COG_number, Synonym, Product)
        End Function

        Sub New()
        End Sub

        Sub New(g As NCBI.GenBank.TabularFormat.ComponentModels.GeneBrief)
            GI = g.PID
            Synonym = g.Synonym
            Length = g.Location.FragmentSize
            COG_number = g.COG
            Product = g.Product
            Location = g.Location
        End Sub

        Public Shared Function TryParse(strLine As String) As GeneBrief
            Dim Tokens As String() = Strings.Split(strLine, vbTab)
            Dim Gene As Assembly.DOOR.GeneBrief = New GeneBrief
            Dim p As Integer
            Gene.OperonID = Tokens(p.MoveNext)
            Gene.GI = Tokens(p.MoveNext)
            Gene.Synonym = Tokens(p.MoveNext)
            Gene.Location =
                New ComponentModel.Loci.NucleotideLocation With {
                    .Left = Tokens(p.MoveNext),
                    .Right = Tokens(p.MoveNext),
                    .Strand = GetStrand(Tokens(p.MoveNext))
            }
            Gene.Length = Tokens(p.MoveNext)
            Gene.COG_number = Tokens(p.MoveNext)
            Gene.Product = Tokens(p.MoveNext)

            Return Gene
        End Function
    End Class
End Namespace
Imports System.Runtime.CompilerServices
Imports RDotNET.Extensions.VisualBasic.API

Namespace Custom

    Public Module MyTools

        ''' <summary>
        ''' ```R
        ''' singleColumndf2Vector &lt;- function(df) {
        '''
        '''     df = as.vector(df)
        '''	    df = as.vector(df[,1])
        '''
        '''     return (df)
        ''' }
        ''' ```
        ''' </summary>
        ''' <param name="df$">data frame type in R language</param>
        ''' <returns></returns>
        Public Function SingleColumn2Vector(df$) As String
            df = [as].vector(df)
            df = [as].vector(df)

            Return df
        End Function

        <Extension> Public Function RemovesRlistNULL(list As var) As String
            Return list.Name.RemovesRlistNULL
        End Function

        ''' <summary>
        ''' 从R的list()对象之中删除NA名称以及NULL内容的slot元素
        ''' </summary>
        ''' <param name="list$"></param>
        ''' <returns></returns>
        <Extension> Public Function RemovesRlistNULL(list$) As String
            ' > head(raw.pos)
            ' $<NA>
            ' NULL
            '
            ' $<NA>
            ' NULL
            '
            ' $<NA>
            ' NULL
            '
            ' $<NA>
            ' NULL
            Dim newList$ = base.list()

            SyncLock R
                With R

                    .call = $"for(i in 1:length({list})) {{

                                  name <- names({list}[i]);

                                  if (!is.na(name)) {{
                                      {newList}[[name]] <- {list}[[name]]
                                  }}

                              }}"

                End With
            End SyncLock

            Return newList
        End Function
    End Module
End Namespace
